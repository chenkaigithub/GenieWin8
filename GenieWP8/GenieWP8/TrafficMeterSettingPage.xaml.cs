﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;

using GenieWP8.Resources;
using GenieWP8.ViewModels;
using GenieWP8.DataInfo;

namespace GenieWP8
{
    public partial class TrafficMeterSettingPage : PhoneApplicationPage
    {
        private static TrafficMeterModel settingModel = null; 
        public TrafficMeterSettingPage()
        {
            InitializeComponent();

            // 绑定数据
            if (settingModel == null)
                settingModel = new TrafficMeterModel();
            DataContext = settingModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();
        }

        // 为 TrafficMeterModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            settingModel.TrafficMeterGroups.Clear();
            settingModel.StartDate.Clear();
            settingModel.TrafficLimitation.Clear();
            settingModel.LoadData();

            //判断保存按钮是否可点击
            string monthlylimit = TrafficMeterInfo.changedMonthlyLimit;
            string restarthour = TrafficMeterInfo.changedRestartHour;
            string restartminute = TrafficMeterInfo.changedRestartMinute;
            if (monthlylimit != "" && int.Parse(monthlylimit) != int.Parse(TrafficMeterInfo.MonthlyLimit)
                || restarthour != "" && int.Parse(restarthour) != int.Parse(TrafficMeterInfo.RestartHour)
                || restartminute != "" && int.Parse(restartminute) != int.Parse(TrafficMeterInfo.RestartMinute)
                || TrafficMeterInfo.isRestartDayChanged == true || TrafficMeterInfo.isControlOptionChanged == true)
            {
                appBarButton_save.IsEnabled = true;
            }
            else
            {
                appBarButton_save.IsEnabled = false;
            }
        }       

        //处理在 StartDateLongListSelector 中更改的选定内容
        private void StartDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果所选项为空(没有选定内容)，则不执行任何操作
            if (StartDateLongListSelector.SelectedItem == null)
                return;

            // 导航到新页面
            TrafficMeterInfo.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfo.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfo.changedRestartMinute = restartMinute.Text.Trim();
            NavigationService.Navigate(new Uri("/StartDatePage.xaml", UriKind.Relative));

