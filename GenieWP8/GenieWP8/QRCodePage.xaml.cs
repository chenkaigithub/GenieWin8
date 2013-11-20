using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using GenieWP8.Resources;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Devices;
using ZXing;
using ZXing.QrCode;
using ZXing.Common;
using System.Windows.Threading;
using System.Windows.Media.Imaging;

namespace GenieWP8
{
    public partial class QRCodePage : PhoneApplicationPage
    {
        private PhotoCamera _photoCamera;
        private PhotoCameraLuminanceSource _luminance;
        private readonly DispatcherTimer _timer;
        //解码器
        private Reader _reader = null;

        public QRCodePage()
        {
            InitializeComponent();

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(250);
            //间隔250ms调用读取函数
            _timer.Tick += (o, arg) => ScanPreviewBuffer();
        }

        //用于生成本地化 ApplicationBar 的代码
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            //后退按钮
            ApplicationBarIconButton appBarButton_back = new ApplicationBarIconButton(new Uri("Assets/back.png", UriKind.Relative));
            appBarButton_back.Text = AppResources.btnBack;
            ApplicationBar.Buttons.Add(appBarButton_back);
            appBarButton_back.Click += new EventHandler(appBarButton_back_Click);
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            _reader = new QRCodeReader();
            _photoCamera = new PhotoCamera();
            _photoCamera.Initialized += new EventHandler<CameraOperationCompletedEventArgs>(cam_Initialized);
            _videoBrush.SetSource(_photoCamera);
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatingFrom(System.Windows.Navigation.NavigatingCancelEventArgs e)
        {
            if (_photoCamera != null)
            {
                _timer.Stop();
                _photoCamera.CancelFocus();
                _photoCamera.Dispose();
            }

            base.OnNavigatingFrom(e);
        }

        void cam_Initialized(object sender, CameraOperationCompletedEventArgs e)
        {
            int width = Convert.ToInt32(_photoCamera.PreviewResolution.Width);
            int height = Convert.ToInt32(_photoCamera.PreviewResolution.Height);
            _luminance = new PhotoCameraLuminanceSource(width, height);

            Dispatcher.BeginInvoke(() =>
            {
                _previewTransform.Rotation = _photoCamera.Orientation;
                _timer.Start();
            });
            _photoCamera.FlashMode = FlashMode.Auto;
            _photoCamera.Focus();
        }

        private void ScanPreviewBuffer()
        {
            try
            {
                _photoCamera.GetPreviewBufferY(_luminance.PreviewBufferY);
                var binarizer = new HybridBinarizer(_luminance);
                var binBitmap = new BinaryBitmap(binarizer);
                Result result = _reader.decode(binBitmap);
                if (result != null)
                {
                    //_timer.Stop();
                    Dispatcher.BeginInvoke(() =>
                    {
                        //读取成功，结果存放在content
                        string content = result.Text;
                        string[] decode = content.Split(';');
                        if (decode.Length >= 2)
                        {
                            string[] ssidString = decode[0].Split(':');
                            string[] passwordString = decode[1].Split(':');                                                       
                            if (ssidString.Length >= 2 && ssidString[0] == "WIRELESS" && passwordString.Length >= 2 && passwordString[0] == "PASSWORD")
                            {
                                string ssid = ssidString[1];
                                string password = passwordString[1];
                                Clipboard.SetText(password);
                                MessageBox.Show(AppResources.WiFiName + "：" + ssid + "\r\n" + AppResources.PasswordText + "：" + password + "\r\n" + AppResources.CopyToClipboard);      //由于API未开放，不能自动进行无线连接，暂以MessageBox显示之
                            }
                        }                                                
                    });
                }
                else
                {
                    _photoCamera.Focus();
                }
            }
            catch
            {
            }
        }

        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            if (_photoCamera != null)
            {
                _timer.Stop();
                _photoCamera.CancelFocus();
                _photoCamera.Dispose();
            }
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //重写手机“返回”按钮事件
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            if (_photoCamera != null)
            {
                _timer.Stop();
                _photoCamera.CancelFocus();
                _photoCamera.Dispose();
            }
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }
    }
}