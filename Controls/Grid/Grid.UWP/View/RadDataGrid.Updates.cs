using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    public partial class RadDataGrid
    {
        internal UpdateService updateService;

        UpdateServiceBase<UpdateFlags> IUpdateService<UpdateFlags>.UpdateService
        {
            get
            {
                return this.updateService;
            }
        }

        internal void InvalidateCellsMeasure()
        {
            if (this.cellsPanel != null)
            {
                this.model.pendingMeasureFlags |= InvalidateMeasureFlags.Cells;
                this.cellsPanel.InvalidateMeasure();
            }
        }

        internal void InvalidateCellsArrange()
        {
            if (this.cellsPanel != null)
            {
                this.cellsPanel.InvalidateArrange();
            }
        }

        internal void InvalidateHeadersArrange()
        {
            if (this.columnHeadersPanel != null)
            {
                this.columnHeadersPanel.InvalidateArrange();
            }
        }

        internal void InvalidateHeadersMeasure()
        {
            if (this.columnHeadersPanel != null)
            {
                this.model.pendingMeasureFlags |= InvalidateMeasureFlags.Header;
                this.columnHeadersPanel.InvalidateMeasure();
            }
        }

        private void InvalidatePanelsMeasure()
        {
            this.InvalidateHeadersMeasure();
            this.InvalidateCellsMeasure();
        }
    }
}