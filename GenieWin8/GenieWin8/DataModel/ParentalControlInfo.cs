﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWin8.DataModel
{
    class ParentalControlInfo
    {
        //public static string IsParentalControlSupported;
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

        public static string BypassAccounts;
        public static string BypassUsername;
    }
}
