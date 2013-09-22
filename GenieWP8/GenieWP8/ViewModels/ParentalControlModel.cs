using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using GenieWP8.Resources;
using GenieWP8.DataInfo;

namespace GenieWP8.ViewModels
{
    public class FilterLevelCommon : INotifyPropertyChanged
    {
        private string _id;
        /// <summary>
        /// FilterLevelCommon 属性; 此属性用于标识对象。
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
        /// FilterLevelCommon 属性；此属性在视图中用于使用绑定显示它的选项名称。
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
        /// FilterLevelCommon 属性；此属性在视图中用于使用绑定显示它的内容。
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

    public class BypassAccountGroup
    {
        private string _id;
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
                }
            }
        }

        private string _account;
        public string Account
        {
            get
            {
                return _account;
            }
            set
            {
                if (value != _account)
                {
                    _account = value;
                }
            }
        }
    }

    public class FilterLevelGroup : FilterLevelCommon
    {
    }

    public class ParentalControlModel : INotifyPropertyChanged
    {
        public ParentalControlModel()
        {
            this.FilterLevelGroups = new ObservableCollection<FilterLevelGroup>();
            this.BypassAccountGroups = new ObservableCollection<BypassAccountGroup>();
        }

        public ObservableCollection<FilterLevelGroup> FilterLevelGroups { get; private set; }
        public ObservableCollection<BypassAccountGroup> BypassAccountGroups { get; private set; }

        //public bool IsDataLoaded
        //{
        //    get;
        //    private set;
        //}

        public void LoadData()
        {
            var group1 = new FilterLevelGroup() { ID = "FilterLevel", Title = AppResources.FilterLevel, Content = ParentalControlInfo.filterLevel };
            this.FilterLevelGroups.Add(group1);

            //if (ParentalControlInfo.BypassAccounts != null)
            //{
            //    string[] bypassAccount = ParentalControlInfo.BypassAccounts.Split(';');
            //    for (int i = 0; i < bypassAccount.Length; i++)
            //    {
            //        if (bypassAccount[i] != null && bypassAccount[i] != "")
            //        {
            //            bypassAccountListBox.Items.Add(bypassAccount[i]);
            //        }
            //    }
            //}
            if (ParentalControlInfo.BypassAccounts != null)
            {
                string[] bypassAccount = ParentalControlInfo.BypassAccounts.Split(';');
                for (int i = 0; i < bypassAccount.Length; i++)
                {
                    if (bypassAccount[i] != null && bypassAccount[i] != "")
                    {
                        //bypassAccountListBox.Items.Add(bypassAccount[i]);
                        var group = new BypassAccountGroup() { ID = (i + 1).ToString(), Account = bypassAccount[i] };
                        this.BypassAccountGroups.Add(group);
                    }
                }
            }
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