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

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace GenieWin8
{
    public sealed partial class PopupAbout : UserControl
    {
        public PopupAbout()
        {
            this.InitializeComponent();
        }

        private async void Policy_Click(Object sender, RoutedEventArgs e)
        {
            var uri = new Uri(((HyperlinkButton)sender).Tag.ToString());
	        await Windows.System.Launcher.LaunchUriAsync(uri);
        }
    }
}
