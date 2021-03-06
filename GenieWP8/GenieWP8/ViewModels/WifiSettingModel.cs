﻿using System;
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
        private string _imgpath;
        public string ImgPath
        {
            get
            {
                return _imgpath;
            }
            set
            {
                if (value != _imgpath)
                {
                    _imgpath = value;
                }
            }
        }

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
            this.SignalStrengthGroup = new SettingGroup();
            this.LinkRateGroup = new SettingGroup();
            this.ssidGroup = new SettingGroup();
            this.KeyGroup = new SettingGroup();
            this.ChannelGroup = new SettingGroup();
            this.SecurityGroup = new SettingGroup();
            //this.EditName = new SettingGroup();
            //this.EditKey = new SettingGroup();
            this.EditChannelSecurity = new ObservableCollection<SettingGroup>();
        }

        public SettingGroup SignalStrengthGroup { get; private set; }
        public SettingGroup LinkRateGroup { get; private set; }
        public SettingGroup ssidGroup { get; private set; }
        public SettingGroup KeyGroup { get; private set; }
        public SettingGroup ChannelGroup { get; private set; }
        public SettingGroup SecurityGroup { get; private set; }
        //public SettingGroup EditName { get; private set; }
        //public SettingGroup EditKey { get; private set; }
        public ObservableCollection<SettingGroup> EditChannelSecurity { get; private set; }

        //public bool IsDataLoaded
        //{
        //    get;
        //    private set;
        //}

        public void LoadData()
        {          
            var group1 = new SettingGroup() { ID = "SignalStrength", Title = AppResources.txtSignalStrength, Content = WifiSettingInfo.signalStrength };
            SignalStrengthGroup = group1;

            var group2 = new SettingGroup() { ID = "LinkRate", Title = AppResources.txtLinkRate, Content = WifiSettingInfo.linkRate };
            LinkRateGroup = group2;

            var group3 = new SettingGroup() { ID = "WiFiName", Title = AppResources.WiFiName, Content = WifiSettingInfo.ssid };
            //EditName = group3;
            ssidGroup = group3;

            var group4 = new SettingGroup() { ID = "Password", Title = AppResources.Key_Password, Content = WifiSettingInfo.password };
            //EditKey = group4;
            KeyGroup = group4;

            var group5 = new SettingGroup() { ID = "Channel", Title = AppResources.Channel, Content = WifiSettingInfo.changedChannel };
            group5.Items.Add(new SettingItem() { ID = "Channel-1", Title = "Channel", Content = "Auto", ImgPath="/Assets/WirelessSetting/first.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-2", Title = "Channel", Content = "1", ImgPath = "/Assets/WirelessSetting/second.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-3", Title = "Channel", Content = "2", ImgPath = "/Assets/WirelessSetting/third.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-4", Title = "Channel", Content = "3", ImgPath = "/Assets/WirelessSetting/first.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-5", Title = "Channel", Content = "4", ImgPath = "/Assets/WirelessSetting/second.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-6", Title = "Channel", Content = "5", ImgPath = "/Assets/WirelessSetting/third.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-7", Title = "Channel", Content = "6", ImgPath = "/Assets/WirelessSetting/first.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-8", Title = "Channel", Content = "7", ImgPath = "/Assets/WirelessSetting/second.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-9", Title = "Channel", Content = "8", ImgPath = "/Assets/WirelessSetting/third.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-10", Title = "Channel", Content = "9", ImgPath = "/Assets/WirelessSetting/first.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-11", Title = "Channel", Content = "10", ImgPath = "/Assets/WirelessSetting/second.png", Group = group5 });
            group5.Items.Add(new SettingItem() { ID = "Channel-12", Title = "Channel", Content = "11", ImgPath = "/Assets/WirelessSetting/third.png", Group = group5 });
            this.EditChannelSecurity.Add(group5);
            ChannelGroup = group5;

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
            group6.Items.Add(new SettingItem() { ID = "Security_None", Title = "Security", Content = AppResources.Security_None, ImgPath = "/Assets/WirelessSetting/first.png", Group = group6 });
            group6.Items.Add(new SettingItem() { ID = "Security_WPA2-PSK[AES]", Title = "Security", Content = AppResources.Security_WPA2PSK_AES, ImgPath = "/Assets/WirelessSetting/second.png", Group = group6 });
            group6.Items.Add(new SettingItem() { ID = "Security_WPA-PSK+WPA2-PSK", Title = "Security", Content = AppResources.Security_WPAPSK_WPA2PSK, ImgPath = "/Assets/WirelessSetting/third.png", Group = group6 });
            this.EditChannelSecurity.Add(group6);
            SecurityGroup = group6;
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