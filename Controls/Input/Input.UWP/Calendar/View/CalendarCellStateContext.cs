using System;
using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents a context passed to a <see cref="CalendarCellStateSelector"/> instance.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes", Justification = "Override these methods as needed.")]
    public struct CalendarCellStateContext
    {
        private CalendarCellModel cell;

        internal CalendarCellStateContext(CalendarCellModel cell)
        {
            this.cell = cell;
        }

        /// <summary>
        /// Gets the date of the respective calendar cell.
        /// </summary>
        public DateTime Date
        {
            get
            {
                return this.cell.Date;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the associated calendar cell is selectable (i.e. enabled).
        /// </summary>
        public bool IsBlackout
        {
            get
            {
                return this.cell.IsBlackout;
            }
            set
            {
                this.cell.IsBlackout = value;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the associated month cell is special.
        /// </summary>
        public bool IsSpecial
        {
            get
            {
                var monthCell = this.cell as CalendarMonthCellModel;
                if (monthCell != null)
                {
                    return monthCell.IsSpecial;
                }

                return false;
            }
            set
            {
                var monthCell = this.cell as CalendarMonthCellModel;
                if (monthCell != null)
                {
                    monthCell.IsSpecial = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the associated month cell is special and is read-only.
        /// </summary>
        public bool IsSpecialReadOnly
        {
            get
            {
                var monthCell = this.cell as CalendarMonthCellModel;
                if (monthCell != null)
                {
                    return monthCell.IsSpecialReadOnly;
                }

                return false;
            }
            set
            {
                var monthCell = this.cell as CalendarMonthCellModel;
                if (monthCell != null)
                {
                    monthCell.IsSpecialReadOnly = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the associated calendar cell is highlighted.
        /// </summary>
        public bool IsHighlighted
        {
            get
            {
                return this.cell.IsHighlighted;
            }
            set
            {
                this.cell.IsHighlighted = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell is selected.
        /// </summary>
        //// TODO: Make this read-write?
        public bool IsSelected
        {
            get
            {
                return this.cell.IsSelected;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell belongs to another view.
        /// </summary>
        public bool IsFromAnotherView
        {
            get
            {
                return this.cell.IsFromAnotherView;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the pointer is over the associated calendar cell.
        /// </summary>
        public bool IsPointerOver
        {
            get
            {
                return this.cell.IsPointerOver;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell is currently focused.
        /// </summary>
        public bool IsCurrent
        {
            get
            {
                return this.cell.IsCurrent;
            }
        }

        /// <summary>
        /// Gets a value of the special slots that are associated with the cell. Slots are only available for month view.
        /// </summary>
        public IEnumerable<Slot> SpecialSlots
        {
            get
            {
                var monthViewCellModel = this.cell as CalendarMonthCellModel;
                if (monthViewCellModel != null)
                {
                    return monthViewCellModel.slots;
                }

                return null;
            }
        }
    }
}
