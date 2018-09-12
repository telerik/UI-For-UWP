using System;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    internal abstract class CalendarViewModel : Element
    {
        internal double cellWidth;
        private ElementCollection<CalendarCellModel> calendarCells;
        private ElementCollection<CalendarGridLine> calendarDecorations;

        public ElementCollection<CalendarCellModel> CalendarCells
        {
            get
            {
                return this.calendarCells;
            }
        }

        public ElementCollection<CalendarGridLine> CalendarDecorations
        {
            get
            {
                return this.calendarDecorations;
            }
        }

        public CalendarModel Calendar
        {
            get
            {
                return this.GetCalendar();
            }
        }

        public abstract int RowCount
        {
            get;
        }

        public abstract int ColumnCount
        {
            get;
        }

        public virtual int SpecificColumnCount
        {
            get;
            internal set;
        }

        public virtual int BufferItemsCount
        {
            get;
            internal set;
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is CalendarCellModel || child is CalendarHeaderCellModel || child is CalendarGridLine || child is CalendarTimeRulerItem)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }
         
        internal override RadRect ArrangeOverride(RadRect rect)
        {
            this.layoutSlot = rect;

            RadRect clipRect = this.UpdateAnimatableContentClip(rect);

            this.ArrangeCalendarDecorations(clipRect);
            this.ArrangeCalendarContent(clipRect);
            this.ArrangeCalendarHeaders(clipRect);

            return rect;
        }

        internal abstract DateTime GetFirstDateToRender(DateTime date);
        internal abstract DateTime GetNextDateToRender(DateTime date);
        internal abstract void PrepareCalendarCell(CalendarCellModel cell, DateTime date);

        internal virtual void ArrangeCalendarDecorations(RadRect rect)
        {
            this.EnsureCalendarDecorations();

            double cellWidth = rect.Width / (this.ColumnCount + (this.BufferItemsCount * 2));
            if (this.SpecificColumnCount == 0)
            {
                this.SpecificColumnCount = this.RowCount;
            }

            double cellHeight = rect.Height / this.SpecificColumnCount;

            double gridLineThickness = this.Calendar.GridLinesThickness;
            int gridLineHalfThickness = (int)(gridLineThickness / 2);

            if ((this.Calendar.GridLinesVisibility & GridLinesVisibility.Horizontal) == GridLinesVisibility.Horizontal)
            {
                for (int rowIndex = 1; rowIndex < this.RowCount; rowIndex++)
                {
                    CalendarGridLine gridLine = this.GetDecorationByRowIndex(rowIndex);
                    gridLine.IsHorizontal = true;

                    // NOTE: We are not respecting rect.X / rect.Width values here for design purposes:
                    // If week numbers are visible the horizontal gridline should start rendering from the week number panel and not the calendar view.
                    gridLine.Arrange(new RadRect(this.layoutSlot.X, rect.Y + rowIndex * cellHeight - gridLineHalfThickness, this.layoutSlot.Width, gridLineThickness));
                }
            }

            if ((this.Calendar.GridLinesVisibility & GridLinesVisibility.Vertical) == GridLinesVisibility.Vertical)
            {
                for (int columnIndex = 1; columnIndex < this.ColumnCount + (this.BufferItemsCount * 2); columnIndex++)
                {
                    CalendarGridLine gridLine = this.GetDecorationByColumnIndex(columnIndex);
                    gridLine.IsHorizontal = false;

                    gridLine.Arrange(new RadRect(rect.X + columnIndex * cellWidth - gridLineHalfThickness, rect.Y, gridLineThickness, rect.Height));
                }
            }
        }

        internal virtual void EnsureCalendarDecorations()
        {
            if (this.calendarDecorations == null || this.calendarDecorations.Count == 0)
            {
                if (this.calendarDecorations == null)
                {
                    this.calendarDecorations = new ElementCollection<CalendarGridLine>(this);
                }

                // we are generating models only for the inner horizontal and vertical gridlnes as
                // first / last horizontal and vertical grid lines are drawn by RadCalendar border elements.
                int decorationCount = this.RowCount + this.ColumnCount - 2 + (this.BufferItemsCount * 2);

                for (int decorationIndex = 0; decorationIndex < decorationCount; decorationIndex++)
                {
                    CalendarGridLine gridLine = new CalendarGridLine();
                    this.calendarDecorations.Add(gridLine);
                }
            }
            else
            {
                foreach (CalendarGridLine gridLine in this.calendarDecorations)
                {
                    gridLine.layoutSlot = RadRect.Empty;
                }
            }
        }

        internal void ArrangeCalendarContent(RadRect rect)
        {
            this.EnsureCalendarCells();

            this.cellWidth = rect.Width / (this.ColumnCount + (this.BufferItemsCount * 2));

            if (this.SpecificColumnCount == 0)
            {
                this.SpecificColumnCount = this.RowCount;
            }

            double cellHeight = rect.Height / this.SpecificColumnCount;

            DateTime dateToRender = this.GetFirstDateToRender(this.Calendar.DisplayDate);

            int bufferItemsCount = this.BufferItemsCount;
            int itemIndex = 0;
            double previousRight = rect.X;
            double previousBottom = rect.Y;

            for (int rowIndex = 0; rowIndex < this.RowCount; rowIndex++)
            {
                previousRight = rect.X;

                for (int columnIndex = 0; columnIndex < this.ColumnCount + (bufferItemsCount * 2); columnIndex++)
                {
                    CalendarCellModel calendarCell = this.CalendarCells[itemIndex];

                    this.PrepareCalendarCell(calendarCell, dateToRender);

                    double horizontalDifference = columnIndex * this.cellWidth - previousRight + rect.X;
                    double verticalDifference = rowIndex * cellHeight - previousBottom + rect.Y;

                    calendarCell.Arrange(new RadRect(previousRight, previousBottom, this.cellWidth + horizontalDifference, cellHeight + verticalDifference));

                    previousRight = calendarCell.layoutSlot.Right;
                    if (columnIndex == (this.ColumnCount + (bufferItemsCount * 2)) - 1)
                    {
                        previousBottom = calendarCell.layoutSlot.Bottom;
                    }

                    this.SnapToGridLines(calendarCell, rowIndex, columnIndex);

                    dateToRender = this.GetNextDateToRender(dateToRender);

                    itemIndex++;
                }
            }
        }

        protected virtual void ArrangeCalendarHeaders(RadRect viewRect)
        {
        }

        protected virtual RadRect UpdateAnimatableContentClip(RadRect rect)
        {
            this.Calendar.AnimatableContentClip = rect;

            return rect;
        }

        protected void SnapToGridLines(CalendarNode calendarCell, int rowIndex, int columnIndex)
        {
            this.SnapToTopGridLine(calendarCell, rowIndex);
            this.SnapToBottomGridLine(calendarCell, rowIndex);

            this.SnapToLeftGridLine(calendarCell, columnIndex);
            this.SnapToRightGridLine(calendarCell, columnIndex);
        }

        private void EnsureCalendarCells()
        {
            if (this.calendarCells == null || this.calendarCells.Count == 0)
            {
                if (this.calendarCells == null)
                {
                    this.calendarCells = new ElementCollection<CalendarCellModel>(this);
                }

                // New way of indexing cell models to support the ITableProvider and IGridProvider automation providers.
                for (int i = 0; i < this.RowCount; i++)
                {
                    for (int j = 0; j < this.ColumnCount + (this.BufferItemsCount * 2); j++)
                    {
                        CalendarCellModel cell = new CalendarCellModel(i, j);

                        this.calendarCells.Add(cell);
                    }
                }
            }
            else
            {
                foreach (CalendarCellModel cell in this.calendarCells)
                {
                    cell.ClearValue(CalendarCellModel.IsBlackoutPropertyKey);
                    cell.ClearValue(CalendarCellModel.IsHighlightedPropertyKey);
                    cell.ClearValue(CalendarCellModel.IsSelectedPropertyKey);
                    cell.ClearValue(CalendarCellModel.IsFromAnotherViewPropertyKey);
                    cell.ClearValue(CalendarCellModel.IsCurrentPropertyKey);
                    cell.ClearValue(CalendarCellModel.IsPointerOverPropertyKey);
                }
            }
        }

        private void SnapToLeftGridLine(CalendarNode calendarCell, int columnIndex)
        {
            if (columnIndex <= 0 || columnIndex >= (this.ColumnCount + (this.BufferItemsCount * 2)))
            {
                return;
            }

            CalendarGridLine gridLine = this.GetDecorationByColumnIndex(columnIndex);
            if (!gridLine.layoutSlot.IsSizeValid())
            {
                return;
            }

            double difference = gridLine.layoutSlot.Right - calendarCell.layoutSlot.X;
            calendarCell.layoutSlot.X += difference;
            calendarCell.layoutSlot.Width -= difference;
        }

        private void SnapToRightGridLine(CalendarNode calendarCell, int columnIndex)
        {
            if (columnIndex >= this.ColumnCount + (this.BufferItemsCount * 2) - 1 || columnIndex < 0)
            {
                return;
            }

            CalendarGridLine gridLine = this.GetDecorationByColumnIndex(columnIndex + 1);
            if (!gridLine.layoutSlot.IsSizeValid())
            {
                return;
            }

            double difference = gridLine.layoutSlot.X - calendarCell.layoutSlot.Right;
            calendarCell.layoutSlot.Width += difference;
        }

        private void SnapToTopGridLine(CalendarNode calendarCell, int rowIndex)
        {
            if (rowIndex <= 0 || rowIndex >= this.RowCount)
            {
                return;
            }

            CalendarGridLine gridLine = this.GetDecorationByRowIndex(rowIndex);
            if (!gridLine.layoutSlot.IsSizeValid())
            {
                return;
            }

            double difference = gridLine.layoutSlot.Bottom - calendarCell.layoutSlot.Y;
            calendarCell.layoutSlot.Y += difference;
            calendarCell.layoutSlot.Height -= difference;
        }

        private void SnapToBottomGridLine(CalendarNode calendarCell, int rowIndex)
        {
            if (rowIndex >= this.RowCount - 1 || rowIndex < 0)
            {
                return;
            }

            CalendarGridLine gridLine = this.GetDecorationByRowIndex(rowIndex + 1);
            if (!gridLine.layoutSlot.IsSizeValid())
            {
                return;
            }

            double difference = gridLine.layoutSlot.Y - calendarCell.layoutSlot.Bottom;
            calendarCell.layoutSlot.Height += difference;
        }

        private CalendarGridLine GetDecorationByRowIndex(int rowIndex)
        {
            return this.CalendarDecorations[rowIndex - 1];
        }

        private CalendarGridLine GetDecorationByColumnIndex(int columnIndex)
        {
            return this.CalendarDecorations[this.RowCount + columnIndex - 2];
        }
    }
}
