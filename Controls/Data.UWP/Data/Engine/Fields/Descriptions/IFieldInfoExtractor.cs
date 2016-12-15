using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// A <see cref="IDataFieldInfo"/>s provider.
    /// </summary>
    internal interface IFieldInfoExtractor
    {
        /// <summary>
        /// Gets a value indicating whether the extractor is properly initialized.
        /// </summary>
        bool IsInitialized { get; }

        bool IsDynamic { get; }

        /// <summary>
        /// Gets the <see cref="IDataFieldInfo"/>s.
        /// </summary>
        /// <returns>The <see cref="IDataFieldInfo"/>s.</returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Design choice.")]
        IEnumerable<IDataFieldInfo> GetDescriptions();
    }
}