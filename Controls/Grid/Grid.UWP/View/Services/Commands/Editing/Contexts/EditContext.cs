using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Holds information associated with a row edit operation within a <see cref="RadDataGrid"/> instance.
    /// </summary>
    public class EditContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditContext" /> class.
        /// </summary>
        /// <param name="cell">The cell.</param>
        /// <param name="action">The action.</param>
        /// <param name="parameter">The parameter.</param>
        public EditContext(DataGridCellInfo cell, ActionTrigger action, object parameter)
        {
            this.CellInfo = cell;
            this.TriggerAction = action;
            this.Parameter = parameter;
        }

        /// <summary>
        /// Gets the cell info associated with the operation.
        /// </summary>
        public DataGridCellInfo CellInfo { get; private set; }

        /// <summary>
        /// Gets the <see cref="ActionTrigger"/> value that triggered the operation.
        /// </summary>
        /// <value>The trigger action.</value>
        public ActionTrigger TriggerAction { get; private set; }

        /// <summary>
        /// Gets an optional parameter holding additional information associated with the operation.
        /// </summary>
        /// <value>The parameter.</value>
        public object Parameter { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the core action associated with the context is successful.
        /// </summary>
        internal bool IsSuccessful
        {
            get;
            set;
        }
    }
}
