//
// NetworkMapPage.xaml.h
// NetworkMapPage 类的声明
//

#pragma once

#include "Common\LayoutAwarePage.h" // 生成的页眉所必需的
#include "NetworkMapPage.g.h"

namespace NETGEAR_Genie_Win8
{
	/// <summary>
	/// 基本页，提供大多数应用程序通用的特性。
	/// </summary>
	public ref class NetworkMapPage sealed
	{
	public:
		NetworkMapPage();

	private:
		void OnWindowSizeChanged(Platform::Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs e);

	protected:
		virtual void LoadState(Platform::Object^ navigationParameter,
			Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
		virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState) override;
	};
}
