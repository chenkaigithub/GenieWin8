﻿using System;
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
using GenieWin8.DataModel;

namespace GenieWin8.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class GuestSettingCommon : GenieWin8.Common.BindableBase
    {
        public GuestSettingCommon(String uniqueId, String title, String content)
        {
            this._uniqueId = uniqueId;
            this._title = title;
            this._content = content;
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

        private string _content = string.Empty;
        public string Content
        {
            get { return this._content; }
            set { this.SetProperty(ref this._content, value); }
        }
    }

    public class GuestSettingItem : GuestSettingCommon
    {
        public GuestSettingItem(String uniqueId, String title, String content, GuestSettingGroup group)
            : base(uniqueId, title, content)
        {
            this._group = group;
        }

        private GuestSettingGroup _group;
        public GuestSettingGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    public class GuestSettingGroup : GuestSettingCommon
    {
        public GuestSettingGroup(String uniqueId, String title, String content)
            : base(uniqueId, title, content)
        {
        }

        private ObservableCollection<GuestSettingItem> _items = new ObservableCollection<GuestSettingItem>();
        public ObservableCollection<GuestSettingItem> Items
        {
            get { return this._items; }
        }
    }

    public sealed class GuestSettingSource
    {
        private static GuestSettingSource _settingSource = new GuestSettingSource();

        private ObservableCollection<GuestSettingGroup> _guestSettingGroups = new ObservableCollection<GuestSettingGroup>();
        public ObservableCollection<GuestSettingGroup> GuestSettingGroups
        {
            get { return this._guestSettingGroups; }
        }

        public static IEnumerable<GuestSettingGroup> GetGroups(string uniqueId)
        {
            _settingSource = new GuestSettingSource();
            return _settingSource.GuestSettingGroups;
        }

        private ObservableCollection<GuestSettingGroup> _editName = new ObservableCollection<GuestSettingGroup>();
        public ObservableCollection<GuestSettingGroup> EditName
        {
            get { return this._editName; }
        }

        public static IEnumerable<GuestSettingGroup> GetEditName(string uniqueId)
        {
            return _settingSource.EditName;
        }

        private ObservableCollection<GuestSettingGroup> _editKey = new ObservableCollection<GuestSettingGroup>();
        public ObservableCollection<GuestSettingGroup> EditKey
        {
            get { return this._editKey; }
        }

        public static IEnumerable<GuestSettingGroup> GetEditKey(string uniqueId)
        {
            _settingSource = new GuestSettingSource();
            return _settingSource.EditKey;
        }

        private ObservableCollection<GuestSettingGroup> _editTimesegSecurity = new ObservableCollection<GuestSettingGroup>();
        public ObservableCollection<GuestSettingGroup> EditTimesegSecurity
        {
            get { return this._editTimesegSecurity; }
        }

        public static IEnumerable<GuestSettingGroup> GetTimesegSecurity(string uniqueId)
        {
            _settingSource = new GuestSettingSource();
            return _settingSource.EditTimesegSecurity;
        }

        public static GuestSettingGroup GetTimeSegment(string uniqueId)
        {
            // 对于小型数据集可接受简单线性搜索
            var matches = _settingSource.EditTimesegSecurity.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static GuestSettingGroup GetSecurity(string uniqueId)
        {
            _settingSource = new GuestSettingSource();
            // 对于小型数据集可接受简单线性搜索
            var matches = _settingSource.EditTimesegSecurity.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public GuestSettingSource()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            var strTitle = loader.GetString("GuestWiFiName");
            var group1 = new GuestSettingGroup("GuestWiFiName",
                strTitle,
                GuestAccessInfoModel.ssid);
            this.EditName.Add(group1);
            this.GuestSettingGroups.Add(group1);

            strTitle = loader.GetString("Key/Password");
            var group2 = new GuestSettingGroup("Password",
                strTitle,
                GuestAccessInfoModel.password);
            this.EditKey.Add(group2);
            this.GuestSettingGroups.Add(group2);

            strTitle = loader.GetString("TimeSegment");
            var group3 = new GuestSettingGroup("TimeSegment",
                strTitle,
                GuestAccessInfoModel.changedTimePeriod);
            var strContent = loader.GetString("TimeSegment_Always");
            group3.Items.Add(new GuestSettingItem("TimeSegment-1",
                "TimeSegment",
                strContent,
                group3));
            strContent = loader.GetString("TimeSegment_1hour");
            group3.Items.Add(new GuestSettingItem("TimeSegment-2",
                "TimeSegment",
                strContent,
                group3));
            strContent = loader.GetString("TimeSegment_5hours");
            group3.Items.Add(new GuestSettingItem("TimeSegment-3",
                "TimeSegment",
                strContent,
                group3));
            strContent = loader.GetString("TimeSegment_10hours");
            group3.Items.Add(new GuestSettingItem("TimeSegment-4",
                "TimeSegment",
                strContent,
                group3));
            strContent = loader.GetString("TimeSegment_1day");
            group3.Items.Add(new GuestSettingItem("TimeSegment-5",
                "TimeSegment",
                strContent,
                group3));
            strContent = loader.GetString("TimeSegment_1week");
            group3.Items.Add(new GuestSettingItem("TimeSegment-6",
                "TimeSegment",
                strContent,
                group3));
            this.EditTimesegSecurity.Add(group3);
            this.GuestSettingGroups.Add(group3);

            strTitle = loader.GetString("Security");
            string securityType;
            if (GuestAccessInfoModel.changedSecurityType == "Mixed WPA" || GuestAccessInfoModel.changedSecurityType == "WPA-PSK/WPA2-PSK")
            {
                securityType = "WPA-PSK+WPA2-PSK";
            } 
            else
            {
                securityType = GuestAccessInfoModel.changedSecurityType;
            }
            var group4 = new GuestSettingGroup("Security",
                strTitle,
                securityType);
            strContent = loader.GetString("Security_None");
            group4.Items.Add(new GuestSettingItem("Security-1",
                "Security",
                strContent,
                group4));
            strContent = loader.GetString("Security_WPA2-PSK[AES]");
            group4.Items.Add(new GuestSettingItem("Security-2",
                "Security",
                strContent,
                group4));
            strContent = loader.GetString("Security_WPA-PSK+WPA2-PSK");
            group4.Items.Add(new GuestSettingItem("Security-3",
                "Security",
                strContent,
                group4));
            this.EditTimesegSecurity.Add(group4);
            //this.GuestSettingGroups.Add(group4);
        }
    }
}
