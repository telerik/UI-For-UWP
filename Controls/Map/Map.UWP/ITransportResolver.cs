using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Defines a type that is used by the <see cref="ShapefileDataSource"/> to load a <see cref="StorageFile"/> containing the Map Shapes definition.
    /// </summary>
    public interface ITransportResolver
    {
        /// <summary>
        /// Loads the <see cref="StorageFile"/> containing the Map Shapes definition.
        /// </summary>
        /// <param name="uri">The <see cref="Uri"/> pointing to the file location.</param>
        Task<StorageFile> GetStorageFile(Uri uri);
    }
}
