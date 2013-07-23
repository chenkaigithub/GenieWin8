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

namespace GenieWP8
{
    public partial class WifiEditSettingPage : PhoneApplicationPage
    {
        private static WifiSettingModel settingModel = null;
        public WifiEditSettingPage()
        {
            InitializeComponent();

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
            if (!settingModel.IsDataLoaded)
            {
                settingModel.LoadData();
            }
            //settingModel.LoadData();
        }

        //判断SSID是否更改以及保存按钮是否可点击
        private void ssid_changed(Object sender, RoutedEventArgs e)
        {
            //string ssid = SSID.Text.Trim();
            //if (ssid != WifiInfoModel.ssid && ssid != "")
            //{
            //    wifiSettingSave.IsEnabled = true;
            //    WifiInfoModel.isSSIDChanged = true;
            //}
            //else
            //{
            //    WifiInfoModel.isSSIDChanged = false;
            //    if (WifiInfoModel.isPasswordChanged == true || WifiInfoModel.isChannelChanged == true || WifiInfoModel.isSecurityTypeChanged == true)
            //    {
            //        wifiSettingSave.IsEnabled = true;
            //    }
            //    else
            //    {
            //        wifiSettingSave.IsEnabled = false;
            //    }
            //}
        }

        //判断密码是否更改以及保存按钮是否可点击
        private void pwd_changed(Object sender, RoutedEventArgs e)
        {
            //string password = pwd.Text.Trim();
            //if (password != WifiInfoModel.password && password != "")
            //{
            //    wifiSettingSave.IsEnabled = true;
            //    WifiInfoModel.isPasswordChanged = true;
            //}
            //else
            //{
            //    WifiInfoModel.isPasswordChanged = false;
            //    if (WifiInfoModel.isSSIDChanged == true || WifiInfoModel.isChannelChanged == true || WifiInfoModel.isSecurityTypeChanged == true)
            //    {
            //        wifiSettingSave.IsEnabled = true;
            //    }
            //    else
            //    {
            //        wifiSettingSave.IsEnabled = false;
            //    }
            //}
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

            //刷新按钮
            ApplicationBarIconButton appBarButton_save = new ApplicationBarIconButton(new Uri("Assets/save.png", UriKind.Relative));
            appBarButton_save.Text = AppResources.RefreshButtonContent;
            ApplicationBar.Buttons.Add(appBarButton_save);
            appBarButton_save.Click += new EventHandler(appBarButton_save_Click);
        }

        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/WifiSettingPage.xaml", UriKind.Relative));
        }

        //重写手机“返回”按钮事件
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/WifiSettingPage.xaml", UriKind.Relative));
        }

        private void appBarButton_save_Click(object sender, EventArgs e)
        {
            
        }
    }
}