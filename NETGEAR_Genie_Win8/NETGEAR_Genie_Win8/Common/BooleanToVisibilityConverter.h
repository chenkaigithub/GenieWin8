#pragma once

namespace NETGEAR_Genie_Win8
{
	namespace Common
	{
		/// <summary>
		/// 将 true 转换为 <see cref="Visibility::Visible"/> 并将 false 转换为
		/// <see cref="Visibility::Collapsed"/> 的值转换器。
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class BooleanToVisibilityConverter sealed : Windows::UI::Xaml::Data::IValueConverter
		{
		public:
			virtual Platform::Object^ Convert(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
			virtual Platform::Object^ ConvertBack(Platform::Object^ value, Windows::UI::Xaml::Interop::TypeName targetType, Platform::Object^ parameter, Platform::String^ language);
		};
	}
}
