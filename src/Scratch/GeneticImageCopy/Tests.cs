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
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

using NUnit.Framework;

using Scratch.ConvertHexStringToBytes;
using Scratch.GeneticAlgorithm;

namespace Scratch.GeneticImageCopy
{
    [TestFixture]
    public class Tests
    {
        private static decimal _previousPercentage;

        [Test, Explicit]
        public void Given_Monalisa_190x200()
        {
            const string fileNameWithPath = "../../GeneticImageCopy/monalisa.jpg";
            GeneticallyDuplicateWithTriangles(fileNameWithPath);
        }

        private static void Display(int generation, uint fitness, string genes, int shapeSizeInBytes, int width, int height, string howCreated, int max)
        {
            decimal percentage = Math.Round(((max - fitness * 1m) / max) * 100m, 2);
            if (percentage != _previousPercentage)
            {
                _previousPercentage = percentage;
                Console.WriteLine("Generation " + generation + " fitness " + fitness + " by " + howCreated + " = " + percentage + "% match");
                var bitmap = GenesToBitmap(genes, shapeSizeInBytes, width, height);
                string filename = "image_" + generation + ".jpg";
                bitmap.Save(filename);
            }
        }

        private static Bitmap GenesToBitmap(string genes, int shapeSizeInBytes, int bitmapWidth, int bitmapHeight)
        {
            if ((genes.Length & 1) == 1)
            {
                genes += "0";
            }
            var shapes = new List<Triangle>();
            for (int i = 0; i < genes.Length; i += shapeSizeInBytes)
            {
                var bytes = genes.HexToBytes(i, i + shapeSizeInBytes);
                var shape = new Triangle(bytes, bitmapWidth, bitmapHeight);
                shapes.Add(shape);
            }

            var temp = new Bitmap(bitmapWidth, bitmapHeight, PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(temp))
            {
                foreach (var shape in shapes)
                {
                    shape.Draw(graphics);
                }
            }
            return temp;
        }

        private static void GeneticallyDuplicateWithTriangles(string fileNameWithPath)
        {
            _previousPercentage = 100;
            byte[] bitmapBytes;
            int width;
            int height;
            using (var bitmap = new Bitmap(fileNameWithPath))
            {
                width = bitmap.Width;
                height = bitmap.Height;
                using (var stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Bmp);
                    bitmapBytes = stream.ToArray();
                }
            }

            Console.WriteLine(width + "x" + height);
            int pointSizeInBytes = (int)Math.Ceiling(Math.Log(width * height, 2) / 8.0) * 2;
            const int colorSize = 4 * 2;
            int shapeSizeInBytes = 3 * pointSizeInBytes + colorSize; // triangle
            //            int shapeSizeInBytes = 2 * pointSizeInBytes + colorSize; // line
            Func<string, uint> calcFitness = x =>
                {
                    using (var temp = GenesToBitmap(x, shapeSizeInBytes, width, height))
                    {
                        byte[] tempBytes;
                        using (var stream = new MemoryStream())
                        {
                            temp.Save(stream, ImageFormat.Bmp);
                            tempBytes = stream.ToArray();
                        }

                        uint fitness = (uint)Enumerable.Range(0, tempBytes.Length)
                                                 .Sum(i => Math.Abs(tempBytes[i] - bitmapBytes[i]));

                        return fitness;
                    }
                };

            int max = bitmapBytes.Length * 255;
            var solver = new GeneticSolver(2000)
                {
                    UseFastSearch = true,
                    DisplayHowCreatedPercentages = true,
                    DisplayGenes = (generation, fitness, genes, howCreated) => Display(generation, fitness, genes, shapeSizeInBytes, width, height, howCreated, max)
                };
            solver.GetBestGenetically(50 * shapeSizeInBytes, "0123456789ABCDEF", calcFitness, true);
        }

