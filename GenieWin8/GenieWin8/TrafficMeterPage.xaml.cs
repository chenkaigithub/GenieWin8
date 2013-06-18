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
using Windows.UI.Xaml.Shapes;
using Windows.UI;
using Windows.UI.Text;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class TrafficMeterPage : GenieWin8.Common.LayoutAwarePage
    {
        public TrafficMeterPage()
        {
            this.InitializeComponent();
            if (checkTrafficMeter.IsChecked == true)
            {
                TrafficMeterList.Visibility = Visibility.Visible;
                TotalCanvas.Visibility = Visibility.Visible;
                AverageCanvas.Visibility = Visibility.Visible;
            }
            else if (checkTrafficMeter.IsChecked == false)
            {
                TrafficMeterList.Visibility = Visibility.Collapsed;
                TotalCanvas.Visibility = Visibility.Collapsed;
                AverageCanvas.Visibility = Visibility.Collapsed;
            }
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
            var groups = TrafficMeterSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = groups;
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

        private void TrafficMeter_ItemClick(Object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(TrafficCtrlSettingPage));
        }

        private void OnWindowSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            TotalCanvas.Children.Clear();
            AverageCanvas.Children.Clear();
            TotalCanvas.Children.Add(TextMbytesAverage);
            AverageCanvas.Children.Add(TextMbytesTotal);
            double width = Window.Current.Bounds.Width;

            TextTotal.Margin = new Thickness((width - 200) / 2, 20, 0, 0);
            TextAverage.Margin = new Thickness((width - 200) / 2, 20, 0, 0);
            TotalCanvas.Children.Add(TextTotal);
            AverageCanvas.Children.Add(TextAverage);

            Line TotalYaxis = new Line();
            TotalYaxis.Stroke = new SolidColorBrush(Colors.Black);
            TotalYaxis.StrokeThickness = 3;
            TotalYaxis.X1 = 100; TotalYaxis.Y1 = 50;
            TotalYaxis.X2 = 100; TotalYaxis.Y2 = 750;
            TotalYaxis.Width = width - 120;
            TotalYaxis.Height = 800;
            TotalCanvas.Children.Add(TotalYaxis);

            Line TotalXaxis = new Line();
            TotalXaxis.Stroke = new SolidColorBrush(Colors.Black);
            TotalXaxis.StrokeThickness = 3;
            TotalXaxis.X1 = 100; TotalXaxis.Y1 = 750;
            TotalXaxis.X2 = width - 200; TotalXaxis.Y2 = 750;
            TotalXaxis.Width = width - 120;
            TotalXaxis.Height = 800;
            TotalCanvas.Children.Add(TotalXaxis);

            for (int i = 0; i < 5; i++)
            {
                DoubleCollection dc = new DoubleCollection();
                dc.Add(4);
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 2;
                line.StrokeDashArray = dc;
                line.X1 = 100; line.Y1 = 100 + i * 130;
                line.X2 = width - 200; line.Y2 = 100 + i * 130;
                line.Width = width - 120;
                line.Height = 800;
                TotalCanvas.Children.Add(line);
            }

            Line AverageYaxis = new Line();
            AverageYaxis.Stroke = new SolidColorBrush(Colors.Black);
            AverageYaxis.StrokeThickness = 3;
            AverageYaxis.X1 = 100; AverageYaxis.Y1 = 50;
            AverageYaxis.X2 = 100; AverageYaxis.Y2 = 750;
            AverageYaxis.Width = width - 120;
            AverageYaxis.Height = 800;
            AverageCanvas.Children.Add(AverageYaxis);

            Line AverageXaxis = new Line();
            AverageXaxis.Stroke = new SolidColorBrush(Colors.Black);
            AverageXaxis.StrokeThickness = 3;
            AverageXaxis.X1 = 100; AverageXaxis.Y1 = 750;
            AverageXaxis.X2 = width - 200; AverageXaxis.Y2 = 750;
            AverageXaxis.Width = width - 120;
            AverageXaxis.Height = 800;
            AverageCanvas.Children.Add(AverageXaxis);

            for (int i = 0; i < 5; i++)
            {
                DoubleCollection dc = new DoubleCollection();
                dc.Add(4);
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 2;
                line.StrokeDashArray = dc;
                line.X1 = 100; line.Y1 = 100 + i * 130;
                line.X2 = width - 200; line.Y2 = 100 + i * 130;
                line.Width = width - 120;
                line.Height = 800;
                AverageCanvas.Children.Add(line);
            }
        }

        private void checkTrafficMeter_Click(Object sender, RoutedEventArgs e)
        {
            if (checkTrafficMeter.IsChecked == true)
            {
                TrafficMeterList.Visibility = Visibility.Visible;
                TotalCanvas.Visibility = Visibility.Visible;
                AverageCanvas.Visibility = Visibility.Visible;
            }
            else if (checkTrafficMeter.IsChecked == false)
            {
                TrafficMeterList.Visibility = Visibility.Collapsed;
                TotalCanvas.Visibility = Visibility.Collapsed;
                AverageCanvas.Visibility = Visibility.Collapsed;
            }
        }
    }
}
