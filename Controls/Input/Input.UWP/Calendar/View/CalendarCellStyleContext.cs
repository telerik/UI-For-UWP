using System;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents a context passed to a <see cref="CalendarCellStyleSelector"/> instance.
    /// </summary>
    public class CalendarCellStyleContext
    {
        internal CalendarCellStyleContext()
        {
        }

        internal CalendarCellStyleContext(CalendarCellStateContext context)
        {
            this.Date = context.Date;

            this.IsPointerOver = context.IsPointerOver;
            this.IsCurrent = context.IsCurrent;
            this.IsBlackout = context.IsBlackout;
            this.IsSelected = context.IsSelected;
            this.IsHighlighted = context.IsHighlighted;
            this.IsFromAnotherView = context.IsFromAnotherView;

            this.ApplyCellTemplateDecorations = true;
        }

        /// <summary>
        /// Gets the label of the respective calendar cell.
        /// </summary>
        public string Label
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets the date of the respective calendar cell.
        /// </summary>
        public DateTime Date
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets the custom cell style that will override the default style logic of the calendar cells.
        /// </summary>
        /// <remarks>
        /// This property is null by default. Set it only if you want to override the default cell style logic of the associated calendar cell.
        /// </remarks>
        public CalendarCellStyle CellStyle
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the custom cell template that will override the default style and template logic of the calendar cells.
        /// </summary>
        /// <remarks>
        /// This property is null by default. Set it only if you want to override the default template logic of the associated calendar cell.
        /// </remarks>
        public DataTemplate CellTemplate
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the decorations associated with the default cell visual logic will be applied if custom cell
        /// template is set through the <see cref="CellTemplate"/>.
        /// </summary>
        public bool ApplyCellTemplateDecorations
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the pointer is over the associated calendar cell.
        /// </summary>
        public bool IsPointerOver
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell is currently focused.
        /// </summary>
        public bool IsCurrent
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell is selectable (i.e. enabled).
        /// </summary>
        public bool IsBlackout
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell is highlighted.
        /// </summary>
        public bool IsHighlighted
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell is selected.
        /// </summary>
        public bool IsSelected
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets a value indicating whether the associated calendar cell belongs to another view.
        /// </summary>
        public bool IsFromAnotherView
        {
            get;
            internal set;
        }

        internal Style CalculatedContentCellStyle
        {
            get;
            set;
        }

        internal Style CalculatedDecorationCellStyle
        {
            get;
            set;
        }

        internal Style GetEffectiveCellDecorationStyle()
        {
            Style effectiveStyle = null;

            if (this.CellTemplate == null || this.ApplyCellTemplateDecorations)
            {
                if (this.CellStyle != null)
                {
                    effectiveStyle = this.CellStyle.DecorationStyle;
                }
                else
                {
                    effectiveStyle = this.CalculatedDecorationCellStyle;
                }
            }

            return effectiveStyle;
        }

        internal Style GetEffectiveCellContentStyle()
        {
            Style effectiveStyle = null;

            // NOTE: If custom cell template is used, we assume no additional cell content customizations will be applied.
            if (this.CellTemplate == null)
            {
                if (this.CellStyle != null)
                {
                    effectiveStyle = this.CellStyle.ContentStyle;
                }
                else
                {
                    effectiveStyle = this.CalculatedContentCellStyle;
                }
            }

            return effectiveStyle;
        }
    }
}
