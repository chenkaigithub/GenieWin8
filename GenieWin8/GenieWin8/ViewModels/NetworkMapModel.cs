using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

namespace GenieWin8.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class DeviceCommon : GenieWin8.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public DeviceCommon(String uniqueId)
        {
            this._uniqueId = uniqueId;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }
    }

    public class DeviceItem : DeviceCommon
    {
        public DeviceItem(String uniqueId, String deviceName, String IPaddress, String signalStrength, String linkRate, String MACaddress, DeviceGroup group)
            : base(uniqueId)
        {
            this._deviceName = deviceName;
            this._IPaddress = IPaddress;
            this._signalStrength = signalStrength;
            this._linkRate = linkRate;
            this._MACaddress = MACaddress;
            this._group = group;
        }

        private string _deviceName = string.Empty;
        public string DeviceName
        {
            get { return this._deviceName; }
            set { this.SetProperty(ref this._deviceName, value); }
        }

        private string _IPaddress = string.Empty;
        public string IPAddress
        {
            get { return this._IPaddress; }
            set { this.SetProperty(ref this._IPaddress, value); }
        }

        private string _signalStrength = string.Empty;
        public string SignalStrength
        {
            get { return this._signalStrength; }
            set { this.SetProperty(ref this._signalStrength, value); }
        }

        private string _linkRate = string.Empty;
        public string LinkRate
        {
            get { return this._linkRate; }
            set { this.SetProperty(ref this._linkRate, value); }
        }

        private string _MACaddress = string.Empty;
        public string MACAddress
        {
            get { return this._MACaddress; }
            set { this.SetProperty(ref this._MACaddress, value); }
        }

        private DeviceGroup _group;
        public DeviceGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    public class DeviceGroup : DeviceCommon
    {
        public DeviceGroup(String uniqueId)
            : base(uniqueId)
        {
        }

        private ObservableCollection<DeviceItem> _items = new ObservableCollection<DeviceItem>();
        public ObservableCollection<DeviceItem> Items
        {
            get { return this._items; }
        }
    }

    public sealed class DeviceSource
    {
        private static DeviceSource _deviceSource = new DeviceSource();

        private ObservableCollection<DeviceGroup> _deviceGroups = new ObservableCollection<DeviceGroup>();
        public ObservableCollection<DeviceGroup> DeviceGroups
        {
            get { return this._deviceGroups; }
        }

        public static IEnumerable<DeviceGroup> GetGroups(string uniqueId)
        {
            return _deviceSource.DeviceGroups;
        }

        public DeviceSource()
        {
            var group = new DeviceGroup("DeviceGroup");
            group.Items.Add(new DeviceItem("Router",
                "WNR3500Lv2",
                "192.168.1.1",
                "",
                "",
                "20:4E:7F:04:31:3C",
                group));
            group.Items.Add(new DeviceItem("LocalDevice",
                "android-25531554966beee3",
                "192.168.1.25",
                "78%",
                "5.5Mbps",
                "D4:20:6D:D6:37:D6",
                group));
            this.DeviceGroups.Add(group);
        }
    }
}
