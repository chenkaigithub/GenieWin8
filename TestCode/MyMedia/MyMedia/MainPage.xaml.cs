using SV.UPnPLite.Protocols.DLNA;
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
using System.Reactive.Linq;
using SV.UPnPLite.Protocols.UPnP;
using Windows.Storage.Pickers;
using Windows.Storage;
using System.Text;
using Windows.Storage.AccessCache;
using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;

// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MyMedia
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        List<string> ServerDeviceList = new List<string>();
        List<string> RenderDeviceList = new List<string>();
        MediaServersDiscovery mediaServersDiscovery = new MediaServersDiscovery();
        MediaRenderersDiscovery mediaRenderersDiscovery = new MediaRenderersDiscovery();

        public MainPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 在此页将要在 Frame 中显示时进行调用。
        /// </summary>
        /// <param name="e">描述如何访问此页的事件数据。Parameter
        /// 属性通常用于配置页。</param>
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        private void Button_Click_1(object sender, RoutedEventArgs ae)
        {
            //ServerDeviceList.Clear();
            //ServerList.Items.Clear();
            var devicesDiscovery = new CommonUPnPDevicesDiscovery();

            // Receiving notifications about new devices added to a network
            devicesDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(e =>
            {
                System.Diagnostics.Debug.WriteLine("{0} found", e.Device.FriendlyName);
            });

            // Receiving notifications about devices left the network
            devicesDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(e =>
            {
                System.Diagnostics.Debug.WriteLine("{0} gone", e.Device.FriendlyName);
            });

            //  Receiving notifications about new devices of specific type added to the network
            var newMediaServers = from activityInfo in devicesDiscovery.DevicesActivity
                                  where activityInfo.Activity == DeviceActivity.Available && activityInfo.Device.DeviceType == "urn:schemas-upnp-org:device:MediaServer"
                                  select activityInfo.Device;

            //newMediaServers.Subscribe(server =>
            //{
            //    Console.WriteLine("{0} found", server.FriendlyName);
            //});

            // Getting currently available devices
            //var devices = devicesDiscovery.DiscoveredDevices;

            //var mediaServersDiscovery = new MediaServersDiscovery();
            //var mediaRenderersDiscovery = new MediaRenderersDiscovery();

            //// Enumerating currently available servers
            //foreach (var server in mediaServersDiscovery.DiscoveredDevices)
            //{
            //    System.Diagnostics.Debug.WriteLine("Server found: {0}", server.FriendlyName);
            //}

            // Receiving notifications about new media servers added to a network
            mediaServersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(e =>
            {
                //System.Diagnostics.Debug.WriteLine("Server found: {0}", e.Device.FriendlyName);
                ServerDeviceList.Add(e.Device.FriendlyName);
            });

            // Receiving notifications about media servers left the network
            mediaServersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(e =>
            {
                //System.Diagnostics.Debug.WriteLine("Server gone: {0}", e.Device.FriendlyName);
                ServerDeviceList.Remove(e.Device.FriendlyName);
            });

            // Receiving notifications about new media renders added to a network
            mediaRenderersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(e =>
            {
                //System.Diagnostics.Debug.WriteLine("Render found: {0}", e.Device.FriendlyName);
                RenderDeviceList.Add(e.Device.FriendlyName);
            });

            // Receiving notifications about media renders left the network
            mediaRenderersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(e =>
            {
                //System.Diagnostics.Debug.WriteLine("Render gone: {0}", e.Device.FriendlyName);
                RenderDeviceList.Remove(e.Device.FriendlyName);
            });
        }

        private async void ListServer_Click(object sender, RoutedEventArgs e)
        {
            ServerList.Items.Clear();
            foreach (var server in mediaServersDiscovery.DiscoveredDevices)
            {
                //System.Diagnostics.Debug.WriteLine("Server found: {0}", server.FriendlyName);
                ServerList.Items.Add(server.FriendlyName);
            }

            //var serverDevice = mediaServersDiscovery.DiscoveredDevices.First();

            // Find all image items
            foreach (var serverDevice in mediaServersDiscovery.DiscoveredDevices)
            {
                //var Images = await serverDevice.SearchAsync<ImageItem>();
                //foreach (var image in Images)
                //{
                //    System.Diagnostics.Debug.WriteLine("Title={0}, Date={1}, Server={2}", image.Title, image.Date, serverDevice.FriendlyName);
                //}

                //var videos = await serverDevice.SearchAsync<VideoItem>();
                //foreach (var video in videos)
                //{
                //    System.Diagnostics.Debug.WriteLine("Title={0}, Genre={1}", video.Title, video.Genre);
                //}

                if (serverDevice.FriendlyName == "ReadyDLNA: R6300v2")
                {
                    var Images = await serverDevice.SearchAsync<ImageItem>();
                    var image = Images.First();
                    foreach (var renderer in mediaRenderersDiscovery.DiscoveredDevices)
                    {
                        if (renderer.FriendlyName == "Genie Media Player (HTC Incredible S)")
                        {
                            await renderer.OpenAsync(image);
                            await renderer.PlayAsync();
                        }
                    }
                    //foreach (var video in videos)
                    //{
                    //    System.Diagnostics.Debug.WriteLine("Title={0}, Genre={1}, Server={2}", video.Title, video.Genre, serverDevice.FriendlyName);
                    //}

                    //foreach (var renderer in mediaRenderersDiscovery.DiscoveredDevices)
                    //{
                    //    if (renderer.FriendlyName == "SDK CS Sample PlayToReceiver")
                    //    {
                    //        var videoItem = await serverDevice.SearchAsync<VideoItem>();
                    //        foreach (var video in videoItem)
                    //        {
                    //            if (video.Title == "贝瓦儿歌 第3集")
                    //            {
                    //                await renderer.OpenAsync(video);
                    //                await renderer.PlayAsync();
                    //            }
                    //        }
                    //    }
                    //}
                }
            }
            //var Images = await serverDevice.SearchAsync<ImageItem>();
            //foreach (var image in Images)
            //{
            //    System.Diagnostics.Debug.WriteLine("Title={0}, Date={1}, Description={2}", image.Title, image.Date, image.Description);
            //}

            // Find all video items
            //var videos = await serverDevice.SearchAsync<VideoItem>();
            //foreach (var video in videos)
            //{
            //    System.Diagnostics.Debug.WriteLine("Title={0}, Genre={1}", video.Title, video.Genre);
            //}
        }

        private void ListRender_Click(object sender, RoutedEventArgs e)
        {
            RenderList.Items.Clear();
            foreach (var render in mediaRenderersDiscovery.DiscoveredDevices)
            {
                //System.Diagnostics.Debug.WriteLine("Server found: {0}", render.FriendlyName);
                RenderList.Items.Add(render.FriendlyName);
            }
        }

        private async void Button_Click_2(object sender, RoutedEventArgs e)
        {
            FileOpenPicker openPicker = new FileOpenPicker();
            openPicker.ViewMode = PickerViewMode.List;
            openPicker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            openPicker.FileTypeFilter.Add("*");
            IReadOnlyList<StorageFile> files = await openPicker.PickMultipleFilesAsync();

            string[] fileNames = new string[files.Count];
            if (files.Count > 0)
            {
                int i = 0;
               // fileNames = new string [files.Count];
                //StringBuilder output = new StringBuilder("Picked files:\n");
                // Application now has read/write access to the picked file(s)
                foreach (StorageFile file in files)
                {
                  //  output.Append(file.Path + "\n");
                    fileNames[i] = file.Path;
                    i++;
                }
              //  OutputTextBlock.Text = output.ToString();
            }
            else
            {
            //    OutputTextBlock.Text = "Operation cancelled.";
            }

            PFOnAccept Fun1 = new PFOnAccept(RefComm.OnAccept);
            PFOnTransfer Fun2 = new PFOnTransfer(RefComm.OnTransfer);
            PFOnFineshed Fun3 = new PFOnFineshed(RefComm.OnFinished);
            PFOnRecvMsg Fun4 = new PFOnRecvMsg(RefComm.OnRecvMsg);
            // 初始化，注册回调函数，帮助DLL保存对象指针ptr
            IntPtr ptr = RefComm.Init(Fun1, Fun2, Fun3, Fun4);
            // 监听端口：7777
            RefComm.ListenSendFile(ptr, 7777);

            //string ip = "172.16.0.34";
            string ip = "172.16.0.15";
            string host = "WIN8";
            string type = "GENIEMAP";
            string text = "ttttt";
            // 发送文本消息
            RefComm.SendText(ptr, 7777, ip, host, type, text);

            // 发送文件
            //string[] FileName = new string[3];
            //fileNames[0] = "D:\\Genie\\Genie.apk";
            //fileNames[1] = "D:\\Genie\\NetAssist.exe";
            //fileNames[2] = "D:\\Genie\\libjingle.zip";
            RefComm.SendFiles(ptr, 7777, ip, host, type, fileNames, 3);

            // 发送目录
           // string folder = "D:\\Genie";
            //RefComm.SendFolder(ptr, 7777, ip, host, type, folder);

            // 
            while (true)
            {
              //  Console.WriteLine("wait");
            }
        }

        private async void Button_Click_3(object sender, RoutedEventArgs e)
        {
            FolderPicker folderPicker = new FolderPicker();
            folderPicker.SuggestedStartLocation = PickerLocationId.Desktop;
            //folderPicker.FileTypeFilter.Add(".docx");
            //folderPicker.FileTypeFilter.Add(".xlsx");
            folderPicker.FileTypeFilter.Add("*");
            StorageFolder folder = await folderPicker.PickSingleFolderAsync();
            if (folder != null)
            {
                // Application now has read/write access to all contents in the picked folder (including other sub-folder contents)
                StorageApplicationPermissions.FutureAccessList.AddOrReplace("PickedFolderToken", folder);
               // OutputTextBlock.Text = "Picked folder: " + folder.Name;
            }
            else
            {
               // OutputTextBlock.Text = "Operation cancelled.";
            }
        }
    }
}
