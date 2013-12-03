using GenieWin8.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GenieWin8.DataModel;
using Windows.UI.Popups;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI;
using Windows.Networking.Connectivity;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class LoginPage : GenieWin8.Common.LayoutAwarePage
    {
        public LoginPage()
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
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
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

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="pageState">要使用可序列化状态填充的空字典。</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private void OnWindowSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            stpLoginalertinfo.Width = Window.Current.Bounds.Width;
        }

        private async void LoginButton_Click(Object sender, RoutedEventArgs e)
        {
            InProgress.IsActive = true;
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
                    try
                    {
                        var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                        foreach (var connectionProfile in ConnectionProfiles)
                        {
                            if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                            {
                                MainPageInfo.ssid = connectionProfile.ProfileName;                                                 //保存登陆成功后所连接WiFi的SSID
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                    }
                    
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;                   
                    //this.Frame.GoBack();
                    NavigatedToPage();
                }
                else if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 401)
                {
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    var strtext = loader.GetString("bad_password");
                    var messageDialog = new MessageDialog(strtext);
                    await messageDialog.ShowAsync();
                }
            }
            else
            {
                InProgress.IsActive = false;
                PopupBackgroundTop.Opacity = 0.5;
                PopupBackground.Opacity = 0.5;
                LoginalertPopup.IsOpen = true;
            }
        }

        public async void WritePasswordToFile()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync("Password.txt", CreationCollisionOption.ReplaceExisting);
            try
            {
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, MainPageInfo.rememberpassword);
                }
            }
            catch (FileNotFoundException)
            {

            }
        }

        public async Task<string> ReadPasswordFromFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await storageFolder.GetFileAsync("Password.txt");
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

        private async void NavigatedToPage()
        {
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            UtilityTool util = new UtilityTool();
            Dictionary<string, Dictionary<string, string>> responseDic = new Dictionary<string, Dictionary<string, string>>();
            switch (MainPageInfo.navigatedPage)
            {
                case "MainPage":
                    this.GoHome(null, null);
                    break;

                case "WifiSettingPage":
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
                    attachDeviceAll = await soapApi.GetAttachDevice();
                    //UtilityTool util = new UtilityTool();
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
                    dicResponse = new Dictionary<string, string>();
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
                    break;

                case "GuestAccessPage":
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    //Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = new Dictionary<string, string>();
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
                    break;

                case "NetworkMapPage":
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    //UtilityTool util = new UtilityTool();
                    NetworkMapInfo.geteway = await util.GetGateway();
                    NetworkMapInfo.attachDeviceDic = await soapApi.GetAttachDevice();
                    //Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = new Dictionary<string, string>();
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
                    break;

                case "TrafficMeterPage":
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    //Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = new Dictionary<string, string>();
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
                    break;

                case "ParentalControlPage":
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;

                    //这里需要判断是否已连接因特网
                    //UtilityTool util = new UtilityTool();
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
                        //Dictionary<string, string> dicResponse = new Dictionary<string, string>();
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
                    break;
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

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            LoginalertPopup.IsOpen = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
            PopupBackgroundTop.Opacity = 0.9;
            PopupBackground.Opacity = 0.9;
        }

        private void Button_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            CloseButton.Background = new SolidColorBrush(Color.FromArgb(255, 98, 98, 255));
        }

        private void CloseButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            CloseButton.Background = new SolidColorBrush(Color.FromArgb(255, 48, 48, 255));
        }

    }
}
