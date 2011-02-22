//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;

using NUnit.Framework;

using Scratch.ConvertHexStringToBytes;
using Scratch.GeneticAlgorithm;

namespace Scratch.GeneticImageCopy
{
    [TestFixture]
    public class Demo
    {
        private static decimal _previousPercentage;

        [Test]
        [Explicit]
        public void Draw_with_circles()
        {
            const string fileNameWithPath = "../../GeneticImageCopy/monalisa.jpg";
            DeletePreviousImages();
            GeneticallyDuplicateWithShape<Circle>(fileNameWithPath, 300);
        }

        private static uint CalcFitness(byte[] tempBytes, byte[] bitmapBytes)
        {
            int len = tempBytes.Length;
            int sum = 0;
            unsafe
            {
                fixed (byte* tempPtr = tempBytes, bitmapPtr = bitmapBytes)
                {
                    byte* ap2 = tempPtr, bp2 = bitmapPtr;
                    for (; len > 0; len--)
                    {
                        if (*ap2 > *bp2)
                        {
                            sum += (*ap2 - *bp2);
                        }
                        else
                        {
                            sum += (*bp2 - *ap2);
                        }

                        ap2++;
                        bp2++;
                    }
                }
            }

            return (uint)sum;
        }

        private static void DeletePreviousImages()
        {
            foreach (string file in Directory.GetFiles(".", "image_*.jpg"))
            {
                File.Delete(file);
            }
        }

        private static void Display<T>(int generation, uint fitness, string genes, int shapeSizeInBytes, string howCreated, int max, Bitmap targetImage, Stopwatch timer) where T : IShape
        {
            int width = targetImage.Width;
            int height = targetImage.Height;

            decimal percentage = Math.Round(((max - fitness * 1m) / max) * 100m, 2);
            string filename = "image_" + generation + ".jpg";
            if (percentage == _previousPercentage)
            {
                filename = "final.jpg";
            }
            else
            {
                _previousPercentage = percentage;
                Console.WriteLine("Generation " + generation + " fitness " + fitness + " by " + howCreated + " = " + percentage + "% match");
                File.Delete("final.jpg");
            }
            using (var generatedBitmap = GenesToBitmap<T>(genes, shapeSizeInBytes, width, height, targetImage.PixelFormat))
            {
                using (var combined = new Bitmap(2 * width, 20 + height))
                {
                    using (var graphics = Graphics.FromImage(combined))
                    {
                        for (int i = 0; i < width; i++)
                        {
                            for (int j = 0; j < height; j++)
                            {
                                combined.SetPixel(i, j, generatedBitmap.GetPixel(i, j));
                                combined.SetPixel(width + i, j, targetImage.GetPixel(i, j));
                            }
                        }

                        graphics.FillRectangle(Brushes.White, 0, height, 2 * width, 20);
                        graphics.DrawString("Generation " + generation.ToString().PadRight(10) + percentage.ToString().PadLeft(5) + "%    elapsed: " + timer.Elapsed, new Font("Times New Roman", 12), Brushes.Black, 2, height + 1);
                    }

                    combined.Save(filename);
                }
            }
        }

        private static Bitmap GenesToBitmap<T>(string genes, int shapeSizeInBytes, int bitmapWidth, int bitmapHeight, PixelFormat pixelFormat)
            where T : IShape
        {
            if ((genes.Length & 1) == 1)
            {
                genes += "0";
            }
            var shapes = new List<T>();
            for (int i = 0; i < genes.Length; i += shapeSizeInBytes)
            {
                var bytes = genes.HexToBytes(i, i + shapeSizeInBytes);
                var shape = (T)typeof(T).GetConstructor(new[] { bytes.GetType(), bitmapWidth.GetType(), bitmapHeight.GetType() }).Invoke(new[] { bytes, bitmapWidth, (object)bitmapHeight });
                shapes.Add(shape);
            }

            var temp = new Bitmap(bitmapWidth, bitmapHeight, pixelFormat);
            using (var graphics = Graphics.FromImage(temp))
            {
                foreach (var shape in shapes)
                {
                    shape.Draw(graphics, 0, 0);
                }
            }

            return temp;
        }

