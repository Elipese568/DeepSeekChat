using Microsoft.UI.Xaml.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FileSize = ulong;
namespace DeepSeekChat.Helper.Converters;

public partial class FileSizeFriendlyStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        FileSize size = (FileSize)value;
        const FileSize B   = 1;
        const FileSize KiB = B * 1024;
        const FileSize MiB = KiB * 1024;
        const FileSize GiB = MiB * 1024;

        return size switch
        {
            < KiB => $"{size} B",
            >= KiB and < MiB => $"{(double)size / KiB:N2} KiB",
            >= MiB and < GiB => $"{(double)size / MiB:N2} MiB",
            >= GiB => $"{(double)size / GiB:N2} GiB"
        };
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        throw new NotImplementedException();
    }
}
