using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWin8.DataModel
{
    class WifiInfoModel
    {
        public static string ssid;
        public static string password;
        public static string channel;
        public static string securityType;
        public static string macAddr;




        private string _ssid;
        private string _password;
        private string _channel;
        private string _securityType;
        public string SSID
        {
            get
            {
                return _ssid;
            }
            set
            {
                this._ssid = value; 
            }
        }

        public string Password
        {
            get
            {
                return _password;
            }
            set
            {
                this._password = value;
            }
        }

        public string Channel
        {
            get
            {
                return _channel;
            }
            set
            {
                this._channel = value;
            }
        }

        public string SecurityType
        {
            get
            {
                return _securityType;
            }
            set
            {
                this._securityType = value;
            }
        }

    }
}
