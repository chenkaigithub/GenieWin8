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

//SettingItem::SettingItem(String^ uniqueId, String^ title, String^ content, SourcesGroup^ group)
//	: MediaCommon(uniqueId, title, content)
//{
//	_group = group;
//}
//
//SourcesGroup^ SettingItem::Group::get()
//{
//	return _group.Resolve<SourcesGroup>();
//}
//
//void SettingItem::Group::set(SourcesGroup^ value)
//{
//	if (Group != value)
//	{
//		_group = value;
//		OnPropertyChanged("Group");
//	}
//}

//
// MyMediaGroup
//

MyMediaGroup::MyMediaGroup(String^ uniqueId, String^ title, String^ imagePath)
	: MediaCommon(uniqueId, title, imagePath)
{
}

//
// SourcesGroup
//

SourcesGroup::SourcesGroup(String^ uniqueId, String^ title, String^ imagePath)
	: MediaCommon(uniqueId, title, imagePath)
{
	//_items = ref new Vector<SettingItem^>();
}

//
// PlayersGroup
//
PlayersGroup::PlayersGroup(String^ uniqueId, String^ title, String^ imagePath)
	: MediaCommon(uniqueId, title, imagePath)
{
}

//IObservableVector<SettingItem^>^ SourcesGroup::Items::get()
//{
//	return _items;
//}

//
// MediaSource
//

MediaSource::MediaSource()
{
	_mymediaGroups = ref new Vector<MyMediaGroup^>();
	_sourcesGroups = ref new Vector<SourcesGroup^>();
	_playersGroups = ref new Vector<PlayersGroup^>();
	auto loader = ref new Windows::ApplicationModel::Resources::ResourceLoader();

	auto strTitle = loader->GetString("MyMediaSource");
	auto mymediagroup1 = ref new MyMediaGroup("MyMediaSource",
		strTitle,
		"Assets/ÏÂ±ßÀ¸/browse.png");
	_mymediaGroups->Append(mymediagroup1);

	strTitle = loader->GetString("MyMediaPlayer");
	auto mymediagroup2 = ref new MyMediaGroup("MyMediaPlayer",
		strTitle,
		"Assets/ÏÂ±ßÀ¸/device.png");
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

	auto sourcegroup1 = ref new SourcesGroup("SourceGroup-1",
		"ReadyDLNA: R6200",
		"Assets/icon48.png");
	_sourcesGroups->Append(sourcegroup1);

	auto sourcegroup2 = ref new SourcesGroup("SourceGroup-2",
		"Genie Media Server (iPad Simulator)",
		"Assets/icon48.png");
	_sourcesGroups->Append(sourcegroup2);

	auto sourcegroup3 = ref new SourcesGroup("SourceGroup-3",
		"Genie Media Server (HTC Incredible S)",
		"Assets/icon48.png");
	_sourcesGroups->Append(sourcegroup3);

	auto playergroup1 = ref new PlayersGroup("PlayerGroup-1",
		"Genie Media Player (GT-I9100)",
		"Assets/icon48.png");
	_playersGroups->Append(playergroup1);

	auto playergroup2 = ref new PlayersGroup("PlayerGroup-2",
		"Genie Media Player (iPad Simulator)",
		"Assets/icon48.png");
	_playersGroups->Append(playergroup2);

	auto playergroup3 = ref new PlayersGroup("PlayerGroup-3",
		"Genie Media Player (HTC Incredible S)",
		"Assets/icon48.png");
	_playersGroups->Append(playergroup3);
}

IObservableVector<MyMediaGroup^>^ MediaSource::MyMediaGroups::get()
{
	return _mymediaGroups;
}

IObservableVector<SourcesGroup^>^ MediaSource::SourcesGroups::get()
{
	return _sourcesGroups;
}

IObservableVector<PlayersGroup^>^ MediaSource::PlayersGroups::get()
{
	return _playersGroups;
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

IIterable<SourcesGroup^>^ MediaSource::GetSourceGroups(String^ uniqueId)
{
	Init();
	return _mediaSource->SourcesGroups;
}

IIterable<PlayersGroup^>^ MediaSource::GetPlayerGroups(String^ uniqueId)
{
	Init();
	return _mediaSource->PlayersGroups;
}
