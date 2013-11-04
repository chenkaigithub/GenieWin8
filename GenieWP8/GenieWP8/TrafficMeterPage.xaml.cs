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
using System.Windows.Media;
using System.Windows.Shapes;

namespace GenieWP8
{
    public partial class TrafficMeterPage : PhoneApplicationPage
    {
        private static TrafficMeterModel settingModel = null;
        public TrafficMeterPage()
        {
            InitializeComponent();

            // 将 LongListSelector 控件的数据上下文设置为绑定数据
            if (settingModel == null)
                settingModel = new TrafficMeterModel();
            DataContext = settingModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();

            if (TrafficMeterInfo.isTrafficMeterEnabled == "0")
            {
                checkTrafficMeter.IsChecked = false;
                //TrafficMeterLongListSelector.Visibility = Visibility.Collapsed;
                TrafficMeterPanel.Visibility = Visibility.Collapsed;
                TotalCanvas.Visibility = Visibility.Collapsed;
                AverageCanvas.Visibility = Visibility.Collapsed;
            }
            else if (TrafficMeterInfo.isTrafficMeterEnabled == "1")
            {
                checkTrafficMeter.IsChecked = true;
                //TrafficMeterLongListSelector.Visibility = Visibility.Visible;
                TrafficMeterPanel.Visibility = Visibility.Visible;
                TotalCanvas.Visibility = Visibility.Visible;
                AverageCanvas.Visibility = Visibility.Visible;
            }
        }

