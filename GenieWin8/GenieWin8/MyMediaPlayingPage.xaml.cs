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
using Windows.UI;

using Windows.Media.PlayTo;
using Windows.UI.Core;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Security.ExchangeActiveSyncProvisioning;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class MyMediaPlayingPage : GenieWin8.Common.LayoutAwarePage
    {
        PlayToReceiver receiver = null;
        bool IsReceiverStarted = false;
        bool IsSeeking = false;
        double bufferedPlaybackRate = 0;
        bool justLoadedMedia = false;
        bool IsPlayReceivedPreMediaLoaded = false;
        enum MediaType { None, Image, AudioVideo };
        MediaType currentType = MediaType.None;
        BitmapImage imagerevd = null;

        bool IsFullScreen = false;
        private Size _previousmediasize;
        public Size PreviousMediaSize
        {
            get { return _previousmediasize; }
            set { _previousmediasize = value; }
        }
        private Thickness _previousmediaelementmargin;
        private Thickness _previousScrollViewerMargin;
        private Brush _previousBGColor;
        private DispatcherTimer _timer;
        private bool _sliderpressed = false;

        public MyMediaPlayingPage()
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
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="pageState">要使用可序列化状态填充的空字典。</param>
        protected override async void SaveState(Dictionary<String, Object> pageState)
        {
            if (IsReceiverStarted)
            {
                await receiver.StopAsync();
                IsReceiverStarted = false;
            }
        }

        /// <summary>
        /// This is the click handler for the 'Default' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void startPlayToReceiver(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                try
                {
                    InitialisePlayToReceiver();
                    startDMRButton.IsEnabled = false;
                    stopDMRButton.IsEnabled = true;
                    await receiver.StartAsync();
                    IsReceiverStarted = true;
                    //rootPage.NotifyUser("PlayToReceiver started", NotifyType.StatusMessage);
                    StatusNotify.Text = "Player started to receive";
                }
                catch (Exception ecp)
                {
                    IsReceiverStarted = false;
                    startDMRButton.IsEnabled = true;
                    stopDMRButton.IsEnabled = false;
                    //rootPage.NotifyUser("PlayToReceiver start failed, Error " + ecp.Message, NotifyType.ErrorMessage);
                    StatusNotify.Text = "Player start failed, Error " + ecp.Message;
                }
            }
        }

        /// <summary>
        /// This is the click handler for the 'Other' button.  You would replace this with your own handler
        /// if you have a button or buttons on this page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        async private void stopPlayToReceiver(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                try
                {
                    startDMRButton.IsEnabled = true;
                    stopDMRButton.IsEnabled = false;
                    await receiver.StopAsync();
                    IsReceiverStarted = false;
                    //rootPage.NotifyUser("PlayToReceiver stopped", NotifyType.StatusMessage);
                    StatusNotify.Text = "Player stopped to receive";
                }
                catch (Exception ecp)
                {
                    IsReceiverStarted = true;
                    startDMRButton.IsEnabled = false;
                    stopDMRButton.IsEnabled = true;
                    //rootPage.NotifyUser("PlayToReceiver stop failed, Error " + ecp.Message, NotifyType.ErrorMessage);
                    StatusNotify.Text = "Player stop failed, Error " + ecp.Message;
                }
            }
        }

        private void InitialisePlayToReceiver()
        {
            try
            {
                if (receiver == null)
                {
                    receiver = new PlayToReceiver();
                    receiver.PlayRequested += new TypedEventHandler<PlayToReceiver, object>(receiver_PlayRequested);
                    receiver.PauseRequested += new TypedEventHandler<PlayToReceiver, object>(receiver_PauseRequested);
                    receiver.StopRequested += new TypedEventHandler<PlayToReceiver, object>(receiver_StopRequested);
                    receiver.TimeUpdateRequested += new TypedEventHandler<PlayToReceiver, object>(receiver_TimeUpdateRequested);
                    receiver.CurrentTimeChangeRequested += new TypedEventHandler<PlayToReceiver, CurrentTimeChangeRequestedEventArgs>(receiver_CurrentTimeChangeRequested);
                    receiver.SourceChangeRequested += new TypedEventHandler<PlayToReceiver, SourceChangeRequestedEventArgs>(receiver_SourceChangeRequested);
                    receiver.MuteChangeRequested += new TypedEventHandler<PlayToReceiver, MuteChangeRequestedEventArgs>(receiver_MuteChangeRequested);
                    receiver.PlaybackRateChangeRequested += new TypedEventHandler<PlayToReceiver, PlaybackRateChangeRequestedEventArgs>(receiver_PlaybackRateChangeRequested);
                    receiver.VolumeChangeRequested += new TypedEventHandler<PlayToReceiver, VolumeChangeRequestedEventArgs>(receiver_VolumeChangeRequested);

                    receiver.SupportsAudio = true;
                    receiver.SupportsVideo = true;
                    receiver.SupportsImage = true;

                    //receiver.FriendlyName = "SDK CS Sample PlayToReceiver";
                    EasClientDeviceInformation easClientDeviceInformation = new EasClientDeviceInformation();
                    receiver.FriendlyName = "Genie Media Player (" + easClientDeviceInformation.FriendlyName + ")";

                    timelineSlider.ValueChanged += timelineSlider_ValueChanged;
                    timelineSlider1.ValueChanged += timelineSlider_ValueChanged;

                    PointerEventHandler pointerpressedhandler = new PointerEventHandler(slider_PointerEntered);
                    timelineSlider.AddHandler(Control.PointerPressedEvent, pointerpressedhandler, true);
                    timelineSlider1.AddHandler(Control.PointerPressedEvent, pointerpressedhandler, true);

                    PointerEventHandler pointerreleasedhandler = new PointerEventHandler(slider_PointerCaptureLost);
                    timelineSlider.AddHandler(Control.PointerCaptureLostEvent, pointerreleasedhandler, true);
                    timelineSlider1.AddHandler(Control.PointerCaptureLostEvent, pointerreleasedhandler, true);
                }
            }
            catch (Exception e)
            {
                startDMRButton.IsEnabled = false;
                stopDMRButton.IsEnabled = true;
                //rootPage.NotifyUser("PlayToReceiver initialization failed, Error: " + e.Message, NotifyType.ErrorMessage);
                StatusNotify.Text = "Player initialization failed, Error: " + e.Message;
            }
        }

        private async void receiver_PlayRequested(PlayToReceiver recv, Object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (dmrVideo != null && currentType == MediaType.AudioVideo)
                {
                    IsPlayReceivedPreMediaLoaded = true;
                    dmrVideo.Play();
                    playButton.IsEnabled = false;
                    pauseButton.IsEnabled = true;
                    stopButton.IsEnabled = true;

                    playButton1.IsEnabled = false;
                    pauseButton1.IsEnabled = true;
                    stopButton1.IsEnabled = true;

                    double absvalue = (int)Math.Round(dmrVideo.NaturalDuration.TimeSpan.TotalSeconds, MidpointRounding.AwayFromZero);
                    timelineSlider.Maximum = absvalue;
                    timelineSlider1.Maximum = absvalue;

                    timelineSlider.StepFrequency = SliderFrequency(dmrVideo.NaturalDuration.TimeSpan);
                    timelineSlider1.StepFrequency = SliderFrequency(dmrVideo.NaturalDuration.TimeSpan);
                    SetupTimer();
                }
                else if (currentType == MediaType.Image)
                {
                    dmrImage.Source = imagerevd;
                    receiver.NotifyPlaying();
                    playButton.IsEnabled = false;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;

                    playButton1.IsEnabled = false;
                    pauseButton1.IsEnabled = false;
                    stopButton1.IsEnabled = false;
                }
            });
        }

        private async void receiver_PauseRequested(PlayToReceiver recv, Object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (dmrVideo != null && currentType == MediaType.AudioVideo)
                {
                    if (dmrVideo.CurrentState == MediaElementState.Stopped)
                    {
                        receiver.NotifyPaused();
                        playButton.IsEnabled = true;
                        pauseButton.IsEnabled = false;
                        stopButton.IsEnabled = false;

                        playButton1.IsEnabled = true;
                        pauseButton1.IsEnabled = false;
                        stopButton1.IsEnabled = false;
                    }
                    else
                    {
                        dmrVideo.Pause();
                        playButton.IsEnabled = true;
                        pauseButton.IsEnabled = false;
                        stopButton.IsEnabled = true;

                        playButton1.IsEnabled = true;
                        pauseButton1.IsEnabled = false;
                        stopButton1.IsEnabled = true;
                    }
                }
            });
        }

        private async void receiver_StopRequested(PlayToReceiver recv, Object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (dmrVideo != null && currentType == MediaType.AudioVideo)
                {
                    dmrVideo.Stop();
                    receiver.NotifyStopped();
                    playButton.IsEnabled = true;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;

                    playButton1.IsEnabled = true;
                    pauseButton1.IsEnabled = false;
                    stopButton1.IsEnabled = false;
                }
                else if (dmrImage != null && currentType == MediaType.Image)
                {
                    dmrImage.Source = null;
                    receiver.NotifyStopped();
                    playButton.IsEnabled = false;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;

                    playButton1.IsEnabled = false;
                    pauseButton1.IsEnabled = false;
                    stopButton1.IsEnabled = false;
                }
            });
        }

        private async void receiver_TimeUpdateRequested(PlayToReceiver recv, Object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (IsReceiverStarted)
                {
                    if (dmrVideo != null && currentType == MediaType.AudioVideo)
                    {
                        receiver.NotifyTimeUpdate(dmrVideo.Position);
                    }
                    else if (dmrImage != null && currentType == MediaType.Image)
                    {
                        receiver.NotifyTimeUpdate(new TimeSpan(0));
                    }
                }
            });
        }

        private async void receiver_CurrentTimeChangeRequested(PlayToReceiver recv, CurrentTimeChangeRequestedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (IsReceiverStarted)
                {
                    if (dmrVideo != null && currentType == MediaType.AudioVideo)
                    {
                        if (dmrVideo.CanSeek)
                        {
                            dmrVideo.Position = args.Time;
                            receiver.NotifySeeking();
                            IsSeeking = true;
                        }
                    }
                    else if (currentType == MediaType.Image)
                    {
                        receiver.NotifySeeking();
                        receiver.NotifySeeked();
                    }
                }
            });
        }

        private async void receiver_SourceChangeRequested(PlayToReceiver recv, SourceChangeRequestedEventArgs args)
        {
            IsPlayReceivedPreMediaLoaded = false;
            if (args.Stream == null)
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    if (currentType == MediaType.AudioVideo && dmrVideo != null)
                    {
                        if (this.IsFullScreen)
                            FullscreenToggle();
                        dmrVideo.Stop();
                    }
                    else if (currentType == MediaType.Image && dmrImage != null)
                    {
                        if (this.IsFullScreen)
                            FullscreenToggle();
                        dmrImage.Source = null;
                        dmrImage.Opacity = 0;
                    }
                    currentType = MediaType.None;
                    Title.Text = "";
                    Title1.Text = "";
                    playButton.IsEnabled = false;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;

                    playButton1.IsEnabled = false;
                    pauseButton1.IsEnabled = false;
                    stopButton1.IsEnabled = false;

                    timelineSlider.Visibility = Visibility.Collapsed;
                    timelineSlider1.Visibility = Visibility.Collapsed;
                });
            }
            else if (args.Stream.ContentType.Contains("image"))
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    imagerevd = new BitmapImage();
                    imagerevd.ImageOpened += imagerevd_ImageOpened;
                    imagerevd.SetSource(args.Stream);
                    if (currentType != MediaType.Image)
                    {
                        if (currentType == MediaType.AudioVideo)
                        {
                            if (this.IsFullScreen)
                                FullscreenToggle();
                            dmrVideo.Stop();
                        }
                        dmrImage.Opacity = 1;
                        dmrVideo.Opacity = 0;
                    }
                    currentType = MediaType.Image;
                    Title.Text = args.Title;
                    Title1.Text = args.Title;
                    playButton.IsEnabled = false;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;

                    playButton1.IsEnabled = false;
                    pauseButton1.IsEnabled = false;
                    stopButton1.IsEnabled = false;

                    timelineSlider.Visibility = Visibility.Collapsed;
                    timelineSlider1.Visibility = Visibility.Collapsed;
                });
            }
            else
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
                {
                    try
                    {
                        justLoadedMedia = true;
                        dmrVideo.SetSource(args.Stream, args.Stream.ContentType);
                    }
                    catch (Exception exp)
                    {
                        //rootPage.NotifyUser(exp.Message + " Content Type: " + args.Stream.ContentType, NotifyType.ErrorMessage);
                        StatusNotify.Text = exp.Message + " Content Type: " + args.Stream.ContentType;
                    }

                    if (currentType == MediaType.Image)
                    {
                        if (this.IsFullScreen)
                            FullscreenToggle();
                        dmrImage.Opacity = 0;
                        dmrVideo.Opacity = 1;
                        dmrImage.Source = null;
                    }
                    currentType = MediaType.AudioVideo;
                    Title.Text = args.Title;
                    Title1.Text = args.Title;
                    playButton.IsEnabled = true;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;

                    playButton1.IsEnabled = true;
                    pauseButton1.IsEnabled = false;
                    stopButton1.IsEnabled = false;

                    timelineSlider.Visibility = Visibility.Visible;
                    timelineSlider1.Visibility = Visibility.Visible;
                });
            }
        }

        void imagerevd_ImageOpened(object sender, RoutedEventArgs e)
        {
            receiver.NotifyLoadedMetadata();
        }

        private async void receiver_MuteChangeRequested(PlayToReceiver recv, MuteChangeRequestedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (dmrVideo != null && currentType == MediaType.AudioVideo)
                {
                    dmrVideo.IsMuted = args.Mute;
                }
                else if (dmrImage != null && currentType == MediaType.Image)
                {
                    receiver.NotifyVolumeChange(0, args.Mute);
                }
            });
        }

        private async void receiver_PlaybackRateChangeRequested(PlayToReceiver recv, PlaybackRateChangeRequestedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (dmrVideo != null && currentType == MediaType.AudioVideo)
                {
                    if (dmrVideo.CurrentState != MediaElementState.Opening && dmrVideo.CurrentState != MediaElementState.Closed)
                    {
                        dmrVideo.PlaybackRate = args.Rate;
                    }
                    else
                    {
                        bufferedPlaybackRate = args.Rate;
                    }
                }
            });
        }

        private async void receiver_VolumeChangeRequested(PlayToReceiver recv, VolumeChangeRequestedEventArgs args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (dmrVideo != null && currentType == MediaType.AudioVideo)
                {
                    dmrVideo.Volume = args.Volume;
                }
            });
        }

        private void dmrVideo_VolumeChanged(object sender, RoutedEventArgs e)
        {
            if (IsReceiverStarted)
            {
                receiver.NotifyVolumeChange(dmrVideo.Volume, dmrVideo.IsMuted);
            }
        }

        private void dmrVideo_RateChanged(object sender, Windows.UI.Xaml.Media.RateChangedRoutedEventArgs e)
        {
            if (IsReceiverStarted)
            {
                receiver.NotifyRateChange(dmrVideo.PlaybackRate);
            }

        }

        private void dmrVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (IsReceiverStarted)
            {
                receiver.NotifyLoadedMetadata();
                receiver.NotifyDurationChange(dmrVideo.NaturalDuration.TimeSpan);
                if (IsPlayReceivedPreMediaLoaded == true)
                {
                    dmrVideo.Play();
                }
            }
        }

        private void dmrVideo_CurrentStateChanged(object sender, RoutedEventArgs e)
        {
            if (IsReceiverStarted)
            {
                switch (dmrVideo.CurrentState)
                {
                    case MediaElementState.Playing:
                        receiver.NotifyPlaying();
                        if (_sliderpressed)
                        {
                            _timer.Stop();
                        }
                        else
                        {
                            _timer.Start();
                        }
                        break;
                    case MediaElementState.Paused:
                        if (justLoadedMedia)
                        {
                            receiver.NotifyStopped();
                            justLoadedMedia = false;
                        }
                        else
                        {
                            receiver.NotifyPaused();
                            _timer.Stop();
                        }
                        break;
                    case MediaElementState.Stopped:
                        receiver.NotifyStopped();
                        _timer.Stop();
                        timelineSlider.Value = 0.0;
                        timelineSlider1.Value = 0.0;
                        break;
                    default:
                        break;
                }
            }
        }

        private void dmrVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (IsReceiverStarted)
            {
                StopTimer();
                timelineSlider.Value = 0.0;
                timelineSlider1.Value = 0.0;

                receiver.NotifyEnded();
                if (dmrVideo != null)
                    dmrVideo.Stop();
                playButton.IsEnabled = true;
                pauseButton.IsEnabled = false;
                stopButton.IsEnabled = false;

                playButton1.IsEnabled = true;
                pauseButton1.IsEnabled = false;
                stopButton1.IsEnabled = false;
            }
        }

        private void dmrVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (IsReceiverStarted)
            {
                receiver.NotifyError();
                playButton.IsEnabled = false;
                pauseButton.IsEnabled = false;
                stopButton.IsEnabled = false;

                playButton1.IsEnabled = false;
                pauseButton1.IsEnabled = false;
                stopButton1.IsEnabled = false;
            }
        }

        private void dmrVideo_SeekCompleted(object sender, RoutedEventArgs e)
        {
            if (IsReceiverStarted)
            {
                try
                {
                    if (!IsSeeking)
                    {
                        receiver.NotifySeeking();
                    }
                    receiver.NotifySeeked();
                    IsSeeking = false;
                }
                catch (InvalidOperationException exp)
                {
                    //rootPage.NotifyUser(exp.Message, NotifyType.ErrorMessage);
                    StatusNotify.Text = exp.Message;
                }
            }
        }

        private void dmrVideo_DownloadProgressChanged_1(object sender, RoutedEventArgs e)
        {
            if (dmrVideo.DownloadProgress == 1 && bufferedPlaybackRate > 0)
            {
                dmrVideo.PlaybackRate = bufferedPlaybackRate;
                bufferedPlaybackRate = 0;
            }
        }

        private void dmrImage_ImageFailed_1(object sender, ExceptionRoutedEventArgs e)
        {
            receiver.NotifyError();
            playButton.IsEnabled = false;
            pauseButton.IsEnabled = false;
            stopButton.IsEnabled = false;

            playButton1.IsEnabled = false;
            pauseButton1.IsEnabled = false;
            stopButton1.IsEnabled = false;
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            if (dmrVideo != null && currentType == MediaType.AudioVideo)
            {
                IsPlayReceivedPreMediaLoaded = true;
                dmrVideo.Play();
                playButton.IsEnabled = false;
                pauseButton.IsEnabled = true;
                stopButton.IsEnabled = true;

                playButton1.IsEnabled = false;
                pauseButton1.IsEnabled = true;
                stopButton1.IsEnabled = true;

                SetupTimer();
            }
            else if (currentType == MediaType.Image)
            {
                dmrImage.Source = imagerevd;
                receiver.NotifyPlaying();
                playButton.IsEnabled = false;
                pauseButton.IsEnabled = false;
                stopButton.IsEnabled = false;

                playButton1.IsEnabled = false;
                pauseButton1.IsEnabled = false;
                stopButton1.IsEnabled = false;
            }
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (dmrVideo != null && currentType == MediaType.AudioVideo)
            {
                if (dmrVideo.CurrentState == MediaElementState.Stopped)
                {
                    receiver.NotifyPaused();
                    playButton.IsEnabled = true;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;

                    playButton1.IsEnabled = true;
                    pauseButton1.IsEnabled = false;
                    stopButton1.IsEnabled = false;
                }
                else
                {
                    dmrVideo.Pause();
                    playButton.IsEnabled = true;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = true;

                    playButton1.IsEnabled = true;
                    pauseButton1.IsEnabled = false;
                    stopButton1.IsEnabled = true;
                }
            }
        }

        private void stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (dmrVideo != null && currentType == MediaType.AudioVideo)
            {
                dmrVideo.Stop();
                receiver.NotifyStopped();
                playButton.IsEnabled = true;
                pauseButton.IsEnabled = false;
                stopButton.IsEnabled = false;

                playButton1.IsEnabled = true;
                pauseButton1.IsEnabled = false;
                stopButton1.IsEnabled = false;
            }
            else if (dmrImage != null && currentType == MediaType.Image)
            {
                dmrImage.Source = null;
                receiver.NotifyStopped();
                playButton.IsEnabled = false;
                pauseButton.IsEnabled = false;
                stopButton.IsEnabled = false;

                playButton1.IsEnabled = false;
                pauseButton1.IsEnabled = false;
                stopButton1.IsEnabled = false;
            }
        }

        private void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            FullscreenToggle();
        }

        private void StackPanel_KeyDown(object sender, KeyRoutedEventArgs e)
        {
            if (IsFullScreen && e.Key == Windows.System.VirtualKey.Escape)
                FullscreenToggle();
            e.Handled = true;
        }

        private void FullscreenToggle()
        {
            this.IsFullScreen = !this.IsFullScreen;

            if (this.IsFullScreen)
            {
                Header.Visibility = Visibility.Collapsed;
                
                _previousmediaelementmargin = LayoutRoot.Margin;

                MediaScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Hidden;
                MediaScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Hidden;

                ControlDMRPanel.Visibility = Visibility.Collapsed;
                StatusNotify.Visibility = Visibility.Collapsed;
                ControlMediaPanel.Visibility = Visibility.Collapsed;
                Title.Visibility = Visibility.Collapsed;

                _previousBGColor = LayoutRoot.Background;
                LayoutRoot.Margin = new Thickness();
                LayoutRoot.Background = new SolidColorBrush(Colors.Black);

                _previousScrollViewerMargin = MediaScrollViewer.Margin;
                MediaScrollViewer.Margin = new Thickness(0);

                BottomAppBar.Visibility = Visibility.Visible;

                if (dmrVideo != null && currentType == MediaType.AudioVideo)
                {
                    timelineSlider.Visibility = Visibility.Collapsed;
                    timelineSlider1.Visibility = Visibility.Visible;
                    dmrImage.Visibility = Visibility.Collapsed;
                    _previousmediasize.Height = dmrVideo.Height;
                    _previousmediasize.Width = dmrVideo.Width;

                    dmrVideo.Width = Window.Current.Bounds.Width;
                    dmrVideo.Height = Window.Current.Bounds.Height;
                }
                else if (dmrImage != null && currentType == MediaType.Image)
                {
                    dmrVideo.Visibility = Visibility.Collapsed;
                    _previousmediasize.Height = dmrImage.Height;
                    _previousmediasize.Width = dmrImage.Width;

                    dmrImage.Width = Window.Current.Bounds.Width;
                    dmrImage.Height = Window.Current.Bounds.Height;
                }
            }
            else
            {
                Header.Visibility = Visibility.Visible;

                MediaScrollViewer.HorizontalScrollBarVisibility = ScrollBarVisibility.Auto;
                MediaScrollViewer.VerticalScrollBarVisibility = ScrollBarVisibility.Auto;

                ControlDMRPanel.Visibility = Visibility.Visible;
                StatusNotify.Visibility = Visibility.Visible;
                ControlMediaPanel.Visibility = Visibility.Visible;
                Title.Visibility = Visibility.Visible;

                LayoutRoot.Background = _previousBGColor;
                LayoutRoot.Margin = _previousmediaelementmargin;

                MediaScrollViewer.Margin = _previousScrollViewerMargin;

                BottomAppBar.Visibility = Visibility.Collapsed;

                if (dmrVideo != null && currentType == MediaType.AudioVideo)
                {
                    timelineSlider.Visibility = Visibility.Visible;
                    timelineSlider1.Visibility = Visibility.Collapsed;
                    dmrImage.Visibility = Visibility.Visible;
                    dmrVideo.Width = _previousmediasize.Width;
                    dmrVideo.Height = _previousmediasize.Height;
                }
                else if (dmrImage != null && currentType == MediaType.Image)
                {
                    dmrVideo.Visibility = Visibility.Visible;
                    dmrImage.Width = _previousmediasize.Width;
                    dmrImage.Height = _previousmediasize.Height;
                }
            }
        }

        void timelineSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Slider old value = {0} new value = {1}.", e.OldValue, e.NewValue);
            if (!_sliderpressed)
            {
                dmrVideo.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        void slider_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Pointer entered event fired");
            _sliderpressed = true;
        }

        void slider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            //System.Diagnostics.Debug.WriteLine("Pointer capture lost event fired");
            //System.Diagnostics.Debug.WriteLine("Slider value at capture lost {0}", timelineSlider.Value);
            //dmrVideo.PlaybackRate = 1;
            if (!this.IsFullScreen)
            {
                dmrVideo.Position = TimeSpan.FromSeconds(timelineSlider.Value);
            }
            else
            {
                dmrVideo.Position = TimeSpan.FromSeconds(timelineSlider1.Value);
            }
            _sliderpressed = false;
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(timelineSlider.StepFrequency);
            StartTimer();
        }

        private void _timer_Tick(object sender, object e)
        {
            if (!_sliderpressed)
            {
                timelineSlider.Value = dmrVideo.Position.TotalSeconds;
                timelineSlider1.Value = dmrVideo.Position.TotalSeconds;
            }
        }

        private void StartTimer()
        {
            _timer.Tick += _timer_Tick;
            _timer.Start();
        }

        private void StopTimer()
        {
            _timer.Stop();
            _timer.Tick -= _timer_Tick;
        }

        private double SliderFrequency(TimeSpan timevalue)
        {
            double stepfrequency = -1;

            double absvalue = (int)Math.Round(timevalue.TotalSeconds, MidpointRounding.AwayFromZero);
            stepfrequency = (int)(Math.Round(absvalue / 100));

            if (timevalue.TotalMinutes >= 10 && timevalue.TotalMinutes < 30)
            {
                stepfrequency = 10;
            }
            else if (timevalue.TotalMinutes >= 30 && timevalue.TotalMinutes < 60)
            {
                stepfrequency = 30;
            }
            else if (timevalue.TotalHours >= 1)
            {
                stepfrequency = 60;
            }

            if (stepfrequency == 0) stepfrequency += 1;

            if (stepfrequency == 1)
            {
                stepfrequency = absvalue / 100;
            }

            return stepfrequency;
        }
    }
}
