#pragma once

#include <collection.h>
#include "Common\BindableBase.h"

namespace NETGEAR_Genie_Win8
{
	namespace Data
	{
		ref class MyMediaGroup;

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

		[Windows::UI::Xaml::Data::Bindable]
		public ref class MediaItem sealed : MediaCommon
		{
		public:
			MediaItem(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ imagePath, Platform::String^ content, MyMediaGroup^ group);

			property Platform::String^ Content { Platform::String^ get(); void set(Platform::String^ value); }
			property MyMediaGroup^ Group { MyMediaGroup^ get(); void set(MyMediaGroup^ value); }

		private:
			Platform::WeakReference _group;
			Platform::String^ _content;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class MyMediaGroup sealed : public MediaCommon
		{
		public:
			MyMediaGroup(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ imagePath);
			property Windows::Foundation::Collections::IObservableVector<MediaItem^>^ Items
			{
				Windows::Foundation::Collections::IObservableVector<MediaItem^>^ get();
			}

		private:
			Platform::Collections::Vector<MediaItem^>^ _items;
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
			static Windows::Foundation::Collections::IIterable<MyMediaGroup^>^ GetMymediaGroups(Platform::String^ uniqueId);
			static MyMediaGroup^ GetSourceGroup(Platform::String^ uniqueId);
			static MyMediaGroup^ GetPlayerGroup(Platform::String^ uniqueId);

		private: 
			static void Init();
			Platform::Collections::Vector<MyMediaGroup^>^ _mymediaGroups;
		};
	}
}
