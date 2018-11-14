using System.Linq;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu.Commands
{
    internal class NavigateBackCommand : RadialMenuCommand
    {
        public override bool CanExecute(object parameter)
        {
            return this.Owner.model.viewState.MenuLevels.Count > 0;
        }

        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            var menuLevels = this.Owner.model.viewState.MenuLevels;

            if (menuLevels.Count > 0)
            {
                var sourceItem = menuLevels.Pop();
                var targetLevel = menuLevels.FirstOrDefault();
                var startAngleLevels = this.Owner.model.viewState.StartAngleLevels;

                startAngleLevels.RemoveAt(startAngleLevels.Count - 1);
                var startAngle = startAngleLevels.LastOrDefault();

                this.Owner.RaiseNavigateCommand(targetLevel, sourceItem, startAngle, true);

                if (targetLevel == null)
                {
                    this.Owner.menuButton.TransformToNormal();
                }
            }
        }
    }
}
