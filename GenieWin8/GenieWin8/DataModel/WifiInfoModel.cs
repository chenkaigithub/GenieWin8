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
        public static string region;
        public static string wirelessMode;

        public static string changedChannel;
        public static string changedSecurityType;
        public static bool isSSIDChanged;                       //标识SSID是否更改
        public static bool isPasswordChanged;                   //标识密码是否更改
        public static bool isChannelChanged;                    //标识频道是否更改
        public static bool isSecurityTypeChanged;               //标识安全是否更改

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
