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
	double PI = 3.141592653589793;
	MapFlipView->Items->Clear();
	double width = Window::Current->Bounds.Width;
	double height = Window::Current->Bounds.Height - 140;
	double r1 = width/2 - 100;
	double r2 = height/2 - 100;
	//m = （总设备数 - 1（路由器/交换机）-1（本设备））/ 6 的商
	//n 为上式得余数，即最后一页除路由器（交换机）和本设备外的设备数
	int m = 1;	//满设备(即为8)的映射页数，这里假定为1页
	int n = 3;	//最后一页设备数量，这里假定为3
	for(int i = 0; i < m + 1; i++)
	{
		Grid^ map = ref new Grid();

		Image^ internet = ref new Image();
		static Uri^ _baseUri = ref new Uri("ms-appx:///");
		internet->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/internet72.png"));
		internet->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Right;
		internet->Margin = Windows::UI::Xaml::Thickness(0,0,50,0);
		internet->Width = 100; internet->Height = 100;
		

		Button^ BtnRouter = ref new Button();
		BtnRouter->SetValue(BtnRouter->WidthProperty, 150);
		BtnRouter->SetValue(BtnRouter->HeightProperty, 150);
		BtnRouter->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Center;
		BtnRouter->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Center;
		Image^ imgRouter = ref new Image();
		imgRouter->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/repeater72.png"));
		imgRouter->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
		BtnRouter->Content = imgRouter;
		BtnRouter->Margin = Windows::UI::Xaml::Thickness(0,0,0,0);

		if (i != m)
		{	
			double Angle = 360.0 / 8;
			for (int j = 0; j < 8; j++)
			{
				double x = r1 * cos(j * Angle * PI / 180);
				double y = r2 * sin(j * Angle * PI / 180);
				Line^ line = ref new Line();
				line->X1 = width/2; line->Y1 = height/2;
				line->X2 = width/2 + x; line->Y2 = height/2 - y;
				line->Stroke = ref new SolidColorBrush(Colors::SeaGreen);
				line->StrokeThickness = 3;
				map->Children->Append(line);

				if (j == 1)
				{
					Button^ BtnDeviceLocal = ref new Button();
					BtnDeviceLocal->SetValue(BtnDeviceLocal->WidthProperty, 100);
					BtnDeviceLocal->SetValue(BtnDeviceLocal->HeightProperty, 100);
					BtnDeviceLocal->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
					BtnDeviceLocal->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
					BtnDeviceLocal->Margin = Windows::UI::Xaml::Thickness(width/2 + x - 50, height/2 - y - 50, 0, 0);
					Image^ imgDeviceLocal = ref new Image();
					imgDeviceLocal->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/AndroidPhone72.png"));
					imgDeviceLocal->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
					TextBlock^ DeviceNameTextLocal = ref new TextBlock();
					DeviceNameTextLocal->Text = "android-25531554966beee3";
					StackPanel^ stpDeviceLocal = ref new StackPanel();
					stpDeviceLocal->Children->Append(imgDeviceLocal);
					stpDeviceLocal->Children->Append(DeviceNameTextLocal);
					BtnDeviceLocal->Content = stpDeviceLocal;
					map->Children->Append(BtnDeviceLocal);
				}
				else if (j > 1)
				{
					Button^ BtnDevice = ref new Button();
					BtnDevice->SetValue(BtnDevice->WidthProperty, 100);
					BtnDevice->SetValue(BtnDevice->HeightProperty, 100);
					BtnDevice->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
					BtnDevice->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
					BtnDevice->Margin = Windows::UI::Xaml::Thickness(width/2 + x - 50, height/2 - y - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
					Image^ imgDevice = ref new Image();
					imgDevice->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
					imgDevice->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
					TextBlock^ DeviceNameText = ref new TextBlock();
					DeviceNameText->Text = "device-" + (6*i+j-1).ToString();
					StackPanel^ stpDevice = ref new StackPanel();
					stpDevice->Children->Append(imgDevice);
					stpDevice->Children->Append(DeviceNameText);
					BtnDevice->Content = stpDevice;
					map->Children->Append(BtnDevice);
				}				
			}
		}
		else
		{
			double Angle = 360.0 / (n + 2);
			for (int j = 0; j < n + 2; j++)
			{
				double x = r1 * cos(j * Angle * PI / 180);
				double y = r2 * sin(j * Angle * PI / 180);
				Line^ line = ref new Line();
				line->X1 = width/2; line->Y1 = height/2;
				line->X2 = width/2 + x; line->Y2 = height/2 - y;
				line->Stroke = ref new SolidColorBrush(Colors::SeaGreen);
				line->StrokeThickness = 3;
				map->Children->Append(line);

				if (j == 1)
				{
					Button^ BtnDeviceLocal = ref new Button();
					BtnDeviceLocal->SetValue(BtnDeviceLocal->WidthProperty, 100);
					BtnDeviceLocal->SetValue(BtnDeviceLocal->HeightProperty, 100);
					BtnDeviceLocal->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
					BtnDeviceLocal->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
					BtnDeviceLocal->Margin = Windows::UI::Xaml::Thickness(width/2 + x - 50, height/2 - y - 50, 0, 0);
					Image^ imgDeviceLocal = ref new Image();
					imgDeviceLocal->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/AndroidPhone72.png"));
					imgDeviceLocal->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
					TextBlock^ DeviceNameTextLocal = ref new TextBlock();
					DeviceNameTextLocal->Text = "android-25531554966beee3";
					StackPanel^ stpDeviceLocal = ref new StackPanel();
					stpDeviceLocal->Children->Append(imgDeviceLocal);
					stpDeviceLocal->Children->Append(DeviceNameTextLocal);
					BtnDeviceLocal->Content = stpDeviceLocal;
					map->Children->Append(BtnDeviceLocal);
				}
				else if (j > 1)
				{
					Button^ BtnDevice = ref new Button();
					BtnDevice->SetValue(BtnDevice->WidthProperty, 100);
					BtnDevice->SetValue(BtnDevice->HeightProperty, 100);
					BtnDevice->HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
					BtnDevice->VerticalAlignment = Windows::UI::Xaml::VerticalAlignment::Top;
					BtnDevice->Margin = Windows::UI::Xaml::Thickness(width/2 + x - 50, height/2 - y - 50, 0, 0);	// -50 为纠正由图标大小（100，100）造成的偏差
					Image^ imgDevice = ref new Image();
					imgDevice->Source = ref new BitmapImage(_baseUri->CombineUri("Assets/devices/networkdev72.png"));
					imgDevice->Stretch = Windows::UI::Xaml::Media::Stretch::UniformToFill;
					TextBlock^ DeviceNameText = ref new TextBlock();
					DeviceNameText->Text = "device-" + (6*m+j-1).ToString();
					StackPanel^ stpDevice = ref new StackPanel();
					stpDevice->Children->Append(imgDevice);
					stpDevice->Children->Append(DeviceNameText);
					BtnDevice->Content = stpDevice;
					map->Children->Append(BtnDevice);
				}
			}
		}
		map->Children->Append(internet);
		map->Children->Append(BtnRouter);
		MapFlipView->Items->Append(map);
	}
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
