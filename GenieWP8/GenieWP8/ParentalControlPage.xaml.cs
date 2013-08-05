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

namespace GenieWP8
{
    public partial class ParentalControlPage : PhoneApplicationPage
    {
        private static ParentalControlModel settingModel = null;
        public ParentalControlPage()
        {
            InitializeComponent();

            // 将 LongListSelector 控件的数据上下文设置为绑定数据
            if (settingModel == null)
                settingModel = new ParentalControlModel();
            DataContext = settingModel;

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();

            if ((this.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                RegisterScrollViewer.Height = 500;
                FilterLevelScrollViewer.Height = 500;
                SettingCompleteScrollViewer.Height = 500;
            }
            else
            {
                RegisterScrollViewer.Height = 250;
                FilterLevelScrollViewer.Height = 250;
                SettingCompleteScrollViewer.Height = 250;
            }
        }

        // 为 ParentalControlModel 项加载数据
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            settingModel.FilterLevelGroups.Clear();
            settingModel.LoadData();

            if (!ParentalControlInfo.IsOpenDNSLoggedIn)	//未登录OpenDNS账户
            {
                if (!EnquirePopup.IsOpen)
                {
                    EnquirePopup.IsOpen = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    InProgress.Visibility = Visibility.Collapsed;
                    pleasewait.Visibility = Visibility.Collapsed;
                }
            }
            else
            {
                if (ParentalControlInfo.isParentalControlEnabled == "0")
                {
                    checkPatentalControl.IsChecked = false;
                    FilterLevelLongListSelector.Visibility = Visibility.Collapsed;
                    ChangeCustomSettings.Visibility = Visibility.Collapsed;
                    stpOpenDNSAccount.Visibility = Visibility.Collapsed;
                    BypassAccount.Visibility = Visibility.Collapsed;
                }
                else if (ParentalControlInfo.isParentalControlEnabled == "1")
                {
                    checkPatentalControl.IsChecked = true;
                    FilterLevelLongListSelector.Visibility = Visibility.Visible;
                    ChangeCustomSettings.Visibility = Visibility.Visible;
                    stpOpenDNSAccount.Visibility = Visibility.Visible;
                    BypassAccount.Visibility = Visibility.Visible;
                }
            }

            if (ParentalControlInfo.Username != null)
            {
                OpenDNSUserName.Text = ParentalControlInfo.Username;
            }
            else
            {
                OpenDNSUserName.Text = "";
            }

            ParentalControlInfo.BypassChildrenDeviceId = await ReadBypassChildrenDeviceIdFromFile();                      //读取本地保存的DeviceId，如果不为空则获得当前登录的Bypass账户
            if (ParentalControlInfo.BypassChildrenDeviceId != null && ParentalControlInfo.BypassChildrenDeviceId != "")
            {
                ParentalControlInfo.IsBypassUserLoggedIn = true;
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse1 = new Dictionary<string, string>();
                dicResponse1 = await webApi.GetUserForChildDeviceId(ParentalControlInfo.BypassChildrenDeviceId);
                if (dicResponse1["status"] == "success")
                {
                    ParentalControlInfo.BypassUsername = dicResponse1["bypass_user"];
                }
                else
                {
                    MessageBox.Show(dicResponse1["error_message"]);
                }
            }
            else
            {
                ParentalControlInfo.IsBypassUserLoggedIn = false;
            }

            if (ParentalControlInfo.BypassUsername != null)
            {
                bypassaccount.Text = ParentalControlInfo.BypassUsername;
            }
            else
            {
                bypassaccount.Text = "";
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
                FilterLevelLongListSelector.Visibility = Visibility.Visible;
                ChangeCustomSettings.Visibility = Visibility.Visible;
                stpOpenDNSAccount.Visibility = Visibility.Visible;
                BypassAccount.Visibility = Visibility.Visible;
            }
            else if (checkPatentalControl.IsChecked == false)
            {
                parentalControlEnable = "0";
                dicResponse = await soapApi.EnableParentalControl(parentalControlEnable);
                FilterLevelLongListSelector.Visibility = Visibility.Collapsed;
                ChangeCustomSettings.Visibility = Visibility.Collapsed;
                stpOpenDNSAccount.Visibility = Visibility.Collapsed;
                BypassAccount.Visibility = Visibility.Collapsed;
            }
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        // 处理在 LongListSelector 中更改的选定内容
        private void FilterLevelLongListSelector_SelectionChanged(Object sender, SelectionChangedEventArgs e)
        {
            // 如果所选项为空(没有选定内容)，则不执行任何操作
            if (FilterLevelLongListSelector.SelectedItem == null)
                return;

            //PopupFilterLevel __popupFilterLevel = new PopupFilterLevel();
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
                    default:
                        break;
                }
                FilterLevelPopup.IsOpen = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
            }

