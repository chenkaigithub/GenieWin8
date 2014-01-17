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
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Text;
using GenieWin8.DataModel;
using Windows.Storage;
using Windows.UI.Popups;
using System.Threading.Tasks;
using Windows.Networking.Connectivity;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class DeviceInfoPage : GenieWin8.Common.LayoutAwarePage
    {
        private string routerImage;
        private static bool IsWifiSsidChanged;
        public DeviceInfoPage()
        {
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
            ImageNameGenerator imagePath = new ImageNameGenerator(MainPageInfo.model);
            ///获取路由器图标路径
            routerImage = imagePath.getImagePath(); 
        }

        private void App_Resuming(Object sender, Object e)
        {
            //判断所连接Wifi的Ssid是否改变
            IsWifiSsidChanged = true;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        if (connectionProfile.ProfileName == MainPageInfo.ssid)
                            IsWifiSsidChanged = false;
                        else
                            IsWifiSsidChanged = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
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
            //判断所连接Wifi的Ssid是否改变
            IsWifiSsidChanged = true;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        if (connectionProfile.ProfileName == MainPageInfo.ssid)
                            IsWifiSsidChanged = false;
                        else
                            IsWifiSsidChanged = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            var Group = DeviceSource.GetGroup((String)(navigationParameter));

            StpTitle.Children.Clear();
            //StpDeviceInfo.Children.Clear();

            //Image TitleImage = new Image();
            TitleImage.HorizontalAlignment = HorizontalAlignment.Left;
            TitleImage.VerticalAlignment = VerticalAlignment.Center;
            TitleImage.Margin = new Thickness(10, 0, 0, 0);
            TitleImage.Width = 100; TitleImage.Height = 100;

            //TextBlock Title = new TextBlock();
            Title.Text = Group.NODE.deviceName;
            //Title.Text = WifiInfoModel.ssid;
            Title.FontSize = 40;
            Title.VerticalAlignment = VerticalAlignment.Center;
            Title.Margin = new Thickness(10, 0, 0, 0);
            Title.FontWeight = FontWeights.Bold;
            Title.Width = Window.Current.Bounds.Width - 200;
            Title.TextWrapping = TextWrapping.Wrap;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            //DeviceName
            var strtext = loader.GetString("txtDeviceName");
            txtDeviceName.Text = strtext;
            txtDeviceName.FontSize = 30;
            txtDeviceName.Margin = new Thickness(10, 10, 0, 0);
            txtDeviceName.HorizontalAlignment = HorizontalAlignment.Left;
            txtBlockDeviceName.Text = Group.NODE.deviceName;
            txtBlockDeviceName.FontSize = 25;
            txtBlockDeviceName.Width = 660;
            txtBlockDeviceName.TextWrapping = TextWrapping.Wrap;
            txtBlockDeviceName.TextAlignment = TextAlignment.Center;
            txtBlockDeviceName.HorizontalAlignment = HorizontalAlignment.Center;
            txtBoxDeviceName.Text = Group.NODE.deviceName;
            txtBoxDeviceName.FontSize = 25;
            txtBoxDeviceName.HorizontalAlignment = HorizontalAlignment.Center;
            txtBoxDeviceName.TextAlignment = TextAlignment.Center;
            txtBoxDeviceName.Width = 400;
            txtBoxDeviceName.Visibility = Visibility.Collapsed;

            //Type
            if (Group.NODE.deviceType != null)
            {
                strtext = loader.GetString("txtType");
                txtType.Text = strtext;
                txtType.FontSize = 30;
                txtType.Margin = new Thickness(10, 10, 0, 0);
                txtType.HorizontalAlignment = HorizontalAlignment.Left;
                switch (Group.NODE.deviceType)
                {
                    case "imacdev":
                        Type.Text = loader.GetString("imacdev");
                        break;
                    case "ipad":
                        Type.Text = loader.GetString("ipad");
                        break;
                    case "ipadmini":
                        Type.Text = loader.GetString("ipadmini");
                        break;
                    case "iphone":
                        Type.Text = loader.GetString("iphone");
                        break;
                    case "iphone5":
                        Type.Text = loader.GetString("iphone5");
                        break;
                    case "ipodtouch":
                        Type.Text = loader.GetString("ipodtouch");
                        break;
                    case "amazonkindle":
                        Type.Text = loader.GetString("amazonkindle");
                        break;
                    case "androiddevice":
                        Type.Text = loader.GetString("androiddevice");
                        break;
                    case "androidphone":
                        Type.Text = loader.GetString("androidphone");
                        break;
                    case "androidtablet":
                        Type.Text = loader.GetString("androidtablet");
                        break;
                    case "blurayplayer":
                        Type.Text = loader.GetString("blurayplayer");
                        break;
                    case "bridge":
                        Type.Text = loader.GetString("bridge");
                        break;
                    case "cablestb":
                        Type.Text = loader.GetString("cablestb");
                        break;
                    case "cameradev":
                        Type.Text = loader.GetString("cameradev");
                        break;
                    case "dvr":
                        Type.Text = loader.GetString("dvr");
                        break;
                    case "gamedev":
                        Type.Text = loader.GetString("gamedev");
                        break;
                    case "linuxpc":
                        Type.Text = loader.GetString("linuxpc");
                        break;
                    case "macminidev":
                        Type.Text = loader.GetString("macminidev");
                        break;
                    case "macprodev":
                        Type.Text = loader.GetString("macprodev");
                        break;
                    case "macbookdev":
                        Type.Text = loader.GetString("macbookdev");
                        break;
                    case "mediadev":
                        Type.Text = loader.GetString("mediadev");
                        break;
                    case "networkdev":
                        Type.Text = loader.GetString("networkdev");
                        break;
                    case "stb":
                        Type.Text = loader.GetString("stb");
                        break;
                    case "printerdev":
                        Type.Text = loader.GetString("printerdev");
                        break;
                    case "repeater":
                        Type.Text = loader.GetString("repeater");
                        break;
                    case "gatewaydev":
                        Type.Text = loader.GetString("gatewaydev");
                        break;
                    case "satellitestb":
                        Type.Text = loader.GetString("satellitestb");
                        break;
                    case "scannerdev":
                        Type.Text = loader.GetString("scannerdev");
                        break;
                    case "slingbox":
                        Type.Text = loader.GetString("slingbox");
                        break;
                    case "mobiledev":
                        Type.Text = loader.GetString("mobiledev");
                        break;
                    case "netstoragedev":
                        Type.Text = loader.GetString("netstoragedev");
                        break;
                    case "switchdev":
                        Type.Text = loader.GetString("switchdev");
                        break;
                    case "tv":
                        Type.Text = loader.GetString("tv");
                        break;
                    case "tablepc":
                        Type.Text = loader.GetString("tablepc");
                        break;
                    case "unixpc":
                        Type.Text = loader.GetString("unixpc");
                        break;
                    case "windowspc":
                        Type.Text = loader.GetString("windowspc");
                        break;
                    case "windowsphone":
                        Type.Text = loader.GetString("windowsphone");
                        break;
                    case "windowstablet":
                        Type.Text = loader.GetString("windowstablet");
                        break; 
                }
                Type.FontSize = 25;
                Type.HorizontalAlignment = HorizontalAlignment.Center;

                for (int i = 0; i < 38; i++ )
                {
                    Image imgDevice = new Image();
                    Uri _baseUri = new Uri("ms-appx:///");
                    imgDevice.Stretch = Stretch.UniformToFill;
                    imgDevice.Width = 25;
                    imgDevice.Height = 25;
                    StackPanel stpDevice = new StackPanel();
                    stpDevice.Orientation = Orientation.Horizontal;
                    TextBlock texttype = new TextBlock();
                    texttype.Margin = new Thickness(5, 0, 0, 0);
                    switch (i)
                    {
                        case 0:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/imacdev.png"));
                            strtext = loader.GetString("imacdev");
                            texttype.Text = strtext;
                            break;
                        case 1:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipad.png"));
                            strtext = loader.GetString("ipad");
                            texttype.Text = strtext;
                            break;
                        case 2:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipadmini.png"));
                            strtext = loader.GetString("ipadmini");
                            texttype.Text = strtext;
                            break;
                        case 3:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone.png"));
                            strtext = loader.GetString("iphone");
                            texttype.Text = strtext;
                            break;
                        case 4:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone5.png"));
                            strtext = loader.GetString("iphone5");
                            texttype.Text = strtext;
                            break;
                        case 5:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipodtouch.png"));
                            strtext = loader.GetString("ipodtouch");
                            texttype.Text = strtext;
                            break;
                        case 6:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/amazonkindle.png"));
                            strtext = loader.GetString("amazonkindle");
                            texttype.Text = strtext;
                            break;
                        case 7:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androiddevice.png"));
                            strtext = loader.GetString("androiddevice");
                            texttype.Text = strtext;
                            break;
                        case 8:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                            strtext = loader.GetString("androidphone");
                            texttype.Text = strtext;
                            break;
                        case 9:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidtablet.png"));
                            strtext = loader.GetString("androidtablet");
                            texttype.Text = strtext;
                            break;
                        case 10:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/blurayplayer.png"));
                            strtext = loader.GetString("blurayplayer");
                            texttype.Text = strtext;
                            break;
                        case 11:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/bridge.png"));
                            strtext = loader.GetString("bridge");
                            texttype.Text = strtext;
                            break;
                        case 12:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cablestb.png"));
                            strtext = loader.GetString("cablestb");
                            texttype.Text = strtext;
                            break;
                        case 13:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cameradev.png"));
                            strtext = loader.GetString("cameradev");
                            texttype.Text = strtext;
                            break;
                        case 14:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/dvr.png"));
                            strtext = loader.GetString("dvr");
                            texttype.Text = strtext;
                            break;
                        case 15:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gamedev.png"));
                            strtext = loader.GetString("gamedev");
                            texttype.Text = strtext;
                            break;
                        case 16:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/linuxpc.png"));
                            strtext = loader.GetString("linuxpc");
                            texttype.Text = strtext;
                            break;
                        case 17:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macminidev.png"));
                            strtext = loader.GetString("macminidev");
                            texttype.Text = strtext;
                            break;
                        case 18:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macprodev.png"));
                            strtext = loader.GetString("macprodev");
                            texttype.Text = strtext;
                            break;
                        case 19:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macbookdev.png"));
                            strtext = loader.GetString("macbookdev");
                            texttype.Text = strtext;
                            break;
                        case 20:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mediadev.png"));
                            strtext = loader.GetString("mediadev");
                            texttype.Text = strtext;
                            break;
                        case 21:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/networkdev.png"));
                            strtext = loader.GetString("networkdev");
                            texttype.Text = strtext;
                            break;
                        case 22:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/stb.png"));
                            strtext = loader.GetString("stb");
                            texttype.Text = strtext;
                            break;
                        case 23:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/printerdev.png"));
                            strtext = loader.GetString("printerdev");
                            texttype.Text = strtext;
                            break;
                        case 24:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/repeater.png"));
                            strtext = loader.GetString("repeater");
                            texttype.Text = strtext;
                            break;
                        case 25:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                            strtext = loader.GetString("gatewaydev");
                            texttype.Text = strtext;
                            break;
                        case 26:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/satellitestb.png"));
                            strtext = loader.GetString("satellitestb");
                            texttype.Text = strtext;
                            break;
                        case 27:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/scannerdev.png"));
                            strtext = loader.GetString("scannerdev");
                            texttype.Text = strtext;
                            break;
                        case 28:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/slingbox.png"));
                            strtext = loader.GetString("slingbox");
                            texttype.Text = strtext;
                            break;
                        case 29:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mobiledev.png"));
                            strtext = loader.GetString("mobiledev");
                            texttype.Text = strtext;
                            break;
                        case 30:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/netstoragedev.png"));
                            strtext = loader.GetString("netstoragedev");
                            texttype.Text = strtext;
                            break;
                        case 31:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/switchdev.png"));
                            strtext = loader.GetString("switchdev");
                            texttype.Text = strtext;
                            break;
                        case 32:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tv.png"));
                            strtext = loader.GetString("tv");
                            texttype.Text = strtext;
                            break;
                        case 33:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tablepc.png"));
                            strtext = loader.GetString("tablepc");
                            texttype.Text = strtext;
                            break;
                        case 34:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/unixpc.png"));
                            strtext = loader.GetString("unixpc");
                            texttype.Text = strtext;
                            break;
                        case 35:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowspc.png"));
                            strtext = loader.GetString("windowspc");
                            texttype.Text = strtext;
                            break;
                        case 36:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowsphone.png"));
                            strtext = loader.GetString("windowsphone");
                            texttype.Text = strtext;
                            break;
                        case 37:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowstablet.png"));
                            strtext = loader.GetString("windowstablet");
                            texttype.Text = strtext;
                            break;
                    }
                    stpDevice.Children.Add(imgDevice);
                    stpDevice.Children.Add(texttype);
                    ComboType.Items.Add(stpDevice);
                }
                
                switch (Group.NODE.deviceType)
                {
                    case "imacdev":
                        ComboType.SelectedIndex = 0;
                        break;
                    case "ipad":
                        ComboType.SelectedIndex = 1;
                        break;
                    case "ipadmini":
                        ComboType.SelectedIndex = 2;
                        break;
                    case "iphone":
                        ComboType.SelectedIndex = 3;
                        break;
                    case "iphone5":
                        ComboType.SelectedIndex = 4;
                        break;
                    case "ipodtouch":
                        ComboType.SelectedIndex = 5;
                        break;
                    case "amazonkindle":
                        ComboType.SelectedIndex = 6;
                        break;
                    case "androiddevice":
                        ComboType.SelectedIndex = 7;
                        break;
                    case "androidphone":
                        ComboType.SelectedIndex = 8;
                        break;
                    case "androidtablet":
                        ComboType.SelectedIndex = 9;
                        break;
                    case "blurayplayer":
                        ComboType.SelectedIndex = 10;
                        break;
                    case "bridge":
                        ComboType.SelectedIndex = 11;
                        break;
                    case "cablestb":
                        ComboType.SelectedIndex = 12;
                        break;
                    case "cameradev":
                        ComboType.SelectedIndex = 13;
                        break;
                    case "dvr":
                        ComboType.SelectedIndex = 14;
                        break;
                    case "gamedev":
                        ComboType.SelectedIndex = 15;
                        break;
                    case "linuxpc":
                        ComboType.SelectedIndex = 16;
                        break;
                    case "macminidev":
                        ComboType.SelectedIndex = 17;
                        break;
                    case "macprodev":
                        ComboType.SelectedIndex = 18;
                        break;
                    case "macbookdev":
                        ComboType.SelectedIndex = 19;
                        break;
                    case "mediadev":
                        ComboType.SelectedIndex = 20;
                        break;
                    case "networkdev":
                        ComboType.SelectedIndex = 21;
                        break;
                    case "stb":
                        ComboType.SelectedIndex = 22;
                        break;
                    case "printerdev":
                        ComboType.SelectedIndex = 23;
                        break;
                    case "repeater":
                        ComboType.SelectedIndex = 24;
                        break;
                    case "gatewaydev":
                        ComboType.SelectedIndex = 25;
                        break;
                    case "satellitestb":
                        ComboType.SelectedIndex = 26;
                        break;
                    case "scannerdev":
                        ComboType.SelectedIndex = 27;
                        break;
                    case "slingbox":
                        ComboType.SelectedIndex = 28;
                        break;
                    case "mobiledev":
                        ComboType.SelectedIndex = 29;
                        break;
                    case "netstoragedev":
                        ComboType.SelectedIndex = 30;
                        break;
                    case "switchdev":
                        ComboType.SelectedIndex = 31;
                        break;
                    case "tv":
                        ComboType.SelectedIndex = 32;
                        break;
                    case "tablepc":
                        ComboType.SelectedIndex = 33;
                        break;
                    case "unixpc":
                        ComboType.SelectedIndex = 34;
                        break;
                    case "windowspc":
                        ComboType.SelectedIndex = 35;
                        break;
                    case "windowsphone":
                        ComboType.SelectedIndex = 36;
                        break;
                    case "windowstablet":
                        ComboType.SelectedIndex = 37;
                        break;
                }
                ComboType.Width = 400;
                ComboType.HorizontalAlignment = HorizontalAlignment.Center;
                ComboType.HorizontalContentAlignment = HorizontalAlignment.Center;
                ComboType.Visibility = Visibility.Collapsed;
            }

            //IPAddress
            strtext = loader.GetString("txtIPAddress");
            txtIPAddress.Text = strtext;
            txtIPAddress.FontSize = 30;
            txtIPAddress.Margin = new Thickness(10, 10, 0, 0);
            txtIPAddress.HorizontalAlignment = HorizontalAlignment.Left;
            IPAddress.Text = Group.NODE.IPaddress;
            IPAddress.FontSize = 25;
            IPAddress.HorizontalAlignment = HorizontalAlignment.Center;

            //SignalStrength
            if (Group.NODE.signalStrength != "%" && Group.NODE.signalStrength != null)
            {
                strtext = loader.GetString("txtSignalStrength");
                txtSignalStrength.Text = strtext;
                txtSignalStrength.FontSize = 30;
                txtSignalStrength.Margin = new Thickness(10, 10, 0, 0);
                txtSignalStrength.HorizontalAlignment = HorizontalAlignment.Left;
                SignalStrength.Text = Group.NODE.signalStrength;
                SignalStrength.FontSize = 25;
                SignalStrength.HorizontalAlignment = HorizontalAlignment.Center;
            }

            //LinkRate
            if (Group.NODE.linkRate != "Mbps" && Group.NODE.linkRate != null)
            {
                strtext = loader.GetString("txtLinkRate");
                txtLinkRate.Text = strtext;
                txtLinkRate.FontSize = 30;
                txtLinkRate.Margin = new Thickness(10, 10, 0, 0);
                txtLinkRate.HorizontalAlignment = HorizontalAlignment.Left;
                LinkRate.Text = Group.NODE.linkRate;
                LinkRate.FontSize = 25;
                LinkRate.HorizontalAlignment = HorizontalAlignment.Center;
                strtext = loader.GetString("btnWhat");
                btnWhat.Content = strtext;
                btnWhat.Tag = "http://support.netgear.com/app/answers/list/kw/link%20rate";
                btnWhat.FontSize = 20;
                btnWhat.Margin = new Thickness(10, 0, 0, 0);
                btnWhat.Padding = new Thickness(0, 0, 0, 0);
                btnWhat.HorizontalAlignment = HorizontalAlignment.Right;
            }

            //MACAddress
            strtext = loader.GetString("txtMACAddress");
            txtMACAddress.Text = strtext;
            txtMACAddress.FontSize = 30;
            txtMACAddress.Margin = new Thickness(10, 0, 0, 0);
            txtMACAddress.HorizontalAlignment = HorizontalAlignment.Left;
            MACAddress.Text = Group.NODE.MACaddress;
            MACAddress.FontSize = 25;
            MACAddress.HorizontalAlignment = HorizontalAlignment.Center;
            NetworkMapInfo.deviceMacaddr = Group.NODE.MACaddress;

            //Buttons
            strtext = loader.GetString("btnBack");
            btnBack.Content = strtext;
            btnBack.FontSize = 25;
            btnBack.HorizontalAlignment = HorizontalAlignment.Center;
            btnBack.Click += new RoutedEventHandler(BackButton_Click);
            strtext = loader.GetString("btnModify");
            btnModify.Content = strtext;
            btnModify.FontSize = 25;
            btnModify.HorizontalAlignment = HorizontalAlignment.Center;
            btnModify.Click += new RoutedEventHandler(ModifyButton_Click);
            strtext = loader.GetString("btnApply");
            btnApply.Content = strtext;
            btnApply.FontSize = 25;
            btnApply.HorizontalAlignment = HorizontalAlignment.Center;
            btnApply.Click += new RoutedEventHandler(ApplyButton_Click);
            strtext = loader.GetString("btnFileUpload");
            btnFileUpload.Content = strtext;
            btnFileUpload.FontSize = 25;
            btnFileUpload.HorizontalAlignment = HorizontalAlignment.Center;
            strtext = loader.GetString("btnAllow");
            btnAllow.Content = strtext;
            btnAllow.FontSize = 25;
            btnAllow.HorizontalAlignment = HorizontalAlignment.Center;
            btnAllow.Click += new RoutedEventHandler(AllowButton_Click);
            strtext = loader.GetString("btnBlock");
            btnBlock.Content = strtext;
            btnBlock.FontSize = 25;
            btnBlock.HorizontalAlignment = HorizontalAlignment.Center;
            btnBlock.Click += new RoutedEventHandler(BlockButton_Click);

            if (Group.NODE.uniqueId == "Router")
            {
                Uri _baseUri = new Uri("ms-appx:///");
                TitleImage.Source = new BitmapImage(new Uri(_baseUri, routerImage));
                StpTitle.Children.Add(TitleImage);
                StpTitle.Children.Add(Title);

                strtext = loader.GetString("txtRoutename");
                txtRoutename.Text = strtext;
                txtRoutename.FontSize = 30;
                txtRoutename.Margin = new Thickness(10, 10, 0, 0);
                txtRoutename.HorizontalAlignment = HorizontalAlignment.Left;
                RouteName.Text = Group.NODE.deviceName;
                RouteName.FontSize = 25;
                RouteName.Width = 660;
                RouteName.TextWrapping = TextWrapping.Wrap;
                RouteName.TextAlignment = TextAlignment.Center;
                RouteName.HorizontalAlignment = HorizontalAlignment.Center;

                //Firmware Version
                txtRouterFirmware.Text = loader.GetString("txtRouterFirmware");
                txtRouterFirmware.FontSize = 30;
                txtRouterFirmware.Margin = new Thickness(10, 10, 0, 0);
                txtRouterFirmware.HorizontalAlignment = HorizontalAlignment.Left;
                Firmware.Text = Group.NODE.RouterFirmware;
                Firmware.FontSize = 25;
                Firmware.HorizontalAlignment = HorizontalAlignment.Center;

                btnBack.Width = 250;
                btnBack.Margin = new Thickness(0, 10, 0, 0);

                StpRouter.Visibility = Visibility.Visible;
                StpDeviceName.Visibility = Visibility.Collapsed;
                StpRouterFirmware.Visibility = Visibility.Visible;
                StpType.Visibility = Visibility.Collapsed;
                StpLinkRate.Visibility = Visibility.Collapsed;
                StpSignalStrength.Visibility = Visibility.Collapsed;
                btnBack.Visibility = Visibility.Visible;
                btnFileUpload.Visibility = Visibility.Collapsed;
                btnModify.Visibility = Visibility.Collapsed;
                btnApply.Visibility = Visibility.Collapsed;
                btnAllow.Visibility = Visibility.Collapsed;
                btnBlock.Visibility = Visibility.Collapsed;
            }
            else if (Group.NODE.uniqueId == "LocalDevice")
            {
                Uri _baseUri = new Uri("ms-appx:///");
                TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowstablet.png"));
                StpTitle.Children.Add(TitleImage);
                StpTitle.Children.Add(Title);

                btnBack.Width = 250;
                btnBack.Margin = new Thickness(0, 10, 0, 0);

                StpRouter.Visibility = Visibility.Collapsed;
                StpDeviceName.Visibility = Visibility.Visible;
                StpRouterFirmware.Visibility = Visibility.Collapsed;
                StpType.Visibility = Visibility.Collapsed;
                if (NetworkMapInfo.attachDeviceDic.Count == 0)
                {
                    StpMACAddress.Visibility = Visibility.Collapsed;
                    StpLinkRate.Visibility = Visibility.Collapsed;
                    StpSignalStrength.Visibility = Visibility.Collapsed;
                } 
                else
                {
                    if (Group.NODE.linkRate == "Mbps" || Group.NODE.linkRate == null)
                        StpLinkRate.Visibility = Visibility.Collapsed;
                    else
                        StpLinkRate.Visibility = Visibility.Visible;
                    if (Group.NODE.signalStrength == "%" || Group.NODE.signalStrength == null || Group.NODE.connectType != "wireless")
                        StpSignalStrength.Visibility = Visibility.Collapsed;
                    else
                        StpSignalStrength.Visibility = Visibility.Visible;
                }

                btnBack.Visibility = Visibility.Visible;
                btnFileUpload.Visibility = Visibility.Collapsed;
                btnModify.Visibility = Visibility.Collapsed;
                btnApply.Visibility = Visibility.Collapsed;
                btnAllow.Visibility = Visibility.Collapsed;
                btnBlock.Visibility = Visibility.Collapsed;
            }
            else
            {
                Uri _baseUri = new Uri("ms-appx:///");
                switch (Group.NODE.deviceType)
                {
                    case "imacdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/imacdev.png"));
                        break;
                    case "ipad":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipad.png"));
                        break;
                    case "ipadmini":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipadmini.png"));
                        break;
                    case "iphone":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone.png"));
                        break;
                    case "iphone5":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone5.png"));
                        break;
                    case "ipodtouch":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipodtouch.png"));
                        break;
                    case "amazonkindle":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/amazonkindle.png"));
                        break;
                    case "androiddevice":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androiddevice.png"));
                        break;
                    case "androidphone":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                        break;
                    case "androidtablet":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidtablet.png"));
                        break;
                    case "blurayplayer":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/blurayplayer.png"));
                        break;
                    case "bridge":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/bridge.png"));
                        break;
                    case "cablestb":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cablestb.png"));
                        break;
                    case "cameradev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cameradev.png"));
                        break;
                    case "dvr":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/dvr.png"));
                        break;
                    case "gamedev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gamedev.png"));
                        break;
                    case "linuxpc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/linuxpc.png"));
                        break;
                    case "macminidev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macminidev.png"));
                        break;
                    case "macprodev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macprodev.png"));
                        break;
                    case "macbookdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macbookdev.png"));
                        break;
                    case "mediadev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mediadev.png"));
                        break;
                    case "networkdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/networkdev.png"));
                        break;
                    case "stb":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/stb.png"));
                        break;
                    case "printerdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/printerdev.png"));
                        break;
                    case "repeater":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/repeater.png"));
                        break;
                    case "gatewaydev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                        break;
                    case "satellitestb":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/satellitestb.png"));
                        break;
                    case "scannerdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/scannerdev.png"));
                        break;
                    case "slingbox":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/slingbox.png"));
                        break;
                    case "mobiledev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mobiledev.png"));
                        break;
                    case "netstoragedev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/netstoragedev.png"));
                        break;
                    case "switchdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/switchdev.png"));
                        break;
                    case "tv":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tv.png"));
                        break;
                    case "tablepc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tablepc.png"));
                        break;
                    case "unixpc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/unixpc.png"));
                        break;
                    case "windowspc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowspc.png"));
                        break;
                    case "windowsphone":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowsphone.png"));
                        break;
                    case "windowstablet":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowstablet.png"));
                        break;
                }
                StpTitle.Children.Add(TitleImage);
                StpTitle.Children.Add(Title);

                StpRouter.Visibility = Visibility.Collapsed;
                StpDeviceName.Visibility = Visibility.Visible;
                StpRouterFirmware.Visibility = Visibility.Collapsed;
                if (Group.NODE.deviceType == null)
                    StpType.Visibility = Visibility.Collapsed;
                else
                    StpType.Visibility = Visibility.Visible;

                if (Group.NODE.signalStrength == "%" || Group.NODE.signalStrength == null || Group.NODE.connectType != "wireless")
                    StpSignalStrength.Visibility = Visibility.Collapsed;
                else
                    StpSignalStrength.Visibility = Visibility.Visible;

                if (Group.NODE.linkRate == "Mbps" || Group.NODE.linkRate == null)
                    StpLinkRate.Visibility = Visibility.Collapsed;
                else
                    StpLinkRate.Visibility = Visibility.Visible;

                if (Group.NODE.AccessControl == "" || NetworkMapInfo.IsAccessControlSupported == false || NetworkMapInfo.IsAccessControlEnabled == false)
                {
                    btnBack.Width = 200;
                    btnBack.Margin = new Thickness(200, 10, 0, 0);
                    btnModify.Width = 200;
                    btnModify.Margin = new Thickness(0, 10, 200, 0);
                    btnApply.Width = 200;
                    btnApply.Margin = new Thickness(0, 10, 200, 0);
                    btnAllow.Visibility = Visibility.Collapsed;
                    btnBlock.Visibility = Visibility.Collapsed;
                }
                else
                {
                    btnBack.Width = 150;
                    btnBack.Margin = new Thickness(300, 10, 0, 0);
                    btnModify.Width = 150;
                    btnModify.Margin = new Thickness(0, 10, 300, 0);
                    btnApply.Width = 150;
                    btnApply.Margin = new Thickness(0, 10, 300, 0);
                    btnAllow.Width = 150;
                    btnAllow.Margin = new Thickness(0, 10, 0, 0);
                    btnBlock.Width = 150;
                    btnBlock.Margin = new Thickness(0, 10, 0, 0);
                    if (Group.NODE.AccessControl == "Allow")
                    {
                        btnAllow.Visibility = Visibility.Collapsed;
                        btnBlock.Visibility = Visibility.Visible;
                    }
                    else if (Group.NODE.AccessControl == "Block")
                    {
                        btnAllow.Visibility = Visibility.Visible;
                        btnBlock.Visibility = Visibility.Collapsed;
                    }

                }   
                btnBack.Visibility = Visibility.Visible;
                btnFileUpload.Visibility = Visibility.Collapsed;
                btnModify.Visibility = Visibility.Visible;
                btnApply.Visibility = Visibility.Collapsed;
            }
            NetworkMapInfo.bRefreshMap = false;
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

        private async void BackButton_Click(Object sender, RoutedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "NetworkMapPage";
            } 
            else
            {
                InProgress.IsActive = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                if (NetworkMapInfo.bRefreshMap)
                {
                    NetworkMapInfo.bRefreshMap = false;
                    GenieSoapApi soapApi = new GenieSoapApi();
                    UtilityTool util = new UtilityTool();
                    NetworkMapInfo.geteway = await util.GetGateway();
                    NetworkMapInfo.attachDeviceDic = await soapApi.GetAttachDevice();

                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
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
                }
                else
                {
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                }
                this.Frame.GoBack();
            }
        }

        private void ModifyButton_Click(Object sender, RoutedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "NetworkMapPage";
            } 
            else
            {
                txtBlockDeviceName.Visibility = Visibility.Collapsed;
                txtBoxDeviceName.Visibility = Visibility.Visible;
                Type.Visibility = Visibility.Collapsed;
                ComboType.Visibility = Visibility.Visible;
                btnModify.Visibility = Visibility.Collapsed;
                btnApply.Visibility = Visibility.Visible;
            }
        }

        private void ApplyButton_Click(Object sender, RoutedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "NetworkMapPage";
            } 
            else
            {
                string customDeviceName = txtBoxDeviceName.Text;
                if (customDeviceName != "")
                {
                    txtBlockDeviceName.Text = customDeviceName;
                    Title.Text = customDeviceName;
                }

                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                string customDeviceType = string.Empty;
                Uri _baseUri = new Uri("ms-appx:///");
                switch (ComboType.SelectedIndex)
                {
                    case 0:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/imacdev.png"));
                        customDeviceType = "imacdev";
                        Type.Text = loader.GetString("imacdev");
                        break;
                    case 1:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipad.png"));
                        customDeviceType = "ipad";
                        Type.Text = loader.GetString("ipad");
                        break;
                    case 2:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipadmini.png"));
                        customDeviceType = "ipadmini";
                        Type.Text = loader.GetString("ipadmini");
                        break;
                    case 3:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone.png"));
                        customDeviceType = "iphone";
                        Type.Text = loader.GetString("iphone");
                        break;
                    case 4:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone5.png"));
                        customDeviceType = "iphone5";
                        Type.Text = loader.GetString("iphone5");
                        break;
                    case 5:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipodtouch.png"));
                        customDeviceType = "ipodtouch";
                        Type.Text = loader.GetString("ipodtouch");
                        break;
                    case 6:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/amazonkindle.png"));
                        customDeviceType = "amazonkindle";
                        Type.Text = loader.GetString("amazonkindle");
                        break;
                    case 7:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androiddevice.png"));
                        customDeviceType = "androiddevice";
                        Type.Text = loader.GetString("androiddevice");
                        break;
                    case 8:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                        customDeviceType = "androidphone";
                        Type.Text = loader.GetString("androidphone");
                        break;
                    case 9:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidtablet.png"));
                        customDeviceType = "androidtablet";
                        Type.Text = loader.GetString("androidtablet");
                        break;
                    case 10:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/blurayplayer.png"));
                        customDeviceType = "blurayplayer";
                        Type.Text = loader.GetString("blurayplayer");
                        break;
                    case 11:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/bridge.png"));
                        customDeviceType = "bridge";
                        Type.Text = loader.GetString("bridge");
                        break;
                    case 12:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cablestb.png"));
                        customDeviceType = "cablestb";
                        Type.Text = loader.GetString("cablestb");
                        break;
                    case 13:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cameradev.png"));
                        customDeviceType = "cameradev";
                        Type.Text = loader.GetString("cameradev");
                        break;
                    case 14:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/dvr.png"));
                        customDeviceType = "dvr";
                        Type.Text = loader.GetString("dvr");
                        break;
                    case 15:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gamedev.png"));
                        customDeviceType = "gamedev";
                        Type.Text = loader.GetString("gamedev");
                        break;
                    case 16:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/linuxpc.png"));
                        customDeviceType = "linuxpc";
                        Type.Text = loader.GetString("linuxpc");
                        break;
                    case 17:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macminidev.png"));
                        customDeviceType = "macminidev";
                        Type.Text = loader.GetString("macminidev");
                        break;
                    case 18:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macprodev.png"));
                        customDeviceType = "macprodev";
                        Type.Text = loader.GetString("macprodev");
                        break;
                    case 19:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macbookdev.png"));
                        customDeviceType = "macbookdev";
                        Type.Text = loader.GetString("macbookdev");
                        break;
                    case 20:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mediadev.png"));
                        customDeviceType = "mediadev";
                        Type.Text = loader.GetString("mediadev");
                        break;
                    case 21:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/networkdev.png"));
                        customDeviceType = "networkdev";
                        Type.Text = loader.GetString("networkdev");
                        break;
                    case 22:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/stb.png"));
                        customDeviceType = "stb";
                        Type.Text = loader.GetString("stb");
                        break;
                    case 23:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/printerdev.png"));
                        customDeviceType = "printerdev";
                        Type.Text = loader.GetString("printerdev");
                        break;
                    case 24:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/repeater.png"));
                        customDeviceType = "repeater";
                        Type.Text = loader.GetString("repeater");
                        break;
                    case 25:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                        customDeviceType = "gatewaydev";
                        Type.Text = loader.GetString("gatewaydev");
                        break;
                    case 26:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/satellitestb.png"));
                        customDeviceType = "satellitestb";
                        Type.Text = loader.GetString("satellitestb");
                        break;
                    case 27:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/scannerdev.png"));
                        customDeviceType = "scannerdev";
                        Type.Text = loader.GetString("scannerdev");
                        break;
                    case 28:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/slingbox.png"));
                        customDeviceType = "slingbox";
                        Type.Text = loader.GetString("slingbox");
                        break;
                    case 29:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mobiledev.png"));
                        customDeviceType = "mobiledev";
                        Type.Text = loader.GetString("mobiledev");
                        break;
                    case 30:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/netstoragedev.png"));
                        customDeviceType = "netstoragedev";
                        Type.Text = loader.GetString("netstoragedev");
                        break;
                    case 31:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/switchdev.png"));
                        customDeviceType = "switchdev";
                        Type.Text = loader.GetString("switchdev");
                        break;
                    case 32:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tv.png"));
                        customDeviceType = "tv";
                        Type.Text = loader.GetString("tv");
                        break;
                    case 33:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tablepc.png"));
                        customDeviceType = "tablepc";
                        Type.Text = loader.GetString("tablepc");
                        break;
                    case 34:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/unixpc.png"));
                        customDeviceType = "unixpc";
                        Type.Text = loader.GetString("unixpc");
                        break;
                    case 35:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowspc.png"));
                        customDeviceType = "windowspc";
                        Type.Text = loader.GetString("windowspc");
                        break;
                    case 36:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowsphone.png"));
                        customDeviceType = "windowsphone";
                        Type.Text = loader.GetString("windowsphone");
                        break;
                    case 37:
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowstablet.png"));
                        customDeviceType = "windowstablet";
                        Type.Text = loader.GetString("windowstablet");
                        break;
                }

                string allDeviceInfo = string.Empty;
                string customDeviceInfo = string.Empty;
                bool bFound = false;
                if (NetworkMapInfo.fileContent != "")
                {
                    string[] AllDeviceInfo = NetworkMapInfo.fileContent.Split(';');
                    for (int i = 0; i < AllDeviceInfo.Length; i++)
                    {
                        if (AllDeviceInfo[i] != "" && AllDeviceInfo[i] != null)
                        {
                            string[] DeviceInfo = AllDeviceInfo[i].Split(',');
                            if (DeviceInfo[0] == MACAddress.Text)
                            {
                                bFound = true;
                                DeviceInfo[1] = customDeviceName;
                                DeviceInfo[2] = customDeviceType;
                            }
                            customDeviceInfo = DeviceInfo[0] + "," + DeviceInfo[1] + "," + DeviceInfo[2] + ";";
                            allDeviceInfo += customDeviceInfo;
                        }
                    }
                    if (!bFound)
                    {
                        allDeviceInfo += MACAddress.Text + "," + customDeviceName + "," + customDeviceType + ";";
                    }
                    NetworkMapInfo.fileContent = allDeviceInfo;
                }
                else
                {
                    NetworkMapInfo.fileContent = MACAddress.Text + "," + customDeviceName + "," + customDeviceType + ";";
                }
                WriteDeviceInfoFile();

                txtBlockDeviceName.Visibility = Visibility.Visible;
                txtBoxDeviceName.Visibility = Visibility.Collapsed;
                Type.Visibility = Visibility.Visible;
                ComboType.Visibility = Visibility.Collapsed;
                btnModify.Visibility = Visibility.Visible;
                btnApply.Visibility = Visibility.Collapsed;

                NetworkMapInfo.bRefreshMap = true;
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

        public async void WriteDeviceInfoFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync("CustomDeviceInfo.txt", CreationCollisionOption.ReplaceExisting);     //CustomDeviceInfo.txt中保存本地修改的设备信息，包括设备MAC地址、设备名和设备类型，格式为"MACAddress,DeviceName,DeviceType;"
            try
            {                 
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, NetworkMapInfo.fileContent);
                }
            }
            catch (FileNotFoundException)
            {

            }
        }

        private void txtBoxDeviceName_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (e.Key == Windows.System.VirtualKey.Enter)
            {
                ComboType.Focus(FocusState.Programmatic);
            } 
            else
            {
                base.OnKeyDown(e);
            }
        }    
  
        private async void AllowButton_Click(Object sender, RoutedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "NetworkMapPage";
            } 
            else
            {
                InProgress.IsActive = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                GenieSoapApi soapApi = new GenieSoapApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                while (dicResponse == null || dicResponse.Count == 0)
                {
                    dicResponse = await soapApi.SetBlockDeviceByMAC(NetworkMapInfo.deviceMacaddr, "Allow");
                }
                if (int.Parse(dicResponse["ResponseCode"]) == 0)
                {
                    btnAllow.Visibility = Visibility.Collapsed;
                    btnBlock.Visibility = Visibility.Visible;
                    NetworkMapInfo.bRefreshMap = true;
                }
                else
                {
                    var messageDialog = new MessageDialog(dicResponse["error_message"]);
                    await messageDialog.ShowAsync();
                }
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
        }

        private async void BlockButton_Click(Object sender, RoutedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "NetworkMapPage";
            } 
            else
            {
                InProgress.IsActive = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                GenieSoapApi soapApi = new GenieSoapApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                while (dicResponse == null || dicResponse.Count == 0)
                {
                    dicResponse = await soapApi.SetBlockDeviceByMAC(NetworkMapInfo.deviceMacaddr, "Block");
                }
                if (int.Parse(dicResponse["ResponseCode"]) == 0)
                {
                    btnAllow.Visibility = Visibility.Visible;
                    btnBlock.Visibility = Visibility.Collapsed;
                    NetworkMapInfo.bRefreshMap = true;
                }
                else
                {
                    var messageDialog = new MessageDialog(dicResponse["error_message"]);
                    await messageDialog.ShowAsync();
                }
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
        }

        //private async void btnWhat_Click(object sender, RoutedEventArgs e)
        //{
        //    var uri = new Uri(((HyperlinkButton)sender).Tag.ToString());
        //    await Windows.System.Launcher.LaunchUriAsync(uri);
        //}
    }
}
