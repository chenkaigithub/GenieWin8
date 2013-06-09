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
using GenieWin8.DataModel;

namespace GenieWin8.Data
{
    //[Windows.Foundation.Metadata.WebHostHidden]
    //public abstract class DeviceCommon : GenieWin8.Common.BindableBase
    //{
    //    public DeviceCommon(String uniqueId, String deviceName, String deviceType, String IPaddress, String signalStrength, String linkRate, String MACaddress)
    //    {
    //        this._uniqueId = uniqueId;
    //        this._deviceName = deviceName;
    //        this._deviceType = deviceType;
    //        this._IPaddress = IPaddress;
    //        this._signalStrength = signalStrength;
    //        this._linkRate = linkRate;
    //        this._MACaddress = MACaddress;
    //    }

    //    private string _uniqueId = string.Empty;
    //    public string UniqueId
    //    {
    //        get { return this._uniqueId; }
    //        set { this.SetProperty(ref this._uniqueId, value); }
    //    }

    //    private string _deviceName = string.Empty;
    //    public string DeviceName
    //    {
    //        get { return this._deviceName; }
    //        set { this.SetProperty(ref this._deviceName, value); }
    //    }

    //    private string _deviceType = string.Empty;
    //    public string DeviceType
    //    {
    //        get { return this._deviceType; }
    //        set { this.SetProperty(ref this._deviceType, value); }
    //    }

    //    private string _IPaddress = string.Empty;
    //    public string IPAddress
    //    {
    //        get { return this._IPaddress; }
    //        set { this.SetProperty(ref this._IPaddress, value); }
    //    }

    //    private string _signalStrength = string.Empty;
    //    public string SignalStrength
    //    {
    //        get { return this._signalStrength; }
    //        set { this.SetProperty(ref this._signalStrength, value); }
    //    }

    //    private string _linkRate = string.Empty;
    //    public string LinkRate
    //    {
    //        get { return this._linkRate; }
    //        set { this.SetProperty(ref this._linkRate, value); }
    //    }

    //    private string _MACaddress = string.Empty;
    //    public string MACAddress
    //    {
    //        get { return this._MACaddress; }
    //        set { this.SetProperty(ref this._MACaddress, value); }
    //    }
    //}

    //public class DeviceItem : DeviceCommon
    //{
    //    public DeviceItem(String uniqueId, String deviceName, String IPaddress, String signalStrength, String linkRate, String MACaddress, DeviceGroup group)
    //        : base(uniqueId)
    //    {
    //        this._deviceName = deviceName;
    //        this._IPaddress = IPaddress;
    //        this._signalStrength = signalStrength;
    //        this._linkRate = linkRate;
    //        this._MACaddress = MACaddress;
    //        this._group = group;
    //    }

    //    private string _deviceName = string.Empty;
    //    public string DeviceName
    //    {
    //        get { return this._deviceName; }
    //        set { this.SetProperty(ref this._deviceName, value); }
    //    }

    //    private string _IPaddress = string.Empty;
    //    public string IPAddress
    //    {
    //        get { return this._IPaddress; }
    //        set { this.SetProperty(ref this._IPaddress, value); }
    //    }

    //    private string _signalStrength = string.Empty;
    //    public string SignalStrength
    //    {
    //        get { return this._signalStrength; }
    //        set { this.SetProperty(ref this._signalStrength, value); }
    //    }

    //    private string _linkRate = string.Empty;
    //    public string LinkRate
    //    {
    //        get { return this._linkRate; }
    //        set { this.SetProperty(ref this._linkRate, value); }
    //    }

    //    private string _MACaddress = string.Empty;
    //    public string MACAddress
    //    {
    //        get { return this._MACaddress; }
    //        set { this.SetProperty(ref this._MACaddress, value); }
    //    }

    //    private DeviceGroup _group;
    //    public DeviceGroup Group
    //    {
    //        get { return this._group; }
    //        set { this.SetProperty(ref this._group, value); }
    //    }
    //}

