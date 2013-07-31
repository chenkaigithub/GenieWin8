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
            string loacalIp = ipList.ToList()[1];

            Node NodeRouter = new Node();
            NodeRouter.uniqueId = "Router";
            NodeRouter.deviceName = WifiSettingInfo.ssid;
            NodeRouter.IPaddress = NetworkMapInfo.geteway;
            NodeRouter.MACaddress = WifiSettingInfo.macAddr;
            //var routerGroup = new DeviceGroup(NodeRouter);
            var routerGroup = new DeviceGroup() { NODE = NodeRouter };
            this.DeviceGroups.Add(routerGroup);

            int num = 0;
            foreach (string key in attachDeviceAll.Keys)
            {
                num++;
                Node NodeDevice = new Node();

                if (loacalIp == key)
                {
                    NodeDevice.uniqueId = "LocalDevice";
                }
                else
                {
                    NodeDevice.uniqueId = "Device" + num.ToString();
                }

                bool bFound = false;
                if (NetworkMapInfo.fileContent != "")
                {
                    string[] AllDeviceInfo = NetworkMapInfo.fileContent.Split(';');
                    for (int i = 0; i < AllDeviceInfo.Length; i++)
                    {
                        if (AllDeviceInfo[i] != "" && AllDeviceInfo[i] != null)
                        {
                            string[] DeviceInfo = AllDeviceInfo[i].Split(',');
                            if (DeviceInfo[0] == attachDeviceAll[key]["MacAddress"])
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
                NodeDevice.IPaddress = key;
                NodeDevice.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                NodeDevice.signalStrength = attachDeviceAll[key]["Signal"] + "%";
                NodeDevice.MACaddress = attachDeviceAll[key]["MacAddress"];
                NodeDevice.connectType = attachDeviceAll[key]["Connect"];
                //var group = new DeviceGroup(NodeDevice);
                var group = new DeviceGroup() { NODE = NodeDevice };
                this.DeviceGroups.Add(group);
            }
            //Node node1 = new Node();
            //node1.deviceName = "1";
            //node1.deviceType = "networkdev";
            //node1.connectType = "wireless";
            //node1.IPaddress = "1.1.1.1";
            //node1.linkRate = "1";
            //node1.MACaddress = "1";
            //node1.signalStrength = "1";
            //node1.uniqueId = "1";
            //var group1 = new DeviceGroup() { NODE = node1 };
            //this.DeviceGroups.Add(group1);

            //Node node2 = new Node();
            //node2.deviceName = "2";
            //node2.deviceType = "networkdev";
            //node2.connectType = "wireless";
            //node2.IPaddress = "2.2.2.2";
            //node2.linkRate = "2";
            //node2.MACaddress = "2";
            //node2.signalStrength = "2";
            //node2.uniqueId = "2";
            //var group2 = new DeviceGroup() { NODE = node2 };
            //this.DeviceGroups.Add(group2);

            //Node node3 = new Node();
            //node3.deviceName = "3";
            //node3.deviceType = "networkdev";
            //node3.connectType = "wireless";
            //node3.IPaddress = "3.3.3.3";
            //node3.linkRate = "3";
            //node3.MACaddress = "3";
            //node3.signalStrength = "3";
            //node3.uniqueId = "3";
            //var group3 = new DeviceGroup() { NODE = node3 };
            //this.DeviceGroups.Add(group3);

            //Node node4 = new Node();
            //node4.deviceName = "4";
            //node4.deviceType = "networkdev";
            //node4.connectType = "wireless";
            //node4.IPaddress = "4.4.4.4";
            //node4.linkRate = "4";
            //node4.MACaddress = "4";
            //node4.signalStrength = "4";
            //node4.uniqueId = "4";
            //var group4 = new DeviceGroup() { NODE = node4 };
            //this.DeviceGroups.Add(group4);
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