using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu.Commands
{
    internal class NavigateCommand : RadialMenuCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is NavigateContext;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            NavigateContext context = parameter as NavigateContext;

            if (context == null)
            {
                return;
            }

            this.Owner.model.NavigateToView(context, context.IsBackButtonPressed);
        }
    }
}
