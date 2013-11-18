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
using GenieWin8.DataModel;

namespace GenieWin8.Data
{
    [Windows.Foundation.Metadata.WebHostHidden]
    public abstract class SettingCommon : GenieWin8.Common.BindableBase
    {
        public SettingCommon(String uniqueId, String title, String content)
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

        //public override string ToString()
        //{
        //    return this.Title;
        //}
    }

    public class SettingItem : SettingCommon
    {
        public SettingItem(String uniqueId, String title, String content, SettingGroup group)
            : base(uniqueId, title, content)
        {            
            this._group = group;
        }

        private SettingGroup _group;
        public SettingGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    public class SettingGroup : SettingCommon
    {
        public SettingGroup(String uniqueId, String title, String content)
            : base(uniqueId, title, content)
        {            
        }

        private ObservableCollection<SettingItem> _items = new ObservableCollection<SettingItem>();
        public ObservableCollection<SettingItem> Items
        {
            get { return this._items; }
        }
    }

    public sealed class SettingSource
    {
        private static SettingSource _settingSource = new SettingSource();

        private ObservableCollection<SettingGroup> _settingGroups = new ObservableCollection<SettingGroup>();
        public ObservableCollection<SettingGroup> SettingGroups
        {
            get { return this._settingGroups; }
        }

        public static IEnumerable<SettingGroup> GetGroups(string uniqueId)
        {
            _settingSource = new SettingSource();
            return _settingSource.SettingGroups;
        }

        private ObservableCollection<SettingGroup> _signalStrengthGroup = new ObservableCollection<SettingGroup>();
        public ObservableCollection<SettingGroup> SignalStrengthGroup
        {
            get { return this._signalStrengthGroup; }
        }

        public static IEnumerable<SettingGroup> GetSignalStrengthGroup(string uniqueId)
        {
            return _settingSource.SignalStrengthGroup;
        }

        private ObservableCollection<SettingGroup> _linkRateGroup = new ObservableCollection<SettingGroup>();
        public ObservableCollection<SettingGroup> LinkRateGroup
        {
            get { return this._linkRateGroup; }
        }

        public static IEnumerable<SettingGroup> GetLinkRateGroup(string uniqueId)
        {
            return _settingSource.LinkRateGroup;
        }

        private ObservableCollection<SettingGroup> _editName = new ObservableCollection<SettingGroup>();
        public ObservableCollection<SettingGroup> EditName
        {
            get { return this._editName; }
        }

        public static IEnumerable<SettingGroup> GetEditName(string uniqueId)
        {
            return _settingSource.EditName;
        }

        private ObservableCollection<SettingGroup> _editKey = new ObservableCollection<SettingGroup>();
        public ObservableCollection<SettingGroup> EditKey
        {
            get { return this._editKey; }
        }

        public static IEnumerable<SettingGroup> GetEditKey(string uniqueId)
        {
            _settingSource = new SettingSource();
            return _settingSource.EditKey;
        }

        private ObservableCollection<SettingGroup> _editChannelSecurity = new ObservableCollection<SettingGroup>();
        public ObservableCollection<SettingGroup> EditChannelSecurity
        {
            get { return this._editChannelSecurity; }
        }

        public static IEnumerable<SettingGroup> GetChannelSecurity(string uniqueId)
        {
            _settingSource = new SettingSource();
            return _settingSource.EditChannelSecurity;
        }

        public static SettingGroup GetChannel(string uniqueId)
        {
            // 对于小型数据集可接受简单线性搜索
            var matches = _settingSource.EditChannelSecurity.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static SettingGroup GetSecurity(string uniqueId)
        {
            // 对于小型数据集可接受简单线性搜索
            var matches = _settingSource.EditChannelSecurity.Where((group) => group.UniqueId.Equals(uniqueId));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public SettingSource()
        {
           
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            var strTitle = loader.GetString("txtSignalStrength");
            var groupSignalStrength = new SettingGroup("txtSignalStrength",
                strTitle,
                WifiInfoModel.signalStrength);
            this.SignalStrengthGroup.Add(groupSignalStrength);

            strTitle = loader.GetString("txtLinkRate");
            var groupLinkRate = new SettingGroup("txtLinkRate",
                strTitle,
                WifiInfoModel.linkRate);
            this.LinkRateGroup.Add(groupLinkRate);

            strTitle = loader.GetString("WiFiName");
            var group1 = new SettingGroup("WiFiName",
                strTitle,
                WifiInfoModel.ssid);
            this.EditName.Add(group1);
            this.SettingGroups.Add(group1);

            strTitle = loader.GetString("Key/Password");
            var group2 = new SettingGroup("Password",
                strTitle,
                WifiInfoModel.password);
            this.EditKey.Add(group2);
            this.SettingGroups.Add(group2);

            strTitle = loader.GetString("Channel");
            var group3 = new SettingGroup("Channel",
                strTitle,
                WifiInfoModel.changedChannel);
            group3.Items.Add(new SettingItem("Channel-1",
                "Channel",
                "Auto",
                group3));
            group3.Items.Add(new SettingItem("Channel-2",
                "Channel",
                "1",
                group3));
            group3.Items.Add(new SettingItem("Channel-3",
                "Channel",
                "2",
                group3));
            group3.Items.Add(new SettingItem("Channel-4",
                "Channel",
                "3",
                group3));
            group3.Items.Add(new SettingItem("Channel-5",
                "Channel",
                "4",
                group3));
            group3.Items.Add(new SettingItem("Channel-6",
                "Channel",
                "5",
                group3));
            group3.Items.Add(new SettingItem("Channel-7",
                "Channel",
                "6",
                group3));
            group3.Items.Add(new SettingItem("Channel-8",
                "Channel",
                "7",
                group3));
            group3.Items.Add(new SettingItem("Channel-9",
                "Channel",
                "8",
                group3));
            group3.Items.Add(new SettingItem("Channel-10",
                "Channel",
                "9",
                group3));
            group3.Items.Add(new SettingItem("Channel-11",
                "Channel",
                "10",
                group3));
            group3.Items.Add(new SettingItem("Channel-12",
                "Channel",
                "11",
                group3));
            this.EditChannelSecurity.Add(group3);
            this.SettingGroups.Add(group3);

            strTitle = loader.GetString("Security");
            var group4 = new SettingGroup("Security",
                strTitle,
                WifiInfoModel.changedSecurityType);
            var strContent = loader.GetString("Security_None");
            group4.Items.Add(new SettingItem("Security-1",
                "Security",
                strContent,
                group4));
            strContent = loader.GetString("Security_WPA2-PSK[AES]");
            group4.Items.Add(new SettingItem("Security-2",
                "Security",
                strContent,
                group4));
            strContent = loader.GetString("Security_WPA-PSK+WPA2-PSK");
            group4.Items.Add(new SettingItem("Security-3",
                "Security",
                strContent,
                group4));
            this.EditChannelSecurity.Add(group4);           
        }
    }
}
