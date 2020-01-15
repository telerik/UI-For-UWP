using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal static class EditorsHelper
    {
        internal static void AddPropertyValueConverter(Binding propertyValueBinding, FrameworkElement editor)
        {
            propertyValueBinding.Converter = new EntityPropertyValueConverter();
            propertyValueBinding.ConverterParameter = editor;
        }
    }
}