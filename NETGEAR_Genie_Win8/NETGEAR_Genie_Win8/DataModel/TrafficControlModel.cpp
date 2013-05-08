#include "pch.h"
#include "DataModel/TrafficControlModel.h"

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
// TrafficControlCommon
//

TrafficControlCommon::TrafficControlCommon(String^ uniqueId, String^ title, String^ content)
{
	_uniqueId = uniqueId;
	_title = title;
	_content = content;
}

String^ TrafficControlCommon::UniqueId::get()
{
	return _uniqueId;
}

void TrafficControlCommon::UniqueId::set(String^ value)
{
	if (_uniqueId != value)
	{
		_uniqueId = value;
		OnPropertyChanged("UniqueId");
	}
}

String^ TrafficControlCommon::Title::get()
{
	return _title;
}

void TrafficControlCommon::Title::set(String^ value)
{
	if (_title != value)
	{
		_title = value;
		OnPropertyChanged("Title");
	}
}

String^ TrafficControlCommon::Content::get()
{
	return _content;
}

void TrafficControlCommon::Content::set(String^ value)
{
	if (_content != value)
	{
		_content = value;
		OnPropertyChanged("Content");
	}
}

//
// TrafficControlItem
//

TrafficControlItem::TrafficControlItem(String^ uniqueId, String^ title, String^ content, TrafficControlGroup^ group)
	: TrafficControlCommon(uniqueId, title, content)
{
	_group = group;
}

TrafficControlGroup^ TrafficControlItem::Group::get()
{
	return _group.Resolve<TrafficControlGroup>();
}

void TrafficControlItem::Group::set(TrafficControlGroup^ value)
{
	if (Group != value)
	{
		_group = value;
		OnPropertyChanged("Group");
	}
}

//
// TrafficControlGroup
//

TrafficControlGroup::TrafficControlGroup(String^ uniqueId, String^ title, String^ content)
	: TrafficControlCommon(uniqueId, title, content)
{
	_items = ref new Vector<TrafficControlItem^>();
}

IObservableVector<TrafficControlItem^>^ TrafficControlGroup::Items::get()
{
	return _items;
}

//
// TrafficControlSource
//

TrafficControlSource::TrafficControlSource()
{
	_trafficControlGroups = ref new Vector<TrafficControlGroup^>();
	_limitPerMonth = ref new Vector<TrafficControlGroup^>();
	_startDate = ref new Vector<TrafficControlGroup^>();
	_startTimeHour = ref new Vector<TrafficControlGroup^>();
	_startTimeMin = ref new Vector<TrafficControlGroup^>();
	_trafficLimitation = ref new Vector<TrafficControlGroup^>();

	auto group1 = ref new TrafficControlGroup("LimitPerMonth",
		"每月限制(MB)",
		"limitpermonth");
	_limitPerMonth->Append(group1);
	_trafficControlGroups->Append(group1);

	auto group2 = ref new TrafficControlGroup("StartDate",
		"计数器启动日期",
		"1");
	group2->Items->Append(ref new TrafficControlItem("StartDate-1",
		"StartDate",
		"1",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-2",
		"StartDate",
		"2",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-3",
		"StartDate",
		"3",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-4",
		"StartDate",
		"4",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-5",
		"StartDate",
		"5",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-6",
		"StartDate",
		"6",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-7",
		"StartDate",
		"7",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-8",
		"StartDate",
		"8",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-9",
		"StartDate",
		"9",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-10",
		"StartDate",
		"10",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-11",
		"StartDate",
		"11",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-12",
		"StartDate",
		"12",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-13",
		"StartDate",
		"13",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-14",
		"StartDate",
		"14",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-15",
		"StartDate",
		"15",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-16",
		"StartDate",
		"16",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-17",
		"StartDate",
		"17",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-18",
		"StartDate",
		"18",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-19",
		"StartDate",
		"19",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-20",
		"StartDate",
		"20",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-21",
		"StartDate",
		"21",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-22",
		"StartDate",
		"22",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-23",
		"StartDate",
		"23",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-24",
		"StartDate",
		"24",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-25",
		"StartDate",
		"25",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-26",
		"StartDate",
		"26",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-27",
		"StartDate",
		"27",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-28",
		"StartDate",
		"28",
		group2));
	group2->Items->Append(ref new TrafficControlItem("StartDate-29",
		"StartDate",
		"29",
		group2));
	_startDate->Append(group2);
	_trafficControlGroups->Append(group2);

	String^ STARTTIME_HOUR = "hour";
	String^ STARTTIME_MINUTE = "minute";
	auto hour = ref new TrafficControlGroup("StartTimeHour",
		"启动时间-时",
		STARTTIME_HOUR);
	auto minute = ref new TrafficControlGroup("StartTimeMin",
		"启动时间-分",
		STARTTIME_MINUTE);
	auto group3 = ref new TrafficControlGroup("StartTime",
		"启动时间",
		STARTTIME_HOUR+":"+STARTTIME_MINUTE);
	_startTimeHour->Append(hour);
	_startTimeMin->Append(minute);
	_trafficControlGroups->Append(group3);

	auto group4 = ref new TrafficControlGroup("TrafficLimitation",
		"流量限制",
		"Unlimited");
	group4->Items->Append(ref new TrafficControlItem("TrafficLimitation-1",
		"TrafficLimitation",
		"Unlimited",
		group4));
	group4->Items->Append(ref new TrafficControlItem("TrafficLimitation-2",
		"TrafficLimitation",
		"Download Only",
		group4));
	group4->Items->Append(ref new TrafficControlItem("TrafficLimitation-3",
		"TrafficLimitation",
		"Download & Upload",
		group4));
	_trafficLimitation->Append(group4);
	_trafficControlGroups->Append(group4);
}

