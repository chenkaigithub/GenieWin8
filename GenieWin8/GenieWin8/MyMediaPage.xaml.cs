﻿using GenieWin8.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
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
using Windows.Graphics.Display;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;
using Windows.Media.PlayTo;
using Windows.UI.Core;
using Windows.Security.ExchangeActiveSyncProvisioning;


// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class MyMediaPage : GenieWin8.Common.LayoutAwarePage
    {
        MediaServersDiscovery mediaServersDiscovery = new MediaServersDiscovery();
        MediaRenderersDiscovery mediaRenderersDiscovery = new MediaRenderersDiscovery();
        bool bDMSFounded = false;
        bool bDMRFounded = false;
        int Containers_Count = 0;           //该层目录中的媒体文件夹数目
        int MediaItems_Count = 0;           //该层目录中的媒体资源数目
        Uri _baseUri = new Uri("ms-appx:///");
        private bool _sliderpressed = false;
        private bool resetDuration = false;


        PlayToReceiver receiver = null;
        bool IsSeeking = false;
        double bufferedPlaybackRate = 0;
        bool justLoadedMedia = false;
        bool IsPlayReceivedPreMediaLoaded = false;
        enum MediaType { None, Image, Audio, Video };
        MediaType currentType = MediaType.None;
        BitmapImage imagerevd = null;

        bool IsFullScreen = false;
        private Brush _previousGridbackground;
        private Size _previousmediasize;
        public Size PreviousMediaSize
        {
            get { return _previousmediasize; }
            set { _previousmediasize = value; }
        }
        private Thickness _previousLayoutrootmargin;
        private Thickness _previousFlipViewMargin;
        private Thickness _previousGridMargin;
        private Brush _previousBGColor;
        private DispatcherTimer _timer;

        public MyMediaPage()
        {
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);

            volumeSlider.ValueChanged += volumeSlider_ValueChanged;

            PointerEventHandler pointerpressedhandler = new PointerEventHandler(volumeSlider_PointerEntered);
            volumeSlider.AddHandler(Control.PointerPressedEvent, pointerpressedhandler, true);

            PointerEventHandler pointerreleasedhandler = new PointerEventHandler(volumeSlider_PointerCaptureLost);
            volumeSlider.AddHandler(Control.PointerCaptureLostEvent, pointerreleasedhandler, true);

            PointerEventHandler timelinepointerpressedhandler = new PointerEventHandler(timelineSlider_PointerEntered);
            timelineSlider.AddHandler(Control.PointerPressedEvent, timelinepointerpressedhandler, true);

            PointerEventHandler timelinepointerreleasedhandler = new PointerEventHandler(timelineSlider_PointerCaptureLost);
            timelineSlider.AddHandler(Control.PointerCaptureLostEvent, timelinepointerreleasedhandler, true);
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

            if (compNetworkName != MyMediaInfo.networkName)
            {
                mediaServersDiscovery = new MediaServersDiscovery();
                mediaRenderersDiscovery = new MediaRenderersDiscovery();
                MyMediaInfo.networkName = compNetworkName;
                MyMediaInfo.bDeviceList = true;
            }
            DeviceMediaList.Items.Clear();
            if (MyMediaInfo.bDeviceList)
            {
                if (mediaServersDiscovery.DiscoveredDevices.Count() != 0)
                {
                    foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                    {
                        DeviceMediaList.Items.Add(server.FriendlyName);
                    }
                }
                MyMediaInfo.stackMediaObjects = new Stack<IEnumerable<MediaObject>>();
                MyMediaInfo.stackMediaContainer = new Stack<MediaContainer>();
            }
            else
            {
                this.BrowseMediaContainer();
            }

            MediaRendererList.Items.Clear();
            if (mediaRenderersDiscovery.DiscoveredDevices.Count() != 0)
            {
                foreach (var renderer in mediaRenderersDiscovery.DiscoveredDevices)
                {
                    MediaRendererList.Items.Add(renderer.FriendlyName);
                }
            }
        }

        DispatcherTimer timer = new DispatcherTimer();
        DispatcherTimer timer1 = new DispatcherTimer();
        DispatcherTimer timer2 = new DispatcherTimer();
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

            if (compNetworkName != MyMediaInfo.networkName)
            {
                mediaServersDiscovery = new MediaServersDiscovery();
                MyMediaInfo.networkName = compNetworkName;
                MyMediaInfo.bDeviceList = true;
            }
            DeviceMediaList.Items.Clear();
            if (MyMediaInfo.bDeviceList)
            {
                if (mediaServersDiscovery.DiscoveredDevices.Count() != 0)
                {
                    foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                    {
                        DeviceMediaList.Items.Add(server.FriendlyName);
                    }
                }
                MyMediaInfo.stackMediaObjects = new Stack<IEnumerable<MediaObject>>();
                MyMediaInfo.stackMediaContainer = new Stack<MediaContainer>();
            }
            else
            {
                this.BrowseMediaContainer();
            }

            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += new System.EventHandler<object>(timer_Tick);
            timer.Start();

            if (MyMediaInfo.mediaRenderer != null)
            {
                MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                {
                    MyMediaInfo.mediaRendererState = state;
                    if (state == MediaRendererState.NoMediaRenderer)
                    {
                        MyMediaInfo.mediaRenderer = null;
                        bDMRFounded = false;
                    }
                });
            }
            timer1.Interval = TimeSpan.FromSeconds(1);
            timer1.Tick += new System.EventHandler<object>(timer_Tick1);
            timer1.Start();

            volumeSlider.Value = MyMediaInfo.volume;
            resetDuration = true;

            if (MyMediaInfo.IsDMRAllowtoPlay)
            {
                cbDMRStarted.IsChecked = true;
                try
                {
                    InitialisePlayToReceiver();
                    await receiver.StartAsync();
                    MyMediaInfo.IsDMRAllowtoPlay = true;
                    StatusNotify.Text = "Player started to receive";

                    timer2.Interval = TimeSpan.FromSeconds(5);
                    timer2.Tick += new System.EventHandler<object>(timer_Tick2);
                    timer2.Start();
                }
                catch (Exception ecp)
                {
                    MyMediaInfo.IsDMRAllowtoPlay = false;
                    cbDMRStarted.IsChecked = false;
                    StatusNotify.Text = "Player start failed, Error " + ecp.Message;
                }
            }
            else
            {
                cbDMRStarted.IsChecked = false;
            }

            DeviceMediaList.Height = Window.Current.Bounds.Height - 265;
            MediaRendererList.Height = Window.Current.Bounds.Height - 265;
            if (DisplayProperties.CurrentOrientation == DisplayOrientations.Landscape || DisplayProperties.CurrentOrientation == DisplayOrientations.LandscapeFlipped)
            {
                MediaItemTitle.MaxWidth = Window.Current.Bounds.Width - 100;
                dmrVideo.Height = Window.Current.Bounds.Height - 365;
                dmrVideo.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
                dmrImage.Height = Window.Current.Bounds.Height - 365;
                dmrImage.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
                timelineSlider.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
                SetVolumePanel.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
                volumeSlider.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
            }
            else if (DisplayProperties.CurrentOrientation == DisplayOrientations.Portrait || DisplayProperties.CurrentOrientation == DisplayOrientations.PortraitFlipped)
            {
                MediaItemTitle.MaxWidth = Window.Current.Bounds.Width - 100;
                dmrVideo.Height = (Window.Current.Bounds.Width - 100) * 3 / 4;
                dmrVideo.Width = Window.Current.Bounds.Width - 100;
                dmrImage.Height = (Window.Current.Bounds.Width - 100) * 3 / 4;
                dmrImage.Width = Window.Current.Bounds.Width - 100;
                timelineSlider.Width = Window.Current.Bounds.Width - 100;
                SetVolumePanel.Width = Window.Current.Bounds.Width - 100;
                volumeSlider.Width = Window.Current.Bounds.Width - 100;
            }
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="pageState">要使用可序列化状态填充的空字典。</param>
        protected override async void SaveState(Dictionary<String, Object> pageState)
        {
            timer.Stop();
            timer1.Stop();

            if (MyMediaInfo.IsDMRAllowtoPlay)
            {
                await receiver.StopAsync();
                MyMediaInfo.IsDMRAllowtoPlay = false;
                timer2.Stop();
            }
        }

        //private void SourceButton_Click(object sender, RoutedEventArgs e)
        //{
        //    Button btn = (Button)sender;
        //    if (btn.Name=="SourceButton")
        //    {
        //        this.Frame.Navigate(typeof(MyMediaSourcePage));
        //    }
        //    else if (btn.Name == "PlayerButton")
        //    {
        //        this.Frame.Navigate(typeof(MyMediaPlayerPage));
        //    }
        //    else if (btn.Name == "PlayingButton")
        //    {
        //        this.Frame.Navigate(typeof(MyMediaPlayingPage));
        //    }
        //}

        private void DeviceMediaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MyMediaInfo.bLoadFile)
            {
                MyMediaInfo.bLoadFile = !MyMediaInfo.bLoadFile;
            }
            else
            {
                if (DeviceMediaList.SelectedIndex != -1)
                {
                    LoadMediaFiles();
                }
            }
        }

        async private void LoadMediaFiles()
        {
            if (MyMediaInfo.bDeviceList)
            {
                if (DeviceMediaList.SelectedIndex != -1)
                {
                    PopupBackground.Visibility = Visibility.Visible;
                    foreach (var serverDevice in mediaServersDiscovery.DiscoveredDevices)
                    {
                        if (serverDevice.FriendlyName == DeviceMediaList.SelectedItem.ToString())
                        {
                            bDMSFounded = true;
                            //PopupBackground.Visibility = Visibility.Visible;
                            var rootObjects = await serverDevice.BrowseAsync();
                            if (rootObjects != null)
                            {
                                MyMediaInfo.bDeviceList = false;
                                MyMediaInfo.stackMediaObjects.Push(rootObjects);
                                MyMediaInfo.mediaServer = serverDevice;
                                DeviceMediaList.Items.Clear();
                                StackPanel uplevel_Item = new StackPanel();
                                uplevel_Item.Orientation = Orientation.Horizontal;
                                Image uplevel_Icon = new Image();
                                uplevel_Icon.Width = 30;
                                uplevel_Icon.Height = 30;
                                uplevel_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/toolbar_uplevel.png"));
                                TextBlock uplevel_Title = new TextBlock();
                                uplevel_Title.Text = "... " + MyMediaInfo.mediaServer.FriendlyName;
                                uplevel_Title.FontSize = 20;
                                uplevel_Item.Children.Add(uplevel_Icon);
                                uplevel_Item.Children.Add(uplevel_Title);
                                DeviceMediaList.Items.Add(uplevel_Item);

                                var rootContainers = rootObjects.OfType<MediaContainer>();
                                Containers_Count = rootContainers.Count();
                                MyMediaInfo.mediaContainers = rootContainers;
                                var rootMediaItems = rootObjects.OfType<MediaItem>();
                                MediaItems_Count = rootMediaItems.Count();
                                MyMediaInfo.mediaItems = rootMediaItems;
                                if (Containers_Count > 0)
                                {
                                    foreach (var MediaContainer in rootContainers)
                                    {
                                        StackPanel mediaContainer_Item = new StackPanel();
                                        mediaContainer_Item.Orientation = Orientation.Horizontal;
                                        Image mediaContainer_Icon = new Image();
                                        mediaContainer_Icon.Width = 35;
                                        mediaContainer_Icon.Height = 35;
                                        mediaContainer_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/folder.png"));
                                        TextBlock mediaContainer_Title = new TextBlock();
                                        mediaContainer_Title.Text = MediaContainer.Title;
                                        mediaContainer_Title.FontSize = 22;
                                        mediaContainer_Item.Children.Add(mediaContainer_Icon);
                                        mediaContainer_Item.Children.Add(mediaContainer_Title);
                                        DeviceMediaList.Items.Add(mediaContainer_Item);
                                    }
                                }
                                if (MediaItems_Count > 0)
                                {
                                    foreach (var MediaItem in rootMediaItems)
                                    {
                                        StackPanel mediaItem_Item = new StackPanel();
                                        mediaItem_Item.Orientation = Orientation.Horizontal;
                                        Image mediaItem_Icon = new Image();
                                        mediaItem_Icon.Width = 35;
                                        mediaItem_Icon.Height = 35;
                                        if (MediaItem.Class == "object.item.imageItem")
                                        {
                                            mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                                        }
                                        else if (MediaItem.Class == "object.item.audioItem" || MediaItem.Class == "object.item.audioItem.musicTrack")
                                        {
                                            mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_audio.png"));
                                        }
                                        else if (MediaItem.Class == "object.item.videoItem")
                                        {
                                            mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_video.png"));
                                        }
                                        TextBlock mediaItem_Title = new TextBlock();
                                        mediaItem_Title.Text = MediaItem.Title;
                                        mediaItem_Title.FontSize = 22;
                                        mediaItem_Item.Children.Add(mediaItem_Icon);
                                        mediaItem_Item.Children.Add(mediaItem_Title);
                                        DeviceMediaList.Items.Add(mediaItem_Item);
                                    }

                                    MyMediaInfo.bCurrentDirectory = false;
                                    int index = -1;
                                    foreach (var MediaItem in rootMediaItems)
                                    {
                                        index++;
                                        if (MyMediaInfo.mediaItem != null && MediaItem.Title == MyMediaInfo.mediaItem.Title)
                                        {
                                            MyMediaInfo.bCurrentDirectory = true;
                                            MyMediaInfo.bLoadFile = false;
                                            DeviceMediaList.SelectedIndex = MyMediaInfo.mediaContainers.Count() + index + 1;
                                            break;
                                        }
                                    }
                                }
                                else
                                {
                                    MyMediaInfo.bCurrentDirectory = false;
                                }
                                PopupBackground.Visibility = Visibility.Collapsed;
                            }
                            else        //rootObjects == null
                            {
                                PopupBackground.Visibility = Visibility.Collapsed;
                                var messageDialog = new MessageDialog("Load failed");
                                await messageDialog.ShowAsync();
                            }
                            break;
                        }
                    }
                    if (!bDMSFounded)
                    {
                        //MediaTitle.Text = "Device does not exist, please refresh the list";
                        PopupBackground.Visibility = Visibility.Collapsed;
                    }
                }
            }
            else
            {
                if (DeviceMediaList.SelectedIndex == 0 && MyMediaInfo.stackMediaObjects.Count > 0)             //第一项为返回上一层
                {
                    this.BackToUpperlevel();
                }
                else
                {
                    if (DeviceMediaList.SelectedIndex > 0 && DeviceMediaList.SelectedIndex < Containers_Count + 1)
                    {
                        PopupBackground.Visibility = Visibility.Visible;
                        int containerIndex = DeviceMediaList.SelectedIndex - 1;
                        var ContainerToBrowse = MyMediaInfo.mediaContainers.ElementAt(containerIndex);
                        var rootObjects = await MyMediaInfo.mediaServer.BrowseAsync(ContainerToBrowse);
                        if (rootObjects != null)
                        {
                            MyMediaInfo.stackMediaObjects.Push(rootObjects);
                            MyMediaInfo.stackMediaContainer.Push(ContainerToBrowse);
                            DeviceMediaList.Items.Clear();
                            StackPanel uplevel_Item = new StackPanel();
                            uplevel_Item.Orientation = Orientation.Horizontal;
                            Image uplevel_Icon = new Image();
                            uplevel_Icon.Width = 30;
                            uplevel_Icon.Height = 30;
                            uplevel_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/toolbar_uplevel.png"));
                            TextBlock uplevel_Title = new TextBlock();
                            uplevel_Title.Text = "... " + MyMediaInfo.mediaServer.FriendlyName;
                            uplevel_Title.FontSize = 20;
                            uplevel_Item.Children.Add(uplevel_Icon);
                            uplevel_Item.Children.Add(uplevel_Title);
                            DeviceMediaList.Items.Add(uplevel_Item);

                            var rootContainers = rootObjects.OfType<MediaContainer>();
                            Containers_Count = rootContainers.Count();
                            MyMediaInfo.mediaContainers = rootContainers;
                            var rootMediaItems = rootObjects.OfType<MediaItem>();
                            MediaItems_Count = rootMediaItems.Count();
                            MyMediaInfo.mediaItems = rootMediaItems;
                            if (Containers_Count > 0)
                            {
                                foreach (var MediaContainer in rootContainers)
                                {
                                    StackPanel mediaContainer_Item = new StackPanel();
                                    mediaContainer_Item.Orientation = Orientation.Horizontal;
                                    Image mediaContainer_Icon = new Image();
                                    mediaContainer_Icon.Width = 35;
                                    mediaContainer_Icon.Height = 35;
                                    mediaContainer_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/folder.png"));
                                    TextBlock mediaContainer_Title = new TextBlock();
                                    mediaContainer_Title.Text = MediaContainer.Title;
                                    mediaContainer_Title.FontSize = 22;
                                    mediaContainer_Item.Children.Add(mediaContainer_Icon);
                                    mediaContainer_Item.Children.Add(mediaContainer_Title);
                                    DeviceMediaList.Items.Add(mediaContainer_Item);
                                }
                            }

                            if (MediaItems_Count > 0)
                            {
                                foreach (var MediaItem in rootMediaItems)
                                {
                                    StackPanel mediaItem_Item = new StackPanel();
                                    mediaItem_Item.Orientation = Orientation.Horizontal;
                                    Image mediaItem_Icon = new Image();
                                    mediaItem_Icon.Width = 35;
                                    mediaItem_Icon.Height = 35;
                                    if (MediaItem.Class == "object.item.imageItem")
                                    {
                                        mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                                    }
                                    else if (MediaItem.Class == "object.item.audioItem" || MediaItem.Class == "object.item.audioItem.musicTrack")
                                    {
                                        mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_audio.png"));
                                    }
                                    else if (MediaItem.Class == "object.item.videoItem")
                                    {
                                        mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_video.png"));
                                    }
                                    TextBlock mediaItem_Title = new TextBlock();
                                    mediaItem_Title.Text = MediaItem.Title;
                                    mediaItem_Title.FontSize = 22;
                                    mediaItem_Item.Children.Add(mediaItem_Icon);
                                    mediaItem_Item.Children.Add(mediaItem_Title);
                                    DeviceMediaList.Items.Add(mediaItem_Item);
                                }

                                MyMediaInfo.bCurrentDirectory = false;
                                int index = -1;
                                foreach (var MediaItem in rootMediaItems)
                                {
                                    index++;
                                    if (MyMediaInfo.mediaItem != null && MediaItem.Title == MyMediaInfo.mediaItem.Title)
                                    {
                                        MyMediaInfo.bCurrentDirectory = true;
                                        MyMediaInfo.bLoadFile = false;
                                        DeviceMediaList.SelectedIndex = MyMediaInfo.mediaContainers.Count() + index + 1;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                MyMediaInfo.bCurrentDirectory = false;
                            }
                            PopupBackground.Visibility = Visibility.Collapsed;
                        }
                        else             //rootObjects == null
                        {
                            PopupBackground.Visibility = Visibility.Collapsed;
                            var messageDialog = new MessageDialog("Load failed");
                            await messageDialog.ShowAsync();
                        }
                    }
                    else if (DeviceMediaList.SelectedIndex > Containers_Count)
                    {
                        PopupBackground.Visibility = Visibility.Visible;
                        MyMediaInfo.mediaItemsforSwitch = MyMediaInfo.mediaItems;
                        MyMediaInfo.mediaItemIndex = DeviceMediaList.SelectedIndex - (Containers_Count + 1);
                        MyMediaInfo.mediaItem = MyMediaInfo.mediaItemsforSwitch.ElementAt(MyMediaInfo.mediaItemIndex);
                        MyMediaInfo.bCurrentDirectory = true;
                        if (MyMediaInfo.mediaRenderer != null)
                        {
                            await MyMediaInfo.mediaRenderer.StopAsync();
                            await MyMediaInfo.mediaRenderer.OpenAsync(MyMediaInfo.mediaItem);
                            await MyMediaInfo.mediaRenderer.PlayAsync();
                            PopupBackground.Visibility = Visibility.Collapsed;
                            MymediaFlipview.SelectedIndex = 2;
                            
                            timelineSlider.Maximum = MyMediaInfo.mediaItem.Resources.ElementAt(0).Duration.TotalSeconds;
                            var bg = new ImageBrush();
                            bg.Stretch = Stretch.Uniform;
                            if (MyMediaInfo.mediaItem.Class == "object.item.imageItem")
                            {
                                //mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                                var imageUri = MyMediaInfo.mediaItem.Resources.ElementAt(0).Uri;
                                var imagerecv = new BitmapImage();
                                imagerecv.ImageOpened += imagerevd_ImageOpened;
                                imagerecv.UriSource = new Uri(imageUri);
                                dmrImage.Source = imagerecv;
                                dmrImage.Opacity = 1;
                                Output.Background = null;
                                var mediaRendererState = await MyMediaInfo.mediaRenderer.GetCurrentState();
                                MyMediaInfo.mediaRendererState = mediaRendererState;
                                if (mediaRendererState == MediaRendererState.NoMediaRenderer)
                                {
                                    MyMediaInfo.mediaRenderer = null;
                                }
                            }
                            else if (MyMediaInfo.mediaItem.Class == "object.item.audioItem" || MyMediaInfo.mediaItem.Class == "object.item.audioItem.musicTrack")
                            {
                                dmrImage.Opacity = 0;
                                SetVolumePanel.Visibility = Visibility.Visible;
                                bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_music_icon.png"));
                                Output.Background = bg;
                            }
                            else if (MyMediaInfo.mediaItem.Class == "object.item.videoItem")
                            {
                                dmrImage.Opacity = 0;
                                SetVolumePanel.Visibility = Visibility.Visible;
                                bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_video_icon.png"));
                                Output.Background = bg;
                            }
                            MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                            {
                                //System.Diagnostics.Debug.WriteLine("Playback state : {0}", state);
                                MyMediaInfo.mediaRendererState = state;
                                if (state == MediaRendererState.NoMediaRenderer)
                                {
                                    MyMediaInfo.mediaRenderer = null;
                                }
                            });
                            MyMediaInfo.mediaRenderer.PositionChanges.Subscribe(position =>
                            {
                                //System.Diagnostics.Debug.WriteLine("Playback position : {0}", position);
                                MyMediaInfo.currentPosition = position.TotalSeconds;
                            });
                            MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                        }
                        else
                        {
                            PopupBackground.Visibility = Visibility.Collapsed;
                            var messageDialog = new MessageDialog("Please select one player");
                            await messageDialog.ShowAsync();
                            MymediaFlipview.SelectedIndex = 1;
                        }
                    }
                }
            }
        }

        async void timer_Tick1(object sender, object e)
        {
            if (MyMediaInfo.mediaRenderer == null)
            {
                playButton.IsEnabled = false;
                playButton.Visibility = Visibility.Visible;
                pauseButton.Visibility = Visibility.Collapsed;
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
                    playButton.Visibility = Visibility.Visible;
                    pauseButton.Visibility = Visibility.Collapsed;
                    stopButton.IsEnabled = false;
                    volumeSlider.IsEnabled = false;
                    previousButton.IsEnabled = false;
                    nextButton.IsEnabled = false;
                    timelineSlider.IsEnabled = false;
                    if (MyMediaInfo.mediaItem != null)
                    {
                        MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                        timelineSlider.Value = 0.0;
                        resetDuration = true;
                    }
                }
                else if (MyMediaInfo.mediaRendererState == MediaRendererState.NoMediaPresent)
                {
                    playButton.IsEnabled = false;
                    playButton.Visibility = Visibility.Visible;
                    pauseButton.Visibility = Visibility.Collapsed;
                    stopButton.IsEnabled = false;
                    volumeSlider.IsEnabled = false;
                    previousButton.IsEnabled = false;
                    nextButton.IsEnabled = false;
                    timelineSlider.IsEnabled = false;
                    MediaItemTitle.Text = "";
                    timelineSlider.Value = 0.0;
                }
                else if (MyMediaInfo.mediaRendererState == MediaRendererState.Paused)
                {
                    if (MyMediaInfo.mediaItem != null)
                    {
                        playButton.IsEnabled = true;
                        playButton.Visibility = Visibility.Visible;
                        pauseButton.Visibility = Visibility.Collapsed;
                        stopButton.IsEnabled = true;
                        volumeSlider.IsEnabled = true;
                        previousButton.IsEnabled = true;
                        nextButton.IsEnabled = true;
                        timelineSlider.IsEnabled = true;
                        MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                        timelineSlider.Value = MyMediaInfo.currentPosition;
                    }
                    else
                    {
                        playButton.IsEnabled = false;
                        playButton.Visibility = Visibility.Visible;
                        pauseButton.Visibility = Visibility.Collapsed;
                        stopButton.IsEnabled = false;
                        volumeSlider.IsEnabled = false;
                        previousButton.IsEnabled = false;
                        nextButton.IsEnabled = false;
                        timelineSlider.IsEnabled = false;
                        MediaItemTitle.Text = "";
                    }
                }
                else if (MyMediaInfo.mediaRendererState == MediaRendererState.Playing)
                {
                    if (MyMediaInfo.mediaItem != null)
                    {
                        playButton.IsEnabled = false;
                        stopButton.IsEnabled = true;
                        volumeSlider.IsEnabled = true;
                        previousButton.IsEnabled = true;
                        nextButton.IsEnabled = true;
                        timelineSlider.IsEnabled = true;
                        MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                        if (MyMediaInfo.mediaItem.Class == "object.item.imageItem")
                        {
                            SetVolumePanel.Visibility = Visibility.Collapsed;
                            playButton.Visibility = Visibility.Visible;
                            pauseButton.Visibility = Visibility.Collapsed;
                            stopButton.IsEnabled = false;
                            timelineSlider.IsEnabled = false;
                        }
                        else
                        {
                            SetVolumePanel.Visibility = Visibility.Visible;
                            playButton.Visibility = Visibility.Collapsed;
                            pauseButton.Visibility = Visibility.Visible;
                            stopButton.IsEnabled = true;
                            timelineSlider.IsEnabled = true;
                        }
                        if (resetDuration)
                        {
                            var duration = await MyMediaInfo.mediaRenderer.GetDuration();
                            timelineSlider.Maximum = duration.TotalSeconds;
                            resetDuration = false;
                        }
                        timelineSlider.Value = MyMediaInfo.currentPosition;
                    }
                    else
                    {
                        playButton.IsEnabled = false;
                        playButton.Visibility = Visibility.Visible;
                        pauseButton.Visibility = Visibility.Collapsed;
                        stopButton.IsEnabled = false;
                        SetVolumePanel.Visibility = Visibility.Collapsed;
                        volumeSlider.IsEnabled = false;
                        previousButton.IsEnabled = false;
                        nextButton.IsEnabled = false;
                        timelineSlider.IsEnabled = false;
                        MediaItemTitle.Text = "";
                    }
                }
                else if (MyMediaInfo.mediaRendererState == MediaRendererState.Stopped)
                {
                    if (MyMediaInfo.mediaItem != null)
                    {
                        pauseButton.Visibility = Visibility.Collapsed;
                        stopButton.IsEnabled = false;
                        volumeSlider.IsEnabled = false;
                        previousButton.IsEnabled = true;
                        nextButton.IsEnabled = true;
                        MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                        timelineSlider.Value = 0.0;
                        resetDuration = true;
                        if (MyMediaInfo.mediaItem.Class == "object.item.imageItem")
                        {
                            playButton.IsEnabled = false;
                            playButton.Visibility = Visibility.Visible;
                            SetVolumePanel.Visibility = Visibility.Collapsed;
                            timelineSlider.IsEnabled = false;
                        }
                        else
                        {
                            SetVolumePanel.Visibility = Visibility.Visible;
                            playButton.IsEnabled = true;
                            playButton.Visibility = Visibility.Visible;
                            timelineSlider.IsEnabled = true;
                        }
                    }
                    else
                    {
                        playButton.IsEnabled = false;
                        playButton.Visibility = Visibility.Visible;
                        pauseButton.Visibility = Visibility.Collapsed;
                        stopButton.IsEnabled = false;
                        volumeSlider.IsEnabled = false;
                        previousButton.IsEnabled = false;
                        nextButton.IsEnabled = false;
                        timelineSlider.IsEnabled = false;
                        MediaItemTitle.Text = "";
                    }
                }
            }
        }

        void timer_Tick(object sender, object e)
        {
            if (MyMediaInfo.bDeviceList)
            {
                DeviceMediaList.Items.Clear();
                if (mediaServersDiscovery.DiscoveredDevices.Count() != 0)
                {
                    foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                    {
                        DeviceMediaList.Items.Add(server.FriendlyName);
                    }
                }
            }

            MediaRendererList.Items.Clear();
            if (mediaRenderersDiscovery.DiscoveredDevices.Count() != 0)
            {
                foreach (var renderer in mediaRenderersDiscovery.DiscoveredDevices)
                {
                    MediaRendererList.Items.Add(renderer.FriendlyName);
                }

                foreach (var item in MediaRendererList.Items)
                {
                    if (MyMediaInfo.mediaRenderer != null && MyMediaInfo.mediaRenderer.FriendlyName == item.ToString())
                    {
                        MyMediaInfo.IsPlayerSwitched = false;
                        MediaRendererList.SelectedItem = item;
                    }
                }
            }
        }

        //返回上层目录
        private void BackToUpperlevel()
        {
            if (MyMediaInfo.stackMediaObjects.Count > 0)
            {
                MyMediaInfo.stackMediaObjects.Pop();
            }

            if (MyMediaInfo.stackMediaContainer.Count > 0)
            {
                MyMediaInfo.stackMediaContainer.Pop();
            }

            PopupBackground.Visibility = Visibility.Visible;
            if (MyMediaInfo.stackMediaObjects.Count == 0)
            {
                DeviceMediaList.Items.Clear();
                if (mediaServersDiscovery.DiscoveredDevices.Count() != 0)
                {
                    foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                    {
                        DeviceMediaList.Items.Add(server.FriendlyName);
                    }
                }
                MyMediaInfo.bDeviceList = true;
                bDMSFounded = false;
                MyMediaInfo.stackMediaObjects.Clear();
                MyMediaInfo.bCurrentDirectory = false;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
            else
            {
                PopupBackground.Visibility = Visibility.Collapsed;
                DeviceMediaList.Items.Clear();
                this.BrowseMediaContainer();
            }
        }

        private void DMS_Refresh_Click(object sender, RoutedEventArgs e)
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

            if (compNetworkName != MyMediaInfo.networkName)
            {
                mediaServersDiscovery = new MediaServersDiscovery();
                MyMediaInfo.networkName = compNetworkName;
                MyMediaInfo.bDeviceList = true;  
            }
            DeviceMediaList.Items.Clear();
            if (MyMediaInfo.bDeviceList)
            {
                if (mediaServersDiscovery.DiscoveredDevices.Count() != 0)
                {
                    foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                    {
                        DeviceMediaList.Items.Add(server.FriendlyName);
                    }
                }
                MyMediaInfo.stackMediaObjects = new Stack<IEnumerable<MediaObject>>();
                MyMediaInfo.stackMediaContainer = new Stack<MediaContainer>();
            }
            else
            {
                this.BrowseMediaContainer();
            }
        }

        private async void BrowseMediaContainer()
        {
            if (MyMediaInfo.stackMediaContainer.Count == 0)         //设备的第一层根目录
            {
                bDMSFounded = false;
                PopupBackground.Visibility = Visibility.Visible;
                foreach (var serverDevice in mediaServersDiscovery.DiscoveredDevices)
                {
                    if (serverDevice.FriendlyName == MyMediaInfo.mediaServer.FriendlyName)
                    {
                        bDMSFounded = true;
                        var rootObjects = await serverDevice.BrowseAsync();
                        if (rootObjects != null)
                        {
                            StackPanel uplevel_Item = new StackPanel();
                            uplevel_Item.Orientation = Orientation.Horizontal;
                            Image uplevel_Icon = new Image();
                            uplevel_Icon.Width = 30;
                            uplevel_Icon.Height = 30;
                            uplevel_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/toolbar_uplevel.png"));
                            TextBlock uplevel_Title = new TextBlock();
                            uplevel_Title.Text = "... " + MyMediaInfo.mediaServer.FriendlyName;
                            uplevel_Title.FontSize = 20;
                            uplevel_Item.Children.Add(uplevel_Icon);
                            uplevel_Item.Children.Add(uplevel_Title);
                            DeviceMediaList.Items.Add(uplevel_Item);

                            var rootContainers = rootObjects.OfType<MediaContainer>();
                            Containers_Count = rootContainers.Count();
                            MyMediaInfo.mediaContainers = rootContainers;
                            var rootMediaItems = rootObjects.OfType<MediaItem>();
                            MediaItems_Count = rootMediaItems.Count();
                            MyMediaInfo.mediaItems = rootMediaItems;
                            if (Containers_Count > 0)
                            {
                                foreach (var MediaContainer in rootContainers)
                                {
                                    StackPanel mediaContainer_Item = new StackPanel();
                                    mediaContainer_Item.Orientation = Orientation.Horizontal;
                                    Image mediaContainer_Icon = new Image();
                                    mediaContainer_Icon.Width = 35;
                                    mediaContainer_Icon.Height = 35;
                                    mediaContainer_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/folder.png"));
                                    TextBlock mediaContainer_Title = new TextBlock();
                                    mediaContainer_Title.Text = MediaContainer.Title;
                                    mediaContainer_Title.FontSize = 22;
                                    mediaContainer_Item.Children.Add(mediaContainer_Icon);
                                    mediaContainer_Item.Children.Add(mediaContainer_Title);
                                    DeviceMediaList.Items.Add(mediaContainer_Item);
                                }
                            }

                            if (MediaItems_Count > 0)
                            {
                                foreach (var MediaItem in rootMediaItems)
                                {
                                    StackPanel mediaItem_Item = new StackPanel();
                                    mediaItem_Item.Orientation = Orientation.Horizontal;
                                    Image mediaItem_Icon = new Image();
                                    mediaItem_Icon.Width = 35;
                                    mediaItem_Icon.Height = 35;
                                    if (MediaItem.Class == "object.item.imageItem")
                                    {
                                        mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                                    }
                                    else if (MediaItem.Class == "object.item.audioItem" || MediaItem.Class == "object.item.audioItem.musicTrack")
                                    {
                                        mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_audio.png"));
                                    }
                                    else if (MediaItem.Class == "object.item.videoItem")
                                    {
                                        mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_video.png"));
                                    }
                                    TextBlock mediaItem_Title = new TextBlock();
                                    mediaItem_Title.Text = MediaItem.Title;
                                    mediaItem_Title.FontSize = 22;
                                    mediaItem_Item.Children.Add(mediaItem_Icon);
                                    mediaItem_Item.Children.Add(mediaItem_Title);
                                    DeviceMediaList.Items.Add(mediaItem_Item);
                                }

                                MyMediaInfo.bCurrentDirectory = false;
                                int index = -1;
                                foreach (var MediaItem in rootMediaItems)
                                {
                                    index++;
                                    if (MyMediaInfo.mediaItem != null && MediaItem.Title == MyMediaInfo.mediaItem.Title)
                                    {
                                        MyMediaInfo.bCurrentDirectory = true;
                                        MyMediaInfo.bLoadFile = false;
                                        DeviceMediaList.SelectedIndex = MyMediaInfo.mediaContainers.Count() + index + 1;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                MyMediaInfo.bCurrentDirectory = false;
                            }
                            PopupBackground.Visibility = Visibility.Collapsed;
                        }
                        else        //rootObjects == null
                        {
                            PopupBackground.Visibility = Visibility.Collapsed;
                            this.BackToUpperlevel();
                        }
                        break;
                    }
                }
                if (!bDMSFounded)
                {
                    PopupBackground.Visibility = Visibility.Collapsed;
                    this.BackToUpperlevel();
                }
            }
            else                   //stackMediaContainer.Count != 0    非设备的第一层根目录
            {
                PopupBackground.Visibility = Visibility.Visible;
                var ContainerToBrowse = MyMediaInfo.stackMediaContainer.Peek();
                var rootObjects = await MyMediaInfo.mediaServer.BrowseAsync(ContainerToBrowse);
                if (rootObjects != null)
                {
                    StackPanel uplevel_Item = new StackPanel();
                    uplevel_Item.Orientation = Orientation.Horizontal;
                    Image uplevel_Icon = new Image();
                    uplevel_Icon.Width = 30;
                    uplevel_Icon.Height = 30;
                    uplevel_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/toolbar_uplevel.png"));
                    TextBlock uplevel_Title = new TextBlock();
                    uplevel_Title.Text = "... " + MyMediaInfo.mediaServer.FriendlyName;
                    uplevel_Title.FontSize = 20;
                    uplevel_Item.Children.Add(uplevel_Icon);
                    uplevel_Item.Children.Add(uplevel_Title);
                    DeviceMediaList.Items.Add(uplevel_Item);

                    var rootContainers = rootObjects.OfType<MediaContainer>();
                    Containers_Count = rootContainers.Count();
                    MyMediaInfo.mediaContainers = rootContainers;
                    var rootMediaItems = rootObjects.OfType<MediaItem>();
                    MediaItems_Count = rootMediaItems.Count();
                    MyMediaInfo.mediaItems = rootMediaItems;
                    if (Containers_Count > 0)
                    {
                        foreach (var MediaContainer in rootContainers)
                        {
                            StackPanel mediaContainer_Item = new StackPanel();
                            mediaContainer_Item.Orientation = Orientation.Horizontal;
                            Image mediaContainer_Icon = new Image();
                            mediaContainer_Icon.Width = 35;
                            mediaContainer_Icon.Height = 35;
                            mediaContainer_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/folder.png"));
                            TextBlock mediaContainer_Title = new TextBlock();
                            mediaContainer_Title.Text = MediaContainer.Title;
                            mediaContainer_Title.FontSize = 22;
                            mediaContainer_Item.Children.Add(mediaContainer_Icon);
                            mediaContainer_Item.Children.Add(mediaContainer_Title);
                            DeviceMediaList.Items.Add(mediaContainer_Item);
                        }
                    }

                    if (MediaItems_Count > 0)
                    {
                        foreach (var MediaItem in rootMediaItems)
                        {
                            StackPanel mediaItem_Item = new StackPanel();
                            mediaItem_Item.Orientation = Orientation.Horizontal;
                            Image mediaItem_Icon = new Image();
                            mediaItem_Icon.Width = 35;
                            mediaItem_Icon.Height = 35;
                            if (MediaItem.Class == "object.item.imageItem")
                            {
                                mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                            }
                            else if (MediaItem.Class == "object.item.audioItem" || MediaItem.Class == "object.item.audioItem.musicTrack")
                            {
                                mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_audio.png"));
                            }
                            else if (MediaItem.Class == "object.item.videoItem")
                            {
                                mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_video.png"));
                            }
                            TextBlock mediaItem_Title = new TextBlock();
                            mediaItem_Title.Text = MediaItem.Title;
                            mediaItem_Title.FontSize = 22;
                            mediaItem_Item.Children.Add(mediaItem_Icon);
                            mediaItem_Item.Children.Add(mediaItem_Title);
                            DeviceMediaList.Items.Add(mediaItem_Item);
                        }

                        MyMediaInfo.bCurrentDirectory = false;
                        int index = -1;
                        foreach (var MediaItem in rootMediaItems)
                        {
                            index++;
                            if (MyMediaInfo.mediaItem != null && MediaItem.Title == MyMediaInfo.mediaItem.Title)
                            {
                                MyMediaInfo.bCurrentDirectory = true;
                                MyMediaInfo.bLoadFile = false;
                                DeviceMediaList.SelectedIndex = MyMediaInfo.mediaContainers.Count() + index + 1;
                                break;
                            }
                        }
                    }
                    else
                    {
                        MyMediaInfo.bCurrentDirectory = false;
                    }
                    PopupBackground.Visibility = Visibility.Collapsed;
                }
                else             //rootObjects == null
                {
                    PopupBackground.Visibility = Visibility.Collapsed;
                    this.BackToUpperlevel();
                }
            }
        }

        private void volumeButton_Click(object sender, RoutedEventArgs e)
        {
            if (volumeSlider.Visibility == Visibility.Collapsed)
            {
                volumeSlider.Visibility = Visibility.Visible;
            }
            else
            {
                volumeSlider.Visibility = Visibility.Collapsed;
            }
        }
        private async void playButton_Click(object sender, RoutedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            await MyMediaInfo.mediaRenderer.PlayAsync();
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private async void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            await MyMediaInfo.mediaRenderer.PauseAsync();
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private async void stopButton_Click(object sender, RoutedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            await MyMediaInfo.mediaRenderer.StopAsync();
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private async void previousButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyMediaInfo.bCurrentDirectory)
            {
                if (DeviceMediaList.SelectedIndex == MyMediaInfo.mediaContainers.Count() + 1)
                {
                    DeviceMediaList.SelectedIndex = MyMediaInfo.mediaContainers.Count() + MyMediaInfo.mediaItemsforSwitch.Count();
                }
                else
                {
                    DeviceMediaList.SelectedIndex = DeviceMediaList.SelectedIndex - 1;
                }
            }
            else
            {
                if (MyMediaInfo.mediaItemIndex == 0)
                {
                    MyMediaInfo.mediaItemIndex = MyMediaInfo.mediaItemsforSwitch.Count() - 1;
                }
                else
                {
                    MyMediaInfo.mediaItemIndex = MyMediaInfo.mediaItemIndex - 1;
                }
                PopupBackground.Visibility = Visibility.Visible;
                MyMediaInfo.mediaItem = MyMediaInfo.mediaItemsforSwitch.ElementAt(MyMediaInfo.mediaItemIndex);
                if (MyMediaInfo.mediaRenderer != null)
                {
                    await MyMediaInfo.mediaRenderer.StopAsync();
                    await MyMediaInfo.mediaRenderer.OpenAsync(MyMediaInfo.mediaItem);
                    await MyMediaInfo.mediaRenderer.PlayAsync();
                    PopupBackground.Visibility = Visibility.Collapsed;
                   
                    timelineSlider.Maximum = MyMediaInfo.mediaItem.Resources.ElementAt(0).Duration.TotalSeconds;
                    var bg = new ImageBrush();
                    bg.Stretch = Stretch.Uniform;
                    if (MyMediaInfo.mediaItem.Class == "object.item.imageItem")
                    {
                        //mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                        var imageUri = MyMediaInfo.mediaItem.Resources.ElementAt(0).Uri;
                        var imagerecv = new BitmapImage();
                        imagerecv.ImageOpened += imagerevd_ImageOpened;
                        imagerecv.UriSource = new Uri(imageUri);
                        dmrImage.Source = imagerecv;
                        dmrImage.Opacity = 1;
                        Output.Background = null;
                        var mediaRendererState = await MyMediaInfo.mediaRenderer.GetCurrentState();
                        MyMediaInfo.mediaRendererState = mediaRendererState;
                        if (mediaRendererState == MediaRendererState.NoMediaRenderer)
                        {
                            MyMediaInfo.mediaRenderer = null;
                        }
                    }
                    else if (MyMediaInfo.mediaItem.Class == "object.item.audioItem" || MyMediaInfo.mediaItem.Class == "object.item.audioItem.musicTrack")
                    {
                        dmrImage.Opacity = 0;
                        SetVolumePanel.Visibility = Visibility.Visible;
                        bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_music_icon.png"));
                        Output.Background = bg;
                    }
                    else if (MyMediaInfo.mediaItem.Class == "object.item.videoItem")
                    {
                        dmrImage.Opacity = 0;
                        SetVolumePanel.Visibility = Visibility.Visible;
                        bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_video_icon.png"));
                        Output.Background = bg;
                    }
                    MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                    {
                        MyMediaInfo.mediaRendererState = state;
                        if (state == MediaRendererState.NoMediaRenderer)
                        {
                            MyMediaInfo.mediaRenderer = null;
                            bDMRFounded = false;
                        }
                    });
                }
                else
                {
                    PopupBackground.Visibility = Visibility.Collapsed;
                    var messageDialog = new MessageDialog("Please select one player");
                    await messageDialog.ShowAsync();
                    MymediaFlipview.SelectedIndex = 1;
                }
            }
        }

        private async void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (MyMediaInfo.bCurrentDirectory)
            {
                if (DeviceMediaList.SelectedIndex == MyMediaInfo.mediaContainers.Count() + MyMediaInfo.mediaItemsforSwitch.Count())
                {
                    DeviceMediaList.SelectedIndex = MyMediaInfo.mediaContainers.Count() + 1;
                }
                else
                {
                    DeviceMediaList.SelectedIndex = DeviceMediaList.SelectedIndex + 1;
                }
            }
            else
            {
                if (MyMediaInfo.mediaItemIndex == MyMediaInfo.mediaItemsforSwitch.Count() - 1)
                {
                    MyMediaInfo.mediaItemIndex = 0;
                }
                else
                {
                    MyMediaInfo.mediaItemIndex = MyMediaInfo.mediaItemIndex + 1;
                }
                PopupBackground.Visibility = Visibility.Visible;
                MyMediaInfo.mediaItem = MyMediaInfo.mediaItemsforSwitch.ElementAt(MyMediaInfo.mediaItemIndex);
                if (MyMediaInfo.mediaRenderer != null)
                {
                    await MyMediaInfo.mediaRenderer.StopAsync();
                    await MyMediaInfo.mediaRenderer.OpenAsync(MyMediaInfo.mediaItem);
                    await MyMediaInfo.mediaRenderer.PlayAsync();
                    PopupBackground.Visibility = Visibility.Collapsed;
                    
                    timelineSlider.Maximum = MyMediaInfo.mediaItem.Resources.ElementAt(0).Duration.TotalSeconds;
                    var bg = new ImageBrush();
                    bg.Stretch = Stretch.Uniform;
                    if (MyMediaInfo.mediaItem.Class == "object.item.imageItem")
                    {
                        //mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                        var imageUri = MyMediaInfo.mediaItem.Resources.ElementAt(0).Uri;
                        var imagerecv = new BitmapImage();
                        imagerecv.ImageOpened += imagerevd_ImageOpened;
                        imagerecv.UriSource = new Uri(imageUri);
                        dmrImage.Source = imagerecv;
                        dmrImage.Opacity = 1;
                        Output.Background = null;
                        var mediaRendererState = await MyMediaInfo.mediaRenderer.GetCurrentState();
                        MyMediaInfo.mediaRendererState = mediaRendererState;
                        if (mediaRendererState == MediaRendererState.NoMediaRenderer)
                        {
                            MyMediaInfo.mediaRenderer = null;
                        }
                    }
                    else if (MyMediaInfo.mediaItem.Class == "object.item.audioItem" || MyMediaInfo.mediaItem.Class == "object.item.audioItem.musicTrack")
                    {
                        dmrImage.Opacity = 0;
                        SetVolumePanel.Visibility = Visibility.Visible;
                        bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_music_icon.png"));
                        Output.Background = bg;
                    }
                    else if (MyMediaInfo.mediaItem.Class == "object.item.videoItem")
                    {
                        dmrImage.Opacity = 0;
                        SetVolumePanel.Visibility = Visibility.Visible;
                        bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_video_icon.png"));
                        Output.Background = bg;
                    }
                    MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                    {
                        MyMediaInfo.mediaRendererState = state;
                        if (state == MediaRendererState.NoMediaRenderer)
                        {
                            MyMediaInfo.mediaRenderer = null;
                            bDMRFounded = false;
                        }
                    });
                }
                else
                {
                    PopupBackground.Visibility = Visibility.Collapsed;
                    var messageDialog = new MessageDialog("Please select one player");
                    await messageDialog.ShowAsync();
                    MymediaFlipview.SelectedIndex = 1;
                }
            }
        }

        async void volumeSlider_ValueChanged(object sender, Windows.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
        {
            if (!_sliderpressed)
            {
                PopupBackground.Visibility = Visibility.Visible;
                await MyMediaInfo.mediaRenderer.SetVolume((int)e.NewValue);
                MyMediaInfo.volume = (int)e.NewValue;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
        }

        void volumeSlider_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _sliderpressed = true;
        }

        async void volumeSlider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            await MyMediaInfo.mediaRenderer.SetVolume((int)volumeSlider.Value);
            _sliderpressed = false;
            MyMediaInfo.volume = (int)volumeSlider.Value;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        void timelineSlider_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            timer1.Stop();
            _sliderpressed = true;
        }

        async void timelineSlider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            await MyMediaInfo.mediaRenderer.SetCurrentPosition(TimeSpan.FromSeconds(timelineSlider.Value));
            _sliderpressed = false;
            timer1.Start();
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private void OnWindowSizeChanged(object sender, SizeChangedEventArgs e)
        {
            DeviceMediaList.Height = Window.Current.Bounds.Height - 265;
            MediaRendererList.Height = Window.Current.Bounds.Height - 265;
            if (DisplayProperties.CurrentOrientation == DisplayOrientations.Landscape || DisplayProperties.CurrentOrientation == DisplayOrientations.LandscapeFlipped)
            {
                MediaItemTitle.MaxWidth = Window.Current.Bounds.Width - 100;
                if (this.IsFullScreen)
                {
                    dmrVideo.Width = Window.Current.Bounds.Width;
                    dmrVideo.Height = Window.Current.Bounds.Height;
                    dmrImage.Width = Window.Current.Bounds.Width;
                    dmrImage.Height = Window.Current.Bounds.Height;
                }
                else
                {
                    dmrVideo.Height = Window.Current.Bounds.Height - 365;
                    dmrVideo.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
                    dmrImage.Height = Window.Current.Bounds.Height - 365;
                    dmrImage.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
                }
                timelineSlider.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
                SetVolumePanel.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
                volumeSlider.Width = (Window.Current.Bounds.Height - 365) * 4 / 3;
            }
            else if (DisplayProperties.CurrentOrientation == DisplayOrientations.Portrait || DisplayProperties.CurrentOrientation == DisplayOrientations.PortraitFlipped)
            {
                MediaItemTitle.MaxWidth = Window.Current.Bounds.Width - 100;
                if (this.IsFullScreen)
                {
                    dmrVideo.Width = Window.Current.Bounds.Width;
                    dmrVideo.Height = Window.Current.Bounds.Height;
                    dmrImage.Width = Window.Current.Bounds.Width;
                    dmrImage.Height = Window.Current.Bounds.Height;
                }
                else
                {
                    dmrVideo.Height = (Window.Current.Bounds.Width - 100) * 3 / 4;
                    dmrVideo.Width = Window.Current.Bounds.Width - 100;
                    dmrImage.Height = (Window.Current.Bounds.Width - 100) * 3 / 4;
                    dmrImage.Width = Window.Current.Bounds.Width - 100;
                }
                timelineSlider.Width = Window.Current.Bounds.Width - 100;
                SetVolumePanel.Width = Window.Current.Bounds.Width - 100;
                volumeSlider.Width = Window.Current.Bounds.Width - 100;
            }
        }

        async private void MediaRendererList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!MyMediaInfo.IsPlayerSwitched)
            {
                MyMediaInfo.IsPlayerSwitched = !MyMediaInfo.IsPlayerSwitched;
            }
            else
            {
                if (MediaRendererList.SelectedIndex != -1)
                {
                    foreach (var mediaRenderer in mediaRenderersDiscovery.DiscoveredDevices)
                    {
                        if (mediaRenderer.FriendlyName == MediaRendererList.SelectedItem.ToString())
                        {
                            bDMRFounded = true;
                            if (MyMediaInfo.mediaRenderer == null)
                            {
                                MyMediaInfo.mediaRenderer = mediaRenderer;
                                bDMRFounded = true;
                                MyMediaInfo.IsPlayerSwitched = false;
                                if (MyMediaInfo.mediaItem != null)
                                {
                                    PopupBackground.Visibility = Visibility.Visible;
                                    await MyMediaInfo.mediaRenderer.StopAsync();
                                    await MyMediaInfo.mediaRenderer.OpenAsync(MyMediaInfo.mediaItem);
                                    await MyMediaInfo.mediaRenderer.PlayAsync();
                                    PopupBackground.Visibility = Visibility.Collapsed;
                                    MymediaFlipview.SelectedIndex = 2;
                                    
                                    timelineSlider.Maximum = MyMediaInfo.mediaItem.Resources.ElementAt(0).Duration.TotalSeconds;
                                    var bg = new ImageBrush();
                                    bg.Stretch = Stretch.Uniform;
                                    if (MyMediaInfo.mediaItem.Class == "object.item.imageItem")
                                    {
                                        //mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                                        var imageUri = MyMediaInfo.mediaItem.Resources.ElementAt(0).Uri;
                                        var imagerecv = new BitmapImage();
                                        imagerecv.ImageOpened += imagerevd_ImageOpened;
                                        imagerecv.UriSource = new Uri(imageUri);
                                        dmrImage.Source = imagerecv;
                                        dmrImage.Opacity = 1;
                                        Output.Background = null;
                                        var mediaRendererState = await MyMediaInfo.mediaRenderer.GetCurrentState();
                                        MyMediaInfo.mediaRendererState = mediaRendererState;
                                        if (mediaRendererState == MediaRendererState.NoMediaRenderer)
                                        {
                                            MyMediaInfo.mediaRenderer = null;
                                        }
                                    }
                                    else if (MyMediaInfo.mediaItem.Class == "object.item.audioItem" || MyMediaInfo.mediaItem.Class == "object.item.audioItem.musicTrack")
                                    {
                                        dmrImage.Opacity = 0;
                                        SetVolumePanel.Visibility = Visibility.Visible;
                                        bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_music_icon.png"));
                                        Output.Background = bg;
                                    }
                                    else if (MyMediaInfo.mediaItem.Class == "object.item.videoItem")
                                    {
                                        dmrImage.Opacity = 0;
                                        SetVolumePanel.Visibility = Visibility.Visible;
                                        bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_video_icon.png"));
                                        Output.Background = bg;
                                    }
                                    MyMediaInfo.mediaRenderer.StateChanges.Subscribe(state =>
                                    {
                                        MyMediaInfo.mediaRendererState = state;
                                        if (state == MediaRendererState.NoMediaRenderer)
                                        {
                                            MyMediaInfo.mediaRenderer = null;
                                            bDMRFounded = false;
                                        }
                                    });
                                    MyMediaInfo.mediaRenderer.PositionChanges.Subscribe(position =>
                                    {
                                        MyMediaInfo.currentPosition = position.TotalSeconds;
                                    });
                                    MediaItemTitle.Text = MyMediaInfo.mediaItem.Title;
                                }
                                else
                                {
                                    var messageDialog = new MessageDialog("Please select source");
                                    await messageDialog.ShowAsync();
                                    MymediaFlipview.SelectedIndex = 0;
                                }
                            }
                            else if (mediaRenderer != MyMediaInfo.mediaRenderer)
                            {
                                MyMediaInfo.mediaRenderer = mediaRenderer;
                                MyMediaInfo.mediaItem = null;
                                var messageDialog = new MessageDialog("Please select source");
                                await messageDialog.ShowAsync();
                                MymediaFlipview.SelectedIndex = 0;
                            }
                            break;
                        }
                    }
                    if (!bDMRFounded)
                    {
                        //MediaRendererTitle.Text = "Device does not exist, please refresh the list";
                    }
                }
            }
        }

        private void DMR_Refresh_Click(object sender, RoutedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
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
            if (compNetworkName != MyMediaInfo.networkName)
            {
                mediaRenderersDiscovery = new MediaRenderersDiscovery();
                MyMediaInfo.networkName = compNetworkName;
            }
            MediaRendererList.Items.Clear();
            if (mediaRenderersDiscovery.DiscoveredDevices.Count() != 0)
            {
                foreach (var renderer in mediaRenderersDiscovery.DiscoveredDevices)
                {
                    MediaRendererList.Items.Add(renderer.FriendlyName);
                }

                foreach (var item in MediaRendererList.Items)
                {
                    if (MyMediaInfo.mediaRenderer != null && MyMediaInfo.mediaRenderer.FriendlyName == item.ToString())
                    {
                        MyMediaInfo.IsPlayerSwitched = false;
                        MediaRendererList.SelectedItem = item;
                    }
                }
            }
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        async void timer_Tick2(object sender, object e)
        {
            await receiver.StartAsync();
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

                    EasClientDeviceInformation easClientDeviceInformation = new EasClientDeviceInformation();
                    receiver.FriendlyName = "Genie Media Player (" + easClientDeviceInformation.FriendlyName + ")";

                    DMR_timelineSlider.ValueChanged += timelineSlider_ValueChanged;

                    PointerEventHandler pointerpressedhandler = new PointerEventHandler(slider_PointerEntered);
                    DMR_timelineSlider.AddHandler(Control.PointerPressedEvent, pointerpressedhandler, true);

                    PointerEventHandler pointerreleasedhandler = new PointerEventHandler(slider_PointerCaptureLost);
                    DMR_timelineSlider.AddHandler(Control.PointerCaptureLostEvent, pointerreleasedhandler, true);
                }
            }
            catch (Exception e)
            {
                cbDMRStarted.IsChecked = false;
                StatusNotify.Text = "Player initialization failed, Error: " + e.Message;
            }
        }

        private async void receiver_PlayRequested(PlayToReceiver recv, Object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
                {
                    dmrImage.Opacity = 0;
                    dmrVideo.Opacity = 1;
                    if (!this.IsFullScreen)
                    {
                        FullscreenToggle();
                    }
                    IsPlayReceivedPreMediaLoaded = true;
                    if (currentType == MediaType.Audio)
                    {
                        var bg = new ImageBrush();
                        bg.Stretch = Stretch.Uniform;
                        bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_music_icon.png"));
                        Output.Background = bg;
                    }
                    else
                    {
                        Output.Background = null;
                    }
                    
                    MediaItemTitle.Visibility = Visibility.Collapsed;
                    timelineSlider.Visibility = Visibility.Collapsed;
                    SetVolumePanel.Visibility = Visibility.Collapsed;

                    dmrVideo.Play();
                    DMR_playButton.Visibility = Visibility.Collapsed;
                    DMR_pauseButton.Visibility = Visibility.Visible;
                    DMR_stopButton.IsEnabled = true;

                    double absvalue = (int)Math.Round(dmrVideo.NaturalDuration.TimeSpan.TotalSeconds, MidpointRounding.AwayFromZero);
                    DMR_timelineSlider.Maximum = absvalue;

                    DMR_timelineSlider.StepFrequency = SliderFrequency(dmrVideo.NaturalDuration.TimeSpan);
                    SetupTimer();
                }
                else if (currentType == MediaType.Image)
                {
                    dmrImage.Opacity = 1;
                    dmrVideo.Opacity = 0;
                    if (!this.IsFullScreen)
                    {
                        FullscreenToggle();
                    }
                    Output.Background = null;
                    
                    MediaItemTitle.Visibility = Visibility.Collapsed;
                    timelineSlider.Visibility = Visibility.Collapsed;
                    SetVolumePanel.Visibility = Visibility.Collapsed;

                    dmrImage.Source = imagerevd;
                    receiver.NotifyPlaying();

                    DMR_playButton.IsEnabled = false;
                    DMR_playButton.Visibility = Visibility.Visible;
                    DMR_pauseButton.Visibility = Visibility.Collapsed;
                    DMR_stopButton.IsEnabled = false;
                }
            });
        }

        private async void receiver_PauseRequested(PlayToReceiver recv, Object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
                {
                    if (dmrVideo.CurrentState == MediaElementState.Stopped)
                    {
                        receiver.NotifyPaused();

                        DMR_playButton.IsEnabled = true;
                        DMR_playButton.Visibility = Visibility.Visible;
                        DMR_pauseButton.Visibility = Visibility.Collapsed;
                        DMR_stopButton.IsEnabled = false;
                    }
                    else
                    {
                        dmrVideo.Pause();

                        DMR_playButton.IsEnabled = true;
                        DMR_playButton.Visibility = Visibility.Visible;
                        DMR_pauseButton.Visibility = Visibility.Collapsed;
                        DMR_stopButton.IsEnabled = true;
                    }
                }
            });
        }

        private async void receiver_StopRequested(PlayToReceiver recv, Object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
                {
                    dmrVideo.Stop();
                    receiver.NotifyStopped();

                    DMR_playButton.IsEnabled = true;
                    DMR_playButton.Visibility = Visibility.Visible;
                    DMR_pauseButton.Visibility = Visibility.Collapsed;
                    DMR_stopButton.IsEnabled = false;
                }
                else if (dmrImage != null && currentType == MediaType.Image)
                {
                    dmrImage.Source = null;
                    receiver.NotifyStopped();

                    DMR_playButton.IsEnabled = false;
                    DMR_playButton.Visibility = Visibility.Visible;
                    DMR_pauseButton.Visibility = Visibility.Collapsed;
                    DMR_stopButton.IsEnabled = false;
                }
            });
        }

        private async void receiver_TimeUpdateRequested(PlayToReceiver recv, Object args)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.High, () =>
            {
                if (MyMediaInfo.IsDMRAllowtoPlay)
                {
                    if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
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
                if (MyMediaInfo.IsDMRAllowtoPlay)
                {
                    if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
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
                    if ((currentType == MediaType.Audio || currentType == MediaType.Video) && dmrVideo != null)
                    {
                        //if (this.IsFullScreen)
                        //    FullscreenToggle();
                        dmrVideo.Stop();
                    }
                    else if (currentType == MediaType.Image && dmrImage != null)
                    {
                        //if (this.IsFullScreen)
                        //    FullscreenToggle();
                        dmrImage.Source = null;
                        dmrImage.Opacity = 0;
                    }
                    currentType = MediaType.None;
                    Title.Text = "";

                    DMR_playButton.IsEnabled = false;
                    DMR_playButton.Visibility = Visibility.Visible;
                    DMR_pauseButton.Visibility = Visibility.Collapsed;
                    DMR_stopButton.IsEnabled = false;

                    DMR_timelineSlider.Visibility = Visibility.Collapsed;
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
                        if ((currentType == MediaType.Audio || currentType == MediaType.Video))
                        {
                            if (this.IsFullScreen)
                                FullscreenToggle();
                            dmrVideo.Stop();
                        }
                        //dmrImage.Opacity = 1;
                        //dmrVideo.Opacity = 0;
                    }
                    currentType = MediaType.Image;
                    Title.Text = args.Title;

                    DMR_playButton.IsEnabled = false;
                    DMR_playButton.Visibility = Visibility.Visible;
                    DMR_pauseButton.Visibility = Visibility.Collapsed;
                    DMR_stopButton.IsEnabled = false;

                    DMR_timelineSlider.Visibility = Visibility.Collapsed;
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
                        StatusNotify.Text = exp.Message + " Content Type: " + args.Stream.ContentType;
                    }

                    if (currentType == MediaType.Image)
                    {
                        if (this.IsFullScreen)
                            FullscreenToggle();
                        //dmrImage.Opacity = 0;
                        //dmrVideo.Opacity = 1;
                        dmrImage.Source = null;
                    }
                    if (args.Stream.ContentType.Contains("audio"))
                    {
                        currentType = MediaType.Audio;
                    }
                    else
                    {
                        currentType = MediaType.Video;
                    }
                    
                    Title.Text = args.Title;

                    DMR_playButton.IsEnabled = true;
                    DMR_playButton.Visibility = Visibility.Visible;
                    DMR_pauseButton.Visibility = Visibility.Collapsed;
                    DMR_stopButton.IsEnabled = false;

                    DMR_timelineSlider.Visibility = Visibility.Visible;
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
                if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
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
                if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
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
                if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
                {
                    dmrVideo.Volume = args.Volume;
                }
            });
        }

        private void dmrVideo_VolumeChanged(object sender, RoutedEventArgs e)
        {
            if (MyMediaInfo.IsDMRAllowtoPlay)
            {
                receiver.NotifyVolumeChange(dmrVideo.Volume, dmrVideo.IsMuted);
            }
        }

        private void dmrVideo_RateChanged(object sender, Windows.UI.Xaml.Media.RateChangedRoutedEventArgs e)
        {
            if (MyMediaInfo.IsDMRAllowtoPlay)
            {
                receiver.NotifyRateChange(dmrVideo.PlaybackRate);
            }

        }

        private void dmrVideo_MediaOpened(object sender, RoutedEventArgs e)
        {
            if (MyMediaInfo.IsDMRAllowtoPlay)
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
            if (MyMediaInfo.IsDMRAllowtoPlay)
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
                        DMR_timelineSlider.Value = 0.0;
                        break;
                    default:
                        break;
                }
            }
        }

        private void dmrVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            if (MyMediaInfo.IsDMRAllowtoPlay)
            {
                StopTimer();
                DMR_timelineSlider.Value = 0.0;

                receiver.NotifyEnded();
                if (dmrVideo != null)
                    dmrVideo.Stop();

                DMR_playButton.IsEnabled = true;
                DMR_playButton.Visibility = Visibility.Visible;
                DMR_pauseButton.Visibility = Visibility.Collapsed;
                DMR_stopButton.IsEnabled = false;
            }
        }

        private void dmrVideo_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            if (MyMediaInfo.IsDMRAllowtoPlay)
            {
                receiver.NotifyError();

                DMR_playButton.IsEnabled = false;
                DMR_playButton.Visibility = Visibility.Visible;
                DMR_pauseButton.Visibility = Visibility.Collapsed;
                DMR_stopButton.IsEnabled = false;
            }
        }

        private void dmrVideo_SeekCompleted(object sender, RoutedEventArgs e)
        {
            if (MyMediaInfo.IsDMRAllowtoPlay)
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

            DMR_playButton.IsEnabled = false;
            DMR_playButton.Visibility = Visibility.Visible;
            DMR_pauseButton.Visibility = Visibility.Collapsed;
            DMR_stopButton.IsEnabled = false;
        }

        private void DMR_playButton_Click(object sender, RoutedEventArgs e)
        {
            if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
            {
                IsPlayReceivedPreMediaLoaded = true;
                dmrVideo.Play();

                DMR_playButton.Visibility = Visibility.Collapsed;
                DMR_pauseButton.Visibility = Visibility.Visible;
                DMR_stopButton.IsEnabled = true;

                SetupTimer();
            }
            else if (currentType == MediaType.Image)
            {
                dmrImage.Source = imagerevd;
                receiver.NotifyPlaying();

                DMR_playButton.IsEnabled = false;
                DMR_playButton.Visibility = Visibility.Visible;
                DMR_pauseButton.Visibility = Visibility.Collapsed;
                DMR_stopButton.IsEnabled = false;
            }
        }

        private void DMR_pauseButton_Click(object sender, RoutedEventArgs e)
        {
            if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
            {
                if (dmrVideo.CurrentState == MediaElementState.Stopped)
                {
                    receiver.NotifyPaused();

                    DMR_playButton.IsEnabled = true;
                    DMR_playButton.Visibility = Visibility.Visible;
                    DMR_pauseButton.Visibility = Visibility.Collapsed;
                    DMR_stopButton.IsEnabled = false;
                }
                else
                {
                    dmrVideo.Pause();

                    DMR_playButton.IsEnabled = true;
                    DMR_stopButton.IsEnabled = true;
                }
            }
        }

        private void DMR_stopButton_Click(object sender, RoutedEventArgs e)
        {
            if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
            {
                dmrVideo.Stop();
                receiver.NotifyStopped();

                DMR_playButton.IsEnabled = true;
                DMR_playButton.Visibility = Visibility.Visible;
                DMR_pauseButton.Visibility = Visibility.Collapsed;
                DMR_stopButton.IsEnabled = false;
            }
            else if (dmrImage != null && currentType == MediaType.Image)
            {
                dmrImage.Source = null;
                receiver.NotifyStopped();

                DMR_playButton.IsEnabled = false;
                DMR_playButton.Visibility = Visibility.Visible;
                DMR_pauseButton.Visibility = Visibility.Collapsed;
                DMR_stopButton.IsEnabled = false;
            }
        }

        private void DMR_closeButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.IsFullScreen)
            {
                FullscreenToggle();
            }
            MediaItemTitle.Visibility = Visibility.Visible;
            dmrVideo.Opacity = 0;
            dmrImage.Opacity = 0;
            timelineSlider.Visibility = Visibility.Visible;

            if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
            {
                dmrVideo.Stop();
                receiver.NotifyStopped();
                DMR_playButton.IsEnabled = true;
                DMR_playButton.Visibility = Visibility.Visible;
                DMR_pauseButton.Visibility = Visibility.Collapsed;
                DMR_stopButton.IsEnabled = false;
            }
            else if (dmrImage != null && currentType == MediaType.Image)
            {
                dmrImage.Source = null;
                receiver.NotifyStopped();
                DMR_playButton.IsEnabled = false;
                DMR_playButton.Visibility = Visibility.Visible;
                DMR_pauseButton.Visibility = Visibility.Collapsed;
                DMR_stopButton.IsEnabled = false;
            }
        }

        private void FullscreenToggle()
        {
            this.IsFullScreen = !this.IsFullScreen;

            if (this.IsFullScreen)
            {
                Header.Visibility = Visibility.Collapsed;

                _previousLayoutrootmargin = LayoutRoot.Margin;
                _previousFlipViewMargin = MymediaFlipview.Margin;
                MymediaFlipview.Margin = new Thickness(0);
                MymediaFlipview.Items.Remove(SourcePanel);
                MymediaFlipview.Items.Remove(PlayerPanel);

                StartDMRPanel.Visibility = Visibility.Collapsed;
                StatusNotify.Visibility = Visibility.Collapsed;
                ControlMediaPanel.Visibility = Visibility.Collapsed;

                _previousBGColor = LayoutRoot.Background;
                LayoutRoot.Margin = new Thickness();
                LayoutRoot.Background = new SolidColorBrush(Colors.Black);

                _previousGridbackground = Output.Background;
                _previousGridMargin = Output.Margin;
                Output.Margin = new Thickness(0);

                BottomAppBar.Visibility = Visibility.Visible;

                if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
                {
                    DMR_timelineSlider.Visibility = Visibility.Visible;
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
                MymediaFlipview.Margin = _previousFlipViewMargin;
                MymediaFlipview.Items.Insert(0, SourcePanel);
                MymediaFlipview.Items.Insert(1, PlayerPanel);

                StartDMRPanel.Visibility = Visibility.Visible;
                StatusNotify.Visibility = Visibility.Visible;
                ControlMediaPanel.Visibility = Visibility.Visible;

                LayoutRoot.Background = _previousBGColor;
                LayoutRoot.Margin = _previousLayoutrootmargin;

                if (_previousGridbackground == null)
                {
                    var bg = new ImageBrush();
                    bg.Stretch = Stretch.Uniform;
                    bg.ImageSource = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/playing_video_icon.png"));
                    Output.Background = bg;
                }
                else
                {
                    Output.Background = _previousGridbackground;
                }
                Output.Margin = _previousGridMargin;

                BottomAppBar.Visibility = Visibility.Collapsed;

                if (dmrVideo != null && (currentType == MediaType.Audio || currentType == MediaType.Video))
                {
                    DMR_timelineSlider.Visibility = Visibility.Collapsed;
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
            if (!_sliderpressed)
            {
                dmrVideo.Position = TimeSpan.FromSeconds(e.NewValue);
            }
        }

        void slider_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            _sliderpressed = true;
        }

        void slider_PointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            dmrVideo.Position = TimeSpan.FromSeconds(DMR_timelineSlider.Value);
            _sliderpressed = false;
        }

        private void SetupTimer()
        {
            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromSeconds(DMR_timelineSlider.StepFrequency);
            StartTimer();
        }

        private void _timer_Tick(object sender, object e)
        {
            if (!_sliderpressed)
            {
                DMR_timelineSlider.Value = dmrVideo.Position.TotalSeconds;
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

        private async void cbDMRStarted_Click(object sender, RoutedEventArgs e)
        {
            if (cbDMRStarted.IsChecked == true)
            {
                try
                {
                    InitialisePlayToReceiver();
                    await receiver.StartAsync();
                    MyMediaInfo.IsDMRAllowtoPlay = true;
                    StatusNotify.Text = "Player started to receive";

                    timer2.Interval = TimeSpan.FromSeconds(5);
                    timer2.Tick += new System.EventHandler<object>(timer_Tick2);
                    timer2.Start();
                }
                catch (Exception ecp)
                {
                    MyMediaInfo.IsDMRAllowtoPlay = false;
                    cbDMRStarted.IsChecked = false;
                    StatusNotify.Text = "Player start failed, Error " + ecp.Message;
                }
            }
            else
            {
                try
                {
                    await receiver.StopAsync();
                    MyMediaInfo.IsDMRAllowtoPlay = false;
                    StatusNotify.Text = "Player stopped to receive";

                    timer2.Stop();
                }
                catch (Exception ecp)
                {
                    MyMediaInfo.IsDMRAllowtoPlay = true;
                    cbDMRStarted.IsChecked = true;
                    StatusNotify.Text = "Player stop failed, Error " + ecp.Message;
                }
            }
        }

        //private async void test_Click(object sender, RoutedEventArgs e)
        //{
        //    var a = await MyMediaInfo.mediaRenderer.GetCurrentState();
        //    System.Diagnostics.Debug.WriteLine("Playback state : {0}", a);
        //}
    }

    public sealed class ThumbToolTipValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return TimeSpan.FromSeconds((double)value).ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
