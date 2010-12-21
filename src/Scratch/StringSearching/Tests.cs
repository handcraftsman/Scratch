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

namespace Scratch.StringSearching
{
    /// <summary>
    /// http://stackoverflow.com/questions/3925063/help-fix-my-kmp-search-algorithm/3936998#3936998
    /// </summary>
    [TestFixture]
    public class Tests
    {
        [Test]
        public void Should_get_15_given_text_ABC_ABCDAB_ABCDABCDABDE_and_word_ABCDABD()
        {
            const string text = "ABC ABCDAB ABCDABCDABDE";
            const string word = "ABCDABD";
            int location = KMPSearch.IndexOf(word.ToCharArray(), text.ToCharArray(), 0);
            location.ShouldBeEqualTo(15);
        }

        [Test]
        public void Should_get_computed_table_0_0_0_0_1_2_given_ABCDABD()
        {
            const string input = "ABCDABD";
            var result = KMPSearch.BuildTable(input.ToCharArray());
            result.Length.ShouldBeEqualTo(input.Length);
            result[0].ShouldBeEqualTo(-1);
            result[1].ShouldBeEqualTo(0);
            result[2].ShouldBeEqualTo(0);
            result[3].ShouldBeEqualTo(0);
            result[4].ShouldBeEqualTo(0);
            result[5].ShouldBeEqualTo(1);
            result[6].ShouldBeEqualTo(2);
        }

        [Test]
        public void Should_get_computed_table_0_1_2_3_4_5_given_AAAAAAA()
        {
            const string input = "AAAAAAA";
            var result = KMPSearch.BuildTable(input.ToCharArray());
            result.Length.ShouldBeEqualTo(input.Length);
            result[0].ShouldBeEqualTo(-1);
            result[1].ShouldBeEqualTo(0);
            result[2].ShouldBeEqualTo(1);
            result[3].ShouldBeEqualTo(2);
            result[4].ShouldBeEqualTo(3);
            result[5].ShouldBeEqualTo(4);
            result[6].ShouldBeEqualTo(5);
        }
    }
}