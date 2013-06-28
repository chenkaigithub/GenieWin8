using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GenieWin8
{
    class GenieFcml
    {
        HttpClient httpClient;
        bool retOK;
        public GenieFcml()
        {
            httpClient = new HttpClient();
        }

        public async void Init(string user, string pwd)
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
        }

        public async void fcmlRequest(string to, string requestText)
        {
            string resourceAddress = string.Format("https://genie.netgear.com:443//fcp/send?n={0}");
            string body = "<fcml to=\"{0}\" from=\"{1}@portal\" _tracer=\"{2}\">" 
                        + "{3}</fcml>";
            body = string.Format(body, to);
           
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
        }
    }
}
