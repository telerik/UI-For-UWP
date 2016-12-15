using System;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a type that can resolve a parameter to an <see cref="ImageSource"/> instance.
    /// </summary>
    public interface IImageSourceProvider
    {
        /// <summary>
        /// Retrieves the <see cref="ImageSource"/> instance from the specified parameter.
        /// </summary>
        ImageSource GetImageSource(object parameter);
    }
}
