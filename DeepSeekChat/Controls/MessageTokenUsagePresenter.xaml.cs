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
using DeepSeekChat.Models;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Controls;

public sealed partial class MessageTokenUsagePresenter : UserControl
{
    public TokenUsage? TokenUsageData
    {
        get { return (TokenUsage?)GetValue(TokenUsageProperty); }
        set { SetValue(TokenUsageProperty, value); }
    }

    // Using a DependencyProperty as the backing store for TokenUsage.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty TokenUsageProperty =
        DependencyProperty.Register("TokenUsage", typeof(TokenUsage), typeof(MessageMetadataPresenter), new PropertyMetadata(new TokenUsage() { TotalTokens = 0, CompletionTokens = 0, PromptTokens = 0}));



    public MessageTokenUsagePresenter()
    {
        InitializeComponent();
    }
}
