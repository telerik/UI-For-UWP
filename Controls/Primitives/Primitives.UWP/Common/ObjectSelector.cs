using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines a type that may be used to conditionally select a T instance.
    /// </summary>
    /// <typeparam name="T">The type of the object which is to be selected.</typeparam>
    public class ObjectSelector<T> where T : class
    {
        /// <summary>
        /// Selects a T instance depending on the provided item and container.
        /// </summary>
        /// <param name="item">The object instance for which a brush is to be selected.</param>
        /// <param name="container">The container where the object instance resides.</param>
        public T SelectObject(object item, DependencyObject container)
        {
            return this.SelectObjectCore(item, container);
        }

        /// <summary>
        /// Allows inheritors to provide custom implementation of the <see cref="M:SelectBrush"/> method.
        /// </summary>
        /// <param name="item">The object instance for which a brush is to be selected.</param>
        /// <param name="container">The container where the object instance resides.</param>
        protected virtual T SelectObjectCore(object item, DependencyObject container)
        {
            return null;
        }
    }
}