        // 为 TrafficMeterModel 项加载数据
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            //settingModel.TrafficMeterGroups.Clear();
            //settingModel.StartDate.Clear();
            //settingModel.TrafficLimitation.Clear();
            settingModel.LoadData();
            tbLimitPerMonth.Text = TrafficMeterInfo.MonthlyLimit;
            if (TrafficMeterInfo.changedRestartDay == DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month).ToString())
            {
                tbStartDate.Text = AppResources.LastDay;
            }
            else
            {
                tbStartDate.Text = TrafficMeterInfo.RestartDay;
            }
            tbStartTime.Text = TrafficMeterInfo.RestartHour + ":" + TrafficMeterInfo.RestartMinute;
            if (TrafficMeterInfo.ControlOption == "No limit")
            {
                tbTrafficLimitation.Text = AppResources.TrafficLimitation_Unlimited;
            }
            else if (TrafficMeterInfo.ControlOption == "Download only")
            {
                tbTrafficLimitation.Text = AppResources.TrafficLimitation_Download;
            }
            else if (TrafficMeterInfo.ControlOption == "Both directions")
            {
                tbTrafficLimitation.Text = AppResources.TrafficLimitation_DownloadUpload;
            }
            double width;
            if ((this.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                width = Application.Current.Host.Content.ActualWidth;
                PageTitle.Width = Application.Current.Host.Content.ActualWidth - 20;
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                width = Application.Current.Host.Content.ActualHeight - 150;
                PageTitle.Width = Application.Current.Host.Content.ActualHeight - 150;
            }
            DrawTrafficMeterTable(width);
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private void PhoneApplicationPage_OrientationChanged(Object sender, OrientationChangedEventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            double width;
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                width = Application.Current.Host.Content.ActualWidth;
                PageTitle.Width = Application.Current.Host.Content.ActualWidth - 20;
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                width = Application.Current.Host.Content.ActualHeight - 150;
                PageTitle.Width = Application.Current.Host.Content.ActualHeight - 150;
            }
            DrawTrafficMeterTable(width);
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private void DrawTrafficMeterTable(double width)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            string[] WeekUpload = TrafficMeterInfo.WeekUpload.Split('/');
            string[] WeekDownload = TrafficMeterInfo.WeekDownload.Split('/');
            string[] MonthUpload = TrafficMeterInfo.MonthUpload.Split('/');
            string[] MonthDownload = TrafficMeterInfo.MonthDownload.Split('/');
            string[] LastMonthUpload = TrafficMeterInfo.LastMonthUpload.Split('/');
            string[] LastMonthDownload = TrafficMeterInfo.LastMonthDownload.Split('/');
            double[] arryTotal = {
                                     double.Parse(TrafficMeterInfo.TodayUpload) + double.Parse(TrafficMeterInfo.TodayDownload),
                                     double.Parse(TrafficMeterInfo.YesterdayUpload) + double.Parse(TrafficMeterInfo.YesterdayDownload),
                                     double.Parse(WeekUpload[0]) + double.Parse(WeekDownload[0]),
                                     double.Parse(MonthUpload[0]) + double.Parse(MonthDownload[0]),
                                     double.Parse(LastMonthUpload[0]) + double.Parse(LastMonthDownload[0])
                                 };
            double[] arryAvg = {
                                   double.Parse(WeekUpload[1]) + double.Parse(WeekDownload[1]),
                                   double.Parse(MonthUpload[1]) + double.Parse(MonthDownload[1]),
                                   double.Parse(LastMonthUpload[1]) + double.Parse(LastMonthDownload[1])
                               };
            double maxTotalValue = Max(arryTotal);
            double maxAvgValue = Max(arryAvg);
            //纵坐标取值范围计算
            int maxOrdinateTotal = 0;
            int maxOrdinateAvg = 0;
            if (maxTotalValue >= 0 && maxTotalValue <= 10)
            {
                maxOrdinateTotal = 10;
            }
            else if (maxTotalValue > 10 && maxTotalValue <= 1000)
            {
                maxOrdinateTotal = (int)(maxTotalValue / 10 + 1) * 10;
            }
            else if (maxTotalValue > 1000 && maxTotalValue <= 10000)
            {
                maxOrdinateTotal = (int)(maxTotalValue / 100 + 1) * 100;
            }
            else if (maxTotalValue > 10000 && maxTotalValue <= 100000)
            {
                maxOrdinateTotal = (int)(maxTotalValue / 1000 + 1) * 1000;
            }
            else if (maxTotalValue > 100000 && maxTotalValue <= 1000000)
            {
                maxOrdinateTotal = (int)(maxTotalValue / 10000 + 1) * 10000;
            }
            else
            {
                MessageBox.Show("Need to expand the judgment of maxTotalValue.");
            }
            TotalMbytes_1.Text = (maxOrdinateTotal / 5).ToString();
            TotalMbytes_2.Text = (maxOrdinateTotal / 5 * 2).ToString();
            TotalMbytes_3.Text = (maxOrdinateTotal / 5 * 3).ToString();
            TotalMbytes_4.Text = (maxOrdinateTotal / 5 * 4).ToString();
            TotalMbytes_5.Text = maxOrdinateTotal.ToString();

            if (maxAvgValue >= 0 && maxAvgValue <= 10)
            {
                maxOrdinateAvg = 10;
            }
            else if (maxAvgValue > 10 && maxAvgValue <= 1000)
            {
                maxOrdinateAvg = (int)(maxAvgValue / 10 + 1) * 10;
            }
            else if (maxAvgValue > 1000 && maxAvgValue <= 10000)
            {
                maxOrdinateAvg = (int)(maxAvgValue / 100 + 1) * 100;
            }
            else if (maxAvgValue > 10000 && maxAvgValue <= 100000)
            {
                maxOrdinateAvg = (int)(maxAvgValue / 1000 + 1) * 1000;
            }
            else if (maxAvgValue > 100000 && maxAvgValue <= 1000000)
            {
                maxOrdinateAvg = (int)(maxAvgValue / 10000 + 1) * 10000;
            }
            else
            {
                MessageBox.Show("Need to expand the judgment of maxAvgValue.");
            }
            AvgMbytes_1.Text = (maxOrdinateAvg / 5).ToString();
            AvgMbytes_2.Text = (maxOrdinateAvg / 5 * 2).ToString();
            AvgMbytes_3.Text = (maxOrdinateAvg / 5 * 3).ToString();
            AvgMbytes_4.Text = (maxOrdinateAvg / 5 * 4).ToString();
            AvgMbytes_5.Text = maxOrdinateAvg.ToString();

            TotalCanvas.Children.Clear();
            AverageCanvas.Children.Clear();
            TotalCanvas.Children.Add(TextMbytesTotal);
            TotalCanvas.Children.Add(TotalMbytes_0);
            TotalCanvas.Children.Add(TotalMbytes_1);
            TotalCanvas.Children.Add(TotalMbytes_2);
            TotalCanvas.Children.Add(TotalMbytes_3);
            TotalCanvas.Children.Add(TotalMbytes_4);
            TotalCanvas.Children.Add(TotalMbytes_5);
            AverageCanvas.Children.Add(TextMbytesAverage);
            AverageCanvas.Children.Add(AvgMbytes_0);
            AverageCanvas.Children.Add(AvgMbytes_1);
            AverageCanvas.Children.Add(AvgMbytes_2);
            AverageCanvas.Children.Add(AvgMbytes_3);
            AverageCanvas.Children.Add(AvgMbytes_4);
            AverageCanvas.Children.Add(AvgMbytes_5);

            //double width = Window.Current.Bounds.Width;
            TotalCanvas.Width = width - 24;
            AverageCanvas.Width = width - 24;

            TextTotal.Margin = new Thickness((width - 50) / 2, 10, 0, 0);
            TextAverage.Margin = new Thickness((width - 50) / 2, 10, 0, 0);
            double intervalTotal = (width - 100) / 11;
            double intervalAvg = (width - 100) / 7;
            TodayTotal.Margin = new Thickness(60 + intervalTotal, 480, 0, 0);
            TodayTotal.Width = intervalTotal;
            TodayTotal.TextAlignment = TextAlignment.Center;
            YesterdayTotal.Margin = new Thickness(60 + intervalTotal * 3, 480, 0, 0);
            YesterdayTotal.Width = intervalTotal;
            YesterdayTotal.TextAlignment = TextAlignment.Center;
            WeekTotal.Margin = new Thickness(60 + intervalTotal * 5, 480, 0, 0);
            WeekTotal.Width = intervalTotal;
            WeekTotal.TextAlignment = TextAlignment.Center;
            MonthTotal.Margin = new Thickness(60 + intervalTotal * 7, 480, 0, 0);
            MonthTotal.Width = intervalTotal;
            MonthTotal.TextAlignment = TextAlignment.Center;
            LastMonthTotal.Margin = new Thickness(60 + intervalTotal * 9, 480, 0, 0);
            LastMonthTotal.Width = intervalTotal;
            LastMonthTotal.TextAlignment = TextAlignment.Center;

            WeekAvg.Margin = new Thickness(60 + intervalAvg, 480, 0, 0);
            WeekAvg.Width = intervalAvg;
            WeekAvg.TextAlignment = TextAlignment.Center;
            MonthAvg.Margin = new Thickness(60 + intervalAvg * 3, 480, 0, 0);
            MonthAvg.Width = intervalAvg;
            MonthAvg.TextAlignment = TextAlignment.Center;
            LastMonthAvg.Margin = new Thickness(60 + intervalAvg * 5, 480, 0, 0);
            LastMonthAvg.Width = intervalAvg;
            LastMonthAvg.TextAlignment = TextAlignment.Center;
            TotalCanvas.Children.Add(TextTotal);
            TotalCanvas.Children.Add(TodayTotal);
            TotalCanvas.Children.Add(YesterdayTotal);
            TotalCanvas.Children.Add(WeekTotal);
            TotalCanvas.Children.Add(MonthTotal);
            TotalCanvas.Children.Add(LastMonthTotal);
            AverageCanvas.Children.Add(TextAverage);
            AverageCanvas.Children.Add(WeekAvg);
            AverageCanvas.Children.Add(MonthAvg);
            AverageCanvas.Children.Add(LastMonthAvg);

            //流量图的坐标线条绘制
            Line TotalYaxis = new Line();
            TotalYaxis.Stroke = new SolidColorBrush(Colors.Black);
            TotalYaxis.StrokeThickness = 2;
            TotalYaxis.X1 = 60; TotalYaxis.Y1 = 60;
            TotalYaxis.X2 = 60; TotalYaxis.Y2 = 470;
            TotalYaxis.Width = width - 30;
            TotalYaxis.Height = 550;
            TotalCanvas.Children.Add(TotalYaxis);

            Line TotalXaxis = new Line();
            TotalXaxis.Stroke = new SolidColorBrush(Colors.Black);
            TotalXaxis.StrokeThickness = 2;
            TotalXaxis.X1 = 60; TotalXaxis.Y1 = 470;
            TotalXaxis.X2 = width - 60; TotalXaxis.Y2 = 470;
            TotalXaxis.Width = width - 30;
            TotalXaxis.Height = 550;
            TotalCanvas.Children.Add(TotalXaxis);

            for (int i = 0; i < 5; i++)
            {
                DoubleCollection dc = new DoubleCollection();
                dc.Add(3);
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 1;
                line.StrokeDashArray = dc;
                line.X1 = 60; line.Y1 = 70 + i * 80;
                line.X2 = width - 60; line.Y2 = 70 + i * 80;
                line.Width = width - 30;
                line.Height = 550;
                TotalCanvas.Children.Add(line);
            }

            Line AverageYaxis = new Line();
            AverageYaxis.Stroke = new SolidColorBrush(Colors.Black);
            AverageYaxis.StrokeThickness = 2;
            AverageYaxis.X1 = 60; AverageYaxis.Y1 = 60;
            AverageYaxis.X2 = 60; AverageYaxis.Y2 = 470;
            AverageYaxis.Width = width - 30;
            AverageYaxis.Height = 550;
            AverageCanvas.Children.Add(AverageYaxis);

            Line AverageXaxis = new Line();
            AverageXaxis.Stroke = new SolidColorBrush(Colors.Black);
            AverageXaxis.StrokeThickness = 2;
            AverageXaxis.X1 = 60; AverageXaxis.Y1 = 470;
            AverageXaxis.X2 = width - 60; AverageXaxis.Y2 = 470;
            AverageXaxis.Width = width - 30;
            AverageXaxis.Height = 550;
            AverageCanvas.Children.Add(AverageXaxis);

            for (int i = 0; i < 5; i++)
            {
                DoubleCollection dc = new DoubleCollection();
                dc.Add(3);
                Line line = new Line();
                line.Stroke = new SolidColorBrush(Colors.Black);
                line.StrokeThickness = 1;
                line.StrokeDashArray = dc;
                line.X1 = 60; line.Y1 = 70 + i * 80;
                line.X2 = width - 60; line.Y2 = 70 + i * 80;
                line.Width = width - 30;
                line.Height = 550;
                AverageCanvas.Children.Add(line);
            }

            //流量矩形图绘制
            //Total Today
            double TodayDownloadHeight = double.Parse(TrafficMeterInfo.TodayDownload) / maxOrdinateTotal * 400;
            Rectangle rectTodayDownload = new Rectangle();
            rectTodayDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectTodayDownload.Height = TodayDownloadHeight;
            rectTodayDownload.Width = intervalTotal;
            rectTodayDownload.Margin = new Thickness(60 + intervalTotal, 470 - TodayDownloadHeight, 0, 0);
            TodayDownloadTotal.Text = TrafficMeterInfo.TodayDownload;
            TodayDownloadTotal.TextAlignment = TextAlignment.Center;
            TodayDownloadTotal.Width = intervalTotal;
            TodayDownloadTotal.Margin = new Thickness(60 + intervalTotal, 470 - TodayDownloadHeight / 2 - 20, 0, 0);
            double TodayUploadHeight = double.Parse(TrafficMeterInfo.TodayUpload) / maxOrdinateTotal * 400;
            Rectangle rectTodayUpload = new Rectangle();
            rectTodayUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectTodayUpload.Height = TodayUploadHeight;
            rectTodayUpload.Width = intervalTotal;
            rectTodayUpload.Margin = new Thickness(60 + intervalTotal, 470 - TodayDownloadHeight - TodayUploadHeight, 0, 0);
            TodayUploadTotal.Text = TrafficMeterInfo.TodayUpload;
            TodayUploadTotal.TextAlignment = TextAlignment.Center;
            TodayUploadTotal.Width = intervalTotal;
            TodayUploadTotal.Margin = new Thickness(60 + intervalTotal, 470 - TodayDownloadHeight - TodayUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectTodayDownload);
            TotalCanvas.Children.Add(rectTodayUpload);
            TotalCanvas.Children.Add(TodayDownloadTotal);
            TotalCanvas.Children.Add(TodayUploadTotal);

            //Total Yesterday
            double YesterdayDownloadHeight = double.Parse(TrafficMeterInfo.YesterdayDownload) / maxOrdinateTotal * 400;
            Rectangle rectYesterdayDownload = new Rectangle();
            rectYesterdayDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectYesterdayDownload.Height = YesterdayDownloadHeight;
            rectYesterdayDownload.Width = intervalTotal;
            rectYesterdayDownload.Margin = new Thickness(60 + intervalTotal * 3, 470 - YesterdayDownloadHeight, 0, 0);
            YesterdayDownloadTotal.Text = TrafficMeterInfo.YesterdayDownload;
            YesterdayDownloadTotal.TextAlignment = TextAlignment.Center;
            YesterdayDownloadTotal.Width = intervalTotal;
            YesterdayDownloadTotal.Margin = new Thickness(60 + intervalTotal * 3, 470 - YesterdayDownloadHeight / 2 - 20, 0, 0);
            double YesterdayUploadHeight = double.Parse(TrafficMeterInfo.YesterdayUpload) / maxOrdinateTotal * 400;
            Rectangle rectYesterdayUpload = new Rectangle();
            rectYesterdayUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectYesterdayUpload.Height = YesterdayUploadHeight;
            rectYesterdayUpload.Width = intervalTotal;
            rectYesterdayUpload.Margin = new Thickness(60 + intervalTotal * 3, 470 - YesterdayDownloadHeight - YesterdayUploadHeight, 0, 0);
            YesterdayUploadTotal.Text = TrafficMeterInfo.YesterdayUpload;
            YesterdayUploadTotal.TextAlignment = TextAlignment.Center;
            YesterdayUploadTotal.Width = intervalTotal;
            YesterdayUploadTotal.Margin = new Thickness(60 + intervalTotal * 3, 470 - YesterdayDownloadHeight - YesterdayUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectYesterdayDownload);
            TotalCanvas.Children.Add(rectYesterdayUpload);
            TotalCanvas.Children.Add(YesterdayDownloadTotal);
            TotalCanvas.Children.Add(YesterdayUploadTotal);

            //Total This week
            double WeekDownloadHeight = double.Parse(WeekDownload[0]) / maxOrdinateTotal * 400;
            Rectangle rectWeekDownload = new Rectangle();
            rectWeekDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectWeekDownload.Height = WeekDownloadHeight;
            rectWeekDownload.Width = intervalTotal;
            rectWeekDownload.Margin = new Thickness(60 + intervalTotal * 5, 470 - WeekDownloadHeight, 0, 0);
            WeekDownloadTotal.Text = WeekDownload[0];
            WeekDownloadTotal.TextAlignment = TextAlignment.Center;
            WeekDownloadTotal.Width = intervalTotal;
            WeekDownloadTotal.Margin = new Thickness(60 + intervalTotal * 5, 470 - WeekDownloadHeight / 2 - 20, 0, 0);
            double WeekUploadHeight = double.Parse(WeekUpload[0]) / maxOrdinateTotal * 400;
            Rectangle rectWeekUpload = new Rectangle();
            rectWeekUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectWeekUpload.Height = WeekUploadHeight;
            rectWeekUpload.Width = intervalTotal;
            rectWeekUpload.Margin = new Thickness(60 + intervalTotal * 5, 470 - WeekDownloadHeight - WeekUploadHeight, 0, 0);
            WeekUploadTotal.Text = WeekUpload[0];
            WeekUploadTotal.TextAlignment = TextAlignment.Center;
            WeekUploadTotal.Width = intervalTotal;
            WeekUploadTotal.Margin = new Thickness(60 + intervalTotal * 5, 470 - WeekDownloadHeight - WeekUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectWeekDownload);
            TotalCanvas.Children.Add(rectWeekUpload);
            TotalCanvas.Children.Add(WeekDownloadTotal);
            TotalCanvas.Children.Add(WeekUploadTotal);

            //Total This month
            double MonthDownloadHeight = double.Parse(MonthDownload[0]) / maxOrdinateTotal * 400;
            Rectangle rectMonthDownload = new Rectangle();
            rectMonthDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectMonthDownload.Height = MonthDownloadHeight;
            rectMonthDownload.Width = intervalTotal;
            rectMonthDownload.Margin = new Thickness(60 + intervalTotal * 7, 470 - MonthDownloadHeight, 0, 0);
            MonthDownloadTotal.Text = MonthDownload[0];
            MonthDownloadTotal.TextAlignment = TextAlignment.Center;
            MonthDownloadTotal.Width = intervalTotal;
            MonthDownloadTotal.Margin = new Thickness(60 + intervalTotal * 7, 470 - MonthDownloadHeight / 2 - 20, 0, 0);
            double MonthUploadHeight = double.Parse(MonthUpload[0]) / maxOrdinateTotal * 400;
            Rectangle rectMonthUpload = new Rectangle();
            rectMonthUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectMonthUpload.Height = MonthUploadHeight;
            rectMonthUpload.Width = intervalTotal;
            rectMonthUpload.Margin = new Thickness(60 + intervalTotal * 7, 470 - MonthDownloadHeight - MonthUploadHeight, 0, 0);
            MonthUploadTotal.Text = MonthUpload[0];
            MonthUploadTotal.TextAlignment = TextAlignment.Center;
            MonthUploadTotal.Width = intervalTotal;
            MonthUploadTotal.Margin = new Thickness(60 + intervalTotal * 7, 470 - MonthDownloadHeight - MonthUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectMonthDownload);
            TotalCanvas.Children.Add(rectMonthUpload);
            TotalCanvas.Children.Add(MonthDownloadTotal);
            TotalCanvas.Children.Add(MonthUploadTotal);

            //Total Last month
            double LastMonthDownloadHeight = double.Parse(LastMonthDownload[0]) / maxOrdinateTotal * 400;
            Rectangle rectLastMonthDownload = new Rectangle();
            rectLastMonthDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectLastMonthDownload.Height = LastMonthDownloadHeight;
            rectLastMonthDownload.Width = intervalTotal;
            rectLastMonthDownload.Margin = new Thickness(60 + intervalTotal * 9, 470 - LastMonthDownloadHeight, 0, 0);
            LastMonthDownloadTotal.Text = LastMonthDownload[0];
            LastMonthDownloadTotal.TextAlignment = TextAlignment.Center;
            LastMonthDownloadTotal.Width = intervalTotal;
            LastMonthDownloadTotal.Margin = new Thickness(60 + intervalTotal * 9, 470 - LastMonthDownloadHeight / 2 - 20, 0, 0);
            double LastMonthUploadHeight = double.Parse(LastMonthUpload[0]) / maxOrdinateTotal * 400;
            Rectangle rectLastMonthUpload = new Rectangle();
            rectLastMonthUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectLastMonthUpload.Height = LastMonthUploadHeight;
            rectLastMonthUpload.Width = intervalTotal;
            rectLastMonthUpload.Margin = new Thickness(60 + intervalTotal * 9, 470 - LastMonthDownloadHeight - LastMonthUploadHeight, 0, 0);
            LastMonthUploadTotal.Text = LastMonthUpload[0];
            LastMonthUploadTotal.TextAlignment = TextAlignment.Center;
            LastMonthUploadTotal.Width = intervalTotal;
            LastMonthUploadTotal.Margin = new Thickness(60 + intervalTotal * 9, 470 - LastMonthDownloadHeight - LastMonthUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectLastMonthDownload);
            TotalCanvas.Children.Add(rectLastMonthUpload);
            TotalCanvas.Children.Add(LastMonthDownloadTotal);
            TotalCanvas.Children.Add(LastMonthUploadTotal);

            //Average This week
            double AvgWeekDownloadHeight = double.Parse(WeekDownload[1]) / maxOrdinateAvg * 400;
            Rectangle AvgrectWeekDownload = new Rectangle();
            AvgrectWeekDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            AvgrectWeekDownload.Height = AvgWeekDownloadHeight;
            AvgrectWeekDownload.Width = intervalAvg;
            AvgrectWeekDownload.Margin = new Thickness(60 + intervalAvg, 470 - AvgWeekDownloadHeight, 0, 0);
            WeekDownloadAvg.Text = WeekDownload[1];
            WeekDownloadAvg.TextAlignment = TextAlignment.Center;
            WeekDownloadAvg.Width = intervalAvg;
            WeekDownloadAvg.Margin = new Thickness(60 + intervalAvg, 470 - AvgWeekDownloadHeight / 2 - 20, 0, 0);
            double AvgWeekUploadHeight = double.Parse(WeekUpload[1]) / maxOrdinateAvg * 400;
            Rectangle AvgrectWeekUpload = new Rectangle();
            AvgrectWeekUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            AvgrectWeekUpload.Height = AvgWeekUploadHeight;
            AvgrectWeekUpload.Width = intervalAvg;
            AvgrectWeekUpload.Margin = new Thickness(60 + intervalAvg, 470 - AvgWeekDownloadHeight - AvgWeekUploadHeight, 0, 0);
            WeekUploadAvg.Text = WeekUpload[1];
            WeekUploadAvg.TextAlignment = TextAlignment.Center;
            WeekUploadAvg.Width = intervalAvg;
            WeekUploadAvg.Margin = new Thickness(60 + intervalAvg, 470 - AvgWeekDownloadHeight - AvgWeekUploadHeight - 40, 0, 0);
            AverageCanvas.Children.Add(AvgrectWeekDownload);
            AverageCanvas.Children.Add(AvgrectWeekUpload);
            AverageCanvas.Children.Add(WeekDownloadAvg);
            AverageCanvas.Children.Add(WeekUploadAvg);

            //Average This month
            double AvgMonthDownloadHeight = double.Parse(MonthDownload[1]) / maxOrdinateAvg * 400;
            Rectangle AvgrectMonthDownload = new Rectangle();
            AvgrectMonthDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            AvgrectMonthDownload.Height = AvgMonthDownloadHeight;
            AvgrectMonthDownload.Width = intervalAvg;
            AvgrectMonthDownload.Margin = new Thickness(60 + intervalAvg * 3, 470 - AvgMonthDownloadHeight, 0, 0);
            MonthDownloadAvg.Text = MonthDownload[1];
            MonthDownloadAvg.TextAlignment = TextAlignment.Center;
            MonthDownloadAvg.Width = intervalAvg;
            MonthDownloadAvg.Margin = new Thickness(60 + intervalAvg * 3, 470 - AvgMonthDownloadHeight / 2 - 20, 0, 0);
            double AvgMonthUploadHeight = double.Parse(MonthUpload[1]) / maxOrdinateAvg * 400;
            Rectangle AvgrectMonthUpload = new Rectangle();
            AvgrectMonthUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            AvgrectMonthUpload.Height = AvgMonthUploadHeight;
            AvgrectMonthUpload.Width = intervalAvg;
            AvgrectMonthUpload.Margin = new Thickness(60 + intervalAvg * 3, 470 - AvgMonthDownloadHeight - AvgMonthUploadHeight, 0, 0);
            MonthUploadAvg.Text = MonthUpload[1];
            MonthUploadAvg.TextAlignment = TextAlignment.Center;
            MonthUploadAvg.Width = intervalAvg;
            MonthUploadAvg.Margin = new Thickness(60 + intervalAvg * 3, 470 - AvgMonthDownloadHeight - AvgMonthUploadHeight - 40, 0, 0);
            AverageCanvas.Children.Add(AvgrectMonthDownload);
            AverageCanvas.Children.Add(AvgrectMonthUpload);
            AverageCanvas.Children.Add(MonthDownloadAvg);
            AverageCanvas.Children.Add(MonthUploadAvg);

            //Average Last month
            double AvgLastMonthDownloadHeight = double.Parse(LastMonthDownload[1]) / maxOrdinateAvg * 400;
            Rectangle AvgrectLastMonthDownload = new Rectangle();
            AvgrectLastMonthDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            AvgrectLastMonthDownload.Height = AvgLastMonthDownloadHeight;
            AvgrectLastMonthDownload.Width = intervalAvg;
            AvgrectLastMonthDownload.Margin = new Thickness(60 + intervalAvg * 5, 470 - AvgLastMonthDownloadHeight, 0, 0);
            LastMonthDownloadAvg.Text = LastMonthDownload[1];
            LastMonthDownloadAvg.TextAlignment = TextAlignment.Center;
            LastMonthDownloadAvg.Width = intervalAvg;
            LastMonthDownloadAvg.Margin = new Thickness(60 + intervalAvg * 5, 470 - AvgLastMonthDownloadHeight / 2 - 20, 0, 0);
            double AvgLastMonthUploadHeight = double.Parse(LastMonthUpload[1]) / maxOrdinateAvg * 400;
            Rectangle AvgrectLastMonthUpload = new Rectangle();
            AvgrectLastMonthUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            AvgrectLastMonthUpload.Height = AvgLastMonthUploadHeight;
            AvgrectLastMonthUpload.Width = intervalAvg;
            AvgrectLastMonthUpload.Margin = new Thickness(60 + intervalAvg * 5, 470 - AvgLastMonthDownloadHeight - AvgLastMonthUploadHeight, 0, 0);
            LastMonthUploadAvg.Text = LastMonthUpload[1];
            LastMonthUploadAvg.TextAlignment = TextAlignment.Center;
            LastMonthUploadAvg.Width = intervalAvg;
            LastMonthUploadAvg.Margin = new Thickness(60 + intervalAvg * 5, 470 - AvgLastMonthDownloadHeight - AvgLastMonthUploadHeight - 40, 0, 0);
            AverageCanvas.Children.Add(AvgrectLastMonthDownload);
            AverageCanvas.Children.Add(AvgrectLastMonthUpload);
            AverageCanvas.Children.Add(LastMonthDownloadAvg);
            AverageCanvas.Children.Add(LastMonthUploadAvg);
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;

            //颜色含义指示绘制
            InstructionTotal.Margin = new Thickness(width - 220, 520, 0, 0);
            TotalCanvas.Children.Add(InstructionTotal);
            InstructionAvg.Margin = new Thickness(width - 220, 520, 0, 0);
            AverageCanvas.Children.Add(InstructionAvg);
        }

