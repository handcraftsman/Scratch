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

namespace Scratch.GeneticImageCopy
{
    public abstract class Shape
    {
        protected Shape(IList<byte> bytes, int bitmapWidth, int bitmapHeight, int numberOfPoints)
        {
            BitmapWidth = bitmapWidth;
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

        protected int BitmapWidth { get; private set; }

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
}