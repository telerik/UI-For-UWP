using Telerik.UI.Xaml.Controls.Primitives;

namespace Telerik.UI.Xaml.Controls.Map
{
    public partial class RadMap
    {
        private MapCommandService commandService;

        /// <summary>
        /// Gets the <see cref="MapCommandService"/> instance that handles the commanding support in the map.
        /// </summary>
        public MapCommandService CommandService
        {
            get
            {
                return this.commandService;
            }
        }

        /// <summary>
        /// Gets the collection with user-defined commands, associated with the different <see cref="CommandId"/> values.
        /// </summary>
        public CommandCollection<RadMap> Commands
        {
            get
            {
                return this.commandService.UserCommands;
            }
        }
    }
}
