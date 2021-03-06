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
using Microsoft.Phone.Net.NetworkInformation;

namespace GenieWP8
{
    public partial class StartDatePage : PhoneApplicationPage
    {
        private static TrafficMeterModel settingModel = null;
        private static bool IsWifiSsidChanged;
        public StartDatePage()
        {
            InitializeComponent();
            if ((this.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                PageTitle.Width = Application.Current.Host.Content.ActualWidth - 20;
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                PageTitle.Width = Application.Current.Host.Content.ActualHeight - 150;
            }

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
            //settingModel.TrafficMeterGroups.Clear();
            //settingModel.StartDate.Clear();
            //settingModel.TrafficLimitation.Clear();
            settingModel.LoadData();

            string restartDay = TrafficMeterInfo.changedRestartDay;
            if (int.Parse(restartDay) <= 28)
            {
                StartDateListBox.SelectedIndex = int.Parse(restartDay) - 1;
            } 
            else
            {
                StartDateListBox.SelectedIndex = 28;
            }            
            //StartDateListBox.SelectedIndex = 0;

            //判断所连接Wifi的Ssid是否改变
            IsWifiSsidChanged = true;
            foreach (var network in new NetworkInterfaceList())
            {
                if ((network.InterfaceType == NetworkInterfaceType.Wireless80211) && (network.InterfaceState == ConnectState.Connected))
                {
                    if (network.InterfaceName == MainPageInfo.ssid)
                        IsWifiSsidChanged = false;
                    else
                        IsWifiSsidChanged = true;
                }
            }
        }

        //用于生成本地化 ApplicationBar 的代码
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Default;

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            //后退按钮
            ApplicationBarIconButton appBarButton_back = new ApplicationBarIconButton(new Uri("Assets/back.png", UriKind.Relative));
            appBarButton_back.Text = AppResources.btnBack;
            ApplicationBar.Buttons.Add(appBarButton_back);
            appBarButton_back.Click += new EventHandler(appBarButton_back_Click);
        }

        private void PhoneApplicationPage_OrientationChanged(Object sender, OrientationChangedEventArgs e)
        {
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                PageTitle.Width = Application.Current.Host.Content.ActualWidth - 20;
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                PageTitle.Width = Application.Current.Host.Content.ActualHeight - 150;
            }
        }

        //处理在 ListBox 中更改的选定内容
        int lastIndex = -1;         //记录上次的选择项
        private void StartDate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
                MainPageInfo.navigatedPage = "TrafficMeterPage";
            } 
            else
            {
                int index = StartDateListBox.SelectedIndex;
                if (index == -1)
                    return;

                if (index < 28)
                {
                    TrafficMeterInfo.changedRestartDay = (index + 1).ToString();
                }
                else
                {
                    int RestartDay = DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month);
                    TrafficMeterInfo.changedRestartDay = RestartDay.ToString();
                }

                //判断重启日期是否更改
                if (TrafficMeterInfo.changedRestartDay != TrafficMeterInfo.RestartDay)
                {
                    TrafficMeterInfo.isRestartDayChanged = true;
                }
                else
                {
                    TrafficMeterInfo.isRestartDayChanged = false;
                }

                if (lastIndex != -1 && index != lastIndex)
                {
                    NavigationService.Navigate(new Uri("/TrafficMeterSettingPage.xaml", UriKind.Relative));
                }
                lastIndex = index;
            }
        }

        //返回按钮响应事件
        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/TrafficMeterSettingPage.xaml", UriKind.Relative));
        }

        //重写手机“返回”按钮事件
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/TrafficMeterSettingPage.xaml", UriKind.Relative));
        }
    }
}