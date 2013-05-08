#pragma once

#include <collection.h>
#include "Common\BindableBase.h"

namespace NETGEAR_Genie_Win8
{
	namespace Data
	{
		ref class TrafficControlGroup; 

		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::Bindable]
		public ref class TrafficControlCommon : NETGEAR_Genie_Win8::Common::BindableBase
		{
		internal:
			TrafficControlCommon(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content);

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
		public ref class TrafficControlItem sealed : TrafficControlCommon
		{
		public:
			TrafficControlItem(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content, TrafficControlGroup^ group);
			property TrafficControlGroup^ Group { TrafficControlGroup^ get(); void set(TrafficControlGroup^ value); }

		private:
			Platform::WeakReference _group;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class TrafficControlGroup sealed : public TrafficControlCommon
		{
		public:
			TrafficControlGroup(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ content);
			property Windows::Foundation::Collections::IObservableVector<TrafficControlItem^>^ Items
			{
				Windows::Foundation::Collections::IObservableVector<TrafficControlItem^>^ get();
			}

		private:
			Platform::Collections::Vector<TrafficControlItem^>^ _items;
		};

		[Windows::UI::Xaml::Data::Bindable]
		public ref class TrafficControlSource sealed
		{
		public:			
			TrafficControlSource();
			property Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ TrafficControlGroups
			{
				Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ LimitPerMonth
			{
				Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ StartDate
			{
				Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ StartTimeHour
			{
				Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ StartTimeMin
			{
				Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ get();
			}
			property Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ TrafficLimitation
			{
				Windows::Foundation::Collections::IObservableVector<TrafficControlGroup^>^ get();
			}
			static Windows::Foundation::Collections::IIterable<TrafficControlGroup^>^ GetGroups(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<TrafficControlGroup^>^ GetLimitPerMonth(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<TrafficControlGroup^>^ GetStartDate(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<TrafficControlGroup^>^ GetStartTimeHour(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<TrafficControlGroup^>^ GetStartTimeMin(Platform::String^ uniqueId);
			static Windows::Foundation::Collections::IIterable<TrafficControlGroup^>^ GetTrafficLimitation(Platform::String^ uniqueId);
			static TrafficControlGroup^ GetStartDateItems(Platform::String^ uniqueId);
			static TrafficControlGroup^ GetTrafficLimitationItems(Platform::String^ uniqueId);
			//static TrafficControlGroup^ GetChannel(Platform::String^ uniqueId);
			//static TrafficControlGroup^ GetSecurity(Platform::String^ uniqueId);
			//static TrafficControlGroup^ GetGroup(Platform::String^ uniqueId);
			//static TrafficControlItem^ GetItem(Platform::String^ uniqueId);

		private: 
			static void Init();
			Platform::Collections::Vector<TrafficControlGroup^>^ _trafficControlGroups;
			Platform::Collections::Vector<TrafficControlGroup^>^ _limitPerMonth;
			Platform::Collections::Vector<TrafficControlGroup^>^ _startDate;
			Platform::Collections::Vector<TrafficControlGroup^>^ _startTimeHour;
			Platform::Collections::Vector<TrafficControlGroup^>^ _startTimeMin;
			Platform::Collections::Vector<TrafficControlGroup^>^ _trafficLimitation;
		};
	}
}
