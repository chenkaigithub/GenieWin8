#include "pch.h"
#include "ViewModels\WifiSettingModel.h"

using namespace NETGEAR_Genie_Win8::Data;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::ApplicationModel::Resources::Core;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Media::Imaging;

//
// SettingCommon
//

SettingCommon::SettingCommon(String^ uniqueId, String^ title, String^ content)
{
	_uniqueId = uniqueId;
	_title = title;
	_content = content;
}

String^ SettingCommon::UniqueId::get()
{
	return _uniqueId;
}

void SettingCommon::UniqueId::set(String^ value)
{
	if (_uniqueId != value)
	{
		_uniqueId = value;
		OnPropertyChanged("UniqueId");
	}
}

String^ SettingCommon::Title::get()
{
	return _title;
}

void SettingCommon::Title::set(String^ value)
{
	if (_title != value)
	{
		_title = value;
		OnPropertyChanged("Title");
	}
}

String^ SettingCommon::Content::get()
{
	return _content;
}

void SettingCommon::Content::set(String^ value)
{
	if (_content != value)
	{
		_content = value;
		OnPropertyChanged("Content");
	}
}

//
// SettingItem
//

SettingItem::SettingItem(String^ uniqueId, String^ title, String^ content, SettingGroup^ group)
						 : SettingCommon(uniqueId, title, content)
{
	_group = group;
}

SettingGroup^ SettingItem::Group::get()
{
	return _group.Resolve<SettingGroup>();
}

void SettingItem::Group::set(SettingGroup^ value)
{
	if (Group != value)
	{
		_group = value;
		OnPropertyChanged("Group");
	}
}

//
// SettingGroup
//

SettingGroup::SettingGroup(String^ uniqueId, String^ title, String^ content)
	: SettingCommon(uniqueId, title, content)
{
	_items = ref new Vector<SettingItem^>();
}

IObservableVector<SettingItem^>^ SettingGroup::Items::get()
{
	return _items;
}

//
// SettingSource
//

SettingSource::SettingSource()
{
	_settingGroups = ref new Vector<SettingGroup^>();
	_editName = ref new Vector<SettingGroup^>();
	_editKey = ref new Vector<SettingGroup^>();
	_editChannelSecurity = ref new Vector<SettingGroup^>();
	auto loader = ref new Windows::ApplicationModel::Resources::ResourceLoader();

	auto strTitle = loader->GetString("WiFiName");
	auto group1 = ref new SettingGroup("WiFiName",
		strTitle,
		"wifiname");
	_editName->Append(group1);
	_settingGroups->Append(group1);

	strTitle = loader->GetString("Key/Password");
	auto group2 = ref new SettingGroup("Password",
		strTitle,
		"password");
	_editKey->Append(group2);
	_settingGroups->Append(group2);

	strTitle = loader->GetString("Channel");
	auto group3 = ref new SettingGroup("Channel",
		strTitle,
		"Auto");
	group3->Items->Append(ref new SettingItem("Channel-1",
		"Channel",
		"Auto",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-2",
		"Channel",
		"1",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-3",
		"Channel",
		"2",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-4",
		"Channel",
		"3",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-5",
		"Channel",
		"4",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-6",
		"Channel",
		"5",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-7",
		"Channel",
		"6",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-8",
		"Channel",
		"7",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-9",
		"Channel",
		"8",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-10",
		"Channel",
		"9",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-11",
		"Channel",
		"10",
		group3));
	group3->Items->Append(ref new SettingItem("Channel-12",
		"Channel",
		"11",
		group3));
	_editChannelSecurity->Append(group3);
	_settingGroups->Append(group3);

	strTitle = loader->GetString("Security");
	auto group4 = ref new SettingGroup("Security",
		strTitle,
		"WPA2-PSK[AES]");
	auto strContent = loader->GetString("Security_None");
	group4->Items->Append(ref new SettingItem("Security-1",
		"Security",
		strContent,
		group4));
	strContent = loader->GetString("Security_WPA2-PSK[AES]");
	group4->Items->Append(ref new SettingItem("Security-2",
		"Security",
		strContent,
		group4));
	strContent = loader->GetString("Security_WPA-PSK+WPA2-PSK");
	group4->Items->Append(ref new SettingItem("Security-3",
		"Security",
		strContent,
		group4));
	_editChannelSecurity->Append(group4);
}

IObservableVector<SettingGroup^>^ SettingSource::SettingGroups::get()
{
	return _settingGroups;
}

IObservableVector<SettingGroup^>^ SettingSource::EditName::get()
{
	return _editName;
}

IObservableVector<SettingGroup^>^ SettingSource::EditKey::get()
{
	return _editKey;
}

IObservableVector<SettingGroup^>^ SettingSource::EditChannelSecurity::get()
{
	return _editChannelSecurity;
}

static SettingSource^ _settingSource = nullptr;

void SettingSource::Init()
{
	if (_settingSource == nullptr)
	{
		_settingSource = ref new SettingSource();
	}
}

IIterable<SettingGroup^>^ SettingSource::GetGroups(String^ uniqueId)
{
	Init();
	return _settingSource->SettingGroups;
}

IIterable<SettingGroup^>^ SettingSource::GetEditName(String^ uniqueId)
{
	Init();
	return _settingSource->EditName;
}

IIterable<SettingGroup^>^ SettingSource::GetEditKey(String^ uniqueId)
{
	Init();
	return _settingSource->EditKey;
}

IIterable<SettingGroup^>^ SettingSource::GetChannelSecurity(String^ uniqueId)
{
	Init();
	return _settingSource->EditChannelSecurity;
}

SettingGroup^ SettingSource::GetChannel(String^ uniqueId)
{
	Init();
	for each (auto group in _settingSource->EditChannelSecurity)
	{
		if (group->UniqueId == "Channel") return group;
	}
	return nullptr;
}

SettingGroup^ SettingSource::GetSecurity(String^ uniqueId)
{
	Init();
	for each (auto group in _settingSource->EditChannelSecurity)
	{
		if (group->UniqueId == "Security") return group;
	}
	return nullptr;
}

//SettingItem^ SettingSource::GetItem(String^ uniqueId)
//{
//	Init();
//	// 对于小型数据集可接受简单线性搜索
//	for each (auto group in _settingSource->SettingGroups)
//	{
//		for each (auto item in group->Items)
//		{
//			if (item->UniqueId->Equals(uniqueId)) return item;
//		}
//	}
//	return nullptr;
//}
