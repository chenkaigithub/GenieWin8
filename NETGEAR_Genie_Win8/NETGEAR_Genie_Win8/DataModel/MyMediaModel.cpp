#include "pch.h"
#include "DataModel\MyMediaModel.h"

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
// MediaCommon
//

MediaCommon::MediaCommon(String^ uniqueId, String^ title, String^ imagePath)
{
	_uniqueId = uniqueId;
	_title = title;
	_imagePath = imagePath;
	_image = nullptr;
}

String^ MediaCommon::UniqueId::get()
{
	return _uniqueId;
}

void MediaCommon::UniqueId::set(String^ value)
{
	if (_uniqueId != value)
	{
		_uniqueId = value;
		OnPropertyChanged("UniqueId");
	}
}

String^ MediaCommon::Title::get()
{
	return _title;
}

void MediaCommon::Title::set(String^ value)
{
	if (_title != value)
	{
		_title = value;
		OnPropertyChanged("Title");
	}
}

ImageSource^ MediaCommon::Image::get()
{
	static Uri^ _baseUri = ref new Uri("ms-appx:///");

	if (_image == nullptr && _imagePath != nullptr)
	{
		_image = ref new BitmapImage(_baseUri->CombineUri(_imagePath));
	}
	return _image;
}

void MediaCommon::Image::set(ImageSource^ value)
{
	if (_image != value)
	{
		_image = value;
		_imagePath = nullptr;
		OnPropertyChanged("Image");
		PropertySet set;
	}
}

void MediaCommon::SetImage(String^ path)
{
	_image = nullptr;
	_imagePath = path;
	OnPropertyChanged("Image");
}

//
// SettingItem
//

MediaItem::MediaItem(String^ uniqueId, String^ title, String^ imagePath, String^ content, MyMediaGroup^ group)
	: MediaCommon(uniqueId, title, imagePath)
{
	_content = content;
	_group = group;
}

String^ MediaItem::Content::get()
{
	return _content;
}

void MediaItem::Content::set(String^ value)
{
	if (_content != value)
	{
		_content = value;
		OnPropertyChanged("Content");
	}
}

MyMediaGroup^ MediaItem::Group::get()
{
	return _group.Resolve<MyMediaGroup>();
}

void MediaItem::Group::set(MyMediaGroup^ value)
{
	if (Group != value)
	{
		_group = value;
		OnPropertyChanged("Group");
	}
}

//
// MyMediaGroup
//

MyMediaGroup::MyMediaGroup(String^ uniqueId, String^ title, String^ imagePath)
	: MediaCommon(uniqueId, title, imagePath)
{
	_items = ref new Vector<MediaItem^>();
}

IObservableVector<MediaItem^>^ MyMediaGroup::Items::get()
{
	return _items;
}

//
// MediaSource
//

MediaSource::MediaSource()
{
	_mymediaGroups = ref new Vector<MyMediaGroup^>();
	auto loader = ref new Windows::ApplicationModel::Resources::ResourceLoader();

	auto strTitle = loader->GetString("MyMediaSource");
	auto mymediagroup1 = ref new MyMediaGroup("MyMediaSource",
		strTitle,
		"Assets/ÏÂ±ßÀ¸/browse.png");
	mymediagroup1->Items->Append(ref new MediaItem("Source-1",
		"Source",
		"Assets/icon48.png",
		"ReadyDLNA: R6200",
		mymediagroup1));
	mymediagroup1->Items->Append(ref new MediaItem("Source-2",
		"Source",
		"Assets/icon48.png",
		"Genie Media Server (iPad Simulator)",
		mymediagroup1));
	mymediagroup1->Items->Append(ref new MediaItem("Source-3",
		"Source",
		"Assets/icon48.png",
		"Genie Media Server (HTC Incredible S)",
		mymediagroup1));
	_mymediaGroups->Append(mymediagroup1);

	strTitle = loader->GetString("MyMediaPlayer");
	auto mymediagroup2 = ref new MyMediaGroup("MyMediaPlayer",
		strTitle,
		"Assets/ÏÂ±ßÀ¸/device.png");
	mymediagroup2->Items->Append(ref new MediaItem("Player-1",
		"Player",
		"Assets/icon48.png",
		"Genie Media Player (GT-I9100)",
		mymediagroup2));
	mymediagroup2->Items->Append(ref new MediaItem("Player-2",
		"Player",
		"Assets/icon48.png",
		"Genie Media Player (iPad Simulator)",
		mymediagroup2));
	mymediagroup2->Items->Append(ref new MediaItem("Player-3",
		"Player",
		"Assets/icon48.png",
		"Genie Media Player (HTC Incredible S)",
		mymediagroup2));
	_mymediaGroups->Append(mymediagroup2);

	strTitle = loader->GetString("MyMediaPlaying");
	auto mymediagroup3 = ref new MyMediaGroup("MyMediaPlaying",
		strTitle,
		"Assets/ÏÂ±ßÀ¸/playing.png");
	_mymediaGroups->Append(mymediagroup3);

	strTitle = loader->GetString("MyMediaOption");
	auto mymediagrou4 = ref new MyMediaGroup("MyMediaOption",
		strTitle,
		"Assets/ÏÂ±ßÀ¸/option.png");
	_mymediaGroups->Append(mymediagrou4);
}

IObservableVector<MyMediaGroup^>^ MediaSource::MyMediaGroups::get()
{
	return _mymediaGroups;
}

static MediaSource^ _mediaSource = nullptr;

void MediaSource::Init()
{
	if (_mediaSource == nullptr)
	{
		_mediaSource = ref new MediaSource();
	}
}

IIterable<MyMediaGroup^>^ MediaSource::GetMymediaGroups(String^ uniqueId)
{
	Init();
	return _mediaSource->MyMediaGroups;
}

MyMediaGroup^ MediaSource::GetSourceGroup(String^ uniqueId)
{
	Init();
	for each (auto group in _mediaSource->MyMediaGroups)
	{
		if (group->UniqueId == "MyMediaSource") return group;
	}
	return nullptr;
}

MyMediaGroup^ MediaSource::GetPlayerGroup(String^ uniqueId)
{
	Init();
	for each (auto group in _mediaSource->MyMediaGroups)
	{
		if (group->UniqueId == "MyMediaPlayer") return group;
	}
	return nullptr;
}