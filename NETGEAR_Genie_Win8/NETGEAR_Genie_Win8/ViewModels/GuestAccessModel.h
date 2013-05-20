#pragma once

#include <collection.h>
#include "Common\BindableBase.h"

namespace NETGEAR_Genie_Win8
{
	namespace Data
	{
		ref class GuestSettingGroup; 

		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::Bindable]
		public ref class GuestSettingCommon : NETGEAR_Genie_Win8::Common::BindableBase
		{
		internal:
			GuestSettingCommon(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content);

		public:
			property Platform::String^ UniqueId { Platform::String^ get(); void set(Platform::String^ value); }
			property Platform::String^ Title { Platform::String^ get(); void set(Platform::String^ value); }
			property Platform::String^ Content { Platform::String^ get(); void set(Platform::String^ value); }

		private:
			Platform::String^ _uniqueId;
			Platform::String^ _title;
			Platform::String^ _content;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class GuestSettingItem sealed : GuestSettingCommon
		{
		public:
			GuestSettingItem(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content, GuestSettingGroup^ group);
			property GuestSettingGroup^ Group { GuestSettingGroup^ get(); void set(GuestSettingGroup^ value); }

		private:
			Platform::WeakReference _group;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class GuestSettingGroup sealed : public GuestSettingCommon
		{
		public:
			GuestSettingGroup(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content);
			property Windows::Foundation::Collections::IObservableVector<GuestSettingItem^>^ Items
			{
				Windows::Foundation::Collections::IObservableVector<GuestSettingItem^>^ get();
			}

		private:
			Platform::Collections::Vector<GuestSettingItem^>^ _items;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class GuestSettingSource sealed
		{
		public:			
			GuestSettingSource();
			property Windows::Foundation::Collections::IObservableVector<GuestSettingGroup^>^ GuestSettingGroups
			{
				Windows::Foundation::Collections::IObservableVector<GuestSettingGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<GuestSettingGroup^>^ EditName
			{
				Windows::Foundation::Collections::IObservableVector<GuestSettingGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<GuestSettingGroup^>^ EditTimesegSecurity
			{
				Windows::Foundation::Collections::IObservableVector<GuestSettingGroup^>^ get();
			}
			static Windows::Foundation::Collections::IIterable<GuestSettingGroup^>^ GetGroups(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<GuestSettingGroup^>^ GetEditName(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<GuestSettingGroup^>^ GetTimesegSecurity(Platform::String^ uniqueId);
			static GuestSettingGroup^ GetTimeSegment(Platform::String^ uniqueId);
			static GuestSettingGroup^ GetSecurity(Platform::String^ uniqueId);

		private: 
			static void Init();
			Platform::Collections::Vector<GuestSettingGroup^>^ _guestSettingGroups;
			Platform::Collections::Vector<GuestSettingGroup^>^ _editName;
			Platform::Collections::Vector<GuestSettingGroup^>^ _editTimesegSecurity;
		};
	}
}
