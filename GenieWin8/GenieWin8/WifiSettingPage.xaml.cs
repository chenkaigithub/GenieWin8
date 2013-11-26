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

using Windows.UI.Xaml.Media.Imaging;
using ZXing;
using ZXing.Common;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

namespace GenieWin8
{
    /// <summary>
    /// 基本页，提供大多数应用程序通用的特性。
    /// </summary>
    public sealed partial class WifiSettingPage : GenieWin8.Common.LayoutAwarePage
    {
        public WifiSettingPage()
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
        protected override void LoadState(Object navigationParameter, Dictionary<String, Object> pageState)
        {
            var SettingGroup = SettingSource.GetGroups((String)navigationParameter);
            this.DefaultViewModel["Groups"] = SettingGroup;
            var signalStrengthGroup = SettingSource.GetSignalStrengthGroup((String)navigationParameter);
            this.DefaultViewModel["itemSignalStrength"] = signalStrengthGroup;
            var linkRateGroup = SettingSource.GetLinkRateGroup((String)navigationParameter);
            this.DefaultViewModel["itemLinkRate"] = linkRateGroup;

            if (signalStrengthGroup.ElementAt(0).Content == null)
            {
                stpSignal.Visibility = Visibility.Collapsed;
            } 
            else
            {
                stpSignal.Visibility = Visibility.Visible;
            }
            if (linkRateGroup.ElementAt(0).Content == null)
            {
                stpLinkRate.Visibility = Visibility.Collapsed;
            } 
            else
            {
                stpLinkRate.Visibility = Visibility.Visible;
            }

            //生成二维码
            string codeString = "WIRELESS:" + WifiInfoModel.ssid + ";PASSWORD:" + WifiInfoModel.password;
            WriteableBitmap wb = CreateBarcode(codeString);
            imageQRCode.Source = wb;
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

        private void Setting_ItemClick(Object sender, ItemClickEventArgs e)
        {
            this.Frame.Navigate(typeof(EditSettingPage));
        }

        private async void Refresh_Click(Object sender, RoutedEventArgs e)
        {
            InProgress.IsActive = true;
            PopupBackgroundTop.Visibility = Visibility.Visible;
            PopupBackground.Visibility = Visibility.Visible;
            GenieSoapApi soapApi = new GenieSoapApi();

            Dictionary<string, Dictionary<string, string>> attachDeviceAll = new Dictionary<string, Dictionary<string, string>>();
            attachDeviceAll = await soapApi.GetAttachDevice();
            UtilityTool util = new UtilityTool();
            string loacalIp = util.GetLocalHostIp();
            if (attachDeviceAll.Count == 0)
            {
                WifiInfoModel.linkRate = "";
                WifiInfoModel.signalStrength = "";
            } 
            else
            {
                foreach (string key in attachDeviceAll.Keys)
                {
                    if (loacalIp == attachDeviceAll[key]["Ip"])
                    {
                        if (attachDeviceAll[key].ContainsKey("LinkSpeed"))
                        {
                            WifiInfoModel.linkRate = attachDeviceAll[key]["LinkSpeed"] + "Mbps";
                        }
                        else
                        {
                            WifiInfoModel.linkRate = "";
                        }
                        if (attachDeviceAll[key].ContainsKey("Signal"))
                        {
                            WifiInfoModel.signalStrength = attachDeviceAll[key]["Signal"] + "%";
                        }
                        else
                        {
                            WifiInfoModel.signalStrength = "";
                        }
                    }
                }
            }

            Dictionary<string, string> dicResponse = new Dictionary<string, string>();
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.GetInfo("WLANConfiguration");
            }
            if (dicResponse.Count > 0)
            {
                WifiInfoModel.ssid = dicResponse["NewSSID"];
                WifiInfoModel.changedSsid = dicResponse["NewSSID"];
                WifiInfoModel.channel = dicResponse["NewChannel"];
                WifiInfoModel.changedChannel = dicResponse["NewChannel"];
                WifiInfoModel.securityType = dicResponse["NewWPAEncryptionModes"];
                WifiInfoModel.changedSecurityType = dicResponse["NewWPAEncryptionModes"];
            }
            dicResponse = new Dictionary<string, string>();
            while (dicResponse == null || dicResponse.Count == 0)
            {
                dicResponse = await soapApi.GetWPASecurityKeys();
            }
            if (dicResponse.Count > 0)
            {
                WifiInfoModel.password = dicResponse["NewWPAPassphrase"];
                WifiInfoModel.changedPassword = dicResponse["NewWPAPassphrase"];
            }
            InProgress.IsActive = false;
            PopupBackgroundTop.Visibility = Visibility.Collapsed;
            PopupBackground.Visibility = Visibility.Collapsed;
            this.Frame.Navigate(typeof(WifiSettingPage));
        }

        //创建二维码函数
        public static WriteableBitmap CreateBarcode(string content)
        {
            IBarcodeWriter wt = new BarcodeWriter
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Height = 200,
                    Width = 200
                }
            };
            var bmp = wt.Write(content);
            return bmp;
        }
    }
}
