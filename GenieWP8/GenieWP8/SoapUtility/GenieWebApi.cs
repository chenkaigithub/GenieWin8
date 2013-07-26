using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GenieWP8
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

        /// <summary>
        /// 注册OpenDNS账号时检测用户名是否已存在
        /// 返回结果：
        /// status："success"||"failure"
        /// available："no"||"yes"
        /// </summary>
        /// <param name="username"></param>
        /// <returns>{"status":"success","response":{"available":"no"}}</returns>
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

        /// <summary>
        /// 创建OpenDNS账号
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task<Dictionary<string,string>> CreateAccount(string username, string password, string email)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("username", username);
            param.Add("password", password);
            param.Add("email", email);
            string retText = await WebApiPost("account_create", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string token = jo["response"]["token"].ToString();
                    retParam.Add("available", token);
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

        /// <summary>
        /// 登陆OpenDNS
        /// {"status":"success","response":{"token":"152AACB03FCAEBC6FC52AFD7DBB0DA35"}}
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
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

        /// <summary>
        /// GetLabel
        /// </summary>
        /// <param name="token"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// deviceKey 是由路由器的ModelName和Mac地址组成
        /// 其中ModelName，Mac地址是调用SOAPApi的GetInfo获得
        /// </summary>
        /// <param name="token"></param>
        /// <param name="deviceKey">modelName-MacAddress</param>
        /// <returns></returns>
        public async Task<Dictionary<string,string>> GetDevice(string token,string deviceKey)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            param.Add("device_key", deviceKey);
            string retText = await WebApiPost("device_get", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string deviceId = jo["response"]["device_id"].ToString();
                    retParam.Add("device_id", deviceId);
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

        /// <summary>
        /// 将路由器与OpenDNS账号绑定
        /// deviceKey 是由路由器的ModelName和Mac地址组成
        /// </summary>
        /// <param name="token"></param>
        /// <param name="deviceKey"></param>
        /// <returns></returns>
        public async Task<Dictionary<string,string>> CreateDevice(string token,string deviceKey)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            param.Add("device_key", deviceKey);
            string retText = await WebApiPost("device_create", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string deviceId = jo["response"]["device_id"].ToString();
                    retParam.Add("device_id", deviceId);
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

        /// <summary>
        /// 获取过滤等级
        /// </summary>
        /// <param name="token"></param>
        /// <param name="deviceId"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 设置过滤等级
        /// </summary>
        /// <param name="token"></param>
        /// <param name="deviceId"></param>
        /// <param name="bundle">None||Minimal||Low||Moderate||High</param>
        /// <returns></returns>
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

        /// <summary>
        /// 更改定制的设置api
        /// api_key=3D8C85A77ADA886B967984DF1F8B3711&method=account_relay&token=152AACB03FCAEBC6FC52AFD7DBB0DA35
        /// {"status":"success","response":{"relay_token":"53408EFD4C7B16BB733B39CA469B20F6"}}
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<Dictionary<string,string>> AccountRelay(string token)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            string retText = await WebApiPost("account_relay", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string relayToken = jo["response"]["relay_token"].ToString();
                    retParam.Add("relay_token",relayToken);
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

        /// <summary>
        /// 获取bypass 用户列表
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

        /// <summary>
        /// 登录bypass 账号api
        /// api_key=3D8C85A77ADA886B967984DF1F8B3711&device_password=123456&device_username=user1&method=device_child_get&parent_device_id=0000EE04911BF768
        /// {"status":"success","response":"00000F94FFFCA330"}
        /// </summary>
        /// <param name="parentDeviceId"></param>
        /// <param name="username">bypass 用户名</param>
        /// <param name="device_password">bypass 密码</param>
        public async Task<Dictionary<string, string>> GetDeviceChild(string parentDeviceId, string device_username, string device_password)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("parent_device_id", parentDeviceId);
            param.Add("device_username", device_username);
            param.Add("device_password", device_password);
            string retText = await WebApiPost("device_child_get", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string childDeviceId = jo["response"].ToString();
                    retParam.Add("child_device_id", childDeviceId);
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

        /// <summary>
        /// 通过childDeviceId获取当前设备的bypass用户
        /// </summary>
        /// <param name="childDeviceId"></param>
        public async Task<Dictionary<string,string>> GetUserForChildDeviceId(string childDeviceId)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("device_id", childDeviceId);
            string retText = await WebApiPost("device_child_username_get", param);
            Dictionary<string, string> retParam = new Dictionary<string, string>();
            if (retText != "")
            {
                JObject jo = util.ConvertJsonToObject(retText);
                string status = jo["status"].ToString().ToLower();
                retParam.Add("status", status);
                if (status == "success")
                {
                    string bypassUserName = jo["response"].ToString();
                    retParam.Add("bypass_user",bypassUserName);
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
                HttpStatusCode statusCode = response.StatusCode;
                if (resultstr == null)
                {
                    return "";
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
