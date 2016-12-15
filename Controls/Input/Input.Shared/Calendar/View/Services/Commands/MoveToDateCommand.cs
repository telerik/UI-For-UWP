using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    internal class MoveToDateCommand : CalendarCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is CalendarViewChangeContext;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            CalendarViewChangeContext context = parameter as CalendarViewChangeContext;

            if (context.Date.HasValue)
            {
                this.Owner.MoveToDate(context.Date.Value, context.AnimationStoryboard);
            }
        }
    }
}
