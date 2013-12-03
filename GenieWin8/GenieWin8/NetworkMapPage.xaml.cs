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
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using GenieWin8.DataModel;
using Windows.Storage;
using System.Threading.Tasks;
//using Windows.UI.Popups;
using Windows.Networking.Connectivity;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class NetworkMapPage : GenieWin8.Common.LayoutAwarePage
    {
        private string routerImage;
        private static bool IsWifiSsidChanged;
        public NetworkMapPage()
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

        private async void OnWindowSizeChanged(Object sender, SizeChangedEventArgs e)
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
                if (DeviceSource.GetGroups().ElementAt(1).NODE.AccessControl != "")
                {
                    GenieSoapApi soapApi = new GenieSoapApi();
                    Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                    dicResponse = await soapApi.GetBlockDeviceEnableStatus();
                    if (int.Parse(dicResponse["ResponseCode"]) == 0)
                    {
                        NetworkMapInfo.IsAccessControlSupported = true;
                        if (dicResponse["NewBlockDeviceEnable"] == "0")
                        {
                            NetworkMapInfo.IsAccessControlEnabled = false;
                        }
                        else if (dicResponse["NewBlockDeviceEnable"] == "1")
                        {
                            NetworkMapInfo.IsAccessControlEnabled = true;
                        }
                    }
                    else
                    {
                        NetworkMapInfo.IsAccessControlSupported = false;
                    }
                }
                else
                {
                    NetworkMapInfo.IsAccessControlSupported = false;
                }

                //double Appwidth = e.Size.Width;
                //double AppHeight = e.Size.Height;	
                double PI = 3.141592653589793;
                MapFlipView.Items.Clear();
                double width = Window.Current.Bounds.Width;
                double height = Window.Current.Bounds.Height - 140;
                double r1 = width / 2 - 100;
                double r2 = height / 2 - 100;
                //m = （总设备数 - 1（路由器/交换机）- 1（本设备））/ 6 的商
                //n 为上式得余数，即最后一页除路由器（交换机）和本设备外的设备数
                var Group = DeviceSource.GetGroups();
                var DeviceNumber = Group.Count();
                int m = (DeviceNumber - 2) / 6;
                int n = (DeviceNumber - 2) % 6;
                for (int i = 0; i < m + 1; i++)
                {
                    if (i != m)
                    {
                        double Angle = 360.0 / 8;
                        Grid map = new Grid();

                        //internet图标
                        Image internet = new Image();
                        Uri _baseUri = new Uri("ms-appx:///");
                        internet.Source = new BitmapImage(new Uri(_baseUri, "Assets/internet72.png"));
                        internet.HorizontalAlignment = HorizontalAlignment.Right;
                        internet.Margin = new Thickness(0, 0, 50, 0);
                        internet.Width = 100; internet.Height = 100;

                        //路由器图标
                        Button BtnRouter = new Button();
                        //BtnRouter.Name = "Router";
                        BtnRouter.Name = Group.ElementAt(0).NODE.uniqueId;
                        BtnRouter.Width = 150;
                        BtnRouter.Height = 150;
                        BtnRouter.HorizontalAlignment = HorizontalAlignment.Center;
                        BtnRouter.VerticalAlignment = VerticalAlignment.Center;
                        Image imgRouter = new Image();
                        imgRouter.Source = new BitmapImage(new Uri(_baseUri, routerImage));
                        imgRouter.Stretch = Stretch.Uniform;
                        BtnRouter.Content = imgRouter;
                        BtnRouter.Margin = new Thickness(0, 0, 0, 0);
                        BtnRouter.Background = new SolidColorBrush();
                        BtnRouter.Click += new RoutedEventHandler(DeviceButton_Click);

                        //本设备图标
                        Button BtnLocalDevice = new Button();
                        BtnLocalDevice.Name = Group.ElementAt(1).NODE.uniqueId;
                        BtnLocalDevice.Width = 100;
                        BtnLocalDevice.Height = 100;
                        BtnLocalDevice.HorizontalAlignment = HorizontalAlignment.Left;
                        BtnLocalDevice.VerticalAlignment = VerticalAlignment.Top;
                        double xLocal = r1 * Math.Cos(Angle * PI / 180);
                        double yLocal = r2 * Math.Sin(Angle * PI / 180);
                        BtnLocalDevice.Margin = new Thickness(width / 2 + xLocal - 50, height / 2 - yLocal - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
                        Image imgLocalDevice = new Image();
                        imgLocalDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowstablet.png"));
                        imgLocalDevice.Stretch = Stretch.Uniform;
                        imgLocalDevice.Height = 50;
                        TextBlock LocalDeviceNameText = new TextBlock();
                        LocalDeviceNameText.Text = Group.ElementAt(1).NODE.deviceName;
                        LocalDeviceNameText.FontSize = 15;
                        LocalDeviceNameText.Margin = new Thickness(0, -5, 0, 0);
                        LocalDeviceNameText.Foreground = new SolidColorBrush(Color.FromArgb(255, 90, 90, 90));
                        LocalDeviceNameText.TextTrimming = TextTrimming.WordEllipsis;
                        StackPanel stpLocalDevice = new StackPanel();
                        stpLocalDevice.Children.Add(imgLocalDevice);
                        stpLocalDevice.Children.Add(LocalDeviceNameText);
                        BtnLocalDevice.Content = stpLocalDevice;
                        BtnLocalDevice.Background = new SolidColorBrush();
                        BtnLocalDevice.Click += new RoutedEventHandler(DeviceButton_Click);

                        for (int j = 0; j < 8; j++)
                        {
                            double x = r1 * Math.Cos(j * Angle * PI / 180);
                            double y = r2 * Math.Sin(j * Angle * PI / 180);
                            Line line = new Line();
                            line.X1 = width / 2; line.Y1 = height / 2;
                            line.X2 = width / 2 + x; line.Y2 = height / 2 - y;
                            line.Stroke = new SolidColorBrush(Colors.SeaGreen);
                            line.StrokeThickness = 2;
                            if (j == 0)
                            {
                                //判断路由器是否已连接因特网
                                UtilityTool util = new UtilityTool();
                                bool isConnectToInternet = util.IsConnectedToInternet();
                                if (!isConnectToInternet)
                                {
                                    line.Stroke = new SolidColorBrush(Colors.Red);
                                }
                            }
                            Image imgSignal = new Image();
                            if (j == 1)
                            {
                                DoubleCollection dc = new DoubleCollection();
                                dc.Add(2);
                                line.StrokeDashArray = dc;
                                int signal;
                                if (NetworkMapInfo.attachDeviceDic.Count == 0)
                                {
                                    signal = 100;
                                }
                                else
                                {
                                    string[] strSignal = Group.ElementAt(1).NODE.signalStrength.Split('%');
                                    signal = int.Parse(strSignal[0]);
                                }
                                if (Group.ElementAt(1).NODE.AccessControl == "Allow")
                                {
                                    if (signal <= 20)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_13.png"));
                                    }
                                    else if (signal > 20 && signal <= 40)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_23.png"));
                                    }
                                    else if (signal > 40 && signal <= 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_33.png"));
                                    }
                                    else if (signal > 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_43.png"));
                                    }
                                }
                                else
                                {
                                    if (signal <= 20)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag1.png"));
                                    }
                                    else if (signal > 20 && signal <= 40)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag2.png"));
                                    }
                                    else if (signal > 40 && signal <= 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag3.png"));
                                    }
                                    else if (signal > 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag4.png"));
                                    }
                                }
                                imgSignal.Stretch = Stretch.Fill;
                                imgSignal.Width = 30; imgSignal.Height = 30;
                                imgSignal.HorizontalAlignment = HorizontalAlignment.Left;
                                imgSignal.VerticalAlignment = VerticalAlignment.Top;
                                imgSignal.Margin = new Thickness(width / 2 + x / 2 - 15, height / 2 - y / 2 - 15, 0, 0);
                            }
                            else if (j > 1 && Group.ElementAt(6 * i + j).NODE.connectType == "wireless")
                            {
                                DoubleCollection dc = new DoubleCollection();
                                dc.Add(2);
                                line.StrokeDashArray = dc;
                                string[] signal = Group.ElementAt(6 * i + j).NODE.signalStrength.Split('%');
                                int result = int.Parse(signal[0]);
                                if (Group.ElementAt(6 * i + j).NODE.AccessControl == "Allow")
                                {
                                    if (result <= 20)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_13.png"));
                                    }
                                    else if (result > 20 && result <= 40)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_23.png"));
                                    }
                                    else if (result > 40 && result <= 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_33.png"));
                                    }
                                    else if (result > 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_43.png"));
                                    }
                                }
                                else
                                {
                                    if (result <= 20)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag1.png"));
                                    }
                                    else if (result > 20 && result <= 40)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag2.png"));
                                    }
                                    else if (result > 40 && result <= 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag3.png"));
                                    }
                                    else if (result > 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag4.png"));
                                    }
                                }
                                imgSignal.Stretch = Stretch.Fill;
                                imgSignal.Width = 30; imgSignal.Height = 30;
                                imgSignal.HorizontalAlignment = HorizontalAlignment.Left;
                                imgSignal.VerticalAlignment = VerticalAlignment.Top;
                                imgSignal.Margin = new Thickness(width / 2 + x / 2 - 15, height / 2 - y / 2 - 15, 0, 0);
                            }
                            map.Children.Add(line);
                            map.Children.Add(imgSignal);

                            if (j > 1)
                            {
                                Button BtnDevice = new Button();
                                BtnDevice.Name = Group.ElementAt(6 * i + j).NODE.uniqueId;
                                BtnDevice.Width = 100;
                                BtnDevice.Height = 100;
                                BtnDevice.HorizontalAlignment = HorizontalAlignment.Left;
                                BtnDevice.VerticalAlignment = VerticalAlignment.Top;
                                BtnDevice.Margin = new Thickness(width / 2 + x - 50, height / 2 - y - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
                                Image imgDevice = new Image();
                                switch (Group.ElementAt(6 * i + j).NODE.deviceType)
                                {
                                    case "imacdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/imacdev.png"));
                                        break;
                                    case "ipad":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipad.png"));
                                        break;
                                    case "ipadmini":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipadmini.png"));
                                        break;
                                    case "iphone":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone.png"));
                                        break;
                                    case "iphone5":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone5.png"));
                                        break;
                                    case "ipodtouch":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipodtouch.png"));
                                        break;
                                    case "amazonkindle":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/amazonkindle.png"));
                                        break;
                                    case "androiddevice":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androiddevice.png"));
                                        break;
                                    case "androidphone":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                                        break;
                                    case "androidtablet":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidtablet.png"));
                                        break;
                                    case "blurayplayer":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/blurayplayer.png"));
                                        break;
                                    case "bridge":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/bridge.png"));
                                        break;
                                    case "cablestb":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cablestb.png"));
                                        break;
                                    case "cameradev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cameradev.png"));
                                        break;
                                    case "dvr":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/dvr.png"));
                                        break;
                                    case "gamedev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gamedev.png"));
                                        break;
                                    case "linuxpc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/linuxpc.png"));
                                        break;
                                    case "macminidev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macminidev.png"));
                                        break;
                                    case "macprodev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macprodev.png"));
                                        break;
                                    case "macbookdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macbookdev.png"));
                                        break;
                                    case "mediadev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mediadev.png"));
                                        break;
                                    case "networkdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/networkdev.png"));
                                        break;
                                    case "stb":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/stb.png"));
                                        break;
                                    case "printerdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/printerdev.png"));
                                        break;
                                    case "repeater":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/repeater.png"));
                                        break;
                                    case "gatewaydev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                                        break;
                                    case "satellitestb":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/satellitestb.png"));
                                        break;
                                    case "scannerdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/scannerdev.png"));
                                        break;
                                    case "slingbox":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/slingbox.png"));
                                        break;
                                    case "mobiledev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mobiledev.png"));
                                        break;
                                    case "netstoragedev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/netstoragedev.png"));
                                        break;
                                    case "switchdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/switchdev.png"));
                                        break;
                                    case "tv":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tv.png"));
                                        break;
                                    case "tablepc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tablepc.png"));
                                        break;
                                    case "unixpc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/unixpc.png"));
                                        break;
                                    case "windowspc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowspc.png"));
                                        break;
                                    case "windowsphone":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowsphone.png"));
                                        break;
                                    case "windowstablet":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowstablet.png"));
                                        break;
                                }
                                imgDevice.Stretch = Stretch.Uniform;
                                imgDevice.Height = 50;
                                TextBlock DeviceNameText = new TextBlock();
                                DeviceNameText.Text = Group.ElementAt(6 * i + j).NODE.deviceName;
                                DeviceNameText.FontSize = 15;
                                DeviceNameText.Margin = new Thickness(0, -5, 0, 0);
                                DeviceNameText.Foreground = new SolidColorBrush(Color.FromArgb(255, 90, 90, 90));
                                DeviceNameText.TextTrimming = TextTrimming.WordEllipsis;
                                StackPanel stpDevice = new StackPanel();
                                stpDevice.Children.Add(imgDevice);
                                stpDevice.Children.Add(DeviceNameText);
                                BtnDevice.Content = stpDevice;
                                BtnDevice.Background = new SolidColorBrush();
                                BtnDevice.Click += new RoutedEventHandler(DeviceButton_Click);
                                map.Children.Add(BtnDevice);
                            }	//if (j > 1)		
                        } //for (int j = 0; j < 8; j++)

                        if (NetworkMapInfo.IsAccessControlSupported)
                        {
                            CheckBox cbAccessControl = new CheckBox();
                            cbAccessControl.Name = "cbAccessControl_" + (i + 1).ToString();
                            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                            cbAccessControl.Content = loader.GetString("AccessControl");
                            cbAccessControl.FontSize = 20;
                            cbAccessControl.Foreground = new SolidColorBrush(Colors.Black);
                            if (NetworkMapInfo.IsAccessControlEnabled)
                            {
                                cbAccessControl.IsChecked = true;
                            }
                            else
                            {
                                cbAccessControl.IsChecked = false;
                            }
                            cbAccessControl.HorizontalAlignment = HorizontalAlignment.Right;
                            cbAccessControl.VerticalAlignment = VerticalAlignment.Bottom;
                            cbAccessControl.Margin = new Thickness(0, 0, 10, 10);
                            cbAccessControl.Click += new RoutedEventHandler(AccessControl_checked);
                            map.Children.Add(cbAccessControl);
                        }

                        map.Children.Add(internet);
                        map.Children.Add(BtnRouter);
                        map.Children.Add(BtnLocalDevice);
                        MapFlipView.Items.Add(map);
                    } //if (i != m)
                    else
                    {
                        double Angle = 360.0 / (n + 2);
                        Grid map = new Grid();

                        Image internet = new Image();
                        Uri _baseUri = new Uri("ms-appx:///");
                        internet.Source = new BitmapImage(new Uri(_baseUri, "Assets/internet72.png"));
                        internet.HorizontalAlignment = HorizontalAlignment.Right;
                        internet.Margin = new Thickness(0, 0, 50, 0);
                        internet.Width = 100; internet.Height = 100;


                        Button BtnRouter = new Button();
                        //BtnRouter.Name = "Router";
                        BtnRouter.Name = Group.ElementAt(0).NODE.uniqueId;
                        BtnRouter.SetValue(WidthProperty, 150);
                        BtnRouter.SetValue(HeightProperty, 150);
                        BtnRouter.HorizontalAlignment = HorizontalAlignment.Center;
                        BtnRouter.VerticalAlignment = VerticalAlignment.Center;
                        Image imgRouter = new Image();
                        imgRouter.Source = new BitmapImage(new Uri(_baseUri, routerImage));
                        imgRouter.Stretch = Stretch.Uniform;
                        BtnRouter.Content = imgRouter;
                        BtnRouter.Margin = new Thickness(0, 0, 0, 0);
                        BtnRouter.Background = new SolidColorBrush();
                        BtnRouter.Click += new RoutedEventHandler(DeviceButton_Click);

                        //本设备图标
                        Button BtnLocalDevice = new Button();
                        BtnLocalDevice.Name = Group.ElementAt(1).NODE.uniqueId;
                        BtnLocalDevice.Width = 100;
                        BtnLocalDevice.Height = 100;
                        BtnLocalDevice.HorizontalAlignment = HorizontalAlignment.Left;
                        BtnLocalDevice.VerticalAlignment = VerticalAlignment.Top;
                        double xLocal = r1 * Math.Cos(Angle * PI / 180);
                        double yLocal = r2 * Math.Sin(Angle * PI / 180);
                        BtnLocalDevice.Margin = new Thickness(width / 2 + xLocal - 50, height / 2 - yLocal - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
                        Image imgLocalDevice = new Image();
                        imgLocalDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowstablet.png"));
                        imgLocalDevice.Stretch = Stretch.Uniform;
                        imgLocalDevice.Height = 50;
                        TextBlock LocalDeviceNameText = new TextBlock();
                        LocalDeviceNameText.Text = Group.ElementAt(1).NODE.deviceName;
                        LocalDeviceNameText.FontSize = 15;
                        LocalDeviceNameText.Margin = new Thickness(0, -5, 0, 0);
                        LocalDeviceNameText.Foreground = new SolidColorBrush(Color.FromArgb(255, 90, 90, 90));
                        LocalDeviceNameText.TextTrimming = TextTrimming.WordEllipsis;
                        StackPanel stpLocalDevice = new StackPanel();
                        stpLocalDevice.Children.Add(imgLocalDevice);
                        stpLocalDevice.Children.Add(LocalDeviceNameText);
                        BtnLocalDevice.Content = stpLocalDevice;
                        BtnLocalDevice.Background = new SolidColorBrush();
                        BtnLocalDevice.Click += new RoutedEventHandler(DeviceButton_Click);

                        for (int j = 0; j < n + 2; j++)
                        {
                            double x = r1 * Math.Cos(j * Angle * PI / 180);
                            double y = r2 * Math.Sin(j * Angle * PI / 180);
                            Line line = new Line();
                            line.X1 = width / 2; line.Y1 = height / 2;
                            line.X2 = width / 2 + x; line.Y2 = height / 2 - y;
                            line.Stroke = new SolidColorBrush(Colors.SeaGreen);
                            line.StrokeThickness = 2;
                            if (j == 0)
                            {
                                //判断路由器是否已连接因特网
                                UtilityTool util = new UtilityTool();
                                bool isConnectToInternet = util.IsConnectedToInternet();
                                if (!isConnectToInternet)
                                {
                                    line.Stroke = new SolidColorBrush(Colors.Red);
                                }
                            }
                            Image imgSignal = new Image();
                            if (j == 1)
                            {
                                DoubleCollection dc = new DoubleCollection();
                                dc.Add(2);
                                line.StrokeDashArray = dc;
                                int signal;
                                if (NetworkMapInfo.attachDeviceDic.Count == 0)
                                {
                                    signal = 100;
                                }
                                else
                                {
                                    string[] strSignal = Group.ElementAt(1).NODE.signalStrength.Split('%');
                                    signal = int.Parse(strSignal[0]);
                                }
                                if (Group.ElementAt(1).NODE.AccessControl == "Allow")
                                {
                                    if (signal <= 20)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_13.png"));
                                    }
                                    else if (signal > 20 && signal <= 40)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_23.png"));
                                    }
                                    else if (signal > 40 && signal <= 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_33.png"));
                                    }
                                    else if (signal > 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_43.png"));
                                    }
                                }
                                else
                                {
                                    if (signal <= 20)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag1.png"));
                                    }
                                    else if (signal > 20 && signal <= 40)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag2.png"));
                                    }
                                    else if (signal > 40 && signal <= 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag3.png"));
                                    }
                                    else if (signal > 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag4.png"));
                                    }
                                }
                                imgSignal.Stretch = Stretch.Fill;
                                imgSignal.Width = 30; imgSignal.Height = 30;
                                imgSignal.HorizontalAlignment = HorizontalAlignment.Left;
                                imgSignal.VerticalAlignment = VerticalAlignment.Top;
                                imgSignal.Margin = new Thickness(width / 2 + x / 2 - 15, height / 2 - y / 2 - 15, 0, 0);
                            }
                            if (j > 1 && Group.ElementAt(6 * i + j).NODE.connectType == "wireless")
                            {
                                DoubleCollection dc = new DoubleCollection();
                                dc.Add(2);
                                line.StrokeDashArray = dc;
                                string[] signal = Group.ElementAt(6 * i + j).NODE.signalStrength.Split('%');
                                int result = int.Parse(signal[0]);
                                if (Group.ElementAt(6 * i + j).NODE.AccessControl == "Allow")
                                {
                                    if (result <= 20)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_13.png"));
                                    }
                                    else if (result > 20 && result <= 40)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_23.png"));
                                    }
                                    else if (result > 40 && result <= 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_33.png"));
                                    }
                                    else if (result > 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/signal_43.png"));
                                    }
                                }
                                else
                                {
                                    if (result <= 20)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag1.png"));
                                    }
                                    else if (result > 20 && result <= 40)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag2.png"));
                                    }
                                    else if (result > 40 && result <= 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag3.png"));
                                    }
                                    else if (result > 70)
                                    {
                                        imgSignal.Source = new BitmapImage(new Uri(_baseUri, "Assets/signal/wirelessflag4.png"));
                                    }
                                }
                                imgSignal.Stretch = Stretch.Fill;
                                imgSignal.Width = 30; imgSignal.Height = 30;
                                imgSignal.HorizontalAlignment = HorizontalAlignment.Left;
                                imgSignal.VerticalAlignment = VerticalAlignment.Top;
                                imgSignal.Margin = new Thickness(width / 2 + x / 2 - 15, height / 2 - y / 2 - 15, 0, 0);
                            }
                            map.Children.Add(line);
                            map.Children.Add(imgSignal);

                            if (j > 1)
                            {
                                Button BtnDevice = new Button();
                                BtnDevice.Name = Group.ElementAt(6 * i + j).NODE.uniqueId;
                                BtnDevice.SetValue(WidthProperty, 100);
                                BtnDevice.SetValue(HeightProperty, 100);
                                BtnDevice.HorizontalAlignment = HorizontalAlignment.Left;
                                BtnDevice.VerticalAlignment = VerticalAlignment.Top;
                                BtnDevice.Margin = new Thickness(width / 2 + x - 50, height / 2 - y - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
                                Image imgDevice = new Image();
                                switch (Group.ElementAt(6 * i + j).NODE.deviceType)
                                {
                                    case "imacdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/imacdev.png"));
                                        break;
                                    case "ipad":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipad.png"));
                                        break;
                                    case "ipadmini":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipadmini.png"));
                                        break;
                                    case "iphone":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone.png"));
                                        break;
                                    case "iphone5":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/iphone5.png"));
                                        break;
                                    case "ipodtouch":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/ipodtouch.png"));
                                        break;
                                    case "amazonkindle":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/amazonkindle.png"));
                                        break;
                                    case "androiddevice":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androiddevice.png"));
                                        break;
                                    case "androidphone":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidphone.png"));
                                        break;
                                    case "androidtablet":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/androidtablet.png"));
                                        break;
                                    case "blurayplayer":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/blurayplayer.png"));
                                        break;
                                    case "bridge":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/bridge.png"));
                                        break;
                                    case "cablestb":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cablestb.png"));
                                        break;
                                    case "cameradev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/cameradev.png"));
                                        break;
                                    case "dvr":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/dvr.png"));
                                        break;
                                    case "gamedev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gamedev.png"));
                                        break;
                                    case "linuxpc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/linuxpc.png"));
                                        break;
                                    case "macminidev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macminidev.png"));
                                        break;
                                    case "macprodev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macprodev.png"));
                                        break;
                                    case "macbookdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/macbookdev.png"));
                                        break;
                                    case "mediadev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mediadev.png"));
                                        break;
                                    case "networkdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/networkdev.png"));
                                        break;
                                    case "stb":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/stb.png"));
                                        break;
                                    case "printerdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/printerdev.png"));
                                        break;
                                    case "repeater":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/repeater.png"));
                                        break;
                                    case "gatewaydev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/gatewaydev.png"));
                                        break;
                                    case "satellitestb":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/satellitestb.png"));
                                        break;
                                    case "scannerdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/scannerdev.png"));
                                        break;
                                    case "slingbox":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/slingbox.png"));
                                        break;
                                    case "mobiledev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/mobiledev.png"));
                                        break;
                                    case "netstoragedev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/netstoragedev.png"));
                                        break;
                                    case "switchdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/switchdev.png"));
                                        break;
                                    case "tv":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tv.png"));
                                        break;
                                    case "tablepc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/tablepc.png"));
                                        break;
                                    case "unixpc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/unixpc.png"));
                                        break;
                                    case "windowspc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowspc.png"));
                                        break;
                                    case "windowsphone":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowsphone.png"));
                                        break;
                                    case "windowstablet":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/devices/windowstablet.png"));
                                        break;
                                }
                                imgDevice.Stretch = Stretch.Uniform;
                                imgDevice.Height = 50;
                                TextBlock DeviceNameText = new TextBlock();
                                DeviceNameText.Text = Group.ElementAt(6 * i + j).NODE.deviceName;
                                DeviceNameText.FontSize = 15;
                                DeviceNameText.Margin = new Thickness(0, -5, 0, 0);
                                DeviceNameText.Foreground = new SolidColorBrush(Color.FromArgb(255, 90, 90, 90));
                                DeviceNameText.TextTrimming = TextTrimming.WordEllipsis;
                                StackPanel stpDevice = new StackPanel();
                                stpDevice.Children.Add(imgDevice);
                                stpDevice.Children.Add(DeviceNameText);
                                BtnDevice.Content = stpDevice;
                                BtnDevice.Background = new SolidColorBrush();
                                BtnDevice.Click += new RoutedEventHandler(DeviceButton_Click);
                                map.Children.Add(BtnDevice);
                            }//if (j > 1)
                        }//for (int j = 0; j < n + 1; j++)

                        if (NetworkMapInfo.IsAccessControlSupported)
                        {
                            CheckBox cbAccessControl = new CheckBox();
                            cbAccessControl.Name = "cbAccessControl_" + (i + 1).ToString();
                            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                            cbAccessControl.Content = loader.GetString("AccessControl");
                            cbAccessControl.FontSize = 20;
                            cbAccessControl.Foreground = new SolidColorBrush(Colors.Black);
                            if (NetworkMapInfo.IsAccessControlEnabled)
                            {
                                cbAccessControl.IsChecked = true;
                            }
                            else
                            {
                                cbAccessControl.IsChecked = false;
                            }
                            cbAccessControl.HorizontalAlignment = HorizontalAlignment.Right;
                            cbAccessControl.VerticalAlignment = VerticalAlignment.Bottom;
                            cbAccessControl.Margin = new Thickness(0, 0, 10, 10);
                            cbAccessControl.Click += new RoutedEventHandler(AccessControl_checked);
                            map.Children.Add(cbAccessControl);
                        }

                        map.Children.Add(internet);
                        map.Children.Add(BtnRouter);
                        map.Children.Add(BtnLocalDevice);
                        MapFlipView.Items.Add(map);
                    }
                }
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
        }

        private void DeviceButton_Click(Object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            var UniqueId = btn.Name;
            this.Frame.Navigate(typeof(DeviceInfoPage), UniqueId);
        }

        private async void RefreshButton_Click(Object sender, RoutedEventArgs e)
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
                this.Frame.Navigate(typeof(NetworkMapPage));
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

        private async void AccessControl_checked(Object sender, RoutedEventArgs e)
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
                CheckBox cbAccessControl = (CheckBox)sender;
                if (cbAccessControl.IsChecked == true)
                {
                    dicResponse = await soapApi.SetBlockDeviceEnable("1");
                }
                else if (cbAccessControl.IsChecked == false)
                {
                    dicResponse = await soapApi.SetBlockDeviceEnable("0");
                }
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                this.Frame.Navigate(typeof(NetworkMapPage));
            }
        }
    }
}
