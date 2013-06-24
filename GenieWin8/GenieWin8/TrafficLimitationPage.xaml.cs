using GenieWin8.Data;

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

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class TrafficLimitationPage : GenieWin8.Common.LayoutAwarePage
    {
        public TrafficLimitationPage()
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
            var trafficlimitationGroup = TrafficMeterSource.GetTrafficLimitationItems((String)navigationParameter);
            string controlOption = TrafficMeterInfoModel.ControlOption;
            this.DefaultViewModel["itemTrafficLimitation"] = trafficlimitationGroup.Items;
            switch (controlOption)
            {
                case "No Limit":
                    controlOptionsListView.SelectedIndex = 0;
                    break;
                case "Download only":
                    controlOptionsListView.SelectedIndex = 1;
                    break;
                case "Both directions":
                    controlOptionsListView.SelectedIndex = 2;
                    break;
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

        private void ControlOptions_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int index = controlOptionsListView.SelectedIndex;
            switch (index)
            {
                case 0:
                    TrafficMeterInfoModel.ControlOption = "No Limit";
                    break;
                case 1:
                    TrafficMeterInfoModel.ControlOption = "Download only";
                    break;
                case 2:
                    TrafficMeterInfoModel.ControlOption = "Both directions";
                    break;
            }
            this.Frame.Navigate(typeof(TrafficCtrlSettingPage));
        }
    }
}
