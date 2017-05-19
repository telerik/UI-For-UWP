using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Data.DataForm.Commands
{
    /// <summary>
    /// Represents a command abstraction that is associated with a particular <see cref="RadDataForm"/> instance.
    /// </summary>
    public abstract class DataFormCommand : ControlCommandBase<RadDataForm>
    {
        private CommandId id;

        /// <summary>
        /// Gets or sets the <see cref="CommandId"/> value for this instance.
        /// This value is used to associate a command with a known event within a <see cref="RadDataForm"/> instance.
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
