#include "pch.h"
#include "ViewModels\NetworkMapModel.h"

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
// DeviceCommon
//

DeviceCommon::DeviceCommon(Platform::String^ uniqueId)
{
	_uniqueId = uniqueId;
	
}

String^ DeviceCommon::UniqueId::get()
{
	return _uniqueId;
}

void DeviceCommon::UniqueId::set(String^ value)
{
	if (_uniqueId != value)
	{
		_uniqueId = value;
		OnPropertyChanged("UniqueId");
	}
}

//
// DeviceItem
//

DeviceItem::DeviceItem(Platform::String^ uniqueId, Platform::String^ deviceName, Platform::String^ IPaddress, Platform::String^ signalStrength, Platform::String^ linkRate, Platform::String^ MACaddress, DeviceGroup^ group)
							   : DeviceCommon(uniqueId)
{
	_deviceName = deviceName;
	_IPaddress = IPaddress;
	_signalStrength = signalStrength;
	_linkRate = linkRate;
	_MACaddress = MACaddress;
	_group = group;
}

String^ DeviceItem::DeviceName::get()
{
	return _deviceName;
}

void DeviceItem::DeviceName::set(String^ value)
{
	if (_deviceName != value)
	{
		_deviceName = value;
		OnPropertyChanged("DeviceName");
	}
}

String^ DeviceItem::IPAddress::get()
{
	return _IPaddress;
}

void DeviceItem::IPAddress::set(String^ value)
{
	if (_IPaddress != value)
	{
		_IPaddress = value;
		OnPropertyChanged("IPAddress");
	}
}

String^ DeviceItem::SignalStrength::get()
{
	return _signalStrength;
}

void DeviceItem::SignalStrength::set(String^ value)
{
	if (_signalStrength != value)
	{
		_signalStrength = value;
		OnPropertyChanged("SignalStrength");
	}
}

String^ DeviceItem::LinkRate::get()
{
	return _linkRate;
}

void DeviceItem::LinkRate::set(String^ value)
{
	if (_linkRate != value)
	{
		_linkRate = value;
		OnPropertyChanged("LinkRate");
	}
}

String^ DeviceItem::MACAddress::get()
{
	return _MACaddress;
}

void DeviceItem::MACAddress::set(String^ value)
{
	if (_MACaddress != value)
	{
		_MACaddress = value;
		OnPropertyChanged("MACAddress");
	}
}

DeviceGroup^ DeviceItem::Group::get()
{
	return _group.Resolve<DeviceGroup>();
}

void DeviceItem::Group::set(DeviceGroup^ value)
{
	if (Group != value)
	{
		_group = value;
		OnPropertyChanged("Group");
	}
}

//
// DeviceGroup
//

DeviceGroup::DeviceGroup(Platform::String^ uniqueId)
	: DeviceCommon(uniqueId)
{
	_items = ref new Vector<DeviceItem^>();
}

IObservableVector<DeviceItem^>^ DeviceGroup::Items::get()
{
	return _items;
}

//
// DeviceSource
//

DeviceSource::DeviceSource()
{
	_deviceGroups = ref new Vector<DeviceGroup^>();
	
	auto group = ref new DeviceGroup("DeviceGroup");
	group->Items->Append(ref new DeviceItem("Router",
		"WNR3500Lv2",
		"192.168.1.1",
		"",
		"",
		"20:4E:7F:04:31:3C",
		group));
	group->Items->Append(ref new DeviceItem("LocalDevice",
		"android-25531554966beee3",
		"192.168.1.25",
		"78%",
		"5.5Mbps",
		"D4:20:6D:D6:37:D6",
		group));
	_deviceGroups->Append(group);
}

IObservableVector<DeviceGroup^>^ DeviceSource::DeviceGroups::get()
{
	return _deviceGroups;
}

static DeviceSource^ _deviceSource = nullptr;

void DeviceSource::Init()
{
	if (_deviceSource == nullptr)
	{
		_deviceSource = ref new DeviceSource();
	}
}

IIterable<DeviceGroup^>^ DeviceSource::GetGroups(String^ uniqueId)
{
	Init();
	return _deviceSource->DeviceGroups;
}
