using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Threading;

namespace GenieWP8
{
    public partial class StartPage : PhoneApplicationPage
    {
        DispatcherTimer timer = new DispatcherTimer();      //计时器
        public StartPage()
        {
            InitializeComponent();
           
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += timer_Tick;
            timer.Start();
        }

        int count = 1;     //倒计时间
        void timer_Tick(object sender, object e)
        {
            count--;
            if (count < 0)
            {
                timer.Stop();
                NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
            }
        }
    }
}