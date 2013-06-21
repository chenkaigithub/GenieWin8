using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;
using System.Net;
using System.Net.NetworkInformation;
using Windows.Networking.Connectivity;
using Windows.Networking;


namespace GenieWin8
{
    class UtilityTool
    {
        Dictionary<string, string> responseDic;
        private string dicKey;
        public UtilityTool()
        { }
        public string SubString(string source ,string start, string end)
        {
            int s = source.IndexOf(start);
            int e = source.IndexOf(end);
            return source.Substring(s+start.Length, e - s - end.Length +1);
        }

        /////遍历xml/////////
        public Dictionary<string,string> TraverseXML(string xml)
        {
            responseDic = new Dictionary<string, string>();
            if(xml == null ||xml == "")
            {
                return responseDic;
            }
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(xml);
            XmlNodeList xnl = xmlDoc.DocumentElement.ChildNodes;
            string root = xmlDoc.FirstChild.NextSibling.NodeName;
            foreach(IXmlNode xn in xnl)
            {
                IXmlNode child = xn.FirstChild;
                if (child != null)
                {
                    NodeOperate(child, root);
                }
            }
            return responseDic;

        }

        private void NodeOperate(IXmlNode xn,string root)
        {
            
            if (xn.HasChildNodes())
            {
                dicKey = xn.NodeName;
                System.Diagnostics.Debug.WriteLine(xn.NodeName + "\n");
                //System.Diagnostics.Debug.WriteLine("\n");
                IXmlNode childNode = xn.FirstChild;
                
                NodeOperate(childNode,root);

            }
            else
            {
                
                //System.Diagnostics.Debug.WriteLine(xn.NodeName + "\n");
                System.Diagnostics.Debug.WriteLine(xn.InnerText);
                //System.Diagnostics.Debug.WriteLine("\n");
                string dicValue = xn.InnerText.Trim() ;
                if (dicValue != "")
                {
                    responseDic.Add(dicKey, dicValue);
                }
                if (xn.NextSibling != null)
                {

                    NodeOperate(xn.NextSibling,root);
                }
                else
                {
                    int flag = 0;
                    while (xn.NextSibling == null)
                    {
                        if (xn.NodeName == root)
                        {
                            flag = 1;
                            break;
                        }
                        else
                        {
                            xn = xn.ParentNode;
                        }

                    }
                    if (flag == 0)
                    {
                        NodeOperate(xn.NextSibling,root);
                    }
                    else if (flag == 1)
                    {
                       // System.Diagnostics.Debug.WriteLine("End");
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
                
    }
}
