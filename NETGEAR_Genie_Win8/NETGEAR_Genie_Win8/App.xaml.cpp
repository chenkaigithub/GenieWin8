//
// App.xaml.cpp
// App 类的实现
//

#include "pch.h"
#include "Common\SuspensionManager.h"
#include "MainPage.xaml.h"

#include <ppltasks.h>
#include "FileOpenPickerPage.xaml.h"

using namespace NETGEAR_Genie_Win8;
using namespace NETGEAR_Genie_Win8::Common;

using namespace Concurrency;
using namespace Platform;
using namespace Windows::ApplicationModel;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Core;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;

// “拆分布局应用程序”模板在 http://go.microsoft.com/fwlink/?LinkId=234228 上提供

/// <summary>
/// 初始化单一实例应用程序对象。这是执行的创作代码的第一行，
/// 逻辑上等同于 main() 或 WinMain()。
/// </summary>
App::App()
{
	InitializeComponent();
	Suspending += ref new SuspendingEventHandler(this, &App::OnSuspending);
}

/// <summary>
/// 在应用程序由最终用户正常启动时进行调用。
/// 当启动应用程序以执行打开特定的文件或显示搜索结果等其他操作时，
/// 将使用其他入口点。
/// </summary>
/// <param name="args">有关启动请求和过程的详细信息。</param>
void App::OnLaunched(LaunchActivatedEventArgs^ args)
{
	auto rootFrame = dynamic_cast<Frame^>(Window::Current->Content);

	// 不要在窗口已包含内容时重复应用程序初始化，
	// 只需确保窗口处于活动状态
	if (rootFrame == nullptr)
	{
		// 创建一个 Frame 以用作导航上下文并将其与
		// SuspensionManager 键关联
		rootFrame = ref new Frame();
		SuspensionManager::RegisterFrame(rootFrame, "AppFrame");

		auto prerequisite = task<void>([](){});
		if (args->PreviousExecutionState == ApplicationExecutionState::Terminated)
		{
			// 适当时，仅还原已保存的会话状态，计划
			//还原完成后的最终启动步骤
			prerequisite = SuspensionManager::RestoreAsync();
		}
		prerequisite.then([=](task<void> prerequisite)
		{
			try
			{
				prerequisite.get();
			}
			catch (Platform::Exception^)
			{
				//还原状态时出现问题。
				//假定没有状态并继续
			}

			if (rootFrame->Content == nullptr)
			{
				// 当未还原导航堆栈时，导航到第一页，
				// 并通过将所需信息作为导航参数传入来配置
				//新页
				if (!rootFrame->Navigate(TypeName(MainPage::typeid), "AllGroups"))
				{
					throw ref new FailureException("Failed to create initial page");
				}
			}
			// 将框架放在当前窗口中
			Window::Current->Content = rootFrame;
			// 确保当前窗口处于活动状态
			Window::Current->Activate();

		}, task_continuation_context::use_current());
	}
	else
	{
		if (rootFrame->Content == nullptr)
		{
			// 当未还原导航堆栈时，导航到第一页，
			// 并通过将所需信息作为导航参数传入来配置
			// 参数
			if (!rootFrame->Navigate(TypeName(MainPage::typeid), "AllGroups"))
			{
				throw ref new FailureException("Failed to create initial page");
			}
		}
		// 确保当前窗口处于活动状态
		Window::Current->Activate();
	}
}

/// <summary>
/// 在将要挂起应用程序执行时调用。在不知道应用程序
/// 将被终止还是恢复的情况下保存应用程序状态，
/// 并让内存内容保持不变。
/// </summary>
/// <param name="sender">挂起的请求的源。</param>
/// <param name="e">有关挂起的请求的详细信息。</param>
void App::OnSuspending(Object^ sender, SuspendingEventArgs^ e)
{
	(void) sender;	// 未使用的参数

	auto deferral = e->SuspendingOperation->GetDeferral();
	SuspensionManager::SaveAsync().then([=]()
	{
		deferral->Complete();
	});
}

/// <summary>
/// 在激活应用程序以显示文件打开选取器时调用。
/// </summary>
/// <param name="args">有关激活请求的详细信息。</param>
void NETGEAR_Genie_Win8::App::OnFileOpenPickerActivated(Windows::ApplicationModel::Activation::FileOpenPickerActivatedEventArgs^ args)
{
	auto fileOpenPickerPage = ref new NETGEAR_Genie_Win8::FileOpenPickerPage();
	fileOpenPickerPage->Activate(args);
}
