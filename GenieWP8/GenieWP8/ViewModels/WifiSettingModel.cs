using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using GenieWP8.Resources;
using GenieWP8.DataInfo;

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
            this.StrengthRateGroups = new ObservableCollection<SettingGroup>();
            this.SettingGroups = new ObservableCollection<SettingGroup>();
            this.EditName = new SettingGroup();
            this.EditKey = new SettingGroup();
            this.EditChannelSecurity = new ObservableCollection<SettingGroup>();
        }

        public ObservableCollection<SettingGroup> StrengthRateGroups { get; private set; }
        public ObservableCollection<SettingGroup> SettingGroups { get; private set; }
        public SettingGroup EditName { get; private set; }
        public SettingGroup EditKey { get; private set; }
        public ObservableCollection<SettingGroup> EditChannelSecurity { get; private set; }

        //public bool IsDataLoaded
        //{
        //    get;
        //    private set;
        //}

        public void LoadData()
        {          
            var group1 = new SettingGroup() { ID = "SignalStrength", Title = AppResources.txtSignalStrength, Content = WifiSettingInfo.signalStrength };
            this.StrengthRateGroups.Add(group1);

            var group2 = new SettingGroup() { ID = "LinkRate", Title = AppResources.txtLinkRate, Content = WifiSettingInfo.linkRate };
            this.StrengthRateGroups.Add(group2);

            var group3 = new SettingGroup() { ID = "WiFiName", Title = AppResources.WiFiName, Content = WifiSettingInfo.ssid };
            EditName = group3;
            this.SettingGroups.Add(group3);

            var group4 = new SettingGroup() { ID = "Password", Title = AppResources.Key_Password, Content = WifiSettingInfo.password };
            EditKey = group4;
            this.SettingGroups.Add(group4);

            var group5 = new SettingGroup() { ID = "Channel", Title = AppResources.Channel, Content = WifiSettingInfo.changedChannel };
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

            string securityType = string.Empty;
            if (WifiSettingInfo.changedSecurityType == "None")
            {
                securityType = "None";
            }
            else if (WifiSettingInfo.changedSecurityType == "WPA2-PSK")
            {
                securityType = "WPA2-PSK[AES]";
            }
            else if (WifiSettingInfo.changedSecurityType == "WPA-PSK/WPA2-PSK")
            {
                securityType = "WPA-PSK+WPA2-PSK";
            }
            var group6 = new SettingGroup() { ID = "Security", Title = AppResources.Security, Content = securityType };
            group6.Items.Add(new SettingItem() { ID = "Security_None", Title = "Security", Content = AppResources.Security_None, Group = group6 });
            group6.Items.Add(new SettingItem() { ID = "Security_WPA2-PSK[AES]", Title = "Security", Content = AppResources.Security_WPA2PSK_AES, Group = group6 });
            group6.Items.Add(new SettingItem() { ID = "Security_WPA-PSK+WPA2-PSK", Title = "Security", Content = AppResources.Security_WPAPSK_WPA2PSK, Group = group6 });
            this.EditChannelSecurity.Add(group6);
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