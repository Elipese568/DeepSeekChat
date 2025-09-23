using CommunityToolkit.Mvvm.ComponentModel;
using DeepSeekChat.Helper;
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
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Controls;

public class ScrollModePanelConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var type = (MessageViewMode)value;

        if(type == MessageViewMode.Previewing)
        {
            return "MessagesNormalItemsPanelTemplate".GetResource<ItemsPanelTemplate>(rd: Application.Current.Resources);
        }

        return "MessagesLastItemInViewItemsPanelTemplate".GetResource<ItemsPanelTemplate>(rd: Application.Current.Resources);
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public class ModeOperationVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        var type = (MessageViewMode)value;
        return (type == MessageViewMode.Messaging) ^ (bool.TryParse(parameter as string, out var result) && result) ? Visibility.Visible : Visibility.Collapsed;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}

public enum MessageViewMode
{
    Messaging,
    Previewing
}

[ObservableObject]
public sealed partial class MessagesView : UserControl
{
    public MessagesViewModel MessagesSource
    {
        get { return (MessagesViewModel)GetValue(MessagesSourceProperty); }
        set { SetValue(MessagesSourceProperty, value); }
    }

    // Using a DependencyProperty as the backing store for MessagesSource.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MessagesSourceProperty =
        DependencyProperty.Register("MessagesSource", typeof(MessagesViewModel), typeof(MessagesView), new PropertyMetadata(null));



    public MessageViewMode Mode
    {
        get { return (MessageViewMode)GetValue(ModeProperty); }
        set { SetValue(ModeProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Mode.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty ModeProperty =
        DependencyProperty.Register(nameof(Mode), typeof(MessageViewMode), typeof(MessagesView), new PropertyMetadata(MessageViewMode.Messaging));



    public MessagesView()
    {
        RegisterPropertyChangedCallback(MessagesSourceProperty, (s, e) =>
        {
            OnPropertyChanged(nameof(MessagesSource));
        });
        InitializeComponent();
    }

    public event RoutedEventHandler StopGenerating;
    public event RoutedEventHandler RemoveMessage;
    public event RoutedEventHandler ReferMessage;
    public event RoutedEventHandler GotoMessage;

    public void ScrollIntoView(ApplicationChatMessageViewModel msg)
    {
        InvertedListView.ScrollIntoView(msg);
    }

    private void StopGeneratingButton_Click(object sender, RoutedEventArgs e)
    {
        StopGenerating(sender,e);
    }

    private void RemoveMessageButton_Click(object sender, RoutedEventArgs e)
    {
        RemoveMessage(sender, e);
    }

    private void ReferMessageButton_Click(object sender, RoutedEventArgs e)
    {
        ReferMessage(sender, e);
    }

    private void GotoMessageButton_Click(object sender, RoutedEventArgs e)
    {
        GotoMessage(sender, e);
    }
}
