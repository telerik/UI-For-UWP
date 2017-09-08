using Windows.UI.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    /// <summary>
    /// Represents an ActionContext class.
    /// </summary>
    public class CellHoldingContext
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CellHoldingContext"/> class.
        /// </summary>
        public CellHoldingContext(DataGridCellInfo cellInfo, HoldingState holdingState)
        {
            this.CellInfo = cellInfo;
            this.HoldingState = holdingState;
        }

        /// <summary>
        /// Gets or sets the <see cref="HoldingState"/> of the event.
        /// </summary>
        public HoldingState HoldingState { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="DataGridCellInfo"/> instance over which a holding event has occurred.
        /// </summary>
        public DataGridCellInfo CellInfo { get; set; }
    }
}
