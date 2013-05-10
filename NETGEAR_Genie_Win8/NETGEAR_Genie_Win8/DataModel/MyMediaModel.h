#pragma once

#include <collection.h>
#include "Common\BindableBase.h"

namespace NETGEAR_Genie_Win8
{
	namespace Data
	{
		ref class MyMediaGroup;
		ref class SourcesGroup;
		ref class PlayersGroup;

		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::Bindable]
		public ref class MediaCommon : NETGEAR_Genie_Win8::Common::BindableBase
		{
		internal:
			MediaCommon(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ imagePath);

		public:
			void SetImage(Platform::String^ path);
			property Platform::String^ UniqueId { Platform::String^ get(); void set(Platform::String^ value); }
			property Platform::String^ Title { Platform::String^ get(); void set(Platform::String^ value); }
			property Windows::UI::Xaml::Media::ImageSource^ Image { Windows::UI::Xaml::Media::ImageSource^ get(); void set(Windows::UI::Xaml::Media::ImageSource^ value); }

		private:
			Platform::String^ _uniqueId;
			Platform::String^ _title;
			Windows::UI::Xaml::Media::ImageSource^ _image;
			Platform::String^ _imagePath;
		};

		//[Windows::UI::Xaml::Data::Bindable]
		//public ref class SettingItem sealed : MediaCommon
		//{
		//public:
		//	SettingItem(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content, SourcesGroup^ group);
		//	property SourcesGroup^ Group { SourcesGroup^ get(); void set(SourcesGroup^ value); }

		//private:
		//	Platform::WeakReference _group;
		//};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class MyMediaGroup sealed : public MediaCommon
		{
		public:
			MyMediaGroup(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ imagePath);
		};
		public ref class SourcesGroup sealed : public MediaCommon
		{
		public:
			SourcesGroup(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ imagePath);
		//	property Windows::Foundation::Collections::IObservableVector<SettingItem^>^ Items
		//	{
		//		Windows::Foundation::Collections::IObservableVector<SettingItem^>^ get();
		//	}

		//private:
		//	Platform::Collections::Vector<SettingItem^>^ _items;
		};
		public ref class PlayersGroup sealed : public MediaCommon
		{
		public:
			PlayersGroup(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ imagePath);
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class MediaSource sealed
		{
		public:			
			MediaSource();
			property Windows::Foundation::Collections::IObservableVector<MyMediaGroup^>^ MyMediaGroups
			{
				Windows::Foundation::Collections::IObservableVector<MyMediaGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<SourcesGroup^>^ SourcesGroups
			{
				Windows::Foundation::Collections::IObservableVector<SourcesGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<PlayersGroup^>^ PlayersGroups
			{
				Windows::Foundation::Collections::IObservableVector<PlayersGroup^>^ get();
			}
			static Windows::Foundation::Collections::IIterable<MyMediaGroup^>^ GetMymediaGroups(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<SourcesGroup^>^ GetSourceGroups(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<PlayersGroup^>^ GetPlayerGroups(Platform::String^ uniqueId);

		private: 
			static void Init();
			Platform::Collections::Vector<MyMediaGroup^>^ _mymediaGroups;
			Platform::Collections::Vector<SourcesGroup^>^ _sourcesGroups;
			Platform::Collections::Vector<PlayersGroup^>^ _playersGroups;
		};
	}
}
