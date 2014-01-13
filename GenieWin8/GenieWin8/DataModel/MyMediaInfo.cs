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
        public static MediaRenderer mediaRenderer;
        public static MediaItem mediaItem;
    }
}
