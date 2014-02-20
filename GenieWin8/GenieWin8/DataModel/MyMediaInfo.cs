using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SV.UPnPLite.Protocols.DLNA;
using SV.UPnPLite.Protocols.UPnP;
using SV.UPnPLite.Protocols.DLNA.Services.ContentDirectory;

namespace GenieWin8.DataModel
{
    class MyMediaInfo
    {
        public static MediaServer mediaServer;
        public static IEnumerable<MediaContainer> mediaContainers;
        public static IEnumerable<MediaItem> mediaItems;
        public static IEnumerable<MediaItem> mediaItemsforSwitch;
        public static MediaRenderer mediaRenderer;
        public static MediaItem mediaItem;
        public static MediaRendererState mediaRendererState = MediaRendererState.NoMediaPresent;
        public static int volume = 0;
        public static Stack<IEnumerable<MediaObject>> stackMediaObjects = new Stack<IEnumerable<MediaObject>>();
        public static Stack<MediaContainer> stackMediaContainer = new Stack<MediaContainer>();
        public static string networkName;
        public static int mediaItemIndex = -1;
        public static bool bLoadFile = true;
        public static bool bCurrentDirectory;               //判断是否为当前播放媒体资源所在目录
        public static bool bDeviceList = true;              //判断是否为DMS列表
        public static double currentPosition;               //记录当前播放位置
        public static bool IsDMRAllowtoPlay = true;         //判断播放器是否允许在网络中播放
        public static bool IsPlayerSwitched = true;         //判断是否切换了播放器
    }
}
