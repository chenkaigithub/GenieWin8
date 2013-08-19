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
    public partial class MyMediaSourcePage : PhoneApplicationPage
    {
        public MyMediaSourcePage()
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

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();
        }

        // 为 GuestAccessModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
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

            //刷新按钮
            ApplicationBarIconButton appBarButton_refresh = new ApplicationBarIconButton(new Uri("Assets/refresh.png", UriKind.Relative));
            appBarButton_refresh.Text = AppResources.RefreshButtonContent;
            ApplicationBar.Buttons.Add(appBarButton_refresh);
            appBarButton_refresh.Click += new EventHandler(appBarButton_refresh_Click);

            // 使用 AppResources 中的本地化字符串创建新菜单项。
            ApplicationBarMenuItem appBarMenuItem_Source = new ApplicationBarMenuItem(AppResources.MyMediaSource);
            ApplicationBar.MenuItems.Add(appBarMenuItem_Source);
            appBarMenuItem_Source.IsEnabled = false;

            ApplicationBarMenuItem appBarMenuItem_Player = new ApplicationBarMenuItem(AppResources.MyMediaPlayer);
            ApplicationBar.MenuItems.Add(appBarMenuItem_Player);
            appBarMenuItem_Player.Click += new EventHandler(appBarMenuItem_Player_Click);

            ApplicationBarMenuItem appBarMenuItem_Playing = new ApplicationBarMenuItem(AppResources.MyMediaPlaying);
            ApplicationBar.MenuItems.Add(appBarMenuItem_Playing);
            appBarMenuItem_Playing.Click += new EventHandler(appBarMenuItem_Playing_Click);

            ApplicationBarMenuItem appBarMenuItem_Option = new ApplicationBarMenuItem(AppResources.MyMediaOption);
            ApplicationBar.MenuItems.Add(appBarMenuItem_Option);
            appBarMenuItem_Option.Click += new EventHandler(appBarMenuItem_Option_Click);
        }

        //横竖屏切换响应事件
        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
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

        //返回按钮响应事件
        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //重写手机“返回”按钮事件
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //刷新按钮响应事件
        private void appBarButton_refresh_Click(object sender, EventArgs e)
        {
        }

        private void appBarMenuItem_Player_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MyMediaPlayerPage.xaml", UriKind.Relative));
        }

        private void appBarMenuItem_Playing_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MyMediaPlayingPage.xaml", UriKind.Relative));
        }

        private void appBarMenuItem_Option_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MyMediaOptionPage.xaml", UriKind.Relative));
        }
    }
}