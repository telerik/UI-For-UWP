using System.ComponentModel;
using Telerik.Geospatial;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Defines the contract for data sources that provide <see cref="IMapShape"/> instances.
    /// </summary>
    public interface IShapeDataSource : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the <see cref="MapShapeModelCollection"/> provided by this source.
        /// </summary>
        MapShapeModelCollection Shapes
        {
            get;
        }
    }
}
