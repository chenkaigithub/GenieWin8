//
// MainPageModel.h
// DataSource、DataGroup、DataItem 和 DataCommon 类的声明
//

#pragma once

#include <collection.h>
#include "Common\BindableBase.h"

// 此文件定义的数据模型可充当在添加、移除或修改成员时
// 支持通知的强类型模型的代表性示例。所选
// 属性名称与标准项模板中的数据绑定一致。
//
// 应用程序可以使用此模型作为起始点并以它为基础构建，或完全放弃它并
// 替换为适合其需求的其他内容。

namespace NETGEAR_Genie_Win8
{
	namespace Data
	{
		ref class DataGroup; // 解决 DataItem 和 DataGroup 之间的循环关系

		/// <summary>
		/// <see cref="DataItem"/> 和 <see cref="DataGroup"/> 的基类，
		/// 定义对两者通用的属性。
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		[Windows::UI::Xaml::Data::Bindable]
		public ref class DataCommon : NETGEAR_Genie_Win8::Common::BindableBase
		{
		internal:
			DataCommon(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ imagePath);

		public:
			void SetImage(Platform::String^ path);
			//virtual Platform::String^ GetStringRepresentation() override;
			property Platform::String^ UniqueId { Platform::String^ get(); void set(Platform::String^ value); }
			property Platform::String^ Title { Platform::String^ get(); void set(Platform::String^ value); }
			property Windows::UI::Xaml::Media::ImageSource^ Image { Windows::UI::Xaml::Media::ImageSource^ get(); void set(Windows::UI::Xaml::Media::ImageSource^ value); }

		private:
			Platform::String^ _uniqueId;
			Platform::String^ _title;
			Windows::UI::Xaml::Media::ImageSource^ _image;
			Platform::String^ _imagePath;
		};

		/// <summary>
		/// 泛型项数据模型。
		/// </summary>
		//[Windows::UI::Xaml::Data::Bindable]
		//public ref class DataItem sealed : DataCommon
		//{
		//public:
		//	DataItem(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ imagePath, Platform::String^ content, DataGroup^ group);

		//	property Platform::String^ Content { Platform::String^ get(); void set(Platform::String^ value); }
		//	property DataGroup^ Group { DataGroup^ get(); void set(DataGroup^ value); }

		//private:
		//	Platform::WeakReference _group; // 用于中断引用计数循环的弱引用
		//	Platform::String^ _content;
		//};

		/// <summary>
		/// 泛型组数据模型。
		/// </summary>
		[Windows::UI::Xaml::Data::Bindable]
		public ref class DataGroup sealed : public DataCommon
		{
		public:
			DataGroup(Platform::String^ uniqueId, Platform::String^ title, Platform::String^ imagePath);
		//	property Windows::Foundation::Collections::IObservableVector<DataItem^>^ Items
		//	{
		//		Windows::Foundation::Collections::IObservableVector<DataItem^>^ get();
		//	}
		//	property Windows::Foundation::Collections::IVector<DataItem^>^ TopItems
		//	{
		//		Windows::Foundation::Collections::IVector<DataItem^>^ get();
		//	}

		//private:
		//	Platform::Collections::Vector<DataItem^>^ _items;
		//	Platform::Collections::Vector<DataItem^>^ _topitems;
		//	void ItemsCollectionChanged(Windows::Foundation::Collections::IObservableVector<DataItem^>^ , Windows::Foundation::Collections::IVectorChangedEventArgs^ );
		};

		/// <summary>
		/// 创建包含硬编码内容的组和项的集合。
		/// 
		/// DataSource 用占位符数据而不是实时生产数据
		/// 初始化，因此在设计时和运行时均需提供示例数据。
		/// </summary>
		[Windows::UI::Xaml::Data::Bindable]
		public ref class DataSource sealed
		{
		public:			
			DataSource();
			property Windows::Foundation::Collections::IObservableVector<DataGroup^>^ AllGroups
			{
				Windows::Foundation::Collections::IObservableVector<DataGroup^>^ get();
			}
			static Windows::Foundation::Collections::IIterable<DataGroup^>^ GetGroups(Platform::String^ uniqueId);
			static DataGroup^ GetGroup(Platform::String^ uniqueId);
			//static DataItem^ GetItem(Platform::String^ uniqueId);

		private: 
			static void Init();
			Platform::Collections::Vector<DataGroup^>^ _allGroups;
		};
	}
}
