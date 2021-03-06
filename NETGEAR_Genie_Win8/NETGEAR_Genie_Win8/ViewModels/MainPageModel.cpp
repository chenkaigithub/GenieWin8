﻿//
// MainPageModel.cpp
// DataSource、DataGroup、DataItem 和 DataCommon 类的实现
//
#include "pch.h"
#include "ViewModels\MainPageModel.h"

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
// DataCommon
//

DataCommon::DataCommon(String^ uniqueId, String^ title, String^ imagePath)
{
	_uniqueId = uniqueId;
	_title = title;
	_imagePath = imagePath;
	_image = nullptr;
}

String^ DataCommon::UniqueId::get()
{
	return _uniqueId;
}

void DataCommon::UniqueId::set(String^ value)
{
	if (_uniqueId != value)
	{
		_uniqueId = value;
		OnPropertyChanged("UniqueId");
	}
}

String^ DataCommon::Title::get()
{
	return _title;
}

void DataCommon::Title::set(String^ value)
{
	if (_title != value)
	{
		_title = value;
		OnPropertyChanged("Title");
	}
}

//String^ DataCommon::Subtitle::get()
//{
//	return _subtitle;
//}
//
//void DataCommon::Subtitle::set(String^ value)
//{
//	if (_subtitle != value)
//	{
//		_subtitle = value;
//		OnPropertyChanged("Subtitle");
//	}
//}
//
//String^ DataCommon::Description::get()
//{
//	return _description;
//}
//
//void DataCommon::Description::set(String^ value)
//{
//	if (_description != value)
//	{
//		_description = value;
//		OnPropertyChanged("Description");
//	}
//}

ImageSource^ DataCommon::Image::get()
{
	static Uri^ _baseUri = ref new Uri("ms-appx:///");
	
	if (_image == nullptr && _imagePath != nullptr)
	{
		_image = ref new BitmapImage(_baseUri->CombineUri(_imagePath));
	}
	return _image;
}

void DataCommon::Image::set(ImageSource^ value)
{
	if (_image != value)
	{
		_image = value;
		_imagePath = nullptr;
		OnPropertyChanged("Image");
		PropertySet set;
	}
}

void DataCommon::SetImage(String^ path)
{
	_image = nullptr;
	_imagePath = path;
	OnPropertyChanged("Image");
}

//Platform::String^ DataCommon::GetStringRepresentation()
//{
//	return _title;
//}

//
// DataItem
//

//DataItem::DataItem(String^ uniqueId, String^ title, String^ subtitle, String^ imagePath, String^ description,
//							   String^ content, DataGroup^ group)
//							   : DataCommon(uniqueId, title, subtitle, imagePath, description)
//{
//	_content = content;
//	_group = group;
//}
//
//String^ DataItem::Content::get()
//{
//	return _content;
//}
//
//void DataItem::Content::set(String^ value)
//{
//	if (_content != value)
//	{
//		_content = value;
//		OnPropertyChanged("Content");
//	}
//}
//
//DataGroup^ DataItem::Group::get()
//{
//	return _group.Resolve<DataGroup>();
//}
//
//void DataItem::Group::set(DataGroup^ value)
//{
//	if (Group != value)
//	{
//		_group = value;
//		OnPropertyChanged("Group");
//	}
//}

//
// DataGroup
//

DataGroup::DataGroup(String^ uniqueId, String^ title, String^ imagePath)
	: DataCommon(uniqueId, title, imagePath)
{
	//_items = ref new Vector<DataItem^>();
	//_topitems = ref new Vector<DataItem^>();
	//Items->VectorChanged +=
	//	ref new VectorChangedEventHandler<DataItem^>(this,&DataGroup::ItemsCollectionChanged,CallbackContext::Same);
}

