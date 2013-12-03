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
using Windows.Networking.Connectivity;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class GuestSettingPage : GenieWin8.Common.LayoutAwarePage
    {
        private static bool IsWifiSsidChanged;
        public GuestSettingPage()
        {
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
        }

        private void App_Resuming(Object sender, Object e)
        {
            //判断所连接Wifi的Ssid是否改变
            IsWifiSsidChanged = true;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        if (connectionProfile.ProfileName == MainPageInfo.ssid)
                            IsWifiSsidChanged = false;
                        else
                            IsWifiSsidChanged = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            //判断保存按钮是否可点击
            if (SSID.Text != "" && ((gridKey.Visibility == Visibility.Visible && Password.Text != "") || gridKey.Visibility == Visibility.Collapsed) &&
                (GuestAccessInfoModel.isSSIDChanged == true || GuestAccessInfoModel.isPasswordChanged == true ||
                GuestAccessInfoModel.isTimePeriodChanged == true || GuestAccessInfoModel.isSecurityTypeChanged == true)
                && IsWifiSsidChanged == false)
            {
                //if (GuestAccessInfoModel.securityType == "None")
                //{
                //    Password.Text = "siteview";
                //}
                GuestSettingSave.IsEnabled = true;
            }
            else
            {
                GuestSettingSave.IsEnabled = false;
            }

            if (GuestAccessInfoModel.isOpenGuestAccess == true && IsWifiSsidChanged == false)
            {
                GuestSettingSave.IsEnabled = true;
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
            var editName = GuestSettingSource.GetEditName((String)navigationParameter);
            this.DefaultViewModel["itemName"] = editName;
            SSID.Text = GuestAccessInfoModel.changedSsid;
            var editKey = GuestSettingSource.GetEditKey((String)navigationParameter);
            this.DefaultViewModel["itemKey"] = editKey;
            Password.Text = GuestAccessInfoModel.changedPassword;
            var timesegSecurity = GuestSettingSource.GetTimesegSecurity((String)navigationParameter);
            this.DefaultViewModel["itemTimesegSecurity"] = timesegSecurity;
            if (GuestAccessInfoModel.changedSecurityType == "None")
            {
                Password.Text = "";
                gridKey.Visibility = Visibility.Collapsed;
            } 
            else
            {
                gridKey.Visibility = Visibility.Visible;
            }

            //判断所连接Wifi的Ssid是否改变
            IsWifiSsidChanged = true;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        if (connectionProfile.ProfileName == MainPageInfo.ssid)
                            IsWifiSsidChanged = false;
                        else
                            IsWifiSsidChanged = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            //判断保存按钮是否可点击
            if (SSID.Text != "" && ((gridKey.Visibility == Visibility.Visible && Password.Text != "") || gridKey.Visibility == Visibility.Collapsed) && 
                (GuestAccessInfoModel.isSSIDChanged == true || GuestAccessInfoModel.isPasswordChanged == true ||
                GuestAccessInfoModel.isTimePeriodChanged == true || GuestAccessInfoModel.isSecurityTypeChanged == true)
                && IsWifiSsidChanged == false)
            {
                //if (GuestAccessInfoModel.securityType == "None")
                //{
                //    Password.Text = "siteview";
                //}
                GuestSettingSave.IsEnabled = true;
            }
            else
            {
                GuestSettingSave.IsEnabled = false;
            }

            if (GuestAccessInfoModel.isOpenGuestAccess == true && IsWifiSsidChanged == false)
            {
                GuestSettingSave.IsEnabled = true;
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
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "GuestAccessPage";
            } 
            else
            {
                if (GuestAccessInfoModel.isOpenGuestAccess == true)
                {
                    GuestSettingSave.IsEnabled = true;
                }
                else
                {
                    string ssid = SSID.Text.Trim();
                    string password = Password.Text.Trim();
                    GuestAccessInfoModel.changedSsid = ssid;
                    if (ssid != GuestAccessInfoModel.ssid && ssid != "" && ((gridKey.Visibility == Visibility.Visible && password != "") || gridKey.Visibility == Visibility.Collapsed))
                    {
                        GuestSettingSave.IsEnabled = true;
                        GuestAccessInfoModel.isSSIDChanged = true;
                    }
                    else if (ssid == "" || (gridKey.Visibility == Visibility.Visible && password == ""))
                    {
                        GuestSettingSave.IsEnabled = false;
                        GuestAccessInfoModel.isSSIDChanged = true;
                    }
                    else
                    {
                        GuestAccessInfoModel.isSSIDChanged = false;
                        if ((gridKey.Visibility == Visibility.Visible && GuestAccessInfoModel.isPasswordChanged == true) || GuestAccessInfoModel.isTimePeriodChanged == true || GuestAccessInfoModel.isSecurityTypeChanged == true)
                        {
                            GuestSettingSave.IsEnabled = true;
                        }
                        else
                        {
                            GuestSettingSave.IsEnabled = false;
                        }
                    }
                }
            }                  
        }

        //判断密码是否更改以及保存按钮是否可点击
        private void pwd_changed(Object sender, RoutedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "GuestAccessPage";
            } 
            else
            {
                if (GuestAccessInfoModel.isOpenGuestAccess == true)
                {
                    GuestSettingSave.IsEnabled = true;
                }
                else
                {
                    string ssid = SSID.Text.Trim();
                    string password = Password.Text.Trim();
                    GuestAccessInfoModel.changedPassword = password;
                    if (password != GuestAccessInfoModel.password && password != "" && ssid != "")
                    {
                        GuestSettingSave.IsEnabled = true;
                        GuestAccessInfoModel.isPasswordChanged = true;
                    }
                    else if (password == "" || ssid == "")
                    {
                        GuestSettingSave.IsEnabled = false;
                        GuestAccessInfoModel.isPasswordChanged = true;
                    }
                    else
                    {
                        GuestAccessInfoModel.isPasswordChanged = false;
                        if (GuestAccessInfoModel.isSSIDChanged == true || GuestAccessInfoModel.isTimePeriodChanged == true || GuestAccessInfoModel.isSecurityTypeChanged == true)
                        {
                            GuestSettingSave.IsEnabled = true;
                        }
                        else
                        {
                            GuestSettingSave.IsEnabled = false;
                        }
                    }
                } 
            }       
        }

        private void TimesegSecurity_ItemClick(Object sender, ItemClickEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "GuestAccessPage";
            } 
            else
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
        }

        private async void GoBack_Click(Object sender, RoutedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(GuestAccessPage));
            } 
            else
            {
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.IsActive = true;
                GenieSoapApi soapApi = new GenieSoapApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                while (dicResponse == null || dicResponse.Count == 0)
                {
                    dicResponse = await soapApi.GetGuestAccessEnabled();
                }
                GuestAccessInfoModel.isGuestAccessEnabled = dicResponse["NewGuestAccessEnabled"];
                if (dicResponse["NewGuestAccessEnabled"] == "0" || dicResponse["NewGuestAccessEnabled"] == "1")
                {
                    Dictionary<string, string> dicResponse1 = new Dictionary<string, string>();
                    while (dicResponse1 == null || dicResponse1.Count == 0)
                    {
                        dicResponse1 = await soapApi.GetGuestAccessNetworkInfo();
                    }
                    if (dicResponse1.Count > 0)
                    {
                        GuestAccessInfoModel.ssid = dicResponse1["NewSSID"];
                        GuestAccessInfoModel.changedSsid = dicResponse1["NewSSID"];
                        GuestAccessInfoModel.securityType = dicResponse1["NewSecurityMode"];
                        GuestAccessInfoModel.changedSecurityType = dicResponse1["NewSecurityMode"];
                        if (dicResponse1["NewSecurityMode"] != "None")
                        {
                            GuestAccessInfoModel.password = dicResponse1["NewKey"];
                            GuestAccessInfoModel.changedPassword = dicResponse1["NewKey"];
                        }
                        else
                        {
                            GuestAccessInfoModel.password = "";
                            GuestAccessInfoModel.changedPassword = "";
                        }
                        if (GuestAccessInfoModel.timePeriod == null)
                        {
                            GuestAccessInfoModel.timePeriod = "Always";
                            GuestAccessInfoModel.changedTimePeriod = "Always";
                        }
                    }
                    GuestAccessInfoModel.isSSIDChanged = false;
                    GuestAccessInfoModel.isPasswordChanged = false;
                    GuestAccessInfoModel.isTimePeriodChanged = false;
                    GuestAccessInfoModel.isSecurityTypeChanged = false;
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    this.Frame.Navigate(typeof(GuestAccessPage));
                }
                else if (dicResponse["NewGuestAccessEnabled"] == "2")
                {
                    GuestAccessInfoModel.isSSIDChanged = false;
                    GuestAccessInfoModel.isPasswordChanged = false;
                    GuestAccessInfoModel.isTimePeriodChanged = false;
                    GuestAccessInfoModel.isSecurityTypeChanged = false;
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    var strtext = loader.GetString("notsupport");
                    var messageDialog = new MessageDialog(strtext);
                    await messageDialog.ShowAsync();
                    this.GoHome(null, null);
                }
            }
        }

        //按下回车键后保存
        //protected override void OnKeyDown(KeyRoutedEventArgs e)
        //{
        //    if (e.Key == Windows.System.VirtualKey.Enter)
        //    {
        //        GuestSettingSave.Focus(FocusState.Keyboard);
        //    }
        //    else
        //    {
        //        base.OnKeyDown(e);
        //    }
        //}

        DispatcherTimer timer = new DispatcherTimer();      //计时器
        private async void GuestSettingSave_Click(object sender, RoutedEventArgs e)
        {
            string ssid = SSID.Text;
            string password = Password.Text;
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
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "GuestAccessPage";
            } 
            else
            {
                InProgress.IsActive = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                GenieSoapApi soapApi = new GenieSoapApi();

                string ssid = SSID.Text.Trim();
                string password = Password.Text.Trim();
                if (ssid != "" && ssid != null)
                {
                    if (GuestAccessInfoModel.changedSecurityType.CompareTo("None") == 0)
                    {
                        GuestAccessInfoModel.password = "";
                        password = "";
                    }
                    timer.Interval = TimeSpan.FromSeconds(1);
                    timer.Tick += new System.EventHandler<object>(timer_Tick);
                    timer.Start();
                    if (GuestAccessInfoModel.isOpenGuestAccess)
                    {
                        await soapApi.SetGuestAccessEnabled2(ssid, GuestAccessInfoModel.changedSecurityType, password);
                    }
                    else
                    {
                        await soapApi.SetGuestAccessNetwork(ssid, GuestAccessInfoModel.changedSecurityType, password);
                    }

                    GuestAccessInfoModel.ssid = GuestAccessInfoModel.changedSsid;
                    GuestAccessInfoModel.password = GuestAccessInfoModel.changedPassword;
                    GuestAccessInfoModel.timePeriod = GuestAccessInfoModel.changedTimePeriod;
                    GuestAccessInfoModel.securityType = GuestAccessInfoModel.changedSecurityType;
                    GuestAccessInfoModel.isSSIDChanged = false;
                    GuestAccessInfoModel.isPasswordChanged = false;
                    GuestAccessInfoModel.isTimePeriodChanged = false;
                    GuestAccessInfoModel.isSecurityTypeChanged = false;
                    GuestSettingSave.IsEnabled = false;
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
