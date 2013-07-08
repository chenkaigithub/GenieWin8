using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace GenieWin8
{
    class GenieFcml
    {
        HttpClient httpClient;
        bool retOK;
        string name;
        string cookie; 
        public GenieFcml()
        {
            httpClient = new HttpClient();
        }

        public async Task<string> Init(string user, string pwd)
        {
            string resourceAddress = string.Format("https://genie.netgear.com/fcp/authenticate");
            string sMode = "<authenticate type=\"basic\""
                         + " username=\"{0}\" password=\"{1}\"/>";
            string body = string.Format(sMode,user,pwd);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, resourceAddress);
            StringContent soapContent = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            request.Content = soapContent;
            request.Headers.ConnectionClose = true;
            request.Headers.ExpectContinue = false;
            byte[] resultbt;
            string resultstr;
            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                string header = response.Headers.ToString();
                cookie = GetCookie(header);
                resultbt = await response.Content.ReadAsByteArrayAsync();
                resultstr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    retOK = true;
                }
                else
                {
                    retOK = false;
                }
                if (resultstr != null)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(resultstr);
                    IXmlNode node = xmlDoc.FirstChild;
                    if (node != null)
                    {
                        if (0 == node.NodeName.CompareTo("authenticate"))
                        {
                            foreach (XmlAttribute att in node.Attributes)
                            {
                                if (att.NodeName == "authenticated"&&att.NodeValue =="true")
                                {
 
                                }
                            }
                            if (node.Attributes[1].InnerText!=null)
                            {
                                 
                            }
                        }

                    }
                   
                }
               // return resultstr;
            }
            catch (HttpRequestException hre)
            {
               // return "";
            }
            catch (TaskCanceledException hce)
            {
              //  return "";
            }
            catch (Exception ex)
            {
               // return "";
            }
           // return true;

            ////step 2 init
            body = "<init type=\"ui\" fcmb=\"true\"/>";
            resourceAddress = string.Format("https://genie.netgear.com/fcp/init");
             request = new HttpRequestMessage(HttpMethod.Post, resourceAddress);
             soapContent = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            request.Content = soapContent;
            request.Headers.ConnectionClose = true;
            request.Headers.ExpectContinue = false;
            request.Headers.Add("Cookie", cookie);
           // byte[] resultbt;
           // string resultstr;
            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                resultbt = await response.Content.ReadAsByteArrayAsync();
                resultstr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    retOK = true;
                }
                else
                {
                    retOK = false;
                }
                if (resultstr != null)
                {
                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(resultstr);
                    IXmlNode node = xmlDoc.FirstChild;
                    if (node != null)
                    {
                        if (0 == node.NodeName.CompareTo("init"))
                        {
                            foreach (XmlAttribute att in node.Attributes)
                            {
                                if (att.NodeName == "name")
                                {
                                    name = att.NodeValue.ToString();
                                }
                            }
                        }
                        
                    }
                    string root = xmlDoc.FirstChild.NextSibling.NodeName;
                }
                // return resultstr;
            }
            catch (HttpRequestException hre)
            {
                // return "";
            }
            catch (TaskCanceledException hce)
            {
                //  return "";
            }
            catch (Exception ex)
            {
                // return "";
            }
            return "";
        }

        public async Task<string> SendFcml(HttpMethod method,string resourceAddress,string body)
        {
            HttpRequestMessage request = new HttpRequestMessage(method, resourceAddress);
            StringContent soapContent = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            request.Content = soapContent;
            request.Headers.ConnectionClose = true;
            request.Headers.ExpectContinue = false;
            byte[] resultbt;
            string resultstr;
            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                resultbt = await response.Content.ReadAsByteArrayAsync();
                resultstr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    retOK = true;
                }
                else
                {
                    retOK = false;
                }
                if (resultstr != null)
                {
                }
                 return resultstr;
            }
            catch (HttpRequestException hre)
            {
                 return "";
            }
            catch (TaskCanceledException hce)
            {
                 return "";
            }
            catch (Exception ex)
            {
                 return "";
            }
        }

        public async Task<string> FcmlRequest(string to, string requestText)
        {
            string resourceAddress = string.Format("https://genie.netgear.com:443/fcp/send?n={0}", name);
            string body = "<fcml to=\"{0}\" from=\"{1}@portal\" _tracer=\"{2}\">" 
                        + "{3}</fcml>";
            body = string.Format(body, to,name,1,requestText);
           
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, resourceAddress);
            StringContent soapContent = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            request.Content = soapContent;
            request.Headers.ConnectionClose = true;
            request.Headers.ExpectContinue = false;
            request.Headers.Add("Cookie",cookie);
            byte[] resultbt;
            string resultstr;
            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                resultbt = await response.Content.ReadAsByteArrayAsync();
                resultstr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
                HttpStatusCode statusCode = response.StatusCode;
                if (statusCode == HttpStatusCode.OK)
                {
                    retOK = true;
                    resourceAddress = string.Format("https://genie.netgear.com:443/fcp/receive?n={0}", name);
                    HttpRequestMessage requestRec = new HttpRequestMessage(HttpMethod.Get, resourceAddress);
                    requestRec.Headers.ConnectionClose = true;
                    requestRec.Headers.ExpectContinue = false;
                    requestRec.Headers.Add("Cookie",cookie);
                    HttpResponseMessage responseRec = await httpClient.SendAsync(requestRec);
                    resultbt = await responseRec.Content.ReadAsByteArrayAsync();
                    resultstr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
                   
                }
                else
                {
                    retOK = false;
                }
                if (resultstr != null)
                {
                }
                return resultstr;
            }
            catch (HttpRequestException hre)
            {
                 return "";
            }
            catch (TaskCanceledException hce)
            {
                  return "";
            }
            catch (Exception ex)
            {
                 return "";
            }
            
        }

        /// <summary>
        /// <fcml from="router@portal" to="ui126903@portal"  _tracer="1" >
        ///       <portal.router.cp82194.is_all modelId="controlpointmodel6" model="WNR3500Lv2" local_ip="192.168.9.135" owner="true" active="false" friendly_name="My WNR3500Lv2" type="domain" serial="2P21227Y00005" ip="220.168.30.10"/>
		///		   <portal.router.cp82195.is_all modelId="controlpointmodel6" model="WNR3500Lv2" local_ip="192.168.9.163" owner="true" active="false" friendly_name="My WNR3500Lv2" type="domain" serial="2P21177F0008A" ip="220.168.30.10"/>
        ///	</fcml>
        /// </summary>
        /// <returns></returns>
        public async Task<List<Dictionary<string, string>>> GetCPList()
        {
            List<Dictionary<string, string>> cpList = new List<Dictionary<string, string>>();//存储远程端的每一个路由器信息
            string result = await FcmlRequest("router@portal", "<get/>");
            if (result != "" && result != null)
            {
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(result);
                IXmlNode root = xmlDoc.FirstChild;
                if (root != null)
                {
                    if (0 == root.NodeName.CompareTo("fcml"))
                    {
                        if (root.HasChildNodes())
                        {
                            foreach (IXmlNode node in root.ChildNodes)
                            {
                                Dictionary<string, string> rowDic = new Dictionary<string, string>();
                                string cpName = CpName(node.NodeName);
                                rowDic.Add("cp_identifier", cpName);
                                rowDic.Add("model", node.Attributes[1].NodeValue.ToString());
                                rowDic.Add("active", node.Attributes[4].NodeValue.ToString());
                                rowDic.Add("friendly_name", node.Attributes[5].NodeValue.ToString());
                                rowDic.Add("serial", node.Attributes[7].NodeValue.ToString());

                                cpList.Add(rowDic);
                            }
                        }
                    }
                }
            }
            return cpList;
        }

        /// <summary>
        /// 根据/fcp/authenticate请求中解析出Cookie
        /// </summary>
        /// <param name="head"></param>
        /// <returns></returns>
        private string GetCookie(string head)
        {
            string[] sList = Regex.Split(head, "\r\n|\n");
            Regex rgx = new Regex("Set-Cookie:\\s+(.*)=(.*);");
            for(int i =0; i<sList.Count();i++)
            {
               Match match = rgx.Match(sList[i]);
               if (match.Success)
               {
                   return match.Groups[1].Value.Split(';')[0];
               }
            }
            
            return "";
        }

        /// <summary>
        /// 获取cp标识
        /// </summary>
        /// <param name="tagName">portal.router.cp82194.is_all</param>
        /// <returns>cp82194</returns>
        private string CpName(string tagName)
        {
            string retName = "";
            Regex reg = new Regex("portal\\.router\\.(.*)\\.is_all");
            Match match = reg.Match(tagName);
            if (match.Success)
            {
                retName = match.Groups[1].ToString();
            }
            return retName;
        }
    }
}
