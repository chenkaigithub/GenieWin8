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
    public sealed partial class GuestSettingPage : GenieWin8.Common.LayoutAwarePage
    {
        public GuestSettingPage()
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
            var editName = GuestSettingSource.GetEditName((String)navigationParameter);
            this.DefaultViewModel["itemName"] = editName;
            var editKey = GuestSettingSource.GetEditKey((String)navigationParameter);
            this.DefaultViewModel["itemKey"] = editKey;
            var timesegSecurity = GuestSettingSource.GetTimesegSecurity((String)navigationParameter);
            this.DefaultViewModel["itemTimesegSecurity"] = timesegSecurity;
            if (GuestAccessInfoModel.securityType == "None")
            {
                gridKey.Visibility = Visibility.Collapsed;
            } 
            else
            {
                gridKey.Visibility = Visibility.Visible;
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

        private void TimesegSecurity_ItemClick(Object sender, ItemClickEventArgs e)
        {
            var groupId = ((GuestSettingGroup)e.ClickedItem).UniqueId;
            if (groupId == "TimeSegment")
            {
                this.Frame.Navigate(typeof(GuestTimeSegPage), groupId);
            }
            else if (groupId == "Security")
            {
                this.Frame.Navigate(typeof(GuestSecurityPage), groupId);
            }
        }

        private async void GoBack_Click(Object sender, RoutedEventArgs e)
        {
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetGuestAccessNetworkInfo();
            GuestAccessInfoModel.ssid = dicResponse["NewSSID"];
            GuestAccessInfoModel.securityType = dicResponse["NewSecurityMode"];
            if (dicResponse["NewSecurityMode"] != "None")
                GuestAccessInfoModel.password = dicResponse["NewKey"];
            else
                GuestAccessInfoModel.password = "";
            if (GuestAccessInfoModel.timePeriod == null)
                GuestAccessInfoModel.timePeriod = "Always";
            this.Frame.Navigate(typeof(GuestAccessPage));
        }

        private async void GuestSettingSave_Click(object sender, RoutedEventArgs e)
        {
            GenieSoapApi soapApi = new GenieSoapApi();

            string ssid = SSID.Text.Trim();
            string password = Password.Text.Trim();
            if (ssid != "" && ssid != null)
            {
                if (GuestAccessInfoModel.securityType.CompareTo("None") == 0)
                {
                    GuestAccessInfoModel.password = "";
                    password = "";
                }
                await soapApi.SetGuestAccessNetwork(ssid, GuestAccessInfoModel.securityType, password);
            }
        }
    }
}
