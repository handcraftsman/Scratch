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
using System.Linq;

using NUnit.Framework;

namespace Scratch.ImageColors
{
    [TestFixture]
    public class Demo
    {
        [Test]
        [Explicit]
        public void Colors_by_frequency()
        {
            const string imagePath = "../../GeneticImageCopy/monalisa.jpg";
            var pixels = GetPixelColors(imagePath);
            var grouped = pixels.GroupBy(x => x);
            var orderedByCount = grouped.OrderByDescending(x => x.Count());
            foreach (var group in orderedByCount)
            {
                var color = group.Key;
                string hexColor = String.Format("{0:X2}{1:X2}{2:X2}{3:X2}",
                                                color.A, color.R, color.G, color.B);
                Console.WriteLine(hexColor + "\t" + group.Count());
            }
        }

        private static IEnumerable<Color> GetPixelColors(string imagePath)
        {
            var pixels = new List<Color>();
            using (var image = new Bitmap(imagePath))
            {
                for (int i = 0; i < image.Width; i++)
                {
                    for (int j = 0; j < image.Height; j++)
                    {
                        pixels.Add(image.GetPixel(i, j));
                    }
                }
            }
            return pixels;
        }
    }
}