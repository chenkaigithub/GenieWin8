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
using Windows.Security.ExchangeActiveSyncProvisioning;

namespace GenieWin8.Data
{  
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

        public DeviceSource()
        {
            //Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
            List<List<string>> attachDeviceAll = new List<List<string>>();
            attachDeviceAll = NetworkMapInfo.attachDeviceDic;
            UtilityTool util = new UtilityTool();
            string loacalIp = util.GetLocalHostIp();

            Node NodeRouter = new Node();
            NodeRouter.uniqueId = "Router";
            NodeRouter.deviceName = MainPageInfo.model;
            NodeRouter.RouterFirmware = MainPageInfo.firmware;
            NodeRouter.IPaddress = NetworkMapInfo.geteway;
            NodeRouter.MACaddress = WifiInfoModel.macAddr;
            var routerGroup = new DeviceGroup(NodeRouter);
            this.DeviceGroups.Add(routerGroup);

            if (attachDeviceAll.Count == 0)
            {
                Node NodeDevice = new Node();
                NodeDevice.uniqueId = "LocalDevice";
                EasClientDeviceInformation easClientDeviceInformation = new EasClientDeviceInformation();
                NodeDevice.deviceName = easClientDeviceInformation.FriendlyName;
                NodeDevice.IPaddress = loacalIp;
                NodeDevice.MACaddress = "";
                NodeDevice.connectType = "wireless";
                NodeDevice.AccessControl = "";
                var group = new DeviceGroup(NodeDevice);
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
                        //    NodeDevice.linkRate = "Mbps";
                        //}

                        //if (attachDeviceAll[key].ContainsKey("Signal"))
                        //{
                        //    NodeDevice.signalStrength = attachDeviceAll[key]["Signal"] + "%";
                        //}
                        //else
                        //{
                        //    NodeDevice.signalStrength = "%";
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
                            NodeDevice.linkRate = "Mbps";
                        }

                        if (deviceInfo.Count > 5)
                        {
                            NodeDevice.signalStrength = deviceInfo.ElementAt(5) + "%";
                        }
                        else
                        {
                            NodeDevice.signalStrength = "%";
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
                        var group = new DeviceGroup(NodeDevice);
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
                                    //else
                                    //{
                                    //    NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                                    //    NodeDevice.deviceType = "networkdev";
                                    //}
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
                        //NodeDevice.deviceName = attachDeviceAll[key]["HostName"];
                        //NodeDevice.deviceType = attachDeviceAll[key]["Connect"];
                        //NodeDevice.IPaddress = attachDeviceAll[key]["Ip"];
                        //if (attachDeviceAll[key].ContainsKey("LinkSpeed"))
                        //{
                        //    NodeDevice.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                        //}
                        //else
                        //{
                        //    NodeDevice.linkRate = "Mbps";
                        //}

                        //if (attachDeviceAll[key].ContainsKey("Signal"))
                        //{
                        //    NodeDevice.signalStrength = attachDeviceAll[key]["Signal"] + "%";
                        //}
                        //else
                        //{
                        //    NodeDevice.signalStrength = "%";
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
                            NodeDevice.linkRate = "Mbps";
                        }

                        if (deviceInfo.Count > 5)
                        {
                            NodeDevice.signalStrength = deviceInfo.ElementAt(5) + "%";
                        }
                        else
                        {
                            NodeDevice.signalStrength = "%";
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
                        var group = new DeviceGroup(NodeDevice);
                        this.DeviceGroups.Add(group);
                    }
                }   //foreach
            }   //else
        }
    }
}
