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

using Windows.UI.Xaml.Media.Imaging;
using ZXing;
using ZXing.Common;

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
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            InProgress.IsActive = true;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetGuestAccessEnabled();
            GuestAccessInfoModel.isGuestAccessEnabled = dicResponse["NewGuestAccessEnabled"];
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
            dicResponse = await soapApi.GetGuestAccessNetworkInfo();
            if (dicResponse.Count > 0)
            {
                GuestAccessInfoModel.ssid = dicResponse["NewSSID"];
                GuestAccessInfoModel.changedSsid = dicResponse["NewSSID"];
                GuestAccessInfoModel.securityType = dicResponse["NewSecurityMode"];
                GuestAccessInfoModel.changedSecurityType = dicResponse["NewSecurityMode"];
                if (dicResponse["NewSecurityMode"] != "None")
                {
                    GuestAccessInfoModel.password = dicResponse["NewKey"];
                    GuestAccessInfoModel.changedPassword = dicResponse["NewKey"];
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
            var GuestSettingGroup = GuestSettingSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = GuestSettingGroup;

            //生成二维码
            string codeString = "WIRELESS:" + GuestAccessInfoModel.ssid + ";PASSWORD:" + GuestAccessInfoModel.password;
            WriteableBitmap wb = CreateBarcode(codeString);
            imageQRCode.Source = wb;

            InProgress.IsActive = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
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

        private async void checkGuestSetting_Click(Object sender, RoutedEventArgs e)
        {
            // Create the message dialog and set its content
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
            var strtext = loader.GetString("wirelsssetting");
            var messageDialog = new MessageDialog(strtext);

            // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
            messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.CommandInvokedHandlerYes)));
            messageDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(this.CommandInvokedHandlerNo)));

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
        private async void CommandInvokedHandlerYes(IUICommand command)
        {
            InProgress.IsActive = true;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            if (checkGuestSetting.IsChecked == true)
            {
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await soapApi.GetGuestAccessNetworkInfo();
                if (dicResponse.Count > 0)
                {
                    GuestAccessInfoModel.ssid = dicResponse["NewSSID"];
                    GuestAccessInfoModel.changedSsid = dicResponse["NewSSID"];
                    GuestAccessInfoModel.securityType = dicResponse["NewSecurityMode"];
                    GuestAccessInfoModel.changedSecurityType = dicResponse["NewSecurityMode"];
                    if (dicResponse["NewSecurityMode"] != "None")
                    {
                        GuestAccessInfoModel.password = dicResponse["NewKey"];
                        GuestAccessInfoModel.changedPassword = dicResponse["NewKey"];
                    }
                    else
                    {
                        GuestAccessInfoModel.password = "";
                        GuestAccessInfoModel.changedPassword = "";
                    }
                }
                dicResponse = await soapApi.SetGuestAccessEnabled2(GuestAccessInfoModel.ssid, GuestAccessInfoModel.securityType, GuestAccessInfoModel.password);
                GuestSettingsList.Visibility = Visibility.Visible;
                textScanQRCode.Visibility = Visibility.Visible;
                imageQRCode.Visibility = Visibility.Visible;
            }
            else if (checkGuestSetting.IsChecked == false)
            {
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await soapApi.SetGuestAccessEnabled();
                GuestSettingsList.Visibility = Visibility.Collapsed;
                textScanQRCode.Visibility = Visibility.Collapsed;
                imageQRCode.Visibility = Visibility.Collapsed;
            }
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

        private void CommandInvokedHandlerNo(IUICommand command)
        {
            if (checkGuestSetting.IsChecked == true)
            {
                checkGuestSetting.IsChecked = false;
            }
            else if (checkGuestSetting.IsChecked == false)
            {
                checkGuestSetting.IsChecked = true;
            }           
        }
        #endregion

        private void Refresh_Click(Object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GuestAccessPage));
        }

        //生成二维码
        public static WriteableBitmap CreateBarcode(string content)
        {
            IBarcodeWriter wt = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 200,
                    Width = 200
                }
            };
            var bmp = wt.Write(content);
            return bmp;
        }
    }
}
