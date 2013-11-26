using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWin8.DataModel
{
    class NetworkMapInfo
    {
        public static Dictionary<string, Dictionary<string, string>> attachDeviceDic = new Dictionary<string, Dictionary<string, string>>();
        public static string geteway;
        public static string fileContent;
        public static bool bRefreshMap;
        public static bool IsAccessControlSupported;
        public static bool IsAccessControlEnabled;
        public static string deviceMacaddr;       //记录访问控制单个设备的Mac地址
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
        public string AccessControl;
        //Edge edgelist;
    }

    //class Edge : Line
    //{

    //}
}
