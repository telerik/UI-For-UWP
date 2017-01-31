using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Encapsulates the information associated with the <see cref="RadDataGrid.DataBindingComplete"/> event.
    /// </summary>
    public class DataBindingCompleteEventArgs : EventArgs
    {
        internal DataBindingCompleteEventArgs()
        {
        }

        /// <summary>
        /// Gets the <see cref="IDataView"/> implementation that allows for traversing and/or manipulating the already computed data view.
        /// </summary>
        public IDataView DataView
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the flags that describe the change in the data.
        /// </summary>
        public DataChangeFlags ChangeFlags
        {
            get;
            internal set;
        }
    }
}
