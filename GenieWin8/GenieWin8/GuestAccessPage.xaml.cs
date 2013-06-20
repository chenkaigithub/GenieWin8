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
using GenieWin8.DataModel;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class GuestAccessPage : GenieWin8.Common.LayoutAwarePage
    {
        public GuestAccessPage()
        {
            this.InitializeComponent();
            if (GuestAccessInfoModel.isGuestAccessEnabled == "0")
            {
                checkGuestSetting.IsChecked = false;
                GuestSettingsList.Visibility = Visibility.Collapsed;
                textScanQRCode.Visibility = Visibility.Collapsed;
                imageQRCode.Visibility = Visibility.Collapsed;
            }
            else if (GuestAccessInfoModel.isGuestAccessEnabled == "1")
            {
                checkGuestSetting.IsChecked = true;
                GuestSettingsList.Visibility = Visibility.Visible;
                textScanQRCode.Visibility = Visibility.Visible;
                imageQRCode.Visibility = Visibility.Visible;             
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
            var GuestSettingGroup = GuestSettingSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = GuestSettingGroup;
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

        private void GuestSetting_ItemClick(Object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(GuestSettingPage));
        }

        private void checkGuestSetting_Click(Object sender, RoutedEventArgs e)
        {
            if (checkGuestSetting.IsChecked == true)
            {
                GuestSettingsList.Visibility = Visibility.Visible;
                textScanQRCode.Visibility = Visibility.Visible;
                imageQRCode.Visibility = Visibility.Visible;
            }
            else if (checkGuestSetting.IsChecked == false)
            {
                GuestSettingsList.Visibility = Visibility.Collapsed;
                textScanQRCode.Visibility = Visibility.Collapsed;
                imageQRCode.Visibility = Visibility.Collapsed;
            }
        }
    }
}
