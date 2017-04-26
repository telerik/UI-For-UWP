using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Data.ListView.Commands
{
    /// <summary>
    /// Represents a command abstraction that is associated with a particular <see cref="RadListView"/> instance.
    /// </summary>
    public abstract class ListViewCommand : ControlCommandBase<RadListView>
    {
        private CommandId id;

        /// <summary>
        /// Gets or sets the <see cref="CommandId"/> value for this instance.
        /// This value is used to associate a command with a known event within a <see cref="RadListView"/> instance.
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
