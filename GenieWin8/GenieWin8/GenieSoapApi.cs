using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Windows.Data.Xml.Dom;

namespace GenieWin8
{
    class GenieSoapApi
    {
        HttpClient httpClient;
        UtilityTool util;
        bool retOK;
        public GenieSoapApi()
        {
            httpClient = new HttpClient();
            util = new UtilityTool();
            retOK = false;

        }

        /// <summary>
        /// *****************路由器认证*************
        /// </summary>
        /// <param name="username">admin</param>
        /// <param name="password">password</param>
        /// <returns></returns>
        public async Task<Dictionary<string,string>> Authenticate(string username, string password)
        {
            Dictionary<string, string> param = new Dictionary<string,string>();
            param.Add("NewUsername", username);
            param.Add("NewPassword", password);
            string retParam = await postSoap("ParentalControl", "Authenticate", 5000, param);
            return util.TraverseXML(retParam);
        }

        /// <summary>
        /// *********************GetAttachDevice*************
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string,Dictionary<string,string>>> GetAttachDevice()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            string retParam = "";
            Dictionary<string,Dictionary<string,string>> dicAttachDevice = new Dictionary<string,Dictionary<string,string>>();
            retParam = await postSoap("DeviceInfo", "GetAttachDevice", 5000, param);
            if (retParam != null)
            {
               retParam = util.SubString(retParam, "<NewAttachDevice>", "</NewAttachDevice>");
               string[] tempArray = retParam.Split('@');
               string macAdr = ""; 
               for (int i = 1; i < tempArray.Length; i++)
               {
                   if(tempArray[i] != "" && tempArray[i] != null)
                   {
                       Dictionary<string, string> dicRow = new Dictionary<string, string>();
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
                       if (macAdr != "")
                       {
                           System.Diagnostics.Debug.WriteLine(macAdr);
                           dicAttachDevice.Add(macAdr, dicRow);
                       }
                   }
               }
            }
            return  dicAttachDevice;
        }
        /// <summary>
        /// ************GetInfo*********Get router basic information*****************
        /// </summary>
        /// <param name="module">WLANConfiguration||DeviceInfo</param>
        /// <returns></returns>
        public async Task<Dictionary<string,string>> GetInfo(string module)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            string result = await postSoap(module, "GetInfo", 5000, param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// **************GetWPASecurityKeys*************
        /// 获取WiFi密码
        /// </summary>
        /// <returns>NewWPAPassphrase</returns>
        public async Task<Dictionary<string,string>> GetWPASecurityKeys()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            string result = await postSoap("WLANConfiguration", "GetWPASecurityKeys", 5000, param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// -----------安全类型为None时调用--------
        /// **************SetWLANNoSecurity*****参数格式*****************
        /// SetWLANWEPByPassphrase("4500v2-2G", "US", "9", "217Mbps");
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="region"></param>
        /// <param name="channel"></param>
        /// <param name="wirelessMode"></param>
        public async Task<Dictionary<string, string>> SetWLANNoSecurity(string ssid, string region, string channel, string wirelessMode)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewSSID",ssid);
            param.Add("NewRegion",region);
            param.Add("NewChannel",channel);
            param.Add("NewWirelessMode",wirelessMode);
            string result = await postSoap("WLANConfiguration", "SetWLANNoSecurity", 5000, param);
            return util.TraverseXML(result);
        }

        //////**************参数格式**************
        /// <summary>
        /// 设置Wirless Setting
        /// 说明：安全类型为"WPA-PSK/WPA2-PSK"或者"WPA2-PSK"时调用
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="region"></param>
        /// <param name="channel"></param>
        /// <param name="wirelessMode"></param>
        /// <param name="encryptionModes"></param>
        /// <param name="wpaPassphrase"></param>
        /// SetWLANWEPByPassphrase("4500v2-2G", "US", "9", "217Mbps", "WPA-PSK/WPA2-PSK", "siteview");
        public async Task<Dictionary<string, string>> SetWLANWEPByPassphrase(string ssid, string region, string channel, string wirelessMode, string encryptionModes, string wpaPassphrase)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewSSID", ssid);
            param.Add("NewRegion", region);
            param.Add("NewChannel", channel);
            param.Add("NewWirelessMode", wirelessMode);
            param.Add("NewWPAEncryptionModes", encryptionModes);
            param.Add("NewWPAPassphrase", wpaPassphrase);
            string result = await postSoap("WLANConfiguration", "SetWLANWEPByPassphrase", 5000, param);
            return util.TraverseXML(result);
        }