IObservableVector<TrafficControlGroup^>^ TrafficControlSource::TrafficControlGroups::get()
{
	return _trafficControlGroups;
}

IObservableVector<TrafficControlGroup^>^ TrafficControlSource::LimitPerMonth::get()
{
	return _limitPerMonth;
}

IObservableVector<TrafficControlGroup^>^ TrafficControlSource::StartDate::get()
{
	return _startDate;
}

IObservableVector<TrafficControlGroup^>^ TrafficControlSource::StartTimeHour::get()
{
	return _startTimeHour;
}

IObservableVector<TrafficControlGroup^>^ TrafficControlSource::StartTimeMin::get()
{
	return _startTimeMin;
}

IObservableVector<TrafficControlGroup^>^ TrafficControlSource::TrafficLimitation::get()
{
	return _trafficLimitation;
}

static TrafficControlSource^ _trafficControlSource = nullptr;

void TrafficControlSource::Init()
{
	if (_trafficControlSource == nullptr)
	{
		_trafficControlSource = ref new TrafficControlSource();
	}
}

IIterable<TrafficControlGroup^>^ TrafficControlSource::GetGroups(String^ uniqueId)
{
	Init();
	return _trafficControlSource->TrafficControlGroups;
}

IIterable<TrafficControlGroup^>^ TrafficControlSource::GetLimitPerMonth(String^ uniqueId)
{
	Init();
	return _trafficControlSource->LimitPerMonth;
}

IIterable<TrafficControlGroup^>^ TrafficControlSource::GetStartDate(String^ uniqueId)
{
	Init();
	return _trafficControlSource->StartDate;
}

IIterable<TrafficControlGroup^>^ TrafficControlSource::GetStartTimeHour(String^ uniqueId)
{
	Init();
	return _trafficControlSource->StartTimeHour;
}

IIterable<TrafficControlGroup^>^ TrafficControlSource::GetStartTimeMin(String^ uniqueId)
{
	Init();
	return _trafficControlSource->StartTimeMin;
}

IIterable<TrafficControlGroup^>^ TrafficControlSource::GetTrafficLimitation(String^ uniqueId)
{
	Init();
	return _trafficControlSource->TrafficLimitation;
}

TrafficControlGroup^ TrafficControlSource::GetStartDateItems(String^ uniqueId)
{
	Init();
	for each (auto group in _trafficControlSource->StartDate)
	{
		if (group->UniqueId == "StartDate") return group;
	}
	return nullptr;
}

TrafficControlGroup^ TrafficControlSource::GetTrafficLimitationItems(String^ uniqueId)
{
	Init();
	for each (auto group in _trafficControlSource->TrafficLimitation)
	{
		if (group->UniqueId == "TrafficLimitation") return group;
	}
	return nullptr;
}
//TrafficControlGroup^ TrafficControlSource::GetChannel(String^ uniqueId)
//{
//	Init();
//	for each (auto group in _trafficControlSource->EditChannelSecurity)
//	{
//		if (group->UniqueId == "Channel") return group;
//	}
//	return nullptr;
//}
//
//TrafficControlGroup^ TrafficControlSource::GetSecurity(String^ uniqueId)
//{
//	Init();
//	for each (auto group in _trafficControlSource->EditChannelSecurity)
//	{
//		if (group->UniqueId == "Security") return group;
//	}
//	return nullptr;
//}

//TrafficControlItem^ TrafficControlSource::GetItem(String^ uniqueId)
//{
//	Init();
//	// 对于小型数据集可接受简单线性搜索
//	for each (auto group in _trafficControlSource->TrafficControlGroups)
//	{
//		for each (auto item in group->Items)
//		{
//			if (item->UniqueId->Equals(uniqueId)) return item;
//		}
//	}
//	return nullptr;
//}
