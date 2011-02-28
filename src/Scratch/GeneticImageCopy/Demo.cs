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

using NUnit.Framework;

using Scratch.ConvertHexStringToBytes;
using Scratch.GeneticAlgorithm;

namespace Scratch.GeneticImageCopy
{
    [TestFixture]
    public class Demo
    {
        private static decimal _previousPercentage;
        private static int? _previousGeneration;
        private static decimal _minGenerationGapBetweenWrites;

        [Test]
        [Explicit]
        public void Draw_with_circles()
        {
            const string fileNameWithPath = "../../GeneticImageCopy/monalisa.jpg";
            DeletePreviousImages();
            GeneticallyDuplicateWithShape<Circle>(fileNameWithPath, 150, true);
        }

        [Test]
        public void Time_CalcFitness_Linq()
        {
            TimeCalcFitness(CalcFitnessWithLinq);
        }

        [Test]
        public void Time_CalcFitness_Unsafe()
        {
            TimeCalcFitness(CalcFitness);
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

        private static uint CalcFitnessWithLinq(byte[] tempBytes, byte[] bitmapBytes)
        {
            uint fitness = (uint)Enumerable.Range(0, tempBytes.Length)
                                     .Sum(i => Math.Abs(tempBytes[i] - bitmapBytes[i]));

            return fitness;
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
            if (percentage == _previousPercentage || 
                _previousGeneration != null && generation-_previousGeneration.Value < _minGenerationGapBetweenWrites)
            {
                filename = "final.jpg";
            }
            else
            {
                _previousPercentage = percentage;
                _previousGeneration = generation;
                Console.WriteLine("Generation " + generation + " fitness " + fitness + " by " + howCreated + " = " + percentage + "% match");
                File.Delete("final.jpg");
                _minGenerationGapBetweenWrites = (int)(_minGenerationGapBetweenWrites * 1.01m);
            }
            var shapeCount = genes.Length / shapeSizeInBytes;
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

//                        var srcRect = new Rectangle(0, 0, width, height);
//                        graphics.DrawImage(generatedBitmap, 0, 0, srcRect, GraphicsUnit.Pixel);
//                        graphics.DrawImage(targetImage, width, 0, srcRect, GraphicsUnit.Pixel);

                        graphics.FillRectangle(Brushes.White, 0, height, 2 * width, 20);
                        var elapsed = timer.Elapsed.ToString();
                        graphics.DrawString(
                            "Generation " + generation.ToString().PadRight(10) 
                            + percentage.ToString().PadLeft(5) 
                            + "%    elapsed: " + elapsed.Substring(0, elapsed.LastIndexOf('.'))
                            + shapeCount.ToString().PadLeft(5)+" "+(typeof(T).Name)+(shapeCount != 1 ? "s" : ""), 
                            new Font("Times New Roman", 12), Brushes.Black, 2, height + 1);
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

        private static void GeneticallyDuplicateWithShape<T>(string fileNameWithPath, int numberOfShapes, bool useHillClimbing)
            where T : IShape
        {
            _previousPercentage = 100;
            _minGenerationGapBetweenWrites = 10;
            _previousGeneration = null;
            var targetImage = new Bitmap(fileNameWithPath);
            int width = targetImage.Width;
            int height = targetImage.Height;
            var bitmapBytes = targetImage.GetBytesFromBitmap();
            var timer = new Stopwatch();
            timer.Start();

            Console.WriteLine(width + "x" + height);
            int shapeSizeInBytes = (int)typeof(T).GetMethod("GetEncodingSizeInBytes", BindingFlags.Static | BindingFlags.Public).Invoke(null, new[] { width, (object)height });
            Func<string, uint> calcFitness = x =>
                {
                    using (var temp = GenesToBitmap<T>(x, shapeSizeInBytes, width, height, targetImage.PixelFormat))
                    {
                        var tempBytes = temp.GetBytesFromBitmap();

                        uint fitness = CalcFitness(tempBytes, bitmapBytes);

                        return fitness;
                    }
                };

            int max = bitmapBytes.Length * 255;
            var solver = new GeneticSolver(2000)
                {
                    UseHillClimbing = useHillClimbing,
                    OnlyPermuteNewGenesWhileHillClimbing = false,
                    NumberOfGenesInUnitOfMeaning = shapeSizeInBytes,
                    UseFastSearch = true,
                    DisplayHowCreatedPercentages = true,
                    DisplayGenes = (generation, fitness, genes, howCreated) => Display<T>(generation, fitness, genes, shapeSizeInBytes, howCreated, max, targetImage, timer)
                };
            solver.GetBestGenetically(numberOfShapes * shapeSizeInBytes, "0123456789ABCDEF", calcFitness);
        }

        public static void Main()
        {
            new Demo().Draw_with_circles();
        }

        private static void TimeCalcFitness(Func<byte[], byte[], uint> calcFitness)
        {
            const string fileNameWithPath = "../../GeneticImageCopy/monalisa.jpg";
            var bitmap = new Bitmap(fileNameWithPath);
            var bytes = bitmap.GetBytesFromBitmap();
            var bytes2 = bytes.Reverse().ToArray();
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int runs = 1000;
            for (int i = 0; i < runs; i++)
            {
                uint fitness = calcFitness(bytes, bytes2);
            }
            stopwatch.Stop();
            Console.WriteLine(runs + " runs, average seconds: " + stopwatch.Elapsed.TotalSeconds / runs);
        }
    }
}