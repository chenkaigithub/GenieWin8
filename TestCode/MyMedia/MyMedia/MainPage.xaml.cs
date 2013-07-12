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
    }
}
