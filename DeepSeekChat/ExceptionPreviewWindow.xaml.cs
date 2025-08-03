using DeepSeekChat.Foundation;
using DeepSeekChat.Helper;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ExceptionPreviewWindow : Window
    {
        public string ExceptionString { get; set; }
        public ExceptionPreviewWindow()
        {
            ExtendsContentIntoTitleBar = true;
            InitializeComponent();
        }

        private async void Grid_Loaded(object sender, RoutedEventArgs e)
        {
            ContentDialog exceptionDialog = new()
            {
                Title = "UnexpectedExceptionText".GetLocalized(),
                Content = new ScrollViewer()
                {
                    Content = new TextBlock()
                    {
                        Text = ExceptionString,
                        TextWrapping = TextWrapping.NoWrap
                    },
                    HorizontalScrollBarVisibility = ScrollBarVisibility.Visible,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Visible,
                    HorizontalScrollMode = ScrollMode.Enabled,
                    VerticalScrollMode = ScrollMode.Enabled
                },
                PrimaryButtonText = "Github Issues",
                CloseButtonText = "CloseText".GetLocalized(),
                DefaultButton = ContentDialogButton.Close,
                XamlRoot = Content.XamlRoot,
            };

            var result = await exceptionDialog.ShowAsync();

            if(result == ContentDialogResult.None)
                Close();

            DataPackage dp = new();
            dp.SetText(ExceptionString);

            Clipboard.SetContent(dp);

            await Launcher.LaunchUriAsync(new(ProjectProperties.IssuesPageUrl));
            Close();
        }
    }
}
