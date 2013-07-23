using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using Windows.Data.Xml.Dom;
using System.Net;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;
using Windows.Networking;
using System.Runtime.Serialization.Json;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage.Streams;
using System.Diagnostics;
using System.Xml.Linq;
//using GenieWin8.DataModel;

namespace GenieWin8
{
    /// <summary>
    /// 一些处理实用的公共方法的类
    /// </summary>
    class UtilityTool
    {
        /// <summary>
        /// responseDic保存解析xml所得到的结果
        /// </summary>
        Dictionary<string, string> responseDic;
        private string dicKey;
        public UtilityTool()
        { }
        public string SubString(string source, string start, string end)
        {
            int s = source.IndexOf(start);
            int e = source.IndexOf(end);
            return source.Substring(s + start.Length, e - s - end.Length + 1);
        }

        /////遍历xml/////////
        /// <summary>
        /// 
        /// </summary>
        /// <param name="xml"></param>
        /// <returns></returns>
        public Dictionary<string, string> TraverseXML(string xml)
        {
            responseDic = new Dictionary<string, string>();
            if (xml == null || xml == "")
            {
                return responseDic;
            }

            XDocument xdoc = XDocument.Parse(xml);
            XNode root = xdoc.Root.FirstNode;
            var nodeList = xdoc.Root.Nodes();
            if (nodeList.Count() > 0)
            {
                foreach (XNode node in nodeList)
                {
                    XElement e = (XElement)node;
                    string nodeName, nodeValue;

                    if (e.HasElements)
                    {
                        NodeOperate(e.FirstNode);
                    }
                    else
                    {
                        nodeName = e.Name.LocalName;
                        nodeValue = e.Value;
                        responseDic.Add(nodeName, nodeValue);
                        NodeOperate(e.NextNode);
                    }
                }
            }

            return responseDic;

        }

        /// <summary>
        /// 递归遍历xml节点
        /// </summary>
        /// <param name="root"></param>
        private void NodeOperate(XNode root)
        {
            XElement e = (XElement)root;
            if (e != null)
            {
                string nodeName, nodeValue;
                if (e.HasElements)
                {
                    NodeOperate(e.FirstNode);
                }
                else
                {
                    nodeName = e.Name.LocalName;
                    nodeValue = e.Value;
                    responseDic.Add(nodeName, nodeValue);
                    NodeOperate(e.NextNode);
                }
            }

        }

        //public static IPAddress Find()
        //{
        //    List<string> ipAddresses = new List<string>();

        //    var hostnames = NetworkInformation.GetHostNames();
        //    foreach (var hn in hostnames)
        //    {
        //        if (hn.IPInformation != null)
        //        {
        //            string ipAddress = hn.DisplayName;
        //            ipAddresses.Add(ipAddress);
        //        }
        //    }

        //    IPAddress address = IPAddress.Parse(ipAddresses[0]);
        //    return address;
        //}

        /// <summary>
        /// 获取本机ip地址
        /// </summary>
        /// <returns></returns>
        public string GetLocalHostIp()
        {
            //var conProfile = NetworkInformation.GetInternetConnectionProfile();

            //if (conProfile != null && conProfile.NetworkAdapter != null)
            //{
            //    var hostName =
            //        NetworkInformation.GetHostNames().SingleOrDefault(
            //            hn =>
            //            hn.IPInformation.NetworkAdapter != null &&
            //            hn.IPInformation.NetworkAdapter.NetworkAdapterId ==
            //               conProfile.NetworkAdapter.NetworkAdapterId);



            //if (hostName != null)
            //{
            //    //return the ip address
            //return hostName.CanonicalName;
            //}
            //}


            var icp = NetworkInformation.GetInternetConnectionProfile();
            
            if (icp != null && icp.NetworkAdapter != null)
            {
                var hostname =
                    NetworkInformation.GetHostNames().SingleOrDefault(
                        hn =>
                        hn.IPInformation != null &&
                        hn.IPInformation.NetworkAdapter.NetworkAdapterId ==
                           icp.NetworkAdapter.NetworkAdapterId);
                System.Diagnostics.Debug.WriteLine("可用网络IP:" + hostname.DisplayName);
                System.Diagnostics.Debug.WriteLine("网络状态:" + icp.GetNetworkConnectivityLevel());
                return hostname.DisplayName;
                
            }
            return string.Empty;

        }

        /// <summary>
        /// 获取网关
        /// </summary>
        /// <returns></returns>
        public async Task<string> GetGateway()
        {
            //网关地址
            //string strGateway = "";
            //获取所有网卡
            //NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            //遍历数组
            //foreach (var netWork in nics)
            //{
            //    单个网卡的IP对象
            //    IPInterfaceProperties ip = netWork.GetIPProperties();
            //    获取该IP对象的网关
            //    GatewayIPAddressInformationCollection gateways = ip.GatewayAddresses;
            //    foreach (var gateWay in gateways)
            //    {
            //        如果能够Ping通网关
            //        if (IsPingIP(gateWay.Address.ToString()))
            //        {
            //            得到网关地址
            //            strGateway = gateWay.Address.ToString();
            //            跳出循环
            //            break;
            //        }
            //    }

            //    如果已经得到网关地址
            //    if (strGateway.Length > 0)
            //    {
            //        跳出循环
            //        break;
            //    }
            //}

            //返回网关地址




            HostName serverHost = new HostName("routerlogin.net");
            var clientSocket = new Windows.Networking.Sockets.StreamSocket();

            // Try to connect to the remote host
            await clientSocket.ConnectAsync(serverHost, "http");

            // Get the HostName as DisplayName, CanonicalName, Raw with the IpAddress.
            var ipAddress = clientSocket.Information.RemoteAddress.DisplayName;

            return ipAddress.ToString();
        }

