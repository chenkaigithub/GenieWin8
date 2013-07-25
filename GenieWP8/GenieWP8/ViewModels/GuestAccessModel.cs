using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

using GenieWP8.Resources;
using GenieWP8.DataInfo;

namespace GenieWP8.ViewModels
{
    public class GuestSettingCommon : INotifyPropertyChanged
    {
        private string _id;
        /// <summary>
        /// GuestSettingCommon 属性; 此属性用于标识对象。
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
        /// GuestSettingCommon 属性；此属性在视图中用于使用绑定显示它的选项名称。
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

        private string _content;
        /// <summary>
        /// GuestSettingCommon 属性；此属性在视图中用于使用绑定显示它的内容。
        /// </summary>
        /// <returns></returns>
        public string Content
        {
            get
            {
                return _content;
            }
            set
            {
                if (value != _content)
                {
                    _content = value;
                    NotifyPropertyChanged("Content");
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

    public class GuestSettingItem : GuestSettingCommon
    {
        private GuestSettingGroup _group;
        public GuestSettingGroup Group
        {
            get
            {
                return _group;
            }
            set
            {
                if (value != _group)
                {
                    _group = value;
                }
            }
        }
    }

    public class GuestSettingGroup : GuestSettingCommon
    {
        private ObservableCollection<GuestSettingItem> _items = new ObservableCollection<GuestSettingItem>();

        /// <summary>
        /// GuestSettingItem 对象的集合。
        /// </summary>
        public ObservableCollection<GuestSettingItem> Items
        {
            get { return this._items; }
        }
    }

    public class GuestAccessModel : INotifyPropertyChanged
    {
        public GuestAccessModel()
        {
            this.GuestSettingGroups = new ObservableCollection<GuestSettingGroup>();
            this.EditName = new GuestSettingGroup();
            this.EditKey = new GuestSettingGroup();
            this.EditTimesegSecurity = new ObservableCollection<GuestSettingGroup>();
        }

        public ObservableCollection<GuestSettingGroup> GuestSettingGroups { get; private set; }
        public GuestSettingGroup EditName { get; private set; }
        public GuestSettingGroup EditKey { get; private set; }
        public ObservableCollection<GuestSettingGroup> EditTimesegSecurity { get; private set; }

        //public bool IsDataLoaded
        //{
        //    get;
        //    private set;
        //}

        public void LoadData()
        {
            var group1 = new GuestSettingGroup() { ID = "GuestWiFiName", Title = AppResources.GuestWiFiName, Content = GuestAccessInfo.ssid };
            EditName = group1;
            this.GuestSettingGroups.Add(group1);

            var group2 = new GuestSettingGroup() { ID = "Password", Title = AppResources.Key_Password, Content = GuestAccessInfo.password };
            EditKey = group2;
            this.GuestSettingGroups.Add(group2);

            var group3 = new GuestSettingGroup() { ID = "TimeSegment", Title = AppResources.TimeSegment, Content = GuestAccessInfo.changedTimePeriod };
            group3.Items.Add(new GuestSettingItem() { ID = "TimeSegment-1", Title = "TimeSegment", Content = AppResources.TimeSegment_Always, Group = group3 });
            group3.Items.Add(new GuestSettingItem() { ID = "TimeSegment-2", Title = "TimeSegment", Content = AppResources.TimeSegment_1hour, Group = group3 });
            group3.Items.Add(new GuestSettingItem() { ID = "TimeSegment-3", Title = "TimeSegment", Content = AppResources.TimeSegment_5hours, Group = group3 });
            group3.Items.Add(new GuestSettingItem() { ID = "TimeSegment-4", Title = "TimeSegment", Content = AppResources.TimeSegment_10hours, Group = group3 });
            group3.Items.Add(new GuestSettingItem() { ID = "TimeSegment-5", Title = "TimeSegment", Content = AppResources.TimeSegment_1day, Group = group3 });
            group3.Items.Add(new GuestSettingItem() { ID = "TimeSegment-6", Title = "TimeSegment", Content = AppResources.TimeSegment_1week, Group = group3 });
            this.EditTimesegSecurity.Add(group3);
            this.GuestSettingGroups.Add(group3);

            string securityType = string.Empty;
            if (GuestAccessInfo.changedSecurityType == "None")
            {
                securityType = "None";
            }
            else if (GuestAccessInfo.changedSecurityType == "WPA2-PSK")
            {
                securityType = "WPA2-PSK[AES]";
            }
            else if (GuestAccessInfo.changedSecurityType == "WPA-PSK/WPA2-PSK" || GuestAccessInfo.changedSecurityType == "Mixed WPA")
            {
                securityType = "WPA-PSK+WPA2-PSK";
            }
            var group4 = new GuestSettingGroup() { ID = "Security", Title = AppResources.Security, Content = securityType };
            group4.Items.Add(new GuestSettingItem() { ID = "Security_None", Title = "Security", Content = AppResources.Security_None, Group = group4 });
            group4.Items.Add(new GuestSettingItem() { ID = "Security_WPA2-PSK[AES]", Title = "Security", Content = AppResources.Security_WPA2PSK_AES, Group = group4 });
            group4.Items.Add(new GuestSettingItem() { ID = "Security_WPA-PSK+WPA2-PSK", Title = "Security", Content = AppResources.Security_WPAPSK_WPA2PSK, Group = group4 });
            this.EditTimesegSecurity.Add(group4);
            //this.IsDataLoaded = true;
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