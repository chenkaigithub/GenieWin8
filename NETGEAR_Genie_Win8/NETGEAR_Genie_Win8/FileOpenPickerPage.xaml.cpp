//
// FileOpenPickerPage.xaml.cpp
// FileOpenPickerPage 类的实现
//

#include "pch.h"
#include "FileOpenPickerPage.xaml.h"

using namespace NETGEAR_Genie_Win8;

using namespace Platform;
using namespace Windows::ApplicationModel::Activation;
using namespace Windows::Foundation;
using namespace Windows::Foundation::Collections;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Controls::Primitives;
using namespace Windows::UI::Xaml::Data;
using namespace Windows::UI::Xaml::Input;
using namespace Windows::UI::Xaml::Media;
using namespace Windows::UI::Xaml::Navigation;
using namespace Windows::Storage::Pickers::Provider;

// “文件打开选取器合同”项模板在 http://go.microsoft.com/fwlink/?LinkId=234239 上提供

FileOpenPickerPage::FileOpenPickerPage()
{
	InitializeComponent();
}

/// <summary>
/// 在其他应用程序想要打开此应用程序中的文件时进行调用。
/// </summary>
/// <param name="args">用于与 Windows 协调进程的激活数据。</param>
void FileOpenPickerPage::Activate(FileOpenPickerActivatedEventArgs^ args)
{
	_fileOpenPickerUI = args->FileOpenPickerUI;
	_fileOpenPickerUI->FileRemoved += 
		ref new TypedEventHandler<FileOpenPickerUI^,FileRemovedEventArgs^> (this,&FileOpenPickerPage::FilePickerUI_FileRemoved);
		
	// TODO: 使用 DefaultViewModel->Insert("Files", <value>)，其中 <value> 是一个项集合，
	//     其中每项都应具有可绑定的 Image、Title 和 Description

	DefaultViewModel->Insert("CanGoUp", false);
	Window::Current->Content = this;
	Window::Current->Activate();
}

/// <summary>
/// 当用户从选取器框中移除某一项目时调用
/// </summary>
/// <param name="sender">用于包含可用文件的 FileOpenPickerUI 实例。</param>
/// <param name="args">描述已移除文件的事件数据。</param>
void FileOpenPickerPage::FilePickerUI_FileRemoved(FileOpenPickerUI^ sender, FileRemovedEventArgs^ args)
{
	// TODO: 响应在选取器 UI 中取消选择的项。
}

/// <summary>
/// 在选定的文件集合发生更改时进行调用。
/// </summary>
/// <param name="sender">用于显示可用文件的 GridView 实例。</param>
/// <param name="e">描述选择内容如何发生更改的事件数据。</param>
void FileOpenPickerPage::FileGridView_SelectionChanged(Object^ sender, SelectionChangedEventArgs^ e)
{
	(void) sender;	// 未使用的参数
	(void) e;	// 未使用的参数

	// TODO: 使用 _fileOpenPickerUI->AddFile 和 _fileOpenPickerUI->RemoveFile 更新 Windows UI
}

/// <summary>
/// 在单击“转到上级”按钮时进行调用，并指示用户希望在文件
/// 的层次结构中提升一个级别。
/// </summary>
/// <param name="sender">用于表示“Go up”命令的 Button 实例。</param>
///  <param name="args">描述按钮点击方式的事件数据。</param>
void FileOpenPickerPage::GoUpButton_Click(Object^ sender, RoutedEventArgs^ args)
{
	(void) sender;	// 未使用的参数
	(void) args;	// 未使用的参数

	// TODO: 使用 DefaultViewModel->Insert("CanGoUp", true) 启用相应的命令，
	//       使用 DefaultViewModel->Insert("Files", <value>) 反映文件层次结构遍历
}
