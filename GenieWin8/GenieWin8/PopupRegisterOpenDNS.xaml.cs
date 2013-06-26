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
using Windows.UI.Popups;
using GenieWin8.DataModel;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

namespace GenieWin8
{
    public sealed partial class PopupRegisterOpenDNS : UserControl
    {
        public PopupRegisterOpenDNS()
        {
            this.InitializeComponent();
        }

        private async void CheckAvailable_Click(Object sender, RoutedEventArgs e)
        {
            GenieWebApi webApi = new GenieWebApi();
            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            dicResponse = await webApi.CheckNameAvailable(username.Text);
            if (dicResponse["status"] != "success")
            {
                UnavailableName.Text = dicResponse["error_message"];
                UnavailableName.Visibility = Visibility.Visible;
                ParentalControlInfo.IsUsernameAvailable = false;
            }
            else
            {
                UnavailableName.Visibility = Visibility.Collapsed;
                ParentalControlInfo.IsUsernameAvailable = true;
            }
        }

        private void IsBlankUsername(Object sender, RoutedEventArgs e)
        {
            if (username.Text == "")
            {
                checkNameAvailable.IsEnabled = false;
                ParentalControlInfo.IsEmptyUsername = true;
            }
            else
            {
                ParentalControlInfo.Username = username.Text;
                checkNameAvailable.IsEnabled = true;
                ParentalControlInfo.IsEmptyUsername = false;
            }
        }

        private void IsBlankPassword(Object sender, RoutedEventArgs e)
        {
            if (password.Password == "")
            {
                ParentalControlInfo.IsEmptyPassword = true;
            }
            else
            {
                ParentalControlInfo.Password = password.Password;
                ParentalControlInfo.IsEmptyPassword = false;
            }
        }

        private void eventConfirmPassword(Object sender, RoutedEventArgs e)
        {
            if (password.Password != confirmPassword.Password)
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
    }
}
