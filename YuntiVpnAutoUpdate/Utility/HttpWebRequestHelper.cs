using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace th
{

    public class HttpWebRequestHelper
    {
        /// <summary>
        /// 使用HttpWebRequest 的Post方式获取String 型数据
        /// </summary>
        /// <param name="url">String Url</param>
        /// <param name="postData">String PostData</param>
        /// <param name="cookie">String Cookie</param>
        /// <returns>String 文本</returns>
        public static string HttpRequestPost(string url, string postData, string cookie, Encoding encoding)
        {
            string data = string.Empty;
            HttpWebRequest request = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 3000;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Method = @"POST";
                request.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36";
                request.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
                request.Headers["Cookie"] = cookie;

                byte[] byteArray = Encoding.UTF8.GetBytes(postData);

                using (Stream reqStream = request.GetRequestStream())
                    reqStream.Write(byteArray, 0, byteArray.Length);


                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), encoding))
                        data = streamReader.ReadToEnd();
                }

            }
            catch (Exception)
            {
                return data;
            }
            finally
            {
                request.Abort();
                request = null;
                System.GC.Collect();
            }

            return data;
        }

        /// <summary>
        /// 使用HttpWebRequest 的Get方式获取String 型数据
        /// </summary>
        /// <param name="url">String Url</param>
        /// <param name="cookie">String Cookie</param>
        /// <param name="host">String 如:wenku.baidu.com 不需要加Http://</param>
        /// <returns>String 文本</returns>
        public static string HttpRequestGet(string url, string cookie, string host, Encoding encoding)
        {
            HttpWebRequest request = null;
            string data = string.Empty;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
                request.Timeout = 3000;
                request.ProtocolVersion = HttpVersion.Version11;
                request.Host = host;
                request.Method = @"GET";
                request.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36";
                request.ContentType = @"application/x-www-form-urlencoded; charset=UTF-8";
                request.Headers["Cookie"] = cookie;


                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), encoding))
                        data = streamReader.ReadToEnd();
                }
            }
            catch
            {
                return data;
            }
            finally
            {
                request.Abort();
                request = null;
                System.GC.Collect();
            }

            return data;
        }

        public static string DownloadSourceData(string strUrl, string strCookie, Encoding encoding)
        {
            string strResult = string.Empty;
            HttpWebRequest request = null;

            try
            {
                request = (HttpWebRequest)WebRequest.Create(strUrl);
                request.Timeout = 3000;
                request.ProtocolVersion = HttpVersion.Version11;
                request.UserAgent = @"Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/38.0.2125.111 Safari/537.36";

                using (WebResponse response = request.GetResponse())
                {
                    using (StreamReader streamReader = new StreamReader(response.GetResponseStream(), encoding))
                        strResult = streamReader.ReadToEnd();
                }
            }
            catch
            {
                return strResult;
            }
            finally
            {
                request.Abort();
                request = null;
                System.GC.Collect();
            }

            return strResult;
        }
    }
}
