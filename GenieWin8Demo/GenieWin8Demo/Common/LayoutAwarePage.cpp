#include "pch.h"
#include "LayoutAwarePage.h"
#include "SuspensionManager.h"

using namespace GenieWin8Demo::Common;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::System;
using namespace Windows::UI::Core;
using namespace Windows::UI::ViewManagement;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;
using namespace Windows::UI::Xaml::Navigation;

/// <summary>
/// 初始化 <see cref="LayoutAwarePage"/> 类的新实例。
/// </summary>
LayoutAwarePage::LayoutAwarePage()
{
	if (Windows::ApplicationModel::DesignMode::DesignModeEnabled)
	{
		return;
	}

	// 创建空默认视图模型
	DefaultViewModel = ref new Map<String^, Object^>(std::less<String^>());

	// 当此页是可视化树的一部分时，进行两个更改:
	// 1) 将应用程序视图状态映射到页的可视状态
	// 2) 处理键盘和鼠标导航请求
	Loaded += ref new RoutedEventHandler(this, &LayoutAwarePage::OnLoaded);

	// 当页不再可见时，撤消相同更改
	Unloaded += ref new RoutedEventHandler(this, &LayoutAwarePage::OnUnloaded);
}

static DependencyProperty^ _defaultViewModelProperty =
	DependencyProperty::Register("DefaultViewModel",
	TypeName(IObservableMap<String^, Object^>::typeid), TypeName(LayoutAwarePage::typeid), nullptr);

/// <summary>
/// 标识 <see cref="DefaultViewModel"/> 依赖属性。
/// </summary>
DependencyProperty^ LayoutAwarePage::DefaultViewModelProperty::get()
{
	return _defaultViewModelProperty;
}

/// <summary>
/// 获取 <see cref="IObservableMap&lt;String, Object&gt;"/> 的实现，该实现旨在
/// 用作普通视图模型。
/// </summary>
IObservableMap<String^, Object^>^ LayoutAwarePage::DefaultViewModel::get()
{
	return safe_cast<IObservableMap<String^, Object^>^>(GetValue(DefaultViewModelProperty));
}

/// <summary>
/// 获取 <see cref="IObservableMap&lt;String, Object&gt;"/> 的实现，该实现旨在
/// 用作普通视图模型。
/// </summary>
void LayoutAwarePage::DefaultViewModel::set(IObservableMap<String^, Object^>^ value)
{
	SetValue(DefaultViewModelProperty, value);
}

/// <summary>
/// 在页面属于可视化树的一部分时调用
/// </summary>
/// <param name="sender">触发事件的实例。</param>
/// <param name="e">描述导致事件的条件的事件数据。</param>
void LayoutAwarePage::OnLoaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	this->StartLayoutUpdates(sender, e);

	// 仅当占用整个窗口时，键盘和鼠标导航才适用
	if (this->ActualHeight == Window::Current->Bounds.Height &&
		this->ActualWidth == Window::Current->Bounds.Width)
	{
		// 直接侦听窗口，因此无需焦点
		_acceleratorKeyEventToken = Window::Current->CoreWindow->Dispatcher->AcceleratorKeyActivated +=
			ref new TypedEventHandler<CoreDispatcher^, AcceleratorKeyEventArgs^>(this,
			&LayoutAwarePage::CoreDispatcher_AcceleratorKeyActivated);
		_pointerPressedEventToken = Window::Current->CoreWindow->PointerPressed +=
			ref new TypedEventHandler<CoreWindow^, PointerEventArgs^>(this,
			&LayoutAwarePage::CoreWindow_PointerPressed);
		_navigationShortcutsRegistered = true;
	}
}

/// <summary>
/// 在页面从可视化树中删除时调用
/// </summary>
/// <param name="sender">触发事件的实例。</param>
/// <param name="e">描述导致事件的条件的事件数据。</param>
void LayoutAwarePage::OnUnloaded(Object^ sender, Windows::UI::Xaml::RoutedEventArgs^ e)
{
	if (_navigationShortcutsRegistered)
	{
		Window::Current->CoreWindow->Dispatcher->AcceleratorKeyActivated -= _acceleratorKeyEventToken;
		Window::Current->CoreWindow->PointerPressed -= _pointerPressedEventToken;
		_navigationShortcutsRegistered = false;
	}
	StopLayoutUpdates(sender, e);
}

