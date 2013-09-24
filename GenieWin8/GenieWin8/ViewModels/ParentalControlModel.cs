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
    public abstract class FilterLevelCommon : GenieWin8.Common.BindableBase
    {
        private static Uri _baseUri = new Uri("ms-appx:///");

        public FilterLevelCommon(String uniqueId, String title, String content)
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

    public class FilterLevelGroup : FilterLevelCommon
    {
        public FilterLevelGroup(String uniqueId, String title, String content)
            : base(uniqueId, title, content)
        {
        
        }
    }

    public class BypassAccountGroup : GenieWin8.Common.BindableBase
    {
        public BypassAccountGroup(String uniqueId, String account)
        {
            this._uniqueId = uniqueId;
            this._account = account;
        }

        private string _uniqueId = string.Empty;
        public string UniqueId
        {
            get { return this._uniqueId; }
            set { this.SetProperty(ref this._uniqueId, value); }
        }

        private string _account = string.Empty;
        public string Account
        {
            get { return this._account; }
            set { this.SetProperty(ref this._account, value); }
        }
    }

    public sealed class FilterLevelSource
    {
        private static FilterLevelSource _filterLevelSource = new FilterLevelSource();

        private ObservableCollection<FilterLevelGroup> _filterLevelGroups = new ObservableCollection<FilterLevelGroup>();
        public ObservableCollection<FilterLevelGroup> FilterLevelGroups
        {
            get
            {
                this._filterLevelGroups.Clear();
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

                var strTitle = loader.GetString("FilterLevel");
                var group = new FilterLevelGroup("FilterLevel",
                        strTitle,
                        ParentalControlInfo.filterLevel);
                this._filterLevelGroups.Add(group);
                return this._filterLevelGroups;
            }
        }       

        public static IEnumerable<FilterLevelGroup> GetFilterLevelGroup(string uniqueId)
        {
            return _filterLevelSource.FilterLevelGroups;
        }

        public FilterLevelSource()
        {          
        }
    }

    public sealed class BypassAccountSource
    {
        private static BypassAccountSource _bypassAccountSource = new BypassAccountSource();

        private ObservableCollection<BypassAccountGroup> _bypassAccountGroups = new ObservableCollection<BypassAccountGroup>();
        public ObservableCollection<BypassAccountGroup> BypassAccountGroups
        {
            get
            {
                this._bypassAccountGroups.Clear();
                if (ParentalControlInfo.BypassAccounts != null)
                {
                    string[] bypassAccount = ParentalControlInfo.BypassAccounts.Split(';');
                    for (int i = 0; i < bypassAccount.Length; i++)
                    {
                        if (bypassAccount[i] != null && bypassAccount[i] != "")
                        {
                            //bypassAccountListBox.Items.Add(bypassAccount[i]);
                            var group = new BypassAccountGroup((i + 1).ToString(), bypassAccount[i]);
                            this._bypassAccountGroups.Add(group);
                        }
                    }
                }
                return this._bypassAccountGroups;
            }
        }

        public static IEnumerable<BypassAccountGroup> GetBypassAccountGroup()
        {
            return _bypassAccountSource.BypassAccountGroups;
        }

        public BypassAccountSource()
        {
        }
    }
}
