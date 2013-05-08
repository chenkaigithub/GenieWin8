#pragma once

namespace NETGEAR_Genie_Win8
{
	namespace Common
	{
		/// <summary>
		/// 从 true 转换为 false 以及进行相反转换的值转换器。
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class BooleanNegationConverter sealed : Windows::UI::Xaml::Data::IValueConverter
		{
		public:
			virtual Platform::Object^ Convert(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
			virtual Platform::Object^ ConvertBack(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
		};
	}
}
