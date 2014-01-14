﻿using GenieWin8.Data;

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
using Windows.UI.Xaml.Media.Imaging;

using SV.UPnPLite.Protocols.DLNA;
using SV.UPnPLite.Protocols.UPnP;
using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class MyMediaSourcePage : GenieWin8.Common.LayoutAwarePage
    {
        MediaServersDiscovery mediaServersDiscovery = new MediaServersDiscovery();
        bool bDeviceList;       //判断是否为DMS列表
        bool bDeviceFounded = false;
        int Containers_Count = 0;           //该层目录中的媒体文件夹数目
        int MediaItems_Count = 0;           //该层目录中的媒体资源数目
        Uri _baseUri = new Uri("ms-appx:///");

        Stack<IEnumerable<MediaObject>> stackMediaObjects { get; set; }
        Stack<MediaContainer> stackMediaContainer { get; set; }

        //PlayToManager playToManager = null;
        //CoreDispatcher dispatcher = null;
        //enum MediaType { None, Image, AudioVideo };
        //MediaType currentType = MediaType.None;

        //IReadOnlyList<StorageFolder> MediaServers { get; set; }
        //IReadOnlyList<StorageFolder> MediaFolders { get; set; }
        //IReadOnlyList<StorageFile> MediaFiles { get; set; }
        //StorageFile CurrentMediaFile { get; set; }
        //Stack<StorageFolder> PreviousFolders { get; set; }

        public MyMediaSourcePage()
        {
            this.InitializeComponent();
            //PreviousFolders = new Stack<StorageFolder>();
            stackMediaObjects = new Stack<IEnumerable<MediaObject>>();
            stackMediaContainer = new Stack<MediaContainer>();
            this.InitilizeMediaServers();
        }

        DispatcherTimer timer = new DispatcherTimer();
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
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Tick += new System.EventHandler<object>(timer_Tick);
            timer.Start();
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
        }

        private void InitilizeMediaServers()
        {
            DeviceMediaList.Items.Clear();
            if (mediaServersDiscovery.DiscoveredDevices.Count() == 0)
            {
                MediaTitle.Text = "No MediaServers found";
            }
            else
            {
                foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                {
                    DeviceMediaList.Items.Add(server.FriendlyName);
                }
                MediaTitle.Text = "Media Servers refreshed";
            }
            bDeviceList = true;
        }

        private void DeviceMediaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            //VideoPlayer.Stop();
            //MediaList.Items.Clear();
            //PreviousFolders.Clear();
            if (DeviceMediaList.SelectedIndex != -1)
            {
                MediaTitle.Text = "Loading...";
                LoadMediaFiles();
            }
        }

        async private void LoadMediaFiles()
        {
            //VideoPlayer.Stop();
            //DeviceMediaList.Items.Clear();
            //PreviousFolders.Clear();
            if (bDeviceList)
            {
                if (DeviceMediaList.SelectedIndex != -1)
                {
                    //LoadMediaFiles(MediaServers[AvailableMediaDevices.SelectedIndex]);
                    //PreviousFolders.Push(MediaServers[AvailableMediaDevices.SelectedIndex]);
                    foreach (var serverDevice in mediaServersDiscovery.DiscoveredDevices)
                    {
                        if (serverDevice.FriendlyName == DeviceMediaList.SelectedItem.ToString())
                        {
                            bDeviceFounded = true;
                            var rootObjects = await serverDevice.BrowseAsync();
                            if (rootObjects != null)
                            {
                                bDeviceList = false;
                                stackMediaObjects.Push(rootObjects);
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
                                        //DeviceMediaList.Items.Add(" + " + MediaContainer.Title);
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
                                        //DeviceMediaList.Items.Add(MediaItem.Title);
                                    }
                                }
                                MediaTitle.Text = "Load completed";
                            }
                            else        //rootObjects == null
                            {
                                MediaTitle.Text = "Load failed";
                            }
                            break;
                        }
                    }
                    if (!bDeviceFounded)
                    {
                        MediaTitle.Text = "Device does not exist, please refresh the list";
                    }
                }
            } 
            else
            {
                if (DeviceMediaList.SelectedIndex == 0 && stackMediaObjects.Count > 0)             //第一项为返回上一层
                {
                    this.BackToUpperlevel();
                    //stackMediaObjects.Pop();
                    //if (stackMediaContainer.Count > 0)
                    //{
                    //    stackMediaContainer.Pop();
                    //}

                    //if (stackMediaObjects.Count == 0)
                    //{
                    //    DeviceMediaList.Items.Clear();
                    //    //MediaServers = await KnownFolders.MediaServerDevices.GetFoldersAsync();
                    //    if (mediaServersDiscovery.DiscoveredDevices.Count() == 0)
                    //    {
                    //        MediaTitle.Text = "No MediaServers found";
                    //    }
                    //    else
                    //    {
                    //        foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                    //        {
                    //            DeviceMediaList.Items.Add(server.FriendlyName);
                    //        }
                    //        MediaTitle.Text = "Media Servers refreshed";
                    //    }
                    //    bDeviceList = true;
                    //    bDeviceFounded = false;
                    //    stackMediaObjects.Clear();
                    //} 
                    //else
                    //{
                    //    var rootObjects = stackMediaObjects.Peek();
                    //    DeviceMediaList.Items.Clear();
                    //    StackPanel uplevel_Item = new StackPanel();
                    //    uplevel_Item.Orientation = Orientation.Horizontal;
                    //    Image uplevel_Icon = new Image();
                    //    uplevel_Icon.Width = 30;
                    //    uplevel_Icon.Height = 30;
                    //    uplevel_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/toolbar_uplevel.png"));
                    //    TextBlock uplevel_Title = new TextBlock();
                    //    uplevel_Title.Text = "... " + MyMediaInfo.mediaServer.FriendlyName;
                    //    uplevel_Title.FontSize = 20;
                    //    uplevel_Item.Children.Add(uplevel_Icon);
                    //    uplevel_Item.Children.Add(uplevel_Title);
                    //    DeviceMediaList.Items.Add(uplevel_Item);
                    //    //DeviceMediaList.Items.Add("... " + MyMediaInfo.mediaServer.FriendlyName);
                    //    var rootContainers = rootObjects.OfType<MediaContainer>();
                    //    Containers_Count = rootContainers.Count();
                    //    MyMediaInfo.mediaContainers = rootContainers;
                    //    var rootMediaItems = rootObjects.OfType<MediaItem>();
                    //    MediaItems_Count = rootMediaItems.Count();
                    //    MyMediaInfo.mediaItems = rootMediaItems;

                    //    if (Containers_Count > 0)
                    //    {
                    //        foreach (var MediaContainer in rootContainers)
                    //        {
                    //            StackPanel mediaContainer_Item = new StackPanel();
                    //            mediaContainer_Item.Orientation = Orientation.Horizontal;
                    //            Image mediaContainer_Icon = new Image();
                    //            mediaContainer_Icon.Width = 35;
                    //            mediaContainer_Icon.Height = 35;
                    //            mediaContainer_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/folder.png"));
                    //            TextBlock mediaContainer_Title = new TextBlock();
                    //            mediaContainer_Title.Text = MediaContainer.Title;
                    //            mediaContainer_Title.FontSize = 22;
                    //            mediaContainer_Item.Children.Add(mediaContainer_Icon);
                    //            mediaContainer_Item.Children.Add(mediaContainer_Title);
                    //            DeviceMediaList.Items.Add(mediaContainer_Item);
                    //            //DeviceMediaList.Items.Add(" + " + MediaContainer.Title);
                    //        }
                    //    }
                    //    if (MediaItems_Count > 0)
                    //    {
                    //        foreach (var MediaItem in rootMediaItems)
                    //        {
                    //            StackPanel mediaItem_Item = new StackPanel();
                    //            mediaItem_Item.Orientation = Orientation.Horizontal;
                    //            Image mediaItem_Icon = new Image();
                    //            mediaItem_Icon.Width = 35;
                    //            mediaItem_Icon.Height = 35;
                    //            if (MediaItem.Class == "object.item.imageItem")
                    //            {
                    //                mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                    //            }
                    //            else if (MediaItem.Class == "object.item.audioItem" || MediaItem.Class == "object.item.audioItem.musicTrack")
                    //            {
                    //                mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_audio.png"));
                    //            }
                    //            else if (MediaItem.Class == "object.item.videoItem")
                    //            {
                    //                mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_video.png"));
                    //            }
                    //            TextBlock mediaItem_Title = new TextBlock();
                    //            mediaItem_Title.Text = MediaItem.Title;
                    //            mediaItem_Title.FontSize = 22;
                    //            mediaItem_Item.Children.Add(mediaItem_Icon);
                    //            mediaItem_Item.Children.Add(mediaItem_Title);
                    //            DeviceMediaList.Items.Add(mediaItem_Item);
                    //            //DeviceMediaList.Items.Add(MediaItem.Title);
                    //        }
                    //    }
                    //    MediaTitle.Text = "Load completed";
                    //}
                } 
                else
                {
                    if (DeviceMediaList.SelectedIndex > 0 && DeviceMediaList.SelectedIndex < Containers_Count + 1)
                    {
                        int containerIndex = DeviceMediaList.SelectedIndex - 1;
                        var ContainerToBrowse = MyMediaInfo.mediaContainers.ElementAt(containerIndex);
                        var rootObjects = await MyMediaInfo.mediaServer.BrowseAsync(ContainerToBrowse);
                        if (rootObjects != null)
                        {
                            stackMediaObjects.Push(rootObjects);
                            stackMediaContainer.Push(ContainerToBrowse);
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
                            //DeviceMediaList.Items.Add("... " + MyMediaInfo.mediaServer.FriendlyName);
                            
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
                                    //DeviceMediaList.Items.Add(" + " + MediaContainer.Title);
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
                                    //DeviceMediaList.Items.Add(MediaItem.Title);
                                }
                            }
                            MediaTitle.Text = "Load completed";
                        }
                        else             //rootObjects == null
                        {
                            MediaTitle.Text = "Load failed";
                        }
                    }
                    else if (DeviceMediaList.SelectedIndex > Containers_Count)
                    {
                        int mediaItemIndex = DeviceMediaList.SelectedIndex - (Containers_Count + 1);
                        MyMediaInfo.mediaItem = MyMediaInfo.mediaItems.ElementAt(mediaItemIndex);
                        if (MyMediaInfo.mediaRenderer != null)
                        {
                            await MyMediaInfo.mediaRenderer.StopAsync();
                            await MyMediaInfo.mediaRenderer.OpenAsync(MyMediaInfo.mediaItem);
                            await MyMediaInfo.mediaRenderer.PlayAsync();
                            MediaTitle.Text = "Playing media file...";
                        } 
                        else
                        {
                            //var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                            //var strtext = loader.GetString("selectPlayer");
                            var messageDialog = new MessageDialog("Please select one player");
                            await messageDialog.ShowAsync();
                            MediaTitle.Text = "Play failed";
                        }
                    }
                }
            }
        }

        void timer_Tick(object sender, object e)
        {
            if (bDeviceList)
            {
                DeviceMediaList.Items.Clear();
                //MediaServers = await KnownFolders.MediaServerDevices.GetFoldersAsync();
                if (mediaServersDiscovery.DiscoveredDevices.Count() == 0)
                {
                    MediaTitle.Text = "No MediaServers found";
                }
                else
                {
                    foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                    {
                        DeviceMediaList.Items.Add(server.FriendlyName);
                    }
                    MediaTitle.Text = "Media Servers refreshed";
                }
            }
        }

        //返回上层目录
        private void BackToUpperlevel()
        {
            stackMediaObjects.Pop();
            if (stackMediaContainer.Count > 0)
            {
                stackMediaContainer.Pop();
            }

            if (stackMediaObjects.Count == 0)
            {
                DeviceMediaList.Items.Clear();
                //MediaServers = await KnownFolders.MediaServerDevices.GetFoldersAsync();
                if (mediaServersDiscovery.DiscoveredDevices.Count() == 0)
                {
                    MediaTitle.Text = "No MediaServers found";
                }
                else
                {
                    foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                    {
                        DeviceMediaList.Items.Add(server.FriendlyName);
                    }
                    MediaTitle.Text = "Media Servers refreshed";
                }
                bDeviceList = true;
                bDeviceFounded = false;
                stackMediaObjects.Clear();
            }
            else
            {
                DeviceMediaList.Items.Clear();
                this.BrowseMediaContainer();
                //var rootObjects = stackMediaObjects.Peek();
                //DeviceMediaList.Items.Clear();
                //StackPanel uplevel_Item = new StackPanel();
                //uplevel_Item.Orientation = Orientation.Horizontal;
                //Image uplevel_Icon = new Image();
                //uplevel_Icon.Width = 30;
                //uplevel_Icon.Height = 30;
                //uplevel_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/toolbar_uplevel.png"));
                //TextBlock uplevel_Title = new TextBlock();
                //uplevel_Title.Text = "... " + MyMediaInfo.mediaServer.FriendlyName;
                //uplevel_Title.FontSize = 20;
                //uplevel_Item.Children.Add(uplevel_Icon);
                //uplevel_Item.Children.Add(uplevel_Title);
                //DeviceMediaList.Items.Add(uplevel_Item);
                ////DeviceMediaList.Items.Add("... " + MyMediaInfo.mediaServer.FriendlyName);
                //var rootContainers = rootObjects.OfType<MediaContainer>();
                //Containers_Count = rootContainers.Count();
                //MyMediaInfo.mediaContainers = rootContainers;
                //var rootMediaItems = rootObjects.OfType<MediaItem>();
                //MediaItems_Count = rootMediaItems.Count();
                //MyMediaInfo.mediaItems = rootMediaItems;

                //if (Containers_Count > 0)
                //{
                //    foreach (var MediaContainer in rootContainers)
                //    {
                //        StackPanel mediaContainer_Item = new StackPanel();
                //        mediaContainer_Item.Orientation = Orientation.Horizontal;
                //        Image mediaContainer_Icon = new Image();
                //        mediaContainer_Icon.Width = 35;
                //        mediaContainer_Icon.Height = 35;
                //        mediaContainer_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/folder.png"));
                //        TextBlock mediaContainer_Title = new TextBlock();
                //        mediaContainer_Title.Text = MediaContainer.Title;
                //        mediaContainer_Title.FontSize = 22;
                //        mediaContainer_Item.Children.Add(mediaContainer_Icon);
                //        mediaContainer_Item.Children.Add(mediaContainer_Title);
                //        DeviceMediaList.Items.Add(mediaContainer_Item);
                //        //DeviceMediaList.Items.Add(" + " + MediaContainer.Title);
                //    }
                //}
                //if (MediaItems_Count > 0)
                //{
                //    foreach (var MediaItem in rootMediaItems)
                //    {
                //        StackPanel mediaItem_Item = new StackPanel();
                //        mediaItem_Item.Orientation = Orientation.Horizontal;
                //        Image mediaItem_Icon = new Image();
                //        mediaItem_Icon.Width = 35;
                //        mediaItem_Icon.Height = 35;
                //        if (MediaItem.Class == "object.item.imageItem")
                //        {
                //            mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_image.png"));
                //        }
                //        else if (MediaItem.Class == "object.item.audioItem" || MediaItem.Class == "object.item.audioItem.musicTrack")
                //        {
                //            mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_audio.png"));
                //        }
                //        else if (MediaItem.Class == "object.item.videoItem")
                //        {
                //            mediaItem_Icon.Source = new BitmapImage(new Uri(_baseUri, "Assets/MyMedia/file_video.png"));
                //        }
                //        TextBlock mediaItem_Title = new TextBlock();
                //        mediaItem_Title.Text = MediaItem.Title;
                //        mediaItem_Title.FontSize = 22;
                //        mediaItem_Item.Children.Add(mediaItem_Icon);
                //        mediaItem_Item.Children.Add(mediaItem_Title);
                //        DeviceMediaList.Items.Add(mediaItem_Item);
                //        //DeviceMediaList.Items.Add(MediaItem.Title);
                //    }
                //}
                //MediaTitle.Text = "Load completed";
            }
        }

        private void Refresh_Click(object sender, RoutedEventArgs e)
        {
            DeviceMediaList.Items.Clear();
            if (bDeviceList)
            {
                if (mediaServersDiscovery.DiscoveredDevices.Count() == 0)
                {
                    MediaTitle.Text = "No MediaServers found";
                }
                else
                {
                    foreach (var server in mediaServersDiscovery.DiscoveredDevices)
                    {
                        DeviceMediaList.Items.Add(server.FriendlyName);
                    }
                    MediaTitle.Text = "Media Servers refreshed";
                }
            } 
            else
            {
                this.BrowseMediaContainer();
            }
        }

        private async void BrowseMediaContainer()
        {
            if (stackMediaContainer.Count == 0)         //设备的第一层根目录
            {
                bDeviceFounded = false;
                foreach (var serverDevice in mediaServersDiscovery.DiscoveredDevices)
                {
                    if (serverDevice.FriendlyName == MyMediaInfo.mediaServer.FriendlyName)
                    {
                        bDeviceFounded = true;
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
                                    //DeviceMediaList.Items.Add(" + " + MediaContainer.Title);
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
                                    //DeviceMediaList.Items.Add(MediaItem.Title);
                                }
                            }
                            MediaTitle.Text = "Load completed";
                        }
                        else        //rootObjects == null
                        {
                            this.BackToUpperlevel();
                            MediaTitle.Text = "Load failed, back to device list";
                        }
                        break;
                    }
                }
                if (!bDeviceFounded)
                {
                    this.BackToUpperlevel();
                    MediaTitle.Text = "Device does not exist, please refresh the device list";
                }
            }
            else                   //stackMediaContainer.Count != 0    非设备的第一层根目录
            {
                var ContainerToBrowse = stackMediaContainer.Peek();
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
                    //DeviceMediaList.Items.Add("... " + MyMediaInfo.mediaServer.FriendlyName);

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
                            //DeviceMediaList.Items.Add(" + " + MediaContainer.Title);
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
                            //DeviceMediaList.Items.Add(MediaItem.Title);
                        }
                    }
                    MediaTitle.Text = "Load completed";
                }
                else             //rootObjects == null
                {
                    this.BackToUpperlevel();
                    MediaTitle.Text = "Load failed, back to parent directory";
                }
            }
        }
        //private void Back_Click(object sender, RoutedEventArgs e)
        //{
        //    if (PreviousFolders.Count > 1)
        //    {
        //        PreviousFolders.Pop();
        //        //LoadMediaFiles(PreviousFolders.Peek());
        //    }
        //}

        //private void playButton_Click(object sender, RoutedEventArgs e)
        //{
        //    VideoPlayer.Play();
        //}

        //private void pauseButton_Click(object sender, RoutedEventArgs e)
        //{
        //    VideoPlayer.Pause();
        //}

        //private void SelectReceiver_Click(object sender, RoutedEventArgs e)
        //{
        //    PlayToManager.ShowPlayToUI();
        //}
    }
}
