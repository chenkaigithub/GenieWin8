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

using Windows.Storage;
using Windows.Storage.Streams;
using Windows.Media.MediaProperties;
using Windows.Media.Capture;
using ThoughtWorks.QRCode.Codec;
using ThoughtWorks.QRCode.Codec.Data;
using ThoughtWorks.QRCode.Codec.Util;
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class QRCodePage : GenieWin8.Common.LayoutAwarePage
    {
        private Windows.Media.Capture.MediaCapture mediaCaptureMgr;

        public QRCodePage()
        {
            this.InitializeComponent();
        }
        DispatcherTimer timer = new DispatcherTimer();
        private int _failedCount = 0;
        private int _doCount = 0;

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
            ScanQRCode();
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

        async void ScanQRCode()
        {
            try
            {
                mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                await mediaCaptureMgr.InitializeAsync();

                captureElement.Source = mediaCaptureMgr;
                await mediaCaptureMgr.StartPreviewAsync();

                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += new System.EventHandler<object>(timer_Tick);
                timer.Start();
            }
            catch (Exception exception)
            {
                this.NotifyUser(exception.Message);
            }
        }

        async void timer_Tick(object sender, object e)
        {
            _doCount++;
            try
            {
                Windows.Storage.StorageFile m_photoStorageFile = await Windows.Storage.KnownFolders.PicturesLibrary.CreateFileAsync("qrcode.jpg", CreationCollisionOption.ReplaceExisting);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, m_photoStorageFile);

                IRandomAccessStream stream = await m_photoStorageFile.OpenAsync(FileAccessMode.Read);
                BitmapDecoder decoder = await BitmapDecoder.CreateAsync(BitmapDecoder.PngDecoderId, stream);
                int width = (int)decoder.PixelWidth;
                int height = (int)decoder.PixelHeight;
                WriteableBitmap wb = new WriteableBitmap(width, height);
                wb.SetSource(stream);
                QRCodeDecoder qrCodeDecoder = new QRCodeDecoder();
                QRCodeBitmapImage _image = new QRCodeBitmapImage(wb.PixelBuffer.ToArray(), wb.PixelWidth, wb.PixelHeight);
                string decodeString = qrCodeDecoder.decode(_image, System.Text.Encoding.UTF8);            //decodeString为解码得到的字符串
                timer.Stop();
                //var photoStream = await m_photoStorageFile.OpenAsync(Windows.Storage.FileAccessMode.Read);
                //var bmpimg = new BitmapImage();

                //bmpimg.SetSource(photoStream);
                //imageElement1.Source = bmpimg;
            }
            catch (Exception exception)
            {
                _failedCount++;
                this.NotifyUser(exception.Message);
            }
            finally
            {
                if (_failedCount >= 100)
                {
                    _failedCount = 0;
                    timer.Stop();
                    this.NotifyUser("Application Force stop monitor,because it can not grab valid image for 100 consecutive times.");
                }
            }
        }

        public void NotifyUser(string strMessage)
        {
            StatusBlock.Text = strMessage;

            // Collapse the StatusBlock if it has no text to conserve real estate.
            if (StatusBlock.Text != String.Empty)
            {
                StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Visible;
            }
            else
            {
                StatusBlock.Visibility = Windows.UI.Xaml.Visibility.Collapsed;
            }
        }
    }
}
