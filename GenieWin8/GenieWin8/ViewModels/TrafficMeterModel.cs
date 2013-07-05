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
    public abstract class TrafficMeterCommon : GenieWin8.Common.BindableBase
    {
        public TrafficMeterCommon(String uniqueId, String title, String content)
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

    public class TrafficMeterItem : TrafficMeterCommon
    {
        public TrafficMeterItem(String uniqueId, String title, String content, TrafficMeterGroup group)
            : base(uniqueId, title, content)
        {
            this._group = group;
        }

        private TrafficMeterGroup _group;
        public TrafficMeterGroup Group
        {
            get { return this._group; }
            set { this.SetProperty(ref this._group, value); }
        }
    }

    public class TrafficMeterGroup : TrafficMeterCommon
    {
        public TrafficMeterGroup(String uniqueId, String title, String content)
            : base(uniqueId, title, content)
        {
        }

        private ObservableCollection<TrafficMeterItem> _items = new ObservableCollection<TrafficMeterItem>();
        public ObservableCollection<TrafficMeterItem> Items
        {
            get { return this._items; }
        }
    }

    public sealed class TrafficMeterSource
    {
        private static TrafficMeterSource __trafficMeterSource = new TrafficMeterSource();

        private ObservableCollection<TrafficMeterGroup> _trafficMeterGroups = new ObservableCollection<TrafficMeterGroup>();
        public ObservableCollection<TrafficMeterGroup> TrafficMeterGroups
        {
            get { return this._trafficMeterGroups; }
        }

        public static IEnumerable<TrafficMeterGroup> GetGroups(string uniqueId)
        {
            __trafficMeterSource = new TrafficMeterSource();
            return __trafficMeterSource.TrafficMeterGroups;
        }

        private ObservableCollection<TrafficMeterGroup> _limitPerMonth = new ObservableCollection<TrafficMeterGroup>();
        public ObservableCollection<TrafficMeterGroup> LimitPerMonth
        {
            get { return this._limitPerMonth; }
        }

        public static IEnumerable<TrafficMeterGroup> GetLimitPerMonth(string uniqueId)
        {
            __trafficMeterSource = new TrafficMeterSource();
            return __trafficMeterSource.LimitPerMonth;
        }

        private ObservableCollection<TrafficMeterGroup> _startDate = new ObservableCollection<TrafficMeterGroup>();
        public ObservableCollection<TrafficMeterGroup> StartDate
        {
            get { return this._startDate; }
        }

        public static IEnumerable<TrafficMeterGroup> GetStartDate(string uniqueId)
        {
            __trafficMeterSource = new TrafficMeterSource();
            return __trafficMeterSource.StartDate;
        }

        private ObservableCollection<TrafficMeterGroup> _startTimeHour = new ObservableCollection<TrafficMeterGroup>();
        public ObservableCollection<TrafficMeterGroup> StartTimeHour
        {
            get { return this._startTimeHour; }
        }

        public static IEnumerable<TrafficMeterGroup> GetStartTimeHour(string uniqueId)
        {
            __trafficMeterSource = new TrafficMeterSource();
            return __trafficMeterSource.StartTimeHour;
        }

        private ObservableCollection<TrafficMeterGroup> _startTimeMin = new ObservableCollection<TrafficMeterGroup>();
        public ObservableCollection<TrafficMeterGroup> StartTimeMin
        {
            get { return this._startTimeMin; }
        }

        public static IEnumerable<TrafficMeterGroup> GetStartTimeMin(string uniqueId)
        {
            __trafficMeterSource = new TrafficMeterSource();
            return __trafficMeterSource.StartTimeMin;
        }

        private ObservableCollection<TrafficMeterGroup> _trafficLimitation = new ObservableCollection<TrafficMeterGroup>();
        public ObservableCollection<TrafficMeterGroup> TrafficLimitation
        {
            get { return this._trafficLimitation; }
        }

        public static IEnumerable<TrafficMeterGroup> GetTrafficLimitation(string uniqueId)
        {
            __trafficMeterSource = new TrafficMeterSource();
            return __trafficMeterSource.TrafficLimitation;
        }

        public static TrafficMeterGroup GetStartDateItems(string uniqueId)
        {           
            // 对于小型数据集可接受简单线性搜索
            var matches = __trafficMeterSource.StartDate.Where((group) => group.UniqueId.Equals("StartDate"));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public static TrafficMeterGroup GetTrafficLimitationItems(string uniqueId)
        {
            __trafficMeterSource = new TrafficMeterSource();
            // 对于小型数据集可接受简单线性搜索
            var matches = __trafficMeterSource.TrafficLimitation.Where((group) => group.UniqueId.Equals("TrafficLimitation"));
            if (matches.Count() == 1) return matches.First();
            return null;
        }

        public TrafficMeterSource()
        {
            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();

            var strTitle = loader.GetString("LimitPerMonth");
            var group1 = new TrafficMeterGroup("LimitPerMonth",
                    strTitle,
                    TrafficMeterInfoModel.changedMonthlyLimit);
            this.LimitPerMonth.Add(group1);
            this.TrafficMeterGroups.Add(group1);

            strTitle = loader.GetString("StartDate");
            var group2 = new TrafficMeterGroup("StartDate",
                strTitle,
                TrafficMeterInfoModel.changedRestartDay);
            group2.Items.Add(new TrafficMeterItem("StartDate-1",
                "StartDate",
                "1",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-2",
                "StartDate",
                "2",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-3",
                "StartDate",
                "3",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-4",
                "StartDate",
                "4",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-5",
                "StartDate",
                "5",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-6",
                "StartDate",
                "6",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-7",
                "StartDate",
                "7",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-8",
                "StartDate",
                "8",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-9",
                "StartDate",
                "9",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-10",
                "StartDate",
                "10",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-11",
                "StartDate",
                "11",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-12",
                "StartDate",
                "12",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-13",
                "StartDate",
                "13",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-14",
                "StartDate",
                "14",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-15",
                "StartDate",
                "15",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-16",
                "StartDate",
                "16",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-17",
                "StartDate",
                "17",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-18",
                "StartDate",
                "18",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-19",
                "StartDate",
                "19",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-20",
                "StartDate",
                "20",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-21",
                "StartDate",
                "21",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-22",
                "StartDate",
                "22",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-23",
                "StartDate",
                "23",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-24",
                "StartDate",
                "24",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-25",
                "StartDate",
                "25",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-26",
                "StartDate",
                "26",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-27",
                "StartDate",
                "27",
                group2));
            group2.Items.Add(new TrafficMeterItem("StartDate-28",
                "StartDate",
                "28",
                group2));
            //group2.Items.Add(new TrafficMeterItem("StartDate-29",
            //    "StartDate",
            //    "29",
            //    group2));
            this.StartDate.Add(group2);
            this.TrafficMeterGroups.Add(group2);

            String STARTTIME_HOUR = TrafficMeterInfoModel.changedRestartHour;
            String STARTTIME_MINUTE = TrafficMeterInfoModel.changedRestartMinute;
            strTitle = loader.GetString("CounterStartTimeHour");
            var hour = new TrafficMeterGroup("StartTimeHour",
                    strTitle,
                    STARTTIME_HOUR);
            strTitle = loader.GetString("CounterStartTimeMin");
            var minute = new TrafficMeterGroup("StartTimeMin",
                    strTitle,
                    STARTTIME_MINUTE);
            strTitle = loader.GetString("CounterStartTime");
            var group3 = new TrafficMeterGroup("StartTime",
		        strTitle,
		        STARTTIME_HOUR+":"+STARTTIME_MINUTE);
	        this.StartTimeHour.Add(hour);
            this.StartTimeMin.Add(minute);
            this.TrafficMeterGroups.Add(group3);

            strTitle = loader.GetString("TrafficLimitation");
            string strContent = null;
            if (TrafficMeterInfoModel.changedControlOption == "No limit")
            {
                strContent = loader.GetString("TrafficLimitation_Unlimited");
            }
            else if (TrafficMeterInfoModel.changedControlOption == "Download only")
            {
                strContent = loader.GetString("TrafficLimitation_Download");
            }
            else if (TrafficMeterInfoModel.changedControlOption == "Both directions")
            {
                strContent = loader.GetString("TrafficLimitation_DownloadUpload");
            }
            var group4 = new TrafficMeterGroup("TrafficLimitation",
                    strTitle,
                    strContent);
            strContent = loader.GetString("TrafficLimitation_Unlimited");
            group4.Items.Add(new TrafficMeterItem("TrafficLimitation-1",
                "TrafficLimitation",
                strContent,
                group4));
            strContent = loader.GetString("TrafficLimitation_Download");
            group4.Items.Add(new TrafficMeterItem("TrafficLimitation-2",
                "TrafficLimitation",
                strContent,
                group4));
            strContent = loader.GetString("TrafficLimitation_DownloadUpload");
            group4.Items.Add(new TrafficMeterItem("TrafficLimitation-3",
                "TrafficLimitation",
                strContent,
                group4));
            this.TrafficLimitation.Add(group4);
            this.TrafficMeterGroups.Add(group4);
        }
    }
}
