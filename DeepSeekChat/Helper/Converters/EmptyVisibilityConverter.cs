using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper.Converters;

public partial class EmptyVisibilityConverter : IValueConverter
{
    private static Dictionary<Type, Func<object, bool>> _handler;

    public static void RegisterHandler(Type type, Func<object, bool> handler)
    {
        if (_handler == null)
        {
            _handler = new Dictionary<Type, Func<object, bool>>();
        }
        _handler[type] = handler;
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if(value == null)
        {
            return Microsoft.UI.Xaml.Visibility.Collapsed;
        }

        bool rev = parameter != null && bool.Parse((parameter as string)??"false");

        return _handler[value.GetType()](value) ^ rev ? Microsoft.UI.Xaml.Visibility.Visible : Microsoft.UI.Xaml.Visibility.Collapsed;
    }
    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
