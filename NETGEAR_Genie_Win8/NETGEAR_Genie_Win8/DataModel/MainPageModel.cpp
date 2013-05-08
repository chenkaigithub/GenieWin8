//
// MainPageModel.cpp
// DataSource、DataGroup、DataItem 和 DataCommon 类的实现
//

#include "pch.h"
#include "DataModel\MainPageModel.h"

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

DataSource::DataSource()
{
	_allGroups = ref new Vector<DataGroup^>();

	//String^ LONG_LOREM_IPSUM = "Curabitur class aliquam vestibulum nam curae maecenas sed integer cras phasellus suspendisse quisque donec dis praesent accumsan bibendum pellentesque condimentum adipiscing etiam consequat vivamus dictumst aliquam duis convallis scelerisque est parturient ullamcorper aliquet fusce suspendisse nunc hac eleifend amet blandit facilisi condimentum commodo scelerisque faucibus aenean ullamcorper ante mauris dignissim consectetuer nullam lorem vestibulum habitant conubia elementum pellentesque morbi facilisis arcu sollicitudin diam cubilia aptent vestibulum auctor eget dapibus pellentesque inceptos leo egestas interdum nulla consectetuer suspendisse adipiscing pellentesque proin lobortis sollicitudin augue elit mus congue fermentum parturient fringilla euismod feugiat";
	//String^ ITEM_CONTENT = "Item Content: " + LONG_LOREM_IPSUM + "\n\n" + LONG_LOREM_IPSUM + "\n\n" + LONG_LOREM_IPSUM + "\n\n" + LONG_LOREM_IPSUM + "\n\n" + LONG_LOREM_IPSUM + "\n\n" + LONG_LOREM_IPSUM + "\n\n" + LONG_LOREM_IPSUM;

	auto group1 = ref new DataGroup("WiFiSetting",
		"无线设置",
		"Assets/DarkGray.png");
	_allGroups->Append(group1);

	auto group2 = ref new DataGroup("GuestAccess",
		"访客访问",
		"Assets/LightGray.png");
	_allGroups->Append(group2);

	auto group3 = ref new DataGroup("NetworkMap",
		"网络映射",
		"Assets/MediumGray.png");
	_allGroups->Append(group3);

	auto group4 = ref new DataGroup("ParentalControl",
		"家长控制",
		"Assets/LightGray.png");
	_allGroups->Append(group4);

	auto group5 = ref new DataGroup("TrafficControl",
		"流量控制",
		"Assets/MediumGray.png");
	_allGroups->Append(group5);

	auto group6 = ref new DataGroup("MyMedia",
		"我的媒体",
		"Assets/DarkGray.png");
	_allGroups->Append(group6);

	auto group7 = ref new DataGroup("QRCode",
		"QR码",
		"Assets/DarkGray.png");
	_allGroups->Append(group7);
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
