//*********************************************************
//
// Copyright (c) Microsoft. All rights reserved.
// THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF
// ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY
// IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR
// PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.
//
//*********************************************************

using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using System;
using System.Net.Http;
using SDKTemplate;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using System.Text;

namespace Microsoft.Samples.Networking.HttpClientSample
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Scenario4 : SDKTemplate.Common.LayoutAwarePage, IDisposable
    {
        // A pointer back to the main page.  This is needed if you want to call methods in MainPage such
        // as NotifyUser()
        MainPage rootPage = MainPage.Current;

        HttpClient httpClient;

        public Scenario4()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Helpers.CreateHttpClient(ref httpClient);
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Dispose();
        }

        private async void Start_Click(object sender, RoutedEventArgs e)
        {
            GenieSoapApi soapApi = new GenieSoapApi();
            soapApi.Authenticate();
            //GetInfo();
            //if (httpClient.BaseAddress == null)
            //{
            //    // 'BaseAddress' is a disabled text box, so the value is considered trusted input. When enabling
            //    // the text box make sure to validate the string (e.g., by using Uri.TryCreate()).
            //    // Uri baseAddress = new Uri(BaseAddress.Text);

            //    Uri baseAddress = new Uri("http://routerlogin.net:5000/");

            //    // HttpClient can be configured with a base address: When sending a request using a relative URI,
            //    // the value of BaseAddress will be prepended to the specified relative URI before sending the request.
            //    httpClient.BaseAddress = baseAddress;

            //    httpClient.DefaultRequestHeaders.Add("SOAPAction", "urn:NETGEAR-ROUTER:service:ParentalControl:1#Authenticate");
            //   // httpClient.DefaultRequestHeaders.Add("content-type", "text/xml;charset=utf-8");
            //    httpClient.DefaultRequestHeaders.Add("HOST", "www.routerlogin.com");
            //   httpClient.DefaultRequestHeaders.Add("User-Agent", "SOAP Toolkit 3.0");
            //   // httpClient.DefaultRequestHeaders.Add("connection", "keep-Alive");
            //    httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
            //    httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
            //   // httpClient.DefaultRequestHeaders.Connection.Add("keep-Alive");
            //    //httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("User-Agent", "SOAP Toolkit 3.0"));
            //   // httpClient.DefaultRequestHeaders.Pragma.Add(new NameValueHeaderValue("Pragma", "no-cache"));

            //    httpClient.Timeout = new TimeSpan(TimeSpan.TicksPerMinute*5);
            //    httpClient.DefaultRequestHeaders.ExpectContinue = false;
                

            //}

            //Helpers.ScenarioStarted(StartButton, CancelButton);
            //rootPage.NotifyUser("In progress", NotifyType.StatusMessage);
            //OutputField.Text = string.Empty;

            //try
            //{
            //    // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
            //    // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
            //    // string resourceAddress = AddressField.Text;

            //    string resourceAddress = "soap/server_sa/";
            //    string soap = "<SOAP-ENV:Envelope xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\">\n"
            //                    + "<SOAP-ENV:Header>\n"
            //                    + "<SessionID xsi:type=\"xsd:string\" xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\">E6A88AE69687E58D9A00</SessionID>\n"
            //                    + "</SOAP-ENV:Header>\n"
            //                    + "<SOAP-ENV:Body>\n"
            //                    + "<Authenticate>\n"
            //                    + "<NewPassword xsi:type=\"xsd:string\" xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\">password</NewPassword>\n"
            //                    + "<NewUsername xsi:type=\"xsd:string\" xmlns:xsi=\"http://www.w3.org/1999/XMLSchema-instance\">admin</NewUsername>\n"
            //                    + "</Authenticate>\n"
            //                    + "</SOAP-ENV:Body>\n"
            //                    + "</SOAP-ENV:Envelope>\n";
            //    HttpResponseMessage response = await httpClient.PostAsync(resourceAddress,
            //       new StringContent(soap,Encoding.UTF8,"text/xml"));

                
            //     //HttpResponseMessage response = await httpClient.PostAsync(resourceAddress,
            //     //    new StringContent(RequestBodyField.Text));
            //  //  await Helpers.DisplayTextResult(response, OutputField);

            //    rootPage.NotifyUser("Completed", NotifyType.StatusMessage);
            //}
            //catch (HttpRequestException hre)
            //{
            //    rootPage.NotifyUser("Error", NotifyType.ErrorMessage);
            //    OutputField.Text = hre.ToString();
            //}
            //catch (TaskCanceledException)
            //{
            //    rootPage.NotifyUser("Request canceled.", NotifyType.ErrorMessage);
            //}
            //finally
            //{
            //    Helpers.ScenarioCompleted(StartButton, CancelButton);
            //}


//            string soap =
//                        @"<SOAP-ENV:Envelope xmlns:SOAP-ENV=""http://schemas.xmlsoap.org/soap/envelope/"">
//                        <SOAP-ENV:Header>
//                        <SessionID xsi:type=""xsd:string"" xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"">E6A88AE69687E58D9A00</SessionID>
//                        </SOAP-ENV:Header>
//                        <SOAP-ENV:Body>
//                        <Authenticate>
//                        <NewPassword xsi:type=""xsd:string"" xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"">password</NewPassword>
//                        <NewUsername xsi:type=""xsd:string"" xmlns:xsi=""http://www.w3.org/1999/XMLSchema-instance"">admin</NewUsername>
//                        </Authenticate>
//                        </SOAP-ENV:Body>
//                        </SOAP-ENV:Envelope>";

//            HttpWebRequest req = (HttpWebRequest)WebRequest.Create("http://routerlogin.net:5000/soap/server_sa/");
//            req.Headers["SOAPAction"] = "urn:NETGEAR-ROUTER:service:ParentalControl:1#Authenticate";
//            req.ContentType = "text/xml;charset=utf-8";
//            req.Accept = "text/xml";
//            req.Method = "POST";
//            //  req.Headers["User-Agent"] = "SOAP Toolkit 3.0";
//            req.Headers["Cache-Control"] = "no-cache";
//            req.Headers["Pragma"] = "no-cache";

//            using (Stream stm = await req.GetRequestStreamAsync())
//            {
//                using (StreamWriter stmw = new StreamWriter(stm))
//                {
//                    stmw.Write(soap);
//                }
//            }

//            using (WebResponse response = await req.GetResponseAsync())
//            {

//                Stream responseStream = response.GetResponseStream();
//            }
//            //System.Diagnostics.Debug.WriteLine(buf);

        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            httpClient.CancelPendingRequests();
        }

        public void Dispose()
        {
            if (httpClient != null)
            {
                httpClient.Dispose();
                httpClient = null;
            }
        }

        public async void GetInfo()
        {
            if (httpClient.BaseAddress == null)
            {
                // 'BaseAddress' is a disabled text box, so the value is considered trusted input. When enabling
                // the text box make sure to validate the string (e.g., by using Uri.TryCreate()).
                // Uri baseAddress = new Uri(BaseAddress.Text);

                Uri baseAddress = new Uri("http://routerlogin.net:5000/");

                // HttpClient can be configured with a base address: When sending a request using a relative URI,
                // the value of BaseAddress will be prepended to the specified relative URI before sending the request.
                httpClient.BaseAddress = baseAddress;

                httpClient.DefaultRequestHeaders.Add("SOAPAction", "urn:NETGEAR-ROUTER:service:WLANConfiguration:1#GetInfo");
                // httpClient.DefaultRequestHeaders.Add("content-type", "text/xml;charset=utf-8");
                httpClient.DefaultRequestHeaders.Add("HOST", "www.routerlogin.com");
                httpClient.DefaultRequestHeaders.Add("User-Agent", "SOAP Toolkit 3.0");
                // httpClient.DefaultRequestHeaders.Add("connection", "keep-Alive");
                httpClient.DefaultRequestHeaders.Add("Cache-Control", "no-cache");
                httpClient.DefaultRequestHeaders.Add("Pragma", "no-cache");
                // httpClient.DefaultRequestHeaders.Connection.Add("keep-Alive");
                //httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("User-Agent", "SOAP Toolkit 3.0"));
                // httpClient.DefaultRequestHeaders.Pragma.Add(new NameValueHeaderValue("Pragma", "no-cache"));

                httpClient.Timeout = new TimeSpan(TimeSpan.TicksPerMinute * 5);
                httpClient.DefaultRequestHeaders.ExpectContinue = false;


            }

            Helpers.ScenarioStarted(StartButton, CancelButton);
            rootPage.NotifyUser("In progress", NotifyType.StatusMessage);
            OutputField.Text = string.Empty;

            try
            {
                // 'AddressField' is a disabled text box, so the value is considered trusted input. When enabling the
                // text box make sure to validate user input (e.g., by catching FormatException as shown in scenario 1).
                // string resourceAddress = AddressField.Text;

                string resourceAddress = "soap/server_sa/";
                string soap = "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?><SOAP-ENV:Envelope xmlns:SOAPSDK1=\"http://www.w3.org/2001/XMLSchema\" xmlns:SOAPSDK2=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:SOAPSDK3=\"http://schemas.xmlsoap.org/soap/encoding/\" xmlns:SOAP-ENV=\"http://schemas.xmlsoap.org/soap/envelope/\"><SOAP-ENV:Header><SessionID>58DEE6006A88A967E89A</SessionID></SOAP-ENV:Header><SOAP-ENV:Body><M1:GetInfo xmlns:M1=\"urn:NETGEAR-ROUTER:service:WLANConfiguration:1\"></M1:GetInfo></SOAP-ENV:Body></SOAP-ENV:Envelope>";
                HttpResponseMessage response = await httpClient.PostAsync(resourceAddress,
                    new StringContent(soap, Encoding.UTF8, "text/xml"));


                //HttpResponseMessage response = await httpClient.PostAsync(resourceAddress,
                //    new StringContent(RequestBodyField.Text));
                await Helpers.DisplayTextResult(response, OutputField);

                rootPage.NotifyUser("Completed", NotifyType.StatusMessage);
            }
            catch (HttpRequestException hre)
            {
                rootPage.NotifyUser("Error", NotifyType.ErrorMessage);
                OutputField.Text = hre.ToString();
            }
            catch (TaskCanceledException)
            {
                rootPage.NotifyUser("Request canceled.", NotifyType.ErrorMessage);
            }
            finally
            {
                Helpers.ScenarioCompleted(StartButton, CancelButton);
            }
        }
    }
}
