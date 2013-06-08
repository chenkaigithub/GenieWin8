using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWin8.DataModel
{
    class NetworkMapDodel
    {
        public static Dictionary<string, Dictionary<string, string>> attachDeviceDic = new Dictionary<string, Dictionary<string, string>>();
        public static string geteway;
        public static string fileContent;
    }

    public class Node
    {
        public String uniqueId;
        public String deviceName;
        public String deviceType;
        public String IPaddress;
        public String signalStrength;
        public String linkRate;
        public String MACaddress;
        public String connectType;
        //Edge edgelist;
    }

    //class Edge : Line
    //{

    //}
}
