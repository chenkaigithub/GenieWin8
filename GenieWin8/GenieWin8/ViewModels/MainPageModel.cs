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
    public abstract class DataCommon : GenieWin8.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public DataCommon(String uniqueId, String title, String imagePath)
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
                    this._image = new BitmapImage(new Uri(DataCommon._baseUri, this._imagePath));
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

        //public override string ToString()
        //{
        //    return this.Title;
        //}
    }

    public class DataGroup : DataCommon
    {
        public DataGroup(String uniqueId, String title, String imagePath)
            : base(uniqueId, title, imagePath)
        {
            //Items.CollectionChanged += ItemsCollectionChanged;
        }
    }

    public sealed class DataSource
    {
        private static DataSource _dataSource = new DataSource();

        private ObservableCollection<DataGroup> _allGroups = new ObservableCollection<DataGroup>();
        public ObservableCollection<DataGroup> AllGroups
        {
            get { return this._allGroups; }
        }

        public static IEnumerable<DataGroup> GetGroups(string uniqueId)
        {
            if (!uniqueId.Equals("AllGroups")) throw new ArgumentException("Only 'AllGroups' is supported as a collection of groups");

            return _dataSource.AllGroups;
        }

        public static DataGroup GetGroup(string uniqueId)
        {
            // 对于小型数据集可接受简单线性搜索
            var matches = _dataSource.AllGroups.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        //public static SampleDataItem GetItem(string uniqueId)
        //{
        //    // 对于小型数据集可接受简单线性搜索
        //    var matches = _sampleDataSource.AllGroups.SelectMany(group => group.Items).Where((item) => item.UniqueId.Equals(uniqueId));
        //    if (matches.Count() == 1) return matches.First();
        //    return null;
        //}

        public DataSource()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            var strTitle = loader.GetString("WiFiSetting");
            var group1 = new DataGroup("WiFiSetting",
                    strTitle,
                    "Assets/MainPage/wireless.png");         
            this.AllGroups.Add(group1);

            strTitle = loader.GetString("GuestAccess");
            var group2 = new DataGroup("GuestAccess",
                    strTitle,
                    "Assets/MainPage/guestaccess.png");            
            this.AllGroups.Add(group2);

            strTitle = loader.GetString("NetworkMap");
            var group3 = new DataGroup("NetworkMap",
                    strTitle,
                    "Assets/MainPage/map.png");
            this.AllGroups.Add(group3);

            strTitle = loader.GetString("ParentalControl");
            var group4 = new DataGroup("ParentalControl",
                    strTitle,
                    "Assets/MainPage/parentalcontrols.png");
            this.AllGroups.Add(group4);

            strTitle = loader.GetString("TrafficMeter");
            var group5 = new DataGroup("TrafficMeter",
                    strTitle,
                    "Assets/MainPage/traffic.png");
            this.AllGroups.Add(group5);

            //strTitle = loader.GetString("MyMedia");
            //var group6 = new DataGroup("MyMedia",
            //        strTitle,
            //        "Assets/MainPage/mymedia.png");
            //this.AllGroups.Add(group6);

            strTitle = loader.GetString("QRCode");
            var group7 = new DataGroup("QRCode",
                    strTitle,
                    "Assets/MainPage/qrcode.png");
            this.AllGroups.Add(group7);

            //strTitle = loader.GetString("MarketPlace");
            //var group8 = new DataGroup("MarketPlace",
            //        strTitle,
            //        "Assets/MainPage/appstore.png");
            //this.AllGroups.Add(group8);
        }
    }
}
