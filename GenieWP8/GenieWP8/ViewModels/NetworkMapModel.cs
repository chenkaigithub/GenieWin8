using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using GenieWP8.Resources;
using GenieWP8.DataInfo;
using Microsoft.Phone.Info;

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
            //Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
            List<List<string>> attachDeviceAll = new List<List<string>>();
            attachDeviceAll = NetworkMapInfo.attachDeviceDic;
            UtilityTool util = new UtilityTool();
            var ipList = util.GetCurrentIpAddresses();
            string loacalIp;
            if (ipList.ToList().Count == 0)
            {
                loacalIp = "0.0.0.0";
            } 
            else
            {
                loacalIp = ipList.ToList()[0];
            }

            Node NodeRouter = new Node();
            NodeRouter.uniqueId = "Router";
            NodeRouter.deviceName = MainPageInfo.model;
            NodeRouter.RouterFirmware = MainPageInfo.firmware;
            NodeRouter.IPaddress = NetworkMapInfo.geteway;
            NodeRouter.MACaddress = WifiSettingInfo.macAddr;
            //var routerGroup = new DeviceGroup(NodeRouter);
            var routerGroup = new DeviceGroup() { NODE = NodeRouter };
            this.DeviceGroups.Add(routerGroup);

            if (attachDeviceAll.Count == 0)
            {
                Node NodeDevice = new Node();
                NodeDevice.uniqueId = "LocalDevice";
                NodeDevice.deviceName = DeviceStatus.DeviceManufacturer + " " + DeviceStatus.DeviceName;
                NodeDevice.IPaddress = loacalIp;
                NodeDevice.MACaddress = "";
                NodeDevice.connectType = "wireless";
                NodeDevice.AccessControl = "";
                //var group = new DeviceGroup(NodeDevice);
                var group = new DeviceGroup() { NODE = NodeDevice };
                this.DeviceGroups.Add(group);
            } 
            else
            {
                //foreach (string key in attachDeviceAll.Keys)                                                //先找出本设备放在设备列表第二个（第一个位路由器）
                foreach (List<string> deviceInfo in attachDeviceAll)
                {
                    Node NodeDevice = new Node();

                    //if (loacalIp == attachDeviceAll[key]["Ip"])
                    if (loacalIp == deviceInfo.ElementAt(0))
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
                                    //if (DeviceInfo[0] == key)
                                    if (DeviceInfo[0] == deviceInfo.ElementAt(2))
                                    {
                                        bFound = true;
                                        if (DeviceInfo[1] != "")
                                            NodeDevice.deviceName = DeviceInfo[1];
                                        else
                                            //NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                                            NodeDevice.deviceName = deviceInfo.ElementAt(1);

                                        if (DeviceInfo[2] != "")
                                            NodeDevice.deviceType = DeviceInfo[2];
                                        else
                                            NodeDevice.deviceType = "networkdev";
                                    }
                                }
                            }
                            if (!bFound)
                            {
                                //NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                                NodeDevice.deviceName = deviceInfo.ElementAt(1);
                                NodeDevice.deviceType = "networkdev";
                            }
                        }
                        else
                        {
                            //NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                            NodeDevice.deviceName = deviceInfo.ElementAt(1);
                            NodeDevice.deviceType = "networkdev";
                        }
                        //NodeDevice.IPaddress = attachDeviceAll[key]["Ip"];
                        //if (attachDeviceAll[key].ContainsKey("LinkSpeed"))
                        //{
                        //    NodeDevice.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                        //}
                        //else
                        //{
                        //    NodeDevice.linkRate = "";
                        //}

                        //if (attachDeviceAll[key].ContainsKey("Signal"))
                        //{
                        //    NodeDevice.signalStrength = attachDeviceAll[key]["Signal"];
                        //}
                        //else
                        //{
                        //    NodeDevice.signalStrength = "";
                        //}
                        //NodeDevice.MACaddress = key;
                        //NodeDevice.connectType = attachDeviceAll[key]["Connect"];
                        //if (attachDeviceAll[key].ContainsKey("AccessControl"))
                        //{
                        //    NodeDevice.AccessControl = attachDeviceAll[key]["AccessControl"];
                        //}
                        //else
                        //{
                        //    NodeDevice.AccessControl = "";
                        //}
                        NodeDevice.IPaddress = deviceInfo.ElementAt(0);
                        if (deviceInfo.Count > 4)
                        {
                            NodeDevice.linkRate = deviceInfo.ElementAt(4) + "Mbps";
                        }
                        else
                        {
                            NodeDevice.linkRate = "";
                        }

                        if (deviceInfo.Count > 5)
                        {
                            NodeDevice.signalStrength = deviceInfo.ElementAt(5);
                        }
                        else
                        {
                            NodeDevice.signalStrength = "";
                        }

                        NodeDevice.MACaddress = deviceInfo.ElementAt(2);
                        NodeDevice.connectType = deviceInfo.ElementAt(3);
                        if (deviceInfo.Count > 6)
                        {
                            NodeDevice.AccessControl = deviceInfo.ElementAt(6);
                        }
                        else
                        {
                            NodeDevice.AccessControl = "";
                        }
                        //var group = new DeviceGroup(NodeDevice);
                        var group = new DeviceGroup() { NODE = NodeDevice };
                        this.DeviceGroups.Add(group);
                    }
                }   //foreach

                int num = 0;
                //foreach (string key in attachDeviceAll.Keys)
                foreach (List<string> deviceInfo in attachDeviceAll)
                {
                    num++;
                    Node NodeDevice = new Node();
                    //if (loacalIp != attachDeviceAll[key]["Ip"])
                    if (loacalIp != deviceInfo.ElementAt(0))
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
                                    //if (DeviceInfo[0] == key)
                                    if (DeviceInfo[0] == deviceInfo.ElementAt(2))
                                    {
                                        bFound = true;
                                        if (DeviceInfo[1] != "")
                                            NodeDevice.deviceName = DeviceInfo[1];
                                        else
                                            //NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                                            NodeDevice.deviceName = deviceInfo.ElementAt(1);

                                        if (DeviceInfo[2] != "")
                                            NodeDevice.deviceType = DeviceInfo[2];
                                        else
                                            NodeDevice.deviceType = "networkdev";
                                    }
                                }
                            }
                            if (!bFound)
                            {
                                //NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                                NodeDevice.deviceName = deviceInfo.ElementAt(1);
                                NodeDevice.deviceType = "networkdev";
                            }
                        }
                        else
                        {
                            //NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                            NodeDevice.deviceName = deviceInfo.ElementAt(1);
                            NodeDevice.deviceType = "networkdev";
                        }
                        //NodeDevice.IPaddress = attachDeviceAll[key]["Ip"];
                        //if (attachDeviceAll[key].ContainsKey("LinkSpeed"))
                        //{
                        //    NodeDevice.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                        //}
                        //else
                        //{
                        //    NodeDevice.linkRate = "";
                        //}

                        //if (attachDeviceAll[key].ContainsKey("Signal"))
                        //{
                        //    NodeDevice.signalStrength = attachDeviceAll[key]["Signal"];
                        //}
                        //else
                        //{
                        //    NodeDevice.signalStrength = "";
                        //}
                        //NodeDevice.MACaddress = key;
                        //NodeDevice.connectType = attachDeviceAll[key]["Connect"];
                        //if (attachDeviceAll[key].ContainsKey("AccessControl"))
                        //{
                        //    NodeDevice.AccessControl = attachDeviceAll[key]["AccessControl"];
                        //}
                        //else
                        //{
                        //    NodeDevice.AccessControl = "";
                        //}
                        NodeDevice.IPaddress = deviceInfo.ElementAt(0);
                        if (deviceInfo.Count > 4)
                        {
                            NodeDevice.linkRate = deviceInfo.ElementAt(4) + "Mbps";
                        }
                        else
                        {
                            NodeDevice.linkRate = "";
                        }

                        if (deviceInfo.Count > 5)
                        {
                            NodeDevice.signalStrength = deviceInfo.ElementAt(5);
                        }
                        else
                        {
                            NodeDevice.signalStrength = "";
                        }
                        NodeDevice.MACaddress = deviceInfo.ElementAt(2);
                        NodeDevice.connectType = deviceInfo.ElementAt(3);
                        if (deviceInfo.Count > 6)
                        {
                            NodeDevice.AccessControl = deviceInfo.ElementAt(6);
                        }
                        else
                        {
                            NodeDevice.AccessControl = "";
                        }
                        //var group = new DeviceGroup(NodeDevice);
                        var group = new DeviceGroup() { NODE = NodeDevice };
                        this.DeviceGroups.Add(group);
                    }
                }   //foreach
            }   //else
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