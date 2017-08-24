using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace th
{
    public class WebBrowserHelper
    {
        WebBrowser webBrowser;
        public WebBrowserHelper(WebBrowser webBrowser)
        {
            // TODO: Complete member initialization
            this.webBrowser = webBrowser;
        }

        /// <summary>
        /// 加载Webbrowser页面
        /// </summary>
        /// <param name="url">String 网页地址</param>
        /// <param name="flag">标识用来识别是否得到想要的数据</param>
        /// <param name="encoding">设置读取网站Html源码的字符</param>
        /// <returns>页面Html代码</returns>
        public string LoadingPage(string url, string flag, Encoding encoding)
        {
            try
            {
                DateTime dtmStartTime = DateTime.Now;
                string strData = string.Empty;

                Navigate(url);

                while (true)
                {
                    WaitLoading();

                    using (StreamReader streamReader = new StreamReader(webBrowser.DocumentStream, encoding))
                        strData = streamReader.ReadToEnd();

                    if (strData.Contains(flag))
                        return strData;

                    TimeOutReload(ref dtmStartTime, url, 30);
                }
            }
            finally
            {
                MemoryHelper.WebbrowserFlushMemory();
            }
        }

        /// <summary>
        /// 实行Webbrowser导航
        /// </summary>
        /// <param name="strUrl"></param>
        void Navigate(string strUrl)
        {
            webBrowser.ScriptErrorsSuppressed = true;
            webBrowser.Stop();
            webBrowser.Navigate(strUrl);
        }

        /// <summary>
        /// 实行等待Webbrowser等待,必须执行等待不然Webbrowser无法加载页面
        /// </summary>
        void WaitLoading()
        {
            Application.DoEvents();
            Thread.Sleep(700);//停止100ms 让Webbrowser加载源代码
        }

        /// <summary>
        /// 由于网速过慢导致的超时将重新加载页面
        /// </summary>
        /// <param name="dtNow">DateTime 时间现在时</param>
        /// <param name="strUrl">Url链接</param>
        /// <param name="intSeconds">设置的超时时间隔,以秒为单位</param>
        void TimeOutReload(ref DateTime dtNow, string strUrl, int intSeconds)
        {
            TimeSpan timeSpan = DateTime.Now - dtNow;
            if (timeSpan.TotalSeconds >= intSeconds)
            {
                string strPatten = "?";

                if (strUrl.Contains(@"?"))
                    strPatten = @"&";

                Navigate(strUrl + @"" + strPatten + "sx=" + DateTime.Now.Ticks.ToString());

                dtNow = DateTime.Now;
            }
        }

        /// <summary>
        /// 将已有的网页写入一个标识设置为久网页
        /// </summary>
        void SetOldPage()
        {
            HtmlElement elem = webBrowser.Document.CreateElement("span");
            elem.InnerText = "oldPage";
            webBrowser.Document.Body.AppendChild(elem);
        }
    }
}
