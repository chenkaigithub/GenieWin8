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

using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.Codec.Util;
using Windows.UI.Xaml.Media.Imaging;
//using System.Runtime.InteropServices.WindowsRuntime;
//using Windows.Graphics.Imaging;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class WifiSettingPage : GenieWin8.Common.LayoutAwarePage
    {
        public WifiSettingPage()
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
            var SettingGroup = SettingSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = SettingGroup;

            //生成二维码(未完成，暂时注释)
            //ThoughtWorks.QRCode.Codec.QRCodeEncoder _qrCodeEncoder = new ThoughtWorks.QRCode.Codec.QRCodeEncoder();
            //_qrCodeEncoder.QRCodeEncodeMode = QRCodeEncoder.ENCODE_MODE.BYTE;
            //_qrCodeEncoder.QRCodeErrorCorrect = QRCodeEncoder.ERROR_CORRECTION.Q;
            //_qrCodeEncoder.QRCodeVersion = 0;
            //_qrCodeEncoder.QRCodeScale = 7;
            //string text = "Test text for QRcode";
            //QRCodeBitmapImage image = _qrCodeEncoder.Encode(text, System.Text.Encoding.UTF8);
            //WriteableBitmap wb = new WriteableBitmap(image.Width, image.Height);
            //ThoughtWorks.QRCode.Utilities.WriteableBitmapFromArray(wb, image.ImageByteArray);
            //imageQRCode.Source = wb;

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

        private void Setting_ItemClick(Object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(EditSettingPage));
        }

        private async void Refresh_Click(Object sender, RoutedEventArgs e)
        {
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetInfo("WLANConfiguration");
            if (dicResponse.Count > 0)
            {
                WifiInfoModel.ssid = dicResponse["NewSSID"];
                WifiInfoModel.channel = dicResponse["NewChannel"];
                WifiInfoModel.changedChannel = dicResponse["NewChannel"];
                WifiInfoModel.securityType = dicResponse["NewWPAEncryptionModes"];
                WifiInfoModel.changedSecurityType = dicResponse["NewWPAEncryptionModes"];
            }           
            dicResponse = await soapApi.GetWPASecurityKeys();
            if (dicResponse.Count > 0)
            {
                WifiInfoModel.password = dicResponse["NewWPAPassphrase"];
            }            
            this.Frame.Navigate(typeof(WifiSettingPage));
        }
    }
}