        private static void GeneticallyDuplicateWithShape<T>(string fileNameWithPath, int numberOfShapes)
            where T : IShape
        {
            _previousPercentage = 100;
            var targetImage = new Bitmap(fileNameWithPath);
            int width = targetImage.Width;
            int height = targetImage.Height;
            var bitmapBytes = GetBytesFromBitmap(targetImage);
            var timer = new Stopwatch();
            timer.Start();

            Console.WriteLine(width + "x" + height);
            int shapeSizeInBytes = (int)typeof(T).GetMethod("GetEncodingSizeInBytes", BindingFlags.Static | BindingFlags.Public).Invoke(null, new[] { width, (object)height });
            Func<string, uint> calcFitness = x =>
                {
                    using (var temp = GenesToBitmap<T>(x, shapeSizeInBytes, width, height, targetImage.PixelFormat))
                    {
                        var tempBytes = GetBytesFromBitmap(temp);

                        uint fitness = CalcFitness(tempBytes, bitmapBytes);

                        return fitness;
                    }
                };

            int max = bitmapBytes.Length * 255;
            var solver = new GeneticSolver(2000)
                {
                    NumberOfGenesInUnitOfMeaning = shapeSizeInBytes,
                    UseFastSearch = true,
                    DisplayHowCreatedPercentages = true,
                    DisplayGenes = (generation, fitness, genes, howCreated) => Display<T>(generation, fitness, genes, shapeSizeInBytes, howCreated, max, targetImage, timer)
                };
            solver.GetBestGenetically(numberOfShapes * shapeSizeInBytes, "0123456789ABCDEF", calcFitness, true);
        }

        private static byte[] GetBytesFromBitmap(Bitmap bitmap)
        {
            var rectangle = new Rectangle(0, 0, bitmap.Width, bitmap.Height);
            var bData = bitmap.LockBits(rectangle, ImageLockMode.ReadWrite, bitmap.PixelFormat);
            byte[] data;
            try
            {
                int size = bData.Stride * bData.Height;
                data = new byte[size];
                Marshal.Copy(bData.Scan0, data, 0, size);
            }
            finally
            {
                bitmap.UnlockBits(bData);
            }
            return data;
        }

        public static void Main()
        {
            new Demo().Draw_with_circles();
        }
    }

    public interface IShape
    {
        void Draw(Graphics graphics, int offsetX, int offsetY);
    }

    public abstract class Shape
    {
        protected Shape(IList<byte> bytes, int bitmapWidth, int bitmapHeight, int numberOfPoints)
        {
            var points = new List<Point>();

            int pointSize = (bytes.Count - 4) / numberOfPoints;

            for (int i = 0; i < numberOfPoints; i++)
            {
                int bitmapOffset = 0;
                for (int j = 0; j < pointSize; j++)
                {
                    int byteOffset = i * pointSize + j;
                    bitmapOffset <<= 8;
                    bitmapOffset |= bytes[byteOffset];
                }
                bitmapOffset = Math.Abs(bitmapOffset) % (bitmapWidth * bitmapHeight);
                int y = bitmapOffset / bitmapWidth;
                int x = bitmapOffset % bitmapWidth;
                points.Add(new Point(x, y));
            }

            Points = points.ToArray();
            int colorOffset = numberOfPoints * pointSize;
            Color = Color.FromArgb(bytes[colorOffset], bytes[colorOffset + 1], bytes[colorOffset + 2], bytes[colorOffset + 3]);
        }

        public Color Color { get; private set; }
        public Point[] Points { get; private set; }

