using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using GenieWP8.Resources;

namespace GenieWP8.ViewModels
{
    public class SettingCommon : INotifyPropertyChanged
    {
        private string _id;
        /// <summary>
        /// SettingCommon 属性; 此属性用于标识对象。
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
        /// SettingCommon 属性；此属性在视图中用于使用绑定显示它的选项名称。
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
        /// SettingCommon 属性；此属性在视图中用于使用绑定显示它的内容。
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

    public class SettingItem : SettingCommon
    {
        private SettingGroup _group;
        public SettingGroup Group
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

    public class SettingGroup : SettingCommon
    {
        private ObservableCollection<SettingItem> _items = new ObservableCollection<SettingItem>();

        /// <summary>
        /// SettingItem 对象的集合。
        /// </summary>
        public ObservableCollection<SettingItem> Items
        {
            get { return this._items; }
        }
    }

    public class WifiSettingModel : INotifyPropertyChanged
    {
        public WifiSettingModel()
        {
            //this.Items = new ObservableCollection<SettingItem>();
            this.SettingGroups = new ObservableCollection<SettingGroup>();
            this.EditName = new ObservableCollection<SettingGroup>();
            this.EditKey = new ObservableCollection<SettingGroup>();
            this.EditChannelSecurity = new ObservableCollection<SettingGroup>();
        }

        //private ObservableCollection<SettingGroup> _settingGroups = new ObservableCollection<SettingGroup>();
        public ObservableCollection<SettingGroup> SettingGroups { get; private set; }
        public ObservableCollection<SettingGroup> EditName { get; private set; }
        public ObservableCollection<SettingGroup> EditKey { get; private set; }
        public ObservableCollection<SettingGroup> EditChannelSecurity { get; private set; }

        //public static SettingGroup GetChannel(string uniqueId)
        //{
        //    // 对于小型数据集可接受简单线性搜索
        //    var matches = _settingSource.EditChannelSecurity.Where((group) => group.UniqueId.Equals(uniqueId));
        //    if (matches.Count() == 1) return matches.First();
        //    return null;
        //}

        //public static SettingGroup GetSecurity(string uniqueId)
        //{
        //    // 对于小型数据集可接受简单线性搜索
        //    var matches = _settingSource.EditChannelSecurity.Where((group) => group.UniqueId.Equals(uniqueId));
        //    if (matches.Count() == 1) return matches.First();
        //    return null;
        //}

        public bool IsDataLoaded
        {
            get;
            private set;
        }

        public void LoadData()
        {
            var group1 = new SettingGroup() { ID = "SignalStrength", Title = AppResources.txtSignalStrength, Content = "80%" };
            this.SettingGroups.Add(group1);

            var group2 = new SettingGroup() { ID = "LinkRate", Title = AppResources.txtLinkRate, Content = "20Mbps" };
            this.SettingGroups.Add(group2);

            var group3 = new SettingGroup() { ID = "WiFiName", Title = AppResources.WiFiName, Content = "4500-2G" };
            this.EditName.Add(group3);
            this.SettingGroups.Add(group3);

            var group4 = new SettingGroup() { ID = "Password", Title = AppResources.Key_Password, Content = "siteview" };
            this.EditKey.Add(group4);
            this.SettingGroups.Add(group4);

            var group5 = new SettingGroup() { ID = "Channel", Title = AppResources.Channel, Content = "Auto" };
            group5.Items.Add(new SettingItem() { ID = "Channel-1", Title = "Channel", Content = "Auto", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-2", Title = "Channel", Content = "1", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-3", Title = "Channel", Content = "2", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-4", Title = "Channel", Content = "3", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-5", Title = "Channel", Content = "4", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-6", Title = "Channel", Content = "5", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-7", Title = "Channel", Content = "6", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-8", Title = "Channel", Content = "7", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-9", Title = "Channel", Content = "8", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-10", Title = "Channel", Content = "9", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-11", Title = "Channel", Content = "10", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-12", Title = "Channel", Content = "11", Group = group5 });
            this.EditChannelSecurity.Add(group5);
            this.SettingGroups.Add(group5);

            var group6 = new SettingGroup() { ID = "Security", Title = AppResources.Security, Content = "None" };
            group6.Items.Add(new SettingItem() { ID = "Security_None", Title = "Security", Content = "None", Group = group6 });
            group6.Items.Add(new SettingItem() { ID = "Security_WPA2-PSK[AES]", Title = "Security", Content = "WPA2-PSK[AES]", Group = group6 });
            group6.Items.Add(new SettingItem() { ID = "Security_WPA-PSK+WPA2-PSK", Title = "Security", Content = "WPA-PSK+WPA2-PSK", Group = group6 });
            this.EditChannelSecurity.Add(group6);
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