using Business;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace RouteTool
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        private void btnPing_Click(object sender, EventArgs e)
        {
            string urlString = this.txtUrl.Text.Trim();
            string enter = "\r\n";
            string localIp = string.Empty
                , ipAddress = string.Empty;

            LocalInfo LocalInfo = new LocalInfo();


            urlString = Regex.Replace(urlString, @"http://", string.Empty, RegexOptions.IgnoreCase).Replace(@"/", string.Empty);
            Ping ping = new Ping();
            PingReply urlInfo = ping.Send(urlString);

            if (urlInfo.Status != IPStatus.Success)
                return;

            ipAddress = urlInfo.Address.ToString();

            if (string.IsNullOrEmpty(this.txtUrlInfo.Text.Trim()))
                this.txtUrlInfo.Text = "IP:" + ipAddress + enter;
            else
                this.txtUrlInfo.AppendText("IP:" + ipAddress + enter);


            string strCmdText = string.Format("route add {0} mask 255.255.255.255 {1} metric 4270", ipAddress, LocalInfo.IPGATEWAY);
            Command cmd = new Command();
            string resultInfo = cmd.Execute(strCmdText);

            this.txtUrlInfo.AppendText("result:" + resultInfo + enter);
        }
    }

    public class LocalInfo
    {

        /// <summary>
        /// Mac地址
        /// </summary>
        public string MAC { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IP { get; set; }
        /// <summary>
        /// 子网掩码
        /// </summary>
        public string IPSUBNET { get; set; }
        /// <summary>
        /// 默认网关
        /// </summary>
        public string IPGATEWAY { get; set; }

        public LocalInfo()
        {
            ManagementClass mc = new ManagementClass("Win32_NetworkAdapterConfiguration");
            ManagementObjectCollection nics = mc.GetInstances();
            foreach (ManagementObject nic in nics)
            {
                if (Convert.ToBoolean(nic["ipEnabled"]) == true)
                {
                    MAC = nic["MacAddress"].ToString();//Mac地址
                    IP = (nic["IPAddress"] as String[])[0];//IP地址
                    IPSUBNET = (nic["IPSubnet"] as String[])[0];//子网掩码
                    IPGATEWAY = (nic["DefaultIPGateway"] as String[])[0];//默认网关

                    return;
                }
            }
        }

    }
}
