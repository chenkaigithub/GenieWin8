using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GenieWin8
{
    class GenieFcml
    {
        HttpClient httpClient;
        public GenieFcml()
        {
            httpClient = new HttpClient();
        }

        public async void Init(string user, string pwd)
        {
            string resourceAddress = string.Format("https://genie.netgear.com/fcp/authenticate HTTP/1.1");
            string sMode = "<authenticate type=\"basic\""
                         + " username=\"%1\" password=\"%2\"/>";
            string body = string.Format(sMode,user,pwd);

            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, resourceAddress);
            StringContent soapContent = new StringContent(body, Encoding.UTF8, "text/xml");
            request.Content = soapContent;
           // request.Content.Headers.Add("SOAPAction", soapAction);
          //  CacheControlHeaderValue nocache = new CacheControlHeaderValue();
            //nocache.NoCache = true;
          //  request.Headers.CacheControl = nocache;
           // request.Headers.Pragma.Add(new NameValueHeaderValue("no-cache"));
            //request.Headers.UserAgent.Add(new ProductInfoHeaderValue("SOAP Toolkit 3.0")); 
            request.Headers.ExpectContinue = false;
            byte[] resultbt;
            string resultstr;
            try
            {
                HttpResponseMessage response = await httpClient.SendAsync(request);
                resultbt = await response.Content.ReadAsByteArrayAsync();
                resultstr = Encoding.UTF8.GetString(resultbt, 0, resultbt.Length);
                //HttpStatusCode statusCode = response.StatusCode;
                //if (statusCode == HttpStatusCode.OK)
                //{
                //    retOK = true;
                //}
                //else
                //{
                //    retOK = false;
                //}
                
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

        public void fcmlRequest(string to, string request)
        {
            string body = "<fcml to=\"%1\" from=\"%2@portal\" _tracer=\"%3\">" 
                        + "%4</fcml>";
            body = string.Format(body, to);
        }
    }
}
