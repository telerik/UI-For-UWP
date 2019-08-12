using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// This class is a logical representation of each cell within the month view of the calendar control.
    /// </summary>
    public class CalendarMonthCellModel : CalendarCellModel
    {
        internal static readonly int IsSpecialPropertyKey = PropertyKeys.Register(typeof(bool), "IsSpecial");
        internal static readonly int IsSpecialReadOnlyPropertyKey = PropertyKeys.Register(typeof(bool), "IsSpecialReadOnly");

        internal List<Slot> slots;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarMonthCellModel"/> class.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="column">The column index of the cell.</param>
        public CalendarMonthCellModel(int row, int column) 
            : base(row, column)
        {
        }

        /// <summary>
        /// Gets a value indicating whether the month cell is special.
        /// </summary>
        public bool IsSpecial
        {
            get
            {
                return this.GetTypedValue<bool>(IsSpecialPropertyKey, false);
            }
            internal set
            {
                this.SetValue(IsSpecialPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the month cell is special and is read-only.
        /// </summary>
        public bool IsSpecialReadOnly
        {
            get
            {
                return this.GetTypedValue<bool>(IsSpecialReadOnlyPropertyKey, false);
            }
            internal set
            {
                this.SetValue(IsSpecialReadOnlyPropertyKey, value);
            }
        }

        internal new bool IsNormal
        {
            get
            {
                return !this.IsFromAnotherView && !this.IsBlackout && !this.IsSelected && !this.IsHighlighted && !this.IsPointerOver && !this.IsCurrent && !this.IsSpecial && !this.IsSpecialReadOnly;
            }
        }

        internal override void OnPropertyChanging(RadPropertyEventArgs e)
        {
            base.OnPropertyChanging(e);

            if (e.Key == IsSpecialReadOnlyPropertyKey && e.NewValue != null)
            {
                var isReadOnly = (bool)e.NewValue;
                if (isReadOnly && this.IsSelected)
                {
                    this.IsSelected = false;
                }
            }
            else if (e.Key == CalendarCellModel.IsSelectedPropertyKey && e.NewValue != null)
            {
                bool isSelected = (bool)e.NewValue;
                if (isSelected && this.IsSpecialReadOnly)
                {
                    e.Cancel = true;
                }
            }
        }
    }
}
