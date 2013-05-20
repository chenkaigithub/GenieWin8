#pragma once

#include <collection.h>
#include "Common\BindableBase.h"

namespace NETGEAR_Genie_Win8
{
	namespace Data
	{
		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::Bindable]
		public ref class FilterLevelCommon : NETGEAR_Genie_Win8::Common::BindableBase
		{
		internal:
			FilterLevelCommon(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content);

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
		public ref class FilterLevelGroup sealed : public FilterLevelCommon
		{
		public:
			FilterLevelGroup(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content);
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class FilterLevelSource sealed
		{
		public:			
			FilterLevelSource();
			property Windows::Foundation::Collections::IObservableVector<FilterLevelGroup^>^ FilterLevelGroups
			{
				Windows::Foundation::Collections::IObservableVector<FilterLevelGroup^>^ get();
			}			
			static Windows::Foundation::Collections::IIterable<FilterLevelGroup^>^ GetGroup(Platform::String^ uniqueId);

		private: 
			static void Init();
			Platform::Collections::Vector<FilterLevelGroup^>^ _filterLevelGroups;
		};
	}
}
