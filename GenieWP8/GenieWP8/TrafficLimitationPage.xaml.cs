using System;
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
    public partial class TrafficLimitationPage : PhoneApplicationPage
    {
        private static TrafficMeterModel settingModel = null;
        public TrafficLimitationPage()
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

            string controlOption = TrafficMeterInfo.changedControlOption;
            switch (controlOption)
            {
                case "No limit":
                    ControlOptionListBox.SelectedIndex = 0;
                    break;
                case "Download only":
                    ControlOptionListBox.SelectedIndex = 1;
                    break;
                case "Both directions":
                    ControlOptionListBox.SelectedIndex = 2;
                    break;
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
        private void ControlOption_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = ControlOptionListBox.SelectedIndex;
            if (index == -1)
                return;

            switch (index)
            {
                case 0:
                    TrafficMeterInfo.changedControlOption = "No limit";
                    break;
                case 1:
                    TrafficMeterInfo.changedControlOption = "Download only";
                    break;
                case 2:
                    TrafficMeterInfo.changedControlOption = "Both directions";
                    break;
            }

            //判断流量限制是否更改
            if (TrafficMeterInfo.changedControlOption != TrafficMeterInfo.ControlOption)
            {
                TrafficMeterInfo.isControlOptionChanged = true;
            }
            else
            {
                TrafficMeterInfo.isControlOptionChanged = false;
            }

            if (lastIndex != -1 && index != lastIndex)
            {
                NavigationService.Navigate(new Uri("/TrafficMeterSettingPage.xaml", UriKind.Relative));
            }
            lastIndex = index;
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