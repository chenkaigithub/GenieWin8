#pragma once

#include <collection.h>

namespace GenieWin8Demo
{
	namespace Common
	{
		/// <summary>
		/// 提供几方面重要便利的 Page 的典型实现:
		/// <list type="bullet">
		/// <item>
		/// <description>应用程序视图状态到可视状态的映射</description>
		/// </item>
		/// <item>
		/// <description>GoBack、GoForward 和 GoHome 事件处理程序</description>
		/// </item>
		/// <item>
		/// <description>用于导航的鼠标和键盘快捷键</description>
		/// </item>
		/// <item>
		/// <description>用于导航和进程生命期管理的状态管理</description>
		/// </item>
		/// <item>
		/// <description>默认视图模型</description>
		/// </item>
		/// </list>
		/// </summary>
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class LayoutAwarePage : Windows::UI::Xaml::Controls::Page
		{
		internal:
			LayoutAwarePage();

		public:
			void StartLayoutUpdates(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void StopLayoutUpdates(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void InvalidateVisualState();
			static property Windows::UI::Xaml::DependencyProperty^ DefaultViewModelProperty
			{
				Windows::UI::Xaml::DependencyProperty^ get();
			};
			property Windows::Foundation::Collections::IObservableMap<Platform::String^, Platform::Object^>^ DefaultViewModel
			{
				Windows::Foundation::Collections::IObservableMap<Platform::String^, Platform::Object^>^ get();
				void set(Windows::Foundation::Collections::IObservableMap<Platform::String^, Platform::Object^>^ value);
			}

		protected:
			virtual void GoHome(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			virtual void GoBack(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			virtual void GoForward(Platform::Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			virtual Platform::String^ DetermineVisualState(Windows::UI::ViewManagement::ApplicationViewState viewState);
			virtual void OnNavigatedTo(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
			virtual void OnNavigatedFrom(Windows::UI::Xaml::Navigation::NavigationEventArgs^ e) override;
			virtual void LoadState(Platform::Object^ navigationParameter,
				Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState);
			virtual void SaveState(Windows::Foundation::Collections::IMap<Platform::String^, Platform::Object^>^ pageState);

		private:
			Platform::String^ _pageKey;
			bool _navigationShortcutsRegistered;
			Platform::Collections::Map<Platform::String^, Platform::Object^>^ _defaultViewModel;
			Windows::Foundation::EventRegistrationToken _windowSizeEventToken,
				_acceleratorKeyEventToken, _pointerPressedEventToken;
			Platform::Collections::Vector<Windows::UI::Xaml::Controls::Control^>^ _layoutAwareControls;
			void WindowSizeChanged(Platform::Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e);
			void OnLoaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void OnUnloaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e);
			void CoreDispatcher_AcceleratorKeyActivated(Windows::UI::Core::CoreDispatcher^ sender,
				Windows::UI::Core::AcceleratorKeyEventArgs^ args);
			void CoreWindow_PointerPressed(Windows::UI::Core::CoreWindow^ sender,
				Windows::UI::Core::PointerEventArgs^ args);
			LayoutAwarePage^ _this; // 对自身的强引用，已在 OnUnload 中清除
		};
	}
}
