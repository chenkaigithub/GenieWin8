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
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            InProgress.IsActive = true;
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
                        Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                        dicResponse = await webApi1.GetUserForChildDeviceId(ParentalControlInfo.BypassChildrenDeviceId);
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
                }
                else
                {
                    ParentalControlInfo.IsOpenDNSLoggedIn = false;
                    ParentalControlInfo.IsBypassUserLoggedIn = false;
                    StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
                    try
                    {
                        StorageFile file = await storageFolder.GetFileAsync("Bypass_childrenDeviceId.txt");
                        if (file != null)
                        {
                            await file.DeleteAsync(StorageDeleteOption.PermanentDelete);
                        }  
                    }
                    catch (FileNotFoundException)
                    {

                    }
                }
            }
            

            var FilterLevelGroup = FilterLevelSource.GetFilterLevelGroup((String)navigationParameter);
            this.DefaultViewModel["Group"] = FilterLevelGroup;

            if (!ParentalControlInfo.IsOpenDNSLoggedIn)	//未登录OpenDNS账户
            {
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                if (!EnquirePopup.IsOpen && !RegisterPopup.IsOpen && !LoginPopup.IsOpen)
                {
                    EnquirePopup.IsOpen = true;
                    refreshButton.IsEnabled = false;
                }
            }
            else
            {
                InProgress.IsActive = true;
                pleasewait.Visibility = Visibility.Visible;
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
                refreshButton.IsEnabled = true;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
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
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
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
                    if (dicResponse["error"] == "4012")
                    {
                        PopupEnquireBypassAccount.IsOpen = true;
                    } 
                    else
                    {
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Collapsed;
                        var messageDialog = new MessageDialog(dicResponse["error_message"]);
                        await messageDialog.ShowAsync();
                    }                                       
                } 
            } 
            else                                                                        //已登录Bypass账户
            {
                this.Frame.Navigate(typeof(BypassAccountLogoutPage));
            }                     
        }

        private void CancelButton_Click(Object sender, RoutedEventArgs e)
        {
            if (PopupEnquireBypassAccount.IsOpen)
            {
                PopupEnquireBypassAccount.IsOpen = false;
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
            }
        }

        private async void OkButton_Click(Object sender, RoutedEventArgs e)
        {
            if (PopupEnquireBypassAccount.IsOpen)
            {
                PopupEnquireBypassAccount.IsOpen = false;
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                var uri = new Uri("http://netgear.opendns.com/sign_in.php?");
                await Windows.System.Launcher.LaunchUriAsync(uri);
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
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
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
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
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
            if (dicResponse.Count > 0)
            {
                if (dicResponse["status"] != "success")
                {
                    InProgress1.IsActive = false;
                    IsAvailableName.Visibility = Visibility.Visible;
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
                InProgress1.IsActive = false;
                IsAvailableName.Visibility = Visibility.Visible;
                IsAvailableName.Text = "Check failed!";
                IsAvailableName.Foreground = new SolidColorBrush(Colors.Red);
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
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
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
                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                        PopupBackground.Visibility = Visibility.Visible;
                        refreshButton.IsEnabled = false;
                    }
                }              
            }          
        }

        ////登陆用户名是否为空
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

        ////登陆密码是否为空
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

        //登陆上一步
        private void LoginPreviousButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LoginPopup.IsOpen)
	        {
		        EnquirePopup.IsOpen = true;
		        LoginPopup.IsOpen = false;
                InProgress.IsActive = false;
                pleasewait.Visibility = Visibility.Collapsed;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
                refreshButton.IsEnabled = false;
	        }
        }

        //登陆下一步
        private async void LoginNextButton_Click(Object sender, RoutedEventArgs e)
        {
            if (LoginUsername.Text == "" || LoginPassword.Password == "")
            {
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var messageDialog = new MessageDialog(loader.GetString("EmptyUsernameOrPassword"));
                await messageDialog.ShowAsync();
            }
            else
            {
                InProgress.IsActive = true;
                pleasewait.Visibility = Visibility.Visible;
                //登录OpenDNS账号
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.BeginLogin(LoginUsername.Text, LoginPassword.Password);
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
                                            InProgress.IsActive = false;
                                            pleasewait.Visibility = Visibility.Collapsed;
                                            var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                                            var messageDialog = new MessageDialog(loader.GetString("AnotherUserRegisters"));
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
                                        }
                                        FilterLevelPopup.IsOpen = true;
                                        ParentalControlInfo.IsCategoriesChanged = false;
                                        InProgress.IsActive = false;
                                        pleasewait.Visibility = Visibility.Collapsed;
                                        PopupBackgroundTop.Visibility = Visibility.Collapsed;
                                        PopupBackground.Visibility = Visibility.Visible;
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
                else
                {
                    InProgress.IsActive = false;
                    pleasewait.Visibility = Visibility.Collapsed;
                    var messageDialog = new MessageDialog("Login openDNS failed");
                    await messageDialog.ShowAsync();
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
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Visible;
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
                InProgress3.IsActive = false;
                pleasewait3.Visibility = Visibility.Collapsed;
                var messageDialog = new MessageDialog(dicResponse["error_message"]);
                await messageDialog.ShowAsync();
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
                    InProgress3.IsActive = false;
                    pleasewait3.Visibility = Visibility.Collapsed;
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Visible;
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
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
		        PopupBackground.Visibility = Visibility.Collapsed;
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
                ParentalControlInfo.isParentalControlEnabled = "1";
                FilterLevelListView.Visibility = Visibility.Visible;
                ChangeCustomSettings.Visibility = Visibility.Visible;
                stpOpenDNSAccount.Visibility = Visibility.Visible;
                BypassAccount.Visibility = Visibility.Visible;
            }
            else if (checkPatentalControl.IsChecked == false)
            {
                parentalControlEnable = "0";
                dicResponse = await soapApi.EnableParentalControl(parentalControlEnable);
                ParentalControlInfo.isParentalControlEnabled = "0";
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
            InProgress.IsActive = true;
            pleasewait.Visibility = Visibility.Visible;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;

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
            InProgress.IsActive = false;
            pleasewait.Visibility = Visibility.Collapsed;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
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
                return fileContent;
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

        //注册时按下回车键后默认为点击下一步
        //private void RegOnKeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    TextBox tb = (TextBox)sender;
        //    if (e.Key == Windows.System.VirtualKey.Enter)
        //    {
        //        RegisterNextButton.Focus(FocusState.Keyboard);
        //    }
        //    else
        //    {
        //        base.OnKeyDown(e);
        //    }
        //}

        //登录时按下回车键后默认为点击下一步
        //private void LoginOnKeyDown(object sender, KeyRoutedEventArgs e)
        //{
        //    TextBox tb = (TextBox)sender;
        //    if (e.Key == Windows.System.VirtualKey.Enter)
        //    {
        //        LoginNextButton.Focus(FocusState.Keyboard);
        //    }
        //    else
        //    {
        //        base.OnKeyDown(e);
        //    }
        //}

        //登录OpenDNS账号后保存信息，不切换路由器就无需再次登录
        public async void WriteSavedInfoToFile()
        {
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            StorageFile file = await storageFolder.CreateFileAsync("LoginOpenDNSSavedInfo.txt", CreationCollisionOption.ReplaceExisting);
            try
            {
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, ParentalControlInfo.SavedInfo);
                }
            }
            catch (FileNotFoundException)
            {

            }
        }

        //读取保存的信息
        public async Task<string> ReadSavedInfoFromFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = ApplicationData.Current.LocalFolder;
            try
            {
                StorageFile file = await storageFolder.GetFileAsync("LoginOpenDNSSavedInfo.txt");
                if (file != null)
                {
                    fileContent = await FileIO.ReadTextAsync(file);
                }
            }
            catch (FileNotFoundException)
            {
                return fileContent;
            }
            return fileContent;
        }
    }
}