//void DataGroup::ItemsCollectionChanged(Windows::Foundation::Collections::IObservableVector<DataItem^>^ sender, Windows::Foundation::Collections::IVectorChangedEventArgs^ args)
//{
//	// 由于两个原因提供要从 GroupedItemsPage 绑定到的完整
//	// 项集合的子集: GridView 不会虚拟化大型项集合，并且它
//	// 可在浏览包含大量项的组时改进用户
//	// 体验。
//	//
//	// 最多显示 12 项，因为无论显示 1、2、3、4 还是 6 行，
//	// 它都生成填充网格列
//
//	if(args->CollectionChange == CollectionChange::Reset)
//	{
//		TopItems->Clear();
//		return;
//	}
//
//	if(args->Index >= 12)
//	{
//		return;
//	}
//
//	switch(args->CollectionChange)
//	{
//	case CollectionChange::ItemInserted:
//		TopItems->InsertAt(args->Index, Items->GetAt(args->Index));
//		if(TopItems->Size > 12)
//		{
//			TopItems->RemoveAt(12);
//		}
//		break;
//	case CollectionChange::ItemChanged:
//		TopItems->SetAt(args->Index,Items->GetAt(args->Index));
//		break;
//	case CollectionChange::ItemRemoved:
//		TopItems->RemoveAt(args->Index);
//		if(Items->Size >= 12)
//		{
//			TopItems->Append(Items->GetAt(11));
//		}
//		break;
//	}
//}
//
//IObservableVector<DataItem^>^ DataGroup::Items::get()
//{
//	return _items;
//}
//
//IVector<DataItem^>^ DataGroup::TopItems::get()
//{
//	return _topitems;
//}

//
// DataSource
//

//typedef Platform::String^ hello(Platform::String^);
DataSource::DataSource()
{
	_allGroups = ref new Vector<DataGroup^>();
	auto loader = ref new Windows::ApplicationModel::Resources::ResourceLoader();

	auto strTitle = loader->GetString("WiFiSetting");
	auto group1 = ref new DataGroup("WiFiSetting",
		strTitle,
		"Assets/NETGER 首页小图标/wireless.png");
	_allGroups->Append(group1);

	strTitle = loader->GetString("GuestAccess");
	auto group2 = ref new DataGroup("GuestAccess",
		strTitle,
		"Assets/NETGER 首页小图标/guestaccess.png");
	_allGroups->Append(group2);

	strTitle = loader->GetString("NetworkMap");
	auto group3 = ref new DataGroup("NetworkMap",
		strTitle,
		"Assets/NETGER 首页小图标/map.png");
	_allGroups->Append(group3);

	strTitle = loader->GetString("ParentalControl");
	auto group4 = ref new DataGroup("ParentalControl",
		strTitle,
		"Assets/NETGER 首页小图标/parentalcontrols.png");
	_allGroups->Append(group4);

	strTitle = loader->GetString("TrafficControl");
	auto group5 = ref new DataGroup("TrafficControl",
		strTitle,
		"Assets/NETGER 首页小图标/traffic.png");
	_allGroups->Append(group5);

	strTitle = loader->GetString("MyMedia");
	auto group6 = ref new DataGroup("MyMedia",
		strTitle,
		"Assets/NETGER 首页小图标/mymedia.png");
	_allGroups->Append(group6);

	strTitle = loader->GetString("QRCode");
	auto group7 = ref new DataGroup("QRCode",
		strTitle,
		"Assets/NETGER 首页小图标/qrcode.png");
	_allGroups->Append(group7);

	strTitle = loader->GetString("MarketPlace");
	auto group8 = ref new DataGroup("MarketPlace",
		strTitle,
		"Assets/NETGER 首页小图标/appstore.png");
	_allGroups->Append(group8);
}

IObservableVector<DataGroup^>^ DataSource::AllGroups::get()
{
	return _allGroups;
}

static DataSource^ _dataSource = nullptr;

void DataSource::Init()
{
	if (_dataSource == nullptr)
	{
		_dataSource = ref new DataSource();
	}
}
IIterable<DataGroup^>^ DataSource::GetGroups(String^ uniqueId)
{
	Init();
	String^ AllGroupsId = "AllGroups";
	if (!AllGroupsId->Equals(uniqueId)) throw ref new InvalidArgumentException("Only 'AllGroups' is supported as a collection of groups");

	return _dataSource->AllGroups;
}

DataGroup^ DataSource::GetGroup(String^ uniqueId)
{
	Init();
	// 对于小型数据集可接受简单线性搜索
	for each (auto group in _dataSource->AllGroups)
	{
		if (group->UniqueId->Equals(uniqueId)) return group;
	}
	return nullptr;
}

//DataItem^ DataSource::GetItem(String^ uniqueId)
//{
//	Init();
//	// 对于小型数据集可接受简单线性搜索
//	for each (auto group in _dataSource->AllGroups)
//	{
//		for each (auto item in group->Items)
//		{
//			if (item->UniqueId->Equals(uniqueId)) return item;
//		}
//	}
//	return nullptr;
//}
