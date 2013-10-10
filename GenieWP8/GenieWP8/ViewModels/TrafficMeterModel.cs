using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

using GenieWP8.Resources;
using GenieWP8.DataInfo;

namespace GenieWP8.ViewModels
{
    public class TrafficMeterCommon : INotifyPropertyChanged
    {
        private string _id;
        /// <summary>
        /// TrafficMeterCommon 属性; 此属性用于标识对象。
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
        /// TrafficMeterCommon 属性；此属性在视图中用于使用绑定显示它的选项名称。
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
        /// TrafficMeterCommon 属性；此属性在视图中用于使用绑定显示它的内容。
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

    public class TrafficMeterItem : TrafficMeterCommon
    {
        private TrafficMeterGroup _group;
        public TrafficMeterGroup Group
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

    public class TrafficMeterGroup : TrafficMeterCommon
    {
        private ObservableCollection<TrafficMeterItem> _items = new ObservableCollection<TrafficMeterItem>();

        /// <summary>
        /// TrafficMeterItem 对象的集合。
        /// </summary>
        public ObservableCollection<TrafficMeterItem> Items
        {
            get { return this._items; }
        }
    }

    public class TrafficMeterModel : INotifyPropertyChanged
    {
        public TrafficMeterModel()
        {
            this.TrafficMeterGroups = new ObservableCollection<TrafficMeterGroup>();
            this.LimitPerMonth = new TrafficMeterGroup();
            this.StartDate = new ObservableCollection<TrafficMeterGroup>();
            this.StartTimeHour = new TrafficMeterGroup();
            this.StartTimeMin = new TrafficMeterGroup();
            this.TrafficLimitation = new ObservableCollection<TrafficMeterGroup>();
        }

        public ObservableCollection<TrafficMeterGroup> TrafficMeterGroups { get; private set; }
        public TrafficMeterGroup LimitPerMonth { get; private set; }
        public ObservableCollection<TrafficMeterGroup> StartDate { get; private set; }
        public TrafficMeterGroup StartTimeHour { get; private set; }
        public TrafficMeterGroup StartTimeMin { get; private set; }
        public ObservableCollection<TrafficMeterGroup> TrafficLimitation { get; private set; }

        //public bool IsDataLoaded
        //{
        //    get;
        //    private set;
        //}

        public void LoadData()
        {
            var group1 = new TrafficMeterGroup() { ID = "LimitPerMonth", Title = AppResources.LimitPerMonth, Content = TrafficMeterInfo.changedMonthlyLimit };
            LimitPerMonth = group1;
            this.TrafficMeterGroups.Add(group1);

            var group2 = new TrafficMeterGroup() { ID = "StartDate", Title = AppResources.StartDate, Content = TrafficMeterInfo.changedRestartDay };
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-1", Title = "StartDate", Content = "1", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-2", Title = "StartDate", Content = "2", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-3", Title = "StartDate", Content = "3", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-4", Title = "StartDate", Content = "4", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-5", Title = "StartDate", Content = "5", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-6", Title = "StartDate", Content = "6", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-7", Title = "StartDate", Content = "7", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-8", Title = "StartDate", Content = "8", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-9", Title = "StartDate", Content = "9", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-10", Title = "StartDate", Content = "10", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-11", Title = "StartDate", Content = "11", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-12", Title = "StartDate", Content = "12", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-13", Title = "StartDate", Content = "13", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-14", Title = "StartDate", Content = "14", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-15", Title = "StartDate", Content = "15", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-16", Title = "StartDate", Content = "16", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-17", Title = "StartDate", Content = "17", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-18", Title = "StartDate", Content = "18", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-19", Title = "StartDate", Content = "19", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-20", Title = "StartDate", Content = "20", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-21", Title = "StartDate", Content = "21", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-22", Title = "StartDate", Content = "22", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-23", Title = "StartDate", Content = "23", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-24", Title = "StartDate", Content = "24", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-25", Title = "StartDate", Content = "25", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-26", Title = "StartDate", Content = "26", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-27", Title = "StartDate", Content = "27", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-28", Title = "StartDate", Content = "28", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-Last", Title = "StartDate", Content = AppResources.LastDay, Group = group2 });
            this.StartDate.Add(group2);
            this.TrafficMeterGroups.Add(group2);

            String STARTTIME_HOUR = TrafficMeterInfo.changedRestartHour;
            String STARTTIME_MINUTE = TrafficMeterInfo.changedRestartMinute;
            var hour = new TrafficMeterGroup() { ID = "StartTimeHour", Title = AppResources.CounterStartTimeHour, Content = STARTTIME_HOUR };
            var minute = new TrafficMeterGroup() { ID = "StartTimeMin", Title = AppResources.CounterStartTimeMin, Content = STARTTIME_MINUTE };
            var group3 = new TrafficMeterGroup() { ID = "StartTime", Title = AppResources.CounterStartTime, Content = STARTTIME_HOUR + ":" + STARTTIME_MINUTE };
            StartTimeHour = hour;
            StartTimeMin = minute;
            this.TrafficMeterGroups.Add(group3);

            string strContent = null;
            if (TrafficMeterInfo.changedControlOption == "No limit")
            {
                strContent = AppResources.TrafficLimitation_Unlimited;
            }
            else if (TrafficMeterInfo.changedControlOption == "Download only")
            {
                strContent = AppResources.TrafficLimitation_Download;
            }
            else if (TrafficMeterInfo.changedControlOption == "Both directions")
            {
                strContent = AppResources.TrafficLimitation_DownloadUpload;
            }
            var group4 = new TrafficMeterGroup() { ID = "TrafficLimitation", Title = AppResources.TrafficLimitation, Content = strContent };
            group4.Items.Add(new TrafficMeterItem() { ID = "TrafficLimitation-1", Title = "TrafficLimitation", Content = AppResources.TrafficLimitation_Unlimited, Group = group4 });
            group4.Items.Add(new TrafficMeterItem() { ID = "TrafficLimitation-2", Title = "TrafficLimitation", Content = AppResources.TrafficLimitation_Download, Group = group4 });
            group4.Items.Add(new TrafficMeterItem() { ID = "TrafficLimitation-3", Title = "TrafficLimitation", Content = AppResources.TrafficLimitation_DownloadUpload, Group = group4 });
            this.TrafficLimitation.Add(group4);
            this.TrafficMeterGroups.Add(group4);

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