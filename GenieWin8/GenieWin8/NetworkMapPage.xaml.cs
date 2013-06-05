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
                imgRouter.Source = new BitmapImage(new Uri(_baseUri, "Assets/repeater72.png"));
                imgRouter.Stretch = Stretch.UniformToFill;
                BtnRouter.Content = imgRouter;
                BtnRouter.Margin = new Thickness(0, 0, 0, 0);
                BtnRouter.Click += new RoutedEventHandler(DeviceButton_Click);

                if (i != m)
                {
                    double Angle = 360.0 / 8;
                    for (int j = 0; j < 8; j++)
                    {
                        double x = r1 * Math.Cos(j * Angle * PI / 180);
                        double y = r2 * Math.Sin(j * Angle * PI / 180);
                        Line line = new Line();
                        line.X1 = width / 2; line.Y1 = height / 2;
                        line.X2 = width / 2 + x; line.Y2 = height / 2 - y;
                        line.Stroke = new SolidColorBrush(Colors.SeaGreen);
                        line.StrokeThickness = 3;
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
                            if (BtnDevice.Name == "LocalDevice")
                            {
                                imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/AndroidPhone72.png"));
                            }
                            else
                            {
                                imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/networkdev72.png"));
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
                }
                else
                {
                    double Angle = 360.0 / (n + 2);
                    for (int j = 0; j < n + 2; j++)
                    {
                        double x = r1 * Math.Cos(j * Angle * PI / 180);
                        double y = r2 * Math.Sin(j * Angle * PI / 180);
                        Line line = new Line();
                        line.X1 = width / 2; line.Y1 = height / 2;
                        line.X2 = width / 2 + x; line.Y2 = height / 2 - y;
                        line.Stroke = new SolidColorBrush(Colors.SeaGreen);
                        line.StrokeThickness = 3;
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
                            if (BtnDevice.Name == "LocalDevice")
                            {
                                imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/AndroidPhone72.png"));
                            }
                            else
                            {
                                imgDevice.Source = new BitmapImage(new Uri(_baseUri, "Assets/networkdev72.png"));
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
                }
                map.Children.Add(internet);
                map.Children.Add(BtnRouter);
                MapFlipView.Items.Add(map);
            }
        }

        private void DeviceButton_Click(Object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            var UniqueId = btn.Name;
            this.Frame.Navigate(typeof(DeviceInfoPage), UniqueId);
        }
    }
}
