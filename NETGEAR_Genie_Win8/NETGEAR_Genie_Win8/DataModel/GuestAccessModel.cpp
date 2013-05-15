#include "pch.h"
#include "DataModel\GuestAccessModel.h"

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
// GuestSettingCommon
//

GuestSettingCommon::GuestSettingCommon(String^ uniqueId, String^ title, String^ content)
{
	_uniqueId = uniqueId;
	_title = title;
	_content = content;
}

String^ GuestSettingCommon::UniqueId::get()
{
	return _uniqueId;
}

void GuestSettingCommon::UniqueId::set(String^ value)
{
	if (_uniqueId != value)
	{
		_uniqueId = value;
		OnPropertyChanged("UniqueId");
	}
}

String^ GuestSettingCommon::Title::get()
{
	return _title;
}

void GuestSettingCommon::Title::set(String^ value)
{
	if (_title != value)
	{
		_title = value;
		OnPropertyChanged("Title");
	}
}

String^ GuestSettingCommon::Content::get()
{
	return _content;
}

void GuestSettingCommon::Content::set(String^ value)
{
	if (_content != value)
	{
		_content = value;
		OnPropertyChanged("Content");
	}
}

//
// GuestSettingItem
//

GuestSettingItem::GuestSettingItem(String^ uniqueId, String^ title, String^ content, GuestSettingGroup^ group)
	: GuestSettingCommon(uniqueId, title, content)
{
	_group = group;
}

GuestSettingGroup^ GuestSettingItem::Group::get()
{
	return _group.Resolve<GuestSettingGroup>();
}

void GuestSettingItem::Group::set(GuestSettingGroup^ value)
{
	if (Group != value)
	{
		_group = value;
		OnPropertyChanged("Group");
	}
}

//
// GuestSettingGroup
//

GuestSettingGroup::GuestSettingGroup(String^ uniqueId, String^ title, String^ content)
	: GuestSettingCommon(uniqueId, title, content)
{
	_items = ref new Vector<GuestSettingItem^>();
}

IObservableVector<GuestSettingItem^>^ GuestSettingGroup::Items::get()
{
	return _items;
}

//
// GuestSettingSource
//

GuestSettingSource::GuestSettingSource()
{
	_guestSettingGroups = ref new Vector<GuestSettingGroup^>();
	_editName = ref new Vector<GuestSettingGroup^>();
	_editTimesegSecurity = ref new Vector<GuestSettingGroup^>();
	auto loader = ref new Windows::ApplicationModel::Resources::ResourceLoader();

	auto strTitle = loader->GetString("GuestWiFiName");
	auto group1 = ref new GuestSettingGroup("GuestWiFiName",
		strTitle,
		"wifiname");
	_editName->Append(group1);
	_guestSettingGroups->Append(group1);

	strTitle = loader->GetString("TimeSegment");
	auto group2 = ref new GuestSettingGroup("TimeSegment",
		strTitle,
		"Always");
	auto strContent = loader->GetString("TimeSegment_Always");
	group2->Items->Append(ref new GuestSettingItem("TimeSegment-1",
		"TimeSegment",
		strContent,
		group2));
	strContent = loader->GetString("TimeSegment_1hour");
	group2->Items->Append(ref new GuestSettingItem("TimeSegment-2",
		"TimeSegment",
		strContent,
		group2));
	strContent = loader->GetString("TimeSegment_5hours");
	group2->Items->Append(ref new GuestSettingItem("TimeSegment-3",
		"TimeSegment",
		strContent,
		group2));
	strContent = loader->GetString("TimeSegment_10hours");
	group2->Items->Append(ref new GuestSettingItem("TimeSegment-4",
		"TimeSegment",
		strContent,
		group2));
	strContent = loader->GetString("TimeSegment_1day");
	group2->Items->Append(ref new GuestSettingItem("TimeSegment-5",
		"TimeSegment",
		strContent,
		group2));
	strContent = loader->GetString("TimeSegment_1week");
	group2->Items->Append(ref new GuestSettingItem("TimeSegment-6",
		"TimeSegment",
		strContent,
		group2));
	_editTimesegSecurity->Append(group2);
	_guestSettingGroups->Append(group2);

	strTitle = loader->GetString("Security");
	auto group3 = ref new GuestSettingGroup("Security",
		strTitle,
		"WPA2-PSK[AES]");
	strContent = loader->GetString("Security_None");
	group3->Items->Append(ref new GuestSettingItem("Security-1",
		"Security",
		strContent,
		group3));
	strContent = loader->GetString("Security_WPA2-PSK[AES]");
	group3->Items->Append(ref new GuestSettingItem("Security-2",
		"Security",
		strContent,
		group3));
	strContent = loader->GetString("Security_WPA-PSK+WPA2-PSK");
	group3->Items->Append(ref new GuestSettingItem("Security-3",
		"Security",
		strContent,
		group3));
	_editTimesegSecurity->Append(group3);
	_guestSettingGroups->Append(group3);
}

IObservableVector<GuestSettingGroup^>^ GuestSettingSource::GuestSettingGroups::get()
{
	return _guestSettingGroups;
}

IObservableVector<GuestSettingGroup^>^ GuestSettingSource::EditName::get()
{
	return _editName;
}

IObservableVector<GuestSettingGroup^>^ GuestSettingSource::EditTimesegSecurity::get()
{
	return _editTimesegSecurity;
}

static GuestSettingSource^ _settingSource = nullptr;

void GuestSettingSource::Init()
{
	if (_settingSource == nullptr)
	{
		_settingSource = ref new GuestSettingSource();
	}
}

IIterable<GuestSettingGroup^>^ GuestSettingSource::GetGroups(String^ uniqueId)
{
	Init();
	return _settingSource->GuestSettingGroups;
}

IIterable<GuestSettingGroup^>^ GuestSettingSource::GetEditName(String^ uniqueId)
{
	Init();
	return _settingSource->EditName;
}

IIterable<GuestSettingGroup^>^ GuestSettingSource::GetTimesegSecurity(String^ uniqueId)
{
	Init();
	return _settingSource->EditTimesegSecurity;
}

GuestSettingGroup^ GuestSettingSource::GetTimeSegment(String^ uniqueId)
{
	Init();
	for each (auto group in _settingSource->EditTimesegSecurity)
	{
		if (group->UniqueId == "TimeSegment") return group;
	}
	return nullptr;
}

GuestSettingGroup^ GuestSettingSource::GetSecurity(String^ uniqueId)
{
	Init();
	for each (auto group in _settingSource->EditTimesegSecurity)
	{
		if (group->UniqueId == "Security") return group;
	}
	return nullptr;
}