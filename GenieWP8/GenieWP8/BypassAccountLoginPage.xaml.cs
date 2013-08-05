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

namespace GenieWP8
{
    public partial class BypassAccountLoginPage : PhoneApplicationPage
    {
        public BypassAccountLoginPage()
        {
            InitializeComponent();

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

        private void LoadState()
        {
            tbBypassUserName.Text = ParentalControlInfo.BypassUsername;
        }

        private void appBarButton_back_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
        }

        //取消按钮事件
        private void CancelButton_Click(object sender, EventArgs e)
        {
            NavigationService.GoBack();
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

            string Username = tbBypassUserName.Text.Trim();
            string Password = tbBypassPassword.Password;
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.GetDeviceChild(ParentalControlInfo.DeviceId, Username, Password);
            if (dicResponse["status"] == "success")
            {
                ParentalControlInfo.BypassUsername = Username;
                ParentalControlInfo.BypassChildrenDeviceId = dicResponse["child_device_id"];
                WriteChildrenDeviceIdToFile();                  //登录成功后将childrenDeviceId保存到本地，如果未注销则以后登录Genie时，通过读取本地DeviceId获得当前登录的Bypass账户
                GenieSoapApi soapApi = new GenieSoapApi();
                dicResponse.Clear();
                UtilityTool util = new UtilityTool();
                string macAddress = util.GetLocalMacAddress();
                macAddress = macAddress.Replace(":", "");       ///本机mac地址
                dicResponse = await soapApi.SetDNSMasqDeviceID(macAddress, ParentalControlInfo.BypassChildrenDeviceId);

                PopupBackground.Visibility = Visibility.Collapsed;
                NavigationService.Navigate(new Uri("/ParentalControlPage.xaml", UriKind.Relative));
            }
            else
            {
                PopupBackground.Visibility = Visibility.Collapsed;
                MessageBox.Show(dicResponse["error_message"]);
            }
        }

        public async void WriteChildrenDeviceIdToFile()
        {
            IsolatedStorageFile fileStorage = IsolatedStorageFile.GetUserStoreForApplication();
            try
            {
                if (!fileStorage.FileExists("Bypass_childrenDeviceId.txt"))
                {
                    using (var file = fileStorage.CreateFile("Bypass_childrenDeviceId.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(ParentalControlInfo.BypassChildrenDeviceId);
                        }
                    }
                }
                else
                {
                    fileStorage.DeleteFile("Bypass_childrenDeviceId.txt");
                    using (var file = fileStorage.CreateFile("Bypass_childrenDeviceId.txt"))
                    {
                        using (var writer = new StreamWriter(file))
                        {
                            await writer.WriteAsync(ParentalControlInfo.BypassChildrenDeviceId);
                        }
                    }
                }
            }
            catch (System.Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}