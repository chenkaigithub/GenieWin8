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
                GuestAccessInfoModel.securityType = dicResponse["NewSecurityMode"];
                if (dicResponse["NewSecurityMode"] != "None")
                    GuestAccessInfoModel.password = dicResponse["NewKey"];
                else
                    GuestAccessInfoModel.password = "";
                if (GuestAccessInfoModel.timePeriod == null)
                {
                    GuestAccessInfoModel.timePeriod = "Always";
                    GuestAccessInfoModel.changedTimePeriod = "Always";
                }
            }            
            var GuestSettingGroup = GuestSettingSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = GuestSettingGroup;

            //生成二维码（未完成，暂时注释）
            //ThoughtWorks.QRCode.Codec.QRCodeEncoder _qrCodeEncoder = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
            //_qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //_qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            //_qrCodeEncoder.QRCodeVersion = 0;
            //_qrCodeEncoder.QRCodeScale = 7;
            //string text = "Test text for QRcode";
            //QRCodeBitmapImage image = _qrCodeEncoder.Encode(text, System.Text.Encoding.UTF8);
            //WriteableBitmap wb = new WriteableBitmap(image.Width, image.Height);
            //ThoughtWorks.QRCode.Utilities.WriteableBitmapFromArray(wb, image.ImageByteArray);
            //StorageFile file = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync("Genie_QRCode.png", CreationCollisionOption.ReplaceExisting);
            //IRandomAccessStream fileStream = await file.OpenAsync(FileAccessMode.ReadWrite);
            //BitmapEncoder encoder = await BitmapEncoder.CreateAsync(BitmapEncoder.PngEncoderId, fileStream);
            //Stream pixelStream = wb.PixelBuffer.AsStream();
            //byte[] pixels = new byte[pixelStream.Length];
            //pixelStream.Read(pixels, 0, pixels.Length);
            ////pixel format should convert to rgba8 
            //for (int i = 0; i < pixels.Length; i += 4)
            //{
            //    byte temp = pixels[i];
            //    pixels[i] = pixels[i + 2];
            //    pixels[i + 2] = temp;
            //}

            //encoder.SetPixelData(
            //  BitmapPixelFormat.Rgba8,
            //  BitmapAlphaMode.Straight,
            //  (uint)wb.PixelWidth,
            //  (uint)wb.PixelHeight,
            //  96, // Horizontal DPI 
            //  96, // Vertical DPI 
            //  pixels
            //  );
            //await encoder.FlushAsync();
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
            if (checkGuestSetting.IsChecked == true)
            {           
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await soapApi.GetGuestAccessNetworkInfo();
                if (dicResponse.Count > 0)
                {
                    GuestAccessInfoModel.ssid = dicResponse["NewSSID"];
                    GuestAccessInfoModel.securityType = dicResponse["NewSecurityMode"];
                    GuestAccessInfoModel.changedSecurityType = dicResponse["NewSecurityMode"];
                    if (dicResponse["NewSecurityMode"] != "None")
                        GuestAccessInfoModel.password = dicResponse["NewKey"];
                    else
                        GuestAccessInfoModel.password = "";
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
            //此处导航回到主页还存在问题，待解决。
            //this.Frame.Navigate(typeof(MainPage));
        }
        #endregion

        private void Refresh_Click(Object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(GuestAccessPage));
        }
    }
}
