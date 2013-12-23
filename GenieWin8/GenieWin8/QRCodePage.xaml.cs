﻿using System;
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

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class QRCodePage : GenieWin8.Common.LayoutAwarePage
    {
        private Windows.Media.Capture.MediaCapture mediaCaptureMgr;
        private bool bMediaCaptureInitialized;
        DeviceInformationCollection m_devInfoCollection = null;
        int SelectedCameraIndex = -1;

        public QRCodePage()
        {
            this.InitializeComponent();
        }
        DispatcherTimer timer = new DispatcherTimer();
        //private int _failedCount = 0;

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
            //ScanQRCode();
            EnumedCameraList.Items.Clear();
            m_devInfoCollection = await DeviceInformation.FindAllAsync(DeviceClass.VideoCapture);
            for (int i = 0; i < m_devInfoCollection.Count; i++)
            {
                EnumedCameraList.Items.Add(m_devInfoCollection[i].Name);
            }

            if (EnumedCameraList.Items.Count > 0 && SelectedCameraIndex != -1)
            {
                EnumedCameraList.SelectedIndex = SelectedCameraIndex;
            }
            else if (EnumedCameraList.Items.Count > 0)
            { 
                EnumedCameraList.SelectedIndex = 0;
            }
        }

        //protected override void OnNavigatedTo(NavigationEventArgs e)
        //{
        //    int a = 0;
        //    base.OnNavigatedTo(e);
        //}

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="pageState">要使用可序列化状态填充的空字典。</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        private async void GoBack_Click(Object sender, RoutedEventArgs e)
        {
            timer.Stop();
            if (bMediaCaptureInitialized)
            {
                await mediaCaptureMgr.StopPreviewAsync();
            }
            this.GoHome(null, null);
        }

        private async void OnWindowSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            //timer.Stop();
            try
            {
                mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                var settings = new MediaCaptureInitializationSettings();
                var chosenDevInfo = m_devInfoCollection[EnumedCameraList.SelectedIndex];
                settings.VideoDeviceId = chosenDevInfo.Id;
                await mediaCaptureMgr.InitializeAsync(settings);
                bMediaCaptureInitialized = true;
                captureElement.Source = mediaCaptureMgr;
                await mediaCaptureMgr.StartPreviewAsync();
                if (DisplayProperties.CurrentOrientation == DisplayOrientations.Landscape)
                {
                    mediaCaptureMgr.SetPreviewRotation(VideoRotation.None);
                }
                else if (DisplayProperties.CurrentOrientation == DisplayOrientations.Portrait)
                {
                    if(chosenDevInfo.Name.Contains("Front"))
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
                    }
                    else if(chosenDevInfo.Name.Contains("Back"))
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                    }
                }
                else if (DisplayProperties.CurrentOrientation == DisplayOrientations.LandscapeFlipped)
                {
                    mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise180Degrees);
                }
                else if (DisplayProperties.CurrentOrientation == DisplayOrientations.PortraitFlipped)
                {
                    if (chosenDevInfo.Name.Contains("Front"))
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                    }
                    else if (chosenDevInfo.Name.Contains("Back"))
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
                    }
                }

                timer.Interval = TimeSpan.FromMilliseconds(250);
                timer.Tick += new System.EventHandler<object>(timer_Tick);
                timer.Start();
            }
            catch (Exception exception)
            {
                //this.NotifyUser(exception.Message);
            }
        }

        async void timer_Tick(object sender, object e)
        {
            try
            {
                StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                StorageFile m_photoStorageFile = await storageFolder.CreateFileAsync("qrcode.jpg", CreationCollisionOption.ReplaceExisting);
                ImageEncodingProperties imageProperties = ImageEncodingProperties.CreateJpeg();
                await mediaCaptureMgr.CapturePhotoToStorageFileAsync(imageProperties, m_photoStorageFile);

                IRandomAccessStream stream = await m_photoStorageFile.OpenAsync(FileAccessMode.Read);
                // initialize with 1,1 to get the current size of the image
                var writeableBmp = new WriteableBitmap(1, 1);
                writeableBmp.SetSource(stream);
                // and create it again because otherwise the WB isn't fully initialized and decoding
                // results in a IndexOutOfRange
                writeableBmp = new WriteableBitmap(writeableBmp.PixelWidth, writeableBmp.PixelHeight);
                stream.Seek(0);
                writeableBmp.SetSource(stream);

                var result = ScanBitmap(writeableBmp);
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
                //_failedCount++;
                //this.NotifyUser(exception.Message);
            }
            //finally
            //{
            //    if (_failedCount >= 100)
            //    {
            //        _failedCount = 0;
            //        timer.Stop();
            //        this.NotifyUser("Application Force stop monitor,because it can not grab valid image for 100 consecutive times.");
            //    }
            //}
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

        private async void EnumedCameraList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {

                mediaCaptureMgr = new Windows.Media.Capture.MediaCapture();
                var settings = new MediaCaptureInitializationSettings();
                var chosenDevInfo = m_devInfoCollection[EnumedCameraList.SelectedIndex];
                settings.VideoDeviceId = chosenDevInfo.Id;
                await mediaCaptureMgr.InitializeAsync(settings);
                bMediaCaptureInitialized = true;
                captureElement.Source = mediaCaptureMgr;
                await mediaCaptureMgr.StartPreviewAsync();
                if (DisplayProperties.CurrentOrientation == DisplayOrientations.Landscape)
                {
                    mediaCaptureMgr.SetPreviewRotation(VideoRotation.None);
                }
                else if (DisplayProperties.CurrentOrientation == DisplayOrientations.Portrait)
                {
                    if (chosenDevInfo.Name.Contains("Front"))
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
                    }
                    else if (chosenDevInfo.Name.Contains("Back"))
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                    }
                }
                else if (DisplayProperties.CurrentOrientation == DisplayOrientations.LandscapeFlipped)
                {
                    mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise180Degrees);
                }
                else if (DisplayProperties.CurrentOrientation == DisplayOrientations.PortraitFlipped)
                {
                    if (chosenDevInfo.Name.Contains("Front"))
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise90Degrees);
                    }
                    else if (chosenDevInfo.Name.Contains("Back"))
                    {
                        mediaCaptureMgr.SetPreviewRotation(VideoRotation.Clockwise270Degrees);
                    }
                }
                SelectedCameraIndex = EnumedCameraList.SelectedIndex;
            }
            catch (Exception exception)
            {
                //this.NotifyUser(exception.Message);
            }
        }
    }
}
