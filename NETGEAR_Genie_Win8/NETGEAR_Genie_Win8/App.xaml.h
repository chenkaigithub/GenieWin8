//
// App.xaml.h
// App 类的声明
//

#pragma once

#include "App.g.h"
#include "ViewModels\MainPageModel.h" // App.xaml.cpp 及生成的代码所必需的

namespace NETGEAR_Genie_Win8
{
	/// <summary>
	/// 提供特定于应用程序的行为，以补充默认的应用程序类。
	/// </summary>
	ref class App sealed
	{
	public:
		App();
		virtual void OnLaunched(Windows::ApplicationModel::Activation::LaunchActivatedEventArgs^ args) override;

	private:
		void OnSuspending(Platform::Object^ sender, Windows::ApplicationModel::SuspendingEventArgs^ e);
	};
}
