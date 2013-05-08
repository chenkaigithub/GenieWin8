//
// PopupFilterLevel.xaml.h
// PopupFilterLevel 类的声明
//

#pragma once

#include "PopupFilterLevel.g.h"

namespace NETGEAR_Genie_Win8
{
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class PopupFilterLevel sealed
	{
	public:
		PopupFilterLevel();
		
	private:
		void RadioButton_Checked(Platform::Object^ sender,  Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
