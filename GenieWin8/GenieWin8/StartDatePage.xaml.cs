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
using Windows.Networking.Connectivity;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class StartDatePage : GenieWin8.Common.LayoutAwarePage
    {
        private static bool IsWifiSsidChanged;
        public StartDatePage()
        {
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
        }

        private void App_Resuming(Object sender, Object e)
        {
            //判断所连接Wifi的Ssid是否改变
            IsWifiSsidChanged = true;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        if (connectionProfile.ProfileName == MainPageInfo.ssid)
                            IsWifiSsidChanged = false;
                        else
                            IsWifiSsidChanged = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }
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
            var startdateGroup = TrafficMeterSource.GetStartDateItems((String)navigationParameter);
            string restartDay = TrafficMeterInfoModel.changedRestartDay;
            this.DefaultViewModel["itemStartDate"] = startdateGroup.Items;
            if (int.Parse(restartDay) <= 28)
            {
                restartDayListView.SelectedIndex = int.Parse(restartDay) - 1;
            } 
            else
            {
                restartDayListView.SelectedIndex = 28;
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

        int lastIndex = -1;         //记录上次的选择项
        private void ChangeRestartDayClick(object sender, SelectionChangedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "TrafficMeterPage";
            } 
            else
            {
                int index = restartDayListView.SelectedIndex;
                if (index == -1)
                    return;

                if (index < 28)
                {
                    TrafficMeterInfoModel.changedRestartDay = (index + 1).ToString();
                }
                else
                {
                    int RestartDay = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    TrafficMeterInfoModel.changedRestartDay = RestartDay.ToString();
                }

                //判断重启日期是否更改
                if (TrafficMeterInfoModel.changedRestartDay != TrafficMeterInfoModel.RestartDay)
                {
                    TrafficMeterInfoModel.isRestartDayChanged = true;
                }
                else
                {
                    TrafficMeterInfoModel.isRestartDayChanged = false;
                }

                if (lastIndex != -1 && index != lastIndex)
                {
                    this.Frame.Navigate(typeof(TrafficCtrlSettingPage));
                }
                lastIndex = index;
            }
        }
    }
}
