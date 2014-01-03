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

using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Core;
using Windows.Media.PlayTo;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class MyMediaSourcePage : GenieWin8.Common.LayoutAwarePage
    {
        PlayToManager playToManager = null;
        CoreDispatcher dispatcher = null;
        enum MediaType { None, Image, AudioVideo };
        MediaType currentType = MediaType.None;

        IReadOnlyList<StorageFolder> MediaServers { get; set; }
        IReadOnlyList<StorageFolder> MediaFolders { get; set; }
        IReadOnlyList<StorageFile> MediaFiles { get; set; }
        StorageFile CurrentMediaFile { get; set; }
        Stack<StorageFolder> PreviousFolders { get; set; }

        public MyMediaSourcePage()
        {
            this.InitializeComponent();
            PreviousFolders = new Stack<StorageFolder>();
            this.InitilizeMediaServers();
        }

        /// <summary>
        /// Invoked when this page is about to be displayed in a Frame.
        /// </summary>
        /// <param name="e">Event data that describes how this page was reached.  The Parameter
        /// property is typically used to configure the page.</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            dispatcher = Window.Current.CoreWindow.Dispatcher;
            playToManager = PlayToManager.GetForCurrentView();
            playToManager.SourceRequested += playToManager_SourceRequested;
        }

        void playToManager_SourceRequested(PlayToManager sender, PlayToSourceRequestedEventArgs args)
        {
            var deferral = args.SourceRequest.GetDeferral();
            var handler = dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (currentType == MediaType.Image)
                {
                    args.SourceRequest.SetSource(ImagePlayer.PlayToSource);
                }
                else if (currentType == MediaType.AudioVideo)
                {
                    args.SourceRequest.SetSource(VideoPlayer.PlayToSource);
                }
                deferral.Complete();
            });
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            playToManager.SourceRequested -= playToManager_SourceRequested;
        }

        async private void InitilizeMediaServers()
        {
            try
            {
                AvailableMediaDevices.Items.Clear();
                MediaServers = await KnownFolders.MediaServerDevices.GetFoldersAsync();
                if (MediaServers.Count == 0)
                {
                    MediaTitle.Text = "No MediaServers found";
                }
                else
                {
                    foreach (StorageFolder server in MediaServers)
                    {
                        AvailableMediaDevices.Items.Add(server.DisplayName);
                    }
                    MediaTitle.Text = "Media Servers refreshed";
                }
            }
            catch (Exception ex)
            {
                MediaTitle.Text = "Error querying Media Servers :" + ex.Message;
            }
        }

        private void AvailableMediaDevices_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            VideoPlayer.Stop();
            MediaList.Items.Clear();
            PreviousFolders.Clear();
            if (AvailableMediaDevices.SelectedIndex != -1)
            {
                MediaTitle.Text = "Retrieving media files ...";
                LoadMediaFiles(MediaServers[AvailableMediaDevices.SelectedIndex]);
                PreviousFolders.Push(MediaServers[AvailableMediaDevices.SelectedIndex]);
            }
        }

        async private void LoadMediaFiles(StorageFolder mediaServerFolder)
        {
            try
            {
                MediaFolders = await mediaServerFolder.GetFoldersAsync();
                MediaList.Items.Clear();
                if (MediaFolders.Count > 0)
                {
                    MediaList.Items.Clear();
                    foreach (StorageFolder folder in MediaFolders)
                    {
                        MediaList.Items.Add(" + " + folder.DisplayName);
                    }
                    MediaTitle.Text = "Media folders retrieved";
                }
                var queryOptions = new QueryOptions();

                var queryFolder = mediaServerFolder.CreateFileQueryWithOptions(queryOptions);
                MediaFiles = await queryFolder.GetFilesAsync();
                if (MediaFiles.Count > 0)
                {
                    foreach (StorageFile file in MediaFiles)
                    {
                        MediaList.Items.Add(file.DisplayName);
                    }
                    MediaTitle.Text = "Media files retrieved";
                }
            }
            catch (Exception ex)
            {
                MediaTitle.Text = "Error locating media files " + ex.Message;
            }
        }

        private async void MediaList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                VideoPlayer.Stop();
                if (MediaList.SelectedIndex != -1 && MediaList.SelectedIndex <
                 MediaFolders.Count && MediaFolders.Count != 0)
                {
                    MediaTitle.Text = "Retrieving media files ...";
                    LoadMediaFiles(MediaFolders[MediaList.SelectedIndex]);
                    PreviousFolders.Push(MediaFolders[MediaList.SelectedIndex]);
                    currentType = MediaType.None;
                }
                else if (MediaList.SelectedIndex != -1 && (MediaList.SelectedIndex >=
                         MediaFolders.Count &&
                         MediaList.SelectedIndex < (MediaFolders.Count + MediaFiles.Count)))
                {
                    CurrentMediaFile = MediaFiles[MediaList.SelectedIndex - MediaFolders.Count];
                    var stream = await CurrentMediaFile.OpenAsync(FileAccessMode.Read);
                    if (CurrentMediaFile.ContentType.Contains("image"))
                    {
                        BitmapImage imagerevd = new BitmapImage();
                        imagerevd.SetSource(stream);
                        ImagePlayer.Source = imagerevd;
                        stpVideoPlayer.Opacity = 0;
                        ImagePlayer.Opacity = 1;
                        currentType = MediaType.Image;
                    }
                    else
                    {
                        VideoPlayer.SetSource(stream, CurrentMediaFile.ContentType);
                        VideoPlayer.Play();
                        stpVideoPlayer.Opacity = 1;
                        ImagePlayer.Opacity = 0;
                        currentType = MediaType.AudioVideo;
                    }
                    MediaTitle.Text = "Playing: " + CurrentMediaFile.DisplayName;
                }
            }
            catch (Exception ex)
            {
                MediaTitle.Text = "Error during file selection :" + ex.Message;
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            if (PreviousFolders.Count > 1)
            {
                PreviousFolders.Pop();
                LoadMediaFiles(PreviousFolders.Peek());
            }
        }

        private void playButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Play();
        }

        private void pauseButton_Click(object sender, RoutedEventArgs e)
        {
            VideoPlayer.Pause();
        }

        private void SelectReceiver_Click(object sender, RoutedEventArgs e)
        {
            PlayToManager.ShowPlayToUI();
        }
    }
}