            // 将所选项重置为 null (没有选定内容)
            FilterLevelLongListSelector.SelectedItem = null;
        }

        // 处理 ChangeCustomSettings 触摸输入事件
        private async void ChangeSetting_Click(Object sender, MouseButtonEventArgs e)
        {
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
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    MessageBox.Show(dicResponse["error_message"]);
                }
            }
            else                                                                        //已登录Bypass账户
            {
                NavigationService.Navigate(new Uri("/BypassAccountLogoutPage.xaml", UriKind.Relative));
            }
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
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetEnableStatus();
            ParentalControlInfo.isParentalControlEnabled = dicResponse["ParentalControl"];
            OnNavigatedTo(null);
        }

        //横竖屏切换响应事件
        private void PhoneApplicationPage_OrientationChanged(object sender, OrientationChangedEventArgs e)
        {
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                RegisterScrollViewer.Height = 500;
                FilterLevelScrollViewer.Height = 500;
                SettingCompleteScrollViewer.Height = 500;
            }
            else
            {
                RegisterScrollViewer.Height = 250;
                FilterLevelScrollViewer.Height = 250;
                SettingCompleteScrollViewer.Height = 250;
            }
        }

        //没有OpenDNS账号
        private void NoButton_Click(Object sender, RoutedEventArgs e)
        {
            if (EnquirePopup.IsOpen)
            {
                EnquirePopup.IsOpen = false;
                RegisterPopup.IsOpen = true;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
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
                InProgress.Visibility = Visibility.Visible;
                pleasewait.Visibility = Visibility.Visible;
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.CreateAccount(ParentalControlInfo.Username, ParentalControlInfo.Password, ParentalControlInfo.Email);
                if (dicResponse["status"] != "success")
                {
                    InProgress.Visibility = Visibility.Collapsed;
                    pleasewait.Visibility = Visibility.Collapsed;
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
                        InProgress.Visibility = Visibility.Collapsed;
                        pleasewait.Visibility = Visibility.Collapsed;
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
            }
        }

        //登陆下一步
        private async void LoginNextButton_Click(Object sender, RoutedEventArgs e)
        {
            if (ParentalControlInfo.IsEmptyUsername == true || ParentalControlInfo.IsEmptyPassword == true)
            {
                MessageBox.Show("Failed, please enter the correct information");
            }
            else
            {
                InProgress.Visibility = Visibility.Visible;
                pleasewait.Visibility = Visibility.Visible;
                //登录OpenDNS账号
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.BeginLogin(ParentalControlInfo.Username, ParentalControlInfo.Password);
                if (dicResponse.Count > 0)
                {
                    if (dicResponse["status"] != "success")
                    {
                        InProgress.Visibility = Visibility.Collapsed;
                        pleasewait.Visibility = Visibility.Collapsed;
                        MessageBox.Show(dicResponse["error_message"]);
                    }
                    else
                    {
                        ParentalControlInfo.IsOpenDNSLoggedIn = true;
                        ParentalControlInfo.token = dicResponse["token"];
                        //认证路由器
                        GenieSoapApi soapApi = new GenieSoapApi();
                        Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                        dicResponse2 = await soapApi.Authenticate(MainPageInfo.username, MainPageInfo.password);
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
                                            InProgress.Visibility = Visibility.Collapsed;
                                            pleasewait.Visibility = Visibility.Collapsed;
                                            MessageBox.Show(dicResponse4["error_message"]);
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
                                            default:
                                                break;
                                        }
                                        FilterLevelPopup.IsOpen = true;
                                        PopupBackgroundTop.Visibility = Visibility.Visible;
                                        PopupBackground.Visibility = Visibility.Visible;
                                        InProgress.Visibility = Visibility.Collapsed;
                                        pleasewait.Visibility = Visibility.Collapsed;
                                    }
                                }
                            }
                            else if (dicResponse3.Count > 0 && int.Parse(dicResponse3["ResponseCode"]) == 401)
                            {
                                InProgress.Visibility = Visibility.Collapsed;
                                pleasewait.Visibility = Visibility.Collapsed;
                                MessageBox.Show("Not authenticated");
                            }
                        }
                        else if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 401)
                        {
                            InProgress.Visibility = Visibility.Collapsed;
                            pleasewait.Visibility = Visibility.Collapsed;
                            MessageBox.Show("Invalid username and/or password");
                        }
                    }
                }
                else
                {
                    InProgress.Visibility = Visibility.Collapsed;
                    pleasewait.Visibility = Visibility.Collapsed;
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
            }
        }

        //设置过滤等级下一步
        private async void FilterLvNextButton_Click(Object sender, RoutedEventArgs e)
        {
            InProgress.Visibility = Visibility.Visible;
            pleasewait.Visibility = Visibility.Visible;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.SetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId, ParentalControlInfo.filterLevel);
            if (dicResponse["status"] != "success")
            {
                InProgress.Visibility = Visibility.Collapsed;
                pleasewait.Visibility = Visibility.Collapsed;
                MessageBox.Show(dicResponse["error_message"]);
            }
            else
            {
                if (FilterLevelPopup.IsOpen)
                {
                    FilterLevelPopup.IsOpen = false;
                    SettingCompletePopup.IsOpen = true;
                    PopupBackgroundTop.Visibility = Visibility.Visible;
                    PopupBackground.Visibility = Visibility.Visible;
                    InProgress.Visibility = Visibility.Collapsed;
                    pleasewait.Visibility = Visibility.Collapsed;
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
            dicResponse = await soapApi.GetInfo("WLANConfiguration");
            if (dicResponse.Count > 0)
            {
                macAddr = dicResponse["NewWLANMACAddress"];                      //获取MAC地址
            }
            dicResponse = await soapApi.GetInfo("DeviceInfo");
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
            if (dicResponse["status"] != "success")
            {
                chenckAvailableInProgress.Visibility = Visibility.Collapsed;
                IsAvailableName.Text = dicResponse["error_message"];
                ParentalControlInfo.IsUsernameAvailable = false;
            }
            else
            {
                string isAvailable = dicResponse["available"];
                chenckAvailableInProgress.Visibility = Visibility.Collapsed;
                if (isAvailable == "no")
                {
                    IsAvailableName.Text = "User Name is unavailable.";
                }
                else if (isAvailable == "yes")
                {
                    IsAvailableName.Text = "User Name is Available.";
                }
                else
                {
                    IsAvailableName.Text = dicResponse["available"];
                }
                ParentalControlInfo.IsUsernameAvailable = true;
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
        }

        private void eventConfirmPassword(Object sender, RoutedEventArgs e)
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

        private void IsEmailValid(Object sender, RoutedEventArgs e)
        {
            if (email.Text == "")
            {
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
        }

        private void eventConfirmEmail(Object sender, RoutedEventArgs e)
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

        private void IsBlankLoginUsername(Object sender, RoutedEventArgs e)
        {
            if (LoginUsername.Text == "")
            {
                ParentalControlInfo.IsEmptyUsername = true;
            }
            else
            {
                ParentalControlInfo.Username = LoginUsername.Text;
                ParentalControlInfo.IsEmptyUsername = false;
            }
        }

        private void IsBlankLoginPassword(Object sender, RoutedEventArgs e)
        {
            if (LoginPassword.Password == "")
            {
                ParentalControlInfo.IsEmptyPassword = true;
            }
            else
            {
                ParentalControlInfo.Password = LoginPassword.Password;
                ParentalControlInfo.IsEmptyPassword = false;
            }
        }

        private void RadioButton_Checked(Object sender, RoutedEventArgs e)
        {
            RadioButton rb = (RadioButton)sender;
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