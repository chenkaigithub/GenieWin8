using GenieWin8.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GenieWin8.DataModel;
using Windows.Storage;
using Windows.Storage.Streams;
using System.Threading.Tasks;
using Windows.UI.Popups;

using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using System.Text.RegularExpressions;
using Windows.Networking.Connectivity;

// “项目页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234233 上提供

namespace GenieWin8
{
    /// <summary>
    /// 显示项预览集合的页。在“拆分布局应用程序”中，此页
    /// 用于显示及选择可用组之一。
    /// </summary>
    public sealed partial class MainPage : GenieWin8.Common.LayoutAwarePage
    {
        public MainPage()
        {
            this.InitializeComponent();
        }
       
        /// <summary>
        /// 使用在导航过程中传递的内容填充页。在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="navigationParameter">最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的参数值。
        /// </param>
        /// <param name="pageState">此页在以前会话期间保留的状态
        /// 字典。首次访问页面时为 null。</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            // TODO: 创建适用于问题域的合适数据模型以替换示例数据
            var dataGroups = DataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Items"] = dataGroups;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            tbSsid.Text = loader.GetString("WifiDisconnected");
            Uri _baseUri = new Uri("ms-appx:///");
            imgWifi.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/nowifi.png"));
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        tbSsid.Text = connectionProfile.ProfileName;
                        imgWifi.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag4.png"));
                    }
                    //if ((network.InterfaceType == NetworkInterfaceType.Wireless80211) && (network.InterfaceState == ConnectState.Connected))
                    //{
                    //    tbSsid.Text = network.InterfaceName;
                    //    imgWifi.Source = new BitmapImage(new Uri("Assets/signal/wirelessflag4.png", UriKind.Relative));
                    //}
                }
                //rootPage.NotifyUser(connectionProfileList, NotifyType.StatusMessage);
            }
            catch (Exception ex)
            {
                //rootPage.NotifyUser("Unexpected exception occured: " + ex.ToString(), NotifyType.ErrorMessage);
            }

            if (MainPageInfo.bLogin)
            {
                btnLogin.Visibility = Visibility.Collapsed;
                btnLogout.Visibility = Visibility.Visible;
            } 
            else
            {
                btnLogin.Visibility = Visibility.Visible;
                btnLogout.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 在单击某个项时进行调用。
        /// </summary>
        /// <param name="sender">显示所单击项的 GridView (在应用程序处于对齐状态时
        /// 为 ListView)。</param>
        /// <param name="e">描述所单击项的事件数据。</param>
        private async void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 导航至相应的目标页，并
            // 通过将所需信息作为导航参数传入来配置新页
            //var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            //this.Frame.Navigate(typeof(SplitPage), groupId);
            var groupId = ((DataGroup)e.ClickedItem).UniqueId;
            //if (MainPageInfo.bLogin)	//已登陆
            //{
            GenieSoapApi soapApi = new GenieSoapApi();
            //无线设置
            if (groupId == "WiFiSetting")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;

                    Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
                    attachDeviceAll = await soapApi.GetAttachDevice();
                    UtilityTool util = new UtilityTool();
                    string loacalIp = util.GetLocalHostIp();
                    if (attachDeviceAll.Count == 0)
                    {
                        WifiInfoModel.linkRate = "";
                        WifiInfoModel.signalStrength = "";
                    } 
                    else
                    {
                        foreach (string key in attachDeviceAll.Keys)
                        {
                            if (loacalIp == attachDeviceAll[key]["Ip"])
                            {
                                if (attachDeviceAll[key].ContainsKey("LinkSpeed"))
                                {
                                    WifiInfoModel.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                                }
                                else
                                {
                                    WifiInfoModel.linkRate = "";
                                }
                                if (attachDeviceAll[key].ContainsKey("Signal"))
                                {
                                    WifiInfoModel.signalStrength = attachDeviceAll[key]["Signal"] + "%";
                                }
                                else
                                {
                                    WifiInfoModel.signalStrength = "";
                                }
                            }
                        }
                    }

                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetInfo("WLANConfiguration");
                    }
                    if (dicResponse.Count > 0)
                    {
                        WifiInfoModel.ssid = dicResponse["NewSSID"];
                        WifiInfoModel.changedSsid = dicResponse["NewSSID"];
                        WifiInfoModel.region = dicResponse["NewRegion"];
                        WifiInfoModel.channel = dicResponse["NewChannel"];
                        WifiInfoModel.changedChannel = dicResponse["NewChannel"];
                        WifiInfoModel.wirelessMode = dicResponse["NewWirelessMode"];
                        WifiInfoModel.securityType = dicResponse["NewWPAEncryptionModes"];
                        WifiInfoModel.changedSecurityType = dicResponse["NewWPAEncryptionModes"];
                    }
                    dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetWPASecurityKeys();
                    }
                    if (dicResponse.Count > 0)
                    {
                        WifiInfoModel.password = dicResponse["NewWPAPassphrase"];
                        WifiInfoModel.changedPassword = dicResponse["NewWPAPassphrase"];
                    }
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    this.Frame.Navigate(typeof(WifiSettingPage));
                }
                else	//未登陆
                {
                    this.Frame.Navigate(typeof(LoginPage));
                    MainPageInfo.navigatedPage = "WifiSettingPage";
                }                
            }
            //访客访问
            else if (groupId == "GuestAccess")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetGuestAccessEnabled();
                    }
                    GuestAccessInfoModel.isGuestAccessEnabled = dicResponse["NewGuestAccessEnabled"];
                    if (dicResponse["NewGuestAccessEnabled"] == "0" || dicResponse["NewGuestAccessEnabled"] == "1")
                    {
                        Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                        while (dicResponse2 == null || dicResponse2.Count == 0)
                        {
                            dicResponse2 = await soapApi.GetGuestAccessNetworkInfo();
                        }
                        if (dicResponse2.Count > 0)
                        {
                            GuestAccessInfoModel.ssid = dicResponse2["NewSSID"];
                            GuestAccessInfoModel.changedSsid = dicResponse2["NewSSID"];
                            GuestAccessInfoModel.securityType = dicResponse2["NewSecurityMode"];
                            GuestAccessInfoModel.changedSecurityType = dicResponse2["NewSecurityMode"];
                            if (dicResponse2["NewSecurityMode"] != "None")
                            {
                                GuestAccessInfoModel.password = dicResponse2["NewKey"];
                                GuestAccessInfoModel.changedPassword = dicResponse2["NewKey"];
                            }
                            else
                            {
                                GuestAccessInfoModel.password = "";
                                GuestAccessInfoModel.changedPassword = "";
                            }
                            if (GuestAccessInfoModel.timePeriod == null)
                            {
                                GuestAccessInfoModel.timePeriod = "Always";
                                GuestAccessInfoModel.changedTimePeriod = "Always";
                            }
                        }                        
                        InProgress.IsActive = false;
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        this.Frame.Navigate(typeof(GuestAccessPage));
                    }
                    else if (dicResponse["NewGuestAccessEnabled"] == "2")
                    {
                        InProgress.IsActive = false;
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                        var strtext = loader.GetString("notsupport");
                        var messageDialog = new MessageDialog(strtext);
                        await messageDialog.ShowAsync();
                    }
                }
                else	//未登陆
                {
                    this.Frame.Navigate(typeof(LoginPage));
                    MainPageInfo.navigatedPage = "GuestAccessPage";
                }                
            }
            //网络映射
            else if (groupId == "NetworkMap")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    UtilityTool util = new UtilityTool();
                    NetworkMapInfo.geteway = await util.GetGateway();
                    NetworkMapInfo.attachDeviceDic = await soapApi.GetAttachDevice();

                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetInfo("WLANConfiguration");
                    }
                    if (dicResponse.Count > 0)
                    {
                        WifiInfoModel.ssid = dicResponse["NewSSID"];
                        WifiInfoModel.channel = dicResponse["NewChannel"];
                        WifiInfoModel.securityType = dicResponse["NewWPAEncryptionModes"];
                        WifiInfoModel.macAddr = dicResponse["NewWLANMACAddress"];
                    }
                    NetworkMapInfo.fileContent = await ReadDeviceInfoFile();
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    this.Frame.Navigate(typeof(NetworkMapPage));
                }
                else	//未登陆
                {
                    this.Frame.Navigate(typeof(LoginPage));
                    MainPageInfo.navigatedPage = "NetworkMapPage";
                }                
            }
            //流量控制
            else if (groupId == "TrafficMeter")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.GetTrafficMeterEnabled();
                    }
                    TrafficMeterInfoModel.isTrafficMeterEnabled = dicResponse["NewTrafficMeterEnable"];
                    if (dicResponse["NewTrafficMeterEnable"] == "0" || dicResponse["NewTrafficMeterEnable"] == "1")
                    {
                        Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                        while (dicResponse2 == null || dicResponse2.Count == 0)
                        {
                            dicResponse2 = await soapApi.GetTrafficMeterOptions();
                        }
                        if (dicResponse2.Count > 0)
                        {
                            TrafficMeterInfoModel.MonthlyLimit = dicResponse2["NewMonthlyLimit"];
                            TrafficMeterInfoModel.changedMonthlyLimit = dicResponse2["NewMonthlyLimit"];
                            TrafficMeterInfoModel.RestartHour = dicResponse2["RestartHour"];
                            TrafficMeterInfoModel.changedRestartHour = dicResponse2["RestartHour"];
                            TrafficMeterInfoModel.RestartMinute = dicResponse2["RestartMinute"];
                            TrafficMeterInfoModel.changedRestartMinute = dicResponse2["RestartMinute"];
                            TrafficMeterInfoModel.RestartDay = dicResponse2["RestartDay"];
                            TrafficMeterInfoModel.changedRestartDay = dicResponse2["RestartDay"];
                            TrafficMeterInfoModel.ControlOption = dicResponse2["NewControlOption"];
                            TrafficMeterInfoModel.changedControlOption = dicResponse2["NewControlOption"];
                        }
                        dicResponse2 = new Dictionary<string, string>();
                        while (dicResponse2 == null || dicResponse2.Count == 0)
                        {
                            dicResponse2 = await soapApi.GetTrafficMeterStatistics();
                        }
                        if (dicResponse2.Count > 0)
                        {
                            TrafficMeterInfoModel.TodayUpload = dicResponse2["NewTodayUpload"];
                            TrafficMeterInfoModel.TodayDownload = dicResponse2["NewTodayDownload"];
                            TrafficMeterInfoModel.YesterdayUpload = dicResponse2["NewYesterdayUpload"];
                            TrafficMeterInfoModel.YesterdayDownload = dicResponse2["NewYesterdayDownload"];
                            TrafficMeterInfoModel.WeekUpload = dicResponse2["NewWeekUpload"];
                            TrafficMeterInfoModel.WeekDownload = dicResponse2["NewWeekDownload"];
                            TrafficMeterInfoModel.MonthUpload = dicResponse2["NewMonthUpload"];
                            TrafficMeterInfoModel.MonthDownload = dicResponse2["NewMonthDownload"];
                            TrafficMeterInfoModel.LastMonthUpload = dicResponse2["NewLastMonthUpload"];
                            TrafficMeterInfoModel.LastMonthDownload = dicResponse2["NewLastMonthDownload"];
                        }
                        InProgress.IsActive = false;
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        this.Frame.Navigate(typeof(TrafficMeterPage));
                    }
                    else if (dicResponse["NewTrafficMeterEnable"] == "2")
                    {
                        InProgress.IsActive = false;
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                        var strtext = loader.GetString("notsupport");
                        var messageDialog = new MessageDialog(strtext);
                        await messageDialog.ShowAsync();
                    }
                }
                else	//未登陆
                {
                    this.Frame.Navigate(typeof(LoginPage));
                    MainPageInfo.navigatedPage = "TrafficMeterPage";
                }               
            }
            //家长控制
            else if (groupId == "ParentalControl")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;

                    //这里需要判断是否已连接因特网
                    UtilityTool util = new UtilityTool();
                    bool isConnectToInternet = util.IsConnectedToInternet();
                    if (!isConnectToInternet)
                    {
                        InProgress.IsActive = false;
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                        var strtext = loader.GetString("interneterror");
                        var messageDialog = new MessageDialog(strtext);
                        await messageDialog.ShowAsync();
                    }
                    else
                    {
                        Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                        dicResponse = await soapApi.GetCurrentSetting();
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

                            dicResponse2 = new Dictionary<string, string>();
                            while (dicResponse2 == null || dicResponse2.Count == 0 || int.Parse(dicResponse2["ResponseCode"]) != 0)
                            {
                                dicResponse2 = await soapApi.GetEnableStatus();
                            }
                            ParentalControlInfo.isParentalControlEnabled = dicResponse2["ParentalControl"];
                            InProgress.IsActive = false;
                            PopupBackgroundTop.Visibility = Visibility.Collapsed;
                            PopupBackground.Visibility = Visibility.Collapsed;
                            this.Frame.Navigate(typeof(ParentalControlPage));
                        }
                        else
                        {
                            InProgress.IsActive = false;
                            PopupBackgroundTop.Visibility = Visibility.Collapsed;
                            PopupBackground.Visibility = Visibility.Collapsed;
                            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                            var strtext = loader.GetString("notsupport");
                            var messageDialog = new MessageDialog(strtext);
                            await messageDialog.ShowAsync();
                        }
                    }
                }
                else	//未登陆
                {
                    this.Frame.Navigate(typeof(LoginPage));
                    MainPageInfo.navigatedPage = "ParentalControlPage";
                }
            }
            //我的媒体
            //else if (groupId == "MyMedia")
            //{
                //UtilityTool util = new UtilityTool();
                //GenieSoapApi soapApi = new GenieSoapApi();
                //Dictionary<string, Dictionary<string, string>> responseDic = new Dictionary<string, Dictionary<string, string>>();
                //responseDic = await soapApi.GetAttachDevice();
                //NetworkMapInfo.attachDeviceDic = responseDic;
                //  GenieFcml fcml = new GenieFcml();
                // await fcml.Init("siteviewgenietest@gmail.com", "siteview");
                // await fcml.GetCPList();

                //this.Frame.Navigate(typeof(MyMediaPage));
            //}
            //QRCode
            else if (groupId == "QRCode")
            {
                this.Frame.Navigate(typeof(QRCodePage));
            }
            //MarketPlace
            //else if (groupId == "MarketPlace")
            //{
            //    var uri = new Uri((String)("https://genie.netgear.com/UserProfile/#AppStorePlace:"));
            //    await Windows.System.Launcher.LaunchUriAsync(uri);
            //}         
        }

        private void LoginButton_Click(Object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
            MainPageInfo.navigatedPage = "MainPage";
        }

        private void LogoutButton_Click(Object sender, RoutedEventArgs e)
        {
            MainPageInfo.bLogin = false;
            btnLogin.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Collapsed;
        }

        private async void AboutButton_Click(Object sender, RoutedEventArgs e)
        {
            if (!AboutPopup.IsOpen)
            {
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.IsActive = true;
                GenieSoapApi soapApi = new GenieSoapApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await soapApi.GetCurrentSetting();
                if (dicResponse.Count > 0)
                {
                    if (dicResponse["Model"] != "")
                    {
                        tbRouterModel.Text = dicResponse["Model"];
                    }
                    else
                    {
                        tbRouterModel.Text = "N/A";
                    }

                    if (dicResponse["Firmware"] != "")
                    {
                        string regexFirmware = @"(\D+\d+).(\d+).(\d+).(\d+)";
                        Match FirmwareVersion = Regex.Match(dicResponse["Firmware"], regexFirmware);
                        tbFirmwareVersion.Text = FirmwareVersion.ToString();
                    }
                    else
                    {
                        tbFirmwareVersion.Text = "N/A";
                    }
                }
                else
                {
                    tbRouterModel.Text = "N/A";
                    tbFirmwareVersion.Text = "N/A";
                }
                AboutPopup.IsOpen = true;
                //PopupBackgroundTop.Visibility = Visibility.Visible;
                //PopupBackground.Visibility = Visibility.Visible;
                pleasewait.Visibility = Visibility.Collapsed;
                InProgress.IsActive = false;
                //CloseAboutButton.Visibility = Visibility.Visible;
                //LicenseButton.Visibility = Visibility.Visible;
            }
            if (LicensePopup.IsOpen)
            {
                LicensePopup.IsOpen = false;
                //CloseLicenseButton.Visibility = Visibility.Collapsed;
            }
        }

        //private async void Policy_Click(Object sender, RoutedEventArgs e)
        //{
        //    var uri = new Uri(((HyperlinkButton)sender).Tag.ToString());
        //    await Windows.System.Launcher.LaunchUriAsync(uri);
        //}

        private void CloseAboutButton_Click(Object sender, RoutedEventArgs e)
        {
            if (AboutPopup.IsOpen)
            {
                AboutPopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Visible;
                //CloseAboutButton.Visibility = Visibility.Collapsed;
                //LicenseButton.Visibility = Visibility.Collapsed;
            }
        }
        private void LicenseButton_Click(Object sender, RoutedEventArgs e)
        {
            if (!LicensePopup.IsOpen)
            {
                LicensePopup.IsOpen = true;
                AboutPopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                pleasewait.Visibility = Visibility.Collapsed;
                //CloseLicenseButton.Visibility = Visibility.Visible;
                //CloseAboutButton.Visibility = Visibility.Collapsed;
                //LicenseButton.Visibility = Visibility.Collapsed;
            }
        }
        private void CloseLicenseButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LicensePopup.IsOpen)
            {
                LicensePopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Visible;
                //CloseLicenseButton.Visibility = Visibility.Collapsed;
            }
        }

        public async Task<string> ReadDeviceInfoFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await storageFolder.GetFileAsync("CustomDeviceInfo.txt");    //CustomDeviceInfo.txt中保存本地修改的设备信息，包括设备MAC地址、设备名和设备类型，格式为"MACAddress,DeviceName,DeviceType;"
                if (file != null)
                {
                    fileContent = await FileIO.ReadTextAsync(file);
                }
            }
            catch (FileNotFoundException)
            {
                return fileContent;
            }
            return fileContent;
        }

        private IBuffer RandomAccessStreamToBuffer(IRandomAccessStream randomstream)
        {
            Stream stream = WindowsRuntimeStreamExtensions.AsStreamForRead(randomstream.GetInputStreamAt(0));
            MemoryStream memoryStream = new MemoryStream();
            if (stream != null)
            {
                byte[] bytes = ConvertStreamTobyte(stream);
                if (bytes != null)
                {
                    var binaryWriter = new BinaryWriter(memoryStream);
                    binaryWriter.Write(bytes);
                }
            }
            IBuffer buffer = WindowsRuntimeBufferExtensions.GetWindowsRuntimeBuffer(memoryStream, 0, (int)memoryStream.Length);
            return buffer;
        }


        public static byte[] ConvertStreamTobyte(Stream input)
        {
            byte[] buffer = new byte[16 * 1024];

            using (MemoryStream ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }

        private void SearchGrid_GotFocus(object sender, RoutedEventArgs e)
        {
            Uri _baseUri = new Uri("ms-appx:///");
            seach_bg_left.Source = new BitmapImage(new Uri(_baseUri, "Assets/MainPage/search_input_left.png"));
            seach_bg_middle.Source = new BitmapImage(new Uri(_baseUri, "Assets/MainPage/search_input_middle.png"));
            seach_bg_right.Source = new BitmapImage(new Uri(_baseUri, "Assets/MainPage/search_input_right.png"));
        }

        private void SearchGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            Uri _baseUri = new Uri("ms-appx:///");
            seach_bg_left.Source = new BitmapImage(new Uri(_baseUri, "Assets/MainPage/search_normal_left.png"));
            seach_bg_middle.Source = new BitmapImage(new Uri(_baseUri, "Assets/MainPage/search_normal_middle.png"));
            seach_bg_right.Source = new BitmapImage(new Uri(_baseUri, "Assets/MainPage/search_normal_right.png"));
        }

        private async void Image_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            string text = SearchText.Text.Trim();
            if (text.Contains("%"))
            {
                text = text.Replace("%", "");
            }
            if (text.Contains(" "))
            {
                text = text.Replace(" ", "");
            }
            if (text.Contains("&"))
            {
                text = text.Replace("&", "");
            }
            if (text.Contains("*"))
            {
                text = text.Replace("*", "");
            }
            if (text.Contains(":"))
            {
                text = text.Replace(":", "");
            }
            if (text != "")
            {
                text = Uri.EscapeUriString(text);
                var uri = new Uri((String)("http://support.netgear.com/search/" + text));
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }
    }
}
