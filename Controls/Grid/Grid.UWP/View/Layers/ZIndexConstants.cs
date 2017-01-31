using System;

namespace Telerik.UI.Xaml.Controls.Grid.View
{
    internal static class ZIndexConstants
    {
        // TODO: Rethink the Z-indexes more carefully
        public const int HorizontalGridLineBaseZIndex = 0;
        public const int SelectionControlBackgroundBaseZIndex = 800;
        public const int CurrencyBaseZIndex = 900;
        public const int VerticalGridLineBaseZIndex = 1000;
        public const int CellDecorationsBaseZIndex = VerticalGridLineBaseZIndex + 1000;
        public const int VisualStateControlBaseZIndex = CellDecorationsBaseZIndex + 1000;
        public const int SelectionControlBorderBaseZIndex = VisualStateControlBaseZIndex + 1000;

        public const int EditRowContentZIndex = SelectionControlBorderBaseZIndex + 1000;
    }
}
