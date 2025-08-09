using CommunityToolkit.Mvvm.ComponentModel;
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

    public MessagesView()
    {
        RegisterPropertyChangedCallback(MessagesSourceProperty, (s, e) =>
        {
            OnPropertyChanged(nameof(MessagesSource));
        });
        InitializeComponent();
    }

    public event RoutedEventHandler StopGeneratingEvent;
    public event RoutedEventHandler RemoveMessageEvent;

    private void StopGeneratingButton_Click(object sender, RoutedEventArgs e)
    {
        StopGeneratingEvent(sender,e);
    }

    private void RemoveMessageButton_Click(object sender, RoutedEventArgs e)
    {
        RemoveMessageEvent(sender, e);
    }
}
