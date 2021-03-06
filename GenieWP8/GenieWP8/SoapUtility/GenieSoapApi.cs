﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
//using Windows.Data.Xml.Dom;


namespace GenieWP8
{
    class GenieSoapApi
    {
        HttpClientHandler handler;
        HttpClient httpClient;
        UtilityTool util;
        bool retOK;///发送soap是否返回200 OK
       
        public enum _InnerFlags
        {
            NONE,
            IS_CGDG = 1 << 0,
            IS_5000PORT = 1 << 1
        };
        public static _InnerFlags flag = _InnerFlags.NONE;
        public GenieSoapApi()
        {
            handler = new HttpClientHandler();
            httpClient = new HttpClient(handler);
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
            try
            {
                string retParam = await postSoap("ParentalControl", "Authenticate", param);
                return util.TraverseXML(retParam);
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:Authenticate failed! press 'OK' to retry.");
                return null;
            }
        }

        /// <summary>
        /// *********************GetAttachDevice*************
        /// 返回结果以MacAddress作为Dictionary的key
        /// </summary>
        /// <returns></returns>
        //public async Task<Dictionary<string,Dictionary<string,string>>> GetAttachDevice()
        public async Task<List<List<string>>> GetAttachDevice()
        {
            //Dictionary<string, string> param = new Dictionary<string, string>();
            //string retParam = "";
            //Dictionary<string,Dictionary<string,string>> dicAttachDevice = new Dictionary<string,Dictionary<string,string>>();
            //try
            //{
            //    retParam = await postSoap("DeviceInfo", "GetAttachDevice", param);
            //    if (retParam != null)
            //    {
            //        retParam = util.SubString(retParam, "<NewAttachDevice>", "</NewAttachDevice>");
            //        string[] tempArray = retParam.Split('@');
            //        string macAdr = "";
            //        for (int i = 1; i < tempArray.Length; i++)
            //        {
            //            if (tempArray[i] != "" && tempArray[i] != null)
            //            {
            //                Dictionary<string, string> dicRow = new Dictionary<string, string>();
            //                string[] itemArray = tempArray[i].Split(';');
            //                if (itemArray.Length >= 4)
            //                {
            //                    dicRow.Add("Ip", itemArray[1]);
            //                    dicRow.Add("HostName", itemArray[2]);
            //                    macAdr = itemArray[3];


            //                }
            //                if (itemArray.Length >= 5)
            //                {
            //                    dicRow.Add("Connect", itemArray[4]);
            //                }
            //                if (itemArray.Length >= 7)
            //                {
            //                    dicRow.Add("LinkSpeed", itemArray[5]);
            //                    dicRow.Add("Signal", itemArray[6]);
            //                }
            //                if (itemArray.Length >= 8)
            //                {
            //                    dicRow.Add("AccessControl", itemArray[7]);
            //                }
            //                if (macAdr != "")
            //                {
            //                    System.Diagnostics.Debug.WriteLine(macAdr);
            //                    dicAttachDevice.Add(macAdr, dicRow);
            //                }
            //            }
            //        }
            //    }
            //    return dicAttachDevice;
            //}
            //catch (System.Exception ex)
            //{
            //    //MessageBox.Show("API:GetAttachDevice failed! please retry.");
            //    return null;
            //}
            Dictionary<string, string> param = new Dictionary<string, string>();
            string retParam = "";
            List<List<string>> dicAttachDevice = new List<List<string>>();
            try
            {
                retParam = await postSoap("DeviceInfo", "GetAttachDevice", param);
                if (retParam != null)
                {
                    retParam = util.SubString(retParam, "<NewAttachDevice>", "</NewAttachDevice>");
                    string[] tempArray = retParam.Split('@');
                    string macAdr = "";
                    for (int i = 1; i < tempArray.Length; i++)
                    {
                        if (tempArray[i] != "" && tempArray[i] != null)
                        {
                            List<string> dicRow = new List<string>();
                            string[] itemArray = tempArray[i].Split(';');
                            if (itemArray.Length >= 4)
                            {
                                dicRow.Add(itemArray[1]);           //Ip
                                dicRow.Add(itemArray[2]);           //HostName
                                dicRow.Add(itemArray[3]);           //MacAddress
                                macAdr = itemArray[3];
                            }
                            if (itemArray.Length >= 5)
                            {
                                dicRow.Add(itemArray[4]);            //Connect
                            }
                            if (itemArray.Length >= 7)
                            {
                                dicRow.Add(itemArray[5]);           //LinkSpeed
                                dicRow.Add(itemArray[6]);           //Signal
                            }
                            if (itemArray.Length >= 8)
                            {
                                dicRow.Add(itemArray[7]);           //AccessControl
                            }
                            if (macAdr != "")
                            {
                                System.Diagnostics.Debug.WriteLine(macAdr);
                                dicAttachDevice.Add(dicRow);
                            }
                        }
                    }
                }
                return dicAttachDevice;
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:GetAttachDevice failed! please retry.");
                return null;
            }
        }
        /// <summary>
        /// ************GetInfo*********Get router basic information*****************
        /// </summary>
        /// <param name="module">WLANConfiguration||DeviceInfo</param>
        /// <returns></returns>
        public async Task<Dictionary<string,string>> GetInfo(string module)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            try
            {
                string result = await postSoap(module, "GetInfo", param);
                return util.TraverseXML(result);
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:GetInfo failed! please retry.");
                return null;
            }            
        }

