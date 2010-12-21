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
using System.IO;
using System.Text;
using System.Xml;

namespace Scratch.ConvertHexStringToBytes
{
    /// <summary>
    /// http://stackoverflow.com/questions/854012/how-to-convert-hex-to-a-byte-array/854539#854539
    /// </summary>
    public static class StringExtensions
    {
        public static byte[] HexToBytes(this string hexEncodedBytes, int start, int end)
        {
            int length = end - start;
            const string tagName = "hex";
            string fakeXmlDocument = String.Format("<{1}>{0}</{1}>",
                                                   hexEncodedBytes.Substring(start, length),
                                                   tagName);
            var stream = new MemoryStream(Encoding.ASCII.GetBytes(fakeXmlDocument));
            var reader = XmlReader.Create(stream, new XmlReaderSettings());
            int hexLength = length / 2;
            var result = new byte[hexLength];
            reader.ReadStartElement(tagName);
            reader.ReadContentAsBinHex(result, 0, hexLength);
            return result;
        }
    }
}