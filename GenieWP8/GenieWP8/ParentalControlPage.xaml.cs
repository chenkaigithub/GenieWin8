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
using System.Windows.Input;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;
using System.Windows.Media;

namespace GenieWP8
{
    public partial class ParentalControlPage : PhoneApplicationPage
    {
        private static ParentalControlModel settingModel = null;
        public ParentalControlPage()
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

            // 将 LongListSelector 控件的数据上下文设置为绑定数据
            if (settingModel == null)
                settingModel = new ParentalControlModel();
            DataContext = settingModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();

            if ((this.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                RegisterScrollViewer.Height = 440;
                FilterLevelScrollViewer.Height = 440;
                SettingCompleteScrollViewer.Height = 400;
                CategoriesList.Height = 350;
            }
            else
            {
                RegisterScrollViewer.Height = 220;
                FilterLevelScrollViewer.Height = 220;
                SettingCompleteScrollViewer.Height = 220;
                CategoriesList.Height = 180;
            }
        }

        // 为 ParentalControlModel 项加载数据
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            if (!EnquirePopup.IsOpen && !RegisterPopup.IsOpen && !LoginPopup.IsOpen && !FilterLevelPopup.IsOpen && !CategoriesPopup.IsOpen && !SettingCompletePopup.IsOpen)
            {
                ParentalControlInfo.SavedInfo = await ReadSavedInfoFromFile();
                string[] SavedInfo = ParentalControlInfo.SavedInfo.Split(';');
                if (SavedInfo[0] == ParentalControlInfo.RouterMacaddr)
                {
                    ParentalControlInfo.IsOpenDNSLoggedIn = true;
                    GenieWebApi webApi = new GenieWebApi();
                    ParentalControlInfo.token = SavedInfo[1];
                    ParentalControlInfo.DeviceId = SavedInfo[2];
                    Dictionary<string, string> dicResponse1 = new Dictionary<string, string>();
                    dicResponse1 = await webApi.GetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId);
                    if (dicResponse1["status"] != "success")
                    {
                        ParentalControlInfo.filterLevel = "";
                    }
                    else
                    {
                        ParentalControlInfo.filterLevel = dicResponse1["bundle"];
                    }
                    ParentalControlInfo.categories = dicResponse1["categories"];
                    ParentalControlInfo.Username = SavedInfo[3];

                    ParentalControlInfo.BypassChildrenDeviceId = await ReadBypassChildrenDeviceIdFromFile();                      //读取本地保存的DeviceId，如果不为空则获得当前登录的Bypass账户
                    if (ParentalControlInfo.BypassChildrenDeviceId != null && ParentalControlInfo.BypassChildrenDeviceId != "")
                    {
                        ParentalControlInfo.IsBypassUserLoggedIn = true;
                        GenieWebApi webApi1 = new GenieWebApi();
                        Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                        dicResponse2 = await webApi1.GetUserForChildDeviceId(ParentalControlInfo.BypassChildrenDeviceId);
                        if (dicResponse2["status"] == "success")
                        {
                            ParentalControlInfo.BypassUsername = dicResponse2["bypass_user"];
                        }
                        else
                        {
                            MessageBox.Show(dicResponse2["error_message"]);
                        }
                    }
                    else
                    {
                        ParentalControlInfo.IsBypassUserLoggedIn = false;
                    }
                }
                else
                {
                    ParentalControlInfo.IsOpenDNSLoggedIn = false;
                    ParentalControlInfo.IsBypassUserLoggedIn = false;
                    IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
                    if (fileStorage.FileExists("Bypass_childrenDeviceId.txt"))
                    {
                        fileStorage.DeleteFile("Bypass_childrenDeviceId.txt");
                    }
                }  
            }

            //settingModel.FilterLevelGroups.Clear();
            settingModel.LoadData();           

