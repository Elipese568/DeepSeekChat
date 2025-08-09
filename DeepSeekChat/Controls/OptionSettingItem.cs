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
    public sealed partial class OptionSettingItem : ContentControl
    {
        public OptionSettingItem()
        {
            DefaultStyleKey = typeof(OptionSettingItem);
        }


        public string Header
        {
            get { return (string)GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Header.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register("Header", typeof(string), typeof(OptionSettingItem), new PropertyMetadata(string.Empty));


        public UIElement SubContent
        {
            get { return (UIElement)GetValue(SubContentProperty); }
            set { SetValue(SubContentProperty, value); }
        }

        // Using a DependencyProperty as the backing store for SubContent.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SubContentProperty =
            DependencyProperty.Register("SubContent", typeof(UIElement), typeof(OptionSettingItem), new PropertyMetadata(null));
    }
}
