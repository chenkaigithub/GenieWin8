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
using Windows.UI.Popups;

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
            SSID.Text = WifiInfoModel.changedSsid;
	        var editKey = SettingSource.GetEditKey((String)navigationParameter);
	        this.DefaultViewModel["itemKey"] = editKey;
            pwd.Text = WifiInfoModel.changedPassword;
            var channelsecurity = SettingSource.GetChannelSecurity((String)navigationParameter);
	        this.DefaultViewModel["itemChannelSecurity"] = channelsecurity;
            if (WifiInfoModel.changedSecurityType == "None")
            {
                pwd.Text = "";
                gridKey.Visibility = Visibility.Collapsed;
            }
            else
            {
                gridKey.Visibility = Visibility.Visible;
            }

            //判断保存按钮是否可点击
            if (SSID.Text != "" && ((gridKey.Visibility == Visibility.Visible && pwd.Text != "") || gridKey.Visibility == Visibility.Collapsed) && 
                (WifiInfoModel.isSSIDChanged == true || WifiInfoModel.isPasswordChanged == true || WifiInfoModel.isChannelChanged == true || WifiInfoModel.isSecurityTypeChanged == true))
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
            string password = pwd.Text.Trim();
            WifiInfoModel.changedSsid = ssid;
            if (ssid != WifiInfoModel.ssid && ssid != "" && ((gridKey.Visibility == Visibility.Visible && password != "") || gridKey.Visibility == Visibility.Collapsed))
            {
                wifiSettingSave.IsEnabled = true;
                WifiInfoModel.isSSIDChanged = true;                
            }
            else if (ssid == "" || (gridKey.Visibility == Visibility.Visible && password == ""))
            {
                wifiSettingSave.IsEnabled = false;
                WifiInfoModel.isSSIDChanged = true; 
            }
            else
            {
                WifiInfoModel.isSSIDChanged = false;
                if ((gridKey.Visibility == Visibility.Visible && WifiInfoModel.isPasswordChanged == true) || WifiInfoModel.isChannelChanged == true || WifiInfoModel.isSecurityTypeChanged == true)
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
            string ssid = SSID.Text.Trim();
            string password = pwd.Text.Trim();
            WifiInfoModel.changedPassword = password;
            if (password != WifiInfoModel.password && password != "" && ssid != "")
            {
                wifiSettingSave.IsEnabled = true;
                WifiInfoModel.isPasswordChanged = true;
            }
            else if (password == "" || ssid == "")
            {
                wifiSettingSave.IsEnabled = false;
                WifiInfoModel.isSSIDChanged = true;
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
            InProgress.IsActive = true;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.GetInfo("WLANConfiguration");
            }
            if (dicResponse.Count > 0)
            {
                WifiInfoModel.ssid = dicResponse["NewSSID"];
                WifiInfoModel.changedSsid = dicResponse["NewSSID"];
                WifiInfoModel.channel = dicResponse["NewChannel"];
                WifiInfoModel.changedChannel = dicResponse["NewChannel"];
                WifiInfoModel.securityType = dicResponse["NewWPAEncryptionModes"];
                WifiInfoModel.changedSecurityType = dicResponse["NewWPAEncryptionModes"];
            }
            dicResponse = new Dictionary<string, string>();
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.GetWPASecurityKeys();
            }
            if (dicResponse.Count > 0)
            {
                WifiInfoModel.password = dicResponse["NewWPAPassphrase"];
                WifiInfoModel.changedPassword = dicResponse["NewWPAPassphrase"];
            }            
            WifiInfoModel.isSSIDChanged = false;
            WifiInfoModel.isPasswordChanged = false;
            WifiInfoModel.isChannelChanged = false;
            WifiInfoModel.isSecurityTypeChanged = false;
            InProgress.IsActive = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
            this.Frame.Navigate(typeof(WifiSettingPage));
        }

        //按下回车键后保存
        //protected override void OnKeyDown(KeyRoutedEventArgs e)
        //{
        //    if (e.Key == Windows.System.VirtualKey.Enter)
        //    {
        //        wifiSettingSave.Focus(FocusState.Keyboard);
        //    }
        //    else
        //    {
        //        base.OnKeyDown(e);
        //    }
        //}

        DispatcherTimer timer = new DispatcherTimer();      //计时器
        private async void WifiSettingSave_Click(object sender, RoutedEventArgs e)
        {
            string ssid = SSID.Text;
            string password = pwd.Text;
            bool IsSsidSBC = IsAllowChar(ssid);
            bool IsPasswordSBC = IsAllowChar(password);
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            if (IsSsidSBC)
            {
                var messageDialog = new MessageDialog(loader.GetString("DisallowedSSIDChar"));
                await messageDialog.ShowAsync();
            }
            else if (gridKey.Visibility == Visibility.Visible && password.Length < 8 || password.Length > 64)
            {
                var messageDialog = new MessageDialog(loader.GetString("MsgPasswordFormat"));
                await messageDialog.ShowAsync();
            }
            else if (password.Length == 64)
            {
                bool ret = false;           //ret为true，表示密码符合64位十六进制数字，反之则为false
                char[] ch = password.ToCharArray();
                for (int i = 0; i < ch.Length; i++)
                {
                    if ((ch[i] > 47 && ch[i] < 58) || (ch[i] > 64 && ch[i] < 71))
                        ret = true;
                }
                if (!ret)
                {
                    var messageDialog = new MessageDialog(loader.GetString("DisallowedPasswordChar"));
                    await messageDialog.ShowAsync();
                }
            }
            else if (IsPasswordSBC)
            {
                var messageDialog = new MessageDialog(loader.GetString("DisallowedPasswordChar"));
                await messageDialog.ShowAsync();
            }
            else
            {
                // Create the message dialog and set its content
                //var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var strtext = loader.GetString("wirelsssetting");
                var messageDialog = new MessageDialog(strtext);

                // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.CommandInvokedHandler)));
                messageDialog.Commands.Add(new UICommand("No", null));

                // Set the command that will be invoked by default
                messageDialog.DefaultCommandIndex = 0;

                // Set the command to be invoked when escape is pressed
                messageDialog.CancelCommandIndex = 1;

                // Show the message dialog
                await messageDialog.ShowAsync();
            }           
        }

        #region Commands
        /// <summary>
        /// Callback function for the invocation of the dialog commands.
        /// </summary>
        /// <param name="command">The command that was invoked.</param>
        private async void CommandInvokedHandler(IUICommand command)
        {
            InProgress.IsActive = true;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();

            string ssid = SSID.Text.Trim();
            string password = pwd.Text.Trim();
            if (ssid != "" && ssid != null)
            {
                if (WifiInfoModel.changedSecurityType.CompareTo("None") == 0)
                {
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += new System.EventHandler<object>(timer_Tick);
                    timer.Start();
                    await soapApi.SetWLANNoSecurity(ssid, WifiInfoModel.region, WifiInfoModel.changedChannel, WifiInfoModel.wirelessMode);
                    WifiInfoModel.ssid = WifiInfoModel.changedSsid;
                    WifiInfoModel.password = WifiInfoModel.changedPassword;
                    WifiInfoModel.channel = WifiInfoModel.changedChannel;
                    WifiInfoModel.securityType = WifiInfoModel.changedSecurityType;
                    WifiInfoModel.isSSIDChanged = false;
                    WifiInfoModel.isPasswordChanged = false;
                    WifiInfoModel.isChannelChanged = false;
                    WifiInfoModel.isSecurityTypeChanged = false;
                    wifiSettingSave.IsEnabled = false;
                }
                else
                {
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += new System.EventHandler<object>(timer_Tick);
                    timer.Start();
                    await soapApi.SetWLANWEPByPassphrase(ssid, WifiInfoModel.region, WifiInfoModel.changedChannel, WifiInfoModel.wirelessMode, WifiInfoModel.changedSecurityType, password);
                    WifiInfoModel.ssid = WifiInfoModel.changedSsid;
                    WifiInfoModel.password = WifiInfoModel.changedPassword;
                    WifiInfoModel.channel = WifiInfoModel.changedChannel;
                    WifiInfoModel.securityType = WifiInfoModel.changedSecurityType;
                    WifiInfoModel.isSSIDChanged = false;
                    WifiInfoModel.isPasswordChanged = false;
                    WifiInfoModel.isChannelChanged = false;
                    WifiInfoModel.isSecurityTypeChanged = false;
                    wifiSettingSave.IsEnabled = false;
                }
            }
        }
        #endregion

        int count = 60;     //倒计时间
        async void timer_Tick(object sender, object e)
        {
            waittime.Text = count.ToString();
            count--;
            if (count < 0)
            {
                timer.Stop();
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var strtext = loader.GetString("wirelesssettinrelogin");
                var messageDialog = new MessageDialog(strtext);
                await messageDialog.ShowAsync();
                MainPageInfo.bLogin = false;
                this.GoHome(null, null);
            }
        }

        //通过ASCII码值判断输入字符串是否有不允许字符
        private static bool IsAllowChar(string input)
        {
            if (input == "" || input == null)
                return false;
            bool ret = false;
            char[] ch = input.ToCharArray();
            for (int i = 0; i < ch.Length; i++)
            {
                if (ch[i] < 33 || ch[i] > 126)
                    ret = true;
            }
            return ret;
        }
    }
}
