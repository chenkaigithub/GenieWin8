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
using ZXing;
using ZXing.Common;
using System.Windows.Media.Imaging;

namespace GenieWP8
{
    public partial class WifiSettingPage : PhoneApplicationPage
    {
        private static WifiSettingModel settingModel = null;
        public WifiSettingPage()
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

            // 将 LongListSelector 控件的数据上下文设置为绑定数据
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

            //生成二维码
            string codeString = "WIRELESS:" + WifiSettingInfo.ssid + ";PASSWORD:" + WifiSettingInfo.password;
            WriteableBitmap wb = CreateBarcode(codeString);
            imageQRCode.Source = wb;
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

        // 处理在 LongListSelector 中更改的选定内容
        private void WifiSettingLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果所选项为空(没有选定内容)，则不执行任何操作
            if (WifiSettingLongListSelector.SelectedItem == null)
                return;

            // 导航到新页面
            var groupId = ((SettingGroup)WifiSettingLongListSelector.SelectedItem).ID;
            if (groupId != "SignalStrength" && groupId != "LinkRate")
            {
                NavigationService.Navigate(new Uri("/WifiEditSettingPage.xaml", UriKind.Relative));
            }            

            // 将所选项重置为 null (没有选定内容)
            WifiSettingLongListSelector.SelectedItem = null;
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
            ApplicationBarIconButton appBarButton_refresh = new ApplicationBarIconButton(new Uri("Assets/refresh.png", UriKind.Relative));
            appBarButton_refresh.Text = AppResources.RefreshButtonContent;
            ApplicationBar.Buttons.Add(appBarButton_refresh);
            appBarButton_refresh.Click+=new EventHandler(appBarButton_refresh_Click);
        }

        //返回按钮响应事件
        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //重写手机“返回”按钮事件
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //刷新按钮响应事件
        private async void appBarButton_refresh_Click(object sender, EventArgs e)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();

            Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
            attachDeviceAll = await soapApi.GetAttachDevice();
            UtilityTool util = new UtilityTool();
            var ipList = util.GetCurrentIpAddresses();
            string loacalIp = ipList.ToList()[0];
            foreach (string key in attachDeviceAll.Keys)
            {
                if (loacalIp == key)
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
                WifiSettingInfo.channel = dicResponse["NewChannel"];
                WifiSettingInfo.changedChannel = dicResponse["NewChannel"];
                WifiSettingInfo.securityType = dicResponse["NewWPAEncryptionModes"];
                WifiSettingInfo.changedSecurityType = dicResponse["NewWPAEncryptionModes"];
                Dictionary<string, string> dicResponse1 = new Dictionary<string, string>();
                dicResponse1 = await soapApi.GetWPASecurityKeys();
                if (dicResponse1.Count > 0)
                {
                    WifiSettingInfo.password = dicResponse1["NewWPAPassphrase"];
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    OnNavigatedTo(null);
                }
                else
                {
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    MessageBox.Show("GetWPASecurityKeys failed!");
                }
            }
            else
            {
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                MessageBox.Show("Get WLANConfiguration failed!");
            }           
        }

        //生成二维码
        public static WriteableBitmap CreateBarcode(string content)
        {
            IBarcodeWriter wt = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 200,
                    Width = 200
                }
            };
            var bmp = wt.Write(content);
            return bmp;
        }
    }
}