        /// <summary>
        /// **************GetWPASecurityKeys*************
        /// 获取WiFi密码
        /// </summary>
        /// <returns>NewWPAPassphrase</returns>
        public async Task<Dictionary<string,string>> GetWPASecurityKeys()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            try
            {
                string result = await postSoap("WLANConfiguration", "GetWPASecurityKeys", param);
                return util.TraverseXML(result);
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:GetWPASecurityKeys failed! please retry.");
                return null;
            }           
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
            try
            {
                string result = await postSoap("WLANConfiguration", "SetWLANNoSecurity", param);
                return util.TraverseXML(result);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:SetWLANNoSecurity failed! please retry.");
                return null;
            }

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
            try
            {
                string result = await postSoap("WLANConfiguration", "SetWLANWEPByPassphrase", param);
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:SetWLANWEPByPassphrase failed! please retry.");
                return null;
            }
        }

        //***************************** Wifi setting 5G api********************************

        /// <summary>
        /// 判断路由器是否支持5G
        /// 返回结果：New5GSupported：0(不支持5G),1(支持5G)
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Is5GSupported()
        {
            string result = await postSoap("WLANConfiguration", "Is5GSupported", new Dictionary<string, string>());
            return util.TraverseXML(result);
        }

        /// <summary>
        /// 获取5G信息
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Get5GInfo()
        {
            string result = await postSoap("WLANConfiguration", "Get5GInfo", new Dictionary<string, string>());
            return util.TraverseXML(result);
        }

        /// <summary>
        /// 获取5G的安全密码
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Get5GWPASecurityKeys()
        {
            string result = await postSoap("WLANConfiguration", "Get5GWPASecurityKeys",new Dictionary<string,string> ());
            return util.TraverseXML(result);
        }

        /// <summary>
        /// -----------安全类型为None时调用--------
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="region"></param>
        /// <param name="channel"></param>
        /// <param name="wirelessMode"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Set5GWLANNoSecurity(string ssid, string region, string channel, string wirelessMode)
        {
            Dictionary<string,string> param = new Dictionary<string,string> ();
            param.Add("NewSSID", ssid);
            param.Add("NewRegion", region);
            param.Add("NewChannel", channel);
            param.Add("NewWirelessMode", wirelessMode);
            string result = await postSoap("WLANConfiguration", "Set5GWLANNoSecurity", param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// 5G wifi 设置
        /// 安全类型为"WPA-PSK/WPA2-PSK"或者"WPA2-PSK"时调用
        /// </summary>
        /// <param name="ssid"></param>
        /// <param name="region"></param>
        /// <param name="channel">Auto, 36, 40, 44, 48, 149, 153, 157, 161</param>
        /// <param name="wirelessMode"></param>
        /// <param name="encryptionModes"></param>
        /// <param name="wpaPassphrase"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Set5GWLANWPAPSKByPassphrase(string ssid, string region, string channel, string wirelessMode, string encryptionModes, string wpaPassphrase)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewSSID", ssid);
            param.Add("NewRegion", region);
            param.Add("NewChannel", channel);
            param.Add("NewWirelessMode", wirelessMode);
            param.Add("NewWPAEncryptionModes", encryptionModes);
            param.Add("NewWPAPassphrase", wpaPassphrase);
            string result = await postSoap("WLANConfiguration", "Set5GWLANWPAPSKByPassphrase", param);
            return util.TraverseXML(result);
        }
        ////------------------------------------------ ParentalControl Soap api-----------------------------------------------------------
        /// <summary>
        /// ****************** GetDNSMasqDeviceID ******************
        /// 获得<NewDeviceID>0000EE04911BF768</NewDeviceID> 
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetDNSMasqDeviceID(string newMACAddress = "default")
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewMACAddress", newMACAddress);
            try
            {
                string result = await postSoap("ParentalControl", "GetDNSMasqDeviceID", param);
                return util.TraverseXML(result);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:GetDNSMasqDeviceID failed! please retry.");
                return null;
            }            
        }

        /// <summary>
        /// *********************SetDNSMasqDeviceID**************
        /// 将本机mac地址与已登录的bypass账号进行绑定
        /// 其中：deviceID 为 childDeviceID
        /// </summary>
        /// <param name="macAddress">default</param>
        /// <param name="deviceID">childDeviceID</param>
        /// <returns>0: Success;1: Maxed out (can’t add more);2: Error;401:  Not authenticated;</returns>
        public async Task<Dictionary<string,string>> SetDNSMasqDeviceID(string macAddress,string deviceID)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewMACAddress",macAddress);
            param.Add("NewDeviceID", deviceID);
            try
            {
                string result = await postSoap("ParentalControl", "SetDNSMasqDeviceID", param);
                return util.TraverseXML(result);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:SetDNSMasqDeviceID failed! please retry.");
                return null;
            }           
        }

        /// <summary>
        /// ***********************GetEnableStatus***********
        /// 说明：检测路由器是否开启家长控制
        ///<ParentalControl>0</ParentalControl> 1：已开启
        /// </summary>
        public async Task<Dictionary<string,string>> GetEnableStatus()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            try
            {
                string result = await postSoap("ParentalControl", "GetEnableStatus", param);
                return util.TraverseXML(result);
            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:GetEnableStatus failed! please retry.");
                return null;
            }            
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
            try
            {
                string result = await postSoap("ParentalControl", "EnableParentalControl", param);
                return util.TraverseXML(result);
            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:EnableParentalControl failed! please retry.");
                return null;
            }            
        }

        /// <summary>
        /// bypass 账号注销
        /// </summary>
        /// <param name="macAddress">本机的mac地址</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> DeleteMACAddress(string macAddress)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewMACAddress",macAddress);
            try
            {
                string result = await postSoap("ParentalControl", "DeleteMACAddress", param);
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:DeleteMACAddress failed! please retry.");
                return null;
            }
        }

        ////-----------------------------------------Guest Acccess Soap API---------------------------------------------------------
        /// <summary>
        /// *******************GetGuestAccessEnabled*******
        /// check router support Guest access
        /// </summary>
        /// <returns>NewGuestAccessEnabled：1（enable）,0(not enable)</returns>
        public async Task<Dictionary<string, string>> GetGuestAccessEnabled()
        {
            try
            {
                string result = await postSoap("WLANConfiguration", "GetGuestAccessEnabled", new Dictionary<string, string>());
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:GetGuestAccessEnabled failed! please retry.");
                return null;
            }
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
            try
            {
                string result = await postSoap("WLANConfiguration", "GetGuestAccessNetworkInfo", new Dictionary<string, string>());
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:GetGuestAccessNetworkInfo failed! please retry.");
                return null;
            }
        }

        /// <summary>
        /// *************SetGuestAccessEnabled*************
        /// 禁用访客访问
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> SetGuestAccessEnabled()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewGuestAccessEnabled", "0");
            try
            {
                string result = await postSoap("WLANConfiguration", "SetGuestAccessEnabled", param);
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:SetGuestAccessEnabled failed! please retry.");
                return null;
            }
        }

        /// <summary>
        /// *********************SetGuestAccessEnabled2***************
        /// 条件：访客访问网络已被禁用
        /// 功能：启用访客访问并设置访客网络信息
        /// </summary>
        /// <param name="newSSID"></param>
        /// <param name="newSecurityMode">None||WPA-PSK/WPA2-PSK||WPA2-PSK</param>
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
            try
            {
                string result = await postSoap("WLANConfiguration", "SetGuestAccessEnabled2", param);
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:SetGuestAccessEnabled2 failed! please retry.");
                return null;
            }
        }

        /// <summary>
        /// ********************SetGuestAccessNetwork*********************
        /// 条件：访客网络已启用
        /// 当访客访问没有被禁用时修改访客网络信息
        /// </summary>
        /// <param name="newSSID"></param>
        /// <param name="newSecurityMode">None||WPA-PSK/WPA2-PSK||WPA2-PSK</param>
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
            try
            {
                string result = await postSoap("WLANConfiguration", "SetGuestAccessNetwork", param);
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:SetGuestAccessNetwork failed! please retry.");
                return null;
            }
        }

        //************************************ 5G Guest Access api *****************************************

        /// <summary>
        /// If 5GHz Guest Access is enabled, then return NewGuestAccessEnabled as 1, if not, return 0. If Guest Access is not supported (no multi-SSID support), then return 2.
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Get5GGuestAccessEnabled()
        {
            string result = await postSoap("WLANConfiguration", "Get5GGuestAccessEnabled", new Dictionary<string, string>());
            return util.TraverseXML(result);
        }

        /// <summary>
        /// 获取5G访客网络信息
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Get5GGuestAccessNetworkInfo()
        {
            string result = await postSoap("WLANConfiguration", "Get5GGuestAccessNetworkInfo",new Dictionary<string,string> ());
            return util.TraverseXML(result);
        }

        /// <summary>
        /// 禁用5G访客访问功能
        /// </summary>
        /// <param name="newGuestAccessEnabled"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Set5GGuestAccessEnabled()
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewGuestAccessEnabled", "0");
            string result = await postSoap("WLANConfiguration", "Set5GGuestAccessEnabled",param);
            return util.TraverseXML(result);
        }

        /// <summary>
        /// 条件：5G访客访问网络已被禁用
        /// 功能：启用5G访客访问并设置访客网络信息
        /// </summary>
        /// <param name="newSSID"></param>
        /// <param name="newSecurityMode"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Set5GGuestAccessEnabled2(string newSSID,string newSecurityMode,string newKey)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewSSID", newSSID);
            param.Add("NewSecurityMode", newSecurityMode);
            param.Add("NewKey1", newKey);
            param.Add("NewKey2", "");
            param.Add("NewKey3", "");
            param.Add("NewKey4", "");
            param.Add("NewGuestAccessEnabled", "1");
            string result = await postSoap("WLANConfiguration", "Set5GGuestAccessEnabled2", param);
            return util.TraverseXML(result);
        }

        /// <summary>
        ///  条件：5G访客网络已启用
        /// 当访客访问没有被禁用时修改访客网络信息
        /// </summary>
        /// <param name="newSSID"></param>
        /// <param name="newSecurityMode"></param>
        /// <param name="newKey"></param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> Set5GGuestAccessNetwork(string newSSID, string newSecurityMode, string newKey)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewSSID", newSSID);
            param.Add("NewSecurityMode", newSecurityMode);
            param.Add("NewKey1", newKey);
            param.Add("NewKey2", "");
            param.Add("NewKey3", "");
            param.Add("NewKey4", "");
            string result = await postSoap("WLANConfiguration", "Set5GGuestAccessEnabled2", param);
            return util.TraverseXML(result);
        }

        ////------------------------------ TrafficMeter Control soap api ----------------------------------
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
            try
            {
                string result = await postSoap("DeviceConfig", "EnableTrafficMeter", param);
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:EnableTrafficMeter failed! please retry.");
                return null;
            }
        }

        /// <summary>
        /// 获取当前路由器的流量控制功能状态
        /// NewTrafficMeterEnable：1（enabled），0（disabled），2（Traffic Meter service is not supported）
        /// </summary>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> GetTrafficMeterEnabled()
        {
            try
            {
                string result = await postSoap("DeviceConfig", "GetTrafficMeterEnabled", new Dictionary<string, string>());
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:GetTrafficMeterEnabled failed! please retry.");
                return null;
            }
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
            try
            {
                string result = await postSoap("DeviceConfig", "GetTrafficMeterOptions", new Dictionary<string, string>());
                resultDic = util.TraverseXML(result);
                return resultDic;

            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:GetTrafficMeterOptions failed! please retry.");
                return null;
            }
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
            try
            {
                string result = await postSoap("DeviceConfig", "GetTrafficMeterStatistics", new Dictionary<string, string>());
                resultDic = util.TraverseXML(result);
                return resultDic;

            }
            catch (System.Exception ex)
            {
                //MessageBox.Show("API:GetTrafficMeterStatistics failed! please retry.");
                return null;
            }
        }

        /// <summary>
        /// **************** SetTrafficMeterOptions api**************
        ///  ResponseCode:0 (no error), 1 (reboot required)
        /// </summary>
        /// <param name="newControlOption">No limit|Download only|Both directions</param>
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
            try
            {
                string result = await postSoap("DeviceConfig", "SetTrafficMeterOptions", param);
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:SetTrafficMeterOptions failed! please retry.");
                return null;
            }
        }

        ///********************************** 访问控制 api **********************************


        ///返回结果：NewBlockDeviceEnable：1 (enabled)，0 (disabled)
        /// ResponseCode ：0 (no error)，1 (error)
        public async Task<Dictionary<string, string>> GetBlockDeviceEnableStatus()
        {
            try
            {
                string result = await postSoap("DeviceConfig", "GetBlockDeviceEnableStatus", new Dictionary<string, string>());
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:GetBlockDeviceEnableStatus failed! please retry.");
                return null;
            }
        }

        /// <summary>
        /// newBlockDeviceEnable:1 (enabled)，0 (disabled)
        /// </summary>
        /// <param name="newBlockDeviceEnable">1 || 0</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> SetBlockDeviceEnable(string newBlockDeviceEnable)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewBlockDeviceEnable",newBlockDeviceEnable);
            try
            {
                string result = await postSoap("DeviceConfig", "SetBlockDeviceEnable", param);
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:SetBlockDeviceEnable failed! please retry.");
                return null;
            }
        }

        /// <summary>
        /// 通过mac地址设置阻止或允许访问Internet
        /// 返回结果：0 (no error)，1 (error)，2 (invalid MAC address)，3 (invalid AllowOrBlock parameter)

        /// </summary>
        /// <param name="newMACAddress">(XX:XX:XX:XX:XX:XX format, or XXXXXXXXXXXX format)</param>
        /// <param name="newAllowOrBlock">"Allow"||"Block"</param>
        /// <returns></returns>
        public async Task<Dictionary<string, string>> SetBlockDeviceByMAC(string newMACAddress, string newAllowOrBlock)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("NewMACAddress", newMACAddress);
            param.Add("NewAllowOrBlock", newAllowOrBlock);
            try
            {
                string result = await postSoap("DeviceConfig", "SetBlockDeviceByMAC", param);
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:SetBlockDeviceByMAC failed! please retry.");
                return null;
            }
        }

        public async Task<Dictionary<string, string>> EnableBlockDeviceForAll()
        {
            try
            {
                string result = await postSoap("DeviceConfig", "EnableBlockDeviceForAll", new Dictionary<string, string>());
                return util.TraverseXML(result);

            }
            catch (System.Exception ex)
            {
                MessageBox.Show("API:EnableBlockDeviceForAll failed! please retry.");
                return null;
            }
        }

        /// <summary>
        /// 第一次调用SOAP api默认为80端口，如果不通，换5000端口
        /// </summary>
        /// <param name="module"></param>
        /// <param name="method"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<string> postSoap(string module, string method, Dictionary<string, string> param)
        {
            int port;
            if ((flag & _InnerFlags.IS_5000PORT)!=0)
            {
                port = 5000;
            }
            else
            {
                port = 80;
            }
            string retText =  await _postSoap(module,method,port,param);
            if (!retOK)
            {
                if ((flag & _InnerFlags.IS_5000PORT) != 0)
                {
                    port = 80;
                }
                else
                {
                    port = 5000;
                }
                retText = await _postSoap(module, method, port, param);
                if (retOK)
                {
                    flag = flag ^ _InnerFlags.IS_5000PORT;
                }
            }
            return retText;
        }


        /// <summary>
        ///    *************** send soap api***************
        /// </summary>
        /// <param name="module"></param>
        /// <param name="method"></param>
        /// <param name="port"></param>
        /// <param name="param"></param>
        /// <returns></returns>
        public async Task<string> _postSoap(string module, string method, int port, Dictionary<string,string> param)
        {
            if (isSetApi(method))
            {
                await ConfigurationStarted(port);
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
            

                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, new Uri(resourceAddress));
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
                    if(handler.SupportsTransferEncodingChunked())
                    {
                        request.Headers.TransferEncodingChunked = true;
                    }
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
                    if (retOK && isSetApi(method))
                    {
                        await ConfigurationFinished(port);
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
        public async Task<Dictionary<string,string>> ConfigurationStarted(int port)
        {
             Dictionary<string, string> param = new Dictionary<string,string>();
             //await postSoap("DeviceConfig", "ConfigurationStarted", 5000, param);
            string resourceAddress = string.Format("http://routerlogin.com:{0}/soap/server_sa", port);
            string soapAction = string.Format("urn:NETGEAR-ROUTER:service:DeviceConfig:1#ConfigurationStarted");

            string soapBodyMode = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>"
           + "<SOAP-ENV:Envelope xmlns:SOAPSDK1=\"http://www.w3.org/2001/XMLSchema\" "
            + "xmlns:SOAPSDK2=\"http://www.w3.org/2001/XMLSchema-instance\" "
             + "xmlns:SOAPSDK3=\"http://schemas.xmlsoap.org/soap/encoding/\" "
              + "xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">"
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
                param = util.TraverseXML(resultstr);
                return param;
            }
            catch (HttpRequestException hre)
            {
                return param;
            }
            catch (TaskCanceledException hce)
            {
                return param;
            }
            catch (Exception ex)
            {
                return param;
            }
        }

        /// <summary>
        /// ***************ConfigurationFinished***********
        /// soap api 完成设置
        /// </summary>
        public async Task<string> ConfigurationFinished(int port)
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
                resultStr = resultStr.Replace("\n", "");
                if (resultStr.Length > 0)
                {
                    string[] split_r_n = resultStr.Split(new []{"\r"}, StringSplitOptions.RemoveEmptyEntries);
                    foreach(string item in split_r_n)
                    {
                        string[] splitValue = item.Split('=');
                        if (splitValue[0] != null && splitValue[0] != "")
                        {
                            resultDic.Add(splitValue[0], splitValue[1]);
                        }
                    }
                }
                if (resultDic.Count > 0)
                {
                    setRouteType(resultDic["Model"]);
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

        public void setRouteType(string routerType)
        {
            Regex rgcg = new Regex("^CG");
            Regex regdg = new Regex("^DG");
            string[] strType = { "WNDR3400", "WNDR4500" };
            if (rgcg.IsMatch(routerType) || regdg.IsMatch(routerType))
            {
                flag |= _InnerFlags.IS_CGDG;
            }
            for (int i = 0; i < strType.Length; i++)
            {
                if (routerType.Contains(strType[i]))
                {
                    flag |= _InnerFlags.IS_5000PORT;
                }
            }
        }
    }
}
