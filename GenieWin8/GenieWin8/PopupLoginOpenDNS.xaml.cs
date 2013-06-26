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
using Windows.UI.Popups;
using GenieWin8.DataModel;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace GenieWin8
{
    public sealed partial class PopupLoginOpenDNS : UserControl
    {
        public PopupLoginOpenDNS()
        {
            this.InitializeComponent();
        }

        private void IsBlankUsername(Object sender, RoutedEventArgs e)
        {
            if (username.Text == "")
            {
                ParentalControlInfo.IsEmptyUsername = true;
            }
            else
            {
                ParentalControlInfo.Username = username.Text;
                ParentalControlInfo.IsEmptyUsername = false;
            }
        }

        private void IsBlankPassword(Object sender, RoutedEventArgs e)
        {
            if (password.Password == "")
            {
                ParentalControlInfo.IsEmptyPassword = true;
            }
            else
            {
                ParentalControlInfo.Password = password.Password;
                ParentalControlInfo.IsEmptyPassword = false;
            }
        }
    }
}
