using System;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// This class is a logical representation of each cell within the calendar control.
    /// </summary>
    public class CalendarCellModel : CalendarNode
    {
        internal const int DefaultHighlightedDecorationBaseZIndex = 200;
        internal const int DefaultCurrentDecorationBaseZIndex = 400;
        internal const int DefaultPointerOverDecorationBaseZIndex = 300;

        internal static readonly int IsCurrentPropertyKey = PropertyKeys.Register(typeof(bool), "IsCurrent");
        internal static readonly int IsPointerOverPropertyKey = PropertyKeys.Register(typeof(bool), "IsPointerOver");
        internal static readonly int IsBlackoutPropertyKey = PropertyKeys.Register(typeof(bool), "IsBlackout");
        internal static readonly int IsHighlightedPropertyKey = PropertyKeys.Register(typeof(bool), "IsHighlighted");
        internal static readonly int IsSelectedPropertyKey = PropertyKeys.Register(typeof(bool), "IsSelected");
        internal static readonly int IsFromAnotherViewPropertyKey = PropertyKeys.Register(typeof(bool), "IsFromAnotherView");

        private const int DefaultSelectedDecorationBaseZIndex = 100;

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarCellModel"/> class.
        /// </summary>
        public CalendarCellModel()
        {
            this.TrackPropertyChanging = true;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CalendarCellModel"/> class.
        /// </summary>
        /// <param name="row">The row index of the cell.</param>
        /// <param name="column">The column index of the cell.</param>
        public CalendarCellModel(int row, int column) : this()
        {
            this.RowIndex = row;
            this.ColumnIndex = column;
        }

        /// <summary>
        /// Gets the date represented by the calendar cell.
        /// </summary>
        public DateTime Date
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the calendar cell is selectable (i.e. enabled).
        /// </summary>
        public bool IsBlackout
        {
            get
            {
                return this.GetTypedValue<bool>(IsBlackoutPropertyKey, false);
            }
            internal set
            {
                this.SetValue(IsBlackoutPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the calendar cell is highlighted.
        /// </summary>
        public bool IsHighlighted
        {
            get
            {
                return this.GetTypedValue<bool>(IsHighlightedPropertyKey, false);
            }
            internal set
            {
                this.SetValue(IsHighlightedPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the calendar cell is selected.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.GetTypedValue<bool>(IsSelectedPropertyKey, false);
            }
            internal set
            {
                this.SetValue(IsSelectedPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this calendar cell represents a date that belongs to another view.
        /// </summary>
        public bool IsFromAnotherView
        {
            get
            {
                return this.GetTypedValue<bool>(IsFromAnotherViewPropertyKey, false);
            }
            internal set
            {
                this.SetValue(IsFromAnotherViewPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether the pointer is over this calendar cell.
        /// </summary>
        public bool IsPointerOver
        {
            get
            {
                return this.GetTypedValue<bool>(IsPointerOverPropertyKey, false);
            }
            internal set
            {
                this.SetValue(IsPointerOverPropertyKey, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether this calendar cell is currently focused.
        /// </summary>
        public bool IsCurrent 
        {
            get
            {
                return this.GetTypedValue<bool>(IsCurrentPropertyKey, false);
            }
            internal set
            {
                this.SetValue(IsCurrentPropertyKey, value);
            }
        }

        internal int PrimaryDecorationZIndex
        {
            get
            {
                if (this.IsBlackout)
                {
                    return this.CollectionIndex;
                }
                else if (this.IsSelected)
                {
                    return this.CollectionIndex + DefaultSelectedDecorationBaseZIndex;
                }
                else if (this.IsFromAnotherView)
                {
                    return this.CollectionIndex;
                }
                else if (this.IsHighlighted)
                {
                    return this.CollectionIndex + DefaultHighlightedDecorationBaseZIndex;
                }
                else
                {
                    return this.CollectionIndex;
                }
            }
        }

        internal int CurrentDecorationZIndex
        {
            get
            {
                return this.CollectionIndex + DefaultCurrentDecorationBaseZIndex;
            }
        }

        internal int PointerOverDecorationZIndex
        {
            get
            {
                return this.CollectionIndex + DefaultPointerOverDecorationBaseZIndex;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="CalendarCellModel"/> represents a normal cell. Exposed for testing purposes.
        /// </summary>
        internal bool IsNormal
        {
            get
            {
                return !this.IsFromAnotherView && !this.IsBlackout && !this.IsSelected && !this.IsHighlighted && !this.IsPointerOver && !this.IsCurrent;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void OnPropertyChanging(RadPropertyEventArgs e)
        {
            base.OnPropertyChanging(e);

            if (e.Key == IsBlackoutPropertyKey && e.NewValue != null)
            {
                bool isBlackout = (bool)e.NewValue;
                if (isBlackout && this.IsSelected)
                {
                    this.IsSelected = false;
                }
            }
            else if (e.Key == IsSelectedPropertyKey && e.NewValue != null)
            {
                bool isSelected = (bool)e.NewValue;
                if (isSelected && this.IsBlackout)
                {
                    // Calendar cell cannot be marked as blackout and selected at the same time
                    e.Cancel = true;
                }
            }
        }
    }
}
