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
using GenieWP8.DataInfo;

namespace GenieWP8
{
    public partial class BypassAccountPage : PhoneApplicationPage
    {
        public BypassAccountPage()
        {
            InitializeComponent();

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (ParentalControlInfo.BypassAccounts != null)
            {
                string[] bypassAccount = ParentalControlInfo.BypassAccounts.Split(';');
                for (int i = 0; i < bypassAccount.Length; i++)
                {
                    if (bypassAccount[i] != null && bypassAccount[i] != "")
                    {
                        bypassAccountListBox.Items.Add(bypassAccount[i]);
                    }
                }
            }
        }

        // 处理在 ListBox 中更改的选定内容
        private void BypassAccountItemClick(object sender, SelectionChangedEventArgs e)
        {
            int index = bypassAccountListBox.SelectedIndex;
            string[] bypassAccount = ParentalControlInfo.BypassAccounts.Split(';');
            ParentalControlInfo.BypassUsername = bypassAccount[index];
            NavigationService.Navigate(new Uri("/BypassAccountLoginPage.xaml", UriKind.Relative));
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

        //返回按钮响应事件
        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }
    }
}