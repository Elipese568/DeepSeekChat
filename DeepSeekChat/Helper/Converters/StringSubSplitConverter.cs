using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper.Converters;

public partial class StringSubSplitConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        int len = (int)parameter;
        string str = (value as string);
        return len > str.Length? str : str.Substring(0, len);
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
