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
using GenieWin8.DataModel;
using Windows.UI.Popups;
using Windows.Networking.Connectivity;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class TrafficMeterPage : GenieWin8.Common.LayoutAwarePage
    {
        private static bool IsWifiSsidChanged;
        public TrafficMeterPage()
        {
            this.InitializeComponent();
            Application.Current.Resuming += new EventHandler<Object>(App_Resuming);
            if (TrafficMeterInfoModel.isTrafficMeterEnabled == "0")
            {
                checkTrafficMeter.IsChecked = false;
                TrafficMeterList.Visibility = Visibility.Collapsed;
                TotalCanvas.Visibility = Visibility.Collapsed;
                AverageCanvas.Visibility = Visibility.Collapsed;
            }
            else if (TrafficMeterInfoModel.isTrafficMeterEnabled == "1")
            {
                checkTrafficMeter.IsChecked = true;
                TrafficMeterList.Visibility = Visibility.Visible;
                TotalCanvas.Visibility = Visibility.Visible;
                AverageCanvas.Visibility = Visibility.Visible;
            }
        }

        private void App_Resuming(Object sender, Object e)
        {
            //判断所连接Wifi的Ssid是否改变
            IsWifiSsidChanged = true;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        if (connectionProfile.ProfileName == MainPageInfo.ssid)
                            IsWifiSsidChanged = false;
                        else
                            IsWifiSsidChanged = true;
                    }
                }
            }
            catch (Exception ex)
            {
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
            //判断所连接Wifi的Ssid是否改变
            IsWifiSsidChanged = true;
            try
            {
                var ConnectionProfiles = NetworkInformation.GetConnectionProfiles();
                foreach (var connectionProfile in ConnectionProfiles)
                {
                    if (connectionProfile.GetNetworkConnectivityLevel() != NetworkConnectivityLevel.None)
                    {
                        if (connectionProfile.ProfileName == MainPageInfo.ssid)
                            IsWifiSsidChanged = false;
                        else
                            IsWifiSsidChanged = true;
                    }
                }
            }
            catch (Exception ex)
            {
            }

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
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "TrafficMeterPage";
            } 
            else
            {
                this.Frame.Navigate(typeof(TrafficCtrlSettingPage));
            }
        }

        private async void OnWindowSizeChanged(Object sender, SizeChangedEventArgs e)
        {
            InProgress.IsActive = true;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            string[] WeekUpload = TrafficMeterInfoModel.WeekUpload.Split('/');
            string[] WeekDownload = TrafficMeterInfoModel.WeekDownload.Split('/');
            string[] MonthUpload = TrafficMeterInfoModel.MonthUpload.Split('/');
            string[] MonthDownload = TrafficMeterInfoModel.MonthDownload.Split('/');
            string[] LastMonthUpload = TrafficMeterInfoModel.LastMonthUpload.Split('/');
            string[] LastMonthDownload = TrafficMeterInfoModel.LastMonthDownload.Split('/');
            double[] arryTotal = {
                                     double.Parse(TrafficMeterInfoModel.TodayUpload) + double.Parse(TrafficMeterInfoModel.TodayDownload),
                                     double.Parse(TrafficMeterInfoModel.YesterdayUpload) + double.Parse(TrafficMeterInfoModel.YesterdayDownload),
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
                var messageDialog = new MessageDialog("Need to expand the judgment of maxTotalValue.");
                await messageDialog.ShowAsync();
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
                var messageDialog = new MessageDialog("Need to expand the judgment of maxAvgValue.");
                await messageDialog.ShowAsync();
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

            double width = Window.Current.Bounds.Width;
            TextTotal.Margin = new Thickness((width - 200) / 2, 20, 0, 0);
            TextAverage.Margin = new Thickness((width - 200) / 2, 20, 0, 0);
            double intervalTotal = (width - 300) / 11;
            double intervalAvg = (width - 300) / 7;
            TodayTotal.Margin = new Thickness(100 + intervalTotal, 760, 0, 0);
            TodayTotal.Width = intervalTotal;
            TodayTotal.TextAlignment = TextAlignment.Center;
            YesterdayTotal.Margin = new Thickness(100 + intervalTotal * 3, 760, 0, 0);
            YesterdayTotal.Width = intervalTotal;
            YesterdayTotal.TextAlignment = TextAlignment.Center;
            WeekTotal.Margin = new Thickness(100 + intervalTotal * 5, 760, 0, 0);
            WeekTotal.Width = intervalTotal;
            WeekTotal.TextAlignment = TextAlignment.Center;
            MonthTotal.Margin = new Thickness(100 + intervalTotal * 7, 760, 0, 0);
            MonthTotal.Width = intervalTotal;
            MonthTotal.TextAlignment = TextAlignment.Center;
            LastMonthTotal.Margin = new Thickness(100 + intervalTotal * 9, 760, 0, 0);
            LastMonthTotal.Width = intervalTotal;
            LastMonthTotal.TextAlignment = TextAlignment.Center;

            WeekAvg.Margin = new Thickness(100 + intervalAvg, 760, 0, 0);
            WeekAvg.Width = intervalAvg;
            WeekAvg.TextAlignment = TextAlignment.Center;
            MonthAvg.Margin = new Thickness(100 + intervalAvg * 3, 760, 0, 0);
            MonthAvg.Width = intervalAvg;
            MonthAvg.TextAlignment = TextAlignment.Center;
            LastMonthAvg.Margin = new Thickness(100 + intervalAvg * 5, 760, 0, 0);
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

            //流量矩形图绘制
            //Total Today
            double TodayDownloadHeight = double.Parse(TrafficMeterInfoModel.TodayDownload) / maxOrdinateTotal * 650;
            Rectangle rectTodayDownload = new Rectangle();
            rectTodayDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectTodayDownload.Height = TodayDownloadHeight;
            rectTodayDownload.Width = intervalTotal;
            rectTodayDownload.Margin = new Thickness(100 + intervalTotal, 750 - TodayDownloadHeight, 0, 0);
            TodayDownloadTotal.Text = TrafficMeterInfoModel.TodayDownload;
            TodayDownloadTotal.TextAlignment = TextAlignment.Center;
            TodayDownloadTotal.Width = intervalTotal;
            TodayDownloadTotal.Margin = new Thickness(100 + intervalTotal, 750 - TodayDownloadHeight / 2 - 20, 0, 0);
            double TodayUploadHeight = double.Parse(TrafficMeterInfoModel.TodayUpload) / maxOrdinateTotal * 650;
            Rectangle rectTodayUpload = new Rectangle();
            rectTodayUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectTodayUpload.Height = TodayUploadHeight;
            rectTodayUpload.Width = intervalTotal;
            rectTodayUpload.Margin = new Thickness(100 + intervalTotal, 750 - TodayDownloadHeight - TodayUploadHeight, 0, 0);
            TodayUploadTotal.Text = TrafficMeterInfoModel.TodayUpload;
            TodayUploadTotal.TextAlignment = TextAlignment.Center;
            TodayUploadTotal.Width = intervalTotal;
            TodayUploadTotal.Margin = new Thickness(100 + intervalTotal, 750 - TodayDownloadHeight - TodayUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectTodayDownload);
            TotalCanvas.Children.Add(rectTodayUpload);
            TotalCanvas.Children.Add(TodayDownloadTotal);
            TotalCanvas.Children.Add(TodayUploadTotal);

            //Total Yesterday
            double YesterdayDownloadHeight = double.Parse(TrafficMeterInfoModel.YesterdayDownload) / maxOrdinateTotal * 650;
            Rectangle rectYesterdayDownload = new Rectangle();
            rectYesterdayDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectYesterdayDownload.Height = YesterdayDownloadHeight;
            rectYesterdayDownload.Width = intervalTotal;
            rectYesterdayDownload.Margin = new Thickness(100 + intervalTotal * 3, 750 - YesterdayDownloadHeight, 0, 0);
            YesterdayDownloadTotal.Text = TrafficMeterInfoModel.YesterdayDownload;
            YesterdayDownloadTotal.TextAlignment = TextAlignment.Center;
            YesterdayDownloadTotal.Width = intervalTotal;
            YesterdayDownloadTotal.Margin = new Thickness(100 + intervalTotal * 3, 750 - YesterdayDownloadHeight / 2 - 20, 0, 0);
            double YesterdayUploadHeight = double.Parse(TrafficMeterInfoModel.YesterdayUpload) / maxOrdinateTotal * 650;
            Rectangle rectYesterdayUpload = new Rectangle();
            rectYesterdayUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectYesterdayUpload.Height = YesterdayUploadHeight;
            rectYesterdayUpload.Width = intervalTotal;
            rectYesterdayUpload.Margin = new Thickness(100 + intervalTotal * 3, 750 - YesterdayDownloadHeight - YesterdayUploadHeight, 0, 0);
            YesterdayUploadTotal.Text = TrafficMeterInfoModel.YesterdayUpload;
            YesterdayUploadTotal.TextAlignment = TextAlignment.Center;
            YesterdayUploadTotal.Width = intervalTotal;
            YesterdayUploadTotal.Margin = new Thickness(100 + intervalTotal * 3, 750 - YesterdayDownloadHeight - YesterdayUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectYesterdayDownload);
            TotalCanvas.Children.Add(rectYesterdayUpload);
            TotalCanvas.Children.Add(YesterdayDownloadTotal);
            TotalCanvas.Children.Add(YesterdayUploadTotal);

            //Total This week
            double WeekDownloadHeight = double.Parse(WeekDownload[0]) / maxOrdinateTotal * 650;
            Rectangle rectWeekDownload = new Rectangle();
            rectWeekDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectWeekDownload.Height = WeekDownloadHeight;
            rectWeekDownload.Width = intervalTotal;
            rectWeekDownload.Margin = new Thickness(100 + intervalTotal * 5, 750 - WeekDownloadHeight, 0, 0);
            WeekDownloadTotal.Text = WeekDownload[0];
            WeekDownloadTotal.TextAlignment = TextAlignment.Center;
            WeekDownloadTotal.Width = intervalTotal;
            WeekDownloadTotal.Margin = new Thickness(100 + intervalTotal * 5, 750 - WeekDownloadHeight / 2 - 20, 0, 0);
            double WeekUploadHeight = double.Parse(WeekUpload[0]) / maxOrdinateTotal * 650;
            Rectangle rectWeekUpload = new Rectangle();
            rectWeekUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectWeekUpload.Height = WeekUploadHeight;
            rectWeekUpload.Width = intervalTotal;
            rectWeekUpload.Margin = new Thickness(100 + intervalTotal * 5, 750 - WeekDownloadHeight - WeekUploadHeight, 0, 0);
            WeekUploadTotal.Text = WeekUpload[0];
            WeekUploadTotal.TextAlignment = TextAlignment.Center;
            WeekUploadTotal.Width = intervalTotal;
            WeekUploadTotal.Margin = new Thickness(100 + intervalTotal * 5, 750 - WeekDownloadHeight - WeekUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectWeekDownload);
            TotalCanvas.Children.Add(rectWeekUpload);
            TotalCanvas.Children.Add(WeekDownloadTotal);
            TotalCanvas.Children.Add(WeekUploadTotal);

            //Total This month
            double MonthDownloadHeight = double.Parse(MonthDownload[0]) / maxOrdinateTotal * 650;
            Rectangle rectMonthDownload = new Rectangle();
            rectMonthDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectMonthDownload.Height = MonthDownloadHeight;
            rectMonthDownload.Width = intervalTotal;
            rectMonthDownload.Margin = new Thickness(100 + intervalTotal * 7, 750 - MonthDownloadHeight, 0, 0);
            MonthDownloadTotal.Text = MonthDownload[0];
            MonthDownloadTotal.TextAlignment = TextAlignment.Center;
            MonthDownloadTotal.Width = intervalTotal;
            MonthDownloadTotal.Margin = new Thickness(100 + intervalTotal * 7, 750 - MonthDownloadHeight / 2 - 20, 0, 0);
            double MonthUploadHeight = double.Parse(MonthUpload[0]) / maxOrdinateTotal * 650;
            Rectangle rectMonthUpload = new Rectangle();
            rectMonthUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectMonthUpload.Height = MonthUploadHeight;
            rectMonthUpload.Width = intervalTotal;
            rectMonthUpload.Margin = new Thickness(100 + intervalTotal * 7, 750 - MonthDownloadHeight - MonthUploadHeight, 0, 0);
            MonthUploadTotal.Text = MonthUpload[0];
            MonthUploadTotal.TextAlignment = TextAlignment.Center;
            MonthUploadTotal.Width = intervalTotal;
            MonthUploadTotal.Margin = new Thickness(100 + intervalTotal * 7, 750 - MonthDownloadHeight - MonthUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectMonthDownload);
            TotalCanvas.Children.Add(rectMonthUpload);
            TotalCanvas.Children.Add(MonthDownloadTotal);
            TotalCanvas.Children.Add(MonthUploadTotal);

            //Total Last month
            double LastMonthDownloadHeight = double.Parse(LastMonthDownload[0]) / maxOrdinateTotal * 650;
            Rectangle rectLastMonthDownload = new Rectangle();
            rectLastMonthDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            rectLastMonthDownload.Height = LastMonthDownloadHeight;
            rectLastMonthDownload.Width = intervalTotal;
            rectLastMonthDownload.Margin = new Thickness(100 + intervalTotal * 9, 750 - LastMonthDownloadHeight, 0, 0);
            LastMonthDownloadTotal.Text = LastMonthDownload[0];
            LastMonthDownloadTotal.TextAlignment = TextAlignment.Center;
            LastMonthDownloadTotal.Width = intervalTotal;
            LastMonthDownloadTotal.Margin = new Thickness(100 + intervalTotal * 9, 750 - LastMonthDownloadHeight / 2 - 20, 0, 0);
            double LastMonthUploadHeight = double.Parse(LastMonthUpload[0]) / maxOrdinateTotal * 650;
            Rectangle rectLastMonthUpload = new Rectangle();
            rectLastMonthUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            rectLastMonthUpload.Height = LastMonthUploadHeight;
            rectLastMonthUpload.Width = intervalTotal;
            rectLastMonthUpload.Margin = new Thickness(100 + intervalTotal * 9, 750 - LastMonthDownloadHeight - LastMonthUploadHeight, 0, 0);
            LastMonthUploadTotal.Text = LastMonthUpload[0];
            LastMonthUploadTotal.TextAlignment = TextAlignment.Center;
            LastMonthUploadTotal.Width = intervalTotal;
            LastMonthUploadTotal.Margin = new Thickness(100 + intervalTotal * 9, 750 - LastMonthDownloadHeight - LastMonthUploadHeight - 40, 0, 0);
            TotalCanvas.Children.Add(rectLastMonthDownload);
            TotalCanvas.Children.Add(rectLastMonthUpload);
            TotalCanvas.Children.Add(LastMonthDownloadTotal);
            TotalCanvas.Children.Add(LastMonthUploadTotal);

            //Average This week
            double AvgWeekDownloadHeight = double.Parse(WeekDownload[1]) / maxOrdinateAvg * 650;
            Rectangle AvgrectWeekDownload = new Rectangle();
            AvgrectWeekDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            AvgrectWeekDownload.Height = AvgWeekDownloadHeight;
            AvgrectWeekDownload.Width = intervalAvg;
            AvgrectWeekDownload.Margin = new Thickness(100 + intervalAvg, 750 - AvgWeekDownloadHeight, 0, 0);
            WeekDownloadAvg.Text = WeekDownload[1];
            WeekDownloadAvg.TextAlignment = TextAlignment.Center;
            WeekDownloadAvg.Width = intervalAvg;
            WeekDownloadAvg.Margin = new Thickness(100 + intervalAvg, 750 - AvgWeekDownloadHeight / 2 - 20, 0, 0);
            double AvgWeekUploadHeight = double.Parse(WeekUpload[1]) / maxOrdinateAvg * 650;
            Rectangle AvgrectWeekUpload = new Rectangle();
            AvgrectWeekUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            AvgrectWeekUpload.Height = AvgWeekUploadHeight;
            AvgrectWeekUpload.Width = intervalAvg;
            AvgrectWeekUpload.Margin = new Thickness(100 + intervalAvg, 750 - AvgWeekDownloadHeight - AvgWeekUploadHeight, 0, 0);
            WeekUploadAvg.Text = WeekUpload[1];
            WeekUploadAvg.TextAlignment = TextAlignment.Center;
            WeekUploadAvg.Width = intervalAvg;
            WeekUploadAvg.Margin = new Thickness(100 + intervalAvg, 750 - AvgWeekDownloadHeight - AvgWeekUploadHeight - 40, 0, 0);
            AverageCanvas.Children.Add(AvgrectWeekDownload);
            AverageCanvas.Children.Add(AvgrectWeekUpload);
            AverageCanvas.Children.Add(WeekDownloadAvg);
            AverageCanvas.Children.Add(WeekUploadAvg);

            //Average This month
            double AvgMonthDownloadHeight = double.Parse(MonthDownload[1]) / maxOrdinateAvg * 650;
            Rectangle AvgrectMonthDownload = new Rectangle();
            AvgrectMonthDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            AvgrectMonthDownload.Height = AvgMonthDownloadHeight;
            AvgrectMonthDownload.Width = intervalAvg;
            AvgrectMonthDownload.Margin = new Thickness(100 + intervalAvg * 3, 750 - AvgMonthDownloadHeight, 0, 0);
            MonthDownloadAvg.Text = MonthDownload[1];
            MonthDownloadAvg.TextAlignment = TextAlignment.Center;
            MonthDownloadAvg.Width = intervalAvg;
            MonthDownloadAvg.Margin = new Thickness(100 + intervalAvg * 3, 750 - AvgMonthDownloadHeight / 2 - 20, 0, 0);
            double AvgMonthUploadHeight = double.Parse(MonthUpload[1]) / maxOrdinateAvg * 650;
            Rectangle AvgrectMonthUpload = new Rectangle();
            AvgrectMonthUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            AvgrectMonthUpload.Height = AvgMonthUploadHeight;
            AvgrectMonthUpload.Width = intervalAvg;
            AvgrectMonthUpload.Margin = new Thickness(100 + intervalAvg * 3, 750 - AvgMonthDownloadHeight - AvgMonthUploadHeight, 0, 0);
            MonthUploadAvg.Text = MonthUpload[1];
            MonthUploadAvg.TextAlignment = TextAlignment.Center;
            MonthUploadAvg.Width = intervalAvg;
            MonthUploadAvg.Margin = new Thickness(100 + intervalAvg * 3, 750 - AvgMonthDownloadHeight - AvgMonthUploadHeight - 40, 0, 0);
            AverageCanvas.Children.Add(AvgrectMonthDownload);
            AverageCanvas.Children.Add(AvgrectMonthUpload);
            AverageCanvas.Children.Add(MonthDownloadAvg);
            AverageCanvas.Children.Add(MonthUploadAvg);

            //Average Last month
            double AvgLastMonthDownloadHeight = double.Parse(LastMonthDownload[1]) / maxOrdinateAvg * 650;
            Rectangle AvgrectLastMonthDownload = new Rectangle();
            AvgrectLastMonthDownload.Fill = new SolidColorBrush(Color.FromArgb(255, 128, 100, 162));
            AvgrectLastMonthDownload.Height = AvgLastMonthDownloadHeight;
            AvgrectLastMonthDownload.Width = intervalAvg;
            AvgrectLastMonthDownload.Margin = new Thickness(100 + intervalAvg * 5, 750 - AvgLastMonthDownloadHeight, 0, 0);
            LastMonthDownloadAvg.Text = LastMonthDownload[1];
            LastMonthDownloadAvg.TextAlignment = TextAlignment.Center;
            LastMonthDownloadAvg.Width = intervalAvg;
            LastMonthDownloadAvg.Margin = new Thickness(100 + intervalAvg * 5, 750 - AvgLastMonthDownloadHeight / 2 - 20, 0, 0);
            double AvgLastMonthUploadHeight = double.Parse(LastMonthUpload[1]) / maxOrdinateAvg * 650;
            Rectangle AvgrectLastMonthUpload = new Rectangle();
            AvgrectLastMonthUpload.Fill = new SolidColorBrush(Color.FromArgb(255, 195, 182, 211));
            AvgrectLastMonthUpload.Height = AvgLastMonthUploadHeight;
            AvgrectLastMonthUpload.Width = intervalAvg;
            AvgrectLastMonthUpload.Margin = new Thickness(100 + intervalAvg * 5, 750 - AvgLastMonthDownloadHeight - AvgLastMonthUploadHeight, 0, 0);
            LastMonthUploadAvg.Text = LastMonthUpload[1];
            LastMonthUploadAvg.TextAlignment = TextAlignment.Center;
            LastMonthUploadAvg.Width = intervalAvg;
            LastMonthUploadAvg.Margin = new Thickness(100 + intervalAvg * 5, 750 - AvgLastMonthDownloadHeight - AvgLastMonthUploadHeight - 40, 0, 0);
            AverageCanvas.Children.Add(AvgrectLastMonthDownload);
            AverageCanvas.Children.Add(AvgrectLastMonthUpload);
            AverageCanvas.Children.Add(LastMonthDownloadAvg);
            AverageCanvas.Children.Add(LastMonthUploadAvg);

            //颜色含义指示绘制
            InstructionTotal.Margin = new Thickness(width - 350, 820, 0, 0);
            TotalCanvas.Children.Add(InstructionTotal);
            InstructionAvg.Margin = new Thickness(width - 350, 820, 0, 0);
            AverageCanvas.Children.Add(InstructionAvg);

            InProgress.IsActive = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private async void checkTrafficMeter_Click(Object sender, RoutedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "TrafficMeterPage";
            } 
            else
            {
                if (checkTrafficMeter.IsChecked == true)
                {
                    // Create the message dialog and set its content
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    var strtext = loader.GetString("traffic_enable");
                    var messageDialog = new MessageDialog(strtext);

                    // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                    messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.CommandInvokedHandlerYes)));
                    messageDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(this.CommandInvokedHandlerNo)));

                    // Set the command that will be invoked by default
                    messageDialog.DefaultCommandIndex = 0;

                    // Set the command to be invoked when escape is pressed
                    messageDialog.CancelCommandIndex = 1;

                    // Show the message dialog
                    await messageDialog.ShowAsync();
                }
                else if (checkTrafficMeter.IsChecked == false)
                {
                    // Create the message dialog and set its content
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    var strtext = loader.GetString("traffic_disable");
                    var messageDialog = new MessageDialog(strtext);

                    // Add commands and set their callbacks; both buttons use the same callback function instead of inline event handlers
                    messageDialog.Commands.Add(new UICommand("Yes", new UICommandInvokedHandler(this.CommandInvokedHandlerYes)));
                    messageDialog.Commands.Add(new UICommand("No", new UICommandInvokedHandler(this.CommandInvokedHandlerNo)));

                    // Set the command that will be invoked by default
                    messageDialog.DefaultCommandIndex = 0;

                    // Set the command to be invoked when escape is pressed
                    messageDialog.CancelCommandIndex = 1;

                    // Show the message dialog
                    await messageDialog.ShowAsync();
                }
            }
        }

        #region Commands
        /// <summary>
        /// Callback function for the invocation of the dialog commands.
        /// </summary>
        /// <param name="command">The command that was invoked.</param>
        private async void CommandInvokedHandlerYes(IUICommand command)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "TrafficMeterPage";
            } 
            else
            {
                InProgress.IsActive = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                GenieSoapApi soapApi = new GenieSoapApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                string trafficMeterEnable;
                if (checkTrafficMeter.IsChecked == true)
                {
                    trafficMeterEnable = "1";
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.EnableTrafficMeter(trafficMeterEnable);
                    }
                    TrafficMeterList.Visibility = Visibility.Visible;
                    TotalCanvas.Visibility = Visibility.Visible;
                    AverageCanvas.Visibility = Visibility.Visible;
                }
                else if (checkTrafficMeter.IsChecked == false)
                {
                    trafficMeterEnable = "0";
                    while (dicResponse == null || dicResponse.Count == 0)
                    {
                        dicResponse = await soapApi.EnableTrafficMeter(trafficMeterEnable);
                    }
                    TrafficMeterList.Visibility = Visibility.Collapsed;
                    TotalCanvas.Visibility = Visibility.Collapsed;
                    AverageCanvas.Visibility = Visibility.Collapsed;
                }
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
        }

        private void CommandInvokedHandlerNo(IUICommand command)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "TrafficMeterPage";
            } 
            else
            {
                if (checkTrafficMeter.IsChecked == true)
                {
                    checkTrafficMeter.IsChecked = false;
                }
                else if (checkTrafficMeter.IsChecked == false)
                {
                    checkTrafficMeter.IsChecked = true;
                }
            }
        }
        #endregion

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

        private async void Refresh_Click(Object sender, RoutedEventArgs e)
        {
            if (IsWifiSsidChanged)
            {
                this.Frame.Navigate(typeof(LoginPage));
                MainPageInfo.navigatedPage = "TrafficMeterPage";
            } 
            else
            {
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.IsActive = true;
                GenieSoapApi soapApi = new GenieSoapApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                while (dicResponse == null || dicResponse.Count == 0)
                {
                    dicResponse = await soapApi.GetTrafficMeterEnabled();
                }
                if (dicResponse.Count > 0)
                {
                    TrafficMeterInfoModel.isTrafficMeterEnabled = dicResponse["NewTrafficMeterEnable"];
                    if (TrafficMeterInfoModel.isTrafficMeterEnabled == "0")
                    {
                        checkTrafficMeter.IsChecked = false;
                        //TrafficMeterLongListSelector.Visibility = Visibility.Collapsed;
                        TrafficMeterList.Visibility = Visibility.Collapsed;
                        TotalCanvas.Visibility = Visibility.Collapsed;
                        AverageCanvas.Visibility = Visibility.Collapsed;
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        InProgress.IsActive = false;
                    }
                    else if (TrafficMeterInfoModel.isTrafficMeterEnabled == "1")
                    {
                        checkTrafficMeter.IsChecked = true;
                        //TrafficMeterLongListSelector.Visibility = Visibility.Visible;
                        TrafficMeterList.Visibility = Visibility.Visible;
                        TotalCanvas.Visibility = Visibility.Visible;
                        AverageCanvas.Visibility = Visibility.Visible;
                        Dictionary<string, string> dicResponse1 = new Dictionary<string, string>();
                        while (dicResponse1 == null || dicResponse1.Count == 0)
                        {
                            dicResponse1 = await soapApi.GetTrafficMeterOptions();
                        }
                        if (dicResponse1.Count > 0)
                        {
                            TrafficMeterInfoModel.MonthlyLimit = dicResponse1["NewMonthlyLimit"];
                            TrafficMeterInfoModel.changedMonthlyLimit = dicResponse1["NewMonthlyLimit"];
                            TrafficMeterInfoModel.RestartHour = dicResponse1["RestartHour"];
                            TrafficMeterInfoModel.changedRestartHour = dicResponse1["RestartHour"];
                            TrafficMeterInfoModel.RestartMinute = dicResponse1["RestartMinute"];
                            TrafficMeterInfoModel.changedRestartMinute = dicResponse1["RestartMinute"];
                            TrafficMeterInfoModel.RestartDay = dicResponse1["RestartDay"];
                            TrafficMeterInfoModel.changedRestartDay = dicResponse1["RestartDay"];
                            TrafficMeterInfoModel.ControlOption = dicResponse1["NewControlOption"];
                            TrafficMeterInfoModel.changedControlOption = dicResponse1["NewControlOption"];

                            Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                            while (dicResponse2 == null || dicResponse2.Count == 0)
                            {
                                dicResponse2 = await soapApi.GetTrafficMeterStatistics();
                            }
                            if (dicResponse2.Count > 0)
                            {
                                TrafficMeterInfoModel.TodayUpload = dicResponse2["NewTodayUpload"];
                                TrafficMeterInfoModel.TodayDownload = dicResponse2["NewTodayDownload"];
                                TrafficMeterInfoModel.YesterdayUpload = dicResponse2["NewYesterdayUpload"];
                                TrafficMeterInfoModel.YesterdayDownload = dicResponse2["NewYesterdayDownload"];
                                TrafficMeterInfoModel.WeekUpload = dicResponse2["NewWeekUpload"];
                                TrafficMeterInfoModel.WeekDownload = dicResponse2["NewWeekDownload"];
                                TrafficMeterInfoModel.MonthUpload = dicResponse2["NewMonthUpload"];
                                TrafficMeterInfoModel.MonthDownload = dicResponse2["NewMonthDownload"];
                                TrafficMeterInfoModel.LastMonthUpload = dicResponse2["NewLastMonthUpload"];
                                TrafficMeterInfoModel.LastMonthDownload = dicResponse2["NewLastMonthDownload"];

                                InProgress.IsActive = false;
                                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                                PopupBackground.Visibility = Visibility.Collapsed;
                                this.Frame.Navigate(typeof(TrafficMeterPage));
                            }
                            else
                            {
                                InProgress.IsActive = false;
                                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                                PopupBackground.Visibility = Visibility.Collapsed;
                                var messageDialog = new MessageDialog("GetTrafficMeterStatistics failed!");
                                await messageDialog.ShowAsync();
                            }
                        }
                        else
                        {
                            InProgress.IsActive = false;
                            PopupBackgroundTop.Visibility = Visibility.Collapsed;
                            PopupBackground.Visibility = Visibility.Collapsed;
                            var messageDialog = new MessageDialog("GetGuestAccessNetworkInfo failed!");
                            await messageDialog.ShowAsync();
                        }
                    }
                    else if (TrafficMeterInfoModel.isTrafficMeterEnabled == "2")
                    {
                        InProgress.IsActive = false;
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                        var messageDialog = new MessageDialog(loader.GetString("notsupport"));
                        await messageDialog.ShowAsync();
                        this.GoHome(null, null);
                    }
                }
                else
                {
                    InProgress.IsActive = false;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    var messageDialog = new MessageDialog("GetGuestAccessEnabled failed!");
                    await messageDialog.ShowAsync();
                }
            }
        }
    }
}
