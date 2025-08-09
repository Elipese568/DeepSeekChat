using DeepSeekChat.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using System;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace DeepSeekChat.Helper.Converters;

public class FileTypeToGlyphConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is FileType fileType)
        {
            return fileType switch
            {
                FileType.Document => "\uE8A5", // Document icon
                FileType.Media => "\uE91B",    // Media icon
                _ => "\uE8A5"                  // Default to Document icon
            };
        }
        return DependencyProperty.UnsetValue;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
