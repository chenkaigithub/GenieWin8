﻿using GenieWin8.Data;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using GenieWin8.DataModel;
using Windows.UI.Popups;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class TrafficCtrlSettingPage : GenieWin8.Common.LayoutAwarePage
    {
        public TrafficCtrlSettingPage()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// 使用在导航过程中传递的内容填充页。在从以前的会话
        /// 重新创建页时，也会提供任何已保存状态。
        /// </summary>
        /// <param name="navigationParameter">最初请求此页时传递给
        /// <see cref="Frame.Navigate(Type, Object)"/> 的参数值。
        /// </param>
        /// <param name="pageState">此页在以前会话期间保留的状态
        /// 字典。首次访问页面时为 null。</param>
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var LimitPerMonth = TrafficMeterSource.GetLimitPerMonth((String)navigationParameter);
            this.DefaultViewModel["limitpermonth"] = LimitPerMonth;
            var StartDate = TrafficMeterSource.GetStartDate((String)navigationParameter);
            this.DefaultViewModel["itemStartDate"] = StartDate;
            var StartTimeHour = TrafficMeterSource.GetStartTimeHour((String)navigationParameter);
            this.DefaultViewModel["itemStartTimeHour"] = StartTimeHour;
            var StartTimeMin = TrafficMeterSource.GetStartTimeMin((String)navigationParameter);
            this.DefaultViewModel["itemStartTimeMin"] = StartTimeMin;
            var TrafficLimitation = TrafficMeterSource.GetTrafficLimitation((String)navigationParameter);
            this.DefaultViewModel["itemTrafficLimitation"] = TrafficLimitation;

            //判断保存按钮是否可点击
            string monthlylimit = monthlyLimit.Text.Trim();
            string restarthour = restartHour.Text.Trim();
            string restartminute = restartMinute.Text.Trim();
            if (monthlylimit != "" && int.Parse(monthlylimit) != int.Parse(TrafficMeterInfoModel.MonthlyLimit)
                || restarthour != "" && int.Parse(restarthour) != int.Parse(TrafficMeterInfoModel.RestartHour)
                || restartminute != "" && int.Parse(restartminute) != int.Parse(TrafficMeterInfoModel.RestartMinute)
                || TrafficMeterInfoModel.isRestartDayChanged == true || TrafficMeterInfoModel.isControlOptionChanged == true)
            {
                TrafficMeterSettingSave.IsEnabled = true;
            }
            else
            {
                TrafficMeterSettingSave.IsEnabled = false;
            }
        }

        /// <summary>
        /// 保留与此页关联的状态，以防挂起应用程序或
        /// 从导航缓存中放弃此页。值必须符合
        /// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
        /// </summary>
        /// <param name="pageState">要使用可序列化状态填充的空字典。</param>
        protected override void SaveState(Dictionary<String, Object> pageState)
        {
        }

        //判断每月限制是否更改以及保存按钮是否可点击
        private void monthlyLimit_changed(Object sender, RoutedEventArgs e)
        {
            TrafficMeterInfoModel.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfoModel.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfoModel.changedRestartMinute = restartMinute.Text.Trim();
            if (TrafficMeterInfoModel.changedMonthlyLimit != "" && TrafficMeterInfoModel.changedRestartHour != "" && TrafficMeterInfoModel.changedRestartMinute != ""
                && int.Parse(TrafficMeterInfoModel.changedMonthlyLimit) != int.Parse(TrafficMeterInfoModel.MonthlyLimit))
            {
                TrafficMeterSettingSave.IsEnabled = true;
                TrafficMeterInfoModel.isMonthlyLimitChanged = true;
            }
            else
            {
                TrafficMeterInfoModel.isMonthlyLimitChanged = false;
                if (TrafficMeterInfoModel.changedMonthlyLimit == "")
                {
                    TrafficMeterSettingSave.IsEnabled = false;
                }
                else
                {
                    if (TrafficMeterInfoModel.changedRestartHour != "" && TrafficMeterInfoModel.changedRestartMinute != ""
                        && (TrafficMeterInfoModel.isRestartDayChanged == true || TrafficMeterInfoModel.isRestartHourChanged == true
                        || TrafficMeterInfoModel.isRestartMinuteChanged == true || TrafficMeterInfoModel.isControlOptionChanged == true))
                    {
                        TrafficMeterSettingSave.IsEnabled = true;
                    }
                    else
                    {
                        TrafficMeterSettingSave.IsEnabled = false;
                    }
                }               
            }
        }

        //判断重启时间-小时是否更改以及保存按钮是否可点击
        private void restartHour_changed(Object sender, RoutedEventArgs e)
        {
            TrafficMeterInfoModel.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfoModel.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfoModel.changedRestartMinute = restartMinute.Text.Trim();
            if (TrafficMeterInfoModel.changedMonthlyLimit != "" && TrafficMeterInfoModel.changedRestartHour != "" && TrafficMeterInfoModel.changedRestartMinute != ""
                && int.Parse(TrafficMeterInfoModel.changedRestartHour) != int.Parse(TrafficMeterInfoModel.RestartHour))
            {
                TrafficMeterSettingSave.IsEnabled = true;
                TrafficMeterInfoModel.isRestartHourChanged = true;
            }
            else
            {
                TrafficMeterInfoModel.isRestartHourChanged = false;
                if (TrafficMeterInfoModel.changedRestartHour == "")
                {
                    TrafficMeterSettingSave.IsEnabled = false;
                } 
                else
                {
                    if (TrafficMeterInfoModel.changedMonthlyLimit != "" && TrafficMeterInfoModel.changedRestartMinute != ""
                        && (TrafficMeterInfoModel.isRestartDayChanged == true || TrafficMeterInfoModel.isMonthlyLimitChanged == true
                        || TrafficMeterInfoModel.isRestartMinuteChanged == true || TrafficMeterInfoModel.isControlOptionChanged == true))
                    {
                        TrafficMeterSettingSave.IsEnabled = true;
                    }
                    else
                    {
                        TrafficMeterSettingSave.IsEnabled = false;
                    }
                }               
            }
        }

        //判断重启时间-分钟是否更改以及保存按钮是否可点击
        private void restartMinute_changed(Object sender, RoutedEventArgs e)
        {
            TrafficMeterInfoModel.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfoModel.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfoModel.changedRestartMinute = restartMinute.Text.Trim();
            if (TrafficMeterInfoModel.changedMonthlyLimit != "" && TrafficMeterInfoModel.changedRestartHour != "" && TrafficMeterInfoModel.changedRestartMinute != ""
                && int.Parse(TrafficMeterInfoModel.changedRestartMinute) != int.Parse(TrafficMeterInfoModel.RestartMinute))
            {
                TrafficMeterSettingSave.IsEnabled = true;
                TrafficMeterInfoModel.isRestartMinuteChanged = true;
            }
            else
            {
                TrafficMeterInfoModel.isRestartMinuteChanged = false;
                if (TrafficMeterInfoModel.changedRestartMinute == "")
                {
                    TrafficMeterSettingSave.IsEnabled = false;
                }
                else
                {
                    if (TrafficMeterInfoModel.changedRestartHour != "" && TrafficMeterInfoModel.changedMonthlyLimit != ""
                        && (TrafficMeterInfoModel.isRestartDayChanged == true || TrafficMeterInfoModel.isMonthlyLimitChanged == true
                        || TrafficMeterInfoModel.isRestartHourChanged == true || TrafficMeterInfoModel.isControlOptionChanged == true))
                    {
                        TrafficMeterSettingSave.IsEnabled = true;
                    }
                    else
                    {
                        TrafficMeterSettingSave.IsEnabled = false;
                    }
                }                
            }
        }

        private void StartDate_ItemClick(Object sender, ItemClickEventArgs e)
        {
            TrafficMeterInfoModel.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfoModel.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfoModel.changedRestartMinute = restartMinute.Text.Trim();
            this.Frame.Navigate(typeof(StartDatePage));
        }

        private void TrafficLimitation_ItemClick(Object sender, ItemClickEventArgs e)
        {
            TrafficMeterInfoModel.changedMonthlyLimit = monthlyLimit.Text.Trim();
            TrafficMeterInfoModel.changedRestartHour = restartHour.Text.Trim();
            TrafficMeterInfoModel.changedRestartMinute = restartMinute.Text.Trim();
            this.Frame.Navigate(typeof(TrafficLimitationPage));
        }

        private void GoBack_Click(Object sender, RoutedEventArgs e)
        {
            TrafficMeterInfoModel.isControlOptionChanged = false;
            TrafficMeterInfoModel.isMonthlyLimitChanged = false;
            TrafficMeterInfoModel.isRestartDayChanged = false;
            TrafficMeterInfoModel.isRestartHourChanged = false;
            TrafficMeterInfoModel.isRestartMinuteChanged = false;
            this.Frame.Navigate(typeof(TrafficMeterPage));
        }

        private async void TrafficMeterSettingSave_Click(object sender, RoutedEventArgs e)
        {
            InProgress.IsActive = true;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();

            string MonthlyLimit = monthlyLimit.Text.Trim();
            string RestartHour = restartHour.Text.Trim();
            string RestartMinute = restartMinute.Text.Trim();
            if (MonthlyLimit != "" && MonthlyLimit != null && int.Parse(MonthlyLimit) <= 1000000
                && RestartHour != "" && RestartHour != null && int.Parse(RestartHour) >= 0 && int.Parse(RestartHour) <= 24
                && RestartMinute != "" && RestartMinute != null && int.Parse(RestartMinute) >=0 && int.Parse(RestartMinute) <= 60)
            {
                if (TrafficMeterInfoModel.changedControlOption == "No limit")
                {
                    TrafficMeterInfoModel.changedControlOption = "No Limit";
                }
                await soapApi.SetTrafficMeterOptions(TrafficMeterInfoModel.changedControlOption, MonthlyLimit, RestartHour, RestartMinute, TrafficMeterInfoModel.changedRestartDay);
                if (TrafficMeterInfoModel.changedControlOption == "No Limit")
                {
                    TrafficMeterInfoModel.ControlOption = "No limit";
                }
                TrafficMeterInfoModel.MonthlyLimit = MonthlyLimit;
                TrafficMeterInfoModel.RestartHour = RestartHour;
                TrafficMeterInfoModel.RestartMinute = RestartMinute;
                TrafficMeterInfoModel.RestartDay = TrafficMeterInfoModel.changedRestartDay;
                TrafficMeterInfoModel.isControlOptionChanged = false;
                TrafficMeterInfoModel.isMonthlyLimitChanged = false;
                TrafficMeterInfoModel.isRestartDayChanged = false;
                TrafficMeterInfoModel.isRestartHourChanged = false;
                TrafficMeterInfoModel.isRestartMinuteChanged = false;
                TrafficMeterSettingSave.IsEnabled = false;
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
            else
            {
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                if (int.Parse(RestartHour) < 0 || int.Parse(RestartHour) > 24)
                {
                    var messageDialog = new MessageDialog("RestartHour must be between 0 and 24.");
                    await messageDialog.ShowAsync();
                }
                if (int.Parse(RestartMinute) < 0 || int.Parse(RestartMinute) > 60)
                {
                    var messageDialog = new MessageDialog("RestartMinute must be between 0 and 60.");
                    await messageDialog.ShowAsync();
                }
                if (int.Parse(MonthlyLimit) < 0 || int.Parse(MonthlyLimit) > 1000000)
                {
                    var messageDialog = new MessageDialog("MonthlyLimit must be between 0 and 1000000.");
                    await messageDialog.ShowAsync();
                }
            }
        }
    }
}
