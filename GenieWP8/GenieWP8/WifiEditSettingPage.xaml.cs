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
using System.Windows.Media;

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
            //settingModel.SettingGroups.Clear();
            settingModel.EditChannelSecurity.Clear();
            settingModel.LoadData();
            SSID.Text = WifiSettingInfo.changedSsid;
            pwd.Text = WifiSettingInfo.changedPassword;
            if (WifiSettingInfo.changedSecurityType == "None")
            {
                pwd.Text = "";
                passwordPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                passwordPanel.Visibility = Visibility.Visible;
            }

            //判断保存按钮是否可点击
            if (SSID.Text != "" && ((passwordPanel.Visibility == Visibility.Visible && pwd.Text != "") || passwordPanel.Visibility == Visibility.Collapsed) &&
                (WifiSettingInfo.isSSIDChanged == true || WifiSettingInfo.isPasswordChanged == true || WifiSettingInfo.isChannelChanged == true || WifiSettingInfo.isSecurityTypeChanged == true))
            {
                //if (WifiSettingInfo.securityType == "None")
                //{
                //    pwd.Text = "siteview";
                //}
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
            string password = pwd.Text.Trim();
            WifiSettingInfo.changedSsid = ssid;
            if (ssid != WifiSettingInfo.ssid && ssid != "" && ((passwordPanel.Visibility == Visibility.Visible && password != "") || passwordPanel.Visibility == Visibility.Collapsed))
            {
                appBarButton_save.IsEnabled = true;
                WifiSettingInfo.isSSIDChanged = true;
            }
            else if (ssid == "" || (passwordPanel.Visibility == Visibility.Visible && password == ""))
            {
                appBarButton_save.IsEnabled = false;
                WifiSettingInfo.isSSIDChanged = true;                
            }
            else
            {
                WifiSettingInfo.isSSIDChanged = false;
                if ((passwordPanel.Visibility == Visibility.Visible && WifiSettingInfo.isPasswordChanged == true) || WifiSettingInfo.isChannelChanged == true || WifiSettingInfo.isSecurityTypeChanged == true)
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
            string ssid = SSID.Text.Trim();
            string password = pwd.Text.Trim();
            WifiSettingInfo.changedPassword = password;
            if (password != WifiSettingInfo.password && password != "" && ssid != "")
            {
                appBarButton_save.IsEnabled = true;
                WifiSettingInfo.isPasswordChanged = true;
            }
            else if (password == "" || ssid == "")
            {
                appBarButton_save.IsEnabled = false;
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

        // //处理在 LongListSelector 中更改的选定内容
        //private void channel_securitySetting_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // 如果所选项为空(没有选定内容)，则不执行任何操作
        //    if (channel_securitySettingLongListSelector.SelectedItem == null)
        //        return;

        //    // 导航到新页面
        //    var groupId = ((SettingGroup)channel_securitySettingLongListSelector.SelectedItem).ID;
        //    if (groupId == "Channel")
        //    {
        //        NavigationService.Navigate(new Uri("/WifiEditChannelPage.xaml", UriKind.Relative));
        //    }
        //    else if (groupId == "Security")
        //    {
        //        NavigationService.Navigate(new Uri("/WifiEditSecurityPage.xaml", UriKind.Relative));
        //    }           

        //    // 将所选项重置为 null (没有选定内容)
        //    channel_securitySettingLongListSelector.SelectedItem = null;
        //}

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
            appBarButton_save.Text = AppResources.SaveButtonContent;
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
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.GetInfo("WLANConfiguration");
            }
            if (dicResponse.Count > 0)
            {
                WifiSettingInfo.ssid = dicResponse["NewSSID"];
                WifiSettingInfo.changedSsid = dicResponse["NewSSID"];
                WifiSettingInfo.channel = dicResponse["NewChannel"];
                WifiSettingInfo.changedChannel = dicResponse["NewChannel"];
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
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.GetInfo("WLANConfiguration");
            }
            if (dicResponse.Count > 0)
            {
                WifiSettingInfo.ssid = dicResponse["NewSSID"];
                WifiSettingInfo.changedSsid = dicResponse["NewSSID"];
                WifiSettingInfo.channel = dicResponse["NewChannel"];
                WifiSettingInfo.changedChannel = dicResponse["NewChannel"];
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
            string ssid = SSID.Text;
            string password = pwd.Text;
            bool IsSsidSBC = IsAllowChar(ssid);
            bool IsPasswordSBC = IsAllowChar(password);
            if (IsSsidSBC)
            {
                MessageBox.Show(AppResources.DisallowedSSIDChar);
            }
            else if (passwordPanel.Visibility == Visibility.Visible && password.Length < 8 || password.Length > 64)
            {
                MessageBox.Show(AppResources.MsgPasswordFormat);
            }
            else if (password.Length == 64)
            {
                bool ret = false;           //ret为true，表示密码符合64位十六进制数字，反之则为false
                char[] ch = password.ToCharArray();
                for (int i = 0; i < ch.Length; i++)
                {
                    if ((ch[i] > 47 && ch[i] < 58) || (ch[i] > 64 && ch[i] < 71))
                        ret = true;
                }
                if (!ret)
                {
                    MessageBox.Show(AppResources.DisallowedPasswordChar);
                }
            }
            else if (IsPasswordSBC)
            {
                MessageBox.Show(AppResources.DisallowedPasswordChar);
            }
            else
            {
                PopupEnquire.IsOpen = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
                waittime.Visibility = Visibility.Collapsed;
            }            
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
                if (WifiSettingInfo.changedSecurityType.CompareTo("None") == 0)
                {
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += timer_Tick;
                    timer.Start();
                    await soapApi.SetWLANNoSecurity(ssid, WifiSettingInfo.region, WifiSettingInfo.changedChannel, WifiSettingInfo.wirelessMode);
                    WifiSettingInfo.ssid = WifiSettingInfo.changedSsid;
                    WifiSettingInfo.password = WifiSettingInfo.changedPassword;
                    WifiSettingInfo.channel = WifiSettingInfo.changedChannel;
                    WifiSettingInfo.securityType = WifiSettingInfo.changedSecurityType;
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
                    WifiSettingInfo.ssid = WifiSettingInfo.changedSsid;
                    WifiSettingInfo.password = WifiSettingInfo.changedPassword;
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

        int count = 60;     //倒计时间
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

        //通过ASCII码值判断输入字符串是否有不允许字符
        private static bool IsAllowChar(string input)
        {
            if (input == "" || input == null)
                return false;
            bool ret = false;
            char[] ch = input.ToCharArray();
            for (int i = 0; i < ch.Length; i++)
            {
                if (ch[i] < 33 || ch[i] > 126)
                    ret = true;
            }
            return ret;
        }

        private void SSID_GotFocus(object sender, RoutedEventArgs e)
        {
            SSID.Background = new SolidColorBrush(Colors.White);
        }

        private void pwd_GotFocus(object sender, RoutedEventArgs e)
        {
            pwd.Background = new SolidColorBrush(Colors.White);
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid gridItem = (Grid)sender;
            switch (gridItem.Name)
            {
                case "gridChannel":
                    gridChannel.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                case "gridSecurity":
                    gridSecurity.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                default:
                    break;
            }
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid gridItem = (Grid)sender;
            switch (gridItem.Name)
            {
                case "gridChannel":
                    gridChannel.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridSecurity":
                    gridSecurity.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                default:
                    break;
            }
        }

        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid gridItem = (Grid)sender;
            switch (gridItem.Name)
            {
                case "gridChannel":
                    gridChannel.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    NavigationService.Navigate(new Uri("/WifiEditChannelPage.xaml", UriKind.Relative));
                    break;
                case "gridSecurity":
                    gridSecurity.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    NavigationService.Navigate(new Uri("/WifiEditSecurityPage.xaml", UriKind.Relative));
                    break;
                default:
                    break;
            }
        }
    }
}