#pragma region Navigation support

/// <summary>
/// 作为事件处理程序进行调用，以向后导航页的关联 <see cref="Frame"/>
/// 直至它达到导航堆栈顶部。
/// </summary>
/// <param name="sender">触发事件的实例。</param>
/// <param name="e">描述导致事件的条件的事件数据。</param>
void LayoutAwarePage::GoHome(Object^ sender, RoutedEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;	// 未使用的参数

	// 使用导航框架返回最顶层的页
	if (Frame != nullptr)
	{
		while (Frame->CanGoBack)
		{
			Frame->GoBack();
		}
	}
}

/// <summary>
/// 作为事件处理程序进行调用，以向后导航与此页的 <see cref="Frame"/>
/// 关联的导航堆栈。
/// </summary>
/// <param name="sender">触发事件的实例。</param>
/// <param name="e">描述导致事件的条件的事件数据。</param>
void LayoutAwarePage::GoBack(Object^ sender, RoutedEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;	// 未使用的参数

	// 使用导航框架返回上一页
	if (Frame != nullptr && Frame->CanGoBack)
	{
		Frame->GoBack();
	}
}

/// <summary>
/// 作为事件处理程序进行调用，以向后导航导航堆栈
/// 关联的导航堆栈。
/// </summary>
/// <param name="sender">触发事件的实例。</param>
/// <param name="e">描述导致事件的条件的事件数据。</param>
void LayoutAwarePage::GoForward(Object^ sender, RoutedEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;	// 未使用的参数

	// 使用导航框架前进至下一页
	if (Frame != nullptr && Frame->CanGoForward)
	{
		Frame->GoForward();
	}
}

/// <summary>
/// 当此页处于活动状态并占用整个窗口时，在每次
/// 击键(包括系统键，如 Alt 组合键)时调用。用于检测页之间的键盘
/// 导航(即使在页本身没有焦点时)。
/// </summary>
/// <param name="sender">触发事件的实例。</param>
/// <param name="args">描述导致事件的条件的事件数据。</param>
void LayoutAwarePage::CoreDispatcher_AcceleratorKeyActivated(CoreDispatcher^ sender,
															 AcceleratorKeyEventArgs^ args)
{
	auto virtualKey = args->VirtualKey;

	// 仅当按向左、向右或专用上一页或下一页键时才进一步
	// 调查
	if ((args->EventType == CoreAcceleratorKeyEventType::SystemKeyDown ||
		args->EventType == CoreAcceleratorKeyEventType::KeyDown) &&
		(virtualKey == VirtualKey::Left || virtualKey == VirtualKey::Right ||
		(int)virtualKey == 166 || (int)virtualKey == 167))
	{
		auto coreWindow = Window::Current->CoreWindow;
		auto downState = Windows::UI::Core::CoreVirtualKeyStates::Down;
		bool menuKey = (coreWindow->GetKeyState(VirtualKey::Menu) & downState) == downState;
		bool controlKey = (coreWindow->GetKeyState(VirtualKey::Control) & downState) == downState;
		bool shiftKey = (coreWindow->GetKeyState(VirtualKey::Shift) & downState) == downState;
		bool noModifiers = !menuKey && !controlKey && !shiftKey;
		bool onlyAlt = menuKey && !controlKey && !shiftKey;

		if (((int)virtualKey == 166 && noModifiers) ||
			(virtualKey == VirtualKey::Left && onlyAlt))
		{
			// 在按上一页键或 Alt+向左键时向后导航
			args->Handled = true;
			GoBack(this, ref new RoutedEventArgs());
		}
		else if (((int)virtualKey == 167 && noModifiers) ||
			(virtualKey == VirtualKey::Right && onlyAlt))
		{
			// 在按下一页键或 Alt+向右键时向前导航
			args->Handled = true;
			GoForward(this, ref new RoutedEventArgs());
		}
	}
}

