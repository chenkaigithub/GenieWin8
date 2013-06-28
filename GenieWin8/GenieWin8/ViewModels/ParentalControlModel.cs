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
                        PopupFilterLevel.filterLevel);
                this._filterLevelGroups.Add(group);
                return this._filterLevelGroups;
            }
        }

        public static IEnumerable<FilterLevelGroup> GetGroup(string uniqueId)
        {
            return _filterLevelSource.FilterLevelGroups;
        }

        public FilterLevelSource()
        {          
        }
    }
}
