#include "pch.h"
#include "BooleanToVisibilityConverter.h"

using namespace NETGEAR_Genie_Win8::Common;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Interop;

Object^ BooleanToVisibilityConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	(void) targetType;	// 未使用的参数
	(void) parameter;	// 未使用的参数
	(void) language;	// 未使用的参数

	auto boxedBool = dynamic_cast<Box<bool>^>(value);
	auto boolValue = (boxedBool != nullptr && boxedBool->Value);
	return (boolValue ? Visibility::Visible : Visibility::Collapsed);
}

Object^ BooleanToVisibilityConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	(void) targetType;	// 未使用的参数
	(void) parameter;	// 未使用的参数
	(void) language;	// 未使用的参数

	auto visibility = dynamic_cast<Box<Visibility>^>(value);
	return (visibility != nullptr && visibility->Value == Visibility::Visible);
}
