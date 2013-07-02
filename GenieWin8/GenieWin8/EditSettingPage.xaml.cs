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
    public sealed partial class EditSettingPage : GenieWin8.Common.LayoutAwarePage
    {
        public EditSettingPage()
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
            var editName = SettingSource.GetEditName((String)navigationParameter);
	        this.DefaultViewModel["itemName"] = editName;
	        var editKey = SettingSource.GetEditKey((String)navigationParameter);
	        this.DefaultViewModel["itemKey"] = editKey;
            var channelsecurity = SettingSource.GetChannelSecurity((String)navigationParameter);
	        this.DefaultViewModel["itemChannelSecurity"] = channelsecurity;
            if (WifiInfoModel.changedSecurityType == "None")
            {
                gridKey.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridKey.Visibility = Visibility.Visible;
            }

            //判断保存按钮是否可点击
            if (WifiInfoModel.isChannelChanged == true || WifiInfoModel.isSecurityTypeChanged == true)
            {
                wifiSettingSave.IsEnabled = true;
            }
            else
            {
                wifiSettingSave.IsEnabled = false;
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

        //判断SSID是否更改以及保存按钮是否可点击
        private void ssid_changed(Object sender, RoutedEventArgs e)
        {
            string ssid = SSID.Text.Trim();
            if (ssid != WifiInfoModel.ssid)
            {
                wifiSettingSave.IsEnabled = true;
                WifiInfoModel.isSSIDChanged = true;                
            }
            else
            {
                WifiInfoModel.isSSIDChanged = false;
                if (WifiInfoModel.isPasswordChanged == true || WifiInfoModel.isChannelChanged == true || WifiInfoModel.isSecurityTypeChanged == true)
                {
                    wifiSettingSave.IsEnabled = true;
                } 
                else
                {
                    wifiSettingSave.IsEnabled = false;
                }               
            }
        }

        //判断密码是否更改以及保存按钮是否可点击
        private void pwd_changed(Object sender, RoutedEventArgs e)
        {
            string password = pwd.Text.Trim();
            if (password != WifiInfoModel.password)
            {
                wifiSettingSave.IsEnabled = true;
                WifiInfoModel.isPasswordChanged = true;
            }
            else
            {
                WifiInfoModel.isPasswordChanged = false;
                if (WifiInfoModel.isSSIDChanged == true || WifiInfoModel.isChannelChanged == true || WifiInfoModel.isSecurityTypeChanged == true)
                {
                    wifiSettingSave.IsEnabled = true;
                }
                else
                {
                    wifiSettingSave.IsEnabled = false;
                }
            }
        }

        private void ChannelSecurity_ItemClick(Object sender, ItemClickEventArgs e)
        {
            var groupId = ((SettingGroup)e.ClickedItem).UniqueId;
	        if (groupId == "Channel")
	        {
                this.Frame.Navigate(typeof(EditChannelPage), groupId);
	        } 
	        else if(groupId == "Security")
	        {
                this.Frame.Navigate(typeof(EditSecurityPage), groupId);
	        }
        }

        private async void GoBack_Click(Object sender, RoutedEventArgs e)
        {
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetInfo("WLANConfiguration");
            WifiInfoModel.ssid = dicResponse["NewSSID"];
            WifiInfoModel.channel = dicResponse["NewChannel"];
            WifiInfoModel.changedChannel = dicResponse["NewChannel"];
            WifiInfoModel.securityType = dicResponse["NewWPAEncryptionModes"];
            WifiInfoModel.changedSecurityType = dicResponse["NewWPAEncryptionModes"];
            dicResponse = await soapApi.GetWPASecurityKeys();
            WifiInfoModel.password = dicResponse["NewWPAPassphrase"];
            WifiInfoModel.isSSIDChanged = false;
            WifiInfoModel.isPasswordChanged = false;
            WifiInfoModel.isChannelChanged = false;
            WifiInfoModel.isSecurityTypeChanged = false;
            this.Frame.Navigate(typeof(WifiSettingPage));
        }

        private async void WifiSettingSave_Click(object sender, RoutedEventArgs e)
        {
            GenieSoapApi soapApi = new GenieSoapApi();

            string ssid = SSID.Text.Trim();
            string password = pwd.Text.Trim();
            if (ssid != "" && ssid != null)
            {
                if (WifiInfoModel.securityType.CompareTo("None") == 0)
                {
                    await soapApi.SetWLANNoSecurity(ssid, WifiInfoModel.region, WifiInfoModel.changedChannel, WifiInfoModel.wirelessMode);
                    WifiInfoModel.channel = WifiInfoModel.changedChannel;
                }
                else
                {
                    await soapApi.SetWLANWEPByPassphrase(ssid, WifiInfoModel.region, WifiInfoModel.changedChannel, WifiInfoModel.wirelessMode, WifiInfoModel.changedSecurityType, password);
                    WifiInfoModel.securityType = WifiInfoModel.changedSecurityType;
                }
            }
        }
    }
}