/// <summary>
/// 当此页处于活动状态并占用整个窗口时，在每次鼠标单击、触摸屏点击
/// 或执行等效交互时调用。用于检测浏览器样式下一页和
/// 上一步鼠标按钮单击以在页之间导航。
/// </summary>
/// <param name="sender">触发事件的实例。</param>
/// <param name="args">描述导致事件的条件的事件数据。</param>
void LayoutAwarePage::CoreWindow_PointerPressed(CoreWindow^ sender, PointerEventArgs^ args)
{
	auto properties = args->CurrentPoint->Properties;

	// 忽略与鼠标左键、右键和中键的键关联
	if (properties->IsLeftButtonPressed || properties->IsRightButtonPressed ||
		properties->IsMiddleButtonPressed) return;

	// 如果按下后退或前进(但不是同时)，则进行相应导航
	bool backPressed = properties->IsXButton1Pressed;
	bool forwardPressed = properties->IsXButton2Pressed;
	if (backPressed ^ forwardPressed)
	{
		args->Handled = true;
		if (backPressed) GoBack(this, ref new RoutedEventArgs());
		if (forwardPressed) GoForward(this, ref new RoutedEventArgs());
	}
}

#pragma endregion

#pragma region Visual state switching

/// <summary>
/// 作为事件处理程序调用，通常是在页中发生 <see cref="Control"/> 的
/// <see cref="Loaded"/> 事件时，以指示发送方应开始接收
/// 与应用程序视图状态更改对应的可视状态管理更改。
/// </summary>
/// <param name="sender">支持与视图状态对应的可视状态管理的
/// <see cref="Control"/> 的实例。</param>
/// <param name="e">描述如何进行请求的事件数据。</param>
/// <remarks>在请求布局更新时，会立即使用当前视图状态
/// 设置对应可视状态。强烈建议使用连接到 <see cref="StopLayoutUpdates"/> 的对应
/// <see cref="Unloaded"/> 事件处理程序。
/// <see cref="LayoutAwarePage"/> 的实例会自动在其 Loaded 和 Unloaded 事件中调用这些处理
/// 程序。</remarks>
/// <seealso cref="DetermineVisualState"/>
/// <seealso cref="InvalidateVisualState"/>
void LayoutAwarePage::StartLayoutUpdates(Object^ sender, RoutedEventArgs^ e)
{
	(void) e;	// 未使用的参数

	auto control = safe_cast<Control^>(sender);
	if (_layoutAwareControls == nullptr)
	{
		// 当更新中存在相关控件时，开始侦听视图状态更改
		_layoutAwareControls = ref new Vector<Control^>();
		_windowSizeEventToken = Window::Current->SizeChanged += ref new WindowSizeChangedEventHandler(this, &LayoutAwarePage::WindowSizeChanged);

		// 页面接收子级的通知。请保护该页面，直到我们停止所有控件的布局更新。
		_this = this;
	}
	_layoutAwareControls->Append(control);

	// 设置控件的初始可视状态
	VisualStateManager::GoToState(control, DetermineVisualState(ApplicationView::Value), false);
}

void LayoutAwarePage::WindowSizeChanged(Object^ sender, Windows::UI::Core::WindowSizeChangedEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;	// 未使用的参数

	InvalidateVisualState();
}

/// <summary>
/// 作为事件处理程序调用，通常是发生 <see cref="Control"/> 的
/// <see cref="Unloaded"/> 事件时，以指示发送方应开始接收
/// 与应用程序视图状态更改对应的可视状态管理更改。
/// </summary>
/// <param name="sender">支持与视图状态对应的可视状态管理的
/// <see cref="Control"/> 的实例。</param>
/// <param name="e">描述如何进行请求的事件数据。</param>
/// <remarks>在请求布局更新时，会立即使用当前视图状态
/// 当请求布局更新时。</remarks>
/// <seealso cref="StartLayoutUpdates"/>
void LayoutAwarePage::StopLayoutUpdates(Object^ sender, RoutedEventArgs^ e)
{
	(void) e;	// 未使用的参数

	auto control = safe_cast<Control^>(sender);
	unsigned int index;
	if (_layoutAwareControls != nullptr && _layoutAwareControls->IndexOf(control, &index))
	{
		_layoutAwareControls->RemoveAt(index);
		if (_layoutAwareControls->Size == 0)
		{
			// 当更新中没有相关控件时，停止侦听视图状态更改
			Window::Current->SizeChanged -= _windowSizeEventToken;
			_layoutAwareControls = nullptr;
			// 上一个控件收到了卸载通知。
			_this = nullptr;
		}
	}
}

