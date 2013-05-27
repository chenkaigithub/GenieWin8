using GenieWin8.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Graphics.Display;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// “项目页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234233 上提供

namespace GenieWin8
{
    /// <summary>
    /// 显示项预览集合的页。在“拆分布局应用程序”中，此页
    /// 用于显示及选择可用组之一。
    /// </summary>
    public sealed partial class MainPage : GenieWin8.Common.LayoutAwarePage
    {
        public MainPage()
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
            // TODO: 创建适用于问题域的合适数据模型以替换示例数据
            var dataGroups = DataSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Items"] = dataGroups;
        }

        /// <summary>
        /// 在单击某个项时进行调用。
        /// </summary>
        /// <param name="sender">显示所单击项的 GridView (在应用程序处于对齐状态时
        /// 为 ListView)。</param>
        /// <param name="e">描述所单击项的事件数据。</param>
        private async void ItemView_ItemClick(object sender, ItemClickEventArgs e)
        {
            // 导航至相应的目标页，并
            // 通过将所需信息作为导航参数传入来配置新页
            //var groupId = ((SampleDataGroup)e.ClickedItem).UniqueId;
            //this.Frame.Navigate(typeof(SplitPage), groupId);
            if (true)	//已登陆
            {
		        var groupId = ((DataGroup)e.ClickedItem).UniqueId;
		        if (groupId == "WiFiSetting")
		        {
                    this.Frame.Navigate(typeof(WifiSettingPage));
		        }
		        else if (groupId == "GuestAccess")
		        {
                    //this.Frame.Navigate(typeof(GuestAccessPage), groupId);
		        }
		        else if (groupId == "NetworkMap")
		        {
                    //this.Frame.Navigate(typeof(NetworkMapPage), groupId);
		        }
		        else if (groupId == "TrafficControl")
		        {
                    //this.Frame.Navigate(typeof(TrafficControlPage), groupId);
		        }
		        else if (groupId == "ParentalControl")
		        {
                    //this.Frame.Navigate(typeof(ParentalControlPage), groupId);		
		        }
		        else if (groupId == "MyMedia")
		        {
                    //this.Frame.Navigate(typeof(MyMediaPage), groupId);
		        }
		        else if (groupId == "MarketPlace")
		        {
			        var uri = new Uri((String)("https://genie.netgear.com/UserProfile/#AppStorePlace:"));
			        await Windows.System.Launcher.LaunchUriAsync(uri);
		        }
	        } 
	        else	//未登录，跳到登陆页面
	        {
                //this.Frame.Navigate(typeof(LoginPage), groupId);
	        }
        }

        private void SearchButton_Click(Object sender, RoutedEventArgs e)
        {
        }
        private void LoginButton_Click(Object sender, RoutedEventArgs e)
        {
            //this.Frame.Navigate(typeof(LoginPage));
        }
        private void LogoutButton_Click(Object sender, RoutedEventArgs e)
        {
        }
        private void AboutButton_Click(Object sender, RoutedEventArgs e)
        {
            if (!AboutPopup.IsOpen)
	        {
		        AboutPopup.IsOpen = true;
		        PopupBackgroundTop.Visibility = Windows.UI.Xaml.Visibility.Visible;
		        PopupBackground.Visibility = Windows.UI.Xaml.Visibility.Visible;
		        CloseAboutButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
		        LicenseButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
	        }
	        if (LicensePopup.IsOpen)
	        {
		        LicensePopup.IsOpen = false;
		        CloseLicenseButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
	        }
        }
        private void CloseAboutButton_Click(Object sender, RoutedEventArgs e)
        {
            if (AboutPopup.IsOpen)
	        {
		        AboutPopup.IsOpen = false;
		        PopupBackgroundTop.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
		        PopupBackground.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
		        CloseAboutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                LicenseButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
	        }
        }
        private void LicenseButton_Click(Object sender, RoutedEventArgs e)
        {
	        if (!LicensePopup.IsOpen)
	        {
		        LicensePopup.IsOpen = true;
		        AboutPopup.IsOpen = false;
		        PopupBackgroundTop.Visibility = Windows.UI.Xaml.Visibility.Visible;
		        PopupBackground.Visibility = Windows.UI.Xaml.Visibility.Visible;
		        CloseLicenseButton.Visibility = Windows.UI.Xaml.Visibility.Visible;
		        CloseAboutButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
                LicenseButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
	        }
        }
        private void CloseLicenseButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LicensePopup.IsOpen)
	        {
		        LicensePopup.IsOpen = false;
		        PopupBackgroundTop.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
		        PopupBackground.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
		        CloseLicenseButton.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
	        }
        }
    }
}
