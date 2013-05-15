#include "pch.h"
#include "DataModel\ParentalControlModel.h"

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
// FilterLevelCommon
//

FilterLevelCommon::FilterLevelCommon(String^ uniqueId, String^ title, String^ content)
{
	_uniqueId = uniqueId;
	_title = title;
	_content = content;
}

String^ FilterLevelCommon::UniqueId::get()
{
	return _uniqueId;
}

void FilterLevelCommon::UniqueId::set(String^ value)
{
	if (_uniqueId != value)
	{
		_uniqueId = value;
		OnPropertyChanged("UniqueId");
	}
}

String^ FilterLevelCommon::Title::get()
{
	return _title;
}

void FilterLevelCommon::Title::set(String^ value)
{
	if (_title != value)
	{
		_title = value;
		OnPropertyChanged("Title");
	}
}

String^ FilterLevelCommon::Content::get()
{
	return _content;
}

void FilterLevelCommon::Content::set(String^ value)
{
	if (_content != value)
	{
		_content = value;
		OnPropertyChanged("Content");
	}
}

//
// FilterLevelGroup
//

FilterLevelGroup::FilterLevelGroup(String^ uniqueId, String^ title, String^ content)
	: FilterLevelCommon(uniqueId, title, content)
{
}

//
// FilterLevelSource
//

FilterLevelSource::FilterLevelSource()
{
	
}

extern String^ filterLevel;
IObservableVector<FilterLevelGroup^>^ FilterLevelSource::FilterLevelGroups::get()
{	
	_filterLevelGroups = ref new Vector<FilterLevelGroup^>();
	auto loader = ref new Windows::ApplicationModel::Resources::ResourceLoader();

	auto strTitle = loader->GetString("FilterLevel");
	auto group = ref new FilterLevelGroup("FilterLevel",
		strTitle,
		filterLevel);
	_filterLevelGroups->Append(group);
	return _filterLevelGroups;
}

static FilterLevelSource^ _filterLevelSource = nullptr;

void FilterLevelSource::Init()
{
	if (_filterLevelSource == nullptr)
	{
		_filterLevelSource = ref new FilterLevelSource();
	}
}

IIterable<FilterLevelGroup^>^ FilterLevelSource::GetGroup(String^ uniqueId)
{
	Init();
	return _filterLevelSource->FilterLevelGroups;
}
