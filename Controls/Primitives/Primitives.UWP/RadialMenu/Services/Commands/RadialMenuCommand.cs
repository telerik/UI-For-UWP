using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu.Commands
{
    /// <summary>
    /// Represents a command abstraction that is associated with a particular <see cref="RadRadialMenu"/> instance.
    /// </summary>
    public class RadialMenuCommand : ControlCommandBase<RadRadialMenu>
    {
        private CommandId id;

        /// <summary>
        /// Gets or sets the <see cref="Id"/> value for this instance.
        /// This value is used to associate a command with a known event within a <see cref="RadRadialMenu"/> instance.
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
