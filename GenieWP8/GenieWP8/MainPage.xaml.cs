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
    public partial class MainPage : PhoneApplicationPage
    {
        // 构造函数
        public MainPage()
        {
            InitializeComponent();

            // 将 LongListSelector 控件的数据上下文设置为绑定数据
            DataContext = App.ViewModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();
        }

        // 为 ViewModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (!App.ViewModel.IsDataLoaded)
            {
                App.ViewModel.LoadData();
            }
            //清空BackStack,使得到MainPage能正确退出程序
            int count = NavigationService.BackStack.Count();
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    NavigationService.RemoveBackEntry();
                }
            }
        }

        // 处理在 LongListSelector 中更改的选定内容
        private void MainLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果所选项为空(没有选定内容)，则不执行任何操作
            if (MainLongListSelector.SelectedItem == null)
                return;

            // 导航到新页面
            NavigationService.Navigate(new Uri("/WifiSettingPage.xaml", UriKind.Relative));

            // 将所选项重置为 null (没有选定内容)
            MainLongListSelector.SelectedItem = null;
        }

        //用于生成本地化 ApplicationBar 的代码
        private void BuildLocalizedApplicationBar()
        {
            // 将页面的 ApplicationBar 设置为 ApplicationBar 的新实例。
            ApplicationBar = new ApplicationBar();
            ApplicationBar.Mode = ApplicationBarMode.Minimized;

            // 创建新按钮并将文本值设置为 AppResources 中的本地化字符串。
            //搜索按钮
            ApplicationBarIconButton appBarButton_search = new ApplicationBarIconButton(new Uri("Assets/search.png", UriKind.Relative));
            appBarButton_search.Text = AppResources.SearchText;
            ApplicationBar.Buttons.Add(appBarButton_search);      
     
            //关于按钮
            ApplicationBarIconButton appBarButton_about = new ApplicationBarIconButton(new Uri("Assets/questionmark.png", UriKind.Relative));
            appBarButton_about.Text = AppResources.AboutText;
            ApplicationBar.Buttons.Add(appBarButton_about);
            appBarButton_about.Click += new EventHandler(appBarButton_about_Click);

            // 使用 AppResources 中的本地化字符串创建新菜单项。
            ApplicationBarMenuItem appBarMenuItem_Login = new ApplicationBarMenuItem(AppResources.LoginButtonContent);
            ApplicationBar.MenuItems.Add(appBarMenuItem_Login);
            double width = System.Windows.Application.Current.Host.Content.ActualWidth;
            appBarMenuItem_Login.Click += new EventHandler(appBarMenuItem_Login_Click);
        }

        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            // Switch the placement of the buttons based on an orientation change.
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                ContentPanel.Margin = new Thickness(12, 0, 12, 0);
                LicenseScrollViewer.Height = 450;
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                ContentPanel.Margin = new Thickness(42, 0, 12, 0);
                LicenseScrollViewer.Height = 250;
            }
        }

        private void appBarButton_about_Click(object sender, EventArgs e)
        {
            
        }

        private void appBarMenuItem_Login_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/LoginPage.xaml", UriKind.Relative));
        }

        //隐私权政策链接响应事件
        private async void Policy_Click(Object sender, RoutedEventArgs e)
        {
            var uri = new Uri(((HyperlinkButton)sender).Tag.ToString());
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }

        //关闭“关于”事件
        private void CloseAboutButton_Click(Object sender, RoutedEventArgs e)
        {
            //if (AboutPopup.IsOpen)
            //{
            //    AboutPopup.IsOpen = false;
            //    PopupBackgroundTop.Visibility = Visibility.Collapsed;
            //    PopupBackground.Visibility = Visibility.Collapsed;
            //    pleasewait.Visibility = Visibility.Visible;
            //    CloseAboutButton.Visibility = Visibility.Collapsed;
            //    LicenseButton.Visibility = Visibility.Collapsed;
            //}
        }

        //点击“许可”响应事件
        private void LicenseButton_Click(Object sender, RoutedEventArgs e)
        {
            //if (!LicensePopup.IsOpen)
            //{
            //    LicensePopup.IsOpen = true;
            //    AboutPopup.IsOpen = false;
            //    PopupBackgroundTop.Visibility = Visibility.Visible;
            //    PopupBackground.Visibility = Visibility.Visible;
            //    pleasewait.Visibility = Visibility.Collapsed;
            //    CloseLicenseButton.Visibility = Visibility.Visible;
            //    CloseAboutButton.Visibility = Visibility.Collapsed;
            //    LicenseButton.Visibility = Visibility.Collapsed;
            //}
        }

        //关闭“许可”事件
        private void CloseLicenseButton_Click(Object sender, RoutedEventArgs e)
        {
            //if (LicensePopup.IsOpen)
            //{
            //    LicensePopup.IsOpen = false;
            //    PopupBackgroundTop.Visibility = Visibility.Collapsed;
            //    PopupBackground.Visibility = Visibility.Collapsed;
            //    pleasewait.Visibility = Visibility.Visible;
            //    CloseLicenseButton.Visibility = Visibility.Collapsed;
            //}
        }
    }
}