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

            Image TitleImage = new Image();
            TitleImage.HorizontalAlignment = HorizontalAlignment.Left;
            TitleImage.VerticalAlignment = VerticalAlignment.Center;
            TitleImage.Margin = new Thickness(10, 0, 0, 0);
            TitleImage.Width = 100; TitleImage.Height = 100;

            TextBlock Title = new TextBlock();
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
            txtBoxDeviceName.FontSize = 30;
            txtBoxDeviceName.HorizontalAlignment = HorizontalAlignment.Center;
            txtBoxDeviceName.Width = 200;
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
                Type.Text = Group.NODE.deviceType;
                Type.FontSize = 30;
                Type.HorizontalAlignment = HorizontalAlignment.Center;

                //ComboBox ComboType = new ComboBox();
                //ComboType.Items.Add("Router");
                //ComboType.Items.Add("Network Device");
                //ComboType.Items.Add("Windows PC");
                //ComboType.Width = 200;
                //ComboType.HorizontalAlignment = HorizontalAlignment.Center;


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
                TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/repeater72.png"));
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
                TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/AndroidPhone72.png"));
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
                TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/networkdev72.png"));
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
            btnModify.Visibility = Visibility.Collapsed;
            btnApply.Visibility = Visibility.Visible;
        }

        private void ApplyButton_Click(Object sender, RoutedEventArgs e)
        {

        }
    }
}