            // 将所选项重置为 null (没有选定内容)
            StartDateLongListSelector.SelectedItem = null;
        }

        //处理在 TrafficLimitationLongListSelector 中更改的选定内容
        private void TrafficLimitation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果所选项为空(没有选定内容)，则不执行任何操作
            if (TrafficLimitationLongListSelector.SelectedItem == null)
                return;

            // 导航到新页面
            TrafficMeterInfo.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfo.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfo.changedRestartMinute = restartMinute.Text.Trim();
            NavigationService.Navigate(new Uri("/TrafficLimitationPage.xaml", UriKind.Relative));

            // 将所选项重置为 null (没有选定内容)
            TrafficLimitationLongListSelector.SelectedItem = null;
        }

        //用于生成本地化 ApplicationBar 的代码
        ApplicationBarIconButton appBarButton_back = new ApplicationBarIconButton(new Uri("Assets/back.png", UriKind.Relative));
        ApplicationBarIconButton appBarButton_save = new ApplicationBarIconButton(new Uri("Assets/save.png", UriKind.Relative));
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            //后退按钮
            //ApplicationBarIconButton appBarButton_back = new ApplicationBarIconButton(new Uri("Assets/back.png", UriKind.Relative));
            appBarButton_back.Text = AppResources.btnBack;
            ApplicationBar.Buttons.Add(appBarButton_back);
            appBarButton_back.Click += new EventHandler(appBarButton_back_Click);

            //保存按钮
            //ApplicationBarIconButton appBarButton_save = new ApplicationBarIconButton(new Uri("Assets/save.png", UriKind.Relative));
            appBarButton_save.Text = AppResources.RefreshButtonContent;
            ApplicationBar.Buttons.Add(appBarButton_save);
            appBarButton_save.Click += new EventHandler(appBarButton_save_Click);
        }

        //判断每月限制是否更改以及保存按钮是否可点击
        private void monthlyLimit_changed(Object sender, RoutedEventArgs e)
        {
            TrafficMeterInfo.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfo.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfo.changedRestartMinute = restartMinute.Text.Trim();
            if (TrafficMeterInfo.changedMonthlyLimit != "" && TrafficMeterInfo.changedRestartHour != "" && TrafficMeterInfo.changedRestartMinute != ""
                && int.Parse(TrafficMeterInfo.changedMonthlyLimit) != int.Parse(TrafficMeterInfo.MonthlyLimit))
            {
                appBarButton_save.IsEnabled = true;
                TrafficMeterInfo.isMonthlyLimitChanged = true;
            }
            else
            {
                TrafficMeterInfo.isMonthlyLimitChanged = false;
                if (TrafficMeterInfo.changedMonthlyLimit == "")
                {
                    appBarButton_save.IsEnabled = false;
                }
                else
                {
                    if (TrafficMeterInfo.changedRestartHour != "" && TrafficMeterInfo.changedRestartMinute != ""
                        && (TrafficMeterInfo.isRestartDayChanged == true || TrafficMeterInfo.isRestartHourChanged == true
                        || TrafficMeterInfo.isRestartMinuteChanged == true || TrafficMeterInfo.isControlOptionChanged == true))
                    {
                        appBarButton_save.IsEnabled = true;
                    }
                    else
                    {
                        appBarButton_save.IsEnabled = false;
                    }
                }
            }
        }

        //判断重启时间-小时是否更改以及保存按钮是否可点击
        private void restartHour_changed(Object sender, RoutedEventArgs e)
        {
            TrafficMeterInfo.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfo.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfo.changedRestartMinute = restartMinute.Text.Trim();
            if (TrafficMeterInfo.changedMonthlyLimit != "" && TrafficMeterInfo.changedRestartHour != "" && TrafficMeterInfo.changedRestartMinute != ""
                && int.Parse(TrafficMeterInfo.changedRestartHour) != int.Parse(TrafficMeterInfo.RestartHour))
            {
                appBarButton_save.IsEnabled = true;
                TrafficMeterInfo.isRestartHourChanged = true;
            }
            else
            {
                TrafficMeterInfo.isRestartHourChanged = false;
                if (TrafficMeterInfo.changedRestartHour == "")
                {
                    appBarButton_save.IsEnabled = false;
                }
                else
                {
                    if (TrafficMeterInfo.changedMonthlyLimit != "" && TrafficMeterInfo.changedRestartMinute != ""
                        && (TrafficMeterInfo.isRestartDayChanged == true || TrafficMeterInfo.isMonthlyLimitChanged == true
                        || TrafficMeterInfo.isRestartMinuteChanged == true || TrafficMeterInfo.isControlOptionChanged == true))
                    {
                        appBarButton_save.IsEnabled = true;
                    }
                    else
                    {
                        appBarButton_save.IsEnabled = false;
                    }
                }
            }
        }

        //判断重启时间-分钟是否更改以及保存按钮是否可点击
        private void restartMinute_changed(Object sender, RoutedEventArgs e)
        {
            TrafficMeterInfo.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfo.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfo.changedRestartMinute = restartMinute.Text.Trim();
            if (TrafficMeterInfo.changedMonthlyLimit != "" && TrafficMeterInfo.changedRestartHour != "" && TrafficMeterInfo.changedRestartMinute != ""
                && int.Parse(TrafficMeterInfo.changedRestartMinute) != int.Parse(TrafficMeterInfo.RestartMinute))
            {
                appBarButton_save.IsEnabled = true;
                TrafficMeterInfo.isRestartMinuteChanged = true;
            }
            else
            {
                TrafficMeterInfo.isRestartMinuteChanged = false;
                if (TrafficMeterInfo.changedRestartMinute == "")
                {
                    appBarButton_save.IsEnabled = false;
                }
                else
                {
                    if (TrafficMeterInfo.changedRestartHour != "" && TrafficMeterInfo.changedMonthlyLimit != ""
                        && (TrafficMeterInfo.isRestartDayChanged == true || TrafficMeterInfo.isMonthlyLimitChanged == true
                        || TrafficMeterInfo.isRestartHourChanged == true || TrafficMeterInfo.isControlOptionChanged == true))
                    {
                        appBarButton_save.IsEnabled = true;
                    }
                    else
                    {
                        appBarButton_save.IsEnabled = false;
                    }
                }
            }
        }

        //返回按钮响应事件
        private async void appBarButton_back_Click(object sender, EventArgs e)
        {
            TrafficMeterInfo.isControlOptionChanged = false;
            TrafficMeterInfo.isMonthlyLimitChanged = false;
            TrafficMeterInfo.isRestartDayChanged = false;
            TrafficMeterInfo.isRestartHourChanged = false;
            TrafficMeterInfo.isRestartMinuteChanged = false;

            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetTrafficMeterEnabled();
            TrafficMeterInfo.isTrafficMeterEnabled = dicResponse["NewTrafficMeterEnable"];
            if (dicResponse["NewTrafficMeterEnable"] == "0" || dicResponse["NewTrafficMeterEnable"] == "1")
            {
                Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                dicResponse2 = await soapApi.GetTrafficMeterOptions();
                if (dicResponse2.Count > 0)
                {
                    TrafficMeterInfo.MonthlyLimit = dicResponse2["NewMonthlyLimit"];
                    TrafficMeterInfo.changedMonthlyLimit = dicResponse2["NewMonthlyLimit"];
                    TrafficMeterInfo.RestartHour = dicResponse2["RestartHour"];
                    TrafficMeterInfo.changedRestartHour = dicResponse2["RestartHour"];
                    TrafficMeterInfo.RestartMinute = dicResponse2["RestartMinute"];
                    TrafficMeterInfo.changedRestartMinute = dicResponse2["RestartMinute"];
                    TrafficMeterInfo.RestartDay = dicResponse2["RestartDay"];
                    TrafficMeterInfo.changedRestartDay = dicResponse2["RestartDay"];
                    TrafficMeterInfo.ControlOption = dicResponse2["NewControlOption"];
                    TrafficMeterInfo.changedControlOption = dicResponse2["NewControlOption"];
                }
                dicResponse2 = await soapApi.GetTrafficMeterStatistics();
                if (dicResponse2.Count > 0)
                {
                    TrafficMeterInfo.TodayUpload = dicResponse2["NewTodayUpload"];
                    TrafficMeterInfo.TodayDownload = dicResponse2["NewTodayDownload"];
                    TrafficMeterInfo.YesterdayUpload = dicResponse2["NewYesterdayUpload"];
                    TrafficMeterInfo.YesterdayDownload = dicResponse2["NewYesterdayDownload"];
                    TrafficMeterInfo.WeekUpload = dicResponse2["NewWeekUpload"];
                    TrafficMeterInfo.WeekDownload = dicResponse2["NewWeekDownload"];
                    TrafficMeterInfo.MonthUpload = dicResponse2["NewMonthUpload"];
                    TrafficMeterInfo.MonthDownload = dicResponse2["NewMonthDownload"];
                    TrafficMeterInfo.LastMonthUpload = dicResponse2["NewLastMonthUpload"];
                    TrafficMeterInfo.LastMonthDownload = dicResponse2["NewLastMonthDownload"];
                }
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                NavigationService.Navigate(new Uri("/TrafficMeterPage.xaml", UriKind.Relative));
                //this.Frame.Navigate(typeof(TrafficMeterPage));
            }
            else if (dicResponse["NewTrafficMeterEnable"] == "2")
            {
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                MessageBox.Show(AppResources.notsupport);
            }
        }

        //重写手机“返回”按钮事件
        protected async override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            TrafficMeterInfo.isControlOptionChanged = false;
            TrafficMeterInfo.isMonthlyLimitChanged = false;
            TrafficMeterInfo.isRestartDayChanged = false;
            TrafficMeterInfo.isRestartHourChanged = false;
            TrafficMeterInfo.isRestartMinuteChanged = false;

            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetTrafficMeterEnabled();
            TrafficMeterInfo.isTrafficMeterEnabled = dicResponse["NewTrafficMeterEnable"];
            if (dicResponse["NewTrafficMeterEnable"] == "0" || dicResponse["NewTrafficMeterEnable"] == "1")
            {
                Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                dicResponse2 = await soapApi.GetTrafficMeterOptions();
                if (dicResponse2.Count > 0)
                {
                    TrafficMeterInfo.MonthlyLimit = dicResponse2["NewMonthlyLimit"];
                    TrafficMeterInfo.changedMonthlyLimit = dicResponse2["NewMonthlyLimit"];
                    TrafficMeterInfo.RestartHour = dicResponse2["RestartHour"];
                    TrafficMeterInfo.changedRestartHour = dicResponse2["RestartHour"];
                    TrafficMeterInfo.RestartMinute = dicResponse2["RestartMinute"];
                    TrafficMeterInfo.changedRestartMinute = dicResponse2["RestartMinute"];
                    TrafficMeterInfo.RestartDay = dicResponse2["RestartDay"];
                    TrafficMeterInfo.changedRestartDay = dicResponse2["RestartDay"];
                    TrafficMeterInfo.ControlOption = dicResponse2["NewControlOption"];
                    TrafficMeterInfo.changedControlOption = dicResponse2["NewControlOption"];
                }
                dicResponse2 = await soapApi.GetTrafficMeterStatistics();
                if (dicResponse2.Count > 0)
                {
                    TrafficMeterInfo.TodayUpload = dicResponse2["NewTodayUpload"];
                    TrafficMeterInfo.TodayDownload = dicResponse2["NewTodayDownload"];
                    TrafficMeterInfo.YesterdayUpload = dicResponse2["NewYesterdayUpload"];
                    TrafficMeterInfo.YesterdayDownload = dicResponse2["NewYesterdayDownload"];
                    TrafficMeterInfo.WeekUpload = dicResponse2["NewWeekUpload"];
                    TrafficMeterInfo.WeekDownload = dicResponse2["NewWeekDownload"];
                    TrafficMeterInfo.MonthUpload = dicResponse2["NewMonthUpload"];
                    TrafficMeterInfo.MonthDownload = dicResponse2["NewMonthDownload"];
                    TrafficMeterInfo.LastMonthUpload = dicResponse2["NewLastMonthUpload"];
                    TrafficMeterInfo.LastMonthDownload = dicResponse2["NewLastMonthDownload"];
                }
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                NavigationService.Navigate(new Uri("/TrafficMeterPage.xaml", UriKind.Relative));
                //this.Frame.Navigate(typeof(TrafficMeterPage));
            }
            else if (dicResponse["NewTrafficMeterEnable"] == "2")
            {
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                MessageBox.Show(AppResources.notsupport);
            }
        }

        //按下屏幕键盘回车键后关闭屏幕键盘
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.Focus();
            }
            else
            {
                base.OnKeyDown(e);
            }
        }


        //保存按钮响应事件
        private async void appBarButton_save_Click(object sender, EventArgs e)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;

            GenieSoapApi soapApi = new GenieSoapApi();
            string MonthlyLimit = monthlyLimit.Text.Trim();
            string RestartHour = restartHour.Text.Trim();
            string RestartMinute = restartMinute.Text.Trim();
            if (MonthlyLimit != "" && MonthlyLimit != null && int.Parse(MonthlyLimit) <= 1000000
                && RestartHour != "" && RestartHour != null && int.Parse(RestartHour) >= 0 && int.Parse(RestartHour) <= 23
                && RestartMinute != "" && RestartMinute != null && int.Parse(RestartMinute) >= 0 && int.Parse(RestartMinute) <= 60)
            {
                if (TrafficMeterInfo.changedControlOption == "No limit")
                {
                    TrafficMeterInfo.changedControlOption = "No Limit";
                }
                await soapApi.SetTrafficMeterOptions(TrafficMeterInfo.changedControlOption, MonthlyLimit, RestartHour, RestartMinute, TrafficMeterInfo.changedRestartDay);
                if (TrafficMeterInfo.changedControlOption == "No Limit")
                {
                    TrafficMeterInfo.ControlOption = "No limit";
                }
                TrafficMeterInfo.MonthlyLimit = MonthlyLimit;
                TrafficMeterInfo.RestartHour = RestartHour;
                TrafficMeterInfo.RestartMinute = RestartMinute;
                TrafficMeterInfo.RestartDay = TrafficMeterInfo.changedRestartDay;
                TrafficMeterInfo.isControlOptionChanged = false;
                TrafficMeterInfo.isMonthlyLimitChanged = false;
                TrafficMeterInfo.isRestartDayChanged = false;
                TrafficMeterInfo.isRestartHourChanged = false;
                TrafficMeterInfo.isRestartMinuteChanged = false;
                appBarButton_save.IsEnabled = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
            else
            {
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                if (int.Parse(RestartHour) < 0 || int.Parse(RestartHour) > 23)
                {
                    MessageBox.Show("RestartHour must be between 0 and 23.");
                }
                if (int.Parse(RestartMinute) < 0 || int.Parse(RestartMinute) > 60)
                {
                    MessageBox.Show("RestartMinute must be between 0 and 60.");
                }
                if (int.Parse(MonthlyLimit) < 0 || int.Parse(MonthlyLimit) > 1000000)
                {
                    MessageBox.Show("MonthlyLimit must be between 0 and 1000000.");
                }
            }
        }
    }
}