using Microsoft.UI.Xaml.Markup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeepSeekChat.Helper;

[MarkupExtensionReturnType(ReturnType = typeof(int))]
public sealed class Int32MarkupExtension : MarkupExtension
{
    public Int32MarkupExtension() { }
    public int Value { get; set; }
    protected override object ProvideValue() => Value;
}
