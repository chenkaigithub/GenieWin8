﻿using GenieWin8.Data;

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

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class NetworkMapPage : GenieWin8.Common.LayoutAwarePage
    {
        public NetworkMapPage()
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

        private void OnWindowSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            //double Appwidth = e.Size.Width;
            //double AppHeight = e.Size.Height;	
            double PI = 3.141592653589793;
            MapFlipView.Items.Clear();
            double width = Window.Current.Bounds.Width;
            double height = Window.Current.Bounds.Height - 140;
            double r1 = width / 2 - 100;
            double r2 = height / 2 - 100;
            //m = （总设备数 - 1（路由器/交换机）-1（本设备））/ 6 的商
            //n 为上式得余数，即最后一页除路由器（交换机）和本设备外的设备数
            var Group = DeviceSource.GetGroups();
            var DeviceNumber = Group.Count();
            int m = (DeviceNumber - 1 - 1) / 6;
            int n = (DeviceNumber - 1 - 1) % 6;
            for (int i = 0; i < m + 1; i++)
            {
                //Grid map = new Grid();

                //Image internet = new Image();
                //Uri _baseUri = new Uri("ms-appx:///");
                //internet.Source = new BitmapImage(new Uri(_baseUri, "Assets/internet72.png"));
                //internet.HorizontalAlignment = HorizontalAlignment.Right;
                //internet.Margin = new Thickness(0, 0, 50, 0);
                //internet.Width = 100; internet.Height = 100;


                //Button BtnRouter = new Button();
                ////BtnRouter.Name = "Router";
                //BtnRouter.Name = Group.ElementAt(0).NODE.uniqueId;
                //BtnRouter.SetValue(WidthProperty, 150);
                //BtnRouter.SetValue(HeightProperty, 150);
                //BtnRouter.HorizontalAlignment = HorizontalAlignment.Center;
                //BtnRouter.VerticalAlignment = VerticalAlignment.Center;
                //Image imgRouter = new Image();
                //imgRouter.Source = new BitmapImage(new Uri(_baseUri, "Assets/gatewaydev.png"));
                //imgRouter.Stretch = Stretch.UniformToFill;
                //BtnRouter.Content = imgRouter;
                //BtnRouter.Margin = new Thickness(0, 0, 0, 0);
                //BtnRouter.Click += new RoutedEventHandler(DeviceButton_Click);

                if (i != m)
                {
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
                    imgRouter.Source = new BitmapImage(new Uri(_baseUri, "Assets/gatewaydev.png"));
                    imgRouter.Stretch = Stretch.UniformToFill;
                    BtnRouter.Content = imgRouter;
                    BtnRouter.Margin = new Thickness(0, 0, 0, 0);
                    BtnRouter.Click += new RoutedEventHandler(DeviceButton_Click);

                    double Angle = 360.0 / 8;
                    for (int j = 0; j < 8; j++)
                    {
                        double x = r1 * Math.Cos(j * Angle * PI / 180);
                        double y = r2 * Math.Sin(j * Angle * PI / 180);
                        Line line = new Line();
                        line.X1 = width / 2; line.Y1 = height / 2;
                        line.X2 = width / 2 + x; line.Y2 = height / 2 - y;
                        line.Stroke = new SolidColorBrush(Colors.SeaGreen);
                        line.StrokeThickness = 2;
                        if (j > 0 && Group.ElementAt(7 * i + j).NODE.connectType == "wireless")
                        {
                            DoubleCollection dc = new DoubleCollection();
                            dc.Add(2);
                            line.StrokeDashArray = dc;
                        }
                        map.Children.Add(line);

                        //if (j == 1)
                        //{
                        //    Button BtnDeviceLocal = new Button();
                        //    //BtnDeviceLocal.Name = "LocalDevice";
                        //    BtnDeviceLocal.Name = Group.ElementAt(j).UniqueId;
                        //    BtnDeviceLocal.SetValue(WidthProperty, 100);
                        //    BtnDeviceLocal.SetValue(HeightProperty, 100);
                        //    BtnDeviceLocal.HorizontalAlignment = HorizontalAlignment.Left;
                        //    BtnDeviceLocal.VerticalAlignment = VerticalAlignment.Top;
                        //    BtnDeviceLocal.Margin = new Thickness(width/2 + x - 50, height/2 - y - 50, 0, 0);
                        //    Image imgDeviceLocal = new Image();
                        //    imgDeviceLocal.Source = new BitmapImage(new Uri(_baseUri, "Assets/AndroidPhone72.png"));
                        //    imgDeviceLocal.Stretch = Stretch.UniformToFill;
                        //    TextBlock DeviceNameTextLocal = new TextBlock();
                        //    //DeviceNameTextLocal.Text = "android-25531554966beee3";
                        //    DeviceNameTextLocal.Text = Group.ElementAt(j).DeviceName;
                        //    StackPanel stpDeviceLocal = new StackPanel();
                        //    stpDeviceLocal.Children.Add(imgDeviceLocal);
                        //    stpDeviceLocal.Children.Add(DeviceNameTextLocal);
                        //    BtnDeviceLocal.Content = stpDeviceLocal;
                        //    BtnDeviceLocal.Click += new RoutedEventHandler(DeviceButton_Click);
                        //    map.Children.Add(BtnDeviceLocal);
                        //}
                        //else if (j > 1)
                        //{
                        if (j > 0)
                        {
                            Button BtnDevice = new Button();
                            //BtnDevice.Name = "Device-" + (6 * i + j - 1).ToString();
                            BtnDevice.Name = Group.ElementAt(7 * i + j).NODE.uniqueId;
                            BtnDevice.SetValue(WidthProperty, 100);
                            BtnDevice.SetValue(HeightProperty, 100);
                            BtnDevice.HorizontalAlignment = HorizontalAlignment.Left;
                            BtnDevice.VerticalAlignment = VerticalAlignment.Top;
                            BtnDevice.Margin = new Thickness(width / 2 + x - 50, height / 2 - y - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
                            Image imgDevice = new Image();                            
                            //if (BtnDevice.Name == "LocalDevice")
                            //{
                            //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                            //}
                            //else
                            //{
                            //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/networkdev.png"));
                            //}
                            switch (Group.ElementAt(7 * i + j).NODE.deviceType)
                            {
                                case "gatewaydev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/gatewaydev.png"));
                                    break;
                                case "networkdev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/networkdev.png"));
                                    break;
                                case "windowspc":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/windowspc.png"));
                                    break;
                                case "blurayplayer":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/blurayplayer.png"));
                                    break;
                                case "bridge":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/bridge.png"));
                                    break;
                                case "cablestb":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/cablestb.png"));
                                    break;
                                case "cameradev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/cameradev.png"));
                                    break;
                                case "gamedev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/gamedev.png"));
                                    break;
                                //case "amazonkindledev":
                                //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                                //    break;
                                //case "ipadmini":
                                //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                                //    break;
                                //case "iphone5":
                                //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                                //    break;
                                case "imacdev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/imacdev.png"));
                                    break;
                                case "ipad":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/ipad.png"));
                                    break;
                                case "iphone":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/iphone.png"));
                                    break;
                                case "ipodtouch":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/ipodtouch.png"));
                                    break;
                                case "linuxpc":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/linuxpc.png"));
                                    break;
                                case "macbookdev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/macbookdev.png"));
                                    break;
                                case "macminidev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/macminidev.png"));
                                    break;
                                case "macprodev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/macprodev.png"));
                                    break;
                                case "mediadev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/mediadev.png"));
                                    break;
                                case "mobiledev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/mobiledev.png"));
                                    break;
                                case "netstoragedev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/netstoragedev.png"));
                                    break;
                                case "switchdev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/switchdev.png"));
                                    break;
                                case "printerdev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/printerdev.png"));
                                    break;
                                case "repeater":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/repeater.png"));
                                    break;
                                case "satellitestb":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/satellitestb.png"));
                                    break;
                                case "scannerdev":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/scannerdev.png"));
                                    break;
                                case "slingbox":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/slingbox.png"));
                                    break;
                                case "stb":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/stb.png"));
                                    break;
                                case "tablepc":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/tablepc.png"));
                                    break;
                                case "tv":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/tv.png"));
                                    break;
                                case "unixpc":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/unixpc.png"));
                                    break;
                                case "androiddevice":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androiddevice.png"));
                                    break;
                                case "androidphone":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                                    break;
                                case "androidtablet":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidtablet.png"));
                                    break;
                                case "dvr":
                                    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/dvr.png"));
                                    break;
                            }
                            imgDevice.Stretch = Stretch.UniformToFill;
                            TextBlock DeviceNameText = new TextBlock();
                            //DeviceNameText.Text = "Device-" + (6*i+j-1).ToString();
                            DeviceNameText.Text = Group.ElementAt(7 * i + j).NODE.deviceName;
                            StackPanel stpDevice = new StackPanel();
                            stpDevice.Children.Add(imgDevice);
                            stpDevice.Children.Add(DeviceNameText);
                            BtnDevice.Content = stpDevice;
                            BtnDevice.Click += new RoutedEventHandler(DeviceButton_Click);
                            map.Children.Add(BtnDevice);
                        }
                        //}				
                    }

                    map.Children.Add(internet);
                    map.Children.Add(BtnRouter);
                    MapFlipView.Items.Add(map);
                }
                else
                {
                    if (n != 0)
                    {
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
                        imgRouter.Source = new BitmapImage(new Uri(_baseUri, "Assets/gatewaydev.png"));
                        imgRouter.Stretch = Stretch.UniformToFill;
                        BtnRouter.Content = imgRouter;
                        BtnRouter.Margin = new Thickness(0, 0, 0, 0);
                        BtnRouter.Click += new RoutedEventHandler(DeviceButton_Click);

                        double Angle = 360.0 / (n + 1);
                        for (int j = 0; j < n + 1; j++)
                        {
                            double x = r1 * Math.Cos(j * Angle * PI / 180);
                            double y = r2 * Math.Sin(j * Angle * PI / 180);
                            Line line = new Line();
                            line.X1 = width / 2; line.Y1 = height / 2;
                            line.X2 = width / 2 + x; line.Y2 = height / 2 - y;
                            line.Stroke = new SolidColorBrush(Colors.SeaGreen);
                            line.StrokeThickness = 2;
                            if (j > 0 && Group.ElementAt(7 * i + j).NODE.connectType == "wireless")
                            {
                                DoubleCollection dc = new DoubleCollection();
                                dc.Add(2);
                                line.StrokeDashArray = dc;
                            }
                            map.Children.Add(line);

                            //if (j == 1)
                            //{
                            //    Button BtnDeviceLocal = new Button();
                            //    //BtnDeviceLocal.Name = "LocalDevice";
                            //    BtnDeviceLocal.Name = Group.ElementAt(j).UniqueId;
                            //    BtnDeviceLocal.SetValue(WidthProperty, 100);
                            //    BtnDeviceLocal.SetValue(HeightProperty, 100);
                            //    BtnDeviceLocal.HorizontalAlignment = HorizontalAlignment.Left;
                            //    BtnDeviceLocal.VerticalAlignment = VerticalAlignment.Top;
                            //    BtnDeviceLocal.Margin = new Thickness(width/2 + x - 50, height/2 - y - 50, 0, 0);
                            //    Image imgDeviceLocal = new Image();
                            //    imgDeviceLocal.Source = new BitmapImage(new Uri(_baseUri, "Assets/AndroidPhone72.png"));
                            //    imgDeviceLocal.Stretch = Stretch.UniformToFill;
                            //    TextBlock DeviceNameTextLocal = new TextBlock();
                            //    //DeviceNameTextLocal.Text = "android-25531554966beee3";
                            //    DeviceNameTextLocal.Text = Group.ElementAt(j).DeviceName;
                            //    StackPanel stpDeviceLocal = new StackPanel();
                            //    stpDeviceLocal.Children.Add(imgDeviceLocal);
                            //    stpDeviceLocal.Children.Add(DeviceNameTextLocal);
                            //    BtnDeviceLocal.Content = stpDeviceLocal;
                            //    BtnDeviceLocal.Click += new RoutedEventHandler(DeviceButton_Click);
                            //    map.Children.Add(BtnDeviceLocal);
                            //}
                            //else if (j > 1)
                            //{
                            if (j > 0)
                            {
                                Button BtnDevice = new Button();
                                //BtnDevice.Name = "Device-" + (6 * i + j - 1).ToString();
                                BtnDevice.Name = Group.ElementAt(7 * i + j).NODE.uniqueId;
                                BtnDevice.SetValue(WidthProperty, 100);
                                BtnDevice.SetValue(HeightProperty, 100);
                                BtnDevice.HorizontalAlignment = HorizontalAlignment.Left;
                                BtnDevice.VerticalAlignment = VerticalAlignment.Top;
                                BtnDevice.Margin = new Thickness(width / 2 + x - 50, height / 2 - y - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
                                Image imgDevice = new Image();
                                //if (BtnDevice.Name == "LocalDevice")
                                //{
                                //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                                //}
                                //else
                                //{
                                //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/networkdev.png"));
                                //}
                                switch (Group.ElementAt(7 * i + j).NODE.deviceType)
                                {
                                    case "gatewaydev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/gatewaydev.png"));
                                        break;
                                    case "networkdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/networkdev.png"));
                                        break;
                                    case "windowspc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/windowspc.png"));
                                        break;
                                    case "blurayplayer":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/blurayplayer.png"));
                                        break;
                                    case "bridge":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/bridge.png"));
                                        break;
                                    case "cablestb":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/cablestb.png"));
                                        break;
                                    case "cameradev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/cameradev.png"));
                                        break;
                                    case "gamedev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/gamedev.png"));
                                        break;
                                    //case "amazonkindledev":
                                    //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                                    //    break;
                                    //case "ipadmini":
                                    //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                                    //    break;
                                    //case "iphone5":
                                    //    imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                                    //    break;
                                    case "imacdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/imacdev.png"));
                                        break;
                                    case "ipad":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/ipad.png"));
                                        break;
                                    case "iphone":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/iphone.png"));
                                        break;
                                    case "ipodtouch":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/ipodtouch.png"));
                                        break;
                                    case "linuxpc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/linuxpc.png"));
                                        break;
                                    case "macbookdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/macbookdev.png"));
                                        break;
                                    case "macminidev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/macminidev.png"));
                                        break;
                                    case "macprodev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/macprodev.png"));
                                        break;
                                    case "mediadev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/mediadev.png"));
                                        break;
                                    case "mobiledev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/mobiledev.png"));
                                        break;
                                    case "netstoragedev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/netstoragedev.png"));
                                        break;
                                    case "switchdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/switchdev.png"));
                                        break;
                                    case "printerdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/printerdev.png"));
                                        break;
                                    case "repeater":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/repeater.png"));
                                        break;
                                    case "satellitestb":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/satellitestb.png"));
                                        break;
                                    case "scannerdev":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/scannerdev.png"));
                                        break;
                                    case "slingbox":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/slingbox.png"));
                                        break;
                                    case "stb":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/stb.png"));
                                        break;
                                    case "tablepc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/tablepc.png"));
                                        break;
                                    case "tv":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/tv.png"));
                                        break;
                                    case "unixpc":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/unixpc.png"));
                                        break;
                                    case "androiddevice":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androiddevice.png"));
                                        break;
                                    case "androidphone":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidphone.png"));
                                        break;
                                    case "androidtablet":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/androidtablet.png"));
                                        break;
                                    case "dvr":
                                        imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/dvr.png"));
                                        break;
                                }
                                imgDevice.Stretch = Stretch.UniformToFill;
                                TextBlock DeviceNameText = new TextBlock();
                                //DeviceNameText.Text = "Device-" + (6*i+j-1).ToString();
                                DeviceNameText.Text = Group.ElementAt(7 * i + j).NODE.deviceName;
                                StackPanel stpDevice = new StackPanel();
                                stpDevice.Children.Add(imgDevice);
                                stpDevice.Children.Add(DeviceNameText);
                                BtnDevice.Content = stpDevice;
                                BtnDevice.Click += new RoutedEventHandler(DeviceButton_Click);
                                map.Children.Add(BtnDevice);
                            }
                            //}
                        }
                        map.Children.Add(internet);
                        map.Children.Add(BtnRouter);
                        MapFlipView.Items.Add(map);
                    }
                }
                //map.Children.Add(internet);
                //map.Children.Add(BtnRouter);
                //MapFlipView.Items.Add(map);
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
            GenieSoapApi soapApi = new GenieSoapApi();
            UtilityTool util = new UtilityTool();
            NetworkMapDodel.geteway = await util.GetGateway();
            Dictionary<string, Dictionary<string, string>> responseDic = new Dictionary<string, Dictionary<string, string>>();
            responseDic = await soapApi.GetAttachDevice();
            NetworkMapDodel.attachDeviceDic = responseDic;

            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetInfo("WLANConfiguration");
            WifiInfoModel.ssid = dicResponse["NewSSID"];
            WifiInfoModel.channel = dicResponse["NewChannel"];
            WifiInfoModel.securityType = dicResponse["NewWPAEncryptionModes"];
            WifiInfoModel.macAddr = dicResponse["NewWLANMACAddress"];
            NetworkMapDodel.fileContent = await ReadDeviceInfoFile();
            this.Frame.Navigate(typeof(NetworkMapPage));
        }

        public async Task<string> ReadDeviceInfoFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
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

            }
            return fileContent;
        }
    }
}