        public static void Main()
        {
            new Tests().Given_Monalisa_190x200();
        }
    }

    public interface IShape
    {
        void Draw(Graphics graphics);
    }

    public class Triangle : IShape
    {
        private const int NumberOfPoints = 3;

        public Triangle(IList<byte> bytes, int bitmapWidth, int bitmapHeight)
        {
            var points = new List<Point>();

            int pointSize = (bytes.Count - 4) / NumberOfPoints;

            for (int i = 0; i < NumberOfPoints; i++)
            {
                int bitmapOffset = 0;
                for (int j = 0; j < pointSize; j++)
                {
                    int byteOffset = i * pointSize + j;
                    bitmapOffset <<= 8;
                    bitmapOffset |= bytes[byteOffset];
                }
                bitmapOffset = Math.Abs(bitmapOffset) % (bitmapWidth * bitmapHeight);
                int x = bitmapOffset / bitmapWidth;
                int y = bitmapOffset % bitmapWidth;
                points.Add(new Point(x, y));
            }

            Points = points.ToArray();
            int colorOffset = NumberOfPoints * pointSize;
            Color = Color.FromArgb(bytes[colorOffset], bytes[colorOffset + 1], bytes[colorOffset + 2], bytes[colorOffset + 3]);
        }

        public Color Color { get; private set; }
        public Point[] Points { get; private set; }

        public void Draw(Graphics graphics)
        {
            graphics.FillPolygon(new SolidBrush(Color), Points);
        }
    }

    public class Line : IShape
    {
        private const int NumberOfPoints = 2;

        public Line(IList<byte> bytes, int bitmapWidth, int bitmapHeight)
        {
            var points = new List<Point>();

            int pointSize = (bytes.Count - 4) / NumberOfPoints;

            for (int i = 0; i < NumberOfPoints; i++)
            {
                int bitmapOffset = 0;
                for (int j = 0; j < pointSize; j++)
                {
                    int byteOffset = i * pointSize + j;
                    bitmapOffset <<= 8;
                    bitmapOffset |= bytes[byteOffset];
                }
                bitmapOffset = Math.Abs(bitmapOffset) % (bitmapWidth * bitmapHeight);
                int x = bitmapOffset / bitmapWidth;
                int y = bitmapOffset % bitmapWidth;
                points.Add(new Point(x, y));
            }

            Points = points.ToArray();
            int colorOffset = NumberOfPoints * pointSize;
            Color = Color.FromArgb(bytes[colorOffset], bytes[colorOffset + 1], bytes[colorOffset + 2], bytes[colorOffset + 3]);
        }

        public Color Color { get; private set; }
        public Point[] Points { get; private set; }

        public void Draw(Graphics graphics)
        {
            graphics.DrawLine(new Pen(Color), Points[0], Points[1]);
        }
    }

    public class Ellipse : IShape
    {
        private const int NumberOfPoints = 2;

        public Ellipse(IList<byte> bytes, int bitmapWidth, int bitmapHeight)
        {
            var points = new List<Point>();

            int pointSize = (bytes.Count - 4) / NumberOfPoints;

            for (int i = 0; i < NumberOfPoints; i++)
            {
                int bitmapOffset = 0;
                for (int j = 0; j < pointSize; j++)
                {
                    int byteOffset = i * pointSize + j;
                    bitmapOffset <<= 8;
                    bitmapOffset |= bytes[byteOffset];
                }
                bitmapOffset = Math.Abs(bitmapOffset) % (bitmapWidth * bitmapHeight);
                int x = bitmapOffset / bitmapWidth;
                int y = bitmapOffset % bitmapWidth;
                points.Add(new Point(x, y));
            }

            Points = points.ToArray();
            int colorOffset = NumberOfPoints * pointSize;
            Color = Color.FromArgb(bytes[colorOffset], bytes[colorOffset + 1], bytes[colorOffset + 2], bytes[colorOffset + 3]);
        }

        public Color Color { get; private set; }
        public Point[] Points { get; private set; }

        public void Draw(Graphics graphics)
        {
            graphics.FillEllipse(new SolidBrush(Color), Points[0].X, Points[0].Y, Points[1].X, Points[1].Y);
        }
    }
}