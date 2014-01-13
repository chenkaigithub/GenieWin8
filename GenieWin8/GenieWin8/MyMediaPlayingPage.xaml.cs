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
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
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
                    StatusNotify.Text = "PlayToReceiver started";
                }
                catch (Exception ecp)
                {
                    IsReceiverStarted = false;
                    startDMRButton.IsEnabled = true;
                    stopDMRButton.IsEnabled = false;
                    //rootPage.NotifyUser("PlayToReceiver start failed, Error " + ecp.Message, NotifyType.ErrorMessage);
                    StatusNotify.Text = "PlayToReceiver start failed, Error " + ecp.Message;
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
                    StatusNotify.Text = "PlayToReceiver stopped";
                }
                catch (Exception ecp)
                {
                    IsReceiverStarted = true;
                    startDMRButton.IsEnabled = false;
                    stopDMRButton.IsEnabled = true;
                    //rootPage.NotifyUser("PlayToReceiver stop failed, Error " + ecp.Message, NotifyType.ErrorMessage);
                    StatusNotify.Text = "PlayToReceiver stop failed, Error " + ecp.Message;
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
                }
            }
            catch (Exception e)
            {
                startDMRButton.IsEnabled = false;
                stopDMRButton.IsEnabled = true;
                //rootPage.NotifyUser("PlayToReceiver initialization failed, Error: " + e.Message, NotifyType.ErrorMessage);
                StatusNotify.Text = "PlayToReceiver initialization failed, Error: " + e.Message;
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
                }
                else if (currentType == MediaType.Image)
                {
                    dmrImage.Source = imagerevd;
                    receiver.NotifyPlaying();
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
                    }
                    else
                    {
                        dmrVideo.Pause();
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
                }
                else if (dmrImage != null && currentType == MediaType.Image)
                {
                    dmrImage.Source = null;
                    receiver.NotifyStopped();
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
                        dmrVideo.Stop();
                    }
                    else if (currentType == MediaType.Image && dmrImage != null)
                    {
                        dmrImage.Source = null;
                        dmrImage.Opacity = 0;
                    }
                    currentType = MediaType.None;
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
                            dmrVideo.Stop();
                        }
                        dmrImage.Opacity = 1;
                        dmrVideo.Opacity = 0;
                    }
                    currentType = MediaType.Image;
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
                        dmrImage.Opacity = 0;
                        dmrVideo.Opacity = 1;
                        dmrImage.Source = null;
                    }
                    currentType = MediaType.AudioVideo;
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
                        }
                        break;
                    case MediaElementState.Stopped:
                        receiver.NotifyStopped();
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
                receiver.NotifyEnded();
                if (dmrVideo != null)
                    dmrVideo.Stop();
            }
        }

        private void dmrVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (IsReceiverStarted)
            {
                receiver.NotifyError();
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
        }
    }
}
