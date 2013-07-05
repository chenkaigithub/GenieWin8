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

        public async Task<string> postFcml(string resourceAddress,string body)
        {
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
                   // string fcmlBody = "";
                    //postFcml();
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

        public async Task<string> GetCPList()
        {
            string result = await FcmlRequest("router@portal", "<get/>");
            if (result != "" && result != null)
            { 
                XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.LoadXml(result);
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
            }
            return "";
        }

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
    }
}
