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
using Windows.UI.Xaml.Media.Imaging;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Graphics.Imaging;
using ZXing;
using Windows.ApplicationModel.DataTransfer;
using Windows.UI.Popups;
using Windows.Graphics.Display;
using Windows.Devices.Enumeration;
using Windows.ApplicationModel;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class QRCodePage : GenieWin8.Common.LayoutAwarePage
    {
        private Windows.Media.Capture.MediaCapture mediaCaptureMgr;
        private bool bMediaCaptureStartedPreview = false;
        DeviceInformationCollection m_devInfoCollection = null;
        private EventHandler<Object> m_soundLevelHandler;
        //int SelectedCameraIndex = -1;

        public QRCodePage()
        {
            this.InitializeComponent();
            ScenarioReset();
            m_soundLevelHandler = new EventHandler<Object>(MediaControl_SoundLevelChanged);
            //Application.Current.Suspending += new SuspendingEventHandler(App_Suspending);
            //Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
        }
        DispatcherTimer timer = new DispatcherTimer();
        //private int _failedCount = 0;

        //private async void App_Suspending(Object sender, SuspendingEventArgs e)
        //{
        //    timer.Stop();
        //    if (bMediaCaptureStartedPreview)
        //    {
        //        await mediaCaptureMgr.StopPreviewAsync();
        //        bMediaCaptureStartedPreview = false;
        //    }
        //}

        //private async void App_Resuming(Object sender, Object e)
        //{
        //    m_devInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
        //    if (m_devInfoCollection.Count < 1)
        //    {
        //        WarnNoCamera.Text = "No camera found!";
        //    }
        //    else
        //    {
        //        try
        //        {
        //            mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
        //            var settings = new MediaCaptureInitializationSettings();
        //            if (m_devInfoCollection.Count == 1)
        //            {
        //                settings.VideoDeviceId = m_devInfoCollection[0].Id; // 0 => front, 1 => back
        //            }
        //            else
        //            {
        //                settings.VideoDeviceId = m_devInfoCollection[1].Id; // 0 => front, 1 => back
        //            }
        //            await mediaCaptureMgr.InitializeAsync(settings);
        //            captureElement.Source = mediaCaptureMgr;
        //            await mediaCaptureMgr.StartPreviewAsync();
        //            bMediaCaptureStartedPreview = true;

        //            timer.Interval = TimeSpan.FromMilliseconds(250);
        //            timer.Tick += new System.EventHandler<object>(timer_Tick);
        //            timer.Start();
        //        }
        //        catch (Exception exception)
        //        {
        //        }
        //    }
        //}

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            Windows.Media.MediaControl.SoundLevelChanged += m_soundLevelHandler;
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            Windows.Media.MediaControl.SoundLevelChanged -= m_soundLevelHandler;
        }

        private async void MediaControl_SoundLevelChanged(object sender, Object e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                try
                {
                    if (Windows.Media.MediaControl.SoundLevel != Windows.Media.SoundLevel.Muted)
                    {
                        ScenarioReset();
                    }
                    else
                    {
                        if (bMediaCaptureStartedPreview)
                        {
                            timer.Stop();
                            await mediaCaptureMgr.StopPreviewAsync();
                            bMediaCaptureStartedPreview = false;
                            btnStartPreview.IsEnabled = true;
                            captureElement.Source = null;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            });
        }

        private async void GoBack_Click(Object sender, RoutedEventArgs e)
        {
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, async () =>
            {
                if (bMediaCaptureStartedPreview)
                {
                    timer.Stop();
                    await mediaCaptureMgr.StopPreviewAsync();
                    bMediaCaptureStartedPreview = false;
                    btnStartPreview.IsEnabled = true;
                    captureElement.Source = null;
                }
                this.GoHome(null, null);
            });
        }

        private void OnWindowSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            if (bMediaCaptureStartedPreview)
            {
                if (DisplayProperties.CurrentOrientation == DisplayOrientations.Landscape)
                {
                    mediaCaptureMgr.SetPreviewRotation(VideoRotation.None);
                }
                else if (DisplayProperties.CurrentOrientation == DisplayOrientations.Portrait)
                {
                    mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                }
                else if (DisplayProperties.CurrentOrientation == DisplayOrientations.LandscapeFlipped)
                {
                    mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise180Degrees);
                }
                else if (DisplayProperties.CurrentOrientation == DisplayOrientations.PortraitFlipped)
                {
                    mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
                }
            }
        }

        private void ScenarioReset()
        {
            btnStartPreview.IsEnabled = true;
            captureElement.Source = null;
        }

        internal async void btnStartPreview_Click(Object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            bMediaCaptureStartedPreview = false;
            m_devInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            if (m_devInfoCollection.Count < 1)
            {
                WarnNoCamera.Text = "No camera found!";
            }
            else
            {
                try
                {
                    mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                    var settings = new MediaCaptureInitializationSettings();
                    if (m_devInfoCollection.Count == 1)
                    {
                        settings.VideoDeviceId = m_devInfoCollection[0].Id; // 0 => front, 1 => back
                    }
                    else
                    {
                        settings.VideoDeviceId = m_devInfoCollection[1].Id; // 0 => front, 1 => back
                    }
                    await mediaCaptureMgr.InitializeAsync(settings);

                    btnStartPreview.IsEnabled = false;
                    captureElement.Source = mediaCaptureMgr;
                    await mediaCaptureMgr.StartPreviewAsync();
                    bMediaCaptureStartedPreview = true;

                    if (DisplayProperties.CurrentOrientation == DisplayOrientations.Landscape)
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.None);
                    }
                    else if (DisplayProperties.CurrentOrientation == DisplayOrientations.Portrait)
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                    }
                    else if (DisplayProperties.CurrentOrientation == DisplayOrientations.LandscapeFlipped)
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise180Degrees);
                    }
                    else if (DisplayProperties.CurrentOrientation == DisplayOrientations.PortraitFlipped)
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
                    }

                    timer.Interval = TimeSpan.FromMilliseconds(250);
                    timer.Tick += new System.EventHandler<object>(timer_Tick);
                    timer.Start();
                }
                catch (Exception exception)
                {
                    bMediaCaptureStartedPreview = false;
                    captureElement.Source = null;
                    btnStartPreview.IsEnabled = true;
                }
            }
        }

        async void timer_Tick(object sender, object e)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile m_photoStorageFile = await storageFolder.CreateFileAsync("qrcode.jpg", CreationCollisionOption.GenerateUniqueName);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, m_photoStorageFile);

                IRandomAccessStream stream = await m_photoStorageFile.OpenReadAsync();
                // initialize with 1,1 to get the current size of the image
                var writeableBmp = new WriteableBitmap(1, 1);
                writeableBmp.SetSource(stream);
                // and create it again because otherwise the WB isn't fully initialized and decoding
                // results in a IndexOutOfRange
                writeableBmp = new WriteableBitmap(writeableBmp.PixelWidth, writeableBmp.PixelHeight);
                stream.Seek(0);
                writeableBmp.SetSource(stream);

                var result = ScanBitmap(writeableBmp);
                await m_photoStorageFile.DeleteAsync(StorageDeleteOption.PermanentDelete);
                if (result != null)
                {
                    string decodeString = result.Text;
                    string[] decode = decodeString.Split(';');
                    if (decode.Length >= 2)
                    {
                        string[] ssidString = decode[0].Split(':');
                        string[] passwordString = decode[1].Split(':');
                        if (ssidString.Length >= 2 && ssidString[0] == "WIRELESS" && passwordString.Length >= 2 && passwordString[0] == "PASSWORD")
                        {
                            string ssid = ssidString[1];
                            string password = passwordString[1];
                            var dataPackage = new DataPackage();
                            dataPackage.SetText(password);
                            Clipboard.SetContent(dataPackage);
                            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                            var strWiFiName = loader.GetString("WiFiName");
                            var strPassword = loader.GetString("Key/Password");
                            var strCopyToClipboard = loader.GetString("CopyToClipboard");
                            var messageDialog = new MessageDialog(strWiFiName + "：" + ssid + "\r\n" + strPassword + "：" + password + "\r\n" + strCopyToClipboard);
                            await messageDialog.ShowAsync();
                        }
                    } 
                }
            }
            catch (Exception exception)
            {
            }
        }

        private Result ScanBitmap(WriteableBitmap writeableBmp)
        {
            var barcodeReader = new BarcodeReader
            {
                TryHarder = true,
                AutoRotate = true
            };
            var result = barcodeReader.Decode(writeableBmp);
            return result;
        }
    }
}
