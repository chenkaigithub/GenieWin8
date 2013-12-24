using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using GenieWP8.Resources;
using GenieWP8.DataInfo;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using Microsoft.Phone.Net.NetworkInformation;
using System.Windows.Media;

namespace GenieWP8
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
        {
            InitializeComponent();
            if ((this.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                PageTitle.Width = Application.Current.Host.Content.ActualWidth - 20;
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                PageTitle.Width = Application.Current.Host.Content.ActualHeight - 150;
            }

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();

            //加载页面状态
            LoadState();
        }

        //用于生成本地化 ApplicationBar 的代码
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            //后退按钮
            ApplicationBarIconButton appBarButton_back = new ApplicationBarIconButton(new Uri("Assets/back.png", UriKind.Relative));
            appBarButton_back.Text = AppResources.btnBack;
            ApplicationBar.Buttons.Add(appBarButton_back);
            appBarButton_back.Click += new EventHandler(appBarButton_back_Click);
        }

        private async void LoadState()
        {
            MainPageInfo.rememberpassword = await ReadPasswordFromFile();
            if (MainPageInfo.rememberpassword == "")
            {
                checkRememberPassword.IsChecked = false;
            }
            else
            {
                checkRememberPassword.IsChecked = true;
            }
            tbPassword.Password = MainPageInfo.rememberpassword;
        }

        private void PhoneApplicationPage_OrientationChanged(Object sender, OrientationChangedEventArgs e)
        {
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                PageTitle.Width = Application.Current.Host.Content.ActualWidth - 20;
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                PageTitle.Width = Application.Current.Host.Content.ActualHeight - 150;
            }
        }

        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //重写手机“返回”按钮事件
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //取消按钮事件
        private void CancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //按下屏幕键盘回车键后关闭屏幕键盘
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.Focus();
            } 
            else
            {
                base.OnKeyDown(e);
            }           
        }

        //登录按钮事件
        private async void LoginButton_Click(object sender, EventArgs e)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;          

            string Username = tbUserName.Text.Trim();
            string Password = tbPassword.Password;
            if (checkRememberPassword.IsChecked == true)
            {
                MainPageInfo.rememberpassword = Password;
            }
            else
            {
                MainPageInfo.rememberpassword = "";
            }
            WritePasswordToFile();

            if (DeviceNetworkInformation.IsWiFiEnabled)
            {
                GenieSoapApi soapApi = new GenieSoapApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await soapApi.GetCurrentSetting();

                bool IsModelKeyExists = false;
                foreach (string key in dicResponse.Keys)
                {
                    if (key == "Model")
                    {
                        IsModelKeyExists = true;
                    }
                }
                if (dicResponse.Count > 0 && dicResponse["Firmware"] != "" && IsModelKeyExists && dicResponse["Model"] != "")
                {
                    MainPageInfo.model = dicResponse["Model"];
                    MainPageInfo.firmware = dicResponse["Firmware"];
                    Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                    while (dicResponse2 == null || dicResponse2.Count == 0)
                    {
                        dicResponse2 = await soapApi.Authenticate(Username, Password);
                    }                  
                    if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 0)
                    {
                        MainPageInfo.bLogin = true;
                        MainPageInfo.username = Username;
                        MainPageInfo.password = Password;
                        foreach (var network in new NetworkInterfaceList())
                        {
                            if ((network.InterfaceType == NetworkInterfaceType.Wireless80211) && (network.InterfaceState == ConnectState.Connected))
                            {
                                MainPageInfo.ssid = network.InterfaceName;                                                  //保存登陆成功后所连接WiFi的SSID
                            }
                        }
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        //NavigationService.GoBack();
                        NavigatedToPage();
                    }
                    else if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 401)
                    {
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        var strtext = AppResources.bad_password;
                        MessageBox.Show(strtext);
                    }
                }
                else
                {
                    PopupBackgroundTop.Background = new SolidColorBrush(Colors.Black);
                    PopupBackgroundTop.Opacity = 0.95;
                    PopupBackground.Background = new SolidColorBrush(Colors.Black);
                    PopupBackground.Opacity = 0.95;
                    InProgress.Visibility = Visibility.Collapsed;
                    pleasewait.Visibility = Visibility.Collapsed;
                    LoginalertPopup.IsOpen = true;
                }
            }
            else
            {
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                var strtext = AppResources.login_alertinfo_disableWireless;
                MessageBox.Show(strtext);
            }           
            
        }

        public async void WritePasswordToFile()
        {
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (!fileStorage.FileExists("Password.txt"))
                {
                    using (var file = fileStorage.CreateFile("Password.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(MainPageInfo.rememberpassword);
                        }
                    }
                }
                else
                {
                    fileStorage.DeleteFile("Password.txt");
                    using (var file = fileStorage.CreateFile("Password.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(MainPageInfo.rememberpassword);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public async Task<string> ReadPasswordFromFile()
        {
            string fileContent = string.Empty;
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (fileStorage.FileExists("Password.txt"))
                {
                    using (var file = fileStorage.OpenFile("Password.txt",FileMode.Open,FileAccess.Read))
                    {
                        using (var reader = new StreamReader(file))
                        {
                            fileContent = await reader.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return fileContent;
        }

        private async void NavigatedToPage()
        {
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            UtilityTool util = new UtilityTool();
            Dictionary<string, Dictionary<string, string>> responseDic = new Dictionary<string, Dictionary<string, string>>();
            switch (MainPageInfo.navigatedPage)
            {
                case "MainPage":
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                    break;

                case "WifiSettingPage":
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;                    
                    //Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
                    List<List<string>> attachDeviceAll = new List<List<string>>();
                    attachDeviceAll = await soapApi.GetAttachDevice();   
                    //UtilityTool util = new UtilityTool();
                    var ipList = util.GetCurrentIpAddresses();
                    string loacalIp = ipList.ToList()[0];
                    if (attachDeviceAll.Count == 0)
                    {
                        WifiSettingInfo.linkRate = "";
                        WifiSettingInfo.signalStrength = "";
                    } 
                    else
                    {
                        //foreach (string key in attachDeviceAll.Keys)
                        //{
                        //    if (loacalIp == attachDeviceAll[key]["Ip"])
                        //    {
                        //        if (attachDeviceAll[key].ContainsKey("LinkSpeed"))
                        //        {
                        //            WifiSettingInfo.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                        //        }
                        //        else
                        //        {
                        //            WifiSettingInfo.linkRate = "";
                        //        }
                        //        if (attachDeviceAll[key].ContainsKey("Signal"))
                        //        {
                        //            WifiSettingInfo.signalStrength = attachDeviceAll[key]["Signal"] + "%";
                        //        }
                        //        else
                        //        {
                        //            WifiSettingInfo.signalStrength = "";
                        //        }
                        //    }
                        //}
                        foreach (List<string> deviceInfo in attachDeviceAll)
                        {
                            if (loacalIp == deviceInfo.ElementAt(0))
                            {
                                if (deviceInfo.Count > 4)
                                {
                                    WifiSettingInfo.linkRate = deviceInfo.ElementAt(4) + "Mbps";
                                }
                                else
                                {
                                    WifiSettingInfo.linkRate = "";
                                }
                                if (deviceInfo.Count > 5)
                                {
                                    WifiSettingInfo.signalStrength = deviceInfo.ElementAt(5) + "%";
                                }
                                else
                                {
                                    WifiSettingInfo.signalStrength = "";
                                }
                            }
                        }
                    }
                    
                    dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetInfo("WLANConfiguration");
                    }
                    if (dicResponse.Count > 0)
                    {
                        WifiSettingInfo.ssid = dicResponse["NewSSID"];
                        WifiSettingInfo.changedSsid = dicResponse["NewSSID"];
                        WifiSettingInfo.region = dicResponse["NewRegion"];
                        WifiSettingInfo.channel = dicResponse["NewChannel"];
                        WifiSettingInfo.changedChannel = dicResponse["NewChannel"];
                        WifiSettingInfo.wirelessMode = dicResponse["NewWirelessMode"];
                        WifiSettingInfo.securityType = dicResponse["NewWPAEncryptionModes"];
                        WifiSettingInfo.changedSecurityType = dicResponse["NewWPAEncryptionModes"];
                    }
                    dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetWPASecurityKeys();
                    }
                    if (dicResponse.Count > 0)
                    {
                        WifiSettingInfo.password = dicResponse["NewWPAPassphrase"];
                        WifiSettingInfo.changedPassword = dicResponse["NewWPAPassphrase"];
                    }
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    NavigationService.Navigate(new Uri("/WifiSettingPage.xaml", UriKind.Relative));
                    break;

                case "GuestAccessPage":
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    //Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetGuestAccessEnabled();
                    }                   
                    GuestAccessInfo.isGuestAccessEnabled = dicResponse["NewGuestAccessEnabled"];
                    if (dicResponse["NewGuestAccessEnabled"] == "0" || dicResponse["NewGuestAccessEnabled"] == "1")
                    {
                        Dictionary<string, string> dicResponse1 = new Dictionary<string, string>();
                        while (dicResponse1 == null || dicResponse1.Count == 0)
                        {
                            dicResponse1 = await soapApi.GetGuestAccessNetworkInfo();
                        }                        
                        if (dicResponse1.Count > 0)
                        {
                            GuestAccessInfo.ssid = dicResponse1["NewSSID"];
                            GuestAccessInfo.changedSsid = dicResponse1["NewSSID"];
                            GuestAccessInfo.securityType = dicResponse1["NewSecurityMode"];
                            GuestAccessInfo.changedSecurityType = dicResponse1["NewSecurityMode"];
                            if (dicResponse1["NewSecurityMode"] != "None")
                            {
                                GuestAccessInfo.password = dicResponse1["NewKey"];
                                GuestAccessInfo.changedPassword = dicResponse1["NewKey"];
                            }
                            else
                            {
                                GuestAccessInfo.password = "";
                                GuestAccessInfo.changedPassword = "";
                            }
                            if (GuestAccessInfo.timePeriod == null)
                            {
                                GuestAccessInfo.timePeriod = "Always";
                                GuestAccessInfo.changedTimePeriod = "Always";
                            }
                        }
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        NavigationService.Navigate(new Uri("/GuestAccessPage.xaml", UriKind.Relative));
                    }
                    else if (dicResponse["NewGuestAccessEnabled"] == "2")
                    {
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        MessageBox.Show(AppResources.notsupport);
                    }
                    break;

                case "NetworkMapPage":
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    //UtilityTool util = new UtilityTool();
                    NetworkMapInfo.geteway = await util.GetGateway();
                    //Dictionary<string, Dictionary<string, string>> responseDic = new Dictionary<string, Dictionary<string, string>>();        
                    NetworkMapInfo.attachDeviceDic = await soapApi.GetAttachDevice();

                    //Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetInfo("WLANConfiguration");
                    }
                    if (dicResponse.Count > 0)
                    {
                        WifiSettingInfo.ssid = dicResponse["NewSSID"];
                        WifiSettingInfo.channel = dicResponse["NewChannel"];
                        WifiSettingInfo.securityType = dicResponse["NewWPAEncryptionModes"];
                        WifiSettingInfo.macAddr = dicResponse["NewWLANMACAddress"];
                    }
                    NetworkMapInfo.fileContent = await ReadDeviceInfoFile();
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    //this.Frame.Navigate(typeof(NetworkMapPage));
                    NetworkMapInfo.bTypeChanged = false;
                    NavigationService.Navigate(new Uri("/NetworkMapPage.xaml", UriKind.Relative));
                    break;

                case "TrafficMeterPage":
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    //Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetTrafficMeterEnabled();
                    }
                    TrafficMeterInfo.isTrafficMeterEnabled = dicResponse["NewTrafficMeterEnable"];
                    if (dicResponse["NewTrafficMeterEnable"] == "0" || dicResponse["NewTrafficMeterEnable"] == "1")
                    {
                        Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                        while (dicResponse2 == null || dicResponse2.Count == 0)
                        {
                            dicResponse2 = await soapApi.GetTrafficMeterOptions();
                        }
                        if (dicResponse2.Count > 0)
                        {
                            TrafficMeterInfo.MonthlyLimit = dicResponse2["NewMonthlyLimit"];
                            TrafficMeterInfo.changedMonthlyLimit = dicResponse2["NewMonthlyLimit"];
                            TrafficMeterInfo.RestartHour = dicResponse2["RestartHour"];
                            TrafficMeterInfo.changedRestartHour = dicResponse2["RestartHour"];
                            TrafficMeterInfo.RestartMinute = dicResponse2["RestartMinute"];
                            TrafficMeterInfo.changedRestartMinute = dicResponse2["RestartMinute"];
                            TrafficMeterInfo.RestartDay = dicResponse2["RestartDay"];
                            TrafficMeterInfo.changedRestartDay = dicResponse2["RestartDay"];
                            TrafficMeterInfo.ControlOption = dicResponse2["NewControlOption"];
                            TrafficMeterInfo.changedControlOption = dicResponse2["NewControlOption"];
                        }
                        dicResponse2 = new Dictionary<string, string>();
                        while (dicResponse2 == null || dicResponse2.Count == 0)
                        {
                            dicResponse2 = await soapApi.GetTrafficMeterStatistics();
                        }
                        if (dicResponse2.Count > 0)
                        {
                            TrafficMeterInfo.TodayUpload = dicResponse2["NewTodayUpload"];
                            TrafficMeterInfo.TodayDownload = dicResponse2["NewTodayDownload"];
                            TrafficMeterInfo.YesterdayUpload = dicResponse2["NewYesterdayUpload"];
                            TrafficMeterInfo.YesterdayDownload = dicResponse2["NewYesterdayDownload"];
                            TrafficMeterInfo.WeekUpload = dicResponse2["NewWeekUpload"];
                            TrafficMeterInfo.WeekDownload = dicResponse2["NewWeekDownload"];
                            TrafficMeterInfo.MonthUpload = dicResponse2["NewMonthUpload"];
                            TrafficMeterInfo.MonthDownload = dicResponse2["NewMonthDownload"];
                            TrafficMeterInfo.LastMonthUpload = dicResponse2["NewLastMonthUpload"];
                            TrafficMeterInfo.LastMonthDownload = dicResponse2["NewLastMonthDownload"];
                        }
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        NavigationService.Navigate(new Uri("/TrafficMeterPage.xaml", UriKind.Relative));
                    }
                    else if (dicResponse["NewTrafficMeterEnable"] == "2")
                    {
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        MessageBox.Show(AppResources.notsupport);
                    }
                    break;

                case "ParentalControlPage":
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;                                        
                    //Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = await soapApi.GetCurrentSetting();
                    if (dicResponse.Count > 0)
                    {
                        //判断路由器是否已连接因特网
                        if (dicResponse["InternetConnectionStatus"] != "Up")
                        {
                            PopupBackgroundTop.Visibility = Visibility.Collapsed;
                            PopupBackground.Visibility = Visibility.Collapsed;
                            MessageBox.Show(AppResources.interneterror);
                        }
                        else
                        {
                            if (dicResponse["ParentalControlSupported"] == "1")
                            {
                                ///通过attachDevice获取本机的Mac地址
                                NetworkMapInfo.attachDeviceDic = await soapApi.GetAttachDevice();

                                Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                                while (dicResponse2 == null || dicResponse2.Count == 0)
                                {
                                    dicResponse2 = await soapApi.GetInfo("WLANConfiguration");
                                }
                                ParentalControlInfo.RouterMacaddr = dicResponse2["NewWLANMACAddress"];

                                //dicResponse2 = new Dictionary<string, string>();
                                //while (dicResponse2 == null || dicResponse2.Count == 0 || int.Parse(dicResponse2["ResponseCode"]) != 0)
                                //{
                                //    dicResponse2 = await soapApi.GetEnableStatus();
                                //}
                                //ParentalControlInfo.isParentalControlEnabled = dicResponse2["ParentalControl"];                               
                                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                                PopupBackground.Visibility = Visibility.Collapsed;
                                NavigationService.Navigate(new Uri("/ParentalControlPage.xaml", UriKind.Relative));
                            }
                            else
                            {
                                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                                PopupBackground.Visibility = Visibility.Collapsed;
                                MessageBox.Show(AppResources.notsupport);
                            }
                        }
                    }
                    break;
            }
        }

        public async Task<string> ReadDeviceInfoFile()
        {
            string fileContent = string.Empty;
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (fileStorage.FileExists("CustomDeviceInfo.txt"))
                {
                    using (var file = fileStorage.OpenFile("CustomDeviceInfo.txt", FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(file))
                        {
                            fileContent = await reader.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return fileContent;
        }

        private void tbPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            tbPassword.Background = new SolidColorBrush(Colors.White);
        }

        private async void SupportButton_Click(Object sender, RoutedEventArgs e)
        {
            var uri = new Uri(((HyperlinkButton)sender).Tag.ToString());
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            LoginalertPopup.IsOpen = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            PopupBackgroundTop.Background = new SolidColorBrush(Color.FromArgb(255, 90, 90, 90));
            PopupBackgroundTop.Opacity = 0.9;
            PopupBackground.Background = new SolidColorBrush(Color.FromArgb(255, 90, 90, 90));
            PopupBackground.Opacity = 0.9;
        }
    }
}