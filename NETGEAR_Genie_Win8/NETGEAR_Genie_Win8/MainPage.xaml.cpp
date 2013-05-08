//
// MainPage.xaml.cpp
// MainPage 类的实现
//

#include "pch.h"
#include "DataModel/MainPageModel.h"
#include "MainPage.xaml.h"

using namespace NETGEAR_Genie_Win8;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Popups;
using namespace concurrency;

// “项目页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234233 上提供

MainPage::MainPage()
{
	InitializeComponent();
}

/// <summary>
/// 使用在导航过程中传递的内容填充页。在从以前的会话
/// 重新创建页时，也会提供任何已保存状态。
/// </summary>
/// <param name="navigationParameter">最初请求此页时传递给
/// <see cref="Frame::Navigate(Type, Object)"/> 的参数值。
/// </param>
/// <param name="pageState">此页面在之前的会话期间保留的状态
/// 字典。首次访问页面时为 null。</param>
void MainPage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
	(void) pageState;	// 未使用的参数

	// TODO: 创建适用于问题域的合适数据模型以替换示例数据
	auto dataGroups = Data::DataSource::GetGroups(safe_cast<String^>(navigationParameter));
	DefaultViewModel->Insert("Items", dataGroups);
}

/// <summary>
/// 在单击某个项时进行调用。
/// </summary>
/// <param name="sender">显示所单击项的 GridView (在应用程序处于对齐状态时
/// 为 ListView)。</param>
/// <param name="e">描述所单击项的事件数据。</param>
void MainPage::ItemView_ItemClick(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数

	// 导航至相应的目标页，并
	// 通过将所需信息作为导航参数传入来配置新页
	if (1)	//已登陆
	{
		auto groupId = safe_cast<Data::DataGroup^>(e->ClickedItem)->UniqueId;
		if (groupId == "WiFiSetting")
		{
			Frame->Navigate(TypeName(WifiSettingPage::typeid));
		}
		else if (groupId == "GuestAccess")
		{
			Frame->Navigate(TypeName(GuestAccessPage::typeid));
		}
		else if (groupId == "TrafficControl")
		{
			Frame->Navigate(TypeName(TrafficControlPage::typeid));
		}
		else if (groupId == "ParentalControl")
		{
			//if (0)	//已登陆OpenDNS账户
			//{
				Frame->Navigate(TypeName(ParentalControlPage::typeid));
			//} 
			//else    //未登录
			//{
			//	auto messageDialog = ref new MessageDialog("要设置实时家长控制，您需要拥有OpenDNS账户。已经有账户了吗？");

			//	messageDialog->Commands->Append(ref new UICommand("否", nullptr, PropertyValue::CreateInt32(0)));
			//	messageDialog->Commands->Append(ref new UICommand("是", nullptr, PropertyValue::CreateInt32(1)));

			//	// Show the message dialog and retrieve the id of the chosen command
			//	create_task(messageDialog->ShowAsync()).then([this](IUICommand^ command)
			//	{
			//		if (command->Label == "否")
			//		{
			//			this->Frame->Navigate(TypeName(RegisterOpenDNSPage::typeid));
			//		}
			//		else if (command->Label == "是")
			//		{
			//			this->Frame->Navigate(TypeName(LoginOpenDNSPage::typeid));
			//		}
			//	});
			//}			
		}
	} 
	else	//未登录，跳到登陆页面
	{
		Frame->Navigate(TypeName(LoginPage::typeid));
	}
}

void MainPage::SearchButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数
}

void MainPage::LoginButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	Frame->Navigate(TypeName(LoginPage::typeid));
}

void MainPage::LogoutButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数
}

void MainPage::AboutButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (!AboutPopup->IsOpen)
	{
		AboutPopup->IsOpen = true;
		PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		CloseAboutButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		LicenseButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
	if (LicensePopup->IsOpen)
	{
		LicensePopup->IsOpen = false;
		CloseLicenseButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
}

void MainPage::CloseAboutButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (AboutPopup->IsOpen)
	{
		AboutPopup->IsOpen = false;
		PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		CloseAboutButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		LicenseButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
}

void MainPage::LicenseButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (!LicensePopup->IsOpen)
	{
		LicensePopup->IsOpen = true;
		AboutPopup->IsOpen = false;
		PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		CloseLicenseButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		CloseAboutButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		LicenseButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
}

void MainPage::CloseLicenseButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (LicensePopup->IsOpen)
	{
		LicensePopup->IsOpen = false;
		PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		CloseLicenseButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
}