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
using System.Windows.Threading;

namespace GenieWP8
{
    public partial class WifiEditSettingPage : PhoneApplicationPage
    {
        private static WifiSettingModel settingModel = null;       
        public WifiEditSettingPage()
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

            // 绑定数据
            if (settingModel == null)
                settingModel = new WifiSettingModel();
            DataContext = settingModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();

        }

        // 为 WifiSettingModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (!settingModel.IsDataLoaded)
            //{
            //    settingModel.LoadData();
            //}
            settingModel.SettingGroups.Clear();
            settingModel.EditChannelSecurity.Clear();
            settingModel.LoadData();
            if (WifiSettingInfo.changedSecurityType == "None")
            {
                passwordPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                passwordPanel.Visibility = Visibility.Visible;
            }

            //判断保存按钮是否可点击
            if (WifiSettingInfo.isChannelChanged == true || WifiSettingInfo.isSecurityTypeChanged == true)
            {
                if (WifiSettingInfo.securityType == "None")
                {
                    pwd.Text = "siteview";
                }
                appBarButton_save.IsEnabled = true;
            }
            else
            {
                appBarButton_save.IsEnabled = false;
            }  
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

        //判断SSID是否更改以及保存按钮是否可点击
        private void ssid_changed(Object sender, RoutedEventArgs e)
        {
            string ssid = SSID.Text.Trim();
            if (ssid != WifiSettingInfo.ssid && ssid != "")
            {
                appBarButton_save.IsEnabled = true;
                WifiSettingInfo.isSSIDChanged = true;
            }
            else
            {
                WifiSettingInfo.isSSIDChanged = false;
                if (WifiSettingInfo.isPasswordChanged == true || WifiSettingInfo.isChannelChanged == true || WifiSettingInfo.isSecurityTypeChanged == true)
                {
                    appBarButton_save.IsEnabled = true;
                }
                else
                {
                    appBarButton_save.IsEnabled = false;
                }
            }
        }

        //判断密码是否更改以及保存按钮是否可点击
        private void pwd_changed(Object sender, RoutedEventArgs e)
        {
            string password = pwd.Text.Trim();
            if (password != WifiSettingInfo.password && password != "")
            {
                appBarButton_save.IsEnabled = true;
                WifiSettingInfo.isPasswordChanged = true;
            }
            else
            {
                WifiSettingInfo.isPasswordChanged = false;
                if (WifiSettingInfo.isSSIDChanged == true || WifiSettingInfo.isChannelChanged == true || WifiSettingInfo.isSecurityTypeChanged == true)
                {
                    appBarButton_save.IsEnabled = true;
                }
                else
                {
                    appBarButton_save.IsEnabled = false;
                }
            }
        }

         //处理在 LongListSelector 中更改的选定内容
        private void channel_securitySetting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果所选项为空(没有选定内容)，则不执行任何操作
            if (channel_securitySettingLongListSelector.SelectedItem == null)
                return;

            // 导航到新页面
            var groupId = ((SettingGroup)channel_securitySettingLongListSelector.SelectedItem).ID;
            if (groupId == "Channel")
            {
                NavigationService.Navigate(new Uri("/WifiEditChannelPage.xaml", UriKind.Relative));
            }
            else if (groupId == "Security")
            {
                NavigationService.Navigate(new Uri("/WifiEditSecurityPage.xaml", UriKind.Relative));
            }           

