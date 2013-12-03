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
using Microsoft.Phone.Net.NetworkInformation;

namespace GenieWP8
{
    public partial class BypassAccountLogoutPage : PhoneApplicationPage
    {
        private static bool IsWifiSsidChanged;
        public BypassAccountLogoutPage()
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

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //判断所连接Wifi的Ssid是否改变
            IsWifiSsidChanged = true;
            foreach (var network in new NetworkInterfaceList())
            {
                if ((network.InterfaceType == NetworkInterfaceType.Wireless80211) && (network.InterfaceState == ConnectState.Connected))
                {
                    if (network.InterfaceName == MainPageInfo.ssid)
                        IsWifiSsidChanged = false;
                    else
                        IsWifiSsidChanged = true;
                }
            }
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

        private void LoadState()
        {
            LoggedInAccount.Text = AppResources.LoggedInBypass + " " + ParentalControlInfo.BypassUsername;
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
            NavigationService.GoBack();
        }

        //取消按钮事件
        private void CancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        //登录按钮事件
        private async void LogoutButton_Click(object sender, EventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                MainPageInfo.navigatedPage = "ParentalControlPage";
            } 
            else
            {
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                GenieSoapApi soapApi = new GenieSoapApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                UtilityTool util = new UtilityTool();
                string MacAddress = util.GetLocalMacAddress();  //获取本机mac地址
                MacAddress = MacAddress.Replace(":", "");
                dicResponse = await soapApi.DeleteMACAddress(MacAddress);
                ParentalControlInfo.BypassUsername = "";
                ParentalControlInfo.BypassChildrenDeviceId = "";
                WriteChildrenDeviceIdToFile();                  //登录成功后将childrenDeviceId保存到本地，如果未注销则以后登录Genie时，通过读取本地DeviceId获得当前登录的Bypass账户
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                NavigationService.Navigate(new Uri("/ParentalControlPage.xaml", UriKind.Relative));
            }
        }

        public async void WriteChildrenDeviceIdToFile()
        {
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (!fileStorage.FileExists("Bypass_childrenDeviceId.txt"))
                {
                    using (var file = fileStorage.CreateFile("Bypass_childrenDeviceId.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(ParentalControlInfo.BypassChildrenDeviceId);
                        }
                    }
                }
                else
                {
                    fileStorage.DeleteFile("Bypass_childrenDeviceId.txt");
                    using (var file = fileStorage.CreateFile("Bypass_childrenDeviceId.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(ParentalControlInfo.BypassChildrenDeviceId);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}