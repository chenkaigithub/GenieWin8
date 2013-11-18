using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using GenieWP8.Resources;
using GenieWP8.DataInfo;

namespace GenieWP8.ViewModels
{
    public class DeviceGroup : INotifyPropertyChanged
    {
        private Node _node;
        /// <summary>
        /// DeviceGroup 属性; 此属性用于标识对象。
        /// </summary>
        /// <returns></returns>
        public Node NODE
        {
            get
            {
                return _node;
            }
            set
            {
                if (value != _node)
                {
                    _node = value;
                    NotifyPropertyChanged("NODE");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class NetworkMapModel : INotifyPropertyChanged
    {
        public NetworkMapModel()
        {
            this.DeviceGroups = new ObservableCollection<DeviceGroup>();
        }

        public ObservableCollection<DeviceGroup> DeviceGroups { get; private set; }

        //public bool IsDataLoaded
        //{
        //    get;
        //    private set;
        //}

        public static DeviceGroup GetGroup(string uniqueId)
        {
            // 对于小型数据集可接受简单线性搜索
            NetworkMapModel _deviceSource = new NetworkMapModel();
            _deviceSource.DeviceGroups.Clear();
            _deviceSource.LoadData();
            var matches = _deviceSource.DeviceGroups.Where((group) => group.NODE.uniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public void LoadData()
        {
            //var group1 = new DeviceGroup() { NODE = "SignalStrength", Title = AppResources.txtSignalStrength, Content = "80%" };
            //this.DeviceGroups.Add(group1);
            Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> deviceInfo = new Dictionary<string, string>();
            attachDeviceAll = NetworkMapInfo.attachDeviceDic;
            UtilityTool util = new UtilityTool();
            var ipList = util.GetCurrentIpAddresses();
            string loacalIp = ipList.ToList()[0];

            Node NodeRouter = new Node();
            NodeRouter.uniqueId = "Router";
            NodeRouter.deviceName = MainPageInfo.model;
            NodeRouter.RouterFirmware = MainPageInfo.firmware;
            NodeRouter.IPaddress = NetworkMapInfo.geteway;
            NodeRouter.MACaddress = WifiSettingInfo.macAddr;
            //var routerGroup = new DeviceGroup(NodeRouter);
            var routerGroup = new DeviceGroup() { NODE = NodeRouter };
            this.DeviceGroups.Add(routerGroup);

            foreach (string key in attachDeviceAll.Keys)                                                //先找出本设备放在设备列表第二个（第一个位路由器）
            {
                Node NodeDevice = new Node();

                if (loacalIp == attachDeviceAll[key]["Ip"])
                {
                    NodeDevice.uniqueId = "LocalDevice";
                    bool bFound = false;
                    if (NetworkMapInfo.fileContent != "")
                    {
                        string[] AllDeviceInfo = NetworkMapInfo.fileContent.Split(';');
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
                    NodeDevice.IPaddress = attachDeviceAll[key]["Ip"];
                    if (attachDeviceAll[key].ContainsKey("LinkSpeed"))
                    {
                        NodeDevice.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                    } 
                    else
                    {
                        NodeDevice.linkRate = "";
                    }

                    if (attachDeviceAll[key].ContainsKey("Signal"))
                    {
                        NodeDevice.signalStrength = attachDeviceAll[key]["Signal"];
                    } 
                    else
                    {
                        NodeDevice.signalStrength = "";
                    }                   
                    NodeDevice.MACaddress = key;
                    NodeDevice.connectType = attachDeviceAll[key]["Connect"];
                    if (attachDeviceAll[key].ContainsKey("AccessControl"))
                    {
                        NodeDevice.AccessControl = attachDeviceAll[key]["AccessControl"];
                    } 
                    else
                    {
                        NodeDevice.AccessControl = "";
                    }
                    //var group = new DeviceGroup(NodeDevice);
                    var group = new DeviceGroup() { NODE = NodeDevice };
                    this.DeviceGroups.Add(group);
                }                
            }

            int num = 0;
            foreach (string key in attachDeviceAll.Keys)
            {
                num++;
                Node NodeDevice = new Node();
                if (loacalIp != attachDeviceAll[key]["Ip"])
                {
                    NodeDevice.uniqueId = "Device" + num.ToString();
                    bool bFound = false;
                    if (NetworkMapInfo.fileContent != "")
                    {
                        string[] AllDeviceInfo = NetworkMapInfo.fileContent.Split(';');
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
                    NodeDevice.IPaddress = attachDeviceAll[key]["Ip"];
                    if (attachDeviceAll[key].ContainsKey("LinkSpeed"))
                    {
                        NodeDevice.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                    }
                    else
                    {
                        NodeDevice.linkRate = "";
                    }

                    if (attachDeviceAll[key].ContainsKey("Signal"))
                    {
                        NodeDevice.signalStrength = attachDeviceAll[key]["Signal"];
                    }
                    else
                    {
                        NodeDevice.signalStrength = "";
                    }
                    NodeDevice.MACaddress = key;
                    NodeDevice.connectType = attachDeviceAll[key]["Connect"];
                    if (attachDeviceAll[key].ContainsKey("AccessControl"))
                    {
                        NodeDevice.AccessControl = attachDeviceAll[key]["AccessControl"];
                    }
                    else
                    {
                        NodeDevice.AccessControl = "";
                    }
                    //var group = new DeviceGroup(NodeDevice);
                    var group = new DeviceGroup() { NODE = NodeDevice };
                    this.DeviceGroups.Add(group);
                }                
            }
            //this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}