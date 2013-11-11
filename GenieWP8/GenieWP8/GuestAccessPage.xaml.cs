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
using ZXing;
using ZXing.Common;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Media;

namespace GenieWP8
{
    public partial class GuestAccessPage : PhoneApplicationPage
    {
        private static GuestAccessModel settingModel = null;
        public GuestAccessPage()
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
                settingModel = new GuestAccessModel();
            DataContext = settingModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();

            if (GuestAccessInfo.isGuestAccessEnabled == "0")
            {
                checkGuestSetting.IsChecked = false;
                GuestSettingPanel.Visibility = Visibility.Collapsed;
                textScanQRCode.Visibility = Visibility.Collapsed;
                imageQRCode.Visibility = Visibility.Collapsed;
            }
            else if (GuestAccessInfo.isGuestAccessEnabled == "1")
            {
                checkGuestSetting.IsChecked = true;
                GuestSettingPanel.Visibility = Visibility.Visible;
                textScanQRCode.Visibility = Visibility.Visible;
                imageQRCode.Visibility = Visibility.Visible;
            }
        }

        // 为 GuestAccessModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (!settingModel.IsDataLoaded)
            //{
            //    settingModel.LoadData();
            //}
            //settingModel.GuestSettingGroups.Clear();
            settingModel.EditTimesegSecurity.Clear();
            settingModel.LoadData();
            tbSsid.Text = GuestAccessInfo.ssid;
            tbKey.Text = GuestAccessInfo.password;
            tbTimeseg.Text = GuestAccessInfo.timePeriod;
            //生成二维码
            string codeString = "WIRELESS:" + GuestAccessInfo.ssid + ";PASSWORD:" + GuestAccessInfo.password;
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

        //// 处理在 LongListSelector 中更改的选定内容
        //private void GuestSettingsLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // 如果所选项为空(没有选定内容)，则不执行任何操作
        //    if (GuestSettingsLongListSelector.SelectedItem == null)
        //        return;

        //    // 导航到新页面
        //    GuestAccessInfo.isOpenGuestAccess = false;
        //    NavigationService.Navigate(new Uri("/GuestSettingPage.xaml", UriKind.Relative));

        //    // 将所选项重置为 null (没有选定内容)
        //    GuestSettingsLongListSelector.SelectedItem = null;
        //}

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
            appBarButton_refresh.Click += new EventHandler(appBarButton_refresh_Click);
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

        //checkbox控件响应事件
        private async void checkGuestSetting_Click(Object sender, RoutedEventArgs e)
        {
            //PopupEnquire.IsOpen = true;
            //PopupBackgroundTop.Visibility = Visibility.Visible;
            //PopupBackground.Visibility = Visibility.Visible;
            //InProgress.Visibility = Visibility.Collapsed;
            //pleasewait.Visibility = Visibility.Collapsed;
            GenieSoapApi soapApi = new GenieSoapApi();
            if (checkGuestSetting.IsChecked == true)
            {
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                while (dicResponse == null || dicResponse.Count == 0)
                {
                    dicResponse = await soapApi.GetGuestAccessNetworkInfo();
                }                
                if (dicResponse.Count > 0)
                {
                    GuestAccessInfo.ssid = dicResponse["NewSSID"];
                    GuestAccessInfo.changedSsid = dicResponse["NewSSID"];
                    GuestAccessInfo.securityType = dicResponse["NewSecurityMode"];
                    GuestAccessInfo.changedSecurityType = dicResponse["NewSecurityMode"];
                    if (dicResponse["NewSecurityMode"] != "None")
                    {
                        GuestAccessInfo.password = dicResponse["NewKey"];
                        GuestAccessInfo.changedPassword = dicResponse["NewKey"];
                    }
                    else
                    {
                        GuestAccessInfo.password = "";
                        GuestAccessInfo.changedPassword = "";
                    }
                }
                GuestAccessInfo.isOpenGuestAccess = true;
                NavigationService.Navigate(new Uri("/GuestSettingPage.xaml", UriKind.Relative));
            } 
            else if (checkGuestSetting.IsChecked == false)
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

            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.SetGuestAccessEnabled();
            }
        }

        int count = 60;     //倒计时间
        void timer_Tick(object sender, object e)
        {
            waittime.Text = count.ToString();
            count--;
            if (count < 0)
            {
                timer.Stop();
                GuestSettingPanel.Visibility = Visibility.Collapsed;
                textScanQRCode.Visibility = Visibility.Collapsed;
                imageQRCode.Visibility = Visibility.Collapsed;

                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                MessageBox.Show(AppResources.wirelesssettinrelogin);
                MainPageInfo.bLogin = false;
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }

        //“否”按钮响应事件
        private void NoButton_Click(Object sender, RoutedEventArgs e)
        {
            if (checkGuestSetting.IsChecked == false)
            {
                checkGuestSetting.IsChecked = true;
            }
            PopupEnquire.IsOpen = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        //刷新按钮响应事件
        private async void appBarButton_refresh_Click(object sender, EventArgs e)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.GetGuestAccessEnabled();
            }            
            if (dicResponse.Count > 0)
            {
                GuestAccessInfo.isGuestAccessEnabled = dicResponse["NewGuestAccessEnabled"];
                if (GuestAccessInfo.isGuestAccessEnabled == "0")
                {
                    checkGuestSetting.IsChecked = false;
                    GuestSettingPanel.Visibility = Visibility.Collapsed;
                    textScanQRCode.Visibility = Visibility.Collapsed;
                    imageQRCode.Visibility = Visibility.Collapsed;
                }
                else if (GuestAccessInfo.isGuestAccessEnabled == "1")
                {
                    checkGuestSetting.IsChecked = true;
                    GuestSettingPanel.Visibility = Visibility.Visible;
                    textScanQRCode.Visibility = Visibility.Visible;
                    imageQRCode.Visibility = Visibility.Visible;
                    Dictionary<string, string> dicResponse1 = new Dictionary<string, string>();
                    while (dicResponse1 == null || dicResponse1.Count == 0)
                    {
                        dicResponse1 = await soapApi.GetGuestAccessNetworkInfo();
                    }
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
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;                        
                        OnNavigatedTo(null);
                    }
                    else
                    {
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        MessageBox.Show("GetGuestAccessNetworkInfo failed!");
                    }
                }
                else if (GuestAccessInfo.isGuestAccessEnabled == "2")
                {
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    MessageBox.Show(AppResources.notsupport);
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }               
            }
            else
            {
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                MessageBox.Show("GetGuestAccessEnabled failed!");
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

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid gridItem = (Grid)sender;
            switch (gridItem.Name)
            {
                case "gridSsid":
                    gridSsid.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                case "gridKey":
                    gridKey.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                case "gridTimeseg":
                    gridTimeseg.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
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
                case "gridSsid":
                    gridSsid.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridKey":
                    gridKey.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridTimeseg":
                    gridTimeseg.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
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
                case "gridSsid":
                    gridSsid.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridKey":
                    gridKey.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridTimeseg":
                    gridTimeseg.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    GuestAccessInfo.changedTimePeriod = GuestAccessInfo.timePeriod;
                    break;
                default:
                    break;
            }
            GuestAccessInfo.isOpenGuestAccess = false;
            NavigationService.Navigate(new Uri("/GuestSettingPage.xaml", UriKind.Relative));
        }
    }
}