using DeepSeekChat.Helper;
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

namespace DeepSeekChat.Controls;

public sealed partial class PropertyPresenter : Control
{
    ContentPresenter _headerPresenter;
    public PropertyPresenter()
    {
        this.DefaultStyleKey = typeof(PropertyPresenter);
    }

    public static readonly DependencyProperty HeaderProperty =
        DependencyProperty.Register(nameof(Header), typeof(object), typeof(PropertyPresenter), new PropertyMetadata(default(object)));

    public object Header
    {
        get => (object)GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register(nameof(Value), typeof(object), typeof(PropertyPresenter), new PropertyMetadata(default(object)));

    public object Value
    {
        get => GetValue(ValueProperty);
        set => SetValue(ValueProperty, value);
    }

    protected override void OnApplyTemplate()
    {
        _headerPresenter = GetTemplateChild("PART_Header") as ContentPresenter;

        if(Header is string headerStr)
        {
            var textContent = "PropertyPresenterTextHeaderTemplate".GetResource<DataTemplate>(rd: Application.Current.Resources).LoadContent() as FrameworkElement;
            if(textContent != null)
            {
                textContent.DataContext = Header;
                _headerPresenter.Content = textContent;
            }
        }
        else
        {
            _headerPresenter.Content = Header;
        }

        base.OnApplyTemplate();
    }
}