/// <summary>
/// 针对页中的可视状态管理将 <see cref="ApplicationViewState"/> 值转换为
/// 字符串。默认实现使用枚举值的名称。子类可以
/// 重写此方法以控制使用的映射方案。
/// </summary>
/// <param name="viewState">需要可视状态的视图状态。</param>
/// <returns>用于驱动 <see cref="VisualStateManager"/> 的可视状态名称</returns>
/// <seealso cref="InvalidateVisualState"/>
String^ LayoutAwarePage::DetermineVisualState(ApplicationViewState viewState)
{
	switch (viewState)
	{
	case ApplicationViewState::Filled:
		return "Filled";
	case ApplicationViewState::Snapped:
		return "Snapped";
	case ApplicationViewState::FullScreenPortrait:
		return "FullScreenPortrait";
	case ApplicationViewState::FullScreenLandscape:
	default:
		return "FullScreenLandscape";
	}
}

/// <summary>
/// 使用正确的可视状态更新侦听可视状态更改的所有
/// 控件。
/// </summary>
/// <remarks>
/// 通常与重写 <see cref="DetermineVisualState"/> 结合使用以
/// 通知可能返回了不同值，即使未更改视图状态也是如此。
/// </remarks>
void LayoutAwarePage::InvalidateVisualState()
{
	if (_layoutAwareControls != nullptr)
	{
		String^ visualState = DetermineVisualState(ApplicationView::Value);
		auto controlIterator = _layoutAwareControls->First();
		while (controlIterator->HasCurrent)
		{
			auto control = controlIterator->Current;
			VisualStateManager::GoToState(control, visualState, false);
			controlIterator->MoveNext();
		}
	}
}

#pragma endregion

#pragma region Process lifetime management

/// <summary>
/// 在此页将要在 Frame 中显示时进行调用。
/// </summary>
/// <param name="e">描述如何访问此页的事件数据。Parameter
/// 属性提供要显示的组。</param>
void LayoutAwarePage::OnNavigatedTo(NavigationEventArgs^ e)
{
	// 通过导航返回缓存页不应触发状态加载
	if (_pageKey != nullptr) return;

	auto frameState = SuspensionManager::SessionStateForFrame(Frame);
	_pageKey = "Page-" + Frame->BackStackDepth;

	if (e->NavigationMode == NavigationMode::New)
	{
		// 在向导航堆栈添加新页时清除向前导航的
		// 现有状态
		auto nextPageKey = _pageKey;
		int nextPageIndex = Frame->BackStackDepth;
		while (frameState->HasKey(nextPageKey))
		{
			frameState->Remove(nextPageKey);
			nextPageIndex++;
			nextPageKey = "Page-" + nextPageIndex;
		}

		// 将导航参数传递给新页
		LoadState(e->Parameter, nullptr);
	}
	else
	{
		// 通过将相同策略用于加载挂起状态并从缓存重新创建
		// 放弃的页，将导航参数和保留页状态传递
		// 给页
		LoadState(e->Parameter, safe_cast<IMap<String^, Object^>^>(frameState->Lookup(_pageKey)));
	}
}

/// <summary>
/// 当此页不再在 Frame 中显示时调用。
/// </summary>
/// <param name="e">描述如何访问此页的事件数据。Parameter
/// 属性提供要显示的组。</param>
void LayoutAwarePage::OnNavigatedFrom(NavigationEventArgs^ e)
{
	auto frameState = SuspensionManager::SessionStateForFrame(Frame);
	auto pageState = ref new Map<String^, Object^>();
	SaveState(pageState);
	frameState->Insert(_pageKey, pageState);
}

/// <summary>
/// 使用在导航过程中传递的内容填充页。在从以前的会话
/// 重新创建页时，也会提供任何已保存状态。
/// </summary>
/// <param name="navigationParameter">最初请求此页时传递给
/// <see cref="Frame.Navigate(Type, Object)"/> 的参数值。
/// </param>
/// <param name="pageState">此页面在之前的会话期间保留的状态
/// 字典。首次访问页面时为 null。</param>
void LayoutAwarePage::LoadState(Object^ navigationParameter, IMap<String^, Object^>^ pageState)
{
}

/// <summary>
/// 保留与此页关联的状态，以防挂起应用程序或
/// 从导航缓存中放弃此页。值必须符合
/// <see cref="SuspensionManager.SessionState"/> 的序列化要求。
/// </summary>
/// <param name="pageState">要使用可序列化状态填充的空映射。</param>
void LayoutAwarePage::SaveState(IMap<String^, Object^>^ pageState)
{
}

#pragma endregion
