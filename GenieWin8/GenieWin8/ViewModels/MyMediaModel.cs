using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Windows.ApplicationModel.Resources.Core;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using System.Collections.Specialized;

namespace GenieWin8.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class MediaCommon : GenieWin8.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public MediaCommon(String uniqueId, String title, String imagePath)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._imagePath = imagePath;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _title = string.Empty;
        public string Title
        {
            get { return this._title; }
            set { this.SetProperty(ref this._title, value); }
        }

        private ImageSource _image = null;
        private String _imagePath = null;
        public ImageSource Image
        {
            get
            {
                if (this._image == null && this._imagePath != null)
                {
                    this._image = new BitmapImage(new Uri(MediaCommon._baseUri, this._imagePath));
                }
                return this._image;
            }

            set
            {
                this._imagePath = null;
                this.SetProperty(ref this._image, value);
            }
        }

        public void SetImage(String path)
        {
            this._image = null;
            this._imagePath = path;
            this.OnPropertyChanged("Image");
        }
    }

    public class MediaItem : MediaCommon
    {
        public MediaItem(String uniqueId, String title, String imagePath, String content, MyMediaGroup group)
            : base(uniqueId, title, imagePath)
        {
            this._content = content;
            this._group = group;
        }

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }

        private MyMediaGroup _group;
        public MyMediaGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    public class MyMediaGroup : MediaCommon
    {
        public MyMediaGroup(String uniqueId, String title, String imagePath)
            : base(uniqueId, title, imagePath)
        {
        }

        private ObservableCollection<MediaItem> _items = new ObservableCollection<MediaItem>();
        public ObservableCollection<MediaItem> Items
        {
            get { return this._items; }
        }
    }

    public sealed class MediaSource
    {
        private static MediaSource _mediaSource = new MediaSource();

        private ObservableCollection<MyMediaGroup> _mymediaGroups = new ObservableCollection<MyMediaGroup>();
        public ObservableCollection<MyMediaGroup> MyMediaGroups
        {
            get { return this._mymediaGroups; }
        }

        public static IEnumerable<MyMediaGroup> GetMymediaGroups(string uniqueId)
        {
            return _mediaSource.MyMediaGroups;
        }

        public static MyMediaGroup GetSourceGroup(string uniqueId)
        {
            // 对于小型数据集可接受简单线性搜索
            var matches = _mediaSource.MyMediaGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static MyMediaGroup GetPlayerGroup(string uniqueId)
        {
            // 对于小型数据集可接受简单线性搜索
            var matches = _mediaSource.MyMediaGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public MediaSource()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            var strTitle = loader.GetString("MyMediaSource");
            var mymediagroup1 = new MyMediaGroup("MyMediaSource",
                strTitle,
                "Assets/MyMedia/browse.png");
            //mymediagroup1.Items.Add(new MediaItem("Source-1",
            //    "Source",
            //    "Assets/MyMedia/icon48.png",
            //    "ReadyDLNA: R6200",
            //    mymediagroup1));
            //mymediagroup1.Items.Add(new MediaItem("Source-2",
            //    "Source",
            //    "Assets/MyMedia/icon48.png",
            //    "Genie Media Server (iPad Simulator)",
            //    mymediagroup1));
            //mymediagroup1.Items.Add(new MediaItem("Source-3",
            //    "Source",
            //    "Assets/MyMedia/icon48.png",
            //    "Genie Media Server (HTC Incredible S)",
            //    mymediagroup1));
            this.MyMediaGroups.Add(mymediagroup1);

            strTitle = loader.GetString("MyMediaPlayer");
            var mymediagroup2 = new MyMediaGroup("MyMediaPlayer",
                strTitle,
                "Assets/MyMedia/device.png");
            //mymediagroup2.Items.Add(new MediaItem("Player-1",
            //    "Player",
            //    "Assets/MyMedia/icon48.png",
            //    "Genie Media Player (GT-I9100)",
            //    mymediagroup2));
            //mymediagroup2.Items.Add(new MediaItem("Player-2",
            //    "Player",
            //    "Assets/MyMedia/icon48.png",
            //    "Genie Media Player (iPad Simulator)",
            //    mymediagroup2));
            //mymediagroup2.Items.Add(new MediaItem("Player-3",
            //    "Player",
            //    "Assets/MyMedia/icon48.png",
            //    "Genie Media Player (HTC Incredible S)",
            //    mymediagroup2));
            this.MyMediaGroups.Add(mymediagroup2);

            //strTitle = loader.GetString("MyMediaPlaying");
            //var mymediagroup3 = new MyMediaGroup("MyMediaPlaying",
            //        strTitle,
            //        "Assets/MyMedia/playing.png");
            //this.MyMediaGroups.Add(mymediagroup3);

            strTitle = loader.GetString("MyMediaOption");
            var mymediagrou4 = new MyMediaGroup("MyMediaOption",
                    strTitle,
                    "Assets/MyMedia/option.png");
            this.MyMediaGroups.Add(mymediagrou4);
        }
    }
}
