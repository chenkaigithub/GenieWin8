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
using Windows.Networking.Connectivity;

using SV.UPnPLite.Protocols.DLNA;
using SV.UPnPLite.Protocols.UPnP;
using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class MyMediaPlayerPage : GenieWin8.Common.LayoutAwarePage
    {
        //var devicesDiscovery = new CommonUPnPDevicesDiscovery();
        //var rendererDevice = devicesDiscovery.DiscoveredDevices.First(device => device.DeviceType == "urn:schemas-upnp-org:device:MediaRenderer");
        //var renderingControlService = rendererDevice.Services.First(service => service.ServiceType == "urn:upnp-org:serviceId:RenderingControl");
        CommonUPnPDevicesDiscovery devicesDiscovery = new CommonUPnPDevicesDiscovery();
        MediaRenderersDiscovery mediaRenderersDiscovery = new MediaRenderersDiscovery();
        bool bDeviceFounded = false;
        private string networkName;
        private bool _sliderpressed = false;
        private bool resetDuration = false;

        public MyMediaPlayerPage()
        {
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);

            volumeSlider.ValueChanged += volumeSlider_ValueChanged;

            PointerEventHandler pointerpressedhandler = new PointerEventHandler(slider_PointerEntered);
            volumeSlider.AddHandler(Control.PointerPressedEvent, pointerpressedhandler, true);

            PointerEventHandler pointerreleasedhandler = new PointerEventHandler(slider_PointerCaptureLost);
            volumeSlider.AddHandler(Control.PointerCaptureLostEvent, pointerreleasedhandler, true);
        }

        private void App_Resuming(Object sender, Object e)
        {
            string compNetworkName = null;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        compNetworkName = connectionProfile.ProfileName;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            if (compNetworkName != networkName)
            {
                mediaRenderersDiscovery = new MediaRenderersDiscovery();
                networkName = compNetworkName;
            }

            MediaRendererList.Items.Clear();
            if (mediaRenderersDiscovery.DiscoveredDevices.Count() == 0)
            {
                MediaRendererTitle.Text = "No Media Renderers found";
            }
            else
            {
                foreach (var renderer in mediaRenderersDiscovery.DiscoveredDevices)
                {
                    MediaRendererList.Items.Add(renderer.FriendlyName);
                }
                MediaRendererTitle.Text = "Media Renderers refreshed";
            }
        }

        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timer1 = new DispatcherTimer();
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
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        networkName = connectionProfile.ProfileName;
                    }
                }
            }
            catch (Exception ex)
            {
            }

            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += new System.EventHandler<object>(timer_Tick);
            timer.Start();

            if (MyMediaInfo.mediaRenderer != null)
            {
                MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                {
                    //System.Diagnostics.Debug.WriteLine("Playback state changed: {0}", state);
                    MyMediaInfo.mediaRendererState = state;
                    if (state == MediaRendererState.NoMediaRenderer)
                    {
                        MyMediaInfo.mediaRenderer = null;
                        bDeviceFounded = false;
                        MediaRendererSelect.Text = "No media renderer selected";
                        MediaRendererStatus.Text = "No media renderer selected";
                    }
                });
            }
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += new System.EventHandler<object>(timer_Tick1);
            timer1.Start();

            volumeSlider.Value = MyMediaInfo.volume;
            resetDuration = true;
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="pageState">要使用可序列化状态填充的空字典。</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
            timer.Stop();
            timer1.Stop();
        }

        void timer_Tick(object sender, object e)
        {
            MediaRendererList.Items.Clear();
            if (mediaRenderersDiscovery.DiscoveredDevices.Count() == 0)
            {
                MediaRendererTitle.Text = "No Media Renderers found";
            }
            else
            {
                foreach (var renderer in mediaRenderersDiscovery.DiscoveredDevices)
                {
                    MediaRendererList.Items.Add(renderer.FriendlyName);
                }

                //foreach (var item in MediaRendererList.Items)
                //{
                //    if (MyMediaInfo.mediaRenderer != null && MyMediaInfo.mediaRenderer.FriendlyName == item.ToString())
                //    {
                //        MediaRendererList.SelectedItem = item;
                //    }
                //}
            }
            //if (devicesDiscovery.DiscoveredDevices.Count() == 0)
            //{
            //    MediaRendererTitle.Text = "No Media Renderers found";
            //}
            //else
            //{
            //    foreach (var renderer in devicesDiscovery.DiscoveredDevices)
            //    {
            //        //MediaRendererList.Items.Add(renderer.FriendlyName);
            //        if (renderer.DeviceType == "urn:schemas-upnp-org:device:MediaRenderer")
            //        {
            //            foreach (var service in renderer.Services)
            //            {
            //                if (service.ServiceType == "urn:schemas-upnp-org:service:RenderingControl:1")
            //                {
            //                    MediaRendererList.Items.Add(renderer.FriendlyName);
            //                }
            //            }
            //        }
            //    }
            //}
        }

        async void timer_Tick1(object sender, object e)
        {
            if (MyMediaInfo.mediaRenderer == null)
            {
                playButton.IsEnabled = false;
                pauseButton.IsEnabled = false;
                stopButton.IsEnabled = false;
                volumeSlider.IsEnabled = false;
                previousButton.IsEnabled = false;
                nextButton.IsEnabled = false;
                timelineSlider.IsEnabled = false;
                timelineSlider.Value = 0.0;
            }
            else
            {
                if (MyMediaInfo.mediaRendererState == MediaRendererState.Buffering)
                {
                    playButton.IsEnabled = false;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;
                    volumeSlider.IsEnabled = false;
                    previousButton.IsEnabled = false;
                    nextButton.IsEnabled = false;
                    timelineSlider.IsEnabled = false;
                    if (MyMediaInfo.mediaItem != null)
                    {
                        MediaRendererStatus.Text = "Buffering...";
                        MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                        timelineSlider.Value = 0.0;
                        resetDuration = true;
                    }
                }
                else if (MyMediaInfo.mediaRendererState == MediaRendererState.NoMediaPresent)
                {
                    playButton.IsEnabled = false;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;
                    volumeSlider.IsEnabled = false;
                    previousButton.IsEnabled = false;
                    nextButton.IsEnabled = false;
                    timelineSlider.IsEnabled = false;
                    MediaRendererStatus.Text = "No media file selected";
                    MediaItemTitle.Text = "";
                    timelineSlider.Value = 0.0;
                }
                else if (MyMediaInfo.mediaRendererState == MediaRendererState.Paused)
                {
                    playButton.IsEnabled = true;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = true;
                    volumeSlider.IsEnabled = true;
                    previousButton.IsEnabled = true;
                    nextButton.IsEnabled = true;
                    timelineSlider.IsEnabled = true;
                    if (MyMediaInfo.mediaItem != null)
                    {
                        MediaRendererStatus.Text = "media file paused";
                        MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                        timelineSlider.Value = MyMediaInfo.currentPosition;
                    }
                }
                else if (MyMediaInfo.mediaRendererState == MediaRendererState.Playing)
                {
                    playButton.IsEnabled = false;
                    pauseButton.IsEnabled = true;
                    stopButton.IsEnabled = true;
                    volumeSlider.IsEnabled = true;
                    previousButton.IsEnabled = true;
                    nextButton.IsEnabled = true;
                    timelineSlider.IsEnabled = true;
                    if (MyMediaInfo.mediaItem != null)
                    {
                        MediaRendererStatus.Text = "Playing media file...";
                        MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                        if (resetDuration)
                        {
                            var duration = await MyMediaInfo.mediaRenderer.GetDuration();
                            timelineSlider.Maximum = duration.TotalSeconds;
                            resetDuration = false;
                        }
                        timelineSlider.Value = MyMediaInfo.currentPosition;
                    }
                }
                else if (MyMediaInfo.mediaRendererState == MediaRendererState.Stopped)
                {
                    playButton.IsEnabled = true;
                    pauseButton.IsEnabled = false;
                    stopButton.IsEnabled = false;
                    volumeSlider.IsEnabled = false;
                    previousButton.IsEnabled = true;
                    nextButton.IsEnabled = true;
                    timelineSlider.IsEnabled = true;
                    if (MyMediaInfo.mediaItem != null)
                    {
                        MediaRendererStatus.Text = "media file stopped";
                        MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                        timelineSlider.Value = 0.0;
                    }

                    if (MyMediaInfo.mediaItem.Class == "object.item.imageItem")
                    {
                        playButton.IsEnabled = false;
                        timelineSlider.IsEnabled = false;
                        MediaRendererStatus.Text = "Playing media file...";
                    }
                }
            }
        }

        async private void MediaRendererList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (MediaRendererList.SelectedIndex != -1)
            {
                foreach (var mediaRenderer in mediaRenderersDiscovery.DiscoveredDevices)
                {
                    //if (mediaRenderer.FriendlyName == MediaRendererList.SelectedItem.ToString() && mediaRenderer != MyMediaInfo.mediaRenderer)
                    //{
                    //    MyMediaInfo.mediaRenderer = mediaRenderer;
                    //    MediaRendererTitle.Text = "Media Renderers selected";
                    //    bDeviceFounded = true;
                    //    if (MyMediaInfo.mediaItem != null)
                    //    {
                    //        await MyMediaInfo.mediaRenderer.StopAsync();
                    //        await MyMediaInfo.mediaRenderer.OpenAsync(MyMediaInfo.mediaItem);
                    //        await MyMediaInfo.mediaRenderer.PlayAsync();
                    //        MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                    //        {
                    //            //System.Diagnostics.Debug.WriteLine("Playback state changed: {0}", state);
                    //            MyMediaInfo.mediaRendererState = state;
                    //            if (state == MediaRendererState.NoMediaRenderer)
                    //            {
                    //                MyMediaInfo.mediaRenderer = null;
                    //                MediaRendererTitle.Text = "No media renderer selected";
                    //            }
                    //        });
                    //        MediaRendererTitle.Text = "Playing media file...";
                    //    } 
                    //    else
                    //    {
                    //        //var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    //        //var strtext = loader.GetString("selectSource");
                    //        var messageDialog = new MessageDialog("Please select source");
                    //        await messageDialog.ShowAsync();
                    //    }
                    //    break;
                    //}
                    if (mediaRenderer.FriendlyName == MediaRendererList.SelectedItem.ToString())
                    {
                        bDeviceFounded = true;
                        if (MyMediaInfo.mediaRenderer == null)
                        {
                            MyMediaInfo.mediaRenderer = mediaRenderer;
                            MediaRendererSelect.Text = MyMediaInfo.mediaRenderer.FriendlyName + " is selected";
                            bDeviceFounded = true;
                            if (MyMediaInfo.mediaItem != null)
                            {
                                await MyMediaInfo.mediaRenderer.StopAsync();
                                await MyMediaInfo.mediaRenderer.OpenAsync(MyMediaInfo.mediaItem);
                                await MyMediaInfo.mediaRenderer.PlayAsync();
                                MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                                {
                                    //System.Diagnostics.Debug.WriteLine("Playback state changed: {0}", state);
                                    MyMediaInfo.mediaRendererState = state;
                                    if (state == MediaRendererState.NoMediaRenderer)
                                    {
                                        MyMediaInfo.mediaRenderer = null;
                                        bDeviceFounded = false;
                                        MediaRendererSelect.Text = "No media renderer selected";
                                        MediaRendererStatus.Text = "No media renderer selected";
                                    }
                                });
                                MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                                MediaRendererStatus.Text = "Playing media file...";
                            }
                            else
                            {
                                //var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                                //var strtext = loader.GetString("selectSource");
                                var messageDialog = new MessageDialog("Please select source");
                                await messageDialog.ShowAsync();
                            }
                        }
                        else if (mediaRenderer != MyMediaInfo.mediaRenderer)
                        {
                            MyMediaInfo.mediaRenderer = mediaRenderer;
                            MediaRendererSelect.Text = MyMediaInfo.mediaRenderer.FriendlyName + " is selected";
                            MyMediaInfo.mediaItem = null;
                            var messageDialog = new MessageDialog("Please select source");
                            await messageDialog.ShowAsync();
                        }
                        break;
                    }
                }
                if (!bDeviceFounded)
                {
                    MediaRendererTitle.Text = "Device does not exist, please refresh the list";
                }
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            string compNetworkName = null;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        compNetworkName = connectionProfile.ProfileName;
                    }
                }
            }
            catch (Exception ex)
            {
            }
            if (compNetworkName != networkName)
            {
                mediaRenderersDiscovery = new MediaRenderersDiscovery();
                networkName = compNetworkName;
            }
            MediaRendererList.Items.Clear();
            if (mediaRenderersDiscovery.DiscoveredDevices.Count() == 0)
            {
                MediaRendererTitle.Text = "No Media Renderers found";
            }
            else
            {
                foreach (var renderer in mediaRenderersDiscovery.DiscoveredDevices)
                {
                    MediaRendererList.Items.Add(renderer.FriendlyName);
                }
                MediaRendererTitle.Text = "Media Renderers refreshed";

                //foreach (var item in MediaRendererList.Items)
                //{
                //    if (MyMediaInfo.mediaRenderer != null && MyMediaInfo.mediaRenderer.FriendlyName == item.ToString())
                //    {
                //        MediaRendererList.SelectedItem = item;
                //    }
                //}
            }
        }

        private async void playButton_Click(object sender, RoutedEventArgs e)
        {
            await MyMediaInfo.mediaRenderer.PlayAsync();
        }

        private async void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            await MyMediaInfo.mediaRenderer.PauseAsync();
        }

        private async void stopButton_Click(object sender, RoutedEventArgs e)
        {
            await MyMediaInfo.mediaRenderer.StopAsync();
        }

        private async void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyMediaInfo.mediaItemIndex == 0)
            {
                MyMediaInfo.mediaItemIndex = MyMediaInfo.mediaItems.Count() - 1;
            }
            else
            {
                MyMediaInfo.mediaItemIndex = MyMediaInfo.mediaItemIndex - 1;
            }
            MyMediaInfo.mediaItem = MyMediaInfo.mediaItems.ElementAt(MyMediaInfo.mediaItemIndex);
            if (MyMediaInfo.mediaRenderer != null)
            {
                await MyMediaInfo.mediaRenderer.StopAsync();
                await MyMediaInfo.mediaRenderer.OpenAsync(MyMediaInfo.mediaItem);
                await MyMediaInfo.mediaRenderer.PlayAsync();
                MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                {
                    MyMediaInfo.mediaRendererState = state;
                    if (state == MediaRendererState.NoMediaRenderer)
                    {
                        MyMediaInfo.mediaRenderer = null;
                        bDeviceFounded = false;
                        MediaRendererSelect.Text = "No media renderer selected";
                        MediaRendererStatus.Text = "No media renderer selected";
                    }
                });

                MediaRendererStatus.Text = "Playing media file...";
            }
            else
            {
                var messageDialog = new MessageDialog("Please select one player");
                await messageDialog.ShowAsync();
                MediaRendererSelect.Text = "No media renderer selected";
                MediaRendererStatus.Text = "No media renderer selected";
            }
        }

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyMediaInfo.mediaItemIndex == MyMediaInfo.mediaItems.Count() - 1)
            {
                MyMediaInfo.mediaItemIndex = 0;
            }
            else
            {
                MyMediaInfo.mediaItemIndex = MyMediaInfo.mediaItemIndex + 1;
            }
            MyMediaInfo.mediaItem = MyMediaInfo.mediaItems.ElementAt(MyMediaInfo.mediaItemIndex);
            if (MyMediaInfo.mediaRenderer != null)
            {
                await MyMediaInfo.mediaRenderer.StopAsync();
                await MyMediaInfo.mediaRenderer.OpenAsync(MyMediaInfo.mediaItem);
                await MyMediaInfo.mediaRenderer.PlayAsync();
                MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                {
                    MyMediaInfo.mediaRendererState = state;
                    if (state == MediaRendererState.NoMediaRenderer)
                    {
                        MyMediaInfo.mediaRenderer = null;
                        bDeviceFounded = false;
                        MediaRendererSelect.Text = "No media renderer selected";
                        MediaRendererStatus.Text = "No media renderer selected";
                    }
                });

                MediaRendererStatus.Text = "Playing media file...";
            }
            else
            {
                var messageDialog = new MessageDialog("Please select one player");
                await messageDialog.ShowAsync();
                MediaRendererSelect.Text = "No media renderer selected";
                MediaRendererStatus.Text = "No media renderer selected";
            }
        }

        async void volumeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (!_sliderpressed)
            {
                await MyMediaInfo.mediaRenderer.SetVolume((int)e.NewValue);
                MyMediaInfo.volume = (int)e.NewValue;
            }
        }

        void slider_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _sliderpressed = true;
        }

        async void slider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            await MyMediaInfo.mediaRenderer.SetVolume((int)volumeSlider.Value);
            _sliderpressed = false;
            MyMediaInfo.volume = (int)volumeSlider.Value;
        }
    }
}
