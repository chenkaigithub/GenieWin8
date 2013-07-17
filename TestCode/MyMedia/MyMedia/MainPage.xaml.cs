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


// “空白页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234238 上有介绍

namespace MyMedia
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
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
            var devices = devicesDiscovery.DiscoveredDevices;

            var mediaServersDiscovery = new MediaServersDiscovery();
            var mediaRenderersDiscovery = new MediaRenderersDiscovery();

            // Enumerating currently available servers
            foreach (var server in mediaServersDiscovery.DiscoveredDevices)
            {
                System.Diagnostics.Debug.WriteLine("Server found: {0}", server.FriendlyName);
            }

            //// Receiving notifications about new media servers added to a network
            //mediaServersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Available).Subscribe(e =>
            //{
            //    Console.WriteLine("Server found: {0}", e.Device.FriendlyName);
            //});

            //// Receiving notifications about media renderers left the network
            //mediaRenderersDiscovery.DevicesActivity.Where(e => e.Activity == DeviceActivity.Gone).Subscribe(e =>
            //{
            //    Console.WriteLine("Renderer gone: {0}", e.Device.FriendlyName);
            //});
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
