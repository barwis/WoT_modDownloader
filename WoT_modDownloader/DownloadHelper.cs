using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WoT_modDownloader
{
    static class DownloadHelper
    {
        public static void Report(this BackgroundWorker worker, ComplexResponse resp)
        {
            if (worker == null)
                return;
            worker.ReportProgress((int)resp.Percent, resp);
        }

        public static long GetFileSize(string sourceURL)
        {
            var httpReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(sourceURL);
            var httpRes = (System.Net.HttpWebResponse)httpReq.GetResponse();
            long size = httpRes.ContentLength;
            httpRes.Close();
            return size;
        }

        public static bool Download(string destinationPath, string sourceURL, BackgroundWorker bw)
        {
            int bufferSize = 1024 * 1024;
            long existLen = 0;
            
            var response = new ComplexResponse();

            System.IO.FileStream saveFileStream = null;
            
            try
            {
                System.IO.Stream resStream;
                System.Net.HttpWebRequest httpReq;
                System.Net.HttpWebResponse httpRes;
                httpReq = (System.Net.HttpWebRequest)System.Net.HttpWebRequest.Create(sourceURL);
                httpReq.Proxy = null;

                response.FileSize = GetFileSize(sourceURL);

                if (System.IO.File.Exists(destinationPath) ) //check only when it accepts ranges at all
                {
                    System.IO.FileInfo destinationFileInfo = new System.IO.FileInfo(destinationPath);
                    existLen = destinationFileInfo.Length;
                    response.Message = string.Format("File exists and has {0} bytes.", existLen);
                    bw.Report(response);                    
                }

                if (existLen >= response.FileSize)
                    return true;

                httpReq.AddRange(existLen);
                httpRes = (System.Net.HttpWebResponse)httpReq.GetResponse();
                
                var acceptRanges = String.Compare(httpRes.Headers["Accept-Ranges"], "bytes", true) == 0; //check if server accepts ranges



                if (existLen > 0 && acceptRanges) //if retry is available 
                    saveFileStream = new System.IO.FileStream(destinationPath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite);
                else
                    saveFileStream = new System.IO.FileStream(destinationPath,FileMode.Create, FileAccess.Write, FileShare.ReadWrite);


                resStream = httpRes.GetResponseStream();
                response.TotalBytes = httpRes.ContentLength;
                response.Message = string.Format("Remote file has {0} bytes left.", response.TotalBytes);
                bw.Report(response);

                
                if (response.TotalBytes < 0)
                    throw new WebException("File has no data or does not exist");

                int byteSize;
                

               
                byte[] downBuffer = new byte[bufferSize];
                while ((byteSize = resStream.Read(downBuffer, 0, downBuffer.Length)) > 0)
                {
                    if (bw.CancellationPending == true)
                    {
                        response.Message = "Cancelling";
                        bw.Report(response);
                        httpRes.Close();
                        break;
                    }
                    response.CurrentBytes += byteSize;
                    saveFileStream.Write(downBuffer, 0, byteSize);
                    {
                        response.Message = string.Empty;
                        response.Percent = (response.CurrentBytes * 100.0 / response.TotalBytes);
                        if (response.Percent > 100)
                            response.Percent = 100;
                        bw.Report(response);
                
                    }
                }
                saveFileStream.Close();
                response.Message = string.Format("Downloaded {0} bytes.", response.CurrentBytes);
                bw.Report(response);
                return response.CurrentBytes == response.TotalBytes;
            }
            catch (Exception ex)
            {
                response.Message = string.Format("{0} - {1}", ex.GetType(), ex.Message);
                response.IsError = true;
                bw.Report(response); 
                return false;
            }
            finally
            {
                if (saveFileStream != null)
                    saveFileStream.Dispose();
            }
        }
    }
}
