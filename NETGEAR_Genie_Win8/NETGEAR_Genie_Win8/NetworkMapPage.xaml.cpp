//
// NetworkMapPage.xaml.cpp
// NetworkMapPage 类的实现
//

#include "pch.h"
#include "NetworkMapPage.xaml.h"

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
using namespace Windows::UI::Xaml::Shapes;
using namespace Windows::UI;
using namespace Windows::UI::Xaml::Media::Imaging;

// “基本页”项模板在 http://go.microsoft.com/fwlink/?LinkId=234237 上有介绍

NetworkMapPage::NetworkMapPage()
{
	InitializeComponent();
	
	//Grid^ mapGrid_1 = ref new Grid();
	//Button^ RouterButton1 = ref new Button();
	//RouterButton1->Content = "Router";
	//RouterButton1->SetValue(RouterButton1->WidthProperty, 200);
	//RouterButton1->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Center;
	//RouterButton1->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Center;
	//Platform::String^ a = RouterButton1->GetValue(RouterButton1->WidthProperty)->ToString();
	//RouterButton1->Margin = Windows::UI::Xaml::Thickness(100,0,0,0);
	//mapGrid_1->Children->Append(RouterButton1);
	//Button^ RouterButton2 = ref new Button();
	//Grid^ mapGrid_2 = ref new Grid();
	//RouterButton2->Content = "Router";
	//RouterButton2->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
	//RouterButton2->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
	//mapGrid_2->Children->Append(RouterButton2);
	//MapFlipView->Items->Append(mapGrid_1);
	//MapFlipView->Items->Append(mapGrid_2);
}

