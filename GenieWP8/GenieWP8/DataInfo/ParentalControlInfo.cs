using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWP8.DataInfo
{
    class ParentalControlInfo
    {
        public static string isParentalControlEnabled;

        public static bool IsOpenDNSLoggedIn = false;
        public static string DeviceId;
        public static string Username;
        public static string Password;
        public static string Email;
        public static bool IsUsernameAvailable = false;
        public static bool IsEmptyUsername = true;
        public static bool IsEmptyPassword = true;
        public static bool IsDifferentPassword = true;
        public static bool IsEmptyEmail = true;
        public static bool IsDifferentEmail = true;

        public static string token;
        public static string filterLevel;
        public static string filterLevelSelected;
        public static string categories;
        public static string categoriesSelected;
        public static bool IsCategoriesChanged;

        public static string BypassAccounts;
        public static string BypassUsername;
        public static string BypassChildrenDeviceId;
        public static bool IsBypassUserLoggedIn;

        public static string SavedInfo;
        public static string RouterMacaddr;
    }
}
