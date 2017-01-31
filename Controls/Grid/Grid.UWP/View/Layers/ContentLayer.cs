using System;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal abstract class ContentLayer : DataGridLayer
    {
        protected ContentLayer()
        {
        }

        internal abstract bool CanProcessColumnDefinition(DataGridColumn definition);

        internal abstract object GenerateCellContainer(object rowItem, DataGridColumn definition);

        internal abstract Size MeasureCell(GridCellModel cell, double availableWidth);

        internal abstract void RemoveCell(GridCellModel cell);

        internal abstract void ArrangeCell(GridCellModel cell);
        
        internal abstract void PrepareCell(GridCellModel cell);

        internal virtual void EndArrange()
        {
        }
    }
}
