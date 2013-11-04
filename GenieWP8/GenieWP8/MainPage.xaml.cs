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
using GenieWP8.ViewModels;
using GenieWP8.DataInfo;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Phone.Net.NetworkInformation;

namespace GenieWP8
{
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 将 LongListSelector 控件的数据上下文设置为绑定数据
            DataContext = App.ViewModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();
        }

        // 为 ViewModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            //清空BackStack,使得到MainPage能正确退出程序
            int count = NavigationService.BackStack.Count();
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    NavigationService.RemoveBackEntry();
                }
            }

            // 更新菜单项。
            ApplicationBar.MenuItems.Clear();
            if (MainPageInfo.bLogin)
            {
                ApplicationBarMenuItem appBarMenuItem_Logout = new ApplicationBarMenuItem(AppResources.LogoutButtonContent);
                ApplicationBar.MenuItems.Add(appBarMenuItem_Logout);
                double width = System.Windows.Application.Current.Host.Content.ActualWidth;
                appBarMenuItem_Logout.Click += new EventHandler(appBarMenuItem_Logout_Click);
            }
            else
            {
                ApplicationBarMenuItem appBarMenuItem_Login = new ApplicationBarMenuItem(AppResources.LoginButtonContent);
                ApplicationBar.MenuItems.Add(appBarMenuItem_Login);
                double width = System.Windows.Application.Current.Host.Content.ActualWidth;
                appBarMenuItem_Login.Click += new EventHandler(appBarMenuItem_Login_Click);
            }
        }

        //用于生成本地化 ApplicationBar 的代码
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            //搜索按钮
            //ApplicationBarIconButton appBarButton_search = new ApplicationBarIconButton(new Uri("Assets/search.png", UriKind.Relative));
            //appBarButton_search.Text = AppResources.SearchText;
            //ApplicationBar.Buttons.Add(appBarButton_search);
            //appBarButton_search.Click += new EventHandler(appBarButton_search_Click);
     
            //关于按钮
            ApplicationBarIconButton appBarButton_about = new ApplicationBarIconButton(new Uri("Assets/questionmark.png", UriKind.Relative));
            appBarButton_about.Text = AppResources.AboutText;
            ApplicationBar.Buttons.Add(appBarButton_about);
            appBarButton_about.Click += new EventHandler(appBarButton_about_Click);

            // 使用 AppResources 中的本地化字符串创建新菜单项。
            ApplicationBarMenuItem appBarMenuItem_Login = new ApplicationBarMenuItem(AppResources.LoginButtonContent);
            ApplicationBar.MenuItems.Add(appBarMenuItem_Login);
            double width = System.Windows.Application.Current.Host.Content.ActualWidth;
            appBarMenuItem_Login.Click += new EventHandler(appBarMenuItem_Login_Click);
        }

        //横竖屏切换响应事件
        //private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        //{
        //    // Switch the placement of the buttons based on an orientation change.
        //    if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
        //    {
        //        //ContentPanel.Margin = new Thickness(12, 0, 12, 0);
        //        MainItemsPivot.Margin = new Thickness(12, 0, 12, 0);
        //        LicenseScrollViewer.Height = 450;
        //    }
        //    // If not in portrait, move buttonList content to visible row and column.
        //    else
        //    {
        //        //ContentPanel.Margin = new Thickness(42, 0, 12, 0);
        //        MainItemsPivot.Margin = new Thickness(42, 0, 12, 0);
        //        LicenseScrollViewer.Height = 250;
        //    }
        //}

        private async void appBarButton_about_Click(object sender, EventArgs e)
        {
            if (!AboutPopup.IsOpen)
            {
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Visible;
                pleasewait.Visibility = Visibility.Visible;
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
                //tbSearch.Visibility = Visibility.Collapsed;
                //btnSearch.Visibility = Visibility.Collapsed;
                //PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
            }
            if (LicensePopup.IsOpen)
            {
                LicensePopup.IsOpen = false;
            }
            
        }

        private void appBarMenuItem_Logout_Click(object sender, EventArgs e)
        {
            MainPageInfo.bLogin = false;
            ApplicationBar.MenuItems.Clear();
            ApplicationBarMenuItem appBarMenuItem_Login = new ApplicationBarMenuItem(AppResources.LoginButtonContent);
            ApplicationBar.MenuItems.Add(appBarMenuItem_Login);
            double width = System.Windows.Application.Current.Host.Content.ActualWidth;
            appBarMenuItem_Login.Click += new EventHandler(appBarMenuItem_Login_Click);
        }

        private void appBarMenuItem_Login_Click(object sender, EventArgs e)
        {            
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
            MainPageInfo.navigatedPage = "MainPage";
        }

        //搜索按钮响应事件
        //private async void SearchButton_Click(Object sender, RoutedEventArgs e)
        //{
        //    string text = tbSearch.Text.Trim();
        //    if (text != "")
        //    {
        //        var uri = new Uri((String)("http://support.netgear.com/search/" + text));
        //        await Windows.System.Launcher.LaunchUriAsync(uri);
        //    }
        //}

        //隐私权政策链接响应事件
        private async void Policy_Click(Object sender, RoutedEventArgs e)
        {
            var uri = new Uri(((HyperlinkButton)sender).Tag.ToString());
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        //关闭“关于”事件
        private void CloseAboutButton_Click(Object sender, RoutedEventArgs e)
        {
            if (AboutPopup.IsOpen)
            {
                AboutPopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
        }

        //点击“许可”响应事件
        private void LicenseButton_Click(Object sender, RoutedEventArgs e)
        {
            if (!LicensePopup.IsOpen)
            {
                LicensePopup.IsOpen = true;
                AboutPopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                //tbSearch.Visibility = Visibility.Collapsed;
                //btnSearch.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
            }
        }

        //关闭“许可”事件
        private void CloseLicenseButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LicensePopup.IsOpen)
            {
                LicensePopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
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

        private void tbSearch_GotFocus(object sender, RoutedEventArgs e)
        {
            tbSearch.Background =new SolidColorBrush(Colors.White);
        }

        private async void Image_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            string text = tbSearch.Text.Trim();
            if (text != "")
            {
                text = Uri.EscapeDataString(text);
                var uri = new Uri((String)("http://support.netgear.com/search/" + text));
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }

        private async void MainItem_Click(object sender, RoutedEventArgs e)
        {
            Button btnItem = (Button)sender;
            string btnName = btnItem.Name;
            GenieSoapApi soapApi = new GenieSoapApi();
            //无线设置
            if (btnName == "WirelessSetting")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    InProgress.Visibility = Visibility.Visible;
                    pleasewait.Visibility = Visibility.Visible;

                    Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
                    attachDeviceAll = await soapApi.GetAttachDevice();
                    UtilityTool util = new UtilityTool();
                    var ipList = util.GetCurrentIpAddresses();
                    string loacalIp = ipList.ToList()[0];
                    foreach (string key in attachDeviceAll.Keys)
                    {
                        if (loacalIp == attachDeviceAll[key]["Ip"])
                        {
                            WifiSettingInfo.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                            WifiSettingInfo.signalStrength = attachDeviceAll[key]["Signal"] + "%";
                        }
                    }

                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = await soapApi.GetInfo("WLANConfiguration");
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
                    dicResponse = await soapApi.GetWPASecurityKeys();
                    if (dicResponse.Count > 0)
                    {
                        WifiSettingInfo.password = dicResponse["NewWPAPassphrase"];
                        WifiSettingInfo.changedPassword = dicResponse["NewWPAPassphrase"];
                    }
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    NavigationService.Navigate(new Uri("/WifiSettingPage.xaml", UriKind.Relative));
                }
                else	//未登陆
                {
                    NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                    MainPageInfo.navigatedPage = "WifiSettingPage";
                }
            }
            //访客访问
            else if (btnName == "GuestAccess")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    InProgress.Visibility = Visibility.Visible;
                    pleasewait.Visibility = Visibility.Visible;
                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = await soapApi.GetGuestAccessEnabled();
                    GuestAccessInfo.isGuestAccessEnabled = dicResponse["NewGuestAccessEnabled"];
                    if (dicResponse["NewGuestAccessEnabled"] == "0" || dicResponse["NewGuestAccessEnabled"] == "1")
                    {
                        Dictionary<string, string> dicResponse1 = new Dictionary<string, string>();
                        dicResponse1 = await soapApi.GetGuestAccessNetworkInfo();
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
                }
                else	//未登陆
                {
                    NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                    MainPageInfo.navigatedPage = "GuestAccessPage";
                }
            }
            //网络映射
            else if (btnName == "NetworkMap")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    InProgress.Visibility = Visibility.Visible;
                    pleasewait.Visibility = Visibility.Visible;
                    UtilityTool util = new UtilityTool();
                    NetworkMapInfo.geteway = await util.GetGateway();
                    Dictionary<string, Dictionary<string, string>> responseDic = new Dictionary<string, Dictionary<string, string>>();
                    responseDic = await soapApi.GetAttachDevice();
                    NetworkMapInfo.attachDeviceDic = responseDic;

                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = await soapApi.GetInfo("WLANConfiguration");
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
                }
                else	//未登陆
                {
                    NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                    MainPageInfo.navigatedPage = "NetworkMapPage";
                }
            }
            //流量控制
            else if (btnName == "TrafficMeter")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    InProgress.Visibility = Visibility.Visible;
                    pleasewait.Visibility = Visibility.Visible;
                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = await soapApi.GetTrafficMeterEnabled();
                    TrafficMeterInfo.isTrafficMeterEnabled = dicResponse["NewTrafficMeterEnable"];
                    if (dicResponse["NewTrafficMeterEnable"] == "0" || dicResponse["NewTrafficMeterEnable"] == "1")
                    {
                        Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                        dicResponse2 = await soapApi.GetTrafficMeterOptions();
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
                        dicResponse2 = await soapApi.GetTrafficMeterStatistics();
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
                }
                else	//未登陆
                {
                    NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                    MainPageInfo.navigatedPage = "TrafficMeterPage";
                }
            }
            //家长控制
            else if (btnName == "ParentalControl")
            {
                if (MainPageInfo.bLogin)	//已登陆
                {
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    InProgress.Visibility = Visibility.Visible;
                    pleasewait.Visibility = Visibility.Visible;

                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
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
                                Dictionary<string, Dictionary<string, string>> responseDic = new Dictionary<string, Dictionary<string, string>>();
                                responseDic = await soapApi.GetAttachDevice();
                                NetworkMapInfo.attachDeviceDic = responseDic;

                                Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                                dicResponse2 = await soapApi.GetInfo("WLANConfiguration");
                                ParentalControlInfo.RouterMacaddr = dicResponse2["NewWLANMACAddress"];

                                dicResponse2 = await soapApi.GetEnableStatus();
                                ParentalControlInfo.isParentalControlEnabled = dicResponse2["ParentalControl"];
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
                }
                else	//未登陆
                {
                    NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                    MainPageInfo.navigatedPage = "ParentalControlPage";
                }
            }
            //我的媒体
            //else if (gridName == "MyMedia")
            //{
            //    //NavigationService.Navigate(new Uri("/MyMediaSourcePage.xaml", UriKind.Relative));    
            //}
            //QRCode
            else if (btnName == "QRCode")
            {
                NavigationService.Navigate(new Uri("/QRCodePage.xaml", UriKind.Relative));
            }
            //MarketPlace
            //else if (gridName == "MarketPlace")
            //{
            //    var uri = new Uri((String)("https://genie.netgear.com/UserProfile/#AppStorePlace:"));
            //    await Windows.System.Launcher.LaunchUriAsync(uri);
            //}
        }

        private void SearchGrid_GotFocus(object sender, RoutedEventArgs e)
        {            
            ImageBrush SearchBackground = new ImageBrush();
            SearchBackground.ImageSource = new BitmapImage(new Uri("Assets/MainPage/search_input.png", UriKind.Relative));
            SearchGrid.Background = SearchBackground;
        }

        private void SearchGrid_LostFocus(object sender, RoutedEventArgs e)
        {
            ImageBrush SearchBackground = new ImageBrush();
            SearchBackground.ImageSource = new BitmapImage(new Uri("Assets/MainPage/search_normal.png", UriKind.Relative));
            SearchGrid.Background = SearchBackground;
        }        

        private void tbSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (tbSearch.Text.Contains("%"))
            {
                int CaretPos = tbSearch.SelectionStart;
                tbSearch.Text = tbSearch.Text.Replace("%", "");
                tbSearch.SelectionStart = CaretPos - 1;
            }
            
            if (tbSearch.Text.Contains(" "))
            {
                int CaretPos = tbSearch.SelectionStart;
                tbSearch.Text = tbSearch.Text.Replace(" ", "");
                tbSearch.SelectionStart = CaretPos - 1;
            }
        }
    }
}