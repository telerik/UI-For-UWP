using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Holds information used in ValidateCell command.
    /// </summary>
    public class ValidateCellContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ValidateCellContext" /> class.
        /// </summary>
        /// <param name="errors">The errors.</param>
        /// <param name="cellInfo">The cell info.</param>
        public ValidateCellContext(IList<object> errors, DataGridCellInfo cellInfo)
        {
            this.Errors = errors;
            this.CellInfo = cellInfo;
        }

        /// <summary>
        /// Gets the errors (if any) that occurred during the validation.
        /// </summary>
        public IList<object> Errors { get; private set; }

        /// <summary>
        /// Gets the <see cref="CellInfo"/> object that represents the current grid cell.
        /// </summary>
        public DataGridCellInfo CellInfo { get; private set; }
    }
}
