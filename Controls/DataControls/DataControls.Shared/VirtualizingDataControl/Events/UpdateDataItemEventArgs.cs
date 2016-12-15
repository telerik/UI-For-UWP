using System;
using Telerik.Core.Data;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Holds information about a container item and the data item it should be updated for.
    /// </summary>
    public class UpdateDataItemEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UpdateDataItemEventArgs" /> class.
        /// </summary>
        /// <param name="sourceItem">The source item.</param>
        /// <param name="targetContainerItem">The target container item.</param>
        public UpdateDataItemEventArgs(IDataSourceItem sourceItem, object targetContainerItem)
        {
            this.SourceItem = sourceItem;
            this.TargetContainerItem = targetContainerItem;
        }

        /// <summary>
        /// Gets an instance of the <see cref="IDataSourceItem"/> class that represents
        /// the data provider for the current container item.
        /// </summary>
        /// <value>The source item.</value>
        public IDataSourceItem SourceItem { get; private set; }

        /// <summary>
        /// Gets the target container item updated with the data from the source item.
        /// </summary>
        /// <value>The target container item.</value>
        public object TargetContainerItem { get; private set; }
    }
}
