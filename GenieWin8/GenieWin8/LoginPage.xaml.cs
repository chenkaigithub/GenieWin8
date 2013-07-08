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
using Windows.Storage;
using System.Threading.Tasks;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class LoginPage : GenieWin8.Common.LayoutAwarePage
    {
        public LoginPage()
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
            MainPageInfo.rememberpassword = await ReadPasswordFromFile();
            if (MainPageInfo.rememberpassword == "")
            {
                checkRememberPassword.IsChecked = false;
            } 
            else
            {
                checkRememberPassword.IsChecked = true;
            }
            tbPassword.Password = MainPageInfo.rememberpassword;
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

        private async void LoginButton_Click(Object sender, RoutedEventArgs e)
        {
            InProgress.IsActive = true;
            PopupBackground.Visibility = Visibility.Visible;

            string Username = tbUserName.Text.Trim();
            string Password = tbPassword.Password;
            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetCurrentSetting();
            if (dicResponse.Count > 0 && dicResponse["Firmware"] != "" && dicResponse["Model"] != "")
            {
                Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                dicResponse2 = await soapApi.Authenticate(Username, Password);
                if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 0)
                {
                    MainPageInfo.bLogin = true;
                    MainPageInfo.username = Username;
                    MainPageInfo.password = Password;
                    if (checkRememberPassword.IsChecked == true)
                    {
                        MainPageInfo.rememberpassword = Password;
                    }
                    else
                    {
                        MainPageInfo.rememberpassword = "";                       
                    }
                    InProgress.IsActive = false;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    WritePasswordToFile();
                    this.Frame.GoBack();
                }
                else if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 401)
                {
                    InProgress.IsActive = false;
                    PopupBackground.Visibility = Visibility.Collapsed;
                    var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                    var strtext = loader.GetString("bad_password");
                    var messageDialog = new MessageDialog(strtext);
                    await messageDialog.ShowAsync();
                }
            }
            else
            {
                InProgress.IsActive = false;
                PopupBackground.Visibility = Visibility.Collapsed;
                var loader = new Windows.ApplicationModel.Resources.ResourceLoader();
                var strtext = loader.GetString("login_alertinfo");
                var messageDialog = new MessageDialog(strtext);
                await messageDialog.ShowAsync();
            }
        }

        public async void WritePasswordToFile()
        {
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            StorageFile file = await storageFolder.CreateFileAsync("Password.txt", CreationCollisionOption.ReplaceExisting);
            try
            {
                if (file != null)
                {
                    await FileIO.WriteTextAsync(file, MainPageInfo.rememberpassword);
                }
            }
            catch (FileNotFoundException)
            {

            }
        }

        public async Task<string> ReadPasswordFromFile()
        {
            string fileContent = string.Empty;
            StorageFolder storageFolder = KnownFolders.DocumentsLibrary;
            try
            {
                StorageFile file = await storageFolder.GetFileAsync("Password.txt");
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
