﻿using System;
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
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace GenieWP8
{
    public partial class NetworkMapPage : PhoneApplicationPage
    {
        private static NetworkMapModel settingModel = null;
        private string routerImage ;
        public NetworkMapPage()
        {
            InitializeComponent();
            ImageNameGenerator imagePath = new ImageNameGenerator(MainPageInfo.model);
            ///获取路由器图标路径
            routerImage = imagePath.getImagePath(); 

            // 将 LongListSelector 控件的数据上下文设置为绑定数据
            if (settingModel == null)
                settingModel = new NetworkMapModel();
            DataContext = settingModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();
        }

        // 为 NetworkMapModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (!settingModel.IsDataLoaded)
            //{
            //    settingModel.LoadData();
            //}
            if (NetworkMapInfo.bTypeChanged)
            {
                DeviceInfoPopup.IsOpen = true;
            }
            else
            {
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Visible;
                pleasewait.Visibility = Visibility.Visible;
                settingModel.DeviceGroups.Clear();
                settingModel.LoadData();
                double width;
                double height;
                if ((this.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
                {
                    width = Application.Current.Host.Content.ActualWidth;
                    height = Application.Current.Host.Content.ActualHeight - 250;
                    DeviceInfoScrollViewer.Height = 400;
                }
                // If not in portrait, move buttonList content to visible row and column.
                else
                {
                    width = Application.Current.Host.Content.ActualHeight - 150;
                    height = Application.Current.Host.Content.ActualWidth - 150;
                    DeviceInfoScrollViewer.Height = 250;
                }
                DrawNetworkMap(width, height);
                PopupBackground.Visibility = Visibility.Collapsed;
            }         
        }

        private void PhoneApplicationPage_OrientationChanged(Object sender, OrientationChangedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            double width;
            double height;
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                width = Application.Current.Host.Content.ActualWidth;
                height = Application.Current.Host.Content.ActualHeight - 250;
                DeviceInfoScrollViewer.Height = 400;
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                width = Application.Current.Host.Content.ActualHeight - 150;
                height = Application.Current.Host.Content.ActualWidth - 150;
                DeviceInfoScrollViewer.Height = 250;
            }
            DrawNetworkMap(width, height);
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private async void DrawNetworkMap(double width, double height)
        {         
            MapPivot.Items.Clear();
            double PI = 3.141592653589793;
            //double width = e.NewSize.Width;
            //double height = e.NewSize.Height - 150;
            double r1 = width / 2 - 50;
            double r2 = height / 2 - 50;
            //m = （总设备数 - 1（路由器/交换机））/ 6 的商
            //n 为上式得余数，即最后一页除路由器（交换机）和本设备外的设备数
            var Group = settingModel.DeviceGroups;
            var DeviceNumber = Group.Count();
            int m = (DeviceNumber - 1) / 7;
            int n = (DeviceNumber - 1) % 7;
            for (int i = 0; i < m + 1; i++)
            {
                if (i != m)
                {
                    Grid map = new Grid();

                    Image internet = new Image();
                    internet.Source = new BitmapImage(new Uri("Assets/internet72.png", UriKind.Relative));
                    internet.HorizontalAlignment = HorizontalAlignment.Right;
                    internet.Margin = new Thickness(0, 0, 0, 0);
                    internet.Width = 75; internet.Height = 75;


                    Button BtnRouter = new Button();
                    BtnRouter.Name = Group.ElementAt(0).NODE.uniqueId + "_" + (i + 1).ToString();
                    BtnRouter.Width = 100;
                    BtnRouter.Height = 100;
                    BtnRouter.HorizontalAlignment = HorizontalAlignment.Center;
                    BtnRouter.VerticalAlignment = VerticalAlignment.Center;

                    Image imgRouter = new Image();
                    imgRouter.Source = new BitmapImage(new Uri(routerImage, UriKind.Relative));
                    imgRouter.Stretch = Stretch.UniformToFill;
                    BtnRouter.Content = imgRouter;
                    BtnRouter.Margin = new Thickness(20, 15, 0, 0);
                    BtnRouter.Click += new RoutedEventHandler(RouterButton_Click);

                    double Angle = 360.0 / 8;
                    for (int j = 0; j < 8; j++)
                    {
                        double x = r1 * Math.Cos(j * Angle * PI / 180);
                        double y = r2 * Math.Sin(j * Angle * PI / 180);
                        Line line = new Line();
                        line.X1 = width / 2; line.Y1 = height / 2;
                        line.X2 = width / 2 + x; line.Y2 = height / 2 - y;
                        line.Stroke = new SolidColorBrush(Colors.Green);
                        line.StrokeThickness = 2;
                        if (j == 0)
                        {
                            //判断路由器是否已连接因特网
                            GenieSoapApi soapApi = new GenieSoapApi();
                            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                            dicResponse = await soapApi.GetCurrentSetting();
                            if (dicResponse.Count > 0)
                            {
                                if (dicResponse["InternetConnectionStatus"] != "Up")
                                {
                                    line.Stroke = new SolidColorBrush(Colors.Red);
                                }
                            }
                        }
                        Image imgSignal = new Image();
                        if (j > 0 && Group.ElementAt(7 * i + j).NODE.connectType == "wireless")
                        {
                            DoubleCollection dc = new DoubleCollection();
                            dc.Add(2);
                            line.StrokeDashArray = dc;
                            string[] signal = Group.ElementAt(7 * i + j).NODE.signalStrength.Split('%');
                            int result = int.Parse(signal[0]);
                            if (result <= 20)
                            {
                                imgSignal.Source = new BitmapImage(new Uri("Assets/signal/signal_13.png", UriKind.Relative));
                            }
                            else if (result > 20 && result <= 40)
                            {
                                imgSignal.Source = new BitmapImage(new Uri("Assets/signal/signal_23.png", UriKind.Relative));
                            }
                            else if (result > 40 && result <= 70)
                            {
                                imgSignal.Source = new BitmapImage(new Uri("Assets/signal/signal_33.png", UriKind.Relative));
                            }
                            else if (result > 70)
                            {
                                imgSignal.Source = new BitmapImage(new Uri("Assets/signal/signal_43.png", UriKind.Relative));
                            }
                            imgSignal.Stretch = Stretch.Fill;
                            imgSignal.Width = 30; imgSignal.Height = 30;
                            imgSignal.HorizontalAlignment = HorizontalAlignment.Left;
                            imgSignal.VerticalAlignment = VerticalAlignment.Top;
                            imgSignal.Margin = new Thickness(width / 2 + x / 2 - 15, height / 2 - y / 2 - 15, 0, 0);
                        }
                        map.Children.Add(line);
                        map.Children.Add(imgSignal);

                        if (j > 0)
                        {
                            Button BtnDevice = new Button();
                            BtnDevice.Name = Group.ElementAt(7 * i + j).NODE.uniqueId;
                            BtnDevice.Width = 100;
                            BtnDevice.Height = 100;
                            BtnDevice.HorizontalAlignment = HorizontalAlignment.Left;
                            BtnDevice.VerticalAlignment = VerticalAlignment.Top;
                            BtnDevice.Margin = new Thickness(width / 2 + x - 50, height / 2 - y - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
                            Image imgDevice = new Image();
                            switch (Group.ElementAt(7 * i + j).NODE.deviceType)
                            {
                                case "gatewaydev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/gatewaydev.png", UriKind.Relative));
                                    break;
                                case "networkdev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/networkdev.png", UriKind.Relative));
                                    break;
                                case "windowspc":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/windowspc.png", UriKind.Relative));
                                    break;
                                case "blurayplayer":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/blurayplayer.png", UriKind.Relative));
                                    break;
                                case "bridge":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/bridge.png", UriKind.Relative));
                                    break;
                                case "cablestb":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/cablestb.png", UriKind.Relative));
                                    break;
                                case "cameradev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/cameradev.png", UriKind.Relative));
                                    break;
                                case "gamedev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/gamedev.png", UriKind.Relative));
                                    break;
                                case "imacdev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/imacdev.png", UriKind.Relative));
                                    break;
                                case "ipad":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/ipad.png", UriKind.Relative));
                                    break;
                                case "iphone":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/iphone.png", UriKind.Relative));
                                    break;
                                case "ipodtouch":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/ipodtouch.png", UriKind.Relative));
                                    break;
                                case "linuxpc":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/linuxpc.png", UriKind.Relative));
                                    break;
                                case "macbookdev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/macbookdev.png", UriKind.Relative));
                                    break;
                                case "macminidev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/macminidev.png", UriKind.Relative));
                                    break;
                                case "macprodev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/macprodev.png", UriKind.Relative));
                                    break;
                                case "mediadev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/mediadev.png", UriKind.Relative));
                                    break;
                                case "mobiledev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/mobiledev.png", UriKind.Relative));
                                    break;
                                case "netstoragedev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/netstoragedev.png", UriKind.Relative));
                                    break;
                                case "switchdev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/switchdev.png", UriKind.Relative));
                                    break;
                                case "printerdev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/printerdev.png", UriKind.Relative));
                                    break;
                                case "repeater":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/repeater.png", UriKind.Relative));
                                    break;
                                case "satellitestb":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/satellitestb.png", UriKind.Relative));
                                    break;
                                case "scannerdev":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/scannerdev.png", UriKind.Relative));
                                    break;
                                case "slingbox":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/slingbox.png", UriKind.Relative));
                                    break;
                                case "stb":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/stb.png", UriKind.Relative));
                                    break;
                                case "tablepc":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/tablepc.png", UriKind.Relative));
                                    break;
                                case "tv":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/tv.png", UriKind.Relative));
                                    break;
                                case "unixpc":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/unixpc.png", UriKind.Relative));
                                    break;
                                case "androiddevice":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/androiddevice.png", UriKind.Relative));
                                    break;
                                case "androidphone":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                                    break;
                                case "androidtablet":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/androidtablet.png", UriKind.Relative));
                                    break;
                                case "dvr":
                                    imgDevice.Source = new BitmapImage(new Uri("Assets/devices/dvr.png", UriKind.Relative));
                                    break;
                            }
                            imgDevice.Stretch = Stretch.UniformToFill;
                            TextBlock DeviceNameText = new TextBlock();
                            DeviceNameText.Text = Group.ElementAt(7 * i + j).NODE.deviceName;
                            DeviceNameText.FontSize = 15;
                            DeviceNameText.FontWeight = FontWeights.Normal;
                            DeviceNameText.Margin = new Thickness(0, -10, 0, 0);
                            StackPanel stpDevice = new StackPanel();
                            stpDevice.Children.Add(imgDevice);
                            stpDevice.Children.Add(DeviceNameText);
                            BtnDevice.Content = stpDevice;
                            BtnDevice.Click += new RoutedEventHandler(DeviceButton_Click);
                            map.Children.Add(BtnDevice);
                        }//if (j > 0)
                    }//for (int j = 0; j < 8; j++)

                    map.Children.Add(internet);
                    map.Children.Add(BtnRouter);
                    PivotItem map_PivotItem = new PivotItem();
                    map_PivotItem.Header = "MAP_" + (i + 1).ToString();
                    map_PivotItem.Content = map;
                    MapPivot.Items.Add(map_PivotItem);
                }//if (i != m)
                else
                {
                    if (n != 0)
                    {
                        Grid map = new Grid();

                        Image internet = new Image();
                        internet.Source = new BitmapImage(new Uri("Assets/internet72.png", UriKind.Relative));
                        internet.HorizontalAlignment = HorizontalAlignment.Right;
                        internet.Margin = new Thickness(0, 0, 0, 0);
                        internet.Width = 75; internet.Height = 75;


                        Button BtnRouter = new Button();
                        BtnRouter.Name = Group.ElementAt(0).NODE.uniqueId + "_" + (i + 1).ToString();
                        BtnRouter.Width = 100;
                        BtnRouter.Height = 100;
                        BtnRouter.HorizontalAlignment = HorizontalAlignment.Center;
                        BtnRouter.VerticalAlignment = VerticalAlignment.Center;
                        Image imgRouter = new Image();
                        imgRouter.Source = new BitmapImage(new Uri(routerImage, UriKind.Relative));
                        imgRouter.Stretch = Stretch.UniformToFill;
                        BtnRouter.Content = imgRouter;
                        BtnRouter.Margin = new Thickness(20, 15, 0, 0);
                        BtnRouter.Click += new RoutedEventHandler(RouterButton_Click);

                        double Angle = 360.0 / (n + 1);
                        for (int j = 0; j < n + 1; j++)
                        {
                            double x = r1 * Math.Cos(j * Angle * PI / 180);
                            double y = r2 * Math.Sin(j * Angle * PI / 180);
                            Line line = new Line();
                            line.X1 = width / 2; line.Y1 = height / 2;
                            line.X2 = width / 2 + x; line.Y2 = height / 2 - y;
                            line.Stroke = new SolidColorBrush(Colors.Green);
                            line.StrokeThickness = 2;
                            if (j == 0)
                            {
                                //判断路由器是否已连接因特网
                                GenieSoapApi soapApi = new GenieSoapApi();
                                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                                dicResponse = await soapApi.GetCurrentSetting();
                                if (dicResponse.Count > 0)
                                {
                                    if (dicResponse["InternetConnectionStatus"] != "Up")
                                    {
                                        line.Stroke = new SolidColorBrush(Colors.Red);
                                    }
                                }
                            }
                            Image imgSignal = new Image();
                            if (j > 0 && Group.ElementAt(7 * i + j).NODE.connectType == "wireless")
                            {
                                DoubleCollection dc = new DoubleCollection();
                                dc.Add(2);
                                line.StrokeDashArray = dc;
                                string[] signal = Group.ElementAt(7 * i + j).NODE.signalStrength.Split('%');
                                int result = int.Parse(signal[0]);
                                if (result <= 20)
                                {
                                    imgSignal.Source = new BitmapImage(new Uri("Assets/signal/signal_13.png", UriKind.Relative));
                                }
                                else if (result > 20 && result <= 40)
                                {
                                    imgSignal.Source = new BitmapImage(new Uri("Assets/signal/signal_23.png", UriKind.Relative));
                                }
                                else if (result > 40 && result <= 70)
                                {
                                    imgSignal.Source = new BitmapImage(new Uri("Assets/signal/signal_33.png", UriKind.Relative));
                                }
                                else if (result > 70)
                                {
                                    imgSignal.Source = new BitmapImage(new Uri("Assets/signal/signal_43.png", UriKind.Relative));
                                }
                                imgSignal.Stretch = Stretch.Fill;
                                imgSignal.Width = 30; imgSignal.Height = 30;
                                imgSignal.HorizontalAlignment = HorizontalAlignment.Left;
                                imgSignal.VerticalAlignment = VerticalAlignment.Top;
                                imgSignal.Margin = new Thickness(width / 2 + x / 2 - 15, height / 2 - y / 2 - 15, 0, 0);
                            }
                            map.Children.Add(line);
                            map.Children.Add(imgSignal);

                            if (j > 0)
                            {
                                Button BtnDevice = new Button();
                                BtnDevice.Name = Group.ElementAt(7 * i + j).NODE.uniqueId;
                                BtnDevice.Width = 100;
                                BtnDevice.Height = 100;
                                BtnDevice.HorizontalAlignment = HorizontalAlignment.Left;
                                BtnDevice.VerticalAlignment = VerticalAlignment.Top;
                                BtnDevice.Margin = new Thickness(width / 2 + x - 50, height / 2 - y - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
                                Image imgDevice = new Image();
                                switch (Group.ElementAt(7 * i + j).NODE.deviceType)
                                {
                                    case "gatewaydev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/gatewaydev.png", UriKind.Relative));
                                        break;
                                    case "networkdev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/networkdev.png", UriKind.Relative));
                                        break;
                                    case "windowspc":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/windowspc.png", UriKind.Relative));
                                        break;
                                    case "blurayplayer":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/blurayplayer.png", UriKind.Relative));
                                        break;
                                    case "bridge":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/bridge.png", UriKind.Relative));
                                        break;
                                    case "cablestb":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/cablestb.png", UriKind.Relative));
                                        break;
                                    case "cameradev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/cameradev.png", UriKind.Relative));
                                        break;
                                    case "gamedev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/gamedev.png", UriKind.Relative));
                                        break;
                                    case "imacdev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/imacdev.png", UriKind.Relative));
                                        break;
                                    case "ipad":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/ipad.png", UriKind.Relative));
                                        break;
                                    case "iphone":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/iphone.png", UriKind.Relative));
                                        break;
                                    case "ipodtouch":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/ipodtouch.png", UriKind.Relative));
                                        break;
                                    case "linuxpc":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/linuxpc.png", UriKind.Relative));
                                        break;
                                    case "macbookdev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/macbookdev.png", UriKind.Relative));
                                        break;
                                    case "macminidev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/macminidev.png", UriKind.Relative));
                                        break;
                                    case "macprodev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/macprodev.png", UriKind.Relative));
                                        break;
                                    case "mediadev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/mediadev.png", UriKind.Relative));
                                        break;
                                    case "mobiledev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/mobiledev.png", UriKind.Relative));
                                        break;
                                    case "netstoragedev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/netstoragedev.png", UriKind.Relative));
                                        break;
                                    case "switchdev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/switchdev.png", UriKind.Relative));
                                        break;
                                    case "printerdev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/printerdev.png", UriKind.Relative));
                                        break;
                                    case "repeater":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/repeater.png", UriKind.Relative));
                                        break;
                                    case "satellitestb":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/satellitestb.png", UriKind.Relative));
                                        break;
                                    case "scannerdev":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/scannerdev.png", UriKind.Relative));
                                        break;
                                    case "slingbox":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/slingbox.png", UriKind.Relative));
                                        break;
                                    case "stb":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/stb.png", UriKind.Relative));
                                        break;
                                    case "tablepc":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/tablepc.png", UriKind.Relative));
                                        break;
                                    case "tv":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/tv.png", UriKind.Relative));
                                        break;
                                    case "unixpc":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/unixpc.png", UriKind.Relative));
                                        break;
                                    case "androiddevice":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/androiddevice.png", UriKind.Relative));
                                        break;
                                    case "androidphone":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                                        break;
                                    case "androidtablet":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/androidtablet.png", UriKind.Relative));
                                        break;
                                    case "dvr":
                                        imgDevice.Source = new BitmapImage(new Uri("Assets/devices/dvr.png", UriKind.Relative));
                                        break;
                                }
                                imgDevice.Stretch = Stretch.UniformToFill;
                                TextBlock DeviceNameText = new TextBlock();
                                DeviceNameText.Text = Group.ElementAt(7 * i + j).NODE.deviceName;
                                DeviceNameText.FontSize = 15;
                                DeviceNameText.FontWeight = FontWeights.Normal;
                                DeviceNameText.Margin = new Thickness(0, -10, 0, 0);
                                StackPanel stpDevice = new StackPanel();
                                stpDevice.Children.Add(imgDevice);
                                stpDevice.Children.Add(DeviceNameText);
                                BtnDevice.Content = stpDevice;
                                BtnDevice.Click += new RoutedEventHandler(DeviceButton_Click);
                                map.Children.Add(BtnDevice);
                            }//if (j > 0)
                        }//for (int j = 0; j < n + 1; j++)
                        map.Children.Add(internet);
                        map.Children.Add(BtnRouter);
                        PivotItem map_PivotItem = new PivotItem();
                        map_PivotItem.Header = "MAP_" + (i + 1).ToString();
                        map_PivotItem.Content = map;
                        MapPivot.Items.Add(map_PivotItem);
                    }//if (n != 0)
                }
            }
        }

        private void RouterButton_Click(Object sender, RoutedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Collapsed;
            pleasewait.Visibility = Visibility.Collapsed;
            DeviceInfoPopup.IsOpen = true;
            Button btn = (Button)sender;
            string[] UniqueId = btn.Name.Split('_');
            var Group = NetworkMapModel.GetGroup(UniqueId[0]);

            StpTitle.Children.Clear();

            TitleImage.HorizontalAlignment = HorizontalAlignment.Left;
            TitleImage.VerticalAlignment = VerticalAlignment.Center;
            TitleImage.Margin = new Thickness(10, 0, 0, 0);
            TitleImage.Width = 75; TitleImage.Height = 75;

            Title.Text = Group.NODE.deviceName;
            Title.FontSize = 30;
            Title.VerticalAlignment = VerticalAlignment.Center;
            Title.Margin = new Thickness(10, 0, 0, 0);
            Title.FontWeight = FontWeights.Bold;
            Title.TextWrapping = TextWrapping.Wrap;

            //IPAddress
            txtIPAddress.Text = AppResources.txtIPAddress;
            txtIPAddress.FontSize = 22;
            txtIPAddress.Margin = new Thickness(10, 10, 0, 0);
            txtIPAddress.HorizontalAlignment = HorizontalAlignment.Left;
            IPAddress.Text = Group.NODE.IPaddress;
            IPAddress.FontSize = 26;
            IPAddress.HorizontalAlignment = HorizontalAlignment.Center;            

            //MACAddress
            txtMACAddress.Text = AppResources.txtMACAddress;
            txtMACAddress.FontSize = 22;
            txtMACAddress.Margin = new Thickness(10, 0, 0, 0);
            txtMACAddress.HorizontalAlignment = HorizontalAlignment.Left;
            MACAddress.Text = Group.NODE.MACaddress;
            MACAddress.FontSize = 26;
            MACAddress.HorizontalAlignment = HorizontalAlignment.Center;

            //Buttons
            btnBack.Content = AppResources.btnBack;
            btnBack.FontSize = 24;
            btnBack.Click += new RoutedEventHandler(BackButton_Click);

            if (Group.NODE.uniqueId == "Router")
            {
                TitleImage.Source = new BitmapImage(new Uri(routerImage, UriKind.Relative));
                StpTitle.Children.Add(TitleImage);
                StpTitle.Children.Add(Title);

                txtRoutename.Text = AppResources.txtRoutename;
                txtRoutename.FontSize = 22;
                txtRoutename.Margin = new Thickness(10, 10, 0, 0);
                txtRoutename.HorizontalAlignment = HorizontalAlignment.Left;
                RouteName.Text = Group.NODE.deviceName;
                RouteName.FontSize = 26;
                RouteName.HorizontalAlignment = HorizontalAlignment.Center;

                btnBack.Margin = new Thickness(0, 10, 0, 0);

                StpRouter.Visibility = Visibility.Visible;
                StpDeviceName.Visibility = Visibility.Collapsed;
                StpType.Visibility = Visibility.Collapsed;
                StpLinkRate.Visibility = Visibility.Collapsed;
                StpSignalStrength.Visibility = Visibility.Collapsed;
                btnBack.Visibility = Visibility.Visible;
                btnFileUpload.Visibility = Visibility.Collapsed;
                btnModify.Visibility = Visibility.Collapsed;
                btnApply.Visibility = Visibility.Collapsed;

                NetworkMapInfo.bRefreshMap = false;
            }
            //this.Frame.Navigate(typeof(DeviceInfoPage), UniqueId);
        }
       
        private void DeviceButton_Click(Object sender, RoutedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Collapsed;
            pleasewait.Visibility = Visibility.Collapsed;
            DeviceInfoPopup.IsOpen = true;
            Button btn = (Button)sender;
            var UniqueId = btn.Name;
            var Group = NetworkMapModel.GetGroup(UniqueId);

            StpTitle.Children.Clear();

            TitleImage.HorizontalAlignment = HorizontalAlignment.Left;
            TitleImage.VerticalAlignment = VerticalAlignment.Center;
            TitleImage.Margin = new Thickness(10, 0, 0, 0);
            TitleImage.Width = 75; TitleImage.Height = 75;

            Title.Text = Group.NODE.deviceName;
            Title.FontSize = 30;
            Title.VerticalAlignment = VerticalAlignment.Center;
            Title.Margin = new Thickness(10, 0, 0, 0);
            Title.FontWeight = FontWeights.Bold;
            Title.TextWrapping = TextWrapping.Wrap;

            //DeviceName
            txtDeviceName.Text = AppResources.txtDeviceName;
            txtDeviceName.FontSize = 22;
            txtDeviceName.Margin = new Thickness(10, 10, 0, 0);
            txtDeviceName.HorizontalAlignment = HorizontalAlignment.Left;
            txtBlockDeviceName.Text = Group.NODE.deviceName;
            txtBlockDeviceName.FontSize = 26;
            txtBlockDeviceName.HorizontalAlignment = HorizontalAlignment.Center;
            txtBlockDeviceName.Visibility = Visibility.Visible;
            txtBoxDeviceName.Text = Group.NODE.deviceName;
            txtBoxDeviceName.FontSize = 26;
            txtBoxDeviceName.TextAlignment = TextAlignment.Center;
            txtBoxDeviceName.Visibility = Visibility.Collapsed;

            //Type
            //StpType.Children.Remove(lpType);
            if (Group.NODE.deviceType != null)
            {
                txtType.Text = AppResources.txtType;
                txtType.FontSize = 22;
                txtType.Margin = new Thickness(10, 10, 0, 0);
                txtType.HorizontalAlignment = HorizontalAlignment.Left;
                switch (Group.NODE.deviceType)
                {
                    case "gatewaydev":
                        Type.Text = AppResources.gatewaydev;
                        break;
                    case "networkdev":
                        Type.Text = AppResources.networkdev;
                        break;
                    case "windowspc":
                        Type.Text = AppResources.windowspc;
                        break;
                    case "blurayplayer":
                        Type.Text = AppResources.blurayplayer;
                        break;
                    case "bridge":
                        Type.Text = AppResources.bridge;
                        break;
                    case "cablestb":
                        Type.Text = AppResources.cablestb;
                        break;
                    case "cameradev":
                        Type.Text = AppResources.cameradev;
                        break;
                    case "gamedev":
                        Type.Text = AppResources.gamedev;
                        break;
                    //case "amazonkindledev":
                    //    Type.Text = AppResources.amazonkindledev;
                    //    break;
                    //case "ipadmini":
                    //    Type.Text = AppResources.ipadmini;
                    //    break;
                    //case "iphone5":
                    //    Type.Text = AppResources.iphone5;
                    //    break;
                    case "imacdev":
                        Type.Text = AppResources.imacdev;
                        break;
                    case "ipad":
                        Type.Text = AppResources.ipad;
                        break;
                    case "iphone":
                        Type.Text = AppResources.iphone;
                        break;
                    case "ipodtouch":
                        Type.Text = AppResources.ipodtouch;
                        break;
                    case "linuxpc":
                        Type.Text = AppResources.linuxpc;
                        break;
                    case "macbookdev":
                        Type.Text = AppResources.macbookdev;
                        break;
                    case "macminidev":
                        Type.Text = AppResources.macminidev;
                        break;
                    case "macprodev":
                        Type.Text = AppResources.macprodev;
                        break;
                    case "mediadev":
                        Type.Text = AppResources.mediadev;
                        break;
                    case "mobiledev":
                        Type.Text = AppResources.mobiledev;
                        break;
                    case "netstoragedev":
                        Type.Text = AppResources.netstoragedev;
                        break;
                    case "switchdev":
                        Type.Text = AppResources.switchdev;
                        break;
                    case "printerdev":
                        Type.Text = AppResources.printerdev;
                        break;
                    case "repeater":
                        Type.Text = AppResources.repeater;
                        break;
                    case "satellitestb":
                        Type.Text = AppResources.satellitestb;
                        break;
                    case "scannerdev":
                        Type.Text = AppResources.scannerdev;
                        break;
                    case "slingbox":
                        Type.Text = AppResources.slingbox;
                        break;
                    case "stb":
                        Type.Text = AppResources.stb;
                        break;
                    case "tablepc":
                        Type.Text = AppResources.tablepc;
                        break;
                    case "tv":
                        Type.Text = AppResources.tv;
                        break;
                    case "unixpc":
                        Type.Text = AppResources.unixpc;
                        break;
                    case "androiddevice":
                        Type.Text = AppResources.androiddevice;
                        break;
                    case "androidphone":
                        Type.Text = AppResources.androidphone;
                        break;
                    case "androidtablet":
                        Type.Text = AppResources.androidtablet;
                        break;
                    case "dvr":
                        Type.Text = AppResources.dvr;
                        break;
                }
                Type.FontSize = 26;
                Type.HorizontalAlignment = HorizontalAlignment.Center;
                Type.Visibility = Visibility.Visible;

                List<Devicetype> source = new List<Devicetype>();
                source.Add(new Devicetype() { type = AppResources.gatewaydev, deviceIcon = "Assets/devices/gatewaydev.png" });
                source.Add(new Devicetype() { type = AppResources.networkdev, deviceIcon = "Assets/devices/networkdev.png" });
                source.Add(new Devicetype() { type = AppResources.windowspc, deviceIcon = "Assets/devices/windowspc.png" });
                source.Add(new Devicetype() { type = AppResources.blurayplayer, deviceIcon = "Assets/devices/blurayplayer.png" });
                source.Add(new Devicetype() { type = AppResources.bridge, deviceIcon = "Assets/devices/bridge.png" });
                source.Add(new Devicetype() { type = AppResources.cablestb, deviceIcon = "Assets/devices/cablestb.png" });
                source.Add(new Devicetype() { type = AppResources.cameradev, deviceIcon = "Assets/devices/cameradev.png" });
                source.Add(new Devicetype() { type = AppResources.gamedev, deviceIcon = "Assets/devices/gamedev.png" });
                source.Add(new Devicetype() { type = AppResources.imacdev, deviceIcon = "Assets/devices/imacdev.png" });
                source.Add(new Devicetype() { type = AppResources.ipad, deviceIcon = "Assets/devices/ipad.png" });
                source.Add(new Devicetype() { type = AppResources.iphone, deviceIcon = "Assets/devices/iphone.png" });
                source.Add(new Devicetype() { type = AppResources.ipodtouch, deviceIcon = "Assets/devices/ipodtouch.png" });
                source.Add(new Devicetype() { type = AppResources.linuxpc, deviceIcon = "Assets/devices/linuxpc.png" });
                source.Add(new Devicetype() { type = AppResources.macbookdev, deviceIcon = "Assets/devices/macbookdev.png" });
                source.Add(new Devicetype() { type = AppResources.macminidev, deviceIcon = "Assets/devices/macminidev.png" });
                source.Add(new Devicetype() { type = AppResources.macprodev, deviceIcon = "Assets/devices/macprodev.png" });
                source.Add(new Devicetype() { type = AppResources.mediadev, deviceIcon = "Assets/devices/mediadev.png" });
                source.Add(new Devicetype() { type = AppResources.mobiledev, deviceIcon = "Assets/devices/mobiledev.png" });
                source.Add(new Devicetype() { type = AppResources.netstoragedev, deviceIcon = "Assets/devices/netstoragedev.png" });
                source.Add(new Devicetype() { type = AppResources.switchdev, deviceIcon = "Assets/devices/switchdev.png" });
                source.Add(new Devicetype() { type = AppResources.printerdev, deviceIcon = "Assets/devices/printerdev.png" });
                source.Add(new Devicetype() { type = AppResources.repeater, deviceIcon = "Assets/devices/repeater.png" });
                source.Add(new Devicetype() { type = AppResources.satellitestb, deviceIcon = "Assets/devices/satellitestb.png" });
                source.Add(new Devicetype() { type = AppResources.scannerdev, deviceIcon = "Assets/devices/scannerdev.png" });
                source.Add(new Devicetype() { type = AppResources.slingbox, deviceIcon = "Assets/devices/slingbox.png" });
                source.Add(new Devicetype() { type = AppResources.stb, deviceIcon = "Assets/devices/stb.png" });
                source.Add(new Devicetype() { type = AppResources.tablepc, deviceIcon = "Assets/devices/tablepc.png" });
                source.Add(new Devicetype() { type = AppResources.tv, deviceIcon = "Assets/devices/tv.png" });
                source.Add(new Devicetype() { type = AppResources.unixpc, deviceIcon = "Assets/devices/unixpc.png" });
                source.Add(new Devicetype() { type = AppResources.androiddevice, deviceIcon = "Assets/devices/androiddevice.png" });
                source.Add(new Devicetype() { type = AppResources.androidphone, deviceIcon = "Assets/devices/androidphone.png" });
                source.Add(new Devicetype() { type = AppResources.androidtablet, deviceIcon = "Assets/devices/androidtablet.png" });
                source.Add(new Devicetype() { type = AppResources.dvr, deviceIcon = "Assets/devices/dvr.png" });
                lpType.ItemsSource = source;

                switch (Group.NODE.deviceType)
                {
                    case "gatewaydev":
                        lpType.SelectedIndex = 0;
                        break;
                    case "networkdev":
                        lpType.SelectedIndex = 1;
                        break;
                    case "windowspc":
                        lpType.SelectedIndex = 2;
                        break;
                    case "blurayplayer":
                        lpType.SelectedIndex = 3;
                        break;
                    case "bridge":
                        lpType.SelectedIndex = 4;
                        break;
                    case "cablestb":
                        lpType.SelectedIndex = 5;
                        break;
                    case "cameradev":
                        lpType.SelectedIndex = 6;
                        break;
                    case "gamedev":
                        lpType.SelectedIndex = 7;
                        break;
                    //case "amazonkindledev":
                    //    lpType.SelectedIndex = 8;
                    //    break;
                    //case "ipadmini":
                    //    lpType.SelectedIndex = 9;
                    //    break;
                    //case "iphone5":
                    //    lpType.SelectedIndex = 10;
                    //    break;
                    case "imacdev":
                        lpType.SelectedIndex = 8;
                        break;
                    case "ipad":
                        lpType.SelectedIndex = 9;
                        break;
                    case "iphone":
                        lpType.SelectedIndex = 10;
                        break;
                    case "ipodtouch":
                        lpType.SelectedIndex = 11;
                        break;
                    case "linuxpc":
                        lpType.SelectedIndex = 12;
                        break;
                    case "macbookdev":
                        lpType.SelectedIndex = 13;
                        break;
                    case "macminidev":
                        lpType.SelectedIndex = 14;
                        break;
                    case "macprodev":
                        lpType.SelectedIndex = 15;
                        break;
                    case "mediadev":
                        lpType.SelectedIndex = 16;
                        break;
                    case "mobiledev":
                        lpType.SelectedIndex = 17;
                        break;
                    case "netstoragedev":
                        lpType.SelectedIndex = 18;
                        break;
                    case "switchdev":
                        lpType.SelectedIndex = 19;
                        break;
                    case "printerdev":
                        lpType.SelectedIndex = 20;
                        break;
                    case "repeater":
                        lpType.SelectedIndex = 21;
                        break;
                    case "satellitestb":
                        lpType.SelectedIndex = 22;
                        break;
                    case "scannerdev":
                        lpType.SelectedIndex = 23;
                        break;
                    case "slingbox":
                        lpType.SelectedIndex = 24;
                        break;
                    case "stb":
                        lpType.SelectedIndex = 25;
                        break;
                    case "tablepc":
                        lpType.SelectedIndex = 26;
                        break;
                    case "tv":
                        lpType.SelectedIndex = 27;
                        break;
                    case "unixpc":
                        lpType.SelectedIndex = 28;
                        break;
                    case "androiddevice":
                        lpType.SelectedIndex = 29;
                        break;
                    case "androidphone":
                        lpType.SelectedIndex = 30;
                        break;
                    case "androidtablet":
                        lpType.SelectedIndex = 31;
                        break;
                    case "dvr":
                        lpType.SelectedIndex = 32;
                        break;
                }
                lpType.HorizontalContentAlignment = HorizontalAlignment.Center;
                lpType.Visibility = Visibility.Collapsed;
                //StpType.Children.Add(lpType);

                //StpType.Children.Add(txtType);
                //StpType.Children.Add(Type);
            }

            //IPAddress
            txtIPAddress.Text = AppResources.txtIPAddress;
            txtIPAddress.FontSize = 22;
            txtIPAddress.Margin = new Thickness(10, 10, 0, 0);
            txtIPAddress.HorizontalAlignment = HorizontalAlignment.Left;
            IPAddress.Text = Group.NODE.IPaddress;
            IPAddress.FontSize = 26;
            IPAddress.HorizontalAlignment = HorizontalAlignment.Center;

            //SignalStrength
            if (Group.NODE.signalStrength != null)
            {
                txtSignalStrength.Text = AppResources.txtSignalStrength;
                txtSignalStrength.FontSize = 22;
                txtSignalStrength.Margin = new Thickness(10, 10, 0, 0);
                txtSignalStrength.HorizontalAlignment = HorizontalAlignment.Left;
                SignalStrength.Text = Group.NODE.signalStrength;
                SignalStrength.FontSize = 26;
                SignalStrength.HorizontalAlignment = HorizontalAlignment.Center;
            }

            //LinkRate
            if (Group.NODE.linkRate != null)
            {
                txtLinkRate.Text = AppResources.txtLinkRate;
                txtLinkRate.FontSize = 22;
                txtLinkRate.Margin = new Thickness(10, 10, 0, 0);
                txtLinkRate.HorizontalAlignment = HorizontalAlignment.Left;
                LinkRate.Text = Group.NODE.linkRate;
                LinkRate.FontSize = 26;
                LinkRate.HorizontalAlignment = HorizontalAlignment.Center;
                btnWhat.Content = AppResources.btnWhat;
                btnWhat.Tag = "http://support.netgear.com/app/answers/list/kw/link%20rate";
                btnWhat.FontSize = 20;
                btnWhat.Margin = new Thickness(10, 0, 0, 0);
                btnWhat.Padding = new Thickness(0, 0, 0, 0);
                btnWhat.HorizontalAlignment = HorizontalAlignment.Right;
            }

            //MACAddress
            txtMACAddress.Text = AppResources.txtMACAddress;
            txtMACAddress.FontSize = 22;
            txtMACAddress.Margin = new Thickness(10, 0, 0, 0);
            txtMACAddress.HorizontalAlignment = HorizontalAlignment.Left;
            MACAddress.Text = Group.NODE.MACaddress;
            MACAddress.FontSize = 26;
            MACAddress.HorizontalAlignment = HorizontalAlignment.Center;

            //StackPanel StpMACAddress = new StackPanel();
            //StpMACAddress.Children.Add(txtMACAddress);
            //StpMACAddress.Children.Add(MACAddress);

            //Buttons
            btnBack.Content = AppResources.btnBack;
            btnBack.FontSize = 25;
            btnBack.Width = 200;
            btnBack.HorizontalAlignment = HorizontalAlignment.Center;
            btnBack.Click += new RoutedEventHandler(BackButton_Click);
            btnModify.Content = AppResources.btnModify;
            btnModify.FontSize = 25;
            btnModify.Width = 200;
            btnModify.HorizontalAlignment = HorizontalAlignment.Center;
            btnModify.Click += new RoutedEventHandler(ModifyButton_Click);
            btnApply.Content = AppResources.btnApply;
            btnApply.FontSize = 25;
            btnApply.Width = 200;
            btnApply.HorizontalAlignment = HorizontalAlignment.Center;
            btnApply.Click += new RoutedEventHandler(ApplyButton_Click);
            btnFileUpload.Content = AppResources.btnFileUpload;
            btnFileUpload.FontSize = 25;
            btnFileUpload.Width = 200;
            btnFileUpload.HorizontalAlignment = HorizontalAlignment.Center;

            if (Group.NODE.uniqueId == "LocalDevice")
            {
                //TitleImage.Source = new BitmapImage(new Uri("Assets/androidphone.png", UriKind.Relative));
                switch (Group.NODE.deviceType)
                {
                    case "gatewaydev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/gatewaydev.png", UriKind.Relative));
                        break;
                    case "networkdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/networkdev.png", UriKind.Relative));
                        break;
                    case "windowspc":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/windowspc.png", UriKind.Relative));
                        break;
                    case "blurayplayer":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/blurayplayer.png", UriKind.Relative));
                        break;
                    case "bridge":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/bridge.png", UriKind.Relative));
                        break;
                    case "cablestb":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/cablestb.png", UriKind.Relative));
                        break;
                    case "cameradev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/cameradev.png", UriKind.Relative));
                        break;
                    case "gamedev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/gamedev.png", UriKind.Relative));
                        break;
                    //case "amazonkindledev":
                    //    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                    //    break;
                    //case "ipadmini":
                    //    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                    //    break;
                    //case "iphone5":
                    //    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                    //    break;
                    case "imacdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/imacdev.png", UriKind.Relative));
                        break;
                    case "ipad":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/ipad.png", UriKind.Relative));
                        break;
                    case "iphone":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/iphone.png", UriKind.Relative));
                        break;
                    case "ipodtouch":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/ipodtouch.png", UriKind.Relative));
                        break;
                    case "linuxpc":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/linuxpc.png", UriKind.Relative));
                        break;
                    case "macbookdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/macbookdev.png", UriKind.Relative));
                        break;
                    case "macminidev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/macminidev.png", UriKind.Relative));
                        break;
                    case "macprodev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/macprodev.png", UriKind.Relative));
                        break;
                    case "mediadev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/mediadev.png", UriKind.Relative));
                        break;
                    case "mobiledev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/mobiledev.png", UriKind.Relative));
                        break;
                    case "netstoragedev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/netstoragedev.png", UriKind.Relative));
                        break;
                    case "switchdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/switchdev.png", UriKind.Relative));
                        break;
                    case "printerdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/printerdev.png", UriKind.Relative));
                        break;
                    case "repeater":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/repeater.png", UriKind.Relative));
                        break;
                    case "satellitestb":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/satellitestb.png", UriKind.Relative));
                        break;
                    case "scannerdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/scannerdev.png", UriKind.Relative));
                        break;
                    case "slingbox":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/slingbox.png", UriKind.Relative));
                        break;
                    case "stb":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/stb.png", UriKind.Relative));
                        break;
                    case "tablepc":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/tablepc.png", UriKind.Relative));
                        break;
                    case "tv":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/tv.png", UriKind.Relative));
                        break;
                    case "unixpc":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/unixpc.png", UriKind.Relative));
                        break;
                    case "androiddevice":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androiddevice.png", UriKind.Relative));
                        break;
                    case "androidphone":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                        break;
                    case "androidtablet":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidtablet.png", UriKind.Relative));
                        break;
                    case "dvr":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/dvr.png", UriKind.Relative));
                        break;
                }
                StpTitle.Children.Add(TitleImage);
                StpTitle.Children.Add(Title);

                btnBack.Margin = new Thickness(0, 10, 0, 0);

                StpRouter.Visibility = Visibility.Collapsed;
                StpDeviceName.Visibility = Visibility.Visible;
                StpType.Visibility = Visibility.Collapsed;
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

                NetworkMapInfo.bRefreshMap = false;
            }//if (Group.NODE.uniqueId == "LocalDevice")
            else
            {
                switch (Group.NODE.deviceType)
                {
                    case "gatewaydev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/gatewaydev.png", UriKind.Relative));
                        break;
                    case "networkdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/networkdev.png", UriKind.Relative));
                        break;
                    case "windowspc":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/windowspc.png", UriKind.Relative));
                        break;
                    case "blurayplayer":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/blurayplayer.png", UriKind.Relative));
                        break;
                    case "bridge":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/bridge.png", UriKind.Relative));
                        break;
                    case "cablestb":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/cablestb.png", UriKind.Relative));
                        break;
                    case "cameradev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/cameradev.png", UriKind.Relative));
                        break;
                    case "gamedev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/gamedev.png", UriKind.Relative));
                        break;
                    //case "amazonkindledev":
                    //    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                    //    break;
                    //case "ipadmini":
                    //    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                    //    break;
                    //case "iphone5":
                    //    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                    //    break;
                    case "imacdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/imacdev.png", UriKind.Relative));
                        break;
                    case "ipad":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/ipad.png", UriKind.Relative));
                        break;
                    case "iphone":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/iphone.png", UriKind.Relative));
                        break;
                    case "ipodtouch":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/ipodtouch.png", UriKind.Relative));
                        break;
                    case "linuxpc":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/linuxpc.png", UriKind.Relative));
                        break;
                    case "macbookdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/macbookdev.png", UriKind.Relative));
                        break;
                    case "macminidev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/macminidev.png", UriKind.Relative));
                        break;
                    case "macprodev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/macprodev.png", UriKind.Relative));
                        break;
                    case "mediadev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/mediadev.png", UriKind.Relative));
                        break;
                    case "mobiledev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/mobiledev.png", UriKind.Relative));
                        break;
                    case "netstoragedev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/netstoragedev.png", UriKind.Relative));
                        break;
                    case "switchdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/switchdev.png", UriKind.Relative));
                        break;
                    case "printerdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/printerdev.png", UriKind.Relative));
                        break;
                    case "repeater":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/repeater.png", UriKind.Relative));
                        break;
                    case "satellitestb":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/satellitestb.png", UriKind.Relative));
                        break;
                    case "scannerdev":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/scannerdev.png", UriKind.Relative));
                        break;
                    case "slingbox":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/slingbox.png", UriKind.Relative));
                        break;
                    case "stb":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/stb.png", UriKind.Relative));
                        break;
                    case "tablepc":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/tablepc.png", UriKind.Relative));
                        break;
                    case "tv":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/tv.png", UriKind.Relative));
                        break;
                    case "unixpc":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/unixpc.png", UriKind.Relative));
                        break;
                    case "androiddevice":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androiddevice.png", UriKind.Relative));
                        break;
                    case "androidphone":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                        break;
                    case "androidtablet":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidtablet.png", UriKind.Relative));
                        break;
                    case "dvr":
                        TitleImage.Source = new BitmapImage(new Uri("Assets/devices/dvr.png", UriKind.Relative));
                        break;
                }
                StpTitle.Children.Add(TitleImage);
                StpTitle.Children.Add(Title);

                btnBack.Margin = new Thickness(200, 10, 0, 0);
                btnModify.Margin = new Thickness(0, 10, 200, 0);
                btnApply.Margin = new Thickness(0, 10, 200, 0);

                StpRouter.Visibility = Visibility.Collapsed;
                StpDeviceName.Visibility = Visibility.Visible;
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

                NetworkMapInfo.bRefreshMap = false;
            }
        }

        private void BackButton_Click(Object sender, RoutedEventArgs e)
        {
            DeviceInfoPopup.IsOpen = false;
            PopupBackground.Visibility = Visibility.Collapsed;
            if (NetworkMapInfo.bRefreshMap)
            {
                NetworkMapInfo.bTypeChanged = false;
                OnNavigatedTo(null);
            }            
        }
        
        private void ModifyButton_Click(Object sender, RoutedEventArgs e)
        {
            txtBlockDeviceName.Visibility = Visibility.Collapsed;
            txtBoxDeviceName.Visibility = Visibility.Visible;
            Type.Visibility = Visibility.Collapsed;
            lpType.Visibility = Visibility.Visible;
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

            string customDeviceType = string.Empty;
            switch (lpType.SelectedIndex)
            {
                case 0:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/gatewaydev.png", UriKind.Relative));
                    customDeviceType = "gatewaydev";
                    Type.Text = AppResources.gatewaydev;
                    break;
                case 1:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/networkdev.png", UriKind.Relative));
                    customDeviceType = "networkdev";
                    Type.Text = AppResources.networkdev;
                    break;
                case 2:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/windowspc.png", UriKind.Relative));
                    customDeviceType = "windowspc";
                    Type.Text = AppResources.windowspc;
                    break;
                case 3:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/blurayplayer.png", UriKind.Relative));
                    customDeviceType = "blurayplayer";
                    Type.Text = AppResources.blurayplayer;
                    break;
                case 4:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/bridge.png", UriKind.Relative));
                    customDeviceType = "bridge";
                    Type.Text = AppResources.bridge;
                    break;
                case 5:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/cablestb.png", UriKind.Relative));
                    customDeviceType = "cablestb";
                    Type.Text = AppResources.cablestb;
                    break;
                case 6:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/cameradev.png", UriKind.Relative));
                    customDeviceType = "cameradev";
                    Type.Text = AppResources.cameradev;
                    break;
                case 7:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/gamedev.png", UriKind.Relative));
                    customDeviceType = "gamedev";
                    Type.Text = AppResources.gamedev;
                    break;
                //case 8:
                //    customDeviceType = "amazonkindledev";
                //    Type.Text = AppResources.amazonkindledev;
                //    break;
                //case 9:
                //    customDeviceType = "ipadmini";
                //    Type.Text = AppResources.ipadmini;
                //    break;
                //case 10:
                //    customDeviceType = "iphone5";
                //    Type.Text = AppResources.iphone5;
                //    break;
                case 8:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/imacdev.png", UriKind.Relative));
                    customDeviceType = "imacdev";
                    Type.Text = AppResources.imacdev;
                    break;
                case 9:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/ipad.png", UriKind.Relative));
                    customDeviceType = "ipad";
                    Type.Text = AppResources.ipad;
                    break;
                case 10:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/iphone.png", UriKind.Relative));
                    customDeviceType = "iphone";
                    Type.Text = AppResources.iphone;
                    break;
                case 11:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/ipodtouch.png", UriKind.Relative));
                    customDeviceType = "ipodtouch";
                    Type.Text = AppResources.ipodtouch;
                    break;
                case 12:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/linuxpc.png", UriKind.Relative));
                    customDeviceType = "linuxpc";
                    Type.Text = AppResources.linuxpc;
                    break;
                case 13:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/macbookdev.png", UriKind.Relative));
                    customDeviceType = "macbookdev";
                    Type.Text = AppResources.macbookdev;
                    break;
                case 14:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/macminidev.png", UriKind.Relative));
                    customDeviceType = "macminidev";
                    Type.Text = AppResources.macminidev;
                    break;
                case 15:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/macprodev.png", UriKind.Relative));
                    customDeviceType = "macprodev";
                    Type.Text = AppResources.macprodev;
                    break;
                case 16:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/mediadev.png", UriKind.Relative));
                    customDeviceType = "mediadev";
                    Type.Text = AppResources.mediadev;
                    break;
                case 17:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/mobiledev.png", UriKind.Relative));
                    customDeviceType = "mobiledev";
                    Type.Text = AppResources.mobiledev;
                    break;
                case 18:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/netstoragedev.png", UriKind.Relative));
                    customDeviceType = "netstoragedev";
                    Type.Text = AppResources.netstoragedev;
                    break;
                case 19:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/switchdev.png", UriKind.Relative));
                    customDeviceType = "switchdev";
                    Type.Text = AppResources.switchdev;
                    break;
                case 20:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/printerdev.png", UriKind.Relative));
                    customDeviceType = "printerdev";
                    Type.Text = AppResources.printerdev;
                    break;
                case 21:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/repeater.png", UriKind.Relative));
                    customDeviceType = "repeater";
                    Type.Text = AppResources.repeater;
                    break;
                case 22:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/satellitestb.png", UriKind.Relative));
                    customDeviceType = "satellitestb";
                    Type.Text = AppResources.satellitestb;
                    break;
                case 23:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/scannerdev.png", UriKind.Relative));
                    customDeviceType = "scannerdev";
                    Type.Text = AppResources.scannerdev;
                    break;
                case 24:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/slingbox.png", UriKind.Relative));
                    customDeviceType = "slingbox";
                    Type.Text = AppResources.slingbox;
                    break;
                case 25:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/stb.png", UriKind.Relative));
                    customDeviceType = "stb";
                    Type.Text = AppResources.stb;
                    break;
                case 26:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/tablepc.png", UriKind.Relative));
                    customDeviceType = "tablepc";
                    Type.Text = AppResources.tablepc;
                    break;
                case 27:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/tv.png", UriKind.Relative));
                    customDeviceType = "tv";
                    Type.Text = AppResources.tv;
                    break;
                case 28:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/unixpc.png", UriKind.Relative));
                    customDeviceType = "unixpc";
                    Type.Text = AppResources.unixpc;
                    break;
                case 29:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androiddevice.png", UriKind.Relative));
                    customDeviceType = "androiddevice";
                    Type.Text = AppResources.androiddevice;
                    break;
                case 30:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidphone.png", UriKind.Relative));
                    customDeviceType = "androidphone";
                    Type.Text = AppResources.androidphone;
                    break;
                case 31:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/androidtablet.png", UriKind.Relative));
                    customDeviceType = "androidtablet";
                    Type.Text = AppResources.androidtablet;
                    break;
                case 32:
                    TitleImage.Source = new BitmapImage(new Uri("Assets/devices/dvr.png", UriKind.Relative));
                    customDeviceType = "dvr";
                    Type.Text = AppResources.dvr;
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
            lpType.Visibility = Visibility.Collapsed;
            btnModify.Visibility = Visibility.Visible;
            btnApply.Visibility = Visibility.Collapsed;

            NetworkMapInfo.bRefreshMap = true;
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

        //刷新按钮响应事件
        private async void appBarButton_refresh_Click(object sender, EventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            UtilityTool util = new UtilityTool();
            NetworkMapInfo.geteway = await util.GetGateway();
            Dictionary<string, Dictionary<string, string>> responseDic = new Dictionary<string, Dictionary<string, string>>();
            responseDic = await soapApi.GetAttachDevice();
            NetworkMapInfo.attachDeviceDic = responseDic;

            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetInfo("WLANConfiguration");
            if (dicResponse.Count > 0)
            {
                WifiSettingInfo.ssid = dicResponse["NewSSID"];
                WifiSettingInfo.channel = dicResponse["NewChannel"];
                WifiSettingInfo.securityType = dicResponse["NewWPAEncryptionModes"];
                WifiSettingInfo.macAddr = dicResponse["NewWLANMACAddress"];
            }
            NetworkMapInfo.fileContent = await ReadDeviceInfoFile();
            PopupBackground.Visibility = Visibility.Collapsed;
            NetworkMapInfo.bTypeChanged = false;
            OnNavigatedTo(null);
        }

        private void lpType_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            NetworkMapInfo.bTypeChanged = true;
        }

        //"这是什么"链接响应事件
        private async void btnWhat_Click(Object sender, RoutedEventArgs e)
        {
            var uri = new Uri(((HyperlinkButton)sender).Tag.ToString());
            await Windows.System.Launcher.LaunchUriAsync(uri);
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

        public async void WriteDeviceInfoFile()
        {            
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (!fileStorage.FileExists("CustomDeviceInfo.txt"))            //CustomDeviceInfo.txt中保存本地修改的设备信息，包括设备MAC地址、设备名和设备类型，格式为"MACAddress,DeviceName,DeviceType;"
                {
                    using (var file = fileStorage.CreateFile("CustomDeviceInfo.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(NetworkMapInfo.fileContent);
                        }
                    }
                }
                else
                {
                    fileStorage.DeleteFile("CustomDeviceInfo.txt");
                    using (var file = fileStorage.CreateFile("CustomDeviceInfo.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(NetworkMapInfo.fileContent);
                        }
                    }
                }
            }
            catch (FileNotFoundException)
            {

            }
        }       

        public async Task<string> ReadDeviceInfoFile()
        {
            string fileContent = string.Empty;
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (fileStorage.FileExists("CustomDeviceInfo.txt"))
                {
                    using (var file = fileStorage.OpenFile("CustomDeviceInfo.txt", FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(file))
                        {
                            fileContent = await reader.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return fileContent;
        }
    }
}