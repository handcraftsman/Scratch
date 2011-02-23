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
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

using NUnit.Framework;

namespace Scratch.GeneticImageCopy
{
    public class BitmapExtensionsTests
    {
        [TestFixture]
        public class When_asked_to_GetBytesFromBitmap
        {
            [Test]
            public void Time_GetBytesFromBitmap_Lock()
            {
                TimeGettingBytes(BitmapExtensions.GetBytesFromBitmap);
            }

            [Test]
            public void Time_GetBytesFromBitmap_MemoryStream()
            {
                TimeGettingBytes(GetBytesFromBitmapWithMemoryStream);
            }

            private static byte[] GetBytesFromBitmapWithMemoryStream(Bitmap temp)
            {
                byte[] tempBytes;
                using (var stream = new MemoryStream())
                {
                    temp.Save(stream, ImageFormat.Bmp);
                    tempBytes = stream.ToArray();
                }
                return tempBytes;
            }

            private static void TimeGettingBytes(Func<Bitmap, byte[]> getBytesFromBitmap)
            {
                const string fileNameWithPath = "../../GeneticImageCopy/monalisa.jpg";
                var bitmap = new Bitmap(fileNameWithPath);
                var stopwatch = new Stopwatch();
                stopwatch.Start();
                int runs = 1000;
                for (int i = 0; i < runs; i++)
                {
                    var bytes = getBytesFromBitmap(bitmap);
                }
                stopwatch.Stop();
                Console.WriteLine(runs + " runs, average seconds: " + stopwatch.Elapsed.TotalSeconds / runs);
            }
        }
    }
}