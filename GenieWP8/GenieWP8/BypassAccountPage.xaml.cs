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
using Microsoft.Phone.Net.NetworkInformation;

namespace GenieWP8
{
    public partial class BypassAccountPage : PhoneApplicationPage
    {
        private static ParentalControlModel settingModel = null;
        private static bool IsWifiSsidChanged;
        public BypassAccountPage()
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

            // 将 ListBox 控件的数据上下文设置为绑定数据
            if (settingModel == null)
                settingModel = new ParentalControlModel();
            DataContext = settingModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            settingModel.BypassAccountGroups.Clear();
            settingModel.LoadData();
            //bypassAccountListBox.Items.Clear();
            //if (ParentalControlInfo.BypassAccounts != null)
            //{
            //    string[] bypassAccount = ParentalControlInfo.BypassAccounts.Split(';');
            //    for (int i = 0; i < bypassAccount.Length; i++)
            //    {
            //        if (bypassAccount[i] != null && bypassAccount[i] != "")
            //        {
            //            bypassAccountListBox.Items.Add(bypassAccount[i]);
            //        }
            //    }
            //}

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

        // 处理在 ListBox 中更改的选定内容
        private void BypassAccountItemClick(object sender, SelectionChangedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                MainPageInfo.navigatedPage = "ParentalControlPage";
            } 
            else
            {
                if (bypassAccountListBox.SelectedIndex == -1)
                {
                    return;
                }
                int index = bypassAccountListBox.SelectedIndex;
                string[] bypassAccount = ParentalControlInfo.BypassAccounts.Split(';');
                ParentalControlInfo.BypassUsername = bypassAccount[index];
                NavigationService.Navigate(new Uri("/BypassAccountLoginPage.xaml", UriKind.Relative));
                bypassAccountListBox.SelectedIndex = -1;  
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

        //返回按钮响应事件
        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}