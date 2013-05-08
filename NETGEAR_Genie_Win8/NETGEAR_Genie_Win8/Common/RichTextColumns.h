#pragma once

#include <collection.h>

namespace NETGEAR_Genie_Win8
{
	namespace Common
	{
		/// <summary>
		/// 按适合可用内容所需数量创建附加溢出列的
		/// <see cref="RichTextBlock"/> 的包装。
		/// </summary>
		/// <example>
		/// 下面创建间距 50 像素、宽 400 像素的列的集合
		/// 以包含任意数据绑定内容:
		/// <code>
		/// <RichTextColumns>
		///     <RichTextColumns.ColumnTemplate>
		///         <DataTemplate>
		///             <RichTextBlockOverflow Width="400" Margin="50,0,0,0"/>
		///         </DataTemplate>
		///     </RichTextColumns.ColumnTemplate>
		///     
		///     <RichTextBlock Width="400">
		///         <Paragraph>
		///             <Run Text="{Binding Content}"/>
		///         </Paragraph>
		///     </RichTextBlock>
		/// </RichTextColumns>
		/// </code>
		/// </example>
		/// <remarks>通常用于水平滚动区域，其中未限定的
		/// 空间量允许创建所需全部列。当用于垂直滚动
		/// 空间时，不会存在任何附加列。</remarks>
		[Windows::UI::Xaml::Markup::ContentProperty(Name = "RichTextContent")]
		[Windows::Foundation::Metadata::WebHostHidden]
		public ref class RichTextColumns sealed : Windows::UI::Xaml::Controls::Panel
		{
		public:
			RichTextColumns();
			static property Windows::UI::Xaml::DependencyProperty^ RichTextContentProperty
			{
				Windows::UI::Xaml::DependencyProperty^ get();
			};
			static property Windows::UI::Xaml::DependencyProperty^ ColumnTemplateProperty
			{
				Windows::UI::Xaml::DependencyProperty^ get();
			}
			property Windows::UI::Xaml::DataTemplate^ ColumnTemplate
			{
				Windows::UI::Xaml::DataTemplate^ get() { return safe_cast<Windows::UI::Xaml::DataTemplate^>(GetValue(ColumnTemplateProperty)); }
				void set(Windows::UI::Xaml::DataTemplate^ value) { SetValue(ColumnTemplateProperty, value); }
			};
			property Windows::UI::Xaml::Controls::RichTextBlock^ RichTextContent
			{
				Windows::UI::Xaml::Controls::RichTextBlock^ get() { return safe_cast<Windows::UI::Xaml::Controls::RichTextBlock^>(GetValue(RichTextContentProperty)); }
				void set(Windows::UI::Xaml::Controls::RichTextBlock^ value) { SetValue(RichTextContentProperty, value); }
			};

		protected:
			virtual Windows::Foundation::Size MeasureOverride(Windows::Foundation::Size availableSize) override;
			virtual Windows::Foundation::Size ArrangeOverride(Windows::Foundation::Size finalSize) override;

		internal:
			static void ResetOverflowLayout(Windows::UI::Xaml::DependencyObject^ d, Windows::UI::Xaml::DependencyPropertyChangedEventArgs^ e);

		private:
			Platform::Collections::Vector<Windows::UI::Xaml::Controls::RichTextBlockOverflow^>^ _overflowColumns;
		};
	}
}
