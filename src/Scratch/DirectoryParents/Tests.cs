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
using System.IO;
using System.Linq;

using FluentAssert;

using NUnit.Framework;

namespace Scratch.DirectoryParents
{
    [TestFixture]
    public class Tests
    {
        /// <summary>
        /// http://stackoverflow.com/questions/5291022/get-all-parents-of-a-path-with-linq
        /// </summary>
        [Test]
        public void Should_get_all_parents_of_a_path()
        {
            const string path = @"c:\a\b\c";
            var list = path
                .Generate(Path.GetDirectoryName)
                .Skip(1) // the original
                .TakeWhile(p => p != Path.GetPathRoot(p))
                .ToList();
            list.Count.ShouldBeEqualTo(2);
            list.ShouldContainAll(new[]
                {
                    @"c:\a",
                    @"c:\a\b"
                });
        }

        /// <summary>
        /// http://codereview.stackexchange.com/questions/1225/nested-anonymous-methods/1243#1243
        /// </summary>
        [Test]
        public void Should_only_keep_parent_directories()
        {
            var directories = new List<string>
                {
                    null,
                    "",
                    @"c:\bob",
                    @"c:\bob\mike\nick",
                    @"C:\a\c",
                    @"c:\stuf\morestuff",
                    @"c:\stuf\morestuff\sub1",
                    @"c:\otherstuf",
                    @"c:\otherstuf\sub1",
                    @"c:\otherstuf\sub1a",
                    @"c:\otherstuf\sub2"
                };

            CleanUpFolders(directories);
            directories.Count.ShouldBeEqualTo(4);
            directories.ShouldContainAll(new[]
                {
                    @"c:\bob",
                    @"c:\stuf\morestuff",
                    @"c:\otherstuf",
                    @"C:\a\c"
                });
        }

        private static void CleanUpFolders(List<string> uniqueFolders)
        {
            var folderLookup = new HashSet<string>(uniqueFolders);
            uniqueFolders.RemoveAll(x => String.IsNullOrEmpty(x) ||
                                         x.Generate(Path.GetDirectoryName)
                                             .Skip(1) // the original
                                             .TakeWhile(p => p != Path.GetPathRoot(p))
                                             .Any(folderLookup.Contains));
        }

        private static void CleanUpFoldersOrig(List<string> uniqueFolders)
        {
            uniqueFolders.RemoveAll(
                delegate(string curFolder)
                    {
                        // remove empty
                        if (string.IsNullOrEmpty(curFolder))
                        {
                            return true;
                        }

                        // remove sub paths
                        if (uniqueFolders.Exists(
                            delegate(string s)
                                {
                                    if (!string.IsNullOrEmpty(s) &&
                                        curFolder.StartsWith(s) &&
                                        string.Compare(s, curFolder) != 0)
                                    {
                                        return true;
                                    }
                                    return false;
                                }))
                        {
                            return true;
                        }

                        return false;
                    }
                );
        }
    }
}