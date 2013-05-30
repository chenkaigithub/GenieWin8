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

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class ParentalControlPage : GenieWin8.Common.LayoutAwarePage
    {
        public ParentalControlPage()
        {
            this.InitializeComponent();
            if (false)	//未登录OpenDNS账户
	        {
		        if (!EnquirePopup.IsOpen)
		        {
			        EnquirePopup.IsOpen = true;
			        PopupBackground.Visibility = Visibility.Visible;
			        NoButton.Visibility = Visibility.Visible;
			        YesButton.Visibility = Visibility.Visible;
		        }
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
            var FilterLevelGroup = FilterLevelSource.GetGroup((String)navigationParameter);
            this.DefaultViewModel["Group"] = FilterLevelGroup;
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

        private void FilterLevel_ItemClick(Object sender, ItemClickEventArgs e)
        {
            if (!FilterLevelPopup.IsOpen)
	        {
		        FilterLevelPopup.IsOpen = true;
		        PopupBackground.Visibility = Visibility.Visible;
		        FilterLvPreviousButton.Visibility = Visibility.Visible;
		        FilterLvNextButton.Visibility = Visibility.Visible;
	        }
        }

        private async void ChangeSetting_ItemClick(Object sender, ItemClickEventArgs e)
        {
            var uri = new Uri("http://netgear.opendns.com/account.php?device_id=0000DF346BC636E0");
	        await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        private void Bypass_ItemClick(Object sender, ItemClickEventArgs e)
        {

        }

        private void NoButton_Click(Object sender, RoutedEventArgs e)
        {
            if (EnquirePopup.IsOpen)
	        {
		        EnquirePopup.IsOpen = false;
		        RegisterPopup.IsOpen = true;
		        PopupBackground.Visibility = Visibility.Visible;
		        NoButton.Visibility = Visibility.Collapsed;
		        YesButton.Visibility = Visibility.Collapsed;
		        RegisterPreviousButton.Visibility = Visibility.Visible;
		        RegisterNextButton.Visibility = Visibility.Visible;
	        }
        }

        private void YesButton_Click(Object sender, RoutedEventArgs e)
        {
            if (EnquirePopup.IsOpen)
	        {
		        EnquirePopup.IsOpen = false;
		        LoginPopup.IsOpen = true;
		        PopupBackground.Visibility = Visibility.Visible;
		        NoButton.Visibility = Visibility.Collapsed;
		        YesButton.Visibility = Visibility.Collapsed;
		        LoginPreviousButton.Visibility = Visibility.Visible;
		        LoginNextButton.Visibility = Visibility.Visible;
	        }
        }

        private void RegisterPreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            if (RegisterPopup.IsOpen)
	        {
		        EnquirePopup.IsOpen = true;
		        RegisterPopup.IsOpen = false;
		        PopupBackground.Visibility = Visibility.Visible;
		        NoButton.Visibility = Visibility.Visible;
		        YesButton.Visibility = Visibility.Visible;
		        RegisterPreviousButton.Visibility = Visibility.Collapsed;
		        RegisterNextButton.Visibility = Visibility.Collapsed;
	        }
        }

        private void RegisterNextButton_Click(Object sender, RoutedEventArgs e)
        {
            if (RegisterPopup.IsOpen)
	        {		
		        RegisterPopup.IsOpen = false;
		        LoginPopup.IsOpen = true;
		        PopupBackground.Visibility = Visibility.Visible;
		        RegisterPreviousButton.Visibility = Visibility.Collapsed;
		        RegisterNextButton.Visibility = Visibility.Collapsed;
		        LoginPreviousButton.Visibility = Visibility.Visible;
		        LoginNextButton.Visibility = Visibility.Visible;
	        }
        }

        private void LoginPreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LoginPopup.IsOpen)
	        {
		        EnquirePopup.IsOpen = true;
		        LoginPopup.IsOpen = false;
		        PopupBackground.Visibility = Visibility.Visible;
		        NoButton.Visibility = Visibility.Visible;
		        YesButton.Visibility = Visibility.Visible;
		        LoginPreviousButton.Visibility = Visibility.Collapsed;
		        LoginNextButton.Visibility = Visibility.Collapsed;
	        }
        }

        private void LoginNextButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LoginPopup.IsOpen)
	        {
		        LoginPopup.IsOpen = false;
		        FilterLevelPopup.IsOpen = true;
		        PopupBackground.Visibility = Visibility.Visible;
		        LoginPreviousButton.Visibility = Visibility.Collapsed;
		        LoginNextButton.Visibility = Visibility.Collapsed;
		        FilterLvPreviousButton.Visibility = Visibility.Visible;
		        FilterLvNextButton.Visibility = Visibility.Visible;
	        }
        }

        private void FilterLvPreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            if (FilterLevelPopup.IsOpen)
	        {
		        LoginPopup.IsOpen = true;
		        FilterLevelPopup.IsOpen = false;
		        PopupBackground.Visibility = Visibility.Visible;
		        LoginPreviousButton.Visibility = Visibility.Visible;
		        LoginNextButton.Visibility = Visibility.Visible;
		        FilterLvPreviousButton.Visibility = Visibility.Collapsed;
		        FilterLvNextButton.Visibility = Visibility.Collapsed;
	        }
        }

        private void FilterLvNextButton_Click(Object sender, RoutedEventArgs e)
        {
            if (FilterLevelPopup.IsOpen)
	        {
		        FilterLevelPopup.IsOpen = false;
		        SettingCompletePopup.IsOpen = true;
		        PopupBackground.Visibility = Visibility.Visible;
		        FilterLvPreviousButton.Visibility = Visibility.Collapsed;
		        FilterLvNextButton.Visibility = Visibility.Collapsed;
		        ReturnToStatusButton.Visibility = Visibility.Visible;
	        }
        }

        private void ReturnToStatusButton_Click(Object sender, RoutedEventArgs e)
        {
            if (SettingCompletePopup.IsOpen)
	        {
		        SettingCompletePopup.IsOpen = false;
		        PopupBackground.Visibility = Visibility.Collapsed;
		        ReturnToStatusButton.Visibility = Visibility.Collapsed;
                this.Frame.Navigate(typeof(ParentalControlPage));
	        }
        }
    }
}
