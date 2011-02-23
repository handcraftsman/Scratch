//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Scratch.GeneticImageCopy
{
    public static class BitmapExtensions
    {
        public static byte[] GetBytesFromBitmap(this Bitmap bitmap)
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
    }
}