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
    }

    public class Node
    {
        public String uniqueId;
        public String deviceName;
        public String deviceType;
        public String RouterFirmware;
        public String IPaddress;
        public String signalStrength;
        public String linkRate;
        public String MACaddress;
        public String connectType;
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
