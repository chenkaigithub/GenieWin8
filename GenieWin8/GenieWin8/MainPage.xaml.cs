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
using System.Threading.Tasks;
using Windows.UI.Popups;

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
            if (MainPageInfo.bLogin)	//已登陆
            {
                GenieSoapApi soapApi = new GenieSoapApi();                
                if (groupId == "WiFiSetting")
                {
                    //WifiInfoModel wifiInfo = new WifiInfoModel ();
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = await soapApi.GetInfo("WLANConfiguration");
                    WifiInfoModel.ssid = dicResponse["NewSSID"];
                    WifiInfoModel.channel = dicResponse["NewChannel"];
                    WifiInfoModel.securityType = dicResponse["NewWPAEncryptionModes"];
                    dicResponse = await soapApi.GetWPASecurityKeys();
                    WifiInfoModel.password = dicResponse["NewWPAPassphrase"];
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    this.Frame.Navigate(typeof(WifiSettingPage));
                }
                else if (groupId == "GuestAccess")
                {
                    //await soapApi.GetGuestAccessNetworkInfo();
                    //await soapApi.SetGuestAccessEnabled();
                    //await soapApi.SetGuestAccessEnabled2("NETGEAR-Guest", "WPA2-PSK", "stieview");
                    //await soapApi.SetGuestAccessNetwork("NETGEAR-Guest", "None", "");
                    //await soapApi.GetCurrentSetting();
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = await soapApi.GetGuestAccessEnabled();
                    GuestAccessInfoModel.isGuestAccessEnabled = dicResponse["NewGuestAccessEnabled"];
                    if (dicResponse["NewGuestAccessEnabled"] == "0" || dicResponse["NewGuestAccessEnabled"] == "1")
                    {
                        if (dicResponse["NewGuestAccessEnabled"] == "1")
                        {
                            Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                            dicResponse2 = await soapApi.GetGuestAccessNetworkInfo();
                            GuestAccessInfoModel.ssid = dicResponse2["NewSSID"];
                            GuestAccessInfoModel.securityType = dicResponse2["NewSecurityMode"];
                            if (dicResponse2["NewSecurityMode"] != "None")
                                GuestAccessInfoModel.password = dicResponse2["NewKey"];
                            else
                                GuestAccessInfoModel.password = "";
                        }
                        if (GuestAccessInfoModel.timePeriod == null)
                            GuestAccessInfoModel.timePeriod = "Always";
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
                        var messageDialog = new MessageDialog("The router does not support this function.");
                        await messageDialog.ShowAsync();
                    }
                }
                else if (groupId == "NetworkMap")
                {
                    InProgress.IsActive = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    UtilityTool util = new UtilityTool();
                    NetworkMapDodel.geteway = await util.GetGateway();
                    Dictionary<string, Dictionary<string, string>> responseDic = new Dictionary<string, Dictionary<string, string>>();
                    responseDic = await soapApi.GetAttachDevice();
                    NetworkMapDodel.attachDeviceDic = responseDic;

                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = await soapApi.GetInfo("WLANConfiguration");
                    WifiInfoModel.ssid = dicResponse["NewSSID"];
                    WifiInfoModel.channel = dicResponse["NewChannel"];
                    WifiInfoModel.securityType = dicResponse["NewWPAEncryptionModes"];
                    WifiInfoModel.macAddr = dicResponse["NewWLANMACAddress"];
                    NetworkMapDodel.fileContent = await ReadDeviceInfoFile();
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    this.Frame.Navigate(typeof(NetworkMapPage));
                }
                else if (groupId == "TrafficMeter")
                {
                    this.Frame.Navigate(typeof(TrafficMeterPage));
                }
                else if (groupId == "ParentalControl")
                {
                    await soapApi.GetDNSMasqDeviceID();
                    this.Frame.Navigate(typeof(ParentalControlPage));
                }               
            }
            else	//未登录，跳到登陆页面
            {
                this.Frame.Navigate(typeof(LoginPage));
            }
            if (groupId == "MyMedia")
            {
                this.Frame.Navigate(typeof(MyMediaPage));
            }
            else if (groupId == "MarketPlace")
            {
                var uri = new Uri((String)("https://genie.netgear.com/UserProfile/#AppStorePlace:"));
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }

        private void SearchButton_Click(Object sender, RoutedEventArgs e)
        {
        }
        private void LoginButton_Click(Object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(LoginPage));
        }
        private void LogoutButton_Click(Object sender, RoutedEventArgs e)
        {
            MainPageInfo.bLogin = false;
            btnLogin.Visibility = Visibility.Visible;
            btnLogout.Visibility = Visibility.Collapsed;
        }
        private void AboutButton_Click(Object sender, RoutedEventArgs e)
        {
            if (!AboutPopup.IsOpen)
            {
                AboutPopup.IsOpen = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                CloseAboutButton.Visibility = Visibility.Visible;
                LicenseButton.Visibility = Visibility.Visible;
            }
            if (LicensePopup.IsOpen)
            {
                LicensePopup.IsOpen = false;
                CloseLicenseButton.Visibility = Visibility.Collapsed;
            }
        }
        private void CloseAboutButton_Click(Object sender, RoutedEventArgs e)
        {
            if (AboutPopup.IsOpen)
            {
                AboutPopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                CloseAboutButton.Visibility = Visibility.Collapsed;
                LicenseButton.Visibility = Visibility.Collapsed;
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
                CloseLicenseButton.Visibility = Visibility.Visible;
                CloseAboutButton.Visibility = Visibility.Collapsed;
                LicenseButton.Visibility = Visibility.Collapsed;
            }
        }
        private void CloseLicenseButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LicensePopup.IsOpen)
            {
                LicensePopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                CloseLicenseButton.Visibility = Visibility.Collapsed;
            }
        }

        public async Task<string> ReadDeviceInfoFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
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

            }
            return fileContent;
        }

    }
}
