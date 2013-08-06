
using System.IO;
using System.Text.RegularExpressions;
namespace GenieWP8
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
            imagePath = string.Format("Assets/router/{0}.png",router.ToLower());
            if (!File.Exists(imagePath))
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