    //public class DeviceGroup : DeviceCommon
    //{
    //    public DeviceGroup(String uniqueId, String deviceName, String deviceType, String IPaddress, String signalStrength, String linkRate, String MACaddress)
    //        : base(uniqueId, deviceName, deviceType, IPaddress, signalStrength, linkRate, MACaddress)
    //    {
    //    }

    //    //private ObservableCollection<DeviceItem> _items = new ObservableCollection<DeviceItem>();
    //    //public ObservableCollection<DeviceItem> Items
    //    //{
    //    //    get { return this._items; }
    //    //}
    //}
    public class DeviceGroup
    {
        public DeviceGroup(Node node)
        {
            this._node = node;
        }

        private Node _node = null;
        public Node NODE
        {
            get { return this._node; }
        }
    }

    public sealed class DeviceSource
    {
        //private static DeviceSource _deviceSource = new DeviceSource();

        private ObservableCollection<DeviceGroup> _deviceGroups = new ObservableCollection<DeviceGroup>();
        public ObservableCollection<DeviceGroup> DeviceGroups
        {
            get { return this._deviceGroups; }
        }

        public static IEnumerable<DeviceGroup> GetGroups()
        {
            DeviceSource _deviceSource = new DeviceSource();
            return _deviceSource.DeviceGroups;
        }

