#pragma once

#include <collection.h>
#include "Common\BindableBase.h"

namespace NETGEAR_Genie_Win8
{
	namespace Data
	{
		ref class SettingGroup; 

		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::Bindable]
		public ref class SettingCommon : NETGEAR_Genie_Win8::Common::BindableBase
		{
		internal:
			SettingCommon(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content);

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
		public ref class SettingItem sealed : SettingCommon
		{
		public:
			SettingItem(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content, SettingGroup^ group);
			property SettingGroup^ Group { SettingGroup^ get(); void set(SettingGroup^ value); }

		private:
			Platform::WeakReference _group;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class SettingGroup sealed : public SettingCommon
		{
		public:
			SettingGroup(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content);
			property Windows::Foundation::Collections::IObservableVector<SettingItem^>^ Items
			{
				Windows::Foundation::Collections::IObservableVector<SettingItem^>^ get();
			}

		private:
			Platform::Collections::Vector<SettingItem^>^ _items;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class SettingSource sealed
		{
		public:			
			SettingSource();
			property Windows::Foundation::Collections::IObservableVector<SettingGroup^>^ SettingGroups
			{
				Windows::Foundation::Collections::IObservableVector<SettingGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<SettingGroup^>^ EditName
			{
				Windows::Foundation::Collections::IObservableVector<SettingGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<SettingGroup^>^ EditKey
			{
				Windows::Foundation::Collections::IObservableVector<SettingGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<SettingGroup^>^ EditChannelSecurity
			{
				Windows::Foundation::Collections::IObservableVector<SettingGroup^>^ get();
			}
			static Windows::Foundation::Collections::IIterable<SettingGroup^>^ GetGroups(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<SettingGroup^>^ GetEditName(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<SettingGroup^>^ GetEditKey(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<SettingGroup^>^ GetChannelSecurity(Platform::String^ uniqueId);
			static SettingGroup^ GetChannel(Platform::String^ uniqueId);
			static SettingGroup^ GetSecurity(Platform::String^ uniqueId);
			//static SettingGroup^ GetGroup(Platform::String^ uniqueId);
			//static SettingItem^ GetItem(Platform::String^ uniqueId);

		private: 
			static void Init();
			Platform::Collections::Vector<SettingGroup^>^ _settingGroups;
			Platform::Collections::Vector<SettingGroup^>^ _editName;
			Platform::Collections::Vector<SettingGroup^>^ _editKey;
			Platform::Collections::Vector<SettingGroup^>^ _editChannelSecurity;
		};
	}
}
