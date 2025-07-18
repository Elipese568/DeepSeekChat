using CommunityToolkit.WinUI.UI.Controls;
using DeepSeekChat.Models;
using DeepSeekChat.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Views
{
    public class ProgressStatusVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            return ((ProgressStatus)value == ProgressStatus.InProgress) ^ bool.Parse((string)parameter??"false") ? Visibility.Visible : Visibility.Collapsed;
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ProgressStatusContentConverter : IValueConverter
    {
        // parameter: a        b   (binary format)
        //        reasoning reverse
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var divm = ((ProgressStatus)value);
            var self = ((ReferenceValue)parameter).Value as ApplicationChatMessageViewModel;
            var parameterint = int.Parse(language, System.Globalization.NumberStyles.BinaryNumber);
            return
                (divm == ProgressStatus.InProgress) ^ (parameterint is 0b01 or 0b11) ?
                    parameterint is 0b10 or 0b11 ?
                        self.AiChatCompletion.ReasoningContent // a is 1 meaning is getting reasoning
                        :
                        self.AiChatCompletion.Content          // a is 0 meaning is getting content
                    :
                    DependencyProperty.UnsetValue;                                                     // content isn't completed yet or is would hidden

        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class ReferenceValue : DependencyObject
    {
        public object Value
        {
            get { return (object)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Value.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(object), typeof(ReferenceValue), new PropertyMetadata(0));
    }

    public sealed partial class DiscussionPage : Page
    {
        public DiscussionViewModel ViewModel { get; set; }

        public DiscussionPage(DiscussionItemViewModel item)
        {
            ViewModel = new DiscussionViewModel(item); // 传递item到ViewModel
            this.InitializeComponent();
            
            // 确保消息更新时自动滚动到底部
            ViewModel.ScrollToBottomRequested += (s, e) =>
            {
                InvertedListView.ScrollIntoView(ViewModel.SelectedDiscussItemViewModel.MessagesViewModel.MessageViewModels.LastOrDefault());
            };
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OptionPane.IsPaneOpen = !OptionPane.IsPaneOpen;
        }

        private void StopGeneratingButton_Click(object sender, RoutedEventArgs e)
        {
            ViewModel.StopGenerating();
        }

        private void MarkdownTextBlock_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if((bool)e.NewValue == false)
            {
                ((MarkdownTextBlock)sender).Opacity = 1.0;
            }
        }
    }
}
