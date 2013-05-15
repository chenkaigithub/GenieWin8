//
// ParentalControlPage.xaml.cpp
// ParentalControlPage 类的实现
//

#include "pch.h"
#include "ParentalControlPage.xaml.h"

using namespace NETGEAR_Genie_Win8;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::UI::Xaml::Interop;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

ParentalControlPage::ParentalControlPage()
{
	InitializeComponent();

	if (0)	//未登录OpenDNS账户
	{
		if (!EnquirePopup->IsOpen)
		{
			EnquirePopup->IsOpen = true;
			//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
			PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
			NoButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
			YesButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		}
	}
}

/// <summary>
/// 使用在导航过程中传递的内容填充页。在从以前的会话
/// 重新创建页时，也会提供任何已保存状态。
/// </summary>
/// <param name="navigationParameter">最初请求此页时传递给
/// <see cref="Frame::Navigate(Type, Object)"/> 的参数值。
/// </param>
/// <param name="pageState">此页面在之前的会话期间保留的状态
///映射。首次访问页面时为 null。</param>
void ParentalControlPage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
	(void) pageState;	// 未使用的参数 

	auto FilterLevelGroup = Data::FilterLevelSource::GetGroup(safe_cast<String^>(navigationParameter));
	DefaultViewModel->Insert("Group", FilterLevelGroup);
}

/// <summary>
/// 保留与此页关联的状态，以防挂起应用程序或
/// 从导航缓存中放弃此页。值必须符合
/// <see cref="SuspensionManager::SessionState"/> 的序列化要求。
/// </summary>
/// <param name="pageState">要使用可序列化状态填充的空映射。</param>
void ParentalControlPage::SaveState(IMap<String^, Object^>^ pageState)
{
	(void) pageState;	// 未使用的参数
}

void ParentalControlPage::FilterLevel_ItemClick(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	//Frame->Navigate(TypeName(FilterLevelPage::typeid));
	if (!FilterLevelPopup->IsOpen)
	{
		FilterLevelPopup->IsOpen = true;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		FilterLvPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		FilterLvNextButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
}

void ParentalControlPage::ChangeSetting_ItemClick(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	auto uri = ref new Uri("http://netgear.opendns.com/account.php?device_id=0000DF346BC636E0");
	Windows::System::Launcher::LaunchUriAsync(uri);
}

void ParentalControlPage::Bypass_ItemClick(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数
}

void ParentalControlPage::NoButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (EnquirePopup->IsOpen)
	{
		EnquirePopup->IsOpen = false;
		RegisterPopup->IsOpen = true;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		NoButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		YesButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		RegisterPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		RegisterNextButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
}

void ParentalControlPage::YesButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (EnquirePopup->IsOpen)
	{
		EnquirePopup->IsOpen = false;
		LoginPopup->IsOpen = true;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		NoButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		YesButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		LoginPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		LoginNextButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
}

void ParentalControlPage::RegisterPreviousButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (RegisterPopup->IsOpen)
	{
		EnquirePopup->IsOpen = true;
		RegisterPopup->IsOpen = false;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		NoButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		YesButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		RegisterPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		RegisterNextButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
}

void ParentalControlPage::RegisterNextButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (RegisterPopup->IsOpen)
	{		
		RegisterPopup->IsOpen = false;
		LoginPopup->IsOpen = true;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		RegisterPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		RegisterNextButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		LoginPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		LoginNextButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
}

void ParentalControlPage::LoginPreviousButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (LoginPopup->IsOpen)
	{
		EnquirePopup->IsOpen = true;
		LoginPopup->IsOpen = false;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		NoButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		YesButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		LoginPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		LoginNextButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
}

void ParentalControlPage::LoginNextButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (LoginPopup->IsOpen)
	{
		LoginPopup->IsOpen = false;
		FilterLevelPopup->IsOpen = true;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		LoginPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		LoginNextButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		FilterLvPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		FilterLvNextButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
}

void ParentalControlPage::FilterLvPreviousButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (FilterLevelPopup->IsOpen)
	{
		LoginPopup->IsOpen = true;
		FilterLevelPopup->IsOpen = false;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		LoginPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		LoginNextButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
		FilterLvPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		FilterLvNextButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
	}
}

void ParentalControlPage::FilterLvNextButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (FilterLevelPopup->IsOpen)
	{
		FilterLevelPopup->IsOpen = false;
		SettingCompletePopup->IsOpen = true;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Visible;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Visible;
		FilterLvPreviousButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		FilterLvNextButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		ReturnToStatusButton->Visibility = Windows::UI::Xaml::Visibility::Visible;
	}
}

void ParentalControlPage::ReturnToStatusButton_Click(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;		// 未使用的参数

	if (SettingCompletePopup->IsOpen)
	{
		SettingCompletePopup->IsOpen = false;
		//PopupBackgroundTop->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		PopupBackground->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		ReturnToStatusButton->Visibility = Windows::UI::Xaml::Visibility::Collapsed;
		Frame->Navigate(TypeName(ParentalControlPage::typeid));
	}
}