            if (!ParentalControlInfo.IsOpenDNSLoggedIn)	//未登录OpenDNS账户
            {
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
                if (!EnquirePopup.IsOpen && !RegisterPopup.IsOpen && !LoginPopup.IsOpen)
                {
                    EnquirePopup.IsOpen = true;                    
                    appBarButton_refresh.IsEnabled = false;
                }
            }
            else
            {
                InProgress.Visibility = Visibility.Visible;
                pleasewait.Visibility = Visibility.Visible;
                if (ParentalControlInfo.isParentalControlEnabled == "0")
                {
                    checkPatentalControl.IsChecked = false;
                    ParentalControlPanel.Visibility = Visibility.Collapsed;
                    //FilterLevelLongListSelector.Visibility = Visibility.Collapsed;
                    //ChangeCustomSettings.Visibility = Visibility.Collapsed;
                    //stpOpenDNSAccount.Visibility = Visibility.Collapsed;
                    //BypassAccount.Visibility = Visibility.Collapsed;
                }
                else if (ParentalControlInfo.isParentalControlEnabled == "1")
                {
                    checkPatentalControl.IsChecked = true;
                    ParentalControlPanel.Visibility = Visibility.Visible;
                    //FilterLevelLongListSelector.Visibility = Visibility.Visible;
                    //ChangeCustomSettings.Visibility = Visibility.Visible;
                    //stpOpenDNSAccount.Visibility = Visibility.Visible;
                    //BypassAccount.Visibility = Visibility.Visible;
                }
                tbFilterlevel.Text = ParentalControlInfo.filterLevel;

                if (ParentalControlInfo.Username != null)
                {
                    OpenDNSUserName.Text = ParentalControlInfo.Username;
                }
                else
                {
                    OpenDNSUserName.Text = "";
                }

                if (ParentalControlInfo.IsBypassUserLoggedIn && ParentalControlInfo.BypassUsername != null)
                {
                    bypassaccount.Text = ParentalControlInfo.BypassUsername;
                }
                else
                {
                    bypassaccount.Text = "";
                }                
                appBarButton_refresh.IsEnabled = true;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }           
        }

        //checkbox控件响应事件
        private async void checkParentalControl_Click(Object sender, RoutedEventArgs e)
        {
            
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            string parentalControlEnable;
            if (checkPatentalControl.IsChecked == true)
            {
                parentalControlEnable = "1";
                dicResponse = await soapApi.EnableParentalControl(parentalControlEnable);
                ParentalControlInfo.isParentalControlEnabled = "1";
                ParentalControlPanel.Visibility = Visibility.Visible;
                //FilterLevelLongListSelector.Visibility = Visibility.Visible;
                //ChangeCustomSettings.Visibility = Visibility.Visible;
                //stpOpenDNSAccount.Visibility = Visibility.Visible;
                //BypassAccount.Visibility = Visibility.Visible;
            }
            else if (checkPatentalControl.IsChecked == false)
            {
                parentalControlEnable = "0";
                dicResponse = await soapApi.EnableParentalControl(parentalControlEnable);
                ParentalControlInfo.isParentalControlEnabled = "0";
                ParentalControlPanel.Visibility = Visibility.Collapsed;
                //FilterLevelLongListSelector.Visibility = Visibility.Collapsed;
                //ChangeCustomSettings.Visibility = Visibility.Collapsed;
                //stpOpenDNSAccount.Visibility = Visibility.Collapsed;
                //BypassAccount.Visibility = Visibility.Collapsed;
            }
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        //// 处理在 LongListSelector 中更改的选定内容
        //private void FilterLevelLongListSelector_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        //{
        //    // 如果所选项为空(没有选定内容)，则不执行任何操作
        //    if (FilterLevelLongListSelector.SelectedItem == null)
        //        return;

        //    //PopupFilterLevel __popupFilterLevel = new PopupFilterLevel();
        //    if (!FilterLevelPopup.IsOpen)
        //    {
        //        switch (ParentalControlInfo.filterLevel)
        //        {
        //            case "None":
        //                radioButton_None.IsChecked = true;
        //                break;
        //            case "Minimal":
        //                radioButton_Minimum.IsChecked = true;
        //                break;
        //            case "Low":
        //                radioButton_Low.IsChecked = true;
        //                break;
        //            case "Moderate":
        //                radioButton_Medium.IsChecked = true;
        //                break;
        //            case "High":
        //                radioButton_High.IsChecked = true;
        //                break;
        //            case "Custom":
        //                radioButton_Custom.IsChecked = true;
        //                string[] category = ParentalControlInfo.categories.Split(',');
        //                for (int i = 0; i < category.Length; i++)
        //                {
        //                    switch (category[i])
        //                    {
        //                        case "Academic Fraud":
        //                            category_1.IsChecked = true;
        //                            break;
        //                        case "Adult Themes":
        //                            category_2.IsChecked = true;
        //                            break;
        //                        case "Adware":
        //                            category_3.IsChecked = true;
        //                            break;
        //                        case "Alcohol":
        //                            category_4.IsChecked = true;
        //                            break;
        //                        case "Anime/Manga/Webcomic":
        //                            category_5.IsChecked = true;
        //                            break;
        //                        case "Auctions":
        //                            category_6.IsChecked = true;
        //                            break;
        //                        case "Automotive":
        //                            category_7.IsChecked = true;
        //                            break;
        //                        case "Blogs":
        //                            category_8.IsChecked = true;
        //                            break;
        //                        case "Business Services":
        //                            category_9.IsChecked = true;
        //                            break;
        //                        case "Chat":
        //                            category_10.IsChecked = true;
        //                            break;
        //                        case "Classifieds":
        //                            category_11.IsChecked = true;
        //                            break;
        //                        case "Dating":
        //                            category_12.IsChecked = true;
        //                            break;
        //                        case "Drugs":
        //                            category_13.IsChecked = true;
        //                            break;
        //                        case "Ecommerce/Shopping":
        //                            category_14.IsChecked = true;
        //                            break;
        //                        case "Educational Institutions":
        //                            category_15.IsChecked = true;
        //                            break;
        //                        case "File Storage":
        //                            category_16.IsChecked = true;
        //                            break;
        //                        case "Financial Institutions":
        //                            category_17.IsChecked = true;
        //                            break;
        //                        case "Forums/Message boards":
        //                            category_18.IsChecked = true;
        //                            break;
        //                        case "Gambling":
        //                            category_19.IsChecked = true;
        //                            break;
        //                        case "Games":
        //                            category_20.IsChecked = true;
        //                            break;
        //                        case "German Youth Protection":
        //                            category_21.IsChecked = true;
        //                            break;
        //                        case "Government":
        //                            category_22.IsChecked = true;
        //                            break;
        //                        case "Hate/Discrimination":
        //                            category_23.IsChecked = true;
        //                            break;
        //                        case "Health and Fitness":
        //                            category_24.IsChecked = true;
        //                            break;
        //                        case "Humor":
        //                            category_25.IsChecked = true;
        //                            break;
        //                        case "Instant Messaging":
        //                            category_26.IsChecked = true;
        //                            break;
        //                        case "Jobs/Employment":
        //                            category_27.IsChecked = true;
        //                            break;
        //                        case "Lingerie/Bikini":
        //                            category_28.IsChecked = true;
        //                            break;
        //                        case "Movies":
        //                            category_29.IsChecked = true;
        //                            break;
        //                        case "Music":
        //                            category_30.IsChecked = true;
        //                            break;
        //                        case "News/Media":
        //                            category_31.IsChecked = true;
        //                            break;
        //                        case "Non-Profits":
        //                            category_32.IsChecked = true;
        //                            break;
        //                        case "Nudity":
        //                            category_33.IsChecked = true;
        //                            break;
        //                        case "P2P/File sharing":
        //                            category_34.IsChecked = true;
        //                            break;
        //                        case "Parked Domains":
        //                            category_35.IsChecked = true;
        //                            break;
        //                        case "Photo Sharing":
        //                            category_36.IsChecked = true;
        //                            break;
        //                        case "Podcasts":
        //                            category_37.IsChecked = true;
        //                            break;
        //                        case "Politics":
        //                            category_38.IsChecked = true;
        //                            break;
        //                        case "Pornography":
        //                            category_39.IsChecked = true;
        //                            break;
        //                        case "Portals":
        //                            category_40.IsChecked = true;
        //                            break;
        //                        case "Proxy/Anonymizer":
        //                            category_41.IsChecked = true;
        //                            break;
        //                        case "Radio":
        //                            category_42.IsChecked = true;
        //                            break;
        //                        case "Religious":
        //                            category_43.IsChecked = true;
        //                            break;
        //                        case "Research/Reference":
        //                            category_44.IsChecked = true;
        //                            break;
        //                        case "Search Engines":
        //                            category_45.IsChecked = true;
        //                            break;
        //                        case "Sexuality":
        //                            category_46.IsChecked = true;
        //                            break;
        //                        case "Social Networking":
        //                            category_47.IsChecked = true;
        //                            break;
        //                        case "Software/Technology":
        //                            category_48.IsChecked = true;
        //                            break;
        //                        case "Sports":
        //                            category_49.IsChecked = true;
        //                            break;
        //                        case "Tasteless":
        //                            category_50.IsChecked = true;
        //                            break;
        //                        case "Television":
        //                            category_51.IsChecked = true;
        //                            break;
        //                        case "Tobacco":
        //                            category_52.IsChecked = true;
        //                            break;
        //                        case "Travel":
        //                            category_53.IsChecked = true;
        //                            break;
        //                        case "Typo Squatting":
        //                            category_54.IsChecked = true;
        //                            break;
        //                        case "Video Sharing":
        //                            category_55.IsChecked = true;
        //                            break;
        //                        case "Visual Search Engines":
        //                            category_56.IsChecked = true;
        //                            break;
        //                        case "Weapons":
        //                            category_57.IsChecked = true;
        //                            break;
        //                        case "Web Spam":
        //                            category_58.IsChecked = true;
        //                            break;
        //                        case "Webmail":
        //                            category_59.IsChecked = true;
        //                            break;
        //                        case "Phishing Protection":
        //                            category_60.IsChecked = true;
        //                            break;
        //                    }   //switch (category[i])
        //                }   //for
        //                break;
        //            default:
        //                break;
        //        }
        //        FilterLevelPopup.IsOpen = true;
        //        PopupBackgroundTop.Visibility = Visibility.Visible;
        //        PopupBackground.Visibility = Visibility.Visible;
        //        InProgress.Visibility = Visibility.Collapsed;
        //        pleasewait.Visibility = Visibility.Collapsed;
        //        appBarButton_refresh.IsEnabled = false;
        //    }

        //    // 将所选项重置为 null (没有选定内容)
        //    FilterLevelLongListSelector.SelectedItem = null;
        //}

        // 处理 ChangeCustomSettings 触摸输入事件
        private async void ChangeSetting_Click(Object sender, MouseButtonEventArgs e)
        {
            gridChangeSetting.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.AccountRelay(ParentalControlInfo.token);
            if (dicResponse["status"] == "success")
            {
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                string relay_token = dicResponse["relay_token"];
                var uri = new Uri("http://netgear.opendns.com/account.php?device_id=" + ParentalControlInfo.DeviceId + "&api_key=3D8C85A77ADA886B967984DF1F8B3711" + "&relay_token=" + relay_token);
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
            else
            {
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                MessageBox.Show(dicResponse["error_message"]);
            }
        }

        // 处理 BypassAccount 触摸输入事件
        private async void Bypass_Click(Object sender, MouseButtonEventArgs e)
        {
            gridBypass.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            if (ParentalControlInfo.IsBypassUserLoggedIn == false)                        //未登录Bypass账户
            {
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Visible;
                pleasewait.Visibility = Visibility.Visible;
                ParentalControlInfo.BypassAccounts = "";
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.GetUsersForDeviceId(ParentalControlInfo.DeviceId);
                if (dicResponse["status"] == "success")
                {
                    var jarry = JArray.Parse(dicResponse["bypassUsers"]);
                    if (jarry != null)
                    {
                        for (int i = 0; i < jarry.Count; i++)
                        {
                            string user = jarry[i].ToString();
                            ParentalControlInfo.BypassAccounts = ParentalControlInfo.BypassAccounts + user + ";";
                        }
                    }
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    NavigationService.Navigate(new Uri("/BypassAccountPage.xaml", UriKind.Relative));
                }
                else
                {
                    InProgress.Visibility = Visibility.Collapsed;
                    pleasewait.Visibility = Visibility.Collapsed;
                    if (dicResponse["error"] == "4012")
                    {
                        PopupEnquireBypassAccount.IsOpen = true;
                    }
                    else
                    {
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        MessageBox.Show(dicResponse["error_message"]);
                    }                   
                }
            }
            else                                                                        //已登录Bypass账户
            {
                NavigationService.Navigate(new Uri("/BypassAccountLogoutPage.xaml", UriKind.Relative));
            }
        }

        private void CancelButton_Click(Object sender, RoutedEventArgs e)
        {
            if (PopupEnquireBypassAccount.IsOpen)
            {
                PopupEnquireBypassAccount.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
        }

        private async void OkButton_Click(Object sender, RoutedEventArgs e)
        {
            if (PopupEnquireBypassAccount.IsOpen)
            {
                PopupEnquireBypassAccount.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                var uri = new Uri("http://netgear.opendns.com/sign_in.php?");
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
        }

        //用于生成本地化 ApplicationBar 的代码
        ApplicationBarIconButton appBarButton_refresh = new ApplicationBarIconButton(new Uri("Assets/refresh.png", UriKind.Relative));
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
            //ApplicationBarIconButton appBarButton_refresh = new ApplicationBarIconButton(new Uri("Assets/refresh.png", UriKind.Relative));
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

        //刷新按钮响应事件
        private async void appBarButton_refresh_Click(object sender, EventArgs e)
        {
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;

            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            while (dicResponse == null || dicResponse.Count == 0 || int.Parse(dicResponse["ResponseCode"]) != 0)
            {
                dicResponse = await soapApi.GetEnableStatus();
            }           
            ParentalControlInfo.isParentalControlEnabled = dicResponse["ParentalControl"];

            //获取过滤等级
            GenieWebApi webApi = new GenieWebApi();
            dicResponse = await webApi.GetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId);
            if (dicResponse["status"] != "success")
            {
                ParentalControlInfo.filterLevel = "";
            }
            else
            {
                ParentalControlInfo.filterLevel = dicResponse["bundle"];
            }
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
                case "Custom":
                    radioButton_Custom.IsChecked = true;
                    ParentalControlInfo.categories = dicResponse["categories"];
                    string[] category = ParentalControlInfo.categories.Split(',');
                    for (int i = 0; i < category.Length; i++)
                    {
                        switch (category[i])
                        {
                            case "Academic Fraud":
                                category_1.IsChecked = true;
                                break;
                            case "Adult Themes":
                                category_2.IsChecked = true;
                                break;
                            case "Adware":
                                category_3.IsChecked = true;
                                break;
                            case "Alcohol":
                                category_4.IsChecked = true;
                                break;
                            case "Anime/Manga/Webcomic":
                                category_5.IsChecked = true;
                                break;
                            case "Auctions":
                                category_6.IsChecked = true;
                                break;
                            case "Automotive":
                                category_7.IsChecked = true;
                                break;
                            case "Blogs":
                                category_8.IsChecked = true;
                                break;
                            case "Business Services":
                                category_9.IsChecked = true;
                                break;
                            case "Chat":
                                category_10.IsChecked = true;
                                break;
                            case "Classifieds":
                                category_11.IsChecked = true;
                                break;
                            case "Dating":
                                category_12.IsChecked = true;
                                break;
                            case "Drugs":
                                category_13.IsChecked = true;
                                break;
                            case "Ecommerce/Shopping":
                                category_14.IsChecked = true;
                                break;
                            case "Educational Institutions":
                                category_15.IsChecked = true;
                                break;
                            case "File Storage":
                                category_16.IsChecked = true;
                                break;
                            case "Financial Institutions":
                                category_17.IsChecked = true;
                                break;
                            case "Forums/Message boards":
                                category_18.IsChecked = true;
                                break;
                            case "Gambling":
                                category_19.IsChecked = true;
                                break;
                            case "Games":
                                category_20.IsChecked = true;
                                break;
                            case "German Youth Protection":
                                category_21.IsChecked = true;
                                break;
                            case "Government":
                                category_22.IsChecked = true;
                                break;
                            case "Hate/Discrimination":
                                category_23.IsChecked = true;
                                break;
                            case "Health and Fitness":
                                category_24.IsChecked = true;
                                break;
                            case "Humor":
                                category_25.IsChecked = true;
                                break;
                            case "Instant Messaging":
                                category_26.IsChecked = true;
                                break;
                            case "Jobs/Employment":
                                category_27.IsChecked = true;
                                break;
                            case "Lingerie/Bikini":
                                category_28.IsChecked = true;
                                break;
                            case "Movies":
                                category_29.IsChecked = true;
                                break;
                            case "Music":
                                category_30.IsChecked = true;
                                break;
                            case "News/Media":
                                category_31.IsChecked = true;
                                break;
                            case "Non-Profits":
                                category_32.IsChecked = true;
                                break;
                            case "Nudity":
                                category_33.IsChecked = true;
                                break;
                            case "P2P/File sharing":
                                category_34.IsChecked = true;
                                break;
                            case "Parked Domains":
                                category_35.IsChecked = true;
                                break;
                            case "Photo Sharing":
                                category_36.IsChecked = true;
                                break;
                            case "Podcasts":
                                category_37.IsChecked = true;
                                break;
                            case "Politics":
                                category_38.IsChecked = true;
                                break;
                            case "Pornography":
                                category_39.IsChecked = true;
                                break;
                            case "Portals":
                                category_40.IsChecked = true;
                                break;
                            case "Proxy/Anonymizer":
                                category_41.IsChecked = true;
                                break;
                            case "Radio":
                                category_42.IsChecked = true;
                                break;
                            case "Religious":
                                category_43.IsChecked = true;
                                break;
                            case "Research/Reference":
                                category_44.IsChecked = true;
                                break;
                            case "Search Engines":
                                category_45.IsChecked = true;
                                break;
                            case "Sexuality":
                                category_46.IsChecked = true;
                                break;
                            case "Social Networking":
                                category_47.IsChecked = true;
                                break;
                            case "Software/Technology":
                                category_48.IsChecked = true;
                                break;
                            case "Sports":
                                category_49.IsChecked = true;
                                break;
                            case "Tasteless":
                                category_50.IsChecked = true;
                                break;
                            case "Television":
                                category_51.IsChecked = true;
                                break;
                            case "Tobacco":
                                category_52.IsChecked = true;
                                break;
                            case "Travel":
                                category_53.IsChecked = true;
                                break;
                            case "Typo Squatting":
                                category_54.IsChecked = true;
                                break;
                            case "Video Sharing":
                                category_55.IsChecked = true;
                                break;
                            case "Visual Search Engines":
                                category_56.IsChecked = true;
                                break;
                            case "Weapons":
                                category_57.IsChecked = true;
                                break;
                            case "Web Spam":
                                category_58.IsChecked = true;
                                break;
                            case "Webmail":
                                category_59.IsChecked = true;
                                break;
                            case "Phishing Protection":
                                category_60.IsChecked = true;
                                break;
                        }   //switch (category[i])
                    }   //for
                    break;
                default:
                    break;
            }   //switch (ParentalControlInfo.filterLevel)
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;            
            OnNavigatedTo(null);
        }

        //横竖屏切换响应事件
        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                PageTitle.Width = Application.Current.Host.Content.ActualWidth - 20;
                RegisterScrollViewer.Height = 440;
                FilterLevelScrollViewer.Height = 440;
                SettingCompleteScrollViewer.Height = 400;
                CategoriesList.Height = 350;
            }
            else
            {
                PageTitle.Width = Application.Current.Host.Content.ActualHeight - 150;
                RegisterScrollViewer.Height = 220;
                FilterLevelScrollViewer.Height = 220;
                SettingCompleteScrollViewer.Height = 220;
                CategoriesList.Height = 180;
            }
        }

        //没有OpenDNS账号
        private void NoButton_Click(Object sender, RoutedEventArgs e)
        {
            if (EnquirePopup.IsOpen)
            {
                EnquirePopup.IsOpen = false;
                RegisterPopup.IsOpen = true;
                IsAvailableName.Visibility = Visibility.Collapsed;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
                appBarButton_refresh.IsEnabled = false;
            }
        }

        //拥有OpenDNS账号
        private void YesButton_Click(Object sender, RoutedEventArgs e)
        {
            if (EnquirePopup.IsOpen)
            {
                EnquirePopup.IsOpen = false;
                LoginPopup.IsOpen = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
                appBarButton_refresh.IsEnabled = false;
            }
        }

        //注册上一步
        private void RegisterPreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            if (RegisterPopup.IsOpen)
            {
                EnquirePopup.IsOpen = true;
                RegisterPopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
                appBarButton_refresh.IsEnabled = false;
            }
        }

        //注册下一步
        private async void RegisterNextButton_Click(Object sender, RoutedEventArgs e)
        {
            if (ParentalControlInfo.IsUsernameAvailable == false || ParentalControlInfo.IsEmptyUsername == true || ParentalControlInfo.IsEmptyPassword == true
                || ParentalControlInfo.IsDifferentPassword == true || ParentalControlInfo.IsEmptyEmail == true || ParentalControlInfo.IsDifferentEmail == true)
            {
                MessageBox.Show("Failed, please enter the correct information");
            }
            else
            {
                InProgress1.Visibility = Visibility.Visible;
                pleasewait1.Visibility = Visibility.Visible;
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.CreateAccount(ParentalControlInfo.Username, ParentalControlInfo.Password, ParentalControlInfo.Email);
                if (dicResponse["status"] != "success")
                {
                    InProgress1.Visibility = Visibility.Collapsed;
                    pleasewait1.Visibility = Visibility.Collapsed;
                    MessageBox.Show(dicResponse["error_message"]);
                }
                else
                {
                    if (RegisterPopup.IsOpen)
                    {
                        RegisterPopup.IsOpen = false;
                        LoginPopup.IsOpen = true;
                        PopupBackgroundTop.Visibility = Visibility.Visible;
                        PopupBackground.Visibility = Visibility.Visible;
                        InProgress1.Visibility = Visibility.Collapsed;
                        pleasewait1.Visibility = Visibility.Collapsed;
                        appBarButton_refresh.IsEnabled = false;
                    }
                }
            }
        }

        //登陆上一步
        private void LoginPreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LoginPopup.IsOpen)
            {
                EnquirePopup.IsOpen = true;
                LoginPopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
                appBarButton_refresh.IsEnabled = false;
            }
        }

        //登陆下一步
        private async void LoginNextButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LoginUsername.Text == "" || LoginPassword.Password == "")
            {
                MessageBox.Show(AppResources.EmptyUsernameOrPassword);
            }
            else
            {
                InProgress2.Visibility = Visibility.Visible;
                pleasewait2.Visibility = Visibility.Visible;
                //登录OpenDNS账号
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.BeginLogin(LoginUsername.Text, LoginPassword.Password);
                if (dicResponse.Count > 0)
                {
                    if (dicResponse["status"] != "success")
                    {
                        InProgress2.Visibility = Visibility.Collapsed;
                        pleasewait2.Visibility = Visibility.Collapsed;
                        MessageBox.Show(dicResponse["error_message"]);
                    }
                    else
                    {
                        ParentalControlInfo.IsOpenDNSLoggedIn = true;
                        ParentalControlInfo.Username = LoginUsername.Text;
                        ParentalControlInfo.token = dicResponse["token"];
                        //认证路由器
                        GenieSoapApi soapApi = new GenieSoapApi();
                        Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                        while (dicResponse2 == null || dicResponse2.Count == 0)
                        {
                            dicResponse2 = await soapApi.Authenticate(MainPageInfo.username, MainPageInfo.password);
                        }                      
                        if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 0)
                        {
                            Dictionary<string, string> dicResponse3 = new Dictionary<string, string>();
                            dicResponse3 = await soapApi.GetDNSMasqDeviceID("default");
                            if (dicResponse3.Count > 0 && int.Parse(dicResponse3["ResponseCode"]) == 0)
                            {
                                bool bBindSuccessed = false;
                                ParentalControlInfo.DeviceId = dicResponse3["NewDeviceID"];
                                if (ParentalControlInfo.DeviceId == "")                                   //deviceID == empty
                                {
                                    bBindSuccessed = await BindingAccount();
                                }
                                else                                                                      //deviceID != empty
                                {
                                    Dictionary<string, string> dicResponse4 = new Dictionary<string, string>();
                                    dicResponse4 = await webApi.GetLabel(ParentalControlInfo.token, ParentalControlInfo.DeviceId);
                                    if (dicResponse4.Count > 0 && dicResponse4["status"] == "success")
                                    {
                                        bBindSuccessed = await BindingAccount();
                                    }
                                    else if (dicResponse4.Count > 0 && dicResponse4["status"] != "success")
                                    {
                                        if (dicResponse4["error"] == "4003")
                                        {
                                            ParentalControlInfo.DeviceId = "";
                                            bBindSuccessed = await BindingAccount();
                                        }
                                        else if (dicResponse4["error"] == "4001")
                                        {
                                            InProgress2.Visibility = Visibility.Collapsed;
                                            pleasewait2.Visibility = Visibility.Collapsed;
                                            MessageBox.Show(AppResources.AnotherUserRegisters);
                                            bBindSuccessed = false;
                                        }
                                        else
                                        {
                                            bBindSuccessed = await BindingAccount();
                                        }
                                    }
                                }

                                if (bBindSuccessed)                                                     //绑定账号成功后获取过滤等级，跳到过滤等级设置界面
                                {
                                    Dictionary<string, string> dicResponse5 = new Dictionary<string, string>();
                                    dicResponse5 = await webApi.GetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId);
                                    if (dicResponse5["status"] != "success")
                                    {
                                        ParentalControlInfo.filterLevel = "";
                                    }
                                    else
                                    {
                                        ParentalControlInfo.filterLevel = dicResponse5["bundle"];
                                    }
                                    //PopupFilterLevel __popupFilterLevel = new PopupFilterLevel();       //再次初始化 PopupFilterLevel，标识出过滤等级

                                    if (LoginPopup.IsOpen)
                                    {
                                        LoginPopup.IsOpen = false;
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
                                            case "Custom":
                                                radioButton_Custom.IsChecked = true;
                                                ParentalControlInfo.categories = dicResponse5["categories"];
                                                string[] category = ParentalControlInfo.categories.Split(',');
                                                for (int i = 0; i < category.Length; i++)
                                                {
                                                    switch (category[i])
                                                    {
                                                        case "Academic Fraud":
                                                            category_1.IsChecked = true;
                                                            break;
                                                        case "Adult Themes":
                                                            category_2.IsChecked = true;
                                                            break;
                                                        case "Adware":
                                                            category_3.IsChecked = true;
                                                            break;
                                                        case "Alcohol":
                                                            category_4.IsChecked = true;
                                                            break;
                                                        case "Anime/Manga/Webcomic":
                                                            category_5.IsChecked = true;
                                                            break;
                                                        case "Auctions":
                                                            category_6.IsChecked = true;
                                                            break;
                                                        case "Automotive":
                                                            category_7.IsChecked = true;
                                                            break;
                                                        case "Blogs":
                                                            category_8.IsChecked = true;
                                                            break;
                                                        case "Business Services":
                                                            category_9.IsChecked = true;
                                                            break;
                                                        case "Chat":
                                                            category_10.IsChecked = true;
                                                            break;
                                                        case "Classifieds":
                                                            category_11.IsChecked = true;
                                                            break;
                                                        case "Dating":
                                                            category_12.IsChecked = true;
                                                            break;
                                                        case "Drugs":
                                                            category_13.IsChecked = true;
                                                            break;
                                                        case "Ecommerce/Shopping":
                                                            category_14.IsChecked = true;
                                                            break;
                                                        case "Educational Institutions":
                                                            category_15.IsChecked = true;
                                                            break;
                                                        case "File Storage":
                                                            category_16.IsChecked = true;
                                                            break;
                                                        case "Financial Institutions":
                                                            category_17.IsChecked = true;
                                                            break;
                                                        case "Forums/Message boards":
                                                            category_18.IsChecked = true;
                                                            break;
                                                        case "Gambling":
                                                            category_19.IsChecked = true;
                                                            break;
                                                        case "Games":
                                                            category_20.IsChecked = true;
                                                            break;
                                                        case "German Youth Protection":
                                                            category_21.IsChecked = true;
                                                            break;
                                                        case "Government":
                                                            category_22.IsChecked = true;
                                                            break;
                                                        case "Hate/Discrimination":
                                                            category_23.IsChecked = true;
                                                            break;
                                                        case "Health and Fitness":
                                                            category_24.IsChecked = true;
                                                            break;
                                                        case "Humor":
                                                            category_25.IsChecked = true;
                                                            break;
                                                        case "Instant Messaging":
                                                            category_26.IsChecked = true;
                                                            break;
                                                        case "Jobs/Employment":
                                                            category_27.IsChecked = true;
                                                            break;
                                                        case "Lingerie/Bikini":
                                                            category_28.IsChecked = true;
                                                            break;
                                                        case "Movies":
                                                            category_29.IsChecked = true;
                                                            break;
                                                        case "Music":
                                                            category_30.IsChecked = true;
                                                            break;
                                                        case "News/Media":
                                                            category_31.IsChecked = true;
                                                            break;
                                                        case "Non-Profits":
                                                            category_32.IsChecked = true;
                                                            break;
                                                        case "Nudity":
                                                            category_33.IsChecked = true;
                                                            break;
                                                        case "P2P/File sharing":
                                                            category_34.IsChecked = true;
                                                            break;
                                                        case "Parked Domains":
                                                            category_35.IsChecked = true;
                                                            break;
                                                        case "Photo Sharing":
                                                            category_36.IsChecked = true;
                                                            break;
                                                        case "Podcasts":
                                                            category_37.IsChecked = true;
                                                            break;
                                                        case "Politics":
                                                            category_38.IsChecked = true;
                                                            break;
                                                        case "Pornography":
                                                            category_39.IsChecked = true;
                                                            break;
                                                        case "Portals":
                                                            category_40.IsChecked = true;
                                                            break;
                                                        case "Proxy/Anonymizer":
                                                            category_41.IsChecked = true;
                                                            break;
                                                        case "Radio":
                                                            category_42.IsChecked = true;
                                                            break;
                                                        case "Religious":
                                                            category_43.IsChecked = true;
                                                            break;
                                                        case "Research/Reference":
                                                            category_44.IsChecked = true;
                                                            break;
                                                        case "Search Engines":
                                                            category_45.IsChecked = true;
                                                            break;
                                                        case "Sexuality":
                                                            category_46.IsChecked = true;
                                                            break;
                                                        case "Social Networking":
                                                            category_47.IsChecked = true;
                                                            break;
                                                        case "Software/Technology":
                                                            category_48.IsChecked = true;
                                                            break;
                                                        case "Sports":
                                                            category_49.IsChecked = true;
                                                            break;
                                                        case "Tasteless":
                                                            category_50.IsChecked = true;
                                                            break;
                                                        case "Television":
                                                            category_51.IsChecked = true;
                                                            break;
                                                        case "Tobacco":
                                                            category_52.IsChecked = true;
                                                            break;
                                                        case "Travel":
                                                            category_53.IsChecked = true;
                                                            break;
                                                        case "Typo Squatting":
                                                            category_54.IsChecked = true;
                                                            break;
                                                        case "Video Sharing":
                                                            category_55.IsChecked = true;
                                                            break;
                                                        case "Visual Search Engines":
                                                            category_56.IsChecked = true;
                                                            break;
                                                        case "Weapons":
                                                            category_57.IsChecked = true;
                                                            break;
                                                        case "Web Spam":
                                                            category_58.IsChecked = true;
                                                            break;
                                                        case "Webmail":
                                                            category_59.IsChecked = true;
                                                            break;
                                                        case "Phishing Protection":
                                                            category_60.IsChecked = true;
                                                            break;
                                                    }   //switch (category[i])
                                                }   //for
                                                break;
                                            default:
                                                break;
                                        }   //switch (ParentalControlInfo.filterLevel)
                                        FilterLevelPopup.IsOpen = true;
                                        ParentalControlInfo.IsCategoriesChanged = false;
                                        PopupBackgroundTop.Visibility = Visibility.Visible;
                                        PopupBackground.Visibility = Visibility.Visible;
                                        InProgress2.Visibility = Visibility.Collapsed;
                                        pleasewait2.Visibility = Visibility.Collapsed;
                                        appBarButton_refresh.IsEnabled = false;
                                    }
                                }   //if (bBindSuccessed)
                            }
                            else if (dicResponse3.Count > 0 && int.Parse(dicResponse3["ResponseCode"]) == 401)
                            {
                                InProgress2.Visibility = Visibility.Collapsed;
                                pleasewait2.Visibility = Visibility.Collapsed;
                                MessageBox.Show("Not authenticated");
                            }
                        }
                        else if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 401)
                        {
                            InProgress2.Visibility = Visibility.Collapsed;
                            pleasewait2.Visibility = Visibility.Collapsed;
                            MessageBox.Show("Invalid username and/or password");
                        }
                    }
                }
                else
                {
                    InProgress2.Visibility = Visibility.Collapsed;
                    pleasewait2.Visibility = Visibility.Collapsed;
                    MessageBox.Show("Login OpenDNS failed");
                }
            }
        }

        //设置过滤等级上一步
        private void FilterLvPreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            if (FilterLevelPopup.IsOpen)
            {
                LoginPopup.IsOpen = true;
                FilterLevelPopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
                appBarButton_refresh.IsEnabled = false;
            }
        }

        //设置过滤等级下一步
        private async void FilterLvNextButton_Click(Object sender, RoutedEventArgs e)
        {
            InProgress3.Visibility = Visibility.Visible;
            pleasewait3.Visibility = Visibility.Visible;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            if (ParentalControlInfo.IsCategoriesChanged)
            {
                if (ParentalControlInfo.categoriesSelected == string.Empty)
                {
                    ParentalControlInfo.filterLevelSelected = "None";
                    dicResponse = await webApi.SetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId, ParentalControlInfo.filterLevelSelected, ParentalControlInfo.categories);
                } 
                else
                {
                    dicResponse = await webApi.SetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId, ParentalControlInfo.filterLevelSelected, ParentalControlInfo.categoriesSelected);
                }                
            }
            else
            {
                dicResponse = await webApi.SetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId, ParentalControlInfo.filterLevelSelected, ParentalControlInfo.categories);
            }

            if (dicResponse["status"] != "success")
            {
                InProgress3.Visibility = Visibility.Collapsed;
                pleasewait3.Visibility = Visibility.Collapsed;
                MessageBox.Show(dicResponse["error_message"]);
            }
            else
            {
                if (FilterLevelPopup.IsOpen)
                {
                    ParentalControlInfo.filterLevel = ParentalControlInfo.filterLevelSelected;
                    if (ParentalControlInfo.IsCategoriesChanged)
                    {
                        ParentalControlInfo.categories = ParentalControlInfo.categoriesSelected;
                    }
                    ParentalControlInfo.SavedInfo = string.Empty;
                    ParentalControlInfo.SavedInfo = ParentalControlInfo.RouterMacaddr + ";" + ParentalControlInfo.token + ";" + ParentalControlInfo.DeviceId + ";" + ParentalControlInfo.Username;
                    WriteSavedInfoToFile();
                    FilterLevelPopup.IsOpen = false;
                    SettingCompletePopup.IsOpen = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    InProgress3.Visibility = Visibility.Collapsed;
                    pleasewait3.Visibility = Visibility.Collapsed;
                    appBarButton_refresh.IsEnabled = false;
                }
            }
        }

        //返回状态页面
        private void ReturnToStatusButton_Click(Object sender, RoutedEventArgs e)
        {
            if (SettingCompletePopup.IsOpen)
            {               
                SettingCompletePopup.IsOpen = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                OnNavigatedTo(null);
            }
        }

        public async Task<string> ReadBypassChildrenDeviceIdFromFile()
        {
            string fileContent = string.Empty;
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (fileStorage.FileExists("Bypass_childrenDeviceId.txt"))
                {
                    using (var file = fileStorage.OpenFile("Bypass_childrenDeviceId.txt", FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(file))
                        {
                            fileContent = await reader.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return fileContent;
        }

        //绑定账号
        private async Task<bool> BindingAccount()                                //GetDevice -> CreateDevice -> SetDNSMasqDeviceID 流程
        {
            bool bSuccessed = false;
            GenieSoapApi soapApi = new GenieSoapApi();
            GenieWebApi webApi = new GenieWebApi();
            string macAddr = "";
            string modelName = "";
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.GetInfo("WLANConfiguration");
            }
            if (dicResponse.Count > 0)
            {
                macAddr = dicResponse["NewWLANMACAddress"];                      //获取MAC地址
                ParentalControlInfo.RouterMacaddr = macAddr;
            }
            dicResponse = new Dictionary<string, string>();
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.GetInfo("DeviceInfo");
            }
            if (dicResponse.Count > 0)
            {
                modelName = dicResponse["ModelName"];                            //获取modelname
            }
            string deviceKey = modelName + "-" + macAddr;
            dicResponse = await webApi.GetDevice(ParentalControlInfo.token, deviceKey);
            if (dicResponse.Count > 0 && dicResponse["status"] == "success")
            {
                await soapApi.SetDNSMasqDeviceID("default", ParentalControlInfo.DeviceId);
                bSuccessed = true;
            }
            else if (dicResponse.Count > 0 && dicResponse["status"] != "success")
            {
                Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                dicResponse2 = await webApi.CreateDevice(ParentalControlInfo.token, deviceKey);
                if (dicResponse2.Count > 0 && dicResponse2["status"] == "success")
                {
                    await soapApi.SetDNSMasqDeviceID("default", ParentalControlInfo.DeviceId);
                    bSuccessed = true;
                }
                else if (dicResponse2.Count > 0 && dicResponse2["status"] != "success")
                {
                    InProgress.Visibility = Visibility.Collapsed;
                    pleasewait.Visibility = Visibility.Collapsed;
                    MessageBox.Show(dicResponse2["error_message"]);
                    bSuccessed = false;
                }
            }
            return bSuccessed;
        }

        private async void CheckAvailable_Click(Object sender, RoutedEventArgs e)
        {
            chenckAvailableInProgress.Visibility = Visibility.Visible;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.CheckNameAvailable(RegUsername.Text);
            if (dicResponse.Count > 0)
            {
                if (dicResponse["status"] != "success")
                {
                    chenckAvailableInProgress.Visibility = Visibility.Collapsed;
                    IsAvailableName.Visibility = Visibility.Visible;
                    IsAvailableName.Text = dicResponse["error_message"];
                    IsAvailableName.Foreground = new SolidColorBrush(Colors.Red);
                    ParentalControlInfo.IsUsernameAvailable = false;
                }
                else
                {
                    string isAvailable = dicResponse["available"];
                    chenckAvailableInProgress.Visibility = Visibility.Collapsed;
                    if (isAvailable == "no")
                    {
                        IsAvailableName.Visibility = Visibility.Visible;
                        IsAvailableName.Text = "User Name is unavailable.";
                        IsAvailableName.Foreground = new SolidColorBrush(Colors.Red);
                    }
                    else if (isAvailable == "yes")
                    {
                        IsAvailableName.Visibility = Visibility.Visible;
                        IsAvailableName.Text = "User Name is Available.";
                        IsAvailableName.Foreground = new SolidColorBrush(Colors.Green);
                    }
                    else
                    {
                        IsAvailableName.Visibility = Visibility.Visible;
                        IsAvailableName.Text = dicResponse["available"];
                    }
                    ParentalControlInfo.IsUsernameAvailable = true;
                }
            }
            else
            {
                chenckAvailableInProgress.Visibility = Visibility.Collapsed;
                IsAvailableName.Visibility = Visibility.Visible;
                IsAvailableName.Text = "Check failed!";
                IsAvailableName.Foreground = new SolidColorBrush(Colors.Red);
            }
        }

        private void IsBlankRegUsername(Object sender, RoutedEventArgs e)
        {
            if (RegUsername.Text == "")
            {
                checkNameAvailable.IsEnabled = false;
                ParentalControlInfo.IsEmptyUsername = true;
            }
            else
            {
                ParentalControlInfo.Username = RegUsername.Text;
                checkNameAvailable.IsEnabled = true;
                ParentalControlInfo.IsEmptyUsername = false;
            }
        }

        private void IsBlankRegPassword(Object sender, RoutedEventArgs e)
        {
            if (RegPassword.Password == "")
            {
                ParentalControlInfo.IsEmptyPassword = true;
            }
            else
            {
                ParentalControlInfo.Password = RegPassword.Password;
                ParentalControlInfo.IsEmptyPassword = false;
            }
            eventConfirmPassword(null, null);
        }

        private void eventConfirmPassword(Object sender, RoutedEventArgs e)
        {
            if (ParentalControlInfo.IsEmptyPassword)
            {
                if (confirmPassword.Password != "")
                {
                    differentPassword.Visibility = Visibility.Visible;
                    ParentalControlInfo.IsDifferentPassword = true;
                } 
                else
                {
                    differentPassword.Visibility = Visibility.Collapsed;
                    ParentalControlInfo.IsDifferentPassword = false;
                }
            } 
            else
            {
                if (RegPassword.Password != confirmPassword.Password)
                {
                    differentPassword.Visibility = Visibility.Visible;
                    ParentalControlInfo.IsDifferentPassword = true;
                }
                else
                {
                    differentPassword.Visibility = Visibility.Collapsed;
                    ParentalControlInfo.IsDifferentPassword = false;
                }
            }            
        }

        private void IsEmailValid(Object sender, RoutedEventArgs e)
        {
            if (email.Text == "")
            {
                invalidEmail.Visibility = Visibility.Collapsed;
                ParentalControlInfo.IsEmptyEmail = true;
            }
            else
            {
                bool bValidEmail = ValidateEmail(email.Text);
                if (bValidEmail == true)
                {
                    invalidEmail.Visibility = Visibility.Collapsed;
                    ParentalControlInfo.Email = email.Text;
                    ParentalControlInfo.IsEmptyEmail = false;
                }
                else
                {
                    invalidEmail.Visibility = Visibility.Visible;
                    ParentalControlInfo.IsEmptyEmail = true;
                }
            }
            eventConfirmEmail(null, null);
        }

        private void eventConfirmEmail(Object sender, RoutedEventArgs e)
        {
            if (ParentalControlInfo.IsEmptyEmail)
            {
                if (congfirmEmail.Text != "")
                {
                    differentEmail.Visibility = Visibility.Visible;
                    ParentalControlInfo.IsDifferentEmail = true;
                } 
                else
                {
                    differentEmail.Visibility = Visibility.Collapsed;
                    ParentalControlInfo.IsDifferentEmail = false;
                }
            } 
            else
            {
                if (email.Text != congfirmEmail.Text)
                {
                    differentEmail.Visibility = Visibility.Visible;
                    ParentalControlInfo.IsDifferentEmail = true;
                }
                else
                {
                    differentEmail.Visibility = Visibility.Collapsed;
                    ParentalControlInfo.IsDifferentEmail = false;
                }
            }            
        }

        /// <summary>
        /// 验证邮箱是否合法
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        private bool ValidateEmail(string email)
        {
            string regexEmail = @"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$";
            System.Text.RegularExpressions.RegexOptions options = ((System.Text.RegularExpressions.RegexOptions.IgnorePatternWhitespace

                | System.Text.RegularExpressions.RegexOptions.Multiline)
                        | System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            System.Text.RegularExpressions.Regex regEmail = new System.Text.RegularExpressions.Regex(regexEmail, options);
            if (regEmail.IsMatch(email))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //private void IsBlankLoginUsername(Object sender, RoutedEventArgs e)
        //{
        //    if (LoginUsername.Text == "")
        //    {
        //        ParentalControlInfo.IsEmptyUsername = true;
        //    }
        //    else
        //    {
        //        ParentalControlInfo.Username = LoginUsername.Text;
        //        ParentalControlInfo.IsEmptyUsername = false;
        //    }
        //}

        //private void IsBlankLoginPassword(Object sender, RoutedEventArgs e)
        //{
        //    if (LoginPassword.Password == "")
        //    {
        //        ParentalControlInfo.IsEmptyPassword = true;
        //    }
        //    else
        //    {
        //        ParentalControlInfo.Password = LoginPassword.Password;
        //        ParentalControlInfo.IsEmptyPassword = false;
        //    }
        //}

        private void RadioButton_Checked(Object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
            switch (rb.Name)
            {
                case "radioButton_None":
                    ParentalControlInfo.filterLevelSelected = "None";
                    break;
                case "radioButton_Minimum":
                    ParentalControlInfo.filterLevelSelected = "Minimal";
                    break;
                case "radioButton_Low":
                    ParentalControlInfo.filterLevelSelected = "Low";
                    break;
                case "radioButton_Medium":
                    ParentalControlInfo.filterLevelSelected = "Moderate";
                    break;
                case "radioButton_High":
                    ParentalControlInfo.filterLevelSelected = "High";
                    break;
                case "radioButton_Custom":
                    ParentalControlInfo.filterLevelSelected = "Custom";
                    break;
            }
        }

        //点击“自定义”RadioButton的响应事件
        private void CustomRBClicked(Object sender, RoutedEventArgs e)
        {
            if (!CategoriesPopup.IsOpen)
            {
                FilterLevelPopup.IsOpen = false;
                CategoriesPopup.IsOpen = true;
            }
        }

        //关闭“自定义”类别窗口响应事件
        private void CategoriesCancelBtn_Click(object sender, RoutedEventArgs e)
        {
            if (CategoriesPopup.IsOpen)
            {
                ParentalControlInfo.categoriesSelected = string.Empty;
                if (category_1.IsChecked == true)
                {
                    ParentalControlInfo.categoriesSelected += "Academic Fraud";
                }
                if (category_2.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Adult Themes";
                    else
                        ParentalControlInfo.categoriesSelected += ",Adult Themes";
                }
                if (category_3.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Adware";
                    else
                        ParentalControlInfo.categoriesSelected += ",Adware";
                }
                if (category_4.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Alcohol";
                    else
                        ParentalControlInfo.categoriesSelected += ",Alcohol";
                }
                if (category_5.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Anime/Manga/Webcomic";
                    else
                        ParentalControlInfo.categoriesSelected += ",Anime/Manga/Webcomic";
                }
                if (category_6.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Auctions";
                    else
                        ParentalControlInfo.categoriesSelected += ",Auctions";
                }
                if (category_7.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Automotive";
                    else
                        ParentalControlInfo.categoriesSelected += ",Automotive";
                }
                if (category_8.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Blogs";
                    else
                        ParentalControlInfo.categoriesSelected += ",Blogs";
                }
                if (category_9.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Business Services";
                    else
                        ParentalControlInfo.categoriesSelected += ",Business Services";
                }
                if (category_10.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Chat";
                    else
                        ParentalControlInfo.categoriesSelected += ",Chat";
                }
                if (category_11.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Classifieds";
                    else
                        ParentalControlInfo.categoriesSelected += ",Classifieds";
                }
                if (category_12.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Dating";
                    else
                        ParentalControlInfo.categoriesSelected += ",Dating";
                }
                if (category_13.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Drugs";
                    else
                        ParentalControlInfo.categoriesSelected += ",Drugs";
                }
                if (category_14.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Ecommerce/Shopping";
                    else
                        ParentalControlInfo.categoriesSelected += ",Ecommerce/Shopping";
                }
                if (category_15.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Educational Institutions";
                    else
                        ParentalControlInfo.categoriesSelected += ",Educational Institutions";
                }
                if (category_16.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "File Storage";
                    else
                        ParentalControlInfo.categoriesSelected += ",File Storage";
                }
                if (category_17.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Financial Institutions";
                    else
                        ParentalControlInfo.categoriesSelected += ",Financial Institutions";
                }
                if (category_18.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Forums/Message boards";
                    else
                        ParentalControlInfo.categoriesSelected += ",Forums/Message boards";
                }
                if (category_19.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Gambling";
                    else
                        ParentalControlInfo.categoriesSelected += ",Gambling";
                }
                if (category_20.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Games";
                    else
                        ParentalControlInfo.categoriesSelected += ",Games";
                }
                if (category_21.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "German Youth Protection";
                    else
                        ParentalControlInfo.categoriesSelected += ",German Youth Protection";
                }
                if (category_22.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Government";
                    else
                        ParentalControlInfo.categoriesSelected += ",Government";
                }
                if (category_23.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Hate/Discrimination";
                    else
                        ParentalControlInfo.categoriesSelected += ",Hate/Discrimination";
                }
                if (category_24.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Health and Fitness";
                    else
                        ParentalControlInfo.categoriesSelected += ",Health and Fitness";
                }
                if (category_25.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Humor";
                    else
                        ParentalControlInfo.categoriesSelected += ",Humor";
                }
                if (category_26.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Instant Messaging";
                    else
                        ParentalControlInfo.categoriesSelected += ",Instant Messaging";
                }
                if (category_27.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Jobs/Employment";
                    else
                        ParentalControlInfo.categoriesSelected += ",Jobs/Employment";
                }
                if (category_28.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Lingerie/Bikini";
                    else
                        ParentalControlInfo.categoriesSelected += ",Lingerie/Bikini";
                }
                if (category_29.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Movies";
                    else
                        ParentalControlInfo.categoriesSelected += ",Movies";
                }
                if (category_30.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Music";
                    else
                        ParentalControlInfo.categoriesSelected += ",Music";
                }
                if (category_31.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "News/Media";
                    else
                        ParentalControlInfo.categoriesSelected += ",News/Media";
                }
                if (category_32.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Non-Profits";
                    else
                        ParentalControlInfo.categoriesSelected += ",Non-Profits";
                }
                if (category_33.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Nudity";
                    else
                        ParentalControlInfo.categoriesSelected += ",Nudity";
                }
                if (category_34.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "P2P/File sharing";
                    else
                        ParentalControlInfo.categoriesSelected += ",P2P/File sharing";
                }
                if (category_35.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Parked Domains";
                    else
                        ParentalControlInfo.categoriesSelected += ",Parked Domains";
                }
                if (category_36.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Photo Sharing";
                    else
                        ParentalControlInfo.categoriesSelected += ",Photo Sharing";
                }
                if (category_37.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Podcasts";
                    else
                        ParentalControlInfo.categoriesSelected += ",Podcasts";
                }
                if (category_38.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Politics";
                    else
                        ParentalControlInfo.categoriesSelected += ",Politics";
                }
                if (category_39.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Pornography";
                    else
                        ParentalControlInfo.categoriesSelected += ",Pornography";
                }
                if (category_40.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Portals";
                    else
                        ParentalControlInfo.categoriesSelected += ",Portals";
                }
                if (category_41.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Proxy/Anonymizer";
                    else
                        ParentalControlInfo.categoriesSelected += ",Proxy/Anonymizer";
                }
                if (category_42.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Radio";
                    else
                        ParentalControlInfo.categoriesSelected += ",Radio";
                }
                if (category_43.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Religious";
                    else
                        ParentalControlInfo.categoriesSelected += ",Religious";
                }
                if (category_44.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Research/Reference";
                    else
                        ParentalControlInfo.categoriesSelected += ",Research/Reference";
                }
                if (category_45.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Search Engines";
                    else
                        ParentalControlInfo.categoriesSelected += ",Search Engines";
                }
                if (category_46.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Sexuality";
                    else
                        ParentalControlInfo.categoriesSelected += ",Sexuality";
                }
                if (category_47.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Social Networking";
                    else
                        ParentalControlInfo.categoriesSelected += ",Social Networking";
                }
                if (category_48.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Software/Technology";
                    else
                        ParentalControlInfo.categoriesSelected += ",Software/Technology";
                }
                if (category_49.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Sports";
                    else
                        ParentalControlInfo.categoriesSelected += ",Sports";
                }
                if (category_50.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Tasteless";
                    else
                        ParentalControlInfo.categoriesSelected += ",Tasteless";
                }
                if (category_51.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Television";
                    else
                        ParentalControlInfo.categoriesSelected += ",Television";
                }
                if (category_52.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Tobacco";
                    else
                        ParentalControlInfo.categoriesSelected += ",Tobacco";
                }
                if (category_53.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Travel";
                    else
                        ParentalControlInfo.categoriesSelected += ",Travel";
                }
                if (category_54.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Typo Squatting";
                    else
                        ParentalControlInfo.categoriesSelected += ",Typo Squatting";
                }
                if (category_55.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Video Sharing";
                    else
                        ParentalControlInfo.categoriesSelected += ",Video Sharing";
                }
                if (category_56.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Visual Search Engines";
                    else
                        ParentalControlInfo.categoriesSelected += ",Visual Search Engines";
                }
                if (category_57.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Weapons";
                    else
                        ParentalControlInfo.categoriesSelected += ",Weapons";
                }
                if (category_58.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Web Spam";
                    else
                        ParentalControlInfo.categoriesSelected += ",Web Spam";
                }
                if (category_59.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Webmail";
                    else
                        ParentalControlInfo.categoriesSelected += ",Webmail";
                }
                if (category_60.IsChecked == true)
                {
                    if (ParentalControlInfo.categoriesSelected == string.Empty)
                        ParentalControlInfo.categoriesSelected += "Phishing Protection";
                    else
                        ParentalControlInfo.categoriesSelected += ",Phishing Protection";
                }
                ParentalControlInfo.IsCategoriesChanged = true;
                CategoriesPopup.IsOpen = false;
                FilterLevelPopup.IsOpen = true;
            }
        }

        //全选
        private void SelectAllBtn_Click(object sender, RoutedEventArgs e)
        {
            category_1.IsChecked = true;
            category_2.IsChecked = true;
            category_3.IsChecked = true;
            category_4.IsChecked = true;
            category_5.IsChecked = true;
            category_6.IsChecked = true;
            category_7.IsChecked = true;
            category_8.IsChecked = true;
            category_9.IsChecked = true;
            category_10.IsChecked = true;
            category_11.IsChecked = true;
            category_12.IsChecked = true;
            category_13.IsChecked = true;
            category_14.IsChecked = true;
            category_15.IsChecked = true;
            category_16.IsChecked = true;
            category_17.IsChecked = true;
            category_18.IsChecked = true;
            category_19.IsChecked = true;
            category_20.IsChecked = true;
            category_21.IsChecked = true;
            category_22.IsChecked = true;
            category_23.IsChecked = true;
            category_24.IsChecked = true;
            category_25.IsChecked = true;
            category_26.IsChecked = true;
            category_27.IsChecked = true;
            category_28.IsChecked = true;
            category_29.IsChecked = true;
            category_30.IsChecked = true;
            category_31.IsChecked = true;
            category_32.IsChecked = true;
            category_33.IsChecked = true;
            category_34.IsChecked = true;
            category_35.IsChecked = true;
            category_36.IsChecked = true;
            category_37.IsChecked = true;
            category_38.IsChecked = true;
            category_39.IsChecked = true;
            category_40.IsChecked = true;
            category_41.IsChecked = true;
            category_42.IsChecked = true;
            category_43.IsChecked = true;
            category_44.IsChecked = true;
            category_45.IsChecked = true;
            category_46.IsChecked = true;
            category_47.IsChecked = true;
            category_48.IsChecked = true;
            category_49.IsChecked = true;
            category_50.IsChecked = true;
            category_51.IsChecked = true;
            category_52.IsChecked = true;
            category_53.IsChecked = true;
            category_54.IsChecked = true;
            category_55.IsChecked = true;
            category_56.IsChecked = true;
            category_57.IsChecked = true;
            category_58.IsChecked = true;
            category_59.IsChecked = true;
            category_60.IsChecked = true;
        }

        //全不选
        private void DeselectAllBtn_Click(object sender, RoutedEventArgs e)
        {
            category_1.IsChecked = false;
            category_2.IsChecked = false;
            category_3.IsChecked = false;
            category_4.IsChecked = false;
            category_5.IsChecked = false;
            category_6.IsChecked = false;
            category_7.IsChecked = false;
            category_8.IsChecked = false;
            category_9.IsChecked = false;
            category_10.IsChecked = false;
            category_11.IsChecked = false;
            category_12.IsChecked = false;
            category_13.IsChecked = false;
            category_14.IsChecked = false;
            category_15.IsChecked = false;
            category_16.IsChecked = false;
            category_17.IsChecked = false;
            category_18.IsChecked = false;
            category_19.IsChecked = false;
            category_20.IsChecked = false;
            category_21.IsChecked = false;
            category_22.IsChecked = false;
            category_23.IsChecked = false;
            category_24.IsChecked = false;
            category_25.IsChecked = false;
            category_26.IsChecked = false;
            category_27.IsChecked = false;
            category_28.IsChecked = false;
            category_29.IsChecked = false;
            category_30.IsChecked = false;
            category_31.IsChecked = false;
            category_32.IsChecked = false;
            category_33.IsChecked = false;
            category_34.IsChecked = false;
            category_35.IsChecked = false;
            category_36.IsChecked = false;
            category_37.IsChecked = false;
            category_38.IsChecked = false;
            category_39.IsChecked = false;
            category_40.IsChecked = false;
            category_41.IsChecked = false;
            category_42.IsChecked = false;
            category_43.IsChecked = false;
            category_44.IsChecked = false;
            category_45.IsChecked = false;
            category_46.IsChecked = false;
            category_47.IsChecked = false;
            category_48.IsChecked = false;
            category_49.IsChecked = false;
            category_50.IsChecked = false;
            category_51.IsChecked = false;
            category_52.IsChecked = false;
            category_53.IsChecked = false;
            category_54.IsChecked = false;
            category_55.IsChecked = false;
            category_56.IsChecked = false;
            category_57.IsChecked = false;
            category_58.IsChecked = false;
            category_59.IsChecked = false;
            category_60.IsChecked = false;
        }

        private void CategoriesList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // 如果所选项为空(没有选定内容)，则不执行任何操作
            if (CategoriesList.SelectedItem == null)
                return;

            switch (CategoriesList.SelectedIndex)
            {
                case 0:
                    category_1.IsChecked = true;
                    break;
                case 1:
                    category_2.IsChecked = true;
                    break;
                case 2:
                    category_3.IsChecked = true;
                    break;
                case 3:
                    category_4.IsChecked = true;
                    break;
                case 4:
                    category_5.IsChecked = true;
                    break;
                case 5:
                    category_6.IsChecked = true;
                    break;
                case 6:
                    category_7.IsChecked = true;
                    break;
                case 7:
                    category_8.IsChecked = true;
                    break;
                case 8:
                    category_9.IsChecked = true;
                    break;
                case 9:
                    category_10.IsChecked = true;
                    break;
                case 10:
                    category_11.IsChecked = true;
                    break;
                case 11:
                    category_12.IsChecked = true;
                    break;
                case 12:
                    category_13.IsChecked = true;
                    break;
                case 13:
                    category_14.IsChecked = true;
                    break;
                case 14:
                    category_15.IsChecked = true;
                    break;
                case 15:
                    category_16.IsChecked = true;
                    break;
                case 16:
                    category_17.IsChecked = true;
                    break;
                case 17:
                    category_18.IsChecked = true;
                    break;
                case 18:
                    category_19.IsChecked = true;
                    break;
                case 19:
                    category_20.IsChecked = true;
                    break;
                case 20:
                    category_21.IsChecked = true;
                    break;
                case 21:
                    category_22.IsChecked = true;
                    break;
                case 22:
                    category_23.IsChecked = true;
                    break;
                case 23:
                    category_24.IsChecked = true;
                    break;
                case 24:
                    category_25.IsChecked = true;
                    break;
                case 25:
                    category_26.IsChecked = true;
                    break;
                case 26:
                    category_27.IsChecked = true;
                    break;
                case 27:
                    category_28.IsChecked = true;
                    break;
                case 28:
                    category_29.IsChecked = true;
                    break;
                case 29:
                    category_30.IsChecked = true;
                    break;
                case 30:
                    category_31.IsChecked = true;
                    break;
                case 31:
                    category_32.IsChecked = true;
                    break;
                case 32:
                    category_33.IsChecked = true;
                    break;
                case 33:
                    category_34.IsChecked = true;
                    break;
                case 34:
                    category_35.IsChecked = true;
                    break;
                case 35:
                    category_36.IsChecked = true;
                    break;
                case 36:
                    category_37.IsChecked = true;
                    break;
                case 37:
                    category_38.IsChecked = true;
                    break;
                case 38:
                    category_39.IsChecked = true;
                    break;
                case 39:
                    category_40.IsChecked = true;
                    break;
                case 40:
                    category_41.IsChecked = true;
                    break;
                case 41:
                    category_42.IsChecked = true;
                    break;
                case 42:
                    category_43.IsChecked = true;
                    break;
                case 43:
                    category_44.IsChecked = true;
                    break;
                case 44:
                    category_45.IsChecked = true;
                    break;
                case 45:
                    category_46.IsChecked = true;
                    break;
                case 46:
                    category_47.IsChecked = true;
                    break;
                case 47:
                    category_48.IsChecked = true;
                    break;
                case 48:
                    category_49.IsChecked = true;
                    break;
                case 49:
                    category_50.IsChecked = true;
                    break;
                case 50:
                    category_51.IsChecked = true;
                    break;
                case 51:
                    category_52.IsChecked = true;
                    break;
                case 52:
                    category_53.IsChecked = true;
                    break;
                case 53:
                    category_54.IsChecked = true;
                    break;
                case 54:
                    category_55.IsChecked = true;
                    break;
                case 55:
                    category_56.IsChecked = true;
                    break;
                case 56:
                    category_57.IsChecked = true;
                    break;
                case 57:
                    category_58.IsChecked = true;
                    break;
                case 58:
                    category_59.IsChecked = true;
                    break;
                case 59:
                    category_60.IsChecked = true;
                    break;
            }

            // 将所选项重置为 null (没有选定内容)
            CategoriesList.SelectedItem = null;
        }

        private void RegUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            RegUsername.Background = new SolidColorBrush(Colors.White);
        }

        private void RegPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            RegPassword.Background = new SolidColorBrush(Colors.White);
        }

        private void confirmPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            confirmPassword.Background = new SolidColorBrush(Colors.White);
        }

        private void email_GotFocus(object sender, RoutedEventArgs e)
        {
            email.Background = new SolidColorBrush(Colors.White);
        }

        private void congfirmEmail_GotFocus(object sender, RoutedEventArgs e)
        {
            congfirmEmail.Background = new SolidColorBrush(Colors.White);
        }

        private void LoginUsername_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginUsername.Background = new SolidColorBrush(Colors.White);
        }

        private void LoginPassword_GotFocus(object sender, RoutedEventArgs e)
        {
            LoginPassword.Background = new SolidColorBrush(Colors.White);
        }

        //按下屏幕键盘回车键后关闭屏幕键盘
        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == System.Windows.Input.Key.Enter)
            {
                this.Focus();
            }
            else
            {
                base.OnKeyDown(e);
            } 
        }

        //登录OpenDNS账号后保存信息，不切换路由器就无需再次登录
        public async void WriteSavedInfoToFile()
        {
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (!fileStorage.FileExists("LoginOpenDNSSavedInfo.txt"))
                {
                    using (var file = fileStorage.CreateFile("LoginOpenDNSSavedInfo.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(ParentalControlInfo.SavedInfo);
                        }
                    }
                }
                else
                {
                    fileStorage.DeleteFile("LoginOpenDNSSavedInfo.txt");
                    using (var file = fileStorage.CreateFile("LoginOpenDNSSavedInfo.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(ParentalControlInfo.SavedInfo);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        //读取保存的信息
        public async Task<string> ReadSavedInfoFromFile()
        {
            string fileContent = string.Empty;
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (fileStorage.FileExists("LoginOpenDNSSavedInfo.txt"))
                {
                    using (var file = fileStorage.OpenFile("LoginOpenDNSSavedInfo.txt", FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = new StreamReader(file))
                        {
                            fileContent = await reader.ReadToEndAsync();
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            return fileContent;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            Grid gridItem = (Grid)sender;
            switch (gridItem.Name)
            {
                case "gridFilterlevel":
                    gridFilterlevel.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                case "gridChangeSetting":
                    gridChangeSetting.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                case "gridBypass":
                    gridBypass.Background = new SolidColorBrush(Color.FromArgb(255, 200, 174, 221));
                    break;
                default:
                    break;
            }            
        }

        private void Grid_MouseLeave(object sender, MouseEventArgs e)
        {
            Grid gridItem = (Grid)sender;
            switch (gridItem.Name)
            {
                case "gridFilterlevel":
                    gridFilterlevel.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridChangeSetting":
                    gridChangeSetting.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                case "gridBypass":
                    gridBypass.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
                    break;
                default:
                    break;
            }
        }

        private void gridFilterlevel_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            gridFilterlevel.Background = new SolidColorBrush(Color.FromArgb(255, 255, 255, 255));
            if (!FilterLevelPopup.IsOpen)
            {
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
                    case "Custom":
                        radioButton_Custom.IsChecked = true;
                        string[] category = ParentalControlInfo.categories.Split(',');
                        for (int i = 0; i < category.Length; i++)
                        {
                            switch (category[i])
                            {
                                case "Academic Fraud":
                                    category_1.IsChecked = true;
                                    break;
                                case "Adult Themes":
                                    category_2.IsChecked = true;
                                    break;
                                case "Adware":
                                    category_3.IsChecked = true;
                                    break;
                                case "Alcohol":
                                    category_4.IsChecked = true;
                                    break;
                                case "Anime/Manga/Webcomic":
                                    category_5.IsChecked = true;
                                    break;
                                case "Auctions":
                                    category_6.IsChecked = true;
                                    break;
                                case "Automotive":
                                    category_7.IsChecked = true;
                                    break;
                                case "Blogs":
                                    category_8.IsChecked = true;
                                    break;
                                case "Business Services":
                                    category_9.IsChecked = true;
                                    break;
                                case "Chat":
                                    category_10.IsChecked = true;
                                    break;
                                case "Classifieds":
                                    category_11.IsChecked = true;
                                    break;
                                case "Dating":
                                    category_12.IsChecked = true;
                                    break;
                                case "Drugs":
                                    category_13.IsChecked = true;
                                    break;
                                case "Ecommerce/Shopping":
                                    category_14.IsChecked = true;
                                    break;
                                case "Educational Institutions":
                                    category_15.IsChecked = true;
                                    break;
                                case "File Storage":
                                    category_16.IsChecked = true;
                                    break;
                                case "Financial Institutions":
                                    category_17.IsChecked = true;
                                    break;
                                case "Forums/Message boards":
                                    category_18.IsChecked = true;
                                    break;
                                case "Gambling":
                                    category_19.IsChecked = true;
                                    break;
                                case "Games":
                                    category_20.IsChecked = true;
                                    break;
                                case "German Youth Protection":
                                    category_21.IsChecked = true;
                                    break;
                                case "Government":
                                    category_22.IsChecked = true;
                                    break;
                                case "Hate/Discrimination":
                                    category_23.IsChecked = true;
                                    break;
                                case "Health and Fitness":
                                    category_24.IsChecked = true;
                                    break;
                                case "Humor":
                                    category_25.IsChecked = true;
                                    break;
                                case "Instant Messaging":
                                    category_26.IsChecked = true;
                                    break;
                                case "Jobs/Employment":
                                    category_27.IsChecked = true;
                                    break;
                                case "Lingerie/Bikini":
                                    category_28.IsChecked = true;
                                    break;
                                case "Movies":
                                    category_29.IsChecked = true;
                                    break;
                                case "Music":
                                    category_30.IsChecked = true;
                                    break;
                                case "News/Media":
                                    category_31.IsChecked = true;
                                    break;
                                case "Non-Profits":
                                    category_32.IsChecked = true;
                                    break;
                                case "Nudity":
                                    category_33.IsChecked = true;
                                    break;
                                case "P2P/File sharing":
                                    category_34.IsChecked = true;
                                    break;
                                case "Parked Domains":
                                    category_35.IsChecked = true;
                                    break;
                                case "Photo Sharing":
                                    category_36.IsChecked = true;
                                    break;
                                case "Podcasts":
                                    category_37.IsChecked = true;
                                    break;
                                case "Politics":
                                    category_38.IsChecked = true;
                                    break;
                                case "Pornography":
                                    category_39.IsChecked = true;
                                    break;
                                case "Portals":
                                    category_40.IsChecked = true;
                                    break;
                                case "Proxy/Anonymizer":
                                    category_41.IsChecked = true;
                                    break;
                                case "Radio":
                                    category_42.IsChecked = true;
                                    break;
                                case "Religious":
                                    category_43.IsChecked = true;
                                    break;
                                case "Research/Reference":
                                    category_44.IsChecked = true;
                                    break;
                                case "Search Engines":
                                    category_45.IsChecked = true;
                                    break;
                                case "Sexuality":
                                    category_46.IsChecked = true;
                                    break;
                                case "Social Networking":
                                    category_47.IsChecked = true;
                                    break;
                                case "Software/Technology":
                                    category_48.IsChecked = true;
                                    break;
                                case "Sports":
                                    category_49.IsChecked = true;
                                    break;
                                case "Tasteless":
                                    category_50.IsChecked = true;
                                    break;
                                case "Television":
                                    category_51.IsChecked = true;
                                    break;
                                case "Tobacco":
                                    category_52.IsChecked = true;
                                    break;
                                case "Travel":
                                    category_53.IsChecked = true;
                                    break;
                                case "Typo Squatting":
                                    category_54.IsChecked = true;
                                    break;
                                case "Video Sharing":
                                    category_55.IsChecked = true;
                                    break;
                                case "Visual Search Engines":
                                    category_56.IsChecked = true;
                                    break;
                                case "Weapons":
                                    category_57.IsChecked = true;
                                    break;
                                case "Web Spam":
                                    category_58.IsChecked = true;
                                    break;
                                case "Webmail":
                                    category_59.IsChecked = true;
                                    break;
                                case "Phishing Protection":
                                    category_60.IsChecked = true;
                                    break;
                            }   //switch (category[i])
                        }   //for
                        break;
                    default:
                        break;
                }
                FilterLevelPopup.IsOpen = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
                appBarButton_refresh.IsEnabled = false;
            }
        }
    }
}