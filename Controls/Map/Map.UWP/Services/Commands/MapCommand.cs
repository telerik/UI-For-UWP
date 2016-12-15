using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Map
{
    /// <summary>
    /// Represents the base class for all commands in a <see cref="RadMap"/> instance.
    /// </summary>
    public abstract class MapCommand : ControlCommandBase<RadMap>
    {
        private CommandId id;

        /// <summary>
        /// Gets or sets the <see cref="CommandId"/> value for this instance.
        /// This value is used to associate a command with a known event within a <see cref="RadMap"/> instance.
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
