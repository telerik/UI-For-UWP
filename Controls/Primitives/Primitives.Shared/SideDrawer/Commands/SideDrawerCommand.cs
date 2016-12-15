using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.SideDrawer.Commands
{
    /// <summary>
    /// Represents a command abstraction that is associated with a particular <see cref="SideDrawerCommand"/> instance.
    /// </summary>
    public abstract class SideDrawerCommand : ControlCommandBase<RadSideDrawer>
    {
        private CommandId id;

        /// <summary>
        /// Gets or sets the <see cref="CommandId"/> value for this instance.
        /// This value is used to associate a command with a known event within a <see cref="RadSideDrawer"/> instance.
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
