using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
using System.Windows;
using System.Net.Sockets;
using System.Threading;

using GenieWP8.DataInfo;

namespace GenieWP8
{
    /// <summary>
    /// 处理一些实用的公共方法的类
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
            string rootStr = ((XElement)root).Name.LocalName;
            var nodeList = xdoc.Root.Nodes();
            if (nodeList.Count() > 0)
            {
                foreach (XNode node in nodeList)
                {
                    if (node != null)
                    {
                        NodeOperate(node, rootStr);
                    }
                }
            }
            return responseDic;
        }

        /// <summary>
        /// 递归遍历xml节点
        /// </summary>
        /// <param name="node"></param>
        private void NodeOperate(XNode node,string rootStr)
        {
            XElement e = (XElement)node;
            if (e != null)
            {
                string nodeName, nodeValue;
                if (e.HasElements)
                {
                    NodeOperate(e.FirstNode,rootStr);
                }
                else
                {
                    nodeName = e.Name.LocalName;
                    nodeValue = e.Value;
                    responseDic.Add(nodeName, nodeValue);
                    if (e.NextNode != null)
                    {
                        NodeOperate(e.NextNode,rootStr);
                    }
                    else
                    {
                        int flag = 0;
                        while (e.NextNode == null)
                        {
                            if (e.Name.LocalName == rootStr)
                            {
                                flag = 1;
                                break;
                            }
                            else
                            {
                                e = e.Parent;
                            }
                        }
                        if (flag == 0)
                        {
                            NodeOperate(e.NextNode,rootStr);
                        }
                    }
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

        /// <summary>
        /// 判断是否有Internet
        /// </summary>
        /// <returns></returns>
        public bool IsConnectedToInternet()
        {
            string result = Connect("www.netgear.com", 80);
            if (result == "Success")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        // 同步对象，用来通知一个异步操作已经完成
        static ManualResetEvent _clientDone = new ManualResetEvent(false);
        /// <summary>
        /// 尝试向一个给定的服务器和端口发起一个 Tcp 套接字的链接请求
        /// </summary>
        /// <param name="hostName">服务器的 name</param>
        /// <param name="portNumber">连接的端口</param>
        /// <returns>返回异步套接字的操作结果</returns>
        public string Connect(string hostName, int portNumber)
        {
            // 在这个类的对象的生命周期中会在每次调用中使用
            Socket _socket = null;

            // 为每次异步调用声明一个毫秒级的超时时间，如果在这个时间内没有收到回应，那么这个调用就会被终止
            const int Timeout_Milliseconds = 5000;
            string result = string.Empty;

            // 将网络终结点表示为主机名或 IP 地址和端口号的字符串表示形式。
            DnsEndPoint hostEntry = new DnsEndPoint(hostName, portNumber);

            _socket = new Socket(AddressFamily.InterNetwork, //  IP 版本 4 的地址。
                                          SocketType.Stream,
                                          ProtocolType.Tcp);

            // 创建一个 SocketAsyncEventArgs 对象，在连接请求中使用
            SocketAsyncEventArgs socketEventArg = new SocketAsyncEventArgs();
            socketEventArg.RemoteEndPoint = hostEntry;

            // 把事件委托调用采用内联的方式，以使这个方法包含在内部
            socketEventArg.Completed += new EventHandler<SocketAsyncEventArgs>(delegate(object s, SocketAsyncEventArgs e)
            {
                // 获取连接请求的结果
                result = e.SocketError.ToString();

                // 通知请求结束，解除对 UI 线程的阻塞
                _clientDone.Set();
            });

            // 将事件状态设置为非终止状态，从而导致线程受阻。
            _clientDone.Reset();

            // 开始一个对远程主机连接的异步请求。
            _socket.ConnectAsync(socketEventArg);

            // 阻止 UI 线程，最多 TIMEOUT_MILLISECONDS 时间，如果在指定时间没有响应则继续向下执行
            _clientDone.WaitOne(Timeout_Milliseconds);

            return result;
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
       

        /// <summary>
        /// 获取本机的Mac地址
        /// </summary>
        /// <returns></returns>
        public string GetLocalMacAddress()
        {
            Dictionary<string, Dictionary<string, string>> attachDevice = NetworkMapInfo.attachDeviceDic;
            var iplist = GetCurrentIpAddresses();
            if (iplist.Count() > 0)
            {
                foreach (string ip in iplist)
                {
                    if (attachDevice.Count > 0)
                    {
                        foreach (string key in attachDevice.Keys)
                        {
                            if (ip == attachDevice[key]["Ip"])
                            {
                                return key;
                            }
                        }
                    }
                }
            }

            /*
            byte[] myDeviceID = (byte[])Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceUniqueId");
            string maAddress = BitConverter.ToString(myDeviceID);
            string idAsString = Convert.ToBase64String(myDeviceID);
            string Manufacturer = (string)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceManufacturer");
            string DeviceName = (string)Microsoft.Phone.Info.DeviceExtendedProperties.GetValue("DeviceName");
           // MessageBox.Show("myDeviceID:" + idAsString + "\nManufacturer:" + Manufacturer + "\nDeviceName:" + DeviceName);

            Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceList InterfacesList = new Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceList();
            foreach (Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceInfo specificInterface in InterfacesList)
            {
                if (specificInterface.InterfaceType == Microsoft.Phone.Net.NetworkInformation.NetworkInterfaceType.Wireless80211)
                {
                    Console.WriteLine("This interface is a Wifi Interface :");
                }
            }*/
            return string.Empty;
        }

        /// <summary>
        /// 获取本机网卡的ip地址列表
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetCurrentIpAddresses()
        {
            var ipList = new HashSet<string>();
            var hostnames = NetworkInformation.GetHostNames();
            foreach (var hn in hostnames)
            {
                if (hn.IPInformation != null)
                {
                    string ipAddress = hn.DisplayName;
                    if (ipAddress != "" && ipAddress != null)
                    {
                        ipList.Add(ipAddress);
                    }
                }
            }
             return ipList;
        }

    }
}