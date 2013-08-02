using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWP8.DataInfo
{
    class TrafficMeterInfo
    {
        public static string MonthlyLimit;
        public static string RestartDay;
        public static string RestartHour;
        public static string RestartMinute;
        public static string ControlOption;

        public static string isTrafficMeterEnabled;

        public static string TodayUpload;
        public static string TodayDownload;
        public static string YesterdayUpload;
        public static string YesterdayDownload;
        public static string WeekUpload;
        public static string WeekDownload;
        public static string MonthUpload;
        public static string MonthDownload;
        public static string LastMonthUpload;
        public static string LastMonthDownload;

        public static string changedMonthlyLimit;
        public static string changedRestartHour;
        public static string changedRestartMinute;
        public static string changedRestartDay;
        public static string changedControlOption;
        public static bool isMonthlyLimitChanged;                       //标识每月限制是否更改
        public static bool isRestartDayChanged;                         //标识重启日期是否更改
        public static bool isRestartHourChanged;                        //标识重启时间-小时是否更改
        public static bool isRestartMinuteChanged;                      //标识重启时间-分钟是否更改
        public static bool isControlOptionChanged;                      //标识流量限制是否更改
    }
}
