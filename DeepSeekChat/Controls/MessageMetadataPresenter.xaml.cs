using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using DeepSeekChat.Command;
using CommunityToolkit.Mvvm.ComponentModel;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Controls;

[ObservableObject]
public sealed partial class MessageMetadataPresenter : UserControl
{
    public ChatCompletionMetadata Metadata
    {
        get { return (ChatCompletionMetadata)GetValue(MetadataProperty); }
        set { SetValue(MetadataProperty, value); }
    }

    // Using a DependencyProperty as the backing store for Metadata.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty MetadataProperty =
        DependencyProperty.Register("Metadata", typeof(ChatCompletionMetadata), typeof(MessageMetadataPresenter), new PropertyMetadata(null));

    public MessageMetadataPresenter()
    {
        RegisterPropertyChangedCallback(MetadataProperty, (s, e) =>
        {
            OnPropertyChanged(nameof(Metadata));
        });
        InitializeComponent();
    }
}
