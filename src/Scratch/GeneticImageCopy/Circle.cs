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
            int diameter = Points[1].X;
            if (diameter == 0)
            {
                diameter = BitmapWidth;
            }
            if ((Points[1].Y & 1) == 1)
            {
                diameter += BitmapWidth;
            }
            int centerOfCircleAdjustment = diameter / 2;
            graphics.FillEllipse(new SolidBrush(Color), offsetPoints[0].X - centerOfCircleAdjustment, offsetPoints[0].Y - centerOfCircleAdjustment, diameter, diameter);
        }

        public static int GetEncodingSizeInBytes(int imageWidth, int imageHeight)
        {
            return GetEncodingSizeInBytes(imageWidth, imageHeight, NumberOfPoints);
        }
    }
}