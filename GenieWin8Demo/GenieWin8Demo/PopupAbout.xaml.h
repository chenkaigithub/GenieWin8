//
// PopupAbout.xaml.h
// PopupAbout 类的声明
//

#pragma once

#include "PopupAbout.g.h"

namespace GenieWin8Demo
{
	[Windows::Foundation::Metadata::WebHostHidden]
	public ref class PopupAbout sealed
	{
	public:
		PopupAbout();

	private:
		void Policy_Click(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
	};
}
