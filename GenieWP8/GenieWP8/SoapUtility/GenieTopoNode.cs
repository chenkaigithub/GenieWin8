
using System.IO;
using System.Text.RegularExpressions;
namespace GenieWP8
{
    class ImageNameGenerator
    {
        private string imagePath;
        private string routerName;
        public ImageNameGenerator(string model)
        { 
            Regex regx = new Regex("-|_");
            int index = regx.Match(model).Index;
            if (index > 0)
            {
                routerName = model.Substring(0, index);
            }
            regx = new Regex("v(\\d+)",RegexOptions.IgnoreCase);
            Match match = regx.Match(routerName);
            routerName = routerName.Replace(match.ToString(), "").ToUpper();
            if (routerName == "" || routerName == "N/A")
            {
                routerName = "DEFAULT_NETGEAR";
            }
        }
        public string getImagePath()
        {
            imagePath = string.Format("Assets/router/{1}",routerName);
            if (!File.Exists(imagePath))
            {
                imagePath = string.Format("Assets/router/{1}", "DEFAULT_NETGEAR");
            }
            return imagePath;
        }
    }
}