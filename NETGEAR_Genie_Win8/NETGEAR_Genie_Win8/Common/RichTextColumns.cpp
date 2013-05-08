#include "pch.h"
#include "RichTextColumns.h"

using namespace NETGEAR_Genie_Win8::Common;

using namespace Platform;
using namespace Platform::Collections;
using namespace Windows::Foundation;
using namespace Windows::UI::Xaml;
using namespace Windows::UI::Xaml::Controls;
using namespace Windows::UI::Xaml::Interop;

/// <summary>
/// 初始化 <see cref="RichTextColumns"/> 类的新实例。
/// </summary>
RichTextColumns::RichTextColumns()
{
	HorizontalAlignment = Windows::UI::Xaml::HorizontalAlignment::Left;
}

static DependencyProperty^ _columnTemplateProperty =
	DependencyProperty::Register("ColumnTemplate", TypeName(DataTemplate::typeid), TypeName(RichTextColumns::typeid),
	ref new PropertyMetadata(nullptr, ref new PropertyChangedCallback(
	&RichTextColumns::ResetOverflowLayout)));

/// <summary>
/// 标识 <see cref="ColumnTemplate"/> 依赖属性。
/// </summary>
DependencyProperty^ RichTextColumns::ColumnTemplateProperty::get()
{
	return _columnTemplateProperty;
}

static DependencyProperty^ _richTextContentProperty =
	DependencyProperty::Register("RichTextContent", TypeName(RichTextBlock::typeid), TypeName(RichTextColumns::typeid),
	ref new PropertyMetadata(nullptr, ref new PropertyChangedCallback(
	&RichTextColumns::ResetOverflowLayout)));

/// <summary>
/// 标识 <see cref="RichTextContent"/> 依赖属性。
/// </summary>
DependencyProperty^ RichTextColumns::RichTextContentProperty::get()
{
	return _richTextContentProperty;
}

/// <summary>
/// 当更改内容或溢出模板以重新创建列布局时调用。
/// </summary>
/// <param name="d">发生更改的 <see cref="RichTextColumns"/> 的
/// 实例。</param>
/// <param name="e">描述特定更改的事件数据。</param>
void RichTextColumns::ResetOverflowLayout(DependencyObject^ d, DependencyPropertyChangedEventArgs^ e)
{
	(void) e;	// 未使用的参数

	auto target = dynamic_cast<RichTextColumns^>(d);
	if (target != nullptr)
	{
		// 当发生重大更改时，从头开始重新生成布局
		target->_overflowColumns = nullptr;
		target->Children->Clear();
		target->InvalidateMeasure();
	}
}

/// <summary>
/// 确定是否需要附加溢出列以及是否可以移除
/// 现有列。
/// </summary>
/// <param name="availableSize">可用空间的大小，用于约束
/// 可以创建的附加列数。</param>
/// <returns>原始内容加上所有附加列的最终大小。</returns>
Size RichTextColumns::MeasureOverride(Size availableSize)
{
	if (RichTextContent == nullptr)
	{
		Size emptySize(0, 0);
		return emptySize;
	}

	// 通过附加列列表的缺失指示此操作尚未
	// 进行，确保 RichTextBlock 是
	// 子级
	if (_overflowColumns == nullptr)
	{
		Children->Append(RichTextContent);
		_overflowColumns = ref new Vector<RichTextBlockOverflow^>();
	}

	// 首先度量原始 RichTextBlock 内容
	RichTextContent->Measure(availableSize);
	auto maxWidth = RichTextContent->DesiredSize.Width;
	auto maxHeight = RichTextContent->DesiredSize.Height;
	auto hasOverflow = RichTextContent->HasOverflowContent;

	// 确保存在足够的溢出列
	unsigned int overflowIndex = 0;
	while (hasOverflow && maxWidth < availableSize.Width && ColumnTemplate != nullptr)
	{
		// 在耗尽前使用现有溢出列，然后从
		// 提供的模板创建更多列
		RichTextBlockOverflow^ overflow;
		if (_overflowColumns->Size > overflowIndex)
		{
			overflow = _overflowColumns->GetAt(overflowIndex);
		}
		else
		{
			overflow = safe_cast<RichTextBlockOverflow^>(ColumnTemplate->LoadContent());
			_overflowColumns->Append(overflow);
			Children->Append(overflow);
			if (overflowIndex == 0)
			{
				RichTextContent->OverflowContentTarget = overflow;
			}
			else
			{
				_overflowColumns->GetAt(overflowIndex - 1)->OverflowContentTarget = overflow;
			}
		}

		// 度量新列并准备根据需要进行重复
		Size remainingSize(availableSize.Width - maxWidth, availableSize.Height);
		overflow->Measure(remainingSize);
		maxWidth += overflow->DesiredSize.Width;
		maxHeight = __max(maxHeight, overflow->DesiredSize.Height);
		hasOverflow = overflow->HasOverflowContent;
		overflowIndex++;
	}

	// 断开附加列与溢出链的连接，从我们的专用列列表中移除它们，
	// 然后将它们作为子级移除
	if (_overflowColumns->Size > overflowIndex)
	{
		if (overflowIndex == 0)
		{
			RichTextContent->OverflowContentTarget = nullptr;
		}
		else
		{
			_overflowColumns->GetAt(overflowIndex - 1)->OverflowContentTarget = nullptr;
		}
		while (_overflowColumns->Size > overflowIndex)
		{
			_overflowColumns->RemoveAt(overflowIndex);
			Children->RemoveAt(overflowIndex + 1);
		}
	}

	// 报告最终确定大小
	Size resultingSize(maxWidth, maxHeight);
	return resultingSize;
}

/// <summary>
/// 排列原始内容和所有附加列。
/// </summary>
/// <param name="finalSize">定义必须在其中排列子级的区域的
/// 大小。</param>
/// <returns>子级实际需要的区域的大小。</returns>
Size RichTextColumns::ArrangeOverride(Size finalSize)
{
	float maxWidth = 0;
	float maxHeight = 0;
	auto childrenIterator = Children->First();
	while (childrenIterator->HasCurrent)
	{
		auto child = childrenIterator->Current;
		Rect childRect(maxWidth, 0, child->DesiredSize.Width, finalSize.Height);
		child->Arrange(childRect);
		maxWidth += child->DesiredSize.Width;
		maxHeight = __max(maxHeight, child->DesiredSize.Height);
		childrenIterator->MoveNext();
	}
	Size resultingSize(maxWidth, maxHeight);
	return resultingSize;
}
