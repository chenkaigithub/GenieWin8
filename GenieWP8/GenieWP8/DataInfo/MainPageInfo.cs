using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWP8.DataInfo
{
    class MainPageInfo
    {
        public static bool bLogin = false;
        public static string username;
        public static string password;
        public static string rememberpassword;          //记住密码被勾选时，标识保存在本机的密码。
        public static string model;

        public static string navigatedPage;             //表示从登录页面登录后要跳转到的页面
    }
}
