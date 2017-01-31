using Windows.UI.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Commands
{
    public class CellHoldingContext
    {
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
        /// Gets or sets the <see cref="DataGridCellInfo"/> instance over which a holding event has occured.
        /// </summary>
        public DataGridCellInfo CellInfo { get; set; }
    }
}
