#include "pch.h"
#include "BooleanNegationConverter.h"

using namespace NETGEAR_Genie_Win8::Common;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml::Interop;

Object^ BooleanNegationConverter::Convert(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	(void) targetType;	// 未使用的参数
	(void) parameter;	// 未使用的参数
	(void) language;	// 未使用的参数

	auto boxedBool = dynamic_cast<Box<bool>^>(value);
	auto boolValue = (boxedBool != nullptr && boxedBool->Value);
	return !boolValue;
}

Object^ BooleanNegationConverter::ConvertBack(Object^ value, TypeName targetType, Object^ parameter, String^ language)
{
	(void) targetType;	// 未使用的参数
	(void) parameter;	// 未使用的参数
	(void) language;	// 未使用的参数

	auto boxedBool = dynamic_cast<Box<bool>^>(value);
	auto boolValue = (boxedBool != nullptr && boxedBool->Value);
	return !boolValue;
}
