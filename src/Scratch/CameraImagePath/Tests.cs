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

using FluentAssert;

using NUnit.Framework;

namespace Scratch.CameraImagePath
{
    /// <summary>
    /// http://handcraftsman.wordpress.com/2010/12/04/canon-camera-off-by-one-image-storage/
    /// </summary>
    [TestFixture]
    public class Tests
    {
        private volatile int _directoryNumber;
        private volatile int _imageNumber;

        [Test]
        public void Should_get_path__100CANON_IMG_0002__starting_with_directory_number_100_and_image_number_1()
        {
            _directoryNumber = 100;
            _imageNumber = 1;
            string path = GetNextImagePath();
            path.ShouldBeEqualTo(@"100CANON\IMG_0002");
            _directoryNumber.ShouldBeEqualTo(100);
            _imageNumber.ShouldBeEqualTo(2);
        }

        [Test]
        public void Should_get_path__320CANON_IMG_2000__starting_with_directory_number_319_and_image_number_1999()
        {
            _directoryNumber = 319;
            _imageNumber = 1999;
            string path = GetNextImagePath();
            path.ShouldBeEqualTo(@"320CANON\IMG_2000");
            _directoryNumber.ShouldBeEqualTo(320);
            _imageNumber.ShouldBeEqualTo(2000);
        }

        [Test]
        public void Should_increment_the_directory_number()
        {
            _directoryNumber = 100;
            IncrementDirectoryNumber();
            _directoryNumber.ShouldBeEqualTo(101);
        }

        [Test]
        public void Should_increment_the_directory_number_when_the_image_number_goes_to_the_next_hundred()
        {
            _imageNumber = 99;
            _directoryNumber = 100;
            int next = GetNextImageNumber();
            next.ShouldBeEqualTo(100);
            _imageNumber.ShouldBeEqualTo(100);
            _directoryNumber.ShouldBeEqualTo(101);
        }

        [Test]
        public void Should_increment_the_image_number()
        {
            _imageNumber = 0;
            int next = GetNextImageNumber();
            next.ShouldBeEqualTo(1);
            _imageNumber.ShouldBeEqualTo(1);
        }

        [Test]
        public void Should_reset_the_directory_number_to_100_given_999()
        {
            _directoryNumber = 999;
            IncrementDirectoryNumber();
            _directoryNumber.ShouldBeEqualTo(100);
        }

        [Test]
        public void Should_reset_the_image_number_to_1_given_9999()
        {
            _imageNumber = 9999;
            int next = GetNextImageNumber();
            next.ShouldBeEqualTo(1);
            _imageNumber.ShouldBeEqualTo(1);
        }

        private int GetNextImageNumber()
        {
            if (_imageNumber % 100 == 99)
            {
                IncrementDirectoryNumber();
            }
            _imageNumber = _imageNumber + 1;

            if (_imageNumber > 9999)
            {
                _imageNumber = 1;
            }
            return _imageNumber;
        }

        public string GetNextImagePath()
        {
            int nextImageNumber = GetNextImageNumber();
            return String.Format(
                @"{0:000}CANON\IMG_{1:0000}",
                _directoryNumber,
                nextImageNumber);
        }

        private void IncrementDirectoryNumber()
        {
            _directoryNumber = _directoryNumber + 1;
            if (_directoryNumber > 999)
            {
                _directoryNumber = 100;
            }
        }
    }
}