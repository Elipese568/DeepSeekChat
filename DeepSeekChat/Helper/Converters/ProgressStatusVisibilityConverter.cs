using DeepSeekChat.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Helper.Converters;

public class ProgressStatusVisibilityConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        return (ProgressStatus)value == ProgressStatus.InProgress ^ bool.Parse((string)parameter ?? "false") ? Visibility.Visible : Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
