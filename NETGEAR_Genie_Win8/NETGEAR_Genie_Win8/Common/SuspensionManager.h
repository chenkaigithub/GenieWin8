//
// SuspensionManager.h
// SuspensionManager 类的声明
//

#pragma once

#include <ppltasks.h>

namespace NETGEAR_Genie_Win8
{
	namespace Common
	{
		/// <summary>
		/// SuspensionManager 捕获全局会话状态以简化应用程序的
		/// 进程生命期管理。请注意会话状态在许多条件下将自动清除，
		/// 因此应该只用于存储方便
		/// 在会话之间传递但在应用程序崩溃或
		/// 已升级。
		/// </summary>
		ref class SuspensionManager sealed
		{
		internal:
			static void RegisterFrame(Windows::UI::Xaml::Controls::Frame^ frame, Platform::String^ sessionStateKey);
			static void UnregisterFrame(Windows::UI::Xaml::Controls::Frame^ frame);
			static Concurrency::task<void> SaveAsync(void);
			static Concurrency::task<void> RestoreAsync(void);
			static property Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ SessionState
			{
				Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ get(void);
			};
			static Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ SessionStateForFrame(
				Windows::UI::Xaml::Controls::Frame^ frame);

		private:
			static void RestoreFrameNavigationState(Windows::UI::Xaml::Controls::Frame^ frame);
			static void SaveFrameNavigationState(Windows::UI::Xaml::Controls::Frame^ frame);
		};
	}
}
