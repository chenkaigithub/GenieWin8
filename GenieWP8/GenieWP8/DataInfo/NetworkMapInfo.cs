using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWP8.DataInfo
{
    class NetworkMapInfo
    {
        public static Dictionary<string, Dictionary<string, string>> attachDeviceDic = new Dictionary<string, Dictionary<string, string>>();
        public static string geteway;
        public static string fileContent;
        public static bool bTypeChanged;
        public static bool bRefreshMap;
        public static bool IsAccessControlSupported;
        public static bool IsAccessControlEnabled;
        public static string deviceMacaddr;       //记录访问控制单个设备的Mac地址
    }

    public class Node
    {
        public string uniqueId;
        public string deviceName;
        public string deviceType;
        public string RouterFirmware;
        public string IPaddress;
        public string signalStrength;
        public string linkRate;
        public string MACaddress;
        public string connectType;
        public string AccessControl;
    }

    public class Devicetype
    {
        public string deviceIcon
        {
            get;
            set;
        }

        public string type
        {
            get;
            set;
        }
    }
}
