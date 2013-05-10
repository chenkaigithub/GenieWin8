﻿//
// MyMediaPage.xaml.cpp
// MyMediaPage 类的实现
//

#include "pch.h"
#include "MyMediaPage.xaml.h"
#include "MyMediaSourcePage.xaml.h"

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

MyMediaPage::MyMediaPage()
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
///映射。首次访问页面时为 null。</param>
void MyMediaPage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
	(void) pageState;	// 未使用的参数

	auto mymediaGroups = Data::MediaSource::GetMymediaGroups(safe_cast<String^>(navigationParameter));
	DefaultViewModel->Insert("mymediaGroup", mymediaGroups);
}

/// <summary>
/// 保留与此页关联的状态，以防挂起应用程序或
/// 从导航缓存中放弃此页。值必须符合
/// <see cref="SuspensionManager::SessionState"/> 的序列化要求。
/// </summary>
/// <param name="pageState">要使用可序列化状态填充的空映射。</param>
void MyMediaPage::SaveState(IMap<String^, Object^>^ pageState)
{
	(void) pageState;	// 未使用的参数
}

void MyMediaPage::MyMedia_ItemClick(Object^ sender, ItemClickEventArgs^ e)
{
	(void) sender;	// 未使用的参数

	auto groupId = safe_cast<Data::MyMediaGroup^>(e->ClickedItem)->UniqueId;
	if (groupId == "MyMediaSource")
	{
		Frame->Navigate(TypeName(MyMediaSourcePage::typeid));
	}
	else if (groupId == "MyMediaPlayer")
	{
		//Frame->Navigate(TypeName(GuestAccessPage::typeid));
	}
	else if (groupId == "MyMediaPlaying")
	{
		//Frame->Navigate(TypeName(TrafficControlPage::typeid));
	}
	else if (groupId == "MyMediaOption")
	{
		//Frame->Navigate(TypeName(ParentalControlPage::typeid));		
	} 
}