﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace Microsoft.Samples.Networking.HttpClientSample
{
    class GenieSoapApi
    {
        HttpClient httpClient;
        UtilityTool util;
        public GenieSoapApi()
        {
            httpClient = new HttpClient();
            util = new UtilityTool();

        }
        public async void Authenticate(string username, string password)
        {
            Dictionary<string, string> param = new Dictionary<string,string>();
            param.Add("NewUsername", username);
            param.Add("NewPassword", password);
            string retParam = await postSoap("ParentalControl", "Authenticate", 5000, param);
        }
        public async Task<Dictionary<string,Dictionary<string,string>>> GetAttachDevice()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            string retParam = "";
            Dictionary<string,Dictionary<string,string>> dicAttachDevice = new Dictionary<string,Dictionary<string,string>>();
            retParam = await postSoap("DeviceInfo", "GetAttachDevice", 5000, param);
            if (retParam != null)
            {
               retParam = util.subString(retParam, "<NewAttachDevice>", "</NewAttachDevice>");
               string[] tempArray = retParam.Split('@');
               string macAdr = ""; 
               for (int i = 1; i < tempArray.Length; i++)
               {
                   Dictionary<string,string> dicRow = new Dictionary<string,string>();
                   string[] itemArray = tempArray[i].Split(';');
                   if (itemArray.Length >= 4)
                   {
                       dicRow.Add("Ip", itemArray[1]);
                       dicRow.Add("HostName", itemArray[2]);
                       macAdr = itemArray[3];
                       

                   }
                   if (itemArray.Length >= 5)
                   {
                       dicRow.Add("Connect", itemArray[4]);
                   }
                   if (itemArray.Length >= 7)
                   {
                       dicRow.Add("LinkSpeed", itemArray[5]);
                       dicRow.Add("Signal", itemArray[6]);
                   }
                   dicAttachDevice.Add(macAdr,dicRow);
               }
            }
            return  dicAttachDevice;
        }
        public async Task<string> postSoap(string module, string method, int port, Dictionary<string,string> param)
        {

            string resourceAddress = string.Format("http://routerlogin.com:{0}/soap/server_sa", port);
            string soapAction = string.Format("urn:NETGEAR-ROUTER:service:{0}:1#{1}", module, method);

                string soapBodyMode = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>"
               + "<SOAP-ENV:Envelope xmlns:SOAPSDK1=\"http://www.w3.org/2001/XMLSchema\" "
                +    "xmlns:SOAPSDK2=\"http://www.w3.org/2001/XMLSchema-instance\" "
                 +   "xmlns:SOAPSDK3=\"http://schemas.xmlsoap.org/soap/encoding/\" "
                  +  "xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">"
                   +     "<SOAP-ENV:Header>"
                    +        "<SessionID>58DEE6006A88A967E89A</SessionID>"
                     +   "</SOAP-ENV:Header>"
                      +  "<SOAP-ENV:Body>"
                       +     "<M1:{1} xmlns:M1=\"urn:NETGEAR-ROUTER:service:{0}:1\">"
            +"{2}"
                       +     "</M1:{1}>"
                     +   "</SOAP-ENV:Body>"
               + "</SOAP-ENV:Envelope>";

            string s_para = "";
            if(param.Count > 0)
                    {
                         foreach(KeyValuePair<string, string> kv in param)
                         {
                             s_para += string.Format("<{0}>{1}</{0}>",kv.Key,kv.Value);
                         }
                    }
                    string soapBody = string.Format(soapBodyMode,module,method,s_para);

                if(0 == module.CompareTo("ParentalControl"))
                {
                    soapBodyMode = "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">\n"
                 + "<SOAP-ENV:Header>\n"
                 + "<SessionID xsi:type=\"xsd:string\" xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\">E6A88AE69687E58D9A00</SessionID>\n"
                 + "</SOAP-ENV:Header>\n"
                 + "<SOAP-ENV:Body>\n"
                 + "<{0}>\n"
         + "{1}"
                 + "</{0}>\n"
                 + "</SOAP-ENV:Body>\n"
                 + "</SOAP-ENV:Envelope>\n";
                    string paraString = " xsi:type=\"xsd:string\" xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\"";
                    s_para = "";
;                    if(param.Count > 0)
                    {
                         foreach(KeyValuePair<string, string> kv in param)
                         {
                             s_para += string.Format("<{0}{2}>{1}</{0}>\n",kv.Key,kv.Value,paraString);
                         }
                    }
                    soapBody = string.Format(soapBodyMode,method,s_para);
                }
            

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, resourceAddress);
                StringContent soapContent = new StringContent(soapBody, Encoding.UTF8, "text/xml");
                request.Content = soapContent;
                request.Content.Headers.Add("SOAPAction", soapAction);
                CacheControlHeaderValue nocache = new CacheControlHeaderValue();
                nocache.NoCache = true;
                request.Headers.CacheControl = nocache;
                request.Headers.Pragma.Add(new NameValueHeaderValue("no-cache"));
                //request.Headers.UserAgent.Add(new ProductInfoHeaderValue("SOAP Toolkit 3.0")); 
                request.Headers.ExpectContinue = false;
                byte[] resultbt;
                string resultstr;
                try
                {
                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    resultbt = await response.Content.ReadAsByteArrayAsync();
                    resultstr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
                    System.Diagnostics.Debug.WriteLine(resultstr);
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
        public async void SendStart(string host, int port)
        {
            
        }
        public async void SendFinish(string host, int port)
        {
 
        }
       
    }
}