        //public enum NetworkConnectivityLevel
        //{
        // 摘要:
        //     无连接。
        // None = 0,
        //
        // 摘要:
        //     仅本地网络访问。
        //   LocalAccess = 1,
        //
        // 摘要:
        //     受限的 internet 访问。
        //  ConstrainedInternetAccess = 2,
        //
        // 摘要:
        //     本地和 internet 访问。
        // InternetAccess = 3,
        // }

        /// <summary>
        /// 判断是否有Internet
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToInternet()
        {
            bool connected = false;
            ConnectionProfile cp = NetworkInformation.GetInternetConnectionProfile();
            if (cp != null)
            {
                NetworkConnectivityLevel cl = cp.GetNetworkConnectivityLevel();
                connected = cl == NetworkConnectivityLevel.InternetAccess;
            }
            return connected;
        }
 

        /// <summary>
        /// 将json字符串转换成JObject对象
        /// </summary>
        /// <param name="jsonText"></param>
        /// <returns></returns>
        public JObject ConvertJsonToObject(string jsonText)
        {
            JObject jo = (JObject)JsonConvert.DeserializeObject(jsonText);
            return jo;
        }
       
        public string GetMACAddress()
        {
            //string deviceSerial = string.Empty;
            //// http://msdn.microsoft.com/en-us/library/windows/apps/jj553431
            //Windows.System.Profile.HardwareToken hardwareToken = Windows.System.Profile.HardwareIdentification.GetPackageSpecificToken(null);
            //using (DataReader dataReader = DataReader.FromBuffer(hardwareToken.Id))
            //{
            //    int offset = 0;
            //    while (offset < hardwareToken.Id.Length)
            //    {
            //        byte[] hardwareEntry = new byte[4];
            //        dataReader.ReadBytes(hardwareEntry);

            //        // CPU ID of the processor || Size of the memory || Serial number of the disk device || BIOS
            //        if ((hardwareEntry[0] == 1 || hardwareEntry[0] == 2 || hardwareEntry[0] == 3 || hardwareEntry[0] == 9) && hardwareEntry[1] == 0)
            //        {
            //            if (!string.IsNullOrEmpty(deviceSerial))
            //            {
            //                deviceSerial += "|";
            //            }
            //            deviceSerial += string.Format("{0}.{1}", hardwareEntry[2], hardwareEntry[3]);
            //        }
            //        offset += 4;
            //    }
            //}
            //Debug.WriteLine("deviceSerial=" + deviceSerial);
            
            var names = NetworkInformation.GetHostNames();
            string mac="";
            int foundIdx;
            for (int i = 0; i < names.Count; i++)
            {
                foundIdx = names[i].DisplayName.IndexOf(".local");
                if (foundIdx > 0)
                {
                     mac = names[i].DisplayName.Substring(0, foundIdx);
                }
            }
            if (mac != null)
            {
            }

            return "";
        }

        /// <summary>
        /// 获取本机的Mac地址
        /// </summary>
        /// <returns></returns>
        public string GetLocalMacAddress()
        {
            //Dictionary<string, Dictionary<string, string>> attachDevice = NetworkMapModel.attachDeviceDic;
            //var iplist = GetCurrentIpAddresses();
            //if (iplist.Count() > 0)
            //{
            //    foreach (string ip in iplist)
            //    {
            //        if (attachDevice.Count > 0)
            //        {
            //            foreach (string key in attachDevice.Keys)
            //            {
            //                if (ip == attachDevice[key]["Ip"])
            //                {
            //                    return key;
            //                }
            //            }
            //        }
            //    }
            //}
            return string.Empty;
        }

        /// <summary>
        /// 获取本机网卡的ip地址列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCurrentIpAddresses()
        {
            var profiles = NetworkInformation.GetConnectionProfiles().ToList();

            // the Internet connection profile doesn't seem to be in the above list
           // profiles.Add(NetworkInformation.GetInternetConnectionProfile());

            IEnumerable<HostName> hostnames =
                NetworkInformation.GetHostNames().Where(h =>
                    h.IPInformation != null &&
                    h.IPInformation.NetworkAdapter != null).ToList();

            var tempList = (from h in hostnames
                    from p in profiles
                    where h.IPInformation.NetworkAdapter.NetworkAdapterId ==
                          p.NetworkAdapter.NetworkAdapterId
                    select string.Format("{0}", h.CanonicalName)).ToList();
             var set = new HashSet<string>(tempList);
             return set;
            //select string.Format("{0}, {1}", p.ProfileName, h.CanonicalName)).ToList();
        }

    }
}