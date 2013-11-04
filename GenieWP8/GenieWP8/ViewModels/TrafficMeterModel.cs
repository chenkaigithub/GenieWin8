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
            //this.TrafficMeterGroups = new ObservableCollection<TrafficMeterGroup>();
            this.LimitPerMonth = new TrafficMeterGroup();
            this.StartDate = new TrafficMeterGroup();
            this.StartDateGroup = new ObservableCollection<TrafficMeterGroup>();
            this.StartTime = new TrafficMeterGroup();
            this.StartTimeHour = new TrafficMeterGroup();
            this.StartTimeMin = new TrafficMeterGroup();
            this.TrafficLimitation = new TrafficMeterGroup();
            this.TrafficLimitationGroup = new ObservableCollection<TrafficMeterGroup>();
        }

        //public ObservableCollection<TrafficMeterGroup> TrafficMeterGroups { get; private set; }
        public TrafficMeterGroup LimitPerMonth { get; private set; }
        public TrafficMeterGroup StartDate { get; private set; }
        public ObservableCollection<TrafficMeterGroup> StartDateGroup { get; private set; }
        public TrafficMeterGroup StartTime { get; private set; }
        public TrafficMeterGroup StartTimeHour { get; private set; }
        public TrafficMeterGroup StartTimeMin { get; private set; }
        public TrafficMeterGroup TrafficLimitation { get; private set; }
        public ObservableCollection<TrafficMeterGroup> TrafficLimitationGroup { get; private set; }

        //public bool IsDataLoaded
        //{
        //    get;
        //    private set;
        //}

        public void LoadData()
        {
            var group1 = new TrafficMeterGroup() { ID = "LimitPerMonth", Title = AppResources.LimitPerMonth, Content = TrafficMeterInfo.changedMonthlyLimit };
            LimitPerMonth = group1;
            //this.TrafficMeterGroups.Add(group1);

            var group2 = new TrafficMeterGroup();
            if (TrafficMeterInfo.changedRestartDay == DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString())
            {
                group2 = new TrafficMeterGroup() { ID = "StartDate", Title = AppResources.StartDate, Content = AppResources.LastDay };
            } 
            else
            {
                group2 = new TrafficMeterGroup() { ID = "StartDate", Title = AppResources.StartDate, Content = TrafficMeterInfo.changedRestartDay };
            }
            //var group2 = new TrafficMeterGroup() { ID = "StartDate", Title = AppResources.StartDate, Content = TrafficMeterInfo.changedRestartDay };
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-1", Title = "StartDate", Content = "1", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-2", Title = "StartDate", Content = "2", ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-3", Title = "StartDate", Content = "3", ImgPath = "/Assets/WirelessSetting/third.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-4", Title = "StartDate", Content = "4", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-5", Title = "StartDate", Content = "5", ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-6", Title = "StartDate", Content = "6", ImgPath = "/Assets/WirelessSetting/third.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-7", Title = "StartDate", Content = "7", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-8", Title = "StartDate", Content = "8", ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-9", Title = "StartDate", Content = "9", ImgPath = "/Assets/WirelessSetting/third.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-10", Title = "StartDate", Content = "10", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-11", Title = "StartDate", Content = "11", ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-12", Title = "StartDate", Content = "12", ImgPath = "/Assets/WirelessSetting/third.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-13", Title = "StartDate", Content = "13", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-14", Title = "StartDate", Content = "14", ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-15", Title = "StartDate", Content = "15", ImgPath = "/Assets/WirelessSetting/third.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-16", Title = "StartDate", Content = "16", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-17", Title = "StartDate", Content = "17", ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-18", Title = "StartDate", Content = "18", ImgPath = "/Assets/WirelessSetting/third.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-19", Title = "StartDate", Content = "19", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-20", Title = "StartDate", Content = "20", ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-21", Title = "StartDate", Content = "21", ImgPath = "/Assets/WirelessSetting/third.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-22", Title = "StartDate", Content = "22", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-23", Title = "StartDate", Content = "23", ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-24", Title = "StartDate", Content = "24", ImgPath = "/Assets/WirelessSetting/third.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-25", Title = "StartDate", Content = "25", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-26", Title = "StartDate", Content = "26", ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-27", Title = "StartDate", Content = "27", ImgPath = "/Assets/WirelessSetting/third.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-28", Title = "StartDate", Content = "28", ImgPath = "/Assets/WirelessSetting/first.png", Group = group2 });
            group2.Items.Add(new TrafficMeterItem() { ID = "StartDate-Last", Title = "StartDate", Content = AppResources.LastDay, ImgPath = "/Assets/WirelessSetting/second.png", Group = group2 });
            StartDate = group2;
            this.StartDateGroup.Add(group2);
            //this.TrafficMeterGroups.Add(group2);

            String STARTTIME_HOUR = TrafficMeterInfo.changedRestartHour;
            String STARTTIME_MINUTE = TrafficMeterInfo.changedRestartMinute;
            var hour = new TrafficMeterGroup() { ID = "StartTimeHour", Title = AppResources.CounterStartTimeHour, Content = STARTTIME_HOUR };
            var minute = new TrafficMeterGroup() { ID = "StartTimeMin", Title = AppResources.CounterStartTimeMin, Content = STARTTIME_MINUTE };
            var group3 = new TrafficMeterGroup() { ID = "StartTime", Title = AppResources.CounterStartTime, Content = STARTTIME_HOUR + ":" + STARTTIME_MINUTE };
            StartTimeHour = hour;
            StartTimeMin = minute;
            StartTime = group3;
            //this.TrafficMeterGroups.Add(group3);

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
            group4.Items.Add(new TrafficMeterItem() { ID = "TrafficLimitation-1", Title = "TrafficLimitation", Content = AppResources.TrafficLimitation_Unlimited, ImgPath = "/Assets/WirelessSetting/first.png", Group = group4 });
            group4.Items.Add(new TrafficMeterItem() { ID = "TrafficLimitation-2", Title = "TrafficLimitation", Content = AppResources.TrafficLimitation_Download, ImgPath = "/Assets/WirelessSetting/second.png", Group = group4 });
            group4.Items.Add(new TrafficMeterItem() { ID = "TrafficLimitation-3", Title = "TrafficLimitation", Content = AppResources.TrafficLimitation_DownloadUpload, ImgPath = "/Assets/WirelessSetting/third.png", Group = group4 });
            TrafficLimitation = group4;
            this.TrafficLimitationGroup.Add(group4);
            //this.TrafficMeterGroups.Add(group4);

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