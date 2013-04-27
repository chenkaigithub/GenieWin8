#include "pch.h"
#include "BindableBase.h"

using namespace GenieWin8Demo::Common;

using namespace Platform;
using namespace Windows::UI::Xaml::Data;

/// <summary>
/// 向侦听器通知已更改了某个属性值。
/// </summary>
/// <param name="propertyName">用于通知侦听器的属性的名称。</param>
void BindableBase::OnPropertyChanged(String^ propertyName)
{
	PropertyChanged(this, ref new PropertyChangedEventArgs(propertyName));
}

Windows::UI::Xaml::Data::ICustomProperty^ BindableBase::GetCustomProperty(Platform::String^ name) 
{
	return nullptr;
}

Windows::UI::Xaml::Data::ICustomProperty^ BindableBase::GetIndexedProperty(Platform::String^ name, Windows::UI::Xaml::Interop::TypeName type) 
{
	return nullptr;
}

Platform::String^ BindableBase::GetStringRepresentation() 
{
	return this->ToString(); 
}