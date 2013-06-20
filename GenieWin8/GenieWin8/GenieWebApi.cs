using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

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

        public async void CheckNameAvailable(string username)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("username", "netgear");
            await WebApiPost("check_username", param);
        }

        public async void CreateAccount(string username, string password, string email)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("username", username);
            param.Add("password", password);
            param.Add("email", email);
            await WebApiPost("account_create", param);
        }

        public async Task<string> BeginLogin(string username, string password)
        {
            Dictionary<string,string> param = new Dictionary<string,string> ();
            param.Add("username",username);
            param.Add("password",password);
           await WebApiPost("account_signin",param);
           return "";
        }

        public async void GetLabel(string token, string deviceId)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            param.Add("device_id", deviceId);
            await WebApiPost("label_get", param);
        }

        public async void GetDevice(string token,string deviceKey)
        {
            Dictionary<string, string> param = new Dictionary<string, string>();
            param.Add("token", token);
            param.Add("device_key", deviceKey);
            await WebApiPost("label_get", param);
        }

        public async Task<Dictionary<string, string>> WebApiPost(string function, Dictionary<string, string> param)
        {
            string webApi_URL = "https://api.opendns.com/v1/";
            string apiKey = "3443313B70772D2DD73B45D39F376199";////3D8C85A77ADA886B967984DF1F8B3711///3D8C85A77ADA886B967984DF1F8B3711

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
            StringContent soapContent = new StringContent(soapBody);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, webApi_URL);
            request.Content = soapContent;
            try
            {
                
                HttpResponseMessage response = await httpClient.SendAsync(request);
                byte[] resultbt = await response.Content.ReadAsByteArrayAsync();
                string resultstr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
                HttpStatusCode statusCode = response.StatusCode;
                if (resultstr.Length > 0)
                {
 
                }

                //    return resultstr;
            }
            catch (HttpRequestException hre)
            {
                //   return "";
            }
            catch (TaskCanceledException hce)
            {
                //   return "";
            }
            catch (Exception ex)
            {
                //  return "";
            }
            return new Dictionary<string, string>();
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
