using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using th;

namespace YuntiVpnAutoUpdate
{
    public partial class frmMain : Form
    {
        string saveFilePath = Application.StartupPath + @"\speed_up.zip";
        string vpnupBat = Application.StartupPath + @"\vpnup.bat";
        string vpndownBat = Application.StartupPath + @"\vpndown.bat";
        public frmMain()
        {
            InitializeComponent();
        }

        static Int32 iTime1Counter = 0;
        private void Form1_Load(object sender, EventArgs e)
        {
            Thread thdWord = new Thread(new ThreadStart(OpenWebSite));
            thdWord.SetApartmentState(ApartmentState.STA);
            thdWord.IsBackground = true;
            thdWord.Start();
        }

        void OpenWebSite()
        {
            WebBrowser webBrowser = new WebBrowser();

            string sign_inUrlString = @"https://www.yuntibit.com/users/sign_in";
            string adminUrlString = @"https://www.yuntibit.com/admin";
            string speed_upUrlString = @"https://www.yuntibit.com/admin/speed_up/download?platform=windows_nt_6";
            string siteCookie = string.Empty;

            WriteState("打开网站中...");
            webBrowser.Navigate(sign_inUrlString);
            WebBrowserLoding(webBrowser);
            WriteState("登录中...");
            LoginUser(webBrowser.Document, "Assange", "assange2769");
            WebBrowserLoding(webBrowser);
            siteCookie = BrowserCookieHelper.GetCookieInternal(new Uri(adminUrlString), false);
            WriteState("下载文件...");
            DownloadData(speed_upUrlString, siteCookie);
            WriteState("解压文件...");
            UnZipFile(saveFilePath);
            WriteState("删除文件...");
            DeleteFile(saveFilePath);
            WriteState("bat启动...");

            using (Process process = Process.Start(vpnupBat))
                process.WaitForExit();

            DeleteFile(vpnupBat);
            DeleteFile(vpndownBat);

            System.Environment.Exit(0);
        }

        /// <summary>
        /// 自动登录
        /// </summary>
        void LoginUser(HtmlDocument document, string username, string pwd)
        {
            HtmlElement btnSubmit = document.GetElementById("commit");
            HtmlElement tbUserid = document.GetElementById("user_login");
            HtmlElement tbPasswd = document.GetElementById("user_password");

            if (tbUserid == null || tbPasswd == null)
                return;

            tbUserid.SetAttribute("value", username);
            tbPasswd.InnerText = pwd;
            btnSubmit.InvokeMember("click");
        }

        void WebBrowserLoding(WebBrowser webBrowser)
        {
            try
            {
                this.timer1.Start();
                iTime1Counter = 0;//每次进入方法重新开始时间计数
                Boolean bolIsContinue = true;
                Int32 iComplteCounter = 0;//完成个数

                while (bolIsContinue)
                {
                    Thread.Sleep(200);//每次循环睡眠200ms

                    while (webBrowser.ReadyState != WebBrowserReadyState.Complete || webBrowser.IsBusy == true)//查看Webbrowser是否在加载文档
                    {
                        if (iTime1Counter >= 8)
                        {
                            this.timer1.Stop();
                            webBrowser.Stop();
                            iTime1Counter = 0;//计时个数清零
                            iComplteCounter = 0;//完成个数清零
                            return;
                        }

                        iComplteCounter = 0;//只要执行加载文档完成个数清零
                        Application.DoEvents();
                    }

                    if (webBrowser.ReadyState == WebBrowserReadyState.Complete && webBrowser.IsBusy == false)//记录Webbrowser的完成状态
                        iComplteCounter++;

                    if (iComplteCounter > 5)//如果Webbrowser的完成个数大于5说明已经加载完成
                        bolIsContinue = false;
                }
            }
            catch { }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            iTime1Counter++;
        }

        void DownloadData(string strUrl, string cookie)
        {
            using (WebClient client = new WebClient())
            {
                client.Headers["User-Agent"] = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/39.0.2171.95 Safari/537.36";
                client.Headers["cookie"] = cookie;
                client.DownloadFile(strUrl, saveFilePath);
            }
        }

        void UnZipFile(string zipFilePath)
        {
            if (!File.Exists(zipFilePath))
            {
                Console.WriteLine("Cannot find file '{0}'", zipFilePath);
                return;
            }

            using (ZipInputStream s = new ZipInputStream(File.OpenRead(zipFilePath)))
            {

                ZipEntry theEntry;
                while ((theEntry = s.GetNextEntry()) != null)
                {

                    Console.WriteLine(theEntry.Name);

                    string directoryName = Path.GetDirectoryName(theEntry.Name);
                    string fileName = Path.GetFileName(theEntry.Name);

                    // create directory
                    if (directoryName.Length > 0)
                    {
                        Directory.CreateDirectory(directoryName);
                    }

                    if (fileName != String.Empty)
                    {
                        using (FileStream streamWriter = File.Create(theEntry.Name))
                        {

                            int size = 2048;
                            byte[] data = new byte[2048];
                            while (true)
                            {
                                size = s.Read(data, 0, data.Length);
                                if (size > 0)
                                {
                                    streamWriter.Write(data, 0, size);
                                }
                                else
                                {
                                    break;
                                }
                            }
                        }
                    }
                }
            }
        }

        void DeleteFile(string path)
        {
            if (!File.Exists(path))
                return;

            File.Delete(path);
        }

        void WriteState(string message)
        {
            this.Invoke((EventHandler)delegate
            {
                if (string.IsNullOrEmpty(this.txtState.Text))
                    this.txtState.AppendText(message);
                else
                    this.txtState.AppendText("\r\n" + message);
            });
        }

        private void frmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            DeleteFile(vpnupBat);
            DeleteFile(vpndownBat);
        }
    }
}
