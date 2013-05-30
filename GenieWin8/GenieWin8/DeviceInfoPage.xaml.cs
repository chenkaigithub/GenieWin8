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
            StpDeviceInfo.Children.Clear();

            TextBlock Title = new TextBlock();
            Title.Text = Group.DeviceName;
            Title.FontSize = 40;
            Title.VerticalAlignment = VerticalAlignment.Center;
            Title.Margin = new Thickness(10, 0, 0, 0);
            Title.FontWeight = FontWeights.Bold;
            StpTitle.Children.Add(Title);

            if (Group.UniqueId == "Router")
            {
                Image TitleImage = new Image();
                Uri _baseUri = new Uri("ms-appx:///");
                TitleImage.Source = new BitmapImage(new Uri(_baseUri, "Assets/repeater72.png"));
                TitleImage.HorizontalAlignment = HorizontalAlignment.Left;
                TitleImage.VerticalAlignment = VerticalAlignment.Center;
                TitleImage.Margin = new Thickness(10,0,0,0);
                TitleImage.Width = 100; TitleImage.Height = 100;
                StpTitle.Children.Add(TitleImage);

                TextBlock txtRoutename = new TextBlock();
                txtRoutename.FontSize = 25;
                txtRoutename.Margin = new Thickness(10,10,0,0);
                txtRoutename.HorizontalAlignment = HorizontalAlignment.Left;
                TextBlock RouteName = new TextBlock();
                RouteName.Text = Group.DeviceName;
                RouteName.FontSize = 30;
                RouteName.Margin = new Thickness(0,10,0,0);
                RouteName.HorizontalAlignment = HorizontalAlignment.Center;

                StackPanel StpRouter = new StackPanel();
                StpRouter.Children.Add(txtRoutename);
                StpRouter.Children.Add(RouteName);
                StpDeviceInfo.Children.Add(StpRouter);               
            }
            //MessageDialog msg = new MessageDialog(Group.DeviceName);
            //await msg.ShowAsync();
            //this.DefaultViewModel["Group"] = Group;
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
    }
}
