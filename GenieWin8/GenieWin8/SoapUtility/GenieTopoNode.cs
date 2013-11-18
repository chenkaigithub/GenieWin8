using System.IO;
using System.Text.RegularExpressions;
namespace GenieWin8
{
    /// <summary>
    /// 获取路由器型号对应的图片
    /// </summary>
    class ImageNameGenerator
    {
        private string imagePath;
        private string routerName;
        public ImageNameGenerator(string model)
        {
            routerName = model;
        }
        public string getImagePath()
        {
            string router = getRouterModel();
            //imagePath = string.Format("Assets/router/{0}.png", router.ToLower());
            string[] routers = { "cg3300", "cgd24g", "dg834", "dg834g", "dg834gt", "dg834gv", "dg834n", "dg834pn", "dgn1000_rn", "dgn2000", "dgn2200", "dgn2200m", "dgn3500", "dgnb2100", "dgnd3300", "dgnd3700" 
                               , "dgnd4000", "dm111p", "dm111psp", "jnr3000", "jnr3210", "jwnr2000", "jwnr2000t", "mbm621", "mbr1210_1bmcns", "mbr624gu", "mbrn3000", "r6200", "r6300", "r7000", "rp614", "wgr612"
                               , "wgr614", "wgr614l", "wgt624", "wnb2100", "wndr3300", "wndr3400", "wndr3700", "wndr37av", "wndr3800", "wndr4000", "wndr4500", "wndr4700", "wndrmac", "wnr1000", "wnr2000", "wnr2200"
                               , "wnr3500", "wnr3500l", "wnr500", "wnr612", "wnr834b", "wnr834m", "wnr834t", "wnxr2000", "wpn824", "wpn824ext", "wpn824n" };
            bool bFound = false;
            foreach(string rout in routers)
            {
                if (router == rout)
                {
                    bFound = true;
                    imagePath = string.Format("Assets/router/{0}.png", router.ToLower());
                }
            }
            if (!bFound)
            {
                imagePath = string.Format("Assets/router/{0}.png", "default_netgear");
            }            
            return imagePath;
        }

        public string getRouterModel()
        {
            Regex regx = new Regex("-|_");
            int index = regx.Match(routerName).Index;
            if (index > 0)
            {
                routerName = routerName.Substring(0, index);
            }
            Regex regxv = new Regex("v(\\d+)", RegexOptions.IgnoreCase);
            Match match = regxv.Match(routerName);
            if (match.Success)
            {
                routerName = routerName.Replace(match.ToString(), "").ToUpper();
            }
            else
            {
                routerName = routerName.ToUpper();
            }

            if (routerName == "" || routerName == "N/A")
            {
                routerName = "DEFAULT_NETGEAR";
            }
            return routerName;
        }
    }
}