using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Input.Calendar
{
    /// <summary>
    /// Represents a node in the logical tree of the calendar control.
    /// </summary>
    public abstract class CalendarNode : Node
    {
        internal static readonly int LabelPropertyKey = PropertyKeys.Register(typeof(string), "Label");

        /// <summary>
        /// Gets the label of the calendar cell.
        /// </summary>
        public string Label
        {
            get
            {
                return this.GetTypedValue<string>(LabelPropertyKey, null);
            }
            internal set
            {
                this.SetValue(LabelPropertyKey, value);
            }
        }

        internal int RowIndex
        {
            get;
            set;
        }

        internal int ColumnIndex
        {
            get;
            set;
        }

        // TODO: Reconsider this as it breaks the "model" abstraction (the CalendarCellStyleContext class contains platform-specific visual references).
        internal CalendarCellStyleContext Context
        {
            get;
            set;
        }
    }
}