        DeviceGroup group;
        public static DeviceGroup GetGroup(string uniqueId)
        {
            // 对于小型数据集可接受简单线性搜索
            DeviceSource _deviceSource = new DeviceSource();
            var matches = _deviceSource.DeviceGroups.Where((group) => group.NODE.uniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        //public async void ReadDeviceInfoFile()
        //{
        //    StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
        //    try
        //    {
        //        StorageFile file = await storageFolder.GetFileAsync("CustomDeviceInfo.txt");
        //        if (file != null)
        //        {
        //            fileContent = await FileIO.ReadTextAsync(file);
        //        }
        //    }
        //    catch (FileNotFoundException)
        //    {

        //    }
        //}

        public DeviceSource()
        {
            Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> deviceInfo = new Dictionary<string, string>();
            attachDeviceAll = NetworkMapDodel.attachDeviceDic;
            UtilityTool util = new UtilityTool();
            string loacalIp = util.GetLocalHostIp();

            //var routerGroup = new DeviceGroup("Router",
            //    WifiInfoModel.ssid,
            //    "",
            //    NetworkMapDodel.geteway,
            //    "",
            //    "",
            //    WifiInfoModel.macAddr);
            Node NodeRouter = new Node();
            NodeRouter.uniqueId = "Router";
            NodeRouter.deviceName = WifiInfoModel.ssid;
            NodeRouter.IPaddress = NetworkMapDodel.geteway;
            NodeRouter.MACaddress = WifiInfoModel.macAddr;
            var routerGroup = new DeviceGroup(NodeRouter);
            this.DeviceGroups.Add(routerGroup);

            int num = 0;
            foreach (string key in attachDeviceAll.Keys)
            {
                num++;
                Node NodeDevice = new Node();

                if (loacalIp == attachDeviceAll[key]["Ip"])
                {
                    NodeDevice.uniqueId = "LocalDevice";
                }
                else
                {
                    NodeDevice.uniqueId = "Device" + num.ToString();
                }

                bool bFound = false;
                if (NetworkMapDodel.fileContent != "")
                {
                    string[] AllDeviceInfo = NetworkMapDodel.fileContent.Split(';');
                    for (int i = 0; i < AllDeviceInfo.Length; i++)
                    {
                        if (AllDeviceInfo[i] != "" && AllDeviceInfo[i] != null)
                        {
                            string[] DeviceInfo = AllDeviceInfo[i].Split(',');
                            if (DeviceInfo[0] == key)
                            {
                                bFound = true;
                                if (DeviceInfo[1] != "")
                                    NodeDevice.deviceName = DeviceInfo[1];
                                else
                                    NodeDevice.deviceName = attachDeviceAll[key]["HostName"];

                                if (DeviceInfo[2] != "")
                                    NodeDevice.deviceType = DeviceInfo[2];
                                else
                                    NodeDevice.deviceType = "networkdev";
                            }
                            //else
                            //{
                            //    NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                            //    NodeDevice.deviceType = "networkdev";
                            //}
                        }                        
                    }
                    if (!bFound)
                    {
                        NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                        NodeDevice.deviceType = "networkdev";
                    }
                }
                else
                {
                    NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                    NodeDevice.deviceType = "networkdev";
                }
                //NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                //NodeDevice.deviceType = attachDeviceAll[key]["Connect"];
                NodeDevice.IPaddress = attachDeviceAll[key]["Ip"];
                NodeDevice.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                NodeDevice.signalStrength = attachDeviceAll[key]["Signal"]+"%";
                NodeDevice.MACaddress = key;
                NodeDevice.connectType = attachDeviceAll[key]["Connect"];
                var group = new DeviceGroup(NodeDevice);
                this.DeviceGroups.Add(group);
                //if (loacalIp == attachDeviceAll[key]["Ip"])
                //{
                //    var group = new DeviceGroup("LocalDevice",
                //     attachDeviceAll[key]["HostName"],
                //     attachDeviceAll[key]["Connect"],
                //     attachDeviceAll[key]["Ip"],
                //     attachDeviceAll[key]["LinkSpeed"],
                //     attachDeviceAll[key]["Signal"],
                //     key);
                //    this.DeviceGroups.Add(group);
                //}
                //else
                //{
                //    var group = new DeviceGroup("Device" + i.ToString(),
                //     attachDeviceAll[key]["HostName"],
                //     attachDeviceAll[key]["Connect"],
                //     attachDeviceAll[key]["Ip"],
                //     attachDeviceAll[key]["LinkSpeed"],
                //     attachDeviceAll[key]["Signal"],
                //     key);
                //    this.DeviceGroups.Add(group);
                //}

            }

            //for (int i = 0; i < attachDeviceAll; i++)
            //{
            //   for(int j = 0 j < attachDeviceAll.ke; j++)

            //    deviceInfo = attachDeviceAll;
            //}
            //var group1 = new DeviceGroup("Router",
            //    "WNR3500Lv2",
            //    "",
            //    "192.168.1.1",
            //    "",
            //    "",
            //    "20:4E:7F:04:31:3C");
            //this.DeviceGroups.Add(group1);


            //group.Items.Add(new DeviceItem("Router",
            //    "WNR3500Lv2",
            //    "192.168.1.1",
            //    "",
            //    "",
            //    "20:4E:7F:04:31:3C",
            //    group));
            //group.Items.Add(new DeviceItem("LocalDevice",
            //    "android-25531554966beee3",
            //    "192.168.1.25",
            //    "78%",
            //    "5.5Mbps",
            //    "D4:20:6D:D6:37:D6",
            //    group));
            //this.DeviceGroups.Add(group);

            //var group2 = new DeviceGroup("LocalDevice",
            //    "android-25531554966beee3",
            //    "",
            //    "192.168.1.25",
            //    "78%",
            //    "5.5Mbps",
            //    "D4:20:6D:D6:37:D6");
            //this.DeviceGroups.Add(group2);
            //var group3 = new DeviceGroup("Device-1",
            //    "WN1000RP",
            //    "Network Device",
            //    "192.168.1.250",
            //    "72%",
            //    "72.2Mbps",
            //    "00:8E:F2:FE:7B:5A");
            //this.DeviceGroups.Add(group3);
            //var group4 = new DeviceGroup("Device-2",
            //    "WR3700V4",
            //    "Network Device",
            //    "192.168.1.100",
            //    "",
            //    "",
            //    "10:0D:7F:51:6F:31");
            //this.DeviceGroups.Add(group4);

        }
    }
}
