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
using System.Net;

using NUnit.Framework;

namespace Scratch.FileFtp
{
    /// <summary>
    /// http://stackoverflow.com/questions/3622718/multiple-files-downloading-using-ftp-crash-only-on-runtimenot-debuging
    /// </summary>
    [TestFixture]
    public class FtpDemo
    {
        [Test]
        public void Show()
        {
            string host = "63.245.208.138"; // "ftp.mozilla.org";
            string user = "anonymous";
            string pass = "your@email.com";
            string remote = "pub/mozilla.org/firefox/nightly/latest-trunk/firefox-4.0b8pre.en-US.win32.txt";
            string local = "d:\\temp\\";
            downloadFile(remote, local, host, user, pass);

            remote = "pub/mozilla.org/firefox/nightly/latest-trunk/firefox-4.0b8pre.en-US.linux-i686.txt";
            downloadFile(remote, local, host, user, pass);
        }

        private static void downloadFile(string sourceFile, string targetFolder, string ftpServerIP, string ftpUserID, string ftpPassword)
        {
            if (!Directory.Exists(targetFolder))
            {
                Directory.CreateDirectory(targetFolder);
            }
            string remoteFile = sourceFile.Replace("\\", "//");
            string localFolder = Path.Combine(targetFolder, sourceFile.Substring(sourceFile.LastIndexOf("\\") + 1));
            string filename = "ftp://" + ftpServerIP + "//" + remoteFile;

            var ftpReq = (FtpWebRequest)WebRequest.Create(filename);
            ftpReq.Method = WebRequestMethods.Ftp.DownloadFile;
            ftpReq.Credentials = new NetworkCredential(ftpUserID, ftpPassword);
            ftpReq.UseBinary = true;
            ftpReq.Proxy = null;
            ftpReq.KeepAlive = false; //'3. Settings and action
            try
            {
                using (var response = (FtpWebResponse)(ftpReq.GetResponse()))
                {
                    using (var responseStream = response.GetResponseStream())
                    {
                        using (var fs = new FileStream(localFolder, FileMode.Create))
                        {
                            var buffer = new byte[2047];
                            int read;
                            while ((read = responseStream.Read(buffer, 0, buffer.Length)) != 0)
                            {
                                fs.Write(buffer, 0, read);
                            }
                            responseStream.Close();
                            fs.Flush();
                            fs.Close();
                        }
                        responseStream.Close();
                    }
                    response.Close();
                }
            }
            catch (WebException ex)
            {
                var response = (FtpWebResponse)ex.Response;
                Console.Out.WriteLine(response.StatusDescription);
            }
        }
    }
}