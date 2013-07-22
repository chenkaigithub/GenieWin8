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

namespace GenieWP8
{
    public partial class WifiSettingPage : PhoneApplicationPage
    {
        private static WifiSettingModel settingModel = null;
        public WifiSettingPage()
        {
            InitializeComponent();

            // 将 LongListSelector 控件的数据上下文设置为绑定数据
            if (settingModel == null)
                settingModel = new WifiSettingModel();
            DataContext = settingModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();
        }

        // 为 WifiSettingModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //if (!App.ViewModel.IsDataLoaded)
            //{
            //    App.ViewModel.LoadData();
            //}
            settingModel.LoadData();
        }

        // 处理在 LongListSelector 中更改的选定内容
        private void WifiSettingLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果所选项为空(没有选定内容)，则不执行任何操作
            if (WifiSettingLongListSelector.SelectedItem == null)
                return;

            // 导航到新页面
            //NavigationService.Navigate(new Uri("/WifiSettingPage.xaml", UriKind.Relative));

            // 将所选项重置为 null (没有选定内容)
            WifiSettingLongListSelector.SelectedItem = null;
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

            //刷新按钮
            ApplicationBarIconButton appBarButton_refresh = new ApplicationBarIconButton(new Uri("Assets/refresh.png", UriKind.Relative));
            appBarButton_refresh.Text = AppResources.RefreshButtonContent;
            ApplicationBar.Buttons.Add(appBarButton_refresh);
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            // Switch the placement of the buttons based on an orientation change.
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                TitlePanel.Margin = new Thickness(12, 17, 0, 28);
                ContentPanel.Margin = new Thickness(12, 0, 12, 0);
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                TitlePanel.Margin = new Thickness(42, 17, 0, 28);
                ContentPanel.Margin = new Thickness(42, 0, 12, 0);
            }
        }
    }
}