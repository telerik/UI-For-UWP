using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface ITable
    {
        double PhysicalVerticalOffset { get; }
        double PhysicalHorizontalOffset { get; }
        bool RowHeightIsNaN { get; }
        double RowHeight { get; }

        int FrozenColumnCount { get; }

        void RecycleColumn(ItemInfo itemInfo);
        void RecycleRow(ItemInfo itemInfo);
        void RecycleEditRow(ItemInfo itemInfo);
        double GetWidthForLine(int line);
        double GetHeightForLine(int line);
        void SetHeightForLine(int line, double value);

        double GenerateCellsForColumn(int columnSlot, double largetsColumnElementWidth, IItemInfoNode columnDecorator);
        double GenerateCellsForRow(int rowSlot, double largestRowElementHeight, IItemInfoNode rowDecorator);

        object GetCellValue(ItemInfo rowItemInfo, ItemInfo columnItemInfo);
        void InvalidateHeadersPanelMeasure();
        void InvalidateCellsPanelMeasure();
        void InvalidateCellsPanelArrange();

        void Arrange(Node node);
        RadSize Measure(Node node);

        bool HasExpandedRowDetails(int slot);
    }
}