            // 将所选项重置为 null (没有选定内容)
            channel_securitySettingLongListSelector.SelectedItem = null;
        }

        //用于生成本地化 ApplicationBar 的代码
        ApplicationBarIconButton appBarButton_back = new ApplicationBarIconButton(new Uri("Assets/back.png", UriKind.Relative));
        ApplicationBarIconButton appBarButton_save = new ApplicationBarIconButton(new Uri("Assets/save.png", UriKind.Relative));
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            //后退按钮
            //ApplicationBarIconButton appBarButton_back = new ApplicationBarIconButton(new Uri("Assets/back.png", UriKind.Relative));
            appBarButton_back.Text = AppResources.btnBack;
            ApplicationBar.Buttons.Add(appBarButton_back);
            appBarButton_back.Click += new EventHandler(appBarButton_back_Click);

            //保存按钮
            //ApplicationBarIconButton appBarButton_save = new ApplicationBarIconButton(new Uri("Assets/save.png", UriKind.Relative));
            appBarButton_save.Text = AppResources.RefreshButtonContent;
            ApplicationBar.Buttons.Add(appBarButton_save);
            appBarButton_save.Click += new EventHandler(appBarButton_save_Click);
        }

        //返回按钮响应事件
        private async void appBarButton_back_Click(object sender, EventArgs e)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetInfo("WLANConfiguration");
            if (dicResponse.Count > 0)
            {
                WifiSettingInfo.ssid = dicResponse["NewSSID"];
                WifiSettingInfo.channel = dicResponse["NewChannel"];
                WifiSettingInfo.changedChannel = dicResponse["NewChannel"];
                WifiSettingInfo.securityType = dicResponse["NewWPAEncryptionModes"];
                WifiSettingInfo.changedSecurityType = dicResponse["NewWPAEncryptionModes"];
            }
            dicResponse = await soapApi.GetWPASecurityKeys();
            if (dicResponse.Count > 0)
            {
                WifiSettingInfo.password = dicResponse["NewWPAPassphrase"];
            }
            WifiSettingInfo.isSSIDChanged = false;
            WifiSettingInfo.isPasswordChanged = false;
            WifiSettingInfo.isChannelChanged = false;
            WifiSettingInfo.isSecurityTypeChanged = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(new Uri("/WifiSettingPage.xaml", UriKind.Relative));
        }

        //重写手机“返回”按钮事件
        protected async override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetInfo("WLANConfiguration");
            if (dicResponse.Count > 0)
            {
                WifiSettingInfo.ssid = dicResponse["NewSSID"];
                WifiSettingInfo.channel = dicResponse["NewChannel"];
                WifiSettingInfo.changedChannel = dicResponse["NewChannel"];
                WifiSettingInfo.securityType = dicResponse["NewWPAEncryptionModes"];
                WifiSettingInfo.changedSecurityType = dicResponse["NewWPAEncryptionModes"];
            }
            dicResponse = await soapApi.GetWPASecurityKeys();
            if (dicResponse.Count > 0)
            {
                WifiSettingInfo.password = dicResponse["NewWPAPassphrase"];
            }
            WifiSettingInfo.isSSIDChanged = false;
            WifiSettingInfo.isPasswordChanged = false;
            WifiSettingInfo.isChannelChanged = false;
            WifiSettingInfo.isSecurityTypeChanged = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
            NavigationService.Navigate(new Uri("/WifiSettingPage.xaml", UriKind.Relative));
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

        
        //保存按钮响应事件
        private void appBarButton_save_Click(object sender, EventArgs e)
        {
            PopupEnquire.IsOpen = true;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Collapsed;
            pleasewait.Visibility = Visibility.Collapsed;
            waittime.Visibility = Visibility.Collapsed;
        }

        DispatcherTimer timer = new DispatcherTimer();      //计时器
        //“是”按钮响应事件
        private async void YesButton_Click(Object sender, RoutedEventArgs e)
        {
            PopupEnquire.IsOpen = false;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            waittime.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();

            string ssid = SSID.Text.Trim();
            string password = pwd.Text.Trim();
            if (ssid != "" && ssid != null)
            {
                if (WifiSettingInfo.securityType.CompareTo("None") == 0)
                {
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += timer_Tick;
                    timer.Start();
                    await soapApi.SetWLANNoSecurity(ssid, WifiSettingInfo.region, WifiSettingInfo.changedChannel, WifiSettingInfo.wirelessMode);
                    WifiSettingInfo.channel = WifiSettingInfo.changedChannel;
                    WifiSettingInfo.isSSIDChanged = false;
                    WifiSettingInfo.isPasswordChanged = false;
                    WifiSettingInfo.isChannelChanged = false;
                    WifiSettingInfo.isSecurityTypeChanged = false;
                    appBarButton_save.IsEnabled = false;
                }
                else
                {
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += timer_Tick;
                    timer.Start();
                    await soapApi.SetWLANWEPByPassphrase(ssid, WifiSettingInfo.region, WifiSettingInfo.changedChannel, WifiSettingInfo.wirelessMode, WifiSettingInfo.changedSecurityType, password);
                    WifiSettingInfo.channel = WifiSettingInfo.changedChannel;
                    WifiSettingInfo.securityType = WifiSettingInfo.changedSecurityType;
                    WifiSettingInfo.isSSIDChanged = false;
                    WifiSettingInfo.isPasswordChanged = false;
                    WifiSettingInfo.isChannelChanged = false;
                    WifiSettingInfo.isSecurityTypeChanged = false;
                    appBarButton_save.IsEnabled = false;
                }
            }
        }

        //“否”按钮响应事件
        private void NoButton_Click(Object sender, RoutedEventArgs e)
        {
            PopupEnquire.IsOpen = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        int count = 90;     //倒计时间
        void timer_Tick(object sender, object e)
        {
            waittime.Text = count.ToString();
            count--;
            if (count < 0)
            {
                timer.Stop();
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                MessageBox.Show(AppResources.wirelesssettinrelogin);
                MainPageInfo.bLogin = false;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}