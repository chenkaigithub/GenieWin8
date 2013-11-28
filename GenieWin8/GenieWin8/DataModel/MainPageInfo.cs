using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GenieWin8.DataModel
{
    class MainPageInfo
    {
        public static double itemWidth;                        //主页上每个Item的宽度
        public static double itemHeight;                       //主页上每个Item的高度
        public static double itemImageWidth;                   //主页上每个Item的图片宽度
        public static double itemImageHeight;                  //主页上每个Item的图片高度

        public static bool bLogin = false;
        public static string username;
        public static string password;
        public static string rememberpassword;          //记住密码被勾选时，标识保存在本机的密码。
        public static string model;
        public static string firmware;

        public static string navigatedPage;             //表示从登录页面登录后要跳转到的页面
    }
}
