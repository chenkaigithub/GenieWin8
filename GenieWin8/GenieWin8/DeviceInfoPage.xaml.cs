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
//using Windows.UI.Popups;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class DeviceInfoPage : GenieWin8.Common.LayoutAwarePage
    {
        public DeviceInfoPage()
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
            var Group = DeviceSource.GetGroup((String)(navigationParameter));

            StpTitle.Children.Clear();
            //StpDeviceInfo.Children.Clear();

            //Image TitleImage = new Image();
            TitleImage.HorizontalAlignment = HorizontalAlignment.Left;
            TitleImage.VerticalAlignment = VerticalAlignment.Center;
            TitleImage.Margin = new Thickness(10, 0, 0, 0);
            TitleImage.Width = 100; TitleImage.Height = 100;

            //TextBlock Title = new TextBlock();
            //Title.Text = Group.DeviceName;
            Title.Text = Group.NODE.deviceName;
            Title.FontSize = 40;
            Title.VerticalAlignment = VerticalAlignment.Center;
            Title.Margin = new Thickness(10, 0, 0, 0);
            Title.FontWeight = FontWeights.Bold;

            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            //DeviceName
            //TextBlock txtDeviceName = new TextBlock();
            var strtext = loader.GetString("txtDeviceName");
            txtDeviceName.Text = strtext;
            txtDeviceName.FontSize = 25;
            txtDeviceName.Margin = new Thickness(10, 10, 0, 0);
            txtDeviceName.HorizontalAlignment = HorizontalAlignment.Left;
            //TextBlock txtBlockDeviceName = new TextBlock();
            //DeviceName.Text = Group.DeviceName;
            txtBlockDeviceName.Text = Group.NODE.deviceName;
            txtBlockDeviceName.FontSize = 30;
            txtBlockDeviceName.HorizontalAlignment = HorizontalAlignment.Center;
            //TextBox txtBoxDeviceName = new TextBox();
            txtBoxDeviceName.Text = Group.NODE.deviceName;
            txtBoxDeviceName.FontSize = 26;
            txtBoxDeviceName.HorizontalAlignment = HorizontalAlignment.Center;
            txtBoxDeviceName.TextAlignment = TextAlignment.Center;
            txtBoxDeviceName.Width = 400;
            txtBoxDeviceName.Visibility = Visibility.Collapsed;

            //StackPanel StpDeviceName = new StackPanel();
            //StpDeviceName.Children.Add(txtDeviceName);
            //StpDeviceName.Children.Add(txtBlockDeviceName);
            //StpDeviceName.Children.Add(txtBoxDeviceName);

            //Type
            //StackPanel StpType = new StackPanel();
            if (Group.NODE.deviceType != null)
            {
                //TextBlock txtType = new TextBlock();
                strtext = loader.GetString("txtType");
                txtType.Text = strtext;
                txtType.FontSize = 25;
                txtType.Margin = new Thickness(10, 10, 0, 0);
                txtType.HorizontalAlignment = HorizontalAlignment.Left;
                //TextBlock Type = new TextBlock();
                //Type.Text = Group.DeviceType;
                //Type.Text = Group.NODE.deviceType;
                switch (Group.NODE.deviceType)
                {
                    case "gatewaydev":
                        Type.Text = loader.GetString("gatewaydev");
                        break;
                    case "networkdev":
                        Type.Text = loader.GetString("networkdev");
                        break;
                    case "windowspc":
                        Type.Text = loader.GetString("windowspc");
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
                    case "gamedev":
                        Type.Text = loader.GetString("gamedev");
                        break;
                    //case "amazonkindledev":
                    //    Type.Text = loader.GetString("amazonkindledev");
                    //    break;
                    //case "ipadmini":
                    //    Type.Text = loader.GetString("ipadmini");
                    //    break;
                    //case "iphone5":
                    //    Type.Text = loader.GetString("iphone5");
                    //    break;
                    case "imacdev":
                        Type.Text = loader.GetString("imacdev");
                        break;
                    case "ipad":
                        Type.Text = loader.GetString("ipad");
                        break;
                    case "iphone":
                        Type.Text = loader.GetString("iphone");
                        break;
                    case "ipodtouch":
                        Type.Text = loader.GetString("ipodtouch");
                        break;
                    case "linuxpc":
                        Type.Text = loader.GetString("linuxpc");
                        break;
                    case "macbookdev":
                        Type.Text = loader.GetString("macbookdev");
                        break;
                    case "macminidev":
                        Type.Text = loader.GetString("macminidev");
                        break;
                    case "macprodev":
                        Type.Text = loader.GetString("macprodev");
                        break;
                    case "mediadev":
                        Type.Text = loader.GetString("mediadev");
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
                    case "printerdev":
                        Type.Text = loader.GetString("printerdev");
                        break;
                    case "repeater":
                        Type.Text = loader.GetString("repeater");
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
                    case "stb":
                        Type.Text = loader.GetString("stb");
                        break;
                    case "tablepc":
                        Type.Text = loader.GetString("tablepc");
                        break;
                    case "tv":
                        Type.Text = loader.GetString("tv");
                        break;
                    case "unixpc":
                        Type.Text = loader.GetString("unixpc");
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
                    case "dvr":
                        Type.Text = loader.GetString("dvr");
                        break;
                }
                Type.FontSize = 30;
                Type.HorizontalAlignment = HorizontalAlignment.Center;

                for (int i = 0; i < 33; i++ )
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
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                            strtext = loader.GetString("gatewaydev");
                            texttype.Text = strtext;
                            break;
                        case 1:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/networkdev.png"));
                            strtext = loader.GetString("networkdev");
                            texttype.Text = strtext;
                            break;
                        case 2:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowspc.png"));
                            strtext = loader.GetString("windowspc");
                            texttype.Text = strtext;
                            break;
                        case 3:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/blurayplayer.png"));
                            strtext = loader.GetString("blurayplayer");
                            texttype.Text = strtext;
                            break;
                        case 4:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/bridge.png"));
                            strtext = loader.GetString("bridge");
                            texttype.Text = strtext;
                            break;
                        case 5:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cablestb.png"));
                            strtext = loader.GetString("cablestb");
                            texttype.Text = strtext;
                            break;
                        case 6:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cameradev.png"));
                            strtext = loader.GetString("cameradev");
                            texttype.Text = strtext;
                            break;
                        case 7:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gamedev.png"));
                            strtext = loader.GetString("gamedev");
                            texttype.Text = strtext;
                            break;
                        case 8:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/imacdev.png"));
                            strtext = loader.GetString("imacdev");
                            texttype.Text = strtext;
                            break;
                        case 9:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipad.png"));
                            strtext = loader.GetString("ipad");
                            texttype.Text = strtext;
                            break;
                        case 10:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone.png"));
                            strtext = loader.GetString("iphone");
                            texttype.Text = strtext;
                            break;
                        case 11:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipodtouch.png"));
                            strtext = loader.GetString("ipodtouch");
                            texttype.Text = strtext;
                            break;
                        case 12:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/linuxpc.png"));
                            strtext = loader.GetString("linuxpc");
                            texttype.Text = strtext;
                            break;
                        case 13:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macbookdev.png"));
                            strtext = loader.GetString("macbookdev");
                            texttype.Text = strtext;
                            break;
                        case 14:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macminidev.png"));
                            strtext = loader.GetString("macminidev");
                            texttype.Text = strtext;
                            break;
                        case 15:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macprodev.png"));
                            strtext = loader.GetString("macprodev");
                            texttype.Text = strtext;
                            break;
                        case 16:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mediadev.png"));
                            strtext = loader.GetString("mediadev");
                            texttype.Text = strtext;
                            break;
                        case 17:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mobiledev.png"));
                            strtext = loader.GetString("mobiledev");
                            texttype.Text = strtext;
                            break;
                        case 18:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/netstoragedev.png"));
                            strtext = loader.GetString("netstoragedev");
                            texttype.Text = strtext;
                            break;
                        case 19:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/switchdev.png"));
                            strtext = loader.GetString("switchdev");
                            texttype.Text = strtext;
                            break;
                        case 20:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/printerdev.png"));
                            strtext = loader.GetString("printerdev");
                            texttype.Text = strtext;
                            break;
                        case 21:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/repeater.png"));
                            strtext = loader.GetString("repeater");
                            texttype.Text = strtext;
                            break;
                        case 22:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/satellitestb.png"));
                            strtext = loader.GetString("satellitestb");
                            texttype.Text = strtext;
                            break;
                        case 23:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/scannerdev.png"));
                            strtext = loader.GetString("scannerdev");
                            texttype.Text = strtext;
                            break;
                        case 24:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/slingbox.png"));
                            strtext = loader.GetString("slingbox");
                            texttype.Text = strtext;
                            break;
                        case 25:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/stb.png"));
                            strtext = loader.GetString("stb");
                            texttype.Text = strtext;
                            break;
                        case 26:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tablepc.png"));
                            strtext = loader.GetString("tablepc");
                            texttype.Text = strtext;
                            break;
                        case 27:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tv.png"));
                            strtext = loader.GetString("tv");
                            texttype.Text = strtext;
                            break;
                        case 28:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/unixpc.png"));
                            strtext = loader.GetString("unixpc");
                            texttype.Text = strtext;
                            break;
                        case 29:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androiddevice.png"));
                            strtext = loader.GetString("androiddevice");
                            texttype.Text = strtext;
                            break;
                        case 30:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                            strtext = loader.GetString("androidphone");
                            texttype.Text = strtext;
                            break;
                        case 31:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidtablet.png"));
                            strtext = loader.GetString("androidtablet");
                            texttype.Text = strtext;
                            break;
                        case 32:
                            imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/dvr.png"));
                            strtext = loader.GetString("dvr");
                            texttype.Text = strtext;
                            break;
                    }
                    stpDevice.Children.Add(imgDevice);
                    stpDevice.Children.Add(texttype);
                    ComboType.Items.Add(stpDevice);
                }
                //ComboBox ComboType = new ComboBox();
                //Image imgDevice = new Image();
                //Uri _baseUri = new Uri("ms-appx:///");
                //imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                //imgDevice.Stretch = Stretch.UniformToFill;
                //imgDevice.Width = 25;
                //imgDevice.Height = 25;
                //StackPanel stpDevice = new StackPanel();
                //stpDevice.Orientation = Orientation.Horizontal;
                //strtext = loader.GetString("gatewaydev");
                //TextBlock texttype = new TextBlock();
                //texttype.Text = strtext;
                //texttype.Margin = new Thickness(5, 0, 0, 0);
                //stpDevice.Children.Add(imgDevice);
                //stpDevice.Children.Add(texttype);
                //ComboType.Items.Add(stpDevice);

                //strtext = loader.GetString("networkdev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("windowspc");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("blurayplayer");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("bridge");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("cablestb");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("cameradev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("gamedev");
                //ComboType.Items.Add(strtext);
                ////strtext = loader.GetString("amazonkindledev");
                ////ComboType.Items.Add(strtext);
                ////strtext = loader.GetString("ipadmini");
                ////ComboType.Items.Add(strtext);
                ////strtext = loader.GetString("iphone5");
                ////ComboType.Items.Add(strtext);
                //strtext = loader.GetString("imacdev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("ipad");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("iphone");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("ipodtouch");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("linuxpc");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("macbookdev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("macminidev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("macprodev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("mediadev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("mobiledev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("netstoragedev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("switchdev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("printerdev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("repeater");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("satellitestb");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("scannerdev");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("slingbox");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("stb");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("tablepc");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("tv");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("unixpc");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("androiddevice");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("androidphone");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("androidtablet");
                //ComboType.Items.Add(strtext);
                //strtext = loader.GetString("dvr");
                //ComboType.Items.Add(strtext);
                switch (Group.NODE.deviceType)
                {
                    case "gatewaydev":
                        ComboType.SelectedIndex = 0;
                        break;
                    case "networkdev":
                        ComboType.SelectedIndex = 1;
                        break;
                    case "windowspc":
                        ComboType.SelectedIndex = 2;
                        break;
                    case "blurayplayer":
                        ComboType.SelectedIndex = 3;
                        break;
                    case "bridge":
                        ComboType.SelectedIndex = 4;
                        break;
                    case "cablestb":
                        ComboType.SelectedIndex = 5;
                        break;
                    case "cameradev":
                        ComboType.SelectedIndex = 6;
                        break;
                    case "gamedev":
                        ComboType.SelectedIndex = 7;
                        break;
                    //case "amazonkindledev":
                    //    ComboType.SelectedIndex = 8;
                    //    break;
                    //case "ipadmini":
                    //    ComboType.SelectedIndex = 9;
                    //    break;
                    //case "iphone5":
                    //    ComboType.SelectedIndex = 10;
                    //    break;
                    case "imacdev":
                        ComboType.SelectedIndex = 8;
                        break;
                    case "ipad":
                        ComboType.SelectedIndex = 9;
                        break;
                    case "iphone":
                        ComboType.SelectedIndex = 10;
                        break;
                    case "ipodtouch":
                        ComboType.SelectedIndex = 11;
                        break;
                    case "linuxpc":
                        ComboType.SelectedIndex = 12;
                        break;
                    case "macbookdev":
                        ComboType.SelectedIndex = 13;
                        break;
                    case "macminidev":
                        ComboType.SelectedIndex = 14;
                        break;
                    case "macprodev":
                        ComboType.SelectedIndex = 15;
                        break;
                    case "mediadev":
                        ComboType.SelectedIndex = 16;
                        break;
                    case "mobiledev":
                        ComboType.SelectedIndex = 17;
                        break;
                    case "netstoragedev":
                        ComboType.SelectedIndex = 18;
                        break;
                    case "switchdev":
                        ComboType.SelectedIndex = 19;
                        break;
                    case "printerdev":
                        ComboType.SelectedIndex = 20;
                        break;
                    case "repeater":
                        ComboType.SelectedIndex = 21;
                        break;
                    case "satellitestb":
                        ComboType.SelectedIndex = 22;
                        break;
                    case "scannerdev":
                        ComboType.SelectedIndex = 23;
                        break;
                    case "slingbox":
                        ComboType.SelectedIndex = 24;
                        break;
                    case "stb":
                        ComboType.SelectedIndex = 25;
                        break;
                    case "tablepc":
                        ComboType.SelectedIndex = 26;
                        break;
                    case "tv":
                        ComboType.SelectedIndex = 27;
                        break;
                    case "unixpc":
                        ComboType.SelectedIndex = 28;
                        break;
                    case "androiddevice":
                        ComboType.SelectedIndex = 29;
                        break;
                    case "androidphone":
                        ComboType.SelectedIndex = 30;
                        break;
                    case "androidtablet":
                        ComboType.SelectedIndex = 31;
                        break;
                    case "dvr":
                        ComboType.SelectedIndex = 32;
                        break;
                }
                ComboType.Width = 400;
                ComboType.HorizontalAlignment = HorizontalAlignment.Center;
                ComboType.HorizontalContentAlignment = HorizontalAlignment.Center;
                ComboType.Visibility = Visibility.Collapsed;


                //StpType.Children.Add(txtType);
                //StpType.Children.Add(Type);
            }

            //IPAddress
            //TextBlock txtIPAddress = new TextBlock();
            strtext = loader.GetString("txtIPAddress");
            txtIPAddress.Text = strtext;
            txtIPAddress.FontSize = 25;
            txtIPAddress.Margin = new Thickness(10, 10, 0, 0);
            txtIPAddress.HorizontalAlignment = HorizontalAlignment.Left;
            //TextBlock IPAddress = new TextBlock();
            //IPAddress.Text = Group.IPAddress;
            IPAddress.Text = Group.NODE.IPaddress;
            IPAddress.FontSize = 30;
            IPAddress.HorizontalAlignment = HorizontalAlignment.Center;

            //StackPanel StpIPAddress = new StackPanel();
            //StpIPAddress.Children.Add(txtIPAddress);
            //StpIPAddress.Children.Add(IPAddress);

            //SignalStrength
            //StackPanel StpSignalStrength = new StackPanel();
            if (Group.NODE.signalStrength != null)
            {
                //TextBlock txtSignalStrength = new TextBlock();
                strtext = loader.GetString("txtSignalStrength");
                txtSignalStrength.Text = strtext;
                txtSignalStrength.FontSize = 25;
                txtSignalStrength.Margin = new Thickness(10, 10, 0, 0);
                txtSignalStrength.HorizontalAlignment = HorizontalAlignment.Left;
                //TextBlock SignalStrength = new TextBlock();
                //SignalStrength.Text = Group.SignalStrength;
                SignalStrength.Text = Group.NODE.signalStrength;
                SignalStrength.FontSize = 30;
                SignalStrength.HorizontalAlignment = HorizontalAlignment.Center;


                //StpSignalStrength.Children.Add(txtSignalStrength);
                //StpSignalStrength.Children.Add(SignalStrength);
            }

            //LinkRate
            //StackPanel StpLinkRate = new StackPanel();
            if (Group.NODE.linkRate != null)
            {
                //TextBlock txtLinkRate = new TextBlock();
                strtext = loader.GetString("txtLinkRate");
                txtLinkRate.Text = strtext;
                txtLinkRate.FontSize = 25;
                txtLinkRate.Margin = new Thickness(10, 10, 0, 0);
                txtLinkRate.HorizontalAlignment = HorizontalAlignment.Left;
                //TextBlock LinkRate = new TextBlock();
                //LinkRate.Text = Group.LinkRate;
                LinkRate.Text = Group.NODE.linkRate;
                LinkRate.FontSize = 30;
                LinkRate.HorizontalAlignment = HorizontalAlignment.Center;
                //HyperlinkButton btnWhat = new HyperlinkButton();
                strtext = loader.GetString("btnWhat");
                btnWhat.Content = strtext;
                btnWhat.Tag = "http://support.netgear.com/app/answers/list/kw/link%20rate";
                btnWhat.FontSize = 20;
                btnWhat.Margin = new Thickness(10, 0, 0, 0);
                btnWhat.Padding = new Thickness(0, 0, 0, 0);
                btnWhat.HorizontalAlignment = HorizontalAlignment.Right;


                //StpLinkRate.Children.Add(txtLinkRate);
                //StpLinkRate.Children.Add(LinkRate);
                //StpLinkRate.Children.Add(btnWhat);
            }

            //MACAddress
            //TextBlock txtMACAddress = new TextBlock();
            strtext = loader.GetString("txtMACAddress");
            txtMACAddress.Text = strtext;
            txtMACAddress.FontSize = 25;
            txtMACAddress.Margin = new Thickness(10, 0, 0, 0);
            txtMACAddress.HorizontalAlignment = HorizontalAlignment.Left;
            //TextBlock MACAddress = new TextBlock();
            //MACAddress.Text = Group.MACAddress;
            MACAddress.Text = Group.NODE.MACaddress;
            MACAddress.FontSize = 30;
            MACAddress.HorizontalAlignment = HorizontalAlignment.Center;

            //StackPanel StpMACAddress = new StackPanel();
            //StpMACAddress.Children.Add(txtMACAddress);
            //StpMACAddress.Children.Add(MACAddress);

            //Buttons
            //Button btnBack = new Button();
            strtext = loader.GetString("btnBack");
            btnBack.Content = strtext;
            btnBack.FontSize = 25;
            btnBack.Width = 200;
            btnBack.HorizontalAlignment = HorizontalAlignment.Center;
            btnBack.Click += new RoutedEventHandler(BackButton_Click);
            //Button btnModify = new Button();
            strtext = loader.GetString("btnModify");
            btnModify.Content = strtext;
            btnModify.FontSize = 25;
            btnModify.Width = 200;
            btnModify.HorizontalAlignment = HorizontalAlignment.Center;
            btnModify.Click += new RoutedEventHandler(ModifyButton_Click);
            strtext = loader.GetString("btnApply");
            btnApply.Content = strtext;
            btnApply.FontSize = 25;
            btnApply.Width = 200;
            btnApply.HorizontalAlignment = HorizontalAlignment.Center;
            btnApply.Click += new RoutedEventHandler(ApplyButton_Click);
            //Button btnFileUpload = new Button();
            strtext = loader.GetString("btnFileUpload");
            btnFileUpload.Content = strtext;
            btnFileUpload.FontSize = 25;
            btnFileUpload.Width = 200;
            btnFileUpload.HorizontalAlignment = HorizontalAlignment.Center;

            if (Group.NODE.uniqueId == "Router")
            {
                Uri _baseUri = new Uri("ms-appx:///");
                TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                StpTitle.Children.Add(TitleImage);
                StpTitle.Children.Add(Title);

                //TextBlock txtRoutename = new TextBlock();
                strtext = loader.GetString("txtRoutename");
                txtRoutename.Text = strtext;
                txtRoutename.FontSize = 25;
                txtRoutename.Margin = new Thickness(10, 10, 0, 0);
                txtRoutename.HorizontalAlignment = HorizontalAlignment.Left;
                //TextBlock RouteName = new TextBlock();
                //RouteName.Text = Group.DeviceName;
                RouteName.Text = Group.NODE.deviceName;
                RouteName.FontSize = 30;
                RouteName.HorizontalAlignment = HorizontalAlignment.Center;

                //StackPanel StpRouter = new StackPanel();
                //StpRouter.Children.Add(txtRoutename);
                //StpRouter.Children.Add(RouteName);

                //Grid GridButton = new Grid();
                btnBack.Margin = new Thickness(0, 10, 0, 0);
                //GridButton.Children.Add(btnBack);

                StpRouter.Visibility = Visibility.Visible;
                StpDeviceName.Visibility = Visibility.Collapsed;
                StpType.Visibility = Visibility.Collapsed;
                StpLinkRate.Visibility = Visibility.Collapsed;
                StpSignalStrength.Visibility = Visibility.Collapsed;
                btnBack.Visibility = Visibility.Visible;
                btnFileUpload.Visibility = Visibility.Collapsed;
                btnModify.Visibility = Visibility.Collapsed;
                btnApply.Visibility = Visibility.Collapsed;
                //StpDeviceInfo.Children.Add(StpRouter);
                //StpDeviceInfo.Children.Add(StpIPAddress);
                //StpDeviceInfo.Children.Add(StpMACAddress);
                //StpDeviceInfo.Children.Add(GridButton);
            }
            else if (Group.NODE.uniqueId == "LocalDevice")
            {
                Uri _baseUri = new Uri("ms-appx:///");
                //TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                switch (Group.NODE.deviceType)
                {
                    case "gatewaydev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                        break;
                    case "networkdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/networkdev.png"));
                        break;
                    case "windowspc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowspc.png"));
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
                    case "gamedev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gamedev.png"));
                        break;
                    //case "amazonkindledev":
                    //    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                    //    break;
                    //case "ipadmini":
                    //    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                    //    break;
                    //case "iphone5":
                    //    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                    //    break;
                    case "imacdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/imacdev.png"));
                        break;
                    case "ipad":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipad.png"));
                        break;
                    case "iphone":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone.png"));
                        break;
                    case "ipodtouch":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipodtouch.png"));
                        break;
                    case "linuxpc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/linuxpc.png"));
                        break;
                    case "macbookdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macbookdev.png"));
                        break;
                    case "macminidev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macminidev.png"));
                        break;
                    case "macprodev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macprodev.png"));
                        break;
                    case "mediadev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mediadev.png"));
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
                    case "printerdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/printerdev.png"));
                        break;
                    case "repeater":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/repeater.png"));
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
                    case "stb":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/stb.png"));
                        break;
                    case "tablepc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tablepc.png"));
                        break;
                    case "tv":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tv.png"));
                        break;
                    case "unixpc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/unixpc.png"));
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
                    case "dvr":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/dvr.png"));
                        break;
                }
                StpTitle.Children.Add(TitleImage);
                StpTitle.Children.Add(Title);

                //Grid GridButton = new Grid();
                btnBack.Margin = new Thickness(0, 10, 0, 0);
                //GridButton.Children.Add(btnBack);

                StpRouter.Visibility = Visibility.Collapsed;
                StpDeviceName.Visibility = Visibility.Visible;
                StpType.Visibility = Visibility.Collapsed;
                //StpDeviceInfo.Children.Add(StpDeviceName);
                //StpDeviceInfo.Children.Add(StpIPAddress);
                //StpDeviceInfo.Children.Add(StpSignalStrength);
                //if (Group.NODE.linkRate != null) StpDeviceInfo.Children.Add(StpLinkRate);
                //if (Group.NODE.signalStrength != null) StpDeviceInfo.Children.Add(StpMACAddress);
                //StpDeviceInfo.Children.Add(GridButton);
                if (Group.NODE.linkRate == null)
                    StpLinkRate.Visibility = Visibility.Collapsed;
                else
                    StpLinkRate.Visibility = Visibility.Visible;
                if (Group.NODE.signalStrength == null)
                    StpSignalStrength.Visibility = Visibility.Collapsed;
                else
                    StpSignalStrength.Visibility = Visibility.Visible;

                btnBack.Visibility = Visibility.Visible;
                btnFileUpload.Visibility = Visibility.Collapsed;
                btnModify.Visibility = Visibility.Collapsed;
                btnApply.Visibility = Visibility.Collapsed;
            }
            else
            {
                Uri _baseUri = new Uri("ms-appx:///");
                //TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/networkdev.png"));
                switch (Group.NODE.deviceType)
                {
                    case "gatewaydev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                        break;
                    case "networkdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/networkdev.png"));
                        break;
                    case "windowspc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowspc.png"));
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
                    case "gamedev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gamedev.png"));
                        break;
                    //case "amazonkindledev":
                    //    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                    //    break;
                    //case "ipadmini":
                    //    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                    //    break;
                    //case "iphone5":
                    //    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                    //    break;
                    case "imacdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/imacdev.png"));
                        break;
                    case "ipad":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipad.png"));
                        break;
                    case "iphone":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone.png"));
                        break;
                    case "ipodtouch":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipodtouch.png"));
                        break;
                    case "linuxpc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/linuxpc.png"));
                        break;
                    case "macbookdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macbookdev.png"));
                        break;
                    case "macminidev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macminidev.png"));
                        break;
                    case "macprodev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macprodev.png"));
                        break;
                    case "mediadev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mediadev.png"));
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
                    case "printerdev":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/printerdev.png"));
                        break;
                    case "repeater":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/repeater.png"));
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
                    case "stb":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/stb.png"));
                        break;
                    case "tablepc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tablepc.png"));
                        break;
                    case "tv":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tv.png"));
                        break;
                    case "unixpc":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/unixpc.png"));
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
                    case "dvr":
                        TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/dvr.png"));
                        break;
                }
                StpTitle.Children.Add(TitleImage);
                StpTitle.Children.Add(Title);

                //Grid GridButton = new Grid();
                btnBack.Margin = new Thickness(200, 10, 0, 0);
                btnModify.Margin = new Thickness(0, 10, 200, 0);
                btnApply.Margin = new Thickness(0, 10, 200, 0);
                //GridButton.Children.Add(btnBack);
                //GridButton.Children.Add(btnModify);

                StpRouter.Visibility = Visibility.Collapsed;
                StpDeviceName.Visibility = Visibility.Visible;
                //StpDeviceInfo.Children.Add(StpDeviceName);
                //if (Group.NODE.deviceType != null) StpDeviceInfo.Children.Add(StpType);                    
                //StpDeviceInfo.Children.Add(StpIPAddress);
                //if (Group.NODE.signalStrength != null) StpDeviceInfo.Children.Add(StpSignalStrength);
                //if (Group.NODE.linkRate != null) StpDeviceInfo.Children.Add(StpLinkRate);
                //StpDeviceInfo.Children.Add(StpMACAddress);
                //StpDeviceInfo.Children.Add(GridButton);
                if (Group.NODE.deviceType == null)
                    StpType.Visibility = Visibility.Collapsed;
                else
                    StpType.Visibility = Visibility.Visible;

                if (Group.NODE.signalStrength == null)
                    StpSignalStrength.Visibility = Visibility.Collapsed;
                else
                    StpSignalStrength.Visibility = Visibility.Visible;

                if (Group.NODE.linkRate == null)
                    StpLinkRate.Visibility = Visibility.Collapsed;
                else
                    StpLinkRate.Visibility = Visibility.Visible;

                btnBack.Visibility = Visibility.Visible;
                btnFileUpload.Visibility = Visibility.Collapsed;
                btnModify.Visibility = Visibility.Visible;
                btnApply.Visibility = Visibility.Collapsed;
            }
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

        private void BackButton_Click(Object sender, RoutedEventArgs e)
        {
            this.Frame.GoBack();
        }

        private void ModifyButton_Click(Object sender, RoutedEventArgs e)
        {
            txtBlockDeviceName.Visibility = Visibility.Collapsed;
            txtBoxDeviceName.Visibility = Visibility.Visible;
            Type.Visibility = Visibility.Collapsed;
            ComboType.Visibility = Visibility.Visible;
            btnModify.Visibility = Visibility.Collapsed;
            btnApply.Visibility = Visibility.Visible;
        }

        private void ApplyButton_Click(Object sender, RoutedEventArgs e)
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
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                    customDeviceType = "gatewaydev";
                    Type.Text = loader.GetString("gatewaydev");
                    break;
                case 1:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/networkdev.png"));
                    customDeviceType = "networkdev";
                    Type.Text = loader.GetString("networkdev");
                    break;
                case 2:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowspc.png"));
                    customDeviceType = "windowspc";
                    Type.Text = loader.GetString("windowspc");
                    break;
                case 3:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/blurayplayer.png"));
                    customDeviceType = "blurayplayer";
                    Type.Text = loader.GetString("blurayplayer");
                    break;
                case 4:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/bridge.png"));
                    customDeviceType = "bridge";
                    Type.Text = loader.GetString("bridge");
                    break;
                case 5:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cablestb.png"));
                    customDeviceType = "cablestb";
                    Type.Text = loader.GetString("cablestb");
                    break;
                case 6:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cameradev.png"));
                    customDeviceType = "cameradev";
                    Type.Text = loader.GetString("cameradev");
                    break;
                case 7:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gamedev.png"));
                    customDeviceType = "gamedev";
                    Type.Text = loader.GetString("gamedev");
                    break;
                //case 8:
                //    customDeviceType = "amazonkindledev";
                //    Type.Text = loader.GetString("amazonkindledev");
                //    break;
                //case 9:
                //    customDeviceType = "ipadmini";
                //    Type.Text = loader.GetString("ipadmini");
                //    break;
                //case 10:
                //    customDeviceType = "iphone5";
                //    Type.Text = loader.GetString("iphone5");
                //    break;
                case 8:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/imacdev.png"));
                    customDeviceType = "imacdev";
                    Type.Text = loader.GetString("imacdev");
                    break;
                case 9:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipad.png"));
                    customDeviceType = "ipad";
                    Type.Text = loader.GetString("ipad");
                    break;
                case 10:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone.png"));
                    customDeviceType = "iphone";
                    Type.Text = loader.GetString("iphone");
                    break;
                case 11:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipodtouch.png"));
                    customDeviceType = "ipodtouch";
                    Type.Text = loader.GetString("ipodtouch");
                    break;
                case 12:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/linuxpc.png"));
                    customDeviceType = "linuxpc";
                    Type.Text = loader.GetString("linuxpc");
                    break;
                case 13:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macbookdev.png"));
                    customDeviceType = "macbookdev";
                    Type.Text = loader.GetString("macbookdev");
                    break;
                case 14:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macminidev.png"));
                    customDeviceType = "macminidev";
                    Type.Text = loader.GetString("macminidev");
                    break;
                case 15:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macprodev.png"));
                    customDeviceType = "macprodev";
                    Type.Text = loader.GetString("macprodev");
                    break;
                case 16:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mediadev.png"));
                    customDeviceType = "mediadev";
                    Type.Text = loader.GetString("mediadev");
                    break;
                case 17:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mobiledev.png"));
                    customDeviceType = "mobiledev";
                    Type.Text = loader.GetString("mobiledev");
                    break;
                case 18:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/netstoragedev.png"));
                    customDeviceType = "netstoragedev";
                    Type.Text = loader.GetString("netstoragedev");
                    break;
                case 19:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/switchdev.png"));
                    customDeviceType = "switchdev";
                    Type.Text = loader.GetString("switchdev");
                    break;
                case 20:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/printerdev.png"));
                    customDeviceType = "printerdev";
                    Type.Text = loader.GetString("printerdev");
                    break;
                case 21:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/repeater.png"));
                    customDeviceType = "repeater";
                    Type.Text = loader.GetString("repeater");
                    break;
                case 22:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/satellitestb.png"));
                    customDeviceType = "satellitestb";
                    Type.Text = loader.GetString("satellitestb");
                    break;
                case 23:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/scannerdev.png"));
                    customDeviceType = "scannerdev";
                    Type.Text = loader.GetString("scannerdev");
                    break;
                case 24:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/slingbox.png"));
                    customDeviceType = "slingbox";
                    Type.Text = loader.GetString("slingbox");
                    break;
                case 25:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/stb.png"));
                    customDeviceType = "stb";
                    Type.Text = loader.GetString("stb");
                    break;
                case 26:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tablepc.png"));
                    customDeviceType = "tablepc";
                    Type.Text = loader.GetString("tablepc");
                    break;
                case 27:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tv.png"));
                    customDeviceType = "tv";
                    Type.Text = loader.GetString("tv");
                    break;
                case 28:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/unixpc.png"));
                    customDeviceType = "unixpc";
                    Type.Text = loader.GetString("unixpc");
                    break;
                case 29:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androiddevice.png"));
                    customDeviceType = "androiddevice";
                    Type.Text = loader.GetString("androiddevice");
                    break;
                case 30:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                    customDeviceType = "androidphone";
                    Type.Text = loader.GetString("androidphone");
                    break;
                case 31:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidtablet.png"));
                    customDeviceType = "androidtablet";
                    Type.Text = loader.GetString("androidtablet");
                    break;
                case 32:
                    TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/dvr.png"));
                    customDeviceType = "dvr";
                    Type.Text = loader.GetString("dvr");
                    break;
            }

            string allDeviceInfo = string.Empty;
            string customDeviceInfo = string.Empty;
            bool bFound = false;
            if (NetworkMapDodel.fileContent != "")
            {
                string[] AllDeviceInfo = NetworkMapDodel.fileContent.Split(';');
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
                NetworkMapDodel.fileContent = allDeviceInfo;
            }
            else
            {
                NetworkMapDodel.fileContent = MACAddress.Text + "," + customDeviceName + "," + customDeviceType + ";";              
            }
            WriteDeviceInfoFile();

            txtBlockDeviceName.Visibility = Visibility.Visible;
            txtBoxDeviceName.Visibility = Visibility.Collapsed;
            Type.Visibility = Visibility.Visible;
            ComboType.Visibility = Visibility.Collapsed;
            btnModify.Visibility = Visibility.Visible;
            btnApply.Visibility = Visibility.Collapsed;
        }

        public async void WriteDeviceInfoFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile file = await storageFolder.CreateFileAsync("CustomDeviceInfo.txt", CreationCollisionOption.ReplaceExisting);     //CustomDeviceInfo.txt中保存本地修改的设备信息，包括设备MAC地址、设备名和设备类型，格式为"MACAddress,DeviceName,DeviceType;"
            try
            {                 
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, NetworkMapDodel.fileContent);
                }
            }
            catch (FileNotFoundException)
            {

            }
        }
    }
}