        protected static int GetEncodingSizeInBytes(int imageWidth, int imageHeight, int numberOfPoints)
        {
            int pointSizeInBytes = (int)Math.Ceiling(Math.Log(imageWidth * imageHeight, 2) / 8.0) * 2;
            const int colorSize = 4 * 2;
            int shapeSizeInBytes = numberOfPoints * pointSizeInBytes + colorSize;
            return shapeSizeInBytes;
        }
    }

    public class Triangle : Shape, IShape
    {
        private const int NumberOfPoints = 3;

        public Triangle(IList<byte> bytes, int bitmapWidth, int bitmapHeight)
            : base(bytes, bitmapWidth, bitmapHeight, NumberOfPoints)
        {
        }

        public void Draw(Graphics graphics, int offsetX, int offsetY)
        {
            int avgX = (int)Points.Average(x => x.X);
            int avgY = (int)Points.Average(x => x.Y);
            var offsetPoints = Points.Select(x => new Point(x.X + offsetX - avgX, x.Y + offsetY - avgY)).ToArray();
            graphics.FillPolygon(new SolidBrush(Color), offsetPoints);
        }

        public static int GetEncodingSizeInBytes(int imageWidth, int imageHeight)
        {
            return GetEncodingSizeInBytes(imageWidth, imageHeight, NumberOfPoints);
        }
    }

    public class Line : Shape, IShape
    {
        private const int NumberOfPoints = 2;

        public Line(IList<byte> bytes, int bitmapWidth, int bitmapHeight)
            : base(bytes, bitmapWidth, bitmapHeight, NumberOfPoints)
        {
        }

        public void Draw(Graphics graphics, int offsetX, int offsetY)
        {
            var offsetPoints = Points.Select(x => new Point(x.X + offsetX, x.Y + offsetY)).ToArray();
            graphics.DrawLine(new Pen(Color), offsetPoints[0], offsetPoints[1]);
        }

        public static int GetEncodingSizeInBytes(int imageWidth, int imageHeight)
        {
            return GetEncodingSizeInBytes(imageWidth, imageHeight, NumberOfPoints);
        }
    }

    public class Ellipse : Shape, IShape
    {
        private const int NumberOfPoints = 2;

        public Ellipse(IList<byte> bytes, int bitmapWidth, int bitmapHeight)
            : base(bytes, bitmapWidth, bitmapHeight, NumberOfPoints)
        {
        }

        public void Draw(Graphics graphics, int offsetX, int offsetY)
        {
            var offsetPoints = Points.Select(x => new Point(x.X + offsetX, x.Y + offsetY)).ToArray();
            graphics.FillEllipse(new SolidBrush(Color), offsetPoints[0].X - Points[1].X / 2, offsetPoints[0].Y - Points[1].Y / 2, Points[1].X, Points[1].Y);
        }

        public static int GetEncodingSizeInBytes(int imageWidth, int imageHeight)
        {
            return GetEncodingSizeInBytes(imageWidth, imageHeight, NumberOfPoints);
        }
    }

    public class Circle : Shape, IShape
    {
        private const int NumberOfPoints = 2;

        public Circle(IList<byte> bytes, int bitmapWidth, int bitmapHeight)
            : base(bytes, bitmapWidth, bitmapHeight, NumberOfPoints)
        {
        }

        public void Draw(Graphics graphics, int offsetX, int offsetY)
        {
            var offsetPoints = Points.Select(x => new Point(x.X + offsetX, x.Y + offsetY)).ToArray();
            int centerOfCircleAdjustment = Points[1].X / 2;
            graphics.FillEllipse(new SolidBrush(Color), offsetPoints[0].X - centerOfCircleAdjustment, offsetPoints[0].Y - centerOfCircleAdjustment, Points[1].X, Points[1].X);
        }

        public static int GetEncodingSizeInBytes(int imageWidth, int imageHeight)
        {
            return GetEncodingSizeInBytes(imageWidth, imageHeight, NumberOfPoints);
        }
    }
}