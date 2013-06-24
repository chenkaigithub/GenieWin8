using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenieWin8
{
    class GenieWebApi
    {
        HttpClient httpClient;
        UtilityTool util;
        public GenieWebApi()
        {
            httpClient = new HttpClient();
            util = new UtilityTool();
        }

        public async Task<Dictionary<string,string>> CheckNameAvailable(string username)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("username", username);
            string retText = await WebApiPost("check_username", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string available = jo["response"]["available"].ToString().ToLower();
                    retParam.Add("available", available);
                }
                else
                {
                    string errorCode = jo["error"].ToString();
                    string errorMessage = jo["error_message"].ToString();
                    retParam.Add("error",errorCode);
                    retParam.Add("error_message",errorMessage);
                }
            }
            return retParam;
        }

        public async void CreateAccount(string username, string password, string email)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("username", username);
            param.Add("password", password);
            param.Add("email", email);
            await WebApiPost("account_create", param);
        }

        public async Task<Dictionary<string, string>> BeginLogin(string username, string password)
        {
            Dictionary<string,string> param = new Dictionary<string,string> ();
            param.Add("username",username);
            param.Add("password",password);
            string retText = await WebApiPost("account_signin",param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string token = jo["response"]["token"].ToString();
                    retParam.Add("token", token);
                }
                else
                {
                    string errorCode = jo["error"].ToString();
                    string errorMessage = jo["error_message"].ToString();
                    retParam.Add("error", errorCode);
                    retParam.Add("error_message", errorMessage);
                }
            }
            return retParam;
        }

        public async Task<Dictionary<string,string>> GetLabel(string token, string deviceId)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            param.Add("device_id", deviceId);
            string retText = await WebApiPost("label_get", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    //string available = jo["response"]["available"].ToString().ToLower();
                    //retParam.Add("status", status);
                    //retParam.Add("available", available);
                }
                else
                {
                    string errorCode = jo["error"].ToString();
                    string errorMessage = jo["error_message"].ToString();
                    retParam.Add("error", errorCode);
                    retParam.Add("error_message", errorMessage);
                }
            }
            return retParam;
        }

        public async void GetDevice(string token,string deviceKey)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            param.Add("device_key", deviceKey);
            await WebApiPost("device_get", param);
        }

        public async void CreateDevice(string token,string deviceKey)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            param.Add("device_key", deviceKey);
            await WebApiPost("device_create", param);
        }

        public async Task<Dictionary<string,string>> GetFilters(string token, string deviceId)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            param.Add("device_id", deviceId);
            string retText = await WebApiPost("filters_get", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string bundle = jo["response"]["bundle"].ToString();
                    retParam.Add("bundle", bundle);
                }
                else
                {
                    string errorCode = jo["error"].ToString();
                    string errorMessage = jo["error_message"].ToString();
                    retParam.Add("error", errorCode);
                    retParam.Add("error_message", errorMessage);
                }
            }
            return retParam;
        }

        public async Task<Dictionary<string,string>> SetFilters(string token, string deviceId,string bundle)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            param.Add("device_id", deviceId);
            param.Add("bundle", bundle);
            string retText = await WebApiPost("filters_set", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    
                }
                else
                {
                    string errorCode = jo["error"].ToString();
                    string errorMessage = jo["error_message"].ToString();
                    retParam.Add("error", errorCode);
                    retParam.Add("error_message", errorMessage);
                }
            }
            return retParam;
        }

        public async void AccountRelay(string token)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            await WebApiPost("account_relay", param);
        }

        /// <summary>
        /// 获取bypass 用户
        /// </summary>
        /// <param name="parent_deviceId"></param>
        /// <returns></returns>
        public async Task<Dictionary<string,string>> GetUsersForDeviceId(string parent_deviceId)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("parent_device_id", parent_deviceId);
            string retText = await WebApiPost("device_children_get", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string bypassUsers = jo["response"].ToString();
                    retParam.Add("bypassUsers", bypassUsers);
                }
                else
                {
                    string errorCode = jo["error"].ToString();
                    string errorMessage = jo["error_message"].ToString();
                    retParam.Add("error", errorCode);
                    retParam.Add("error_message", errorMessage);
                }
            }
            return retParam;
        }

        public async void GetDeviceChild(string parentDeviceId, string username, string password)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("parent_device_id", parentDeviceId);
            param.Add("device_username", username);
            param.Add("device_password", password);
            await WebApiPost("device_child_get", param);
        }

        public async void GetUserForChildDeviceId(string childDeviceId)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("device_id", childDeviceId);
            await WebApiPost("device_child_username_get", param);
        }

        public async Task<string> WebApiPost(string function, Dictionary<string, string> param)
        {
            string webApi_URL = "https://api.opendns.com/v1/";
            string apiKey = "3443313B70772D2DD73B45D39F376199";////3D8C85A77ADA886B967984DF1F8B3711

            Dictionary<string,string> tempParam = new Dictionary<string,string>();
            tempParam.Add("api_key", apiKey);
            tempParam.Add("method",function);
            if (param.Count() > 0)
            {
                foreach (KeyValuePair<string, string> kv in param)
                {
                    tempParam.Add(kv.Key, kv.Value);
                }
            }
            string soapBody = encodePostData(tempParam);
            StringContent soapContent = new StringContent(soapBody, Encoding.UTF8, "application/x-www-form-urlencoded");
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, webApi_URL);
            request.Content = soapContent;
            try
            {
                
                HttpResponseMessage response = await httpClient.SendAsync(request);
                byte[] resultbt = await response.Content.ReadAsByteArrayAsync();
                string resultstr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
               // HttpStatusCode statusCode = response.StatusCode;
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
        private string encodePostData(Dictionary<string,string> param)
        {
            string postData = "";
            if (param.Count() > 0)
            {
                foreach (KeyValuePair<string, string> kv in param)
                {
                    postData += kv.Key;
                    postData += "=";
                    postData += kv.Value;
                    postData += "&";
                }
                postData = postData.Remove(postData.LastIndexOf('&'), 1);
            }
            return postData;
        }
    }
}
