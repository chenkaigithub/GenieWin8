#pragma once

#include <collection.h>
#include "Common\BindableBase.h"

namespace NETGEAR_Genie_Win8
{
	namespace Data
	{
		ref class DeviceGroup; 

		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::Bindable]
		public ref class DeviceCommon : NETGEAR_Genie_Win8::Common::BindableBase
		{
		internal:
			DeviceCommon(Platform::String^ uniqueId);

		public:
			property Platform::String^ UniqueId { Platform::String^ get(); void set(Platform::String^ value); }
			

		private:
			Platform::String^ _uniqueId;
			
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class DeviceItem sealed : DeviceCommon
		{
		public:
			DeviceItem(Platform::String^ uniqueId, Platform::String^ deviceName, Platform::String^ IPaddress, Platform::String^ signalStrength, Platform::String^ linkRate, Platform::String^ MACaddress, DeviceGroup^ group);
			
			property Platform::String^ DeviceName { Platform::String^ get(); void set(Platform::String^ value); }
			property Platform::String^ IPAddress { Platform::String^ get(); void set(Platform::String^ value); }
			property Platform::String^ SignalStrength { Platform::String^ get(); void set(Platform::String^ value); }
			property Platform::String^ LinkRate { Platform::String^ get(); void set(Platform::String^ value); }
			property Platform::String^ MACAddress { Platform::String^ get(); void set(Platform::String^ value); }
			property DeviceGroup^ Group { DeviceGroup^ get(); void set(DeviceGroup^ value); }

		private:
			Platform::String^ _deviceName;
			Platform::String^ _IPaddress;
			Platform::String^ _signalStrength;
			Platform::String^ _linkRate;
			Platform::String^ _MACaddress;
			Platform::WeakReference _group;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class DeviceGroup sealed : public DeviceCommon
		{
		public:
			DeviceGroup(Platform::String^ uniqueId);
			property Windows::Foundation::Collections::IObservableVector<DeviceItem^>^ Items
			{
				Windows::Foundation::Collections::IObservableVector<DeviceItem^>^ get();
			}

		private:
			Platform::Collections::Vector<DeviceItem^>^ _items;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class DeviceSource sealed
		{
		public:			
			DeviceSource();
			property Windows::Foundation::Collections::IObservableVector<DeviceGroup^>^ DeviceGroups
			{
				Windows::Foundation::Collections::IObservableVector<DeviceGroup^>^ get();
			}
			static Windows::Foundation::Collections::IIterable<DeviceGroup^>^ GetGroups(Platform::String^ uniqueId);

		private: 
			static void Init();
			Platform::Collections::Vector<DeviceGroup^>^ _deviceGroups;
		};
	}
}
