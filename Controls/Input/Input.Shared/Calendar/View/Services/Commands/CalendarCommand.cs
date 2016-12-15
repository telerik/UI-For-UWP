using System;
using System.Windows.Input;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    /// <summary>
    /// Represents a command abstraction that is associated with a particular <see cref="RadCalendar"/> instance.
    /// </summary>
    public abstract class CalendarCommand : ControlCommandBase<RadCalendar>
    {
        private CommandId id;

        /// <summary>
        /// Gets or sets the <see cref="Id"/> value for this instance.
        /// This value is used to associate a command with a known event within a <see cref="RadCalendar"/> instance.
        /// </summary>
        public CommandId Id
        {
            get
            {
                return this.id;
            }
            set
            {
                this.id = value;            
            }
        }
        
        internal override int CommandId
        {
            get
            {
                return (int)this.id;
            }
        }
    }
}
