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
using GenieWin8.DataModel;
using Windows.UI.Popups;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Windows.Storage;
using System.Threading.Tasks;
using Windows.UI;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class ParentalControlPage : GenieWin8.Common.LayoutAwarePage
    {
        public ParentalControlPage()
        {
            this.InitializeComponent();
            if (!ParentalControlInfo.IsOpenDNSLoggedIn)	//未登录OpenDNS账户
	        {
		        if (!EnquirePopup.IsOpen)
		        {
			        EnquirePopup.IsOpen = true;
			        PopupBackground.Visibility = Visibility.Visible;
			        //NoButton.Visibility = Visibility.Visible;
			        //YesButton.Visibility = Visibility.Visible;
                    refreshButton.IsEnabled = false;
		        }
	        }
            else
            {
                if (ParentalControlInfo.isParentalControlEnabled == "0")
                {
                    checkPatentalControl.IsChecked = false;
                    FilterLevelListView.Visibility = Visibility.Collapsed;
                    ChangeCustomSettings.Visibility = Visibility.Collapsed;
                    stpOpenDNSAccount.Visibility = Visibility.Collapsed;
                    BypassAccount.Visibility = Visibility.Collapsed;
                }
                else if (ParentalControlInfo.isParentalControlEnabled == "1")
                {
                    checkPatentalControl.IsChecked = true;
                    FilterLevelListView.Visibility = Visibility.Visible;
                    ChangeCustomSettings.Visibility = Visibility.Visible;
                    stpOpenDNSAccount.Visibility = Visibility.Visible;
                    BypassAccount.Visibility = Visibility.Visible;
                }
                refreshButton.IsEnabled = true;
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
        protected override async void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var FilterLevelGroup = FilterLevelSource.GetFilterLevelGroup((String)navigationParameter);
            this.DefaultViewModel["Group"] = FilterLevelGroup;
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
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.GetUserForChildDeviceId(ParentalControlInfo.BypassChildrenDeviceId);
                if (dicResponse["status"] == "success")
                {
                    ParentalControlInfo.BypassUsername = dicResponse["bypass_user"];
                } 
                else
                {
                    var messageDialog = new MessageDialog(dicResponse["error_message"]);
                    await messageDialog.ShowAsync();
                }
            }
            else
            {
                ParentalControlInfo.IsBypassUserLoggedIn = false;
            }

            if (ParentalControlInfo.IsBypassUserLoggedIn && ParentalControlInfo.BypassUsername != null)
            {
                bypassaccount.Text = ParentalControlInfo.BypassUsername;
            }
            else
            {
                bypassaccount.Text = "";
            }
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

        private void FilterLevel_ItemClick(Object sender, ItemClickEventArgs e)
        {
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
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
		        //FilterLvPreviousButton.Visibility = Visibility.Visible;
		        //FilterLvNextButton.Visibility = Visibility.Visible;
                refreshButton.IsEnabled = false;
	        }
        }

        private async void ChangeSetting_ItemClick(Object sender, ItemClickEventArgs e)
        {
            InProgress.IsActive = true;
            pleasewait.Visibility = Visibility.Visible;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.AccountRelay(ParentalControlInfo.token);
            if (dicResponse["status"] == "success")
            {
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                string relay_token = dicResponse["relay_token"];
                var uri = new Uri("http://netgear.opendns.com/account.php?device_id=" + ParentalControlInfo.DeviceId + "&api_key=3D8C85A77ADA886B967984DF1F8B3711" + "&relay_token=" + relay_token);
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
            else
            {
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                var messageDialog = new MessageDialog(dicResponse["error_message"]);
                await messageDialog.ShowAsync();
            }
        }

        private async void Bypass_ItemClick(Object sender, ItemClickEventArgs e)
        {
            if (ParentalControlInfo.IsBypassUserLoggedIn == false)                        //未登录Bypass账户
            {
                InProgress.IsActive = true;
                pleasewait.Visibility = Visibility.Visible;
                PopupBackgroundTop.Visibility = Visibility.Visible;
                PopupBackground.Visibility = Visibility.Visible;
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
                    InProgress.IsActive = false;
                    pleasewait.Visibility = Visibility.Collapsed;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    this.Frame.Navigate(typeof(BypassAccountPage));
                }
                else
                {
                    InProgress.IsActive = false;
                    pleasewait.Visibility = Visibility.Collapsed;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    var messageDialog = new MessageDialog(dicResponse["error_message"]);
                    await messageDialog.ShowAsync();
                } 
            } 
            else                                                                        //已登录Bypass账户
            {
                this.Frame.Navigate(typeof(BypassAccountLogoutPage));
            }
                      
        }

        //没有OpenDNS账号
        private void NoButton_Click(Object sender, RoutedEventArgs e)
        {
            if (EnquirePopup.IsOpen)
	        {
		        EnquirePopup.IsOpen = false;
		        RegisterPopup.IsOpen = true;
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
		        //NoButton.Visibility = Visibility.Collapsed;
		        //YesButton.Visibility = Visibility.Collapsed;
		        //RegisterPreviousButton.Visibility = Visibility.Visible;
		        //RegisterNextButton.Visibility = Visibility.Visible;
                refreshButton.IsEnabled = false;
	        }
        }

        //拥有OpenDNS账号
        private void YesButton_Click(Object sender, RoutedEventArgs e)
        {
            if (EnquirePopup.IsOpen)
	        {
		        EnquirePopup.IsOpen = false;
		        LoginPopup.IsOpen = true;
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
		        //NoButton.Visibility = Visibility.Collapsed;
		        //YesButton.Visibility = Visibility.Collapsed;
		        //LoginPreviousButton.Visibility = Visibility.Visible;
		        //LoginNextButton.Visibility = Visibility.Visible;
                refreshButton.IsEnabled = false;
	        }
        }

        //检查可用性
        private async void CheckAvailable_Click(Object sender, RoutedEventArgs e)
        {
            InProgress1.IsActive = true;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.CheckNameAvailable(RegUsername.Text);
            if (dicResponse["status"] != "success")
            {
                InProgress1.IsActive = false;
                IsAvailableName.Text = dicResponse["error_message"];
                IsAvailableName.Foreground = new SolidColorBrush(Colors.Red);
                ParentalControlInfo.IsUsernameAvailable = false;
            }
            else
            {
                string isAvailable = dicResponse["available"];
                InProgress1.IsActive = false;
                if (isAvailable == "no")
                {
                    IsAvailableName.Text = "User Name is unavailable.";
                    IsAvailableName.Foreground = new SolidColorBrush(Colors.Red);
                }
                else if (isAvailable == "yes")
                {
                    IsAvailableName.Text = "User Name is Available.";
                    IsAvailableName.Foreground = new SolidColorBrush(Colors.Green);
                }
                else
                {
                    IsAvailableName.Text = dicResponse["available"];
                }
                ParentalControlInfo.IsUsernameAvailable = true;
            }
        }

        //注册用户名是否为空
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

        //注册密码是否为空
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

        //确认密码
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

        //邮箱是否有效
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

        //确认邮箱
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

        //注册上一步
        private void RegisterPreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            if (RegisterPopup.IsOpen)
	        {
		        EnquirePopup.IsOpen = true;
		        RegisterPopup.IsOpen = false;
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
		        //NoButton.Visibility = Visibility.Visible;
		        //YesButton.Visibility = Visibility.Visible;
		        //RegisterPreviousButton.Visibility = Visibility.Collapsed;
		        //RegisterNextButton.Visibility = Visibility.Collapsed;
                refreshButton.IsEnabled = false;
	        }
        }

        //注册下一步
        private async void RegisterNextButton_Click(Object sender, RoutedEventArgs e)
        {
            if (ParentalControlInfo.IsUsernameAvailable == false || ParentalControlInfo.IsEmptyUsername == true || ParentalControlInfo.IsEmptyPassword == true
                || ParentalControlInfo.IsDifferentPassword == true || ParentalControlInfo.IsEmptyEmail == true || ParentalControlInfo.IsDifferentEmail == true)
            {
                var messageDialog = new MessageDialog("Failed, please enter the correct information");
                await messageDialog.ShowAsync();
            } 
            else
            {
                InProgress2.IsActive = true;
                pleasewait2.Visibility = Visibility.Visible;
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.CreateAccount(ParentalControlInfo.Username, ParentalControlInfo.Password, ParentalControlInfo.Email);
                if (dicResponse["status"] != "success")
                {
                    InProgress2.IsActive = false;
                    pleasewait2.Visibility = Visibility.Collapsed;
                    var messageDialog = new MessageDialog(dicResponse["error_message"]);
                    await messageDialog.ShowAsync();
                } 
                else
                {
                    if (RegisterPopup.IsOpen)
                    {
                        RegisterPopup.IsOpen = false;
                        LoginPopup.IsOpen = true;
                        InProgress2.IsActive = false;
                        pleasewait2.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Visible;
                        //RegisterPreviousButton.Visibility = Visibility.Collapsed;
                        //RegisterNextButton.Visibility = Visibility.Collapsed;
                        //LoginPreviousButton.Visibility = Visibility.Visible;
                        //LoginNextButton.Visibility = Visibility.Visible;
                        refreshButton.IsEnabled = false;
                    }
                }              
            }          
        }

        //登陆用户名是否为空
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

        //登陆密码是否为空
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

        //登陆上一步
        private void LoginPreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LoginPopup.IsOpen)
	        {
		        EnquirePopup.IsOpen = true;
		        LoginPopup.IsOpen = false;
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
		        //NoButton.Visibility = Visibility.Visible;
		        //YesButton.Visibility = Visibility.Visible;
		        //LoginPreviousButton.Visibility = Visibility.Collapsed;
		        //LoginNextButton.Visibility = Visibility.Collapsed;
                refreshButton.IsEnabled = false;
	        }
        }

        //登陆下一步
        private async void LoginNextButton_Click(Object sender, RoutedEventArgs e)
        {
            if (ParentalControlInfo.IsEmptyUsername == true || ParentalControlInfo.IsEmptyPassword == true)
            {
                var messageDialog = new MessageDialog("Failed, please enter the correct information");
                await messageDialog.ShowAsync();
            }
            else
            {
                InProgress.IsActive = true;
                pleasewait.Visibility = Visibility.Visible;
                //登录OpenDNS账号
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.BeginLogin(ParentalControlInfo.Username, ParentalControlInfo.Password);
                if (dicResponse.Count > 0)
                {
                    if (dicResponse["status"] != "success")
                    {
                        InProgress.IsActive = false;
                        pleasewait.Visibility = Visibility.Collapsed;
                        var messageDialog = new MessageDialog(dicResponse["error_message"]);
                        await messageDialog.ShowAsync();
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
                                            InProgress.IsActive = false;
                                            pleasewait.Visibility = Visibility.Collapsed;
                                            var messageDialog = new MessageDialog(dicResponse4["error_message"]);
                                            await messageDialog.ShowAsync();
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
                                        InProgress.IsActive = false;
                                        pleasewait.Visibility = Visibility.Collapsed;
                                        PopupBackground.Visibility = Visibility.Visible;
                                        //LoginPreviousButton.Visibility = Visibility.Collapsed;
                                        //LoginNextButton.Visibility = Visibility.Collapsed;
                                        //FilterLvPreviousButton.Visibility = Visibility.Visible;
                                        //FilterLvNextButton.Visibility = Visibility.Visible;
                                        refreshButton.IsEnabled = false;
                                    }
                                }                               
                            }
                            else if (dicResponse3.Count > 0 && int.Parse(dicResponse3["ResponseCode"]) == 401)
                            {
                                InProgress.IsActive = false;
                                pleasewait.Visibility = Visibility.Collapsed;
                                var messageDialog = new MessageDialog("Not authenticated");
                                await messageDialog.ShowAsync();
                            }                            
                        }
                        else if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 401)
                        {
                            InProgress.IsActive = false;
                            pleasewait.Visibility = Visibility.Collapsed;
                            var messageDialog = new MessageDialog("Invalid username and/or password");
                            await messageDialog.ShowAsync();
                        }
                    }
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
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
		        //LoginPreviousButton.Visibility = Visibility.Visible;
		        //LoginNextButton.Visibility = Visibility.Visible;
		        //FilterLvPreviousButton.Visibility = Visibility.Collapsed;
		        //FilterLvNextButton.Visibility = Visibility.Collapsed;
                refreshButton.IsEnabled = false;
	        }
        }

        //设置过滤等级下一步
        private async void FilterLvNextButton_Click(Object sender, RoutedEventArgs e)
        {
            InProgress3.IsActive = true;
            pleasewait3.Visibility = Visibility.Visible;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.SetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId, ParentalControlInfo.filterLevel);
            if (dicResponse["status"] != "success")
            {
                InProgress3.IsActive = false;
                pleasewait3.Visibility = Visibility.Collapsed;
                var messageDialog = new MessageDialog(dicResponse["error_message"]);
                await messageDialog.ShowAsync();
            } 
            else
            {
                if (FilterLevelPopup.IsOpen)
                {
                    FilterLevelPopup.IsOpen = false;
                    SettingCompletePopup.IsOpen = true;
                    InProgress3.IsActive = false;
                    pleasewait3.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Visible;
                    //FilterLvPreviousButton.Visibility = Visibility.Collapsed;
                    //FilterLvNextButton.Visibility = Visibility.Collapsed;
                    //ReturnToStatusButton.Visibility = Visibility.Visible;
                    refreshButton.IsEnabled = false;
                }
            }           
        }

        //返回状态页面
        private void ReturnToStatusButton_Click(Object sender, RoutedEventArgs e)
        {
            if (SettingCompletePopup.IsOpen)
	        {
		        SettingCompletePopup.IsOpen = false;
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Collapsed;
		        //ReturnToStatusButton.Visibility = Visibility.Collapsed;
                this.Frame.Navigate(typeof(ParentalControlPage));
	        }
        }

        private async void checkParentalControl_Click(Object sender, RoutedEventArgs e)
        {
            InProgress.IsActive = true;
            pleasewait.Visibility = Visibility.Visible;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            string parentalControlEnable;
            if (checkPatentalControl.IsChecked == true)
            {
                parentalControlEnable = "1";
                dicResponse = await soapApi.EnableParentalControl(parentalControlEnable);
                FilterLevelListView.Visibility = Visibility.Visible;
                ChangeCustomSettings.Visibility = Visibility.Visible;
                stpOpenDNSAccount.Visibility = Visibility.Visible;
                BypassAccount.Visibility = Visibility.Visible;
            }
            else if (checkPatentalControl.IsChecked == false)
            {
                parentalControlEnable = "0";
                dicResponse = await soapApi.EnableParentalControl(parentalControlEnable);
                FilterLevelListView.Visibility = Visibility.Collapsed;
                ChangeCustomSettings.Visibility = Visibility.Collapsed;
                stpOpenDNSAccount.Visibility = Visibility.Collapsed;
                BypassAccount.Visibility = Visibility.Collapsed;
            }
            InProgress.IsActive = false;
            pleasewait.Visibility = Visibility.Collapsed;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private async void Refresh_Click(Object sender, RoutedEventArgs e)
        {
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetEnableStatus();
            ParentalControlInfo.isParentalControlEnabled = dicResponse["ParentalControl"];
            //dicResponse = await soapApi.GetDNSMasqDeviceID("default");
            //ParentalControlInfo.DeviceId = dicResponse["NewDeviceID"];
            this.Frame.Navigate(typeof(ParentalControlPage));
        }

        public async Task<string> ReadBypassChildrenDeviceIdFromFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await storageFolder.GetFileAsync("Bypass_childrenDeviceId.txt");
                if (file != null)
                {
                    fileContent = await FileIO.ReadTextAsync(file);
                }
            }
            catch (FileNotFoundException)
            {

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
                    InProgress.IsActive = false;
                    pleasewait.Visibility = Visibility.Collapsed;
                    var messageDialog = new MessageDialog(dicResponse2["error_message"]);
                    await messageDialog.ShowAsync();
                    bSuccessed = false;
                }                
            }
            return bSuccessed;
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
