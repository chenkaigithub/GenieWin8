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

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace GenieWin8
{
    public sealed partial class PopupFilterLevel : UserControl
    {
        public PopupFilterLevel()
        {
            this.InitializeComponent();
            switch (ParentalControlInfo.filterLevel)
            {
                case "None":
                    radioButton_None.IsChecked = true;
                    break;
                case "Minimal":
                    radioButton_Minimum.IsChecked = true;
                    break;
                case "Low":
                    radioButton_Low.IsChecked = true;
                    break;
                case "Moderate":
                    radioButton_Medium.IsChecked = true;
                    break;
                case "High":
                    radioButton_High.IsChecked = true;
                    break;
                default:
                    break;
            }
        }

        public static String filterLevel = string.Empty;
        private void RadioButton_Checked(Object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
	        filterLevel = (String)(rb.Content);
            switch (rb.Name)
            {
                case "radioButton_None":
                    ParentalControlInfo.filterLevel = "None";
                    break;
                case "radioButton_Minimum":
                    ParentalControlInfo.filterLevel = "Minimal";
                    break;
                case "radioButton_Low":
                    ParentalControlInfo.filterLevel = "Low";
                    break;
                case "radioButton_Medium":
                    ParentalControlInfo.filterLevel = "Moderate";
                    break;
                case "radioButton_High":
                    ParentalControlInfo.filterLevel = "High";
                    break;
            }
        } 
    }
}
