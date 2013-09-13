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
using Windows.Storage.Search;
using Windows.UI.Core;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace TestFindMediaServer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        IReadOnlyList<StorageFolder> mediaServers = null;
        public MainPage()
        {
            this.InitializeComponent();
            this.InitilizeMediaServers();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            var client = new SSDPClient();
            client.SearchForDevices();
            client.DeviceFound += ClientOnDeviceFound;
        }

        private void dmsRefreshButton_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                Notify1.Text = "Media Servers being refreshed... ";
                InitilizeMediaServers();
            }
        }

        async private void InitilizeMediaServers()
        {
            try
            {
                dmsSelect1.Items.Clear();
                mediaServers = await KnownFolders.MediaServerDevices.GetFoldersAsync();

                if (mediaServers.Count == 0)
                {
                    Notify1.Text = "No MediaServers found";
                }
                else
                {
                    foreach (StorageFolder server in mediaServers)
                    {
                        dmsSelect1.Items.Add(server.DisplayName);
                    }
                    Notify1.Text = "Media Servers refreshed";
                }
            }
            catch (Exception ecp)
            {
                Notify1.Text = "Error querying Media Servers :" + ecp.Message;
            }
        }

        private void dmsSelect_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (dmsSelect1.SelectedIndex != -1)
            {
                //Notify.Text = "Retrieving media files ...";
                //LoadMediaFiles();
            }
        }



        private async void ClientOnDeviceFound(object sender, DeviceFoundEventArgs deviceFoundEventArgs)
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, delegate
            {
                dmsSelect2.Items.Add(deviceFoundEventArgs.Device.DeviceType.friendlyName);
                //DeviceList.Text += deviceFoundEventArgs.Device.DeviceType.friendlyName + Environment.NewLine;
            });

        }

        private void dmsRefreshButton2_Click(object sender, RoutedEventArgs e)
        {
            dmsSelect2.Items.Clear();
            var client = new SSDPClient();
            client.SearchForDevices();
            client.DeviceFound += ClientOnDeviceFound;
        }
    }
}
