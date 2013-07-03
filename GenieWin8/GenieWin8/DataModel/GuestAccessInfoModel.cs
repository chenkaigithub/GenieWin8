using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWin8.DataModel
{
    class GuestAccessInfoModel
    {
        public static string isGuestAccessEnabled;
        public static string ssid;
        public static string password;
        public static string timePeriod;
        public static string securityType;

        public static string changedTimePeriod;
        public static string changedSecurityType;
        public static bool isSSIDChanged;                       //标识SSID是否更改
        public static bool isPasswordChanged;                   //标识密码是否更改
        public static bool isTimePeriodChanged;                 //标识时间段是否更改
        public static bool isSecurityTypeChanged;               //标识安全是否更改
    }
}