        ////------------------------------------------ ParentalControl Soap api-----------------------------------------------------------
        /// <summary>
        /// ****************** GetDNSMasqDeviceID ******************
        /// 获得<NewDeviceID>0000111111111111</NewDeviceID> 
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetDNSMasqDeviceID(string newMACAddress = "default")
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewMACAddress", newMACAddress);
            string result = await postSoap("ParentalControl", "GetDNSMasqDeviceID", 5000, param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// *********************SetDNSMasqDeviceID**************
        /// </summary>
        /// <param name="macAddress">default</param>
        /// <param name="deviceID"></param>
        /// <returns>0: Success;1: Maxed out (can’t add more);2: Error;401:  Not authenticated;</returns>
        public async Task<Dictionary<string,string>> SetDNSMasqDeviceID(string macAddress,string deviceID)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewMACAddress",macAddress);
            param.Add("NewDeviceID", deviceID);
            string result = await postSoap("ParentalControl","SetDNSMasqDeviceID",5000,param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// ***********************GetEnableStatus***********
        /// 说明：检测路由器是否支持家长控制
        ///
        /// </summary>
        public async Task<Dictionary<string,string>> GetEnableStatus()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            string result = await postSoap("ParentalControl", "GetEnableStatus", 5000, param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// ******************EnableParentalControl*************
        /// 开启或关闭家长控制
        /// newEnable :1(enable),0(disable)
        /// </summary>
        /// <param name="newEnable"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> EnableParentalControl(string newEnable)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewEnable",newEnable);
            string result = await postSoap("ParentalControl", "EnableParentalControl", 5000, param);
            return util.TraverseXML(result);
        }

        public async Task<Dictionary<string, string>> DeleteMACAddress(string macAddress)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewMACAddress",macAddress);
            string result = await postSoap("ParentalControl", "DeleteMACAddress", 5000, param);
            return util.TraverseXML(result);
        }

        ////-----------------------------------------Guest Acccess Soap API---------------------------------------------------------
        /// <summary>
        /// *******************GetGuestAccessEnabled*******
        /// check router support Guest access
        /// </summary>
        /// <returns>NewGuestAccessEnabled：1（enable）,0(not enable)</returns>
        public async Task<Dictionary<string, string>> GetGuestAccessEnabled()
        {
            string result = await postSoap("WLANConfiguration", "GetGuestAccessEnabled",5000,new Dictionary<string,string>());
            return util.TraverseXML(result);
        }

        /// <summary>
        /// *********************GetGuestAccessNetworkInfo***********
        /// 获得访客网络信息
        /// </summary>
        /// <returns> <NewSSID>NETGEAR-Guest</NewSSID>
        //<NewSecurityMode>None</NewSecurityMode>
        //<NewKey>密码</NewKey></returns>
        public async Task<Dictionary<string, string>> GetGuestAccessNetworkInfo()
        {
            Dictionary<string, string> tempResult = new Dictionary<string, string>();
            tempResult = await GetGuestAccessEnabled();
            if(int.Parse(tempResult["ResponseCode"])!=0)
            {
                return new Dictionary<string,string>();
            }
          
            if (int.Parse(tempResult["NewGuestAccessEnabled"]) == 1)
            {
                string result = await postSoap("WLANConfiguration", "GetGuestAccessNetworkInfo", 5000, new Dictionary<string, string>());
                return util.TraverseXML(result);
            }
            tempResult.Clear();
            tempResult.Add("ResponseCode","-1");
            return tempResult;
        }

        /// <summary>
        /// *************SetGuestAccessEnabled*************
        /// 禁用访客访问
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> SetGuestAccessEnabled()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("newGuestAccessEnabled","0");
            string result = await postSoap("WLANConfiguration", "SetGuestAccessEnabled", 5000, param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// *********************SetGuestAccessEnabled2***************
        /// 条件：访客访问网络已被禁用
        /// 功能：启用访客访问并设置访客网络信息
        /// </summary>
        /// <param name="newSSID"></param>
        /// <param name="newSecurityMode"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> SetGuestAccessEnabled2(string newSSID, string newSecurityMode, string newKey)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewSSID",newSSID);
            param.Add("NewSecurityMode", newSecurityMode);
            param.Add("NewKey1", newKey);
            param.Add("NewKey2", "0");
            param.Add("NewKey3", "0");
            param.Add("NewKey4", "0");
            param.Add("NewGuestAccessEnabled", "1");
            string result = await postSoap("WLANConfiguration", "SetGuestAccessEnabled2", 5000, param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// ********************SetGuestAccessNetwork*********************
        /// 条件：访客网络已启用
        /// 当访客访问没有被禁用时修改访客网络信息
        /// </summary>
        /// <param name="newSSID"></param>
        /// <param name="newSecurityMode"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> SetGuestAccessNetwork(string newSSID, string newSecurityMode, string newKey)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewSSID", newSSID);
            param.Add("NewSecurityMode", newSecurityMode);
            param.Add("NewKey1", newKey);
            param.Add("NewKey2", "0");
            param.Add("NewKey3", "0");
            param.Add("NewKey4", "0");
            string result = await postSoap("WLANConfiguration", "SetGuestAccessNetwork", 5000, param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// *****************EnableTrafficMeter api*******************
        /// 启用或者禁用流量控制
        /// ResponseCode:0 (no error), 1 (reboot required)
        /// newTrafficMeterEnable = 1 （enable）
        /// newTrafficMeterEnable = 0 （disable）
        /// </summary>
        /// <param name="newTrafficMeterEnable"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> EnableTrafficMeter(string newTrafficMeterEnable)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewTrafficMeterEnable",newTrafficMeterEnable);
            string result = await postSoap("DeviceConfig", "EnableTrafficMeter",5000,param);
            return util.TraverseXML(result);

        }

        /// <summary>
        /// 获取当前路由器的流量控制功能状态
        /// NewTrafficMeterEnable：1（enabled），0（disabled），2（Traffic Meter service is not supported）
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetTrafficMeterEnabled()
        {
            string result = await postSoap("DeviceConfig", "GetTrafficMeterEnabled",5000,new Dictionary<string,string>());
            return util.TraverseXML(result);
        }

        /// <summary>
        /// **************GetTrafficMeterOptions api****** return soap value***********************
        ///   <m:GetTrafficMeterOptionsResponse
        //xmlns:m="urn:NETGEAT-ROUTER:service:DeviceConfig:1">
        //    <NewControlOption>No limit</NewControlOption>
        //    <NewMonthlyLimit>0</NewMonthlyLimit>
        //    <RestartHour>00</RestartHour>
        //    <RestartMinute>00</RestartMinute>
        //    <RestartDay>1</RestartDay>
        //</m:GetTrafficMeterOptionsResponse>
        /// </summary>
        /// <returns></returns>
        /// 
        public async Task<Dictionary<string, string>> GetTrafficMeterOptions()
        {
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            string result = await postSoap("DeviceConfig", "GetTrafficMeterOptions",5000,new Dictionary<string,string>());
            resultDic = util.TraverseXML(result);
            return resultDic;
        }

        /// <summary>
        ///**********GetTrafficMeterStatistics soap api return result***************************
        ///获得路由器流量信息
        ///   <m:GetTrafficMeterStatisticsResponse
        //    xmlns:m="urn:NETGEAT-ROUTER:service:DeviceConfig:1">
        //    <NewTodayConnectionTime>--:--</NewTodayConnectionTime>
        //    <NewTodayUpload>0.10</NewTodayUpload>
        //    <NewTodayDownload>0.50</NewTodayDownload>
        //    <NewYesterdayConnectionTime>--:--</NewYesterdayConnectionTime>
        //    <NewYesterdayUpload>0.00</NewYesterdayUpload>
        //    <NewYesterdayDownload>0.00</NewYesterdayDownload>
        //    <NewWeekConnectionTime>--:--</NewWeekConnectionTime>
        //    <NewWeekUpload>0.10/0.01</NewWeekUpload>
        //    <NewWeekDownload>0.50/0.07</NewWeekDownload>
        //    <NewMonthConnectionTime>--:--</NewMonthConnectionTime>
        //    <NewMonthUpload>0.10/0.00</NewMonthUpload>
        //    <NewMonthDownload>0.50/0.02</NewMonthDownload>
        //    <NewLastMonthConnectionTime>--:--</NewLastMonthConnectionTime>
        //    <NewLastMonthUpload>1628/54.29</NewLastMonthUpload>
        //    <NewLastMonthDownload>4887/162.91</NewLastMonthDownload>
        //</m:GetTrafficMeterStatisticsResponse>
        //<ResponseCode>000</ResponseCode
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetTrafficMeterStatistics()
        {
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            string result = await postSoap("DeviceConfig", "GetTrafficMeterStatistics", 5000, new Dictionary<string, string>());
            resultDic = util.TraverseXML(result);
            return resultDic;
        }

        /// <summary>
        /// **************** SetTrafficMeterOptions api**************
        ///  ResponseCode:0 (no error), 1 (reboot required)
        /// </summary>
        /// <param name="newControlOption">No Limit|Download only|Both directions</param>
        /// <param name="newMonthlyLimit">Integer, <=1000000 (Mbps)</param>
        /// <param name="restartHour">0~24</param>
        /// <param name="restartMinute">0~60</param>
        /// <param name="restartDay">1~31</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> SetTrafficMeterOptions(string newControlOption,string newMonthlyLimit,string restartHour,string restartMinute, string restartDay)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewControlOption",newControlOption);
            param.Add("NewMonthlyLimit",newMonthlyLimit);
            param.Add("RestartHour",restartHour);
            param.Add("RestartMinute",restartMinute);
            param.Add("RestartDay",restartDay);
            string result = await postSoap("DeviceConfig", "SetTrafficMeterOptions",5000,param);
            return util.TraverseXML(result);
        }



        /// <summary>
        ///    *************** send soap api***************
        /// </summary>
        /// <param name="module"></param>
        /// <param name="method"></param>
        /// <param name="port"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<string> postSoap(string module, string method, int port, Dictionary<string,string> param)
        {
            if (isSetApi(method))
            {
                ConfigurationStarted(5000);
            }
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
                   // string paraString = " xsi:type=\"xsd:string\" xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\"";
                    s_para = "";
;                    if(param.Count > 0)
                    {
                         foreach(KeyValuePair<string, string> kv in param)
                         {
                             s_para += string.Format("<{0}>{1}</{0}>\n",kv.Key,kv.Value);
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
                    HttpStatusCode statusCode = response.StatusCode;
                    if (statusCode == HttpStatusCode.OK)
                    {
                        retOK = true;
                    }
                    else
                    {
                        retOK = false;
                    }
                    System.Diagnostics.Debug.WriteLine(resultstr);
                    if (retOK && isSetApi(method))
                    {
                        ConfigurationFinished(5000);
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
        /// ***************ConfigurationStarted***********
        /// SOAP api 设置开始
        /// </summary>
        public async void ConfigurationStarted(int port)
        {
             //Dictionary<string, string> param = new Dictionary<string,string>();
             //await postSoap("DeviceConfig", "ConfigurationStarted", 5000, param);
            string resourceAddress = string.Format("http://routerlogin.com:{0}/soap/server_sa", port);
            string soapAction = string.Format("urn:NETGEAR-ROUTER:service:DeviceConfig:1#ConfigurationStarted");

            string soapBodyMode = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>"
           + "<SOAP-ENV:Envelope xmlns:SOAPSDK1=\"http://www.w3.org/2001/XMLSchema\" "
            + "xmlns:SOAPSDK2=\"http://www.w3.org/2001/XMLSchema-instance\" "
             + "xmlns:SOAPSDK3=\"http://schemas.xmlsoap.org/soap/encoding/\" "
              + "xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">"
               + "<SOAP-ENV:Header>"
                + "<SessionID>58DEE6006A88A967E89A</SessionID>"
                 + "</SOAP-ENV:Header>"
                  + "<SOAP-ENV:Body>"
                   + "<M1:ConfigurationStarted xmlns:M1=\"urn:NETGEAR-ROUTER:service:DeviceConfig:1\">"
                        + "<NewSessionID>58DEE6006A88A967E89A</NewSessionID>"
                   + "</M1:ConfigurationStarted>"
                 + "</SOAP-ENV:Body>"
           + "</SOAP-ENV:Envelope>";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, resourceAddress);
            StringContent soapContent = new StringContent(soapBodyMode, Encoding.UTF8, "text/xml");
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

                // return resultstr;
            }
            catch (HttpRequestException hre)
            {
                // return "";
            }
            catch (TaskCanceledException hce)
            {
                //   return "";
            }
            catch (Exception ex)
            {
                // return "";
            }
        }

        /// <summary>
        /// ***************ConfigurationFinished***********
        /// soap api 完成设置
        /// </summary>
        public async void ConfigurationFinished(int port)
        {
            //Dictionary<string, string> param = new Dictionary<string, string>();
            //param.Add("NewStatus", "ChangesApplied");
            //await postSoap("DeviceConfig", "ConfigurationFinished", 5000, param);
            string resourceAddress = string.Format("http://routerlogin.com:{0}/soap/server_sa", port);
            string soapAction = string.Format("urn:NETGEAR-ROUTER:service:DeviceConfig:1#ConfigurationFinished");

            string soapBodyMode = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>"
           + "<SOAP-ENV:Envelope xmlns:SOAPSDK1=\"http://www.w3.org/2001/XMLSchema\" "
            + "xmlns:SOAPSDK2=\"http://www.w3.org/2001/XMLSchema-instance\" "
             + "xmlns:SOAPSDK3=\"http://schemas.xmlsoap.org/soap/encoding/\" "
              + "xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">"
               + "<SOAP-ENV:Header>"
                + "<SessionID>58DEE6006A88A967E89A</SessionID>"
                 + "</SOAP-ENV:Header>"
                  + "<SOAP-ENV:Body>"
                   + "<M1:ConfigurationFinished xmlns:M1=\"urn:NETGEAR-ROUTER:service:DeviceConfig:1\">"
                        + "<NewStatus>ChangesApplied</NewStatus>"
                   + "</M1:ConfigurationFinished>"
                 + "</SOAP-ENV:Body>"
           + "</SOAP-ENV:Envelope>";
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, resourceAddress);
            StringContent soapContent = new StringContent(soapBodyMode, Encoding.UTF8, "text/xml");
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

                // return resultstr;
            }
            catch (HttpRequestException hre)
            {
                // return "";
            }
            catch (TaskCanceledException hce)
            {
                //   return "";
            }
            catch (Exception ex)
            {
                // return "";
            }
        }

        /// <summary>
        /// 判断Genie SOAP api 是否进行设置
        /// </summary>
        /// <param name="method"></param>
        /// <returns></returns>
        public bool isSetApi(string method)
        {
            Regex rgx = new Regex ("^Set");
            Regex regex = new Regex("^Enable");
            return (rgx.IsMatch(method) || regex.IsMatch(method) 
                || method.Contains("UpdateNewFirmware") 
                || method.Contains("PressWPSPBC")
                || method.Contains("Reboot")
                    );
        }

        /// <summary>
        /// 判断是否为netgear路由器，并获得路由器的一些信息
        /// </summary>
        /// <returns>Firmware=V1.0.1.20_1.0.40 RegionTag=WNDR4500_NA Region=us Model=WNDR4500 InternetConnectionStatus=Up ParentalControlSupported=1 SOAPVersion=1.15 ReadyShareSupportedLevel=7</returns>
        public async Task<Dictionary<string, string>> GetCurrentSetting()
        {
            Dictionary<string, string> resultDic = new Dictionary<string, string>();
            string url = "http://routerlogin.net/currentsetting.htm";
            string resultStr ;
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, url);
           
                HttpResponseMessage response = await httpClient.SendAsync(request);
               byte[] resultbt = await response.Content.ReadAsByteArrayAsync();
                
                resultStr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
                if (resultStr.Length > 0)
                {
                    string[] split_r_n = resultStr.Split(new []{"\r\n"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach(string item in split_r_n)
                    {
                        string[] splitValue = item.Split(new []{"="}, StringSplitOptions.RemoveEmptyEntries);
                        resultDic.Add(splitValue[0],splitValue[1]);
                    }
                }
              
                System.Diagnostics.Debug.WriteLine(resultStr);
            }
            catch (HttpRequestException hre)
            {
                
            }
            catch (TaskCanceledException hce)
            {
                
            }
            catch (Exception ex)
            {
                
            }
            
            return resultDic;
        }

        
    }
}