void NetworkMapPage::OnWindowSizeChanged(Platform::Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs e)
{
	//double Appwidth = e.Size.Width;
	//double AppHeight = e.Size.Height;	
	MapFlipView->Items->Clear();
	Grid^ map1 = ref new Grid();
	Grid^ map2 = ref new Grid();
	//map1->Children->Clear();
	double width = Window::Current->Bounds.Width;
	double height = Window::Current->Bounds.Height - 140;
	//map1
	Line^ line_1 = ref new Line();
	line_1->X1 = width/2; line_1->Y1 = height/2;
	line_1->X2 = width-100; line_1->Y2 = height/2;
	line_1->Stroke = ref new SolidColorBrush(Colors::Black);
	line_1->StrokeThickness = 2;	
	map1->Children->Append(line_1);
	Line^ line_2 = ref new Line();
	line_2->X1 = width/2; line_2->Y1 = height/2;
	line_2->X2 = width-200; line_2->Y2 = 150;
	line_2->Stroke = ref new SolidColorBrush(Colors::Black);
	line_2->StrokeThickness = 2;	
	map1->Children->Append(line_2);
	Line^ line_3 = ref new Line();
	line_3->X1 = width/2; line_3->Y1 = height/2;
	line_3->X2 = width/2; line_3->Y2 = 100;
	line_3->Stroke = ref new SolidColorBrush(Colors::Black);
	line_3->StrokeThickness = 2;	
	map1->Children->Append(line_3);
	Line^ line_4 = ref new Line();
	line_4->X1 = width/2; line_4->Y1 = height/2;
	line_4->X2 = 100; line_4->Y2 = height/2;
	line_4->Stroke = ref new SolidColorBrush(Colors::Black);
	line_4->StrokeThickness = 2;
	map1->Children->Append(line_4);
	Line^ line_5 = ref new Line();
	line_5->X1 = width/2; line_5->Y1 = height/2;
	line_5->X2 = width/2; line_5->Y2 = height-100;
	line_5->Stroke = ref new SolidColorBrush(Colors::Black);
	line_5->StrokeThickness = 2;	
	map1->Children->Append(line_5);
	Line^ line_6 = ref new Line();
	line_6->X1 = width/2; line_6->Y1 = height/2;
	line_6->X2 = 200; line_6->Y2 = 150;
	line_6->Stroke = ref new SolidColorBrush(Colors::Black);
	line_6->StrokeThickness = 2;	
	map1->Children->Append(line_6);	
	Line^ line_7 = ref new Line();
	line_7->X1 = width/2; line_7->Y1 = height/2;
	line_7->X2 = 200; line_7->Y2 = height-150;
	line_7->Stroke = ref new SolidColorBrush(Colors::Black);
	line_7->StrokeThickness = 2;	
	map1->Children->Append(line_7);	
	Line^ line_8 = ref new Line();
	line_8->X1 = width/2; line_8->Y1 = height/2;
	line_8->X2 = width-200; line_8->Y2 = height-150;
	line_8->Stroke = ref new SolidColorBrush(Colors::Black);
	line_8->StrokeThickness = 2;	
	map1->Children->Append(line_8);

	Image^ internet = ref new Image();
	static Uri^ _baseUri = ref new Uri("ms-appx:///");
	internet->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/internet72.png"));
	internet->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Right;
	internet->Margin = Windows::UI::Xaml::Thickness(0,0,50,0);
	internet->Width = 100; internet->Height = 100;
	map1->Children->Append(internet);

	Button^ BtnRouter = ref new Button();
	BtnRouter->SetValue(BtnRouter->WidthProperty, 150);
	BtnRouter->SetValue(BtnRouter->HeightProperty, 150);
	BtnRouter->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Center;
	BtnRouter->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Center;
	Image^ imgRouter = ref new Image();
	imgRouter->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/repeater72.png"));
	imgRouter->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	//Grid^ gridRouter = ref new Grid();
	//gridRouter->Children->Append(imgDevice);
	BtnRouter->Content = imgRouter;
	BtnRouter->Margin = Windows::UI::Xaml::Thickness(0,0,0,0);
	map1->Children->Append(BtnRouter);

	Button^ BtnDevice_1 = ref new Button();
	BtnDevice_1->SetValue(BtnDevice_1->WidthProperty, 100);
	BtnDevice_1->SetValue(BtnDevice_1->HeightProperty, 100);
	BtnDevice_1->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Right;
	BtnDevice_1->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
	BtnDevice_1->Margin = Windows::UI::Xaml::Thickness(0,100,150,0);
	Image^ imgDevice1 = ref new Image();
	imgDevice1->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/AndroidPhone72.png"));
	imgDevice1->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText1 = ref new TextBlock();
	DeviceNameText1->Text = "android-25531554966beee3";
	StackPanel^ stpDevice1 = ref new StackPanel();
	stpDevice1->Children->Append(imgDevice1);
	stpDevice1->Children->Append(DeviceNameText1);
	BtnDevice_1->Content = stpDevice1;
	map1->Children->Append(BtnDevice_1);

	Button^ BtnDevice_2 = ref new Button();
	BtnDevice_2->SetValue(BtnDevice_2->WidthProperty, 100);
	BtnDevice_2->SetValue(BtnDevice_2->HeightProperty, 100);
	BtnDevice_2->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Center;
	BtnDevice_2->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
	BtnDevice_2->Margin = Windows::UI::Xaml::Thickness(0,50,0,0);
	Image^ imgDevice2 = ref new Image();
	imgDevice2->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
	imgDevice2->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText2 = ref new TextBlock();
	DeviceNameText2->Text = "device_2";
	StackPanel^ stpDevice2 = ref new StackPanel();
	stpDevice2->Children->Append(imgDevice2);
	stpDevice2->Children->Append(DeviceNameText2);
	BtnDevice_2->Content = stpDevice2;
	map1->Children->Append(BtnDevice_2);

	Button^ BtnDevice_3 = ref new Button();
	BtnDevice_3->SetValue(BtnDevice_3->WidthProperty, 100);
	BtnDevice_3->SetValue(BtnDevice_3->HeightProperty, 100);
	BtnDevice_3->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
	BtnDevice_3->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Center;
	BtnDevice_3->Margin = Windows::UI::Xaml::Thickness(50,0,0,0);
	Image^ imgDevice3 = ref new Image();
	imgDevice3->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
	imgDevice3->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText3 = ref new TextBlock();
	DeviceNameText3->Text = "device_3";
	StackPanel^ stpDevice3 = ref new StackPanel();
	stpDevice3->Children->Append(imgDevice3);
	stpDevice3->Children->Append(DeviceNameText3);
	BtnDevice_3->Content = stpDevice3;
	map1->Children->Append(BtnDevice_3);

	Button^ BtnDevice_4 = ref new Button();
	BtnDevice_4->SetValue(BtnDevice_4->WidthProperty, 100);
	BtnDevice_4->SetValue(BtnDevice_4->HeightProperty, 100);
	BtnDevice_4->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Center;
	BtnDevice_4->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Bottom;
	BtnDevice_4->Margin = Windows::UI::Xaml::Thickness(0,0,0,50);
	Image^ imgDevice4 = ref new Image();
	imgDevice4->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
	imgDevice4->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText4 = ref new TextBlock();
	DeviceNameText4->Text = "device_4";
	StackPanel^ stpDevice4 = ref new StackPanel();
	stpDevice4->Children->Append(imgDevice4);
	stpDevice4->Children->Append(DeviceNameText4);
	BtnDevice_4->Content = stpDevice4;
	map1->Children->Append(BtnDevice_4);

	Button^ BtnDevice_5 = ref new Button();
	BtnDevice_5->SetValue(BtnDevice_5->WidthProperty, 100);
	BtnDevice_5->SetValue(BtnDevice_5->HeightProperty, 100);
	BtnDevice_5->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
	BtnDevice_5->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
	BtnDevice_5->Margin = Windows::UI::Xaml::Thickness(150,100,0,0);
	Image^ imgDevice5 = ref new Image();
	imgDevice5->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
	imgDevice5->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText5 = ref new TextBlock();
	DeviceNameText5->Text = "device_5";
	StackPanel^ stpDevice5 = ref new StackPanel();
	stpDevice5->Children->Append(imgDevice5);
	stpDevice5->Children->Append(DeviceNameText5);
	BtnDevice_5->Content = stpDevice5;
	map1->Children->Append(BtnDevice_5);

	Button^ BtnDevice_6 = ref new Button();
	BtnDevice_6->SetValue(BtnDevice_6->WidthProperty, 100);
	BtnDevice_6->SetValue(BtnDevice_6->HeightProperty, 100);
	BtnDevice_6->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
	BtnDevice_6->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Bottom;
	BtnDevice_6->Margin = Windows::UI::Xaml::Thickness(150,0,0,100);
	Image^ imgDevice6 = ref new Image();
	imgDevice6->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
	imgDevice6->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText6 = ref new TextBlock();
	DeviceNameText6->Text = "device_6";
	StackPanel^ stpDevice6 = ref new StackPanel();
	stpDevice6->Children->Append(imgDevice6);
	stpDevice6->Children->Append(DeviceNameText6);
	BtnDevice_6->Content = stpDevice6;
	map1->Children->Append(BtnDevice_6);

	Button^ BtnDevice_7 = ref new Button();
	BtnDevice_7->SetValue(BtnDevice_7->WidthProperty, 100);
	BtnDevice_7->SetValue(BtnDevice_7->HeightProperty, 100);
	BtnDevice_7->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Right;
	BtnDevice_7->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Bottom;
	BtnDevice_7->Margin = Windows::UI::Xaml::Thickness(0,0,150,100);
	Image^ imgDevice7 = ref new Image();
	imgDevice7->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
	imgDevice7->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText7 = ref new TextBlock();
	DeviceNameText7->Text = "device_7";
	StackPanel^ stpDevice7 = ref new StackPanel();
	stpDevice7->Children->Append(imgDevice7);
	stpDevice7->Children->Append(DeviceNameText7);
	BtnDevice_7->Content = stpDevice7;
	map1->Children->Append(BtnDevice_7);

	//map2
	Line^ line_21 = ref new Line();
	line_21->X1 = width/2; line_21->Y1 = height/2;
	line_21->X2 = width-100; line_21->Y2 = height/2;
	line_21->Stroke = ref new SolidColorBrush(Colors::Black);
	line_21->StrokeThickness = 2;	
	map2->Children->Append(line_21);
	Line^ line_22 = ref new Line();
	line_22->X1 = width/2; line_22->Y1 = height/2;
	line_22->X2 = width-200; line_22->Y2 = 150;
	line_22->Stroke = ref new SolidColorBrush(Colors::Black);
	line_22->StrokeThickness = 2;	
	map2->Children->Append(line_22);
	Line^ line_23 = ref new Line();
	line_23->X1 = width/2; line_23->Y1 = height/2;
	line_23->X2 = width/2; line_23->Y2 = 100;
	line_23->Stroke = ref new SolidColorBrush(Colors::Black);
	line_23->StrokeThickness = 2;	
	map2->Children->Append(line_23);
	Line^ line_24 = ref new Line();
	line_24->X1 = width/2; line_24->Y1 = height/2;
	line_24->X2 = 100; line_24->Y2 = height/2;
	line_24->Stroke = ref new SolidColorBrush(Colors::Black);
	line_24->StrokeThickness = 2;
	map2->Children->Append(line_24);
	Line^ line_25 = ref new Line();
	line_25->X1 = width/2; line_25->Y1 = height/2;
	line_25->X2 = width/2; line_25->Y2 = height-100;
	line_25->Stroke = ref new SolidColorBrush(Colors::Black);
	line_25->StrokeThickness = 2;	
	map2->Children->Append(line_25);

	Image^ internet2 = ref new Image();
	//static Uri^ _baseUri = ref new Uri("ms-appx:///");
	internet2->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/internet72.png"));
	internet2->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Right;
	internet2->Margin = Windows::UI::Xaml::Thickness(0,0,50,0);
	internet2->Width = 100; internet2->Height = 100;
	map2->Children->Append(internet2);

	Button^ BtnRouter2 = ref new Button();
	BtnRouter2->SetValue(BtnRouter2->WidthProperty, 150);
	BtnRouter2->SetValue(BtnRouter2->HeightProperty, 150);
	BtnRouter2->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Center;
	BtnRouter2->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Center;
	Image^ imgRouter2 = ref new Image();
	imgRouter2->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/repeater72.png"));
	imgRouter2->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	BtnRouter2->Content = imgRouter2;
	BtnRouter2->Margin = Windows::UI::Xaml::Thickness(0,0,0,0);
	map2->Children->Append(BtnRouter2);

	Button^ BtnDevice_21 = ref new Button();
	BtnDevice_21->SetValue(BtnDevice_21->WidthProperty, 100);
	BtnDevice_21->SetValue(BtnDevice_21->HeightProperty, 100);
	BtnDevice_21->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Right;
	BtnDevice_21->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
	BtnDevice_21->Margin = Windows::UI::Xaml::Thickness(0,100,150,0);
	Image^ imgDevice21 = ref new Image();
	imgDevice21->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/AndroidPhone72.png"));
	imgDevice21->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText21 = ref new TextBlock();
	DeviceNameText21->Text = "android-25531554966beee3";
	StackPanel^ stpDevice21 = ref new StackPanel();
	stpDevice21->Children->Append(imgDevice21);
	stpDevice21->Children->Append(DeviceNameText21);
	BtnDevice_21->Content = stpDevice21;
	map2->Children->Append(BtnDevice_21);

	Button^ BtnDevice_22 = ref new Button();
	BtnDevice_22->SetValue(BtnDevice_22->WidthProperty, 100);
	BtnDevice_22->SetValue(BtnDevice_22->HeightProperty, 100);
	BtnDevice_22->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Center;
	BtnDevice_22->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
	BtnDevice_22->Margin = Windows::UI::Xaml::Thickness(0,50,0,0);
	Image^ imgDevice22 = ref new Image();
	imgDevice22->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
	imgDevice22->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText22 = ref new TextBlock();
	DeviceNameText22->Text = "device_8";
	StackPanel^ stpDevice22 = ref new StackPanel();
	stpDevice22->Children->Append(imgDevice22);
	stpDevice22->Children->Append(DeviceNameText22);
	BtnDevice_22->Content = stpDevice22;
	map2->Children->Append(BtnDevice_22);

	Button^ BtnDevice_23 = ref new Button();
	BtnDevice_23->SetValue(BtnDevice_23->WidthProperty, 100);
	BtnDevice_23->SetValue(BtnDevice_23->HeightProperty, 100);
	BtnDevice_23->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
	BtnDevice_23->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Center;
	BtnDevice_23->Margin = Windows::UI::Xaml::Thickness(50,0,0,0);
	Image^ imgDevice23 = ref new Image();
	imgDevice23->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
	imgDevice23->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText23 = ref new TextBlock();
	DeviceNameText23->Text = "device_9";
	StackPanel^ stpDevice23 = ref new StackPanel();
	stpDevice23->Children->Append(imgDevice23);
	stpDevice23->Children->Append(DeviceNameText23);
	BtnDevice_23->Content = stpDevice23;
	map2->Children->Append(BtnDevice_23);

	Button^ BtnDevice_24 = ref new Button();
	BtnDevice_24->SetValue(BtnDevice_24->WidthProperty, 100);
	BtnDevice_24->SetValue(BtnDevice_24->HeightProperty, 100);
	BtnDevice_24->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Center;
	BtnDevice_24->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Bottom;
	BtnDevice_24->Margin = Windows::UI::Xaml::Thickness(0,0,0,50);
	Image^ imgDevice24 = ref new Image();
	imgDevice24->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
	imgDevice24->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
	TextBlock^ DeviceNameText24 = ref new TextBlock();
	DeviceNameText24->Text = "device_10";
	StackPanel^ stpDevice24 = ref new StackPanel();
	stpDevice24->Children->Append(imgDevice24);
	stpDevice24->Children->Append(DeviceNameText24);
	BtnDevice_24->Content = stpDevice24;
	map2->Children->Append(BtnDevice_24);

	MapFlipView->Items->Append(map1);
	MapFlipView->Items->Append(map2);
}
/// <summary>
/// 使用在导航过程中传递的内容填充页。在从以前的会话
/// 重新创建页时，也会提供任何已保存状态。
/// </summary>
/// <param name="navigationParameter">最初请求此页时传递给
/// <see cref="Frame::Navigate(Type, Object)"/> 的参数值。
/// </param>
/// <param name="pageState">此页面在之前的会话期间保留的状态
///映射。首次访问页面时为 null。</param>
void NetworkMapPage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
	(void) navigationParameter;	// 未使用的参数
	(void) pageState;	// 未使用的参数
}

/// <summary>
/// 保留与此页关联的状态，以防挂起应用程序或
/// 从导航缓存中放弃此页。值必须符合
/// <see cref="SuspensionManager::SessionState"/> 的序列化要求。
/// </summary>
/// <param name="pageState">要使用可序列化状态填充的空映射。</param>
void NetworkMapPage::SaveState(IMap<String^, Object^>^ pageState)
{
	(void) pageState;	// 未使用的参数
}
