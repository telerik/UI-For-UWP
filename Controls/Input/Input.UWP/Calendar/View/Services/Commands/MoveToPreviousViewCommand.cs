using System;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    internal class MoveToPreviousViewCommand : CalendarCommand
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
            this.MoveToPreviousView(context.AnimationStoryboard, context.navigationStep, context.weekendsVisible);
        }

        private void MoveToPreviousView(Storyboard animationStoryboard, int navigationStep, bool weekendsVisible)
        {
            DateTime newDisplayDate = CalendarMathHelper.IncrementByView(this.Owner.DisplayDate, -navigationStep, this.Owner.DisplayMode, weekendsVisible);
            this.Owner.MoveToDate(newDisplayDate, animationStoryboard);
        }
    }
}
