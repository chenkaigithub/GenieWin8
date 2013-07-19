using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GenieWP8.Resources;

namespace GenieWP8.ViewModels
{
    public class ItemViewModel : INotifyPropertyChanged
    {
        private string _id;
        /// <summary>
        /// 示例 ViewModel 属性; 此属性用于标识对象。
        /// </summary>
        /// <returns></returns>
        public string ID
        {
            get
            {
                return _id;
            }
            set
            {
                if (value != _id)
                {
                    _id = value;
                    NotifyPropertyChanged("ID");
                }
            }
        }

        private string _title;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string Title
        {
            get
            {
                return _title;
            }
            set
            {
                if (value != _title)
                {
                    _title = value;
                    NotifyPropertyChanged("Title");
                }
            }
        }

        private string _imagePath;
        /// <summary>
        /// 示例 ViewModel 属性；此属性在视图中用于使用绑定显示它的值。
        /// </summary>
        /// <returns></returns>
        public string ImagePath
        {
            get
            {
                return _imagePath;
            }
            set
            {
                if (value != _imagePath)
                {
                    _imagePath = value;
                    NotifyPropertyChanged("ImagePath");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public class MainViewModel : INotifyPropertyChanged
    {
        public MainViewModel()
        {
            this.Items = new ObservableCollection<ItemViewModel>();
        }

        /// <summary>
        /// ItemViewModel 对象的集合。
        /// </summary>
        public ObservableCollection<ItemViewModel> Items { get; private set; }

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        /// <summary>
        /// 创建一些 ItemViewModel 对象并将其添加到 Items 集合中。
        /// </summary>
        public void LoadData()
        {
            // 示例数据；替换为实际数据
            this.Items.Add(new ItemViewModel() { ID = "WiFiSetting", Title = AppResources.WiFiSetting, ImagePath = "Assets/MainPage/wireless.png" });
            this.Items.Add(new ItemViewModel() { ID = "GuestAccess", Title = AppResources.GuestAccess, ImagePath = "Assets/MainPage/guestaccess.png" });
            this.Items.Add(new ItemViewModel() { ID = "NetworkMap", Title = AppResources.NetworkMap, ImagePath = "Assets/MainPage/map.png" });
            this.Items.Add(new ItemViewModel() { ID = "ParentalControl", Title = AppResources.ParentalControl, ImagePath = "Assets/MainPage/parentalcontrols.png" });
            this.Items.Add(new ItemViewModel() { ID = "TrafficMeter", Title = AppResources.TrafficMeter, ImagePath = "Assets/MainPage/traffic.png" });
            this.Items.Add(new ItemViewModel() { ID = "MyMedia", Title = AppResources.MyMedia, ImagePath = "Assets/MainPage/mymedia.png" });
            this.Items.Add(new ItemViewModel() { ID = "QRCode", Title = AppResources.QRCode, ImagePath = "Assets/MainPage/qrcode.png" });
            this.Items.Add(new ItemViewModel() { ID = "MarketPlace", Title = AppResources.MarketPlace, ImagePath = "Assets/MainPage/appstore.png" });

            this.IsDataLoaded = true;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}