//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace Scratch.GeneticImageCopy
{
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
}