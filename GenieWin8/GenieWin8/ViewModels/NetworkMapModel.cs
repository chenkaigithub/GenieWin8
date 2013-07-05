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
            Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
            Dictionary<string, string> deviceInfo = new Dictionary<string, string>();
            attachDeviceAll = NetworkMapDodel.attachDeviceDic;
            UtilityTool util = new UtilityTool();
            string loacalIp = util.GetLocalHostIp();

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
            }
        }
    }
}