        //// 处理在 LongListSelector 中更改的选定内容
        //private void TrafficMeterLongListSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    // 如果所选项为空(没有选定内容)，则不执行任何操作
        //    if (TrafficMeterLongListSelector.SelectedItem == null)
        //        return;

        //    // 导航到新页面
        //    NavigationService.Navigate(new Uri("/TrafficMeterSettingPage.xaml", UriKind.Relative));

        //    // 将所选项重置为 null (没有选定内容)
        //    TrafficMeterLongListSelector.SelectedItem = null;
        //}

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

        //checkbox控件响应事件
        private void checkTrafficMeter_Click(Object sender, RoutedEventArgs e)
        {
            if (checkTrafficMeter.IsChecked == true)
            {
                TrafficEnableEnquire.IsOpen = true;
                TrafficDisableEnquire.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
            }
            else if (checkTrafficMeter.IsChecked == false)
            {
                TrafficEnableEnquire.IsOpen = false;
                TrafficDisableEnquire.IsOpen = true;               
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
            }
        }

        //“是”按钮响应事件
        private async void YesButton_Click(Object sender, RoutedEventArgs e)
        {
            TrafficEnableEnquire.IsOpen = false;
            TrafficDisableEnquire.IsOpen = false;  
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            string trafficMeterEnable;
            if (checkTrafficMeter.IsChecked == true)
            {
                trafficMeterEnable = "1";
                dicResponse = await soapApi.EnableTrafficMeter(trafficMeterEnable);
                //TrafficMeterLongListSelector.Visibility = Visibility.Visible;
                TrafficMeterPanel.Visibility = Visibility.Visible;
                TotalCanvas.Visibility = Visibility.Visible;
                AverageCanvas.Visibility = Visibility.Visible;
            }
            else if (checkTrafficMeter.IsChecked == false)
            {
                trafficMeterEnable = "0";
                dicResponse = await soapApi.EnableTrafficMeter(trafficMeterEnable);
                //TrafficMeterLongListSelector.Visibility = Visibility.Collapsed;
                TrafficMeterPanel.Visibility = Visibility.Collapsed;
                TotalCanvas.Visibility = Visibility.Collapsed;
                AverageCanvas.Visibility = Visibility.Collapsed;
            }
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        //“否”按钮响应事件
        private void NoButton_Click(Object sender, RoutedEventArgs e)
        {
            if (checkTrafficMeter.IsChecked == true)
            {
                checkTrafficMeter.IsChecked = false;
            }
            else if (checkTrafficMeter.IsChecked == false)
            {
                checkTrafficMeter.IsChecked = true;
            }
            TrafficEnableEnquire.IsOpen = false;
            TrafficDisableEnquire.IsOpen = false;  
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        //刷新按钮响应事件
        private async void appBarButton_refresh_Click(object sender, EventArgs e)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetTrafficMeterEnabled();
            if (dicResponse.Count > 0)
            {
                TrafficMeterInfo.isTrafficMeterEnabled = dicResponse["NewTrafficMeterEnable"];
                if (TrafficMeterInfo.isTrafficMeterEnabled == "0")
                {
                    checkTrafficMeter.IsChecked = false;
                    //TrafficMeterLongListSelector.Visibility = Visibility.Collapsed;
                    TrafficMeterPanel.Visibility = Visibility.Collapsed;
                    TotalCanvas.Visibility = Visibility.Collapsed;
                    AverageCanvas.Visibility = Visibility.Collapsed;
                }
                else if (TrafficMeterInfo.isTrafficMeterEnabled == "1")
                {
                    checkTrafficMeter.IsChecked = true;
                    //TrafficMeterLongListSelector.Visibility = Visibility.Visible;
                    TrafficMeterPanel.Visibility = Visibility.Visible;
                    TotalCanvas.Visibility = Visibility.Visible;
                    AverageCanvas.Visibility = Visibility.Visible;
                    Dictionary<string, string> dicResponse1 = new Dictionary<string, string>();
                    dicResponse1 = await soapApi.GetTrafficMeterOptions();
                    if (dicResponse1.Count > 0)
                    {
                        TrafficMeterInfo.MonthlyLimit = dicResponse1["NewMonthlyLimit"];
                        TrafficMeterInfo.changedMonthlyLimit = dicResponse1["NewMonthlyLimit"];
                        TrafficMeterInfo.RestartHour = dicResponse1["RestartHour"];
                        TrafficMeterInfo.changedRestartHour = dicResponse1["RestartHour"];
                        TrafficMeterInfo.RestartMinute = dicResponse1["RestartMinute"];
                        TrafficMeterInfo.changedRestartMinute = dicResponse1["RestartMinute"];
                        TrafficMeterInfo.RestartDay = dicResponse1["RestartDay"];
                        TrafficMeterInfo.changedRestartDay = dicResponse1["RestartDay"];
                        TrafficMeterInfo.ControlOption = dicResponse1["NewControlOption"];
                        TrafficMeterInfo.changedControlOption = dicResponse1["NewControlOption"];

                        Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                        dicResponse2 = await soapApi.GetTrafficMeterStatistics();
                        if (dicResponse2.Count > 0)
                        {
                            TrafficMeterInfo.TodayUpload = dicResponse2["NewTodayUpload"];
                            TrafficMeterInfo.TodayDownload = dicResponse2["NewTodayDownload"];
                            TrafficMeterInfo.YesterdayUpload = dicResponse2["NewYesterdayUpload"];
                            TrafficMeterInfo.YesterdayDownload = dicResponse2["NewYesterdayDownload"];
                            TrafficMeterInfo.WeekUpload = dicResponse2["NewWeekUpload"];
                            TrafficMeterInfo.WeekDownload = dicResponse2["NewWeekDownload"];
                            TrafficMeterInfo.MonthUpload = dicResponse2["NewMonthUpload"];
                            TrafficMeterInfo.MonthDownload = dicResponse2["NewMonthDownload"];
                            TrafficMeterInfo.LastMonthUpload = dicResponse2["NewLastMonthUpload"];
                            TrafficMeterInfo.LastMonthDownload = dicResponse2["NewLastMonthDownload"];

                            PopupBackgroundTop.Visibility = Visibility.Collapsed;
                            PopupBackground.Visibility = Visibility.Collapsed;
                            OnNavigatedTo(null);
                        }
                        else
                        {
                            PopupBackgroundTop.Visibility = Visibility.Collapsed;
                            PopupBackground.Visibility = Visibility.Collapsed;
                            MessageBox.Show("GetTrafficMeterStatistics failed!");
                        }
                    }
                    else
                    {
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        MessageBox.Show("GetGuestAccessNetworkInfo failed!");
                    }
                }
                else if (TrafficMeterInfo.isTrafficMeterEnabled == "2")
                {
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    MessageBox.Show(AppResources.notsupport);
                    NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
                }
            }
            else
            {
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                MessageBox.Show("GetGuestAccessEnabled failed!");
            }
        }

        static double Max(double[] arry)
        {
            double max = 0;
            for (int i = 0; i < arry.Length; i++)
            {
                if (max <= arry[i])
                {
                    max = arry[i];
                }
            }
            return max;
        }

        private void Grid_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid gridItem = (Grid)sender;
            switch (gridItem.Name)
            {
                case "gridLimitPerMonth":
                    gridLimitPerMonth.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                case "gridStartDate":
                    gridStartDate.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                case "gridStartTime":
                    gridStartTime.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                case "gridTrafficLimitation":
                    gridTrafficLimitation.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                default:
                    break;
            }
        }

        private void Grid_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Grid gridItem = (Grid)sender;
            switch (gridItem.Name)
            {
                case "gridLimitPerMonth":
                    gridLimitPerMonth.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridStartDate":
                    gridStartDate.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridStartTime":
                    gridStartTime.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridTrafficLimitation":
                    gridTrafficLimitation.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                default:
                    break;
            }
        }

        private void Grid_MouseLeftButtonUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Grid gridItem = (Grid)sender;
            switch (gridItem.Name)
            {
                case "gridLimitPerMonth":
                    gridLimitPerMonth.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridStartDate":
                    gridStartDate.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridStartTime":
                    gridStartTime.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridTrafficLimitation":
                    gridTrafficLimitation.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                default:
                    break;
            }
            NavigationService.Navigate(new Uri("/TrafficMeterSettingPage.xaml", UriKind.Relative));
        }
    }
}