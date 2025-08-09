using DeepSeekChat.Models;
using DeepSeekChat.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Imaging;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Controls;

public sealed partial class FilePreviewPresenter : UserControl
{
    public event EventHandler<EventArgs> FilePreviewClosing;

    public FileViewModel PreviewFileViewModel
    {
        get { return (FileViewModel)GetValue(PreviewFileViewModelProperty); }
        set { SetValue(PreviewFileViewModelProperty, value); }
    }

    public static readonly DependencyProperty PreviewFileViewModelProperty =
        DependencyProperty.Register("PreviewFileViewModel", typeof(FileViewModel), typeof(FilePreviewPresenter), new PropertyMetadata(null));

    public string GetRawDataString(Uri fileUri)
    {
        return File.ReadAllText(fileUri.LocalPath);
    }

    public ImageSource GetRawImageSource(Uri fileUri)
    {
        return new BitmapImage(fileUri);
    }

    public string AsString(FileType type)
        => type switch { FileType.Document => "Document", FileType.Media => "Media", _ => "Unknown" };

    public FilePreviewPresenter()
    {
        InitializeComponent();
    }

    private void ExitPreviewFile_Click(object sender, RoutedEventArgs e)
    {
        FilePreviewClosing?.Invoke(this, new EventArgs());
    }
}
