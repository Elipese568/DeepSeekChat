using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper.Converters;

public partial class DictionaryItem : DependencyObject
{
    public object Key
    {
        get { return GetValue(KeyProperty); }
        set { SetValue(KeyProperty, value); }
    }
    public static readonly DependencyProperty KeyProperty =
        DependencyProperty.Register("Key", typeof(object), typeof(DictionaryItem), new PropertyMetadata(null));
    public object Value
    {
        get { return (object)GetValue(ValueProperty); }
        set { SetValue(ValueProperty, value); }
    }
    public static readonly DependencyProperty ValueProperty =
        DependencyProperty.Register("Value", typeof(object), typeof(DictionaryItem), new PropertyMetadata(null));
}

public partial class DictionaryItemCollection : DependencyObject
{
    public ObservableCollection<DictionaryItem> Items
    {
        get { return (ObservableCollection<DictionaryItem>)GetValue(ItemsProperty); }
        set { SetValue(ItemsProperty, value); }
    }
    public static readonly DependencyProperty ItemsProperty =
        DependencyProperty.Register("Items", typeof(ObservableCollection<DictionaryItem>), typeof(DictionaryItemCollection), new PropertyMetadata(new ObservableCollection<DictionaryItem>()));

    public string UD
    {
        get { return (string)GetValue(UDProperty); }
        set { SetValue(UDProperty, value); }
    }

    // Using a DependencyProperty as the backing store for UD.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty UDProperty =
        DependencyProperty.Register("UD", typeof(string), typeof(DictionaryItemCollection), new PropertyMetadata(Random.Shared.Next().ToString()));


}

public class DictionaryConeverter : IValueConverter
{
    private static Dictionary<(Type, Type), Func<object, object, bool>> _handler;

    public static void RegisterHandler((Type, Type) type, Func<object, object, bool> handler)
    {
        if (_handler == null)
        {
            _handler = new();
        }
        _handler[type] = handler;
    }

    public object Convert(object value, Type targetType, object parameter, string language)
    {
        DictionaryItemCollection items = parameter as DictionaryItemCollection;

        foreach (DictionaryItem item in items.Items)
        {
            if(item.Value.GetType() != targetType)
            {
                continue;
            }
            if (_handler[(value.GetType(), item.Key.GetType())](value, item.Key))
            {
                return item.Value;
            }
        }
        
        if(items.Items.FirstOrDefault(x => _handler[(value.GetType(), x.Key.GetType())](value, x.Key), null) is DictionaryItem defaultItem)
        {
            return defaultItem.Value;
        }

        return DependencyProperty.UnsetValue;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
