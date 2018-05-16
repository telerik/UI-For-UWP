using System;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    internal class MoveToLowerViewCommand : CalendarCommand
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
                this.MoveToLowerView(context.Date.Value, context.AnimationStoryboard);
            }
        }

        private void MoveToLowerView(DateTime date, Storyboard animationStoryboard)
        {
            bool displayModeChanged = true;
            switch (this.Owner.DisplayMode)
            {
                case CalendarDisplayMode.MonthView:
                    this.Owner.DisplayMode = CalendarDisplayMode.MultiDayView;
                    this.Owner.DisplayDate = date;
                    break;
                case CalendarDisplayMode.YearView:
                    this.Owner.DisplayMode = CalendarDisplayMode.MonthView;
                    this.Owner.DisplayDate = date;
                    break;
                case CalendarDisplayMode.DecadeView:
                    this.Owner.DisplayMode = CalendarDisplayMode.YearView;
                    this.Owner.DisplayDate = date;
                    break;
                case CalendarDisplayMode.CenturyView:
                    this.Owner.DisplayMode = CalendarDisplayMode.DecadeView;
                    this.Owner.DisplayDate = date;
                    break;
                default:
                    displayModeChanged = false;
                    break;
            }

            if (!displayModeChanged)
            {
                return;
            }

            if (animationStoryboard != null)
            {
                animationStoryboard.Begin();
            }
        }
    }
}
