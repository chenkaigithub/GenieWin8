//
// PopupFilterLevel.xaml.cpp
// PopupFilterLevel 类的实现
//

#include "pch.h"
#include "PopupFilterLevel.xaml.h"

using namespace NETGEAR_Genie_Win8;

using namespace Platform;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// “用户控件”项模板在 http://go.microsoft.com/fwlink/?LinkId=234236 上提供

PopupFilterLevel::PopupFilterLevel()
{
	InitializeComponent();
}

String^ filterLevel;
void PopupFilterLevel::RadioButton_Checked(Object^ sender, RoutedEventArgs^ e)
{
	RadioButton^ rb = dynamic_cast<RadioButton^>(sender);
	filterLevel = dynamic_cast<String^>(rb->Content);
}