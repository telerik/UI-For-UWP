using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Windows.Storage;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents a concrete <see cref="ITransportResolver"/> implementation that loads files, specified as "Local" to the application.
    /// </summary>
    public class LocalTransportResolver : ITransportResolver
    {
        private const string NotSupportedStringFormat = @"Uri scheme {0} is not supported. 
Implement the ITransportResolver interface and assign it to the ShapefileDataSource.TransportResolver property to handle Uri schemes other than ms-appx and ms-appdata.";

        /// <summary>
        /// Loads the <see cref="StorageFile" /> containing the Map Shapes definition.
        /// </summary>
        /// <param name="uri">The <see cref="Uri" /> pointing to the file location.</param>
        /// <exception cref="System.NotSupportedException">The NotSupportedException in case the Uri does not start with 'ms-appx' or 'ms-appdata'.</exception>
        [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1650:ElementDocumentationMustBeSpelledCorrectly", Justification = "Reviewed.")]
        public async Task<StorageFile> GetStorageFile(Uri uri)
        {
            switch (uri.Scheme)
            {
                case "ms-appx":
                case "ms-appdata":
                    return await StorageFile.GetFileFromApplicationUriAsync(uri);
                default:
                    throw new NotSupportedException(string.Format(NotSupportedStringFormat, uri.Scheme));
            }
        }
    }
}
