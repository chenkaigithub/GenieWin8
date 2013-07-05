﻿using GenieWin8.Data;

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
            if (GuestAccessInfoModel.changedSecurityType == "None")
            {
                gridKey.Visibility = Visibility.Collapsed;
            } 
            else
            {
                gridKey.Visibility = Visibility.Visible;
            }

            //判断保存按钮是否可点击
            if (GuestAccessInfoModel.isTimePeriodChanged == true || GuestAccessInfoModel.isSecurityTypeChanged == true)
            {
                GuestSettingSave.IsEnabled = true;
            }
            else
            {
                GuestSettingSave.IsEnabled = false;
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
            if (ssid != GuestAccessInfoModel.ssid && ssid != "")
            {
                GuestSettingSave.IsEnabled = true;
                GuestAccessInfoModel.isSSIDChanged = true;
            }
            else
            {
                GuestAccessInfoModel.isSSIDChanged = false;
                if (GuestAccessInfoModel.isPasswordChanged == true || GuestAccessInfoModel.isTimePeriodChanged == true || GuestAccessInfoModel.isSecurityTypeChanged == true)
                {
                    GuestSettingSave.IsEnabled = true;
                }
                else
                {
                    GuestSettingSave.IsEnabled = false;
                }
            }
        }

        //判断密码是否更改以及保存按钮是否可点击
        private void pwd_changed(Object sender, RoutedEventArgs e)
        {
            string password = Password.Text.Trim();
            if (password != GuestAccessInfoModel.password && password != "")
            {
                GuestSettingSave.IsEnabled = true;
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

        private void GoBack_Click(Object sender, RoutedEventArgs e)
        {
            GuestAccessInfoModel.isSSIDChanged = false;
            GuestAccessInfoModel.isPasswordChanged = false;
            GuestAccessInfoModel.isTimePeriodChanged = false;
            GuestAccessInfoModel.isSecurityTypeChanged = false;
            this.Frame.Navigate(typeof(GuestAccessPage));
        }

        DispatcherTimer timer = new DispatcherTimer();      //计时器
        private async void GuestSettingSave_Click(object sender, RoutedEventArgs e)
        {           
            // Create the message dialog and set its content
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
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
                await soapApi.SetGuestAccessNetwork(ssid, GuestAccessInfoModel.changedSecurityType, password);
                GuestAccessInfoModel.timePeriod = GuestAccessInfoModel.changedTimePeriod;
                GuestAccessInfoModel.securityType = GuestAccessInfoModel.changedSecurityType;
                GuestAccessInfoModel.isSSIDChanged = false;
                GuestAccessInfoModel.isPasswordChanged = false;
                GuestAccessInfoModel.isTimePeriodChanged = false;
                GuestAccessInfoModel.isSecurityTypeChanged = false;
                GuestSettingSave.IsEnabled = false;
            }
        }
        #endregion

        int count = 90;     //倒计时间
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
                //此处导航回到主页还存在问题，待解决。
                //this.Frame.Navigate(typeof(MainPage));
            }
        }
    }
}
