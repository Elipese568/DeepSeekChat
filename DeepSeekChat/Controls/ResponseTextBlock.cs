using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.WinUI.UI.Controls;
using DeepSeekChat.Helper;
using DeepSeekChat.Service;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Documents;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Controls
{
    [ObservableObject]
    public sealed partial class ResponseTextBlock : Control
    {
        private ContentPresenter _content;
        private FrameworkElement _textControl;
        private DependencyProperty _textProperty;

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Text.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register("Text", typeof(string), typeof(ResponseTextBlock), new PropertyMetadata(string.Empty));

        public ResponseTextBlock()
        {
            RegisterPropertyChangedCallback(TextProperty, (s, e) =>
            {
                OnPropertyChanged(nameof(Text));
                _textControl?.SetValue(_textProperty, Text);
            });
            DefaultStyleKey = typeof(ResponseTextBlock);
        }

        protected override void OnApplyTemplate()
        {
            _content = GetTemplateChild("PART_Content") as ContentPresenter;
            bool useMarkdown = bool.Parse(App.Current.GetService<SettingService>().Read(SettingService.SETTING_USE_MARKDOWN_RENDER, "true"));
            
            if(useMarkdown)
            {
                MarkdownTextBlock mtb = new();
                mtb.Style = "ResponseMarkdownTextBlockStyle".GetResource<Style>(rd: Application.Current.Resources);
                mtb.Text = Text;
                _textControl = mtb;
                _textProperty = MarkdownTextBlock.TextProperty;

                _content.Content = mtb;
            }
            else
            {
                TextBlock tb = new()
                {
                    IsTextSelectionEnabled = true,
                    TextWrapping = TextWrapping.Wrap,
                    Text = Text
                };

                _textControl = tb;
                _textProperty = TextBlock.TextProperty;

                _content.Content = tb;
            }
            base.OnApplyTemplate();
        }
    }
}
