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
using GenieWP8.DataInfo;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading.Tasks;

namespace GenieWP8
{
    public partial class LoginPage : PhoneApplicationPage
    {
        public LoginPage()
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

            // 用于本地化 ApplicationBar 的代码
            BuildLocalizedApplicationBar();

            //加载页面状态
            LoadState();
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
        }

        private async void LoadState()
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

        private void PhoneApplicationPage_OrientationChanged(Object sender, OrientationChangedEventArgs e)
        {
            if ((e.Orientation & PageOrientation.Portrait) == (PageOrientation.Portrait))
            {
                PageTitle.Width = Application.Current.Host.Content.ActualWidth - 20;
            }
            // If not in portrait, move buttonList content to visible row and column.
            else
            {
                PageTitle.Width = Application.Current.Host.Content.ActualHeight - 150;
            }
        }

        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //重写手机“返回”按钮事件
        protected override void OnBackKeyPress(System.ComponentModel.CancelEventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //取消按钮事件
        private void CancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/MainPage.xaml", UriKind.Relative));
        }

        //按下屏幕键盘回车键后关闭屏幕键盘
        protected override void OnKeyDown(System.Windows.Input.KeyEventArgs e)
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

        //登录按钮事件
        private async void LoginButton_Click(object sender, EventArgs e)
        {
            PopupBackground.Visibility = Visibility.Visible;          

            string Username = tbUserName.Text.Trim();
            string Password = tbPassword.Password;
            if (checkRememberPassword.IsChecked == true)
            {
                MainPageInfo.rememberpassword = Password;
            }
            else
            {
                MainPageInfo.rememberpassword = "";
            }
            WritePasswordToFile();

            GenieSoapApi soapApi = new GenieSoapApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await soapApi.GetCurrentSetting();
            if (dicResponse.Count > 0 && dicResponse["Firmware"] != "" && dicResponse["Model"] != "")
            {
                MainPageInfo.model = dicResponse["Model"];
                Dictionary<string, string> dicResponse2 = new Dictionary<string, string>();
                dicResponse2 = await soapApi.Authenticate(Username, Password);
                if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 0)
                {
                    MainPageInfo.bLogin = true;
                    MainPageInfo.username = Username;
                    MainPageInfo.password = Password;                                       
                    PopupBackground.Visibility = Visibility.Collapsed;                  
                    NavigationService.GoBack();
                }
                else if (dicResponse2.Count > 0 && int.Parse(dicResponse2["ResponseCode"]) == 401)
                {
                    PopupBackground.Visibility = Visibility.Collapsed;
                    var strtext = AppResources.bad_password;
                    MessageBox.Show(strtext);
                }
            }
            else
            {
                PopupBackground.Visibility = Visibility.Collapsed;
                var strtext = AppResources.login_alertinfo;
                MessageBox.Show(strtext);
            }
        }

        public async void WritePasswordToFile()
        {
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (!fileStorage.FileExists("Password.txt"))
                {
                    using (var file = fileStorage.CreateFile("Password.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(MainPageInfo.rememberpassword);
                        }
                    }
                }
                else
                {
                    fileStorage.DeleteFile("Password.txt");
                    using (var file = fileStorage.CreateFile("Password.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(MainPageInfo.rememberpassword);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        public async Task<string> ReadPasswordFromFile()
        {
            string fileContent = string.Empty;
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (fileStorage.FileExists("Password.txt"))
                {
                    using (var file = fileStorage.OpenFile("Password.txt",FileMode.Open,FileAccess.Read))
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
    }
}