//  * **********************************************************************************
//  * Copyright (c) Clinton Sheppard
//  * This source code is subject to terms and conditions of the MIT License.
//  * A copy of the license can be found in the License.txt file
//  * at the root of this distribution. 
//  * By using this source code in any fashion, you are agreeing to be bound by 
//  * the terms of the MIT License.
//  * You must not remove this notice from this software.
//  * **********************************************************************************

using FluentAssert;

using NUnit.Framework;

namespace Scratch.ConvertHexStringToBytes
{
    /// <summary>
    /// http://stackoverflow.com/questions/854012/how-to-convert-hex-to-a-byte-array/854539#854539
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Convert_hex_string_to_bytes()
        {
            const string input = "0xBAC893CAB8B7FE03C927417A2A3F6A60BD30FF35E250011CB255";
            var bytes = input.HexToBytes(2, input.Length);
            bytes[0].ShouldBeEqualTo((byte)0xba, "0");
            bytes[1].ShouldBeEqualTo((byte)0xc8, "1");
            bytes[2].ShouldBeEqualTo((byte)0x93, "2");
            bytes[3].ShouldBeEqualTo((byte)0xca, "3");
            bytes[4].ShouldBeEqualTo((byte)0xb8, "4");
            bytes[5].ShouldBeEqualTo((byte)0xb7, "5");
            bytes[6].ShouldBeEqualTo((byte)0xfe, "6");
            bytes[7].ShouldBeEqualTo((byte)0x03, "7");
            bytes[8].ShouldBeEqualTo((byte)0xc9, "8");
            bytes[9].ShouldBeEqualTo((byte)0x27, "9");
            bytes[10].ShouldBeEqualTo((byte)0x41, "10");
            bytes[11].ShouldBeEqualTo((byte)0x7a, "11");
            bytes[12].ShouldBeEqualTo((byte)0x2a, "12");
            bytes[13].ShouldBeEqualTo((byte)0x3f, "13");
            bytes[14].ShouldBeEqualTo((byte)0x6a, "14");
            bytes[15].ShouldBeEqualTo((byte)0x60, "15");
            bytes[16].ShouldBeEqualTo((byte)0xbd, "16");
            bytes[17].ShouldBeEqualTo((byte)0x30, "17");
            bytes[18].ShouldBeEqualTo((byte)0xff, "18");
            bytes[19].ShouldBeEqualTo((byte)0x35, "19");
            bytes[20].ShouldBeEqualTo((byte)0xe2, "20");
            bytes[21].ShouldBeEqualTo((byte)0x50, "21");
            bytes[22].ShouldBeEqualTo((byte)0x01, "22");
            bytes[23].ShouldBeEqualTo((byte)0x1c, "23");
            bytes[24].ShouldBeEqualTo((byte)0xb2, "24");
            bytes[25].ShouldBeEqualTo((byte)0x55, "25");
        }
    }
}