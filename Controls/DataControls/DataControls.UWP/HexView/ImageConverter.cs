using System;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media.Imaging;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    internal class ImageConverter : IValueConverter
    {
        private RadHexView owner;

        public ImageConverter(RadHexView owner)
        {
            this.owner = owner;
        }

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is string)
            {
                return new BitmapImage(new Uri(value.ToString())) { DecodePixelHeight = (int)Math.Ceiling(this.owner.LayoutDefinition.ItemLength) };
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
