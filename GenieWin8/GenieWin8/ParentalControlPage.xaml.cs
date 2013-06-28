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
			        NoButton.Visibility = Visibility.Visible;
			        YesButton.Visibility = Visibility.Visible;
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
            var FilterLevelGroup = FilterLevelSource.GetGroup((String)navigationParameter);
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

            if (ParentalControlInfo.BypassUsername != null)
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
            PopupFilterLevel __popupFilterLevel = new PopupFilterLevel();
            if (!FilterLevelPopup.IsOpen)
	        {
		        FilterLevelPopup.IsOpen = true;
                InProgress.IsActive = false;
		        PopupBackground.Visibility = Visibility.Visible;
		        FilterLvPreviousButton.Visibility = Visibility.Visible;
		        FilterLvNextButton.Visibility = Visibility.Visible;
	        }
        }

        private async void ChangeSetting_ItemClick(Object sender, ItemClickEventArgs e)
        {
            InProgress.IsActive = true;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.AccountRelay(ParentalControlInfo.token);
            if (dicResponse["status"] == "success")
            {
                InProgress.IsActive = false;
                PopupBackgroundTop.Visibility = Visibility.Collapsed;
                PopupBackground.Visibility = Visibility.Collapsed;
                string relay_token = dicResponse["relay_token"];
                var uri = new Uri("http://netgear.opendns.com/account.php?device_id=" + ParentalControlInfo.DeviceId + "&api_key=3D8C85A77ADA886B967984DF1F8B3711" + "&relay_token=" + relay_token);
                await Windows.System.Launcher.LaunchUriAsync(uri);
            }
            else
            {
                InProgress.IsActive = false;
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
                    PopupBackgroundTop.Visibility = Visibility.Collapsed;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    this.Frame.Navigate(typeof(BypassAccountPage));
                }
                else
                {
                    InProgress.IsActive = false;
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
		        PopupBackground.Visibility = Visibility.Visible;
		        NoButton.Visibility = Visibility.Collapsed;
		        YesButton.Visibility = Visibility.Collapsed;
		        RegisterPreviousButton.Visibility = Visibility.Visible;
		        RegisterNextButton.Visibility = Visibility.Visible;
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
		        PopupBackground.Visibility = Visibility.Visible;
		        NoButton.Visibility = Visibility.Collapsed;
		        YesButton.Visibility = Visibility.Collapsed;
		        LoginPreviousButton.Visibility = Visibility.Visible;
		        LoginNextButton.Visibility = Visibility.Visible;
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
		        PopupBackground.Visibility = Visibility.Visible;
		        NoButton.Visibility = Visibility.Visible;
		        YesButton.Visibility = Visibility.Visible;
		        RegisterPreviousButton.Visibility = Visibility.Collapsed;
		        RegisterNextButton.Visibility = Visibility.Collapsed;
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
                InProgress.IsActive = true;
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.CreateAccount(ParentalControlInfo.Username, ParentalControlInfo.Password, ParentalControlInfo.Email);
                if (dicResponse["status"] != "success")
                {
                    InProgress.IsActive = false;
                    var messageDialog = new MessageDialog(dicResponse["error_message"]);
                    await messageDialog.ShowAsync();
                } 
                else
                {
                    if (RegisterPopup.IsOpen)
                    {
                        RegisterPopup.IsOpen = false;
                        LoginPopup.IsOpen = true;
                        InProgress.IsActive = false;
                        PopupBackground.Visibility = Visibility.Visible;
                        RegisterPreviousButton.Visibility = Visibility.Collapsed;
                        RegisterNextButton.Visibility = Visibility.Collapsed;
                        LoginPreviousButton.Visibility = Visibility.Visible;
                        LoginNextButton.Visibility = Visibility.Visible;
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
                InProgress.IsActive = false;
		        PopupBackground.Visibility = Visibility.Visible;
		        NoButton.Visibility = Visibility.Visible;
		        YesButton.Visibility = Visibility.Visible;
		        LoginPreviousButton.Visibility = Visibility.Collapsed;
		        LoginNextButton.Visibility = Visibility.Collapsed;
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
                GenieWebApi webApi = new GenieWebApi();
                Dictionary<string, string> dicResponse = new Dictionary<string, string>();
                dicResponse = await webApi.BeginLogin(ParentalControlInfo.Username, ParentalControlInfo.Password);
                if (dicResponse.Count > 0)
                {
                    if (dicResponse["status"] != "success")
                    {
                        InProgress.IsActive = false;
                        var messageDialog = new MessageDialog(dicResponse["error_message"]);
                        await messageDialog.ShowAsync();
                    }
                    else
                    {
                        ParentalControlInfo.IsOpenDNSLoggedIn = true;
                        ParentalControlInfo.token = dicResponse["token"];
                        Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                        dicResponse2 = await webApi.GetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId);
                        if (dicResponse2["status"] != "success")
                        {
                            ParentalControlInfo.filterLevel = "";
                        }
                        else
                        {
                            ParentalControlInfo.filterLevel = dicResponse2["bundle"];
                        }
                        PopupFilterLevel __popupFilterLevel = new PopupFilterLevel();       //再次初始化 PopupFilterLevel，标识出过滤等级

                        if (LoginPopup.IsOpen)
                        {                            
                            LoginPopup.IsOpen = false;
                            FilterLevelPopup.IsOpen = true;
                            InProgress.IsActive = false;
                            PopupBackground.Visibility = Visibility.Visible;
                            LoginPreviousButton.Visibility = Visibility.Collapsed;
                            LoginNextButton.Visibility = Visibility.Collapsed;
                            FilterLvPreviousButton.Visibility = Visibility.Visible;
                            FilterLvNextButton.Visibility = Visibility.Visible;
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
		        PopupBackground.Visibility = Visibility.Visible;
		        LoginPreviousButton.Visibility = Visibility.Visible;
		        LoginNextButton.Visibility = Visibility.Visible;
		        FilterLvPreviousButton.Visibility = Visibility.Collapsed;
		        FilterLvNextButton.Visibility = Visibility.Collapsed;
	        }
        }

        //设置过滤等级下一步
        private async void FilterLvNextButton_Click(Object sender, RoutedEventArgs e)
        {
            InProgress.IsActive = true;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.SetFilters(ParentalControlInfo.token, ParentalControlInfo.DeviceId, ParentalControlInfo.filterLevel);
            if (dicResponse["status"] != "success")
            {
                InProgress.IsActive = false;
                var messageDialog = new MessageDialog(dicResponse["error_message"]);
                await messageDialog.ShowAsync();
            } 
            else
            {
                if (FilterLevelPopup.IsOpen)
                {
                    FilterLevelPopup.IsOpen = false;
                    SettingCompletePopup.IsOpen = true;
                    InProgress.IsActive = false;
                    PopupBackground.Visibility = Visibility.Visible;
                    FilterLvPreviousButton.Visibility = Visibility.Collapsed;
                    FilterLvNextButton.Visibility = Visibility.Collapsed;
                    ReturnToStatusButton.Visibility = Visibility.Visible;
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
		        PopupBackground.Visibility = Visibility.Collapsed;
		        ReturnToStatusButton.Visibility = Visibility.Collapsed;
                this.Frame.Navigate(typeof(ParentalControlPage));
	        }
        }

        private async void checkParentalControl_Click(Object sender, RoutedEventArgs e)
        {
            InProgress.IsActive = true;
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
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
        }

        private async void Refresh_Click(Object sender, RoutedEventArgs e)
        {
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetEnableStatus();
            ParentalControlInfo.isParentalControlEnabled = dicResponse["ParentalControl"];
            dicResponse = await soapApi.GetDNSMasqDeviceID("default");
            ParentalControlInfo.DeviceId = dicResponse["NewDeviceID"];
            this.Frame.Navigate(typeof(ParentalControlPage));
        }

        public async Task<string> ReadBypassChildrenDeviceIdFromFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
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
    }
}
