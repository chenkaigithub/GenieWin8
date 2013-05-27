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

// 此文件定义的数据模型可充当在添加、移除或修改成员时
// 支持通知的强类型模型的代表性示例。所选
// 属性名称与标准项模板中的数据绑定一致。
//
// 应用程序可以使用此模型作为起始点并以它为基础构建，或完全放弃它并
// 替换为适合其需求的其他内容。

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

    //public class SampleDataItem : DataCommon
    //{
    //    public SampleDataItem(String uniqueId, String title, String subtitle, String imagePath, String description, String content, SampleDataGroup group)
    //        : base(uniqueId, title, subtitle, imagePath, description)
    //    {
    //        this._content = content;
    //        this._group = group;
    //    }

    //    private string _content = string.Empty;
    //    public string Content
    //    {
    //        get { return this._content; }
    //        set { this.SetProperty(ref this._content, value); }
    //    }

    //    private SampleDataGroup _group;
    //    public SampleDataGroup Group
    //    {
    //        get { return this._group; }
    //        set { this.SetProperty(ref this._group, value); }
    //    }
    //}

    public class DataGroup : DataCommon
    {
        public DataGroup(String uniqueId, String title, String imagePath)
            : base(uniqueId, title, imagePath)
        {
            //Items.CollectionChanged += ItemsCollectionChanged;
        }

        //private void ItemsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        //{
        //    // 由于两个原因提供要从 GroupedItemsPage 绑定到的完整
        //    // 项集合的子集: GridView 不会虚拟化大型项集合，并且它
        //    // 可在浏览包含大量项的组时改进用户
        //    // 体验。
        //    //
        //    // 最多显示 12 项，因为无论显示 1、2、3、4 还是 6 行，
        //    // 它都生成填充网格列

        //    switch (e.Action)
        //    {
        //        case NotifyCollectionChangedAction.Add:
        //            if (e.NewStartingIndex < 12)
        //            {
        //                TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
        //                if (TopItems.Count > 12)
        //                {
        //                    TopItems.RemoveAt(12);
        //                }
        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Move:
        //            if (e.OldStartingIndex < 12 && e.NewStartingIndex < 12)
        //            {
        //                TopItems.Move(e.OldStartingIndex, e.NewStartingIndex);
        //            }
        //            else if (e.OldStartingIndex < 12)
        //            {
        //                TopItems.RemoveAt(e.OldStartingIndex);
        //                TopItems.Add(Items[11]);
        //            }
        //            else if (e.NewStartingIndex < 12)
        //            {
        //                TopItems.Insert(e.NewStartingIndex, Items[e.NewStartingIndex]);
        //                TopItems.RemoveAt(12);
        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Remove:
        //            if (e.OldStartingIndex < 12)
        //            {
        //                TopItems.RemoveAt(e.OldStartingIndex);
        //                if (Items.Count >= 12)
        //                {
        //                    TopItems.Add(Items[11]);
        //                }
        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Replace:
        //            if (e.OldStartingIndex < 12)
        //            {
        //                TopItems[e.OldStartingIndex] = Items[e.OldStartingIndex];
        //            }
        //            break;
        //        case NotifyCollectionChangedAction.Reset:
        //            TopItems.Clear();
        //            while (TopItems.Count < Items.Count && TopItems.Count < 12)
        //            {
        //                TopItems.Add(Items[TopItems.Count]);
        //            }
        //            break;
        //    }
        //}

        //private ObservableCollection<SampleDataItem> _items = new ObservableCollection<SampleDataItem>();
        //public ObservableCollection<SampleDataItem> Items
        //{
        //    get { return this._items; }
        //}

        //private ObservableCollection<SampleDataItem> _topItem = new ObservableCollection<SampleDataItem>();
        //public ObservableCollection<SampleDataItem> TopItems
        //{
        //    get { return this._topItem; }
        //}
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
                    "Assets/wireless.png");         
            this.AllGroups.Add(group1);

            strTitle = loader.GetString("GuestAccess");
            var group2 = new DataGroup("GuestAccess",
                    strTitle,
                    "Assets/guestaccess.png");            
            this.AllGroups.Add(group2);

            strTitle = loader.GetString("NetworkMap");
            var group3 = new DataGroup("NetworkMap",
                    strTitle,
                    "Assets/map.png");
            this.AllGroups.Add(group3);

            strTitle = loader.GetString("ParentalControl");
            var group4 = new DataGroup("ParentalControl",
                    strTitle,
                    "Assets/parentalcontrols.png");
            this.AllGroups.Add(group4);

            strTitle = loader.GetString("TrafficControl");
            var group5 = new DataGroup("TrafficControl",
                    strTitle,
                    "Assets/traffic.png");
            this.AllGroups.Add(group5);

            strTitle = loader.GetString("MyMedia");
            var group6 = new DataGroup("MyMedia",
                    strTitle,
                    "Assets/mymedia.png");
            this.AllGroups.Add(group6);

            strTitle = loader.GetString("QRCode");
            var group7 = new DataGroup("QRCode",
                    strTitle,
                    "Assets/qrcode.png");
            this.AllGroups.Add(group7);

            strTitle = loader.GetString("MarketPlace");
            var group8 = new DataGroup("MarketPlace",
                    strTitle,
                    "Assets/appstore.png");
            this.AllGroups.Add(group8);
        }
    }
}
