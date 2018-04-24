using System;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    internal class MoveToUpperViewCommand : CalendarCommand
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

            this.MoveToUpperView(context.AnimationStoryboard);
        }

        private void MoveToUpperView(Storyboard animationStoryboard)
        {
            bool displayModeChanged = true;

            switch (this.Owner.DisplayMode)
            {
                case CalendarDisplayMode.MultiDayView:
                    this.Owner.DisplayMode = CalendarDisplayMode.MonthView;
                    break;
                case CalendarDisplayMode.MonthView:
                    this.Owner.DisplayMode = CalendarDisplayMode.YearView;
                    break;
                case CalendarDisplayMode.YearView:
                    this.Owner.DisplayMode = CalendarDisplayMode.DecadeView;
                    break;
                case CalendarDisplayMode.DecadeView:
                    this.Owner.DisplayMode = CalendarDisplayMode.CenturyView;
                    break;
                default:
                    displayModeChanged = false;
                    break;
            }

            if (displayModeChanged && animationStoryboard != null)
            {
                animationStoryboard.Begin();
            }
        }
    }
}
