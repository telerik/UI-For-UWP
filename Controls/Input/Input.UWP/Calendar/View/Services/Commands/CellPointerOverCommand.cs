namespace Telerik.UI.Xaml.Controls.Input.Calendar.Commands
{
    internal class CellPointerOverCommand : CalendarCommand
    {
        /// <summary>
        /// Determines whether the command can be executed against the provided parameter.
        /// </summary>
        public override bool CanExecute(object parameter)
        {
            return parameter is CalendarCellModel;
        }

        /// <summary>
        /// Performs the core action given the provided parameter.
        /// </summary>
        public override void Execute(object parameter)
        {
            base.Execute(parameter);

            if (this.Owner.DisplayMode != CalendarDisplayMode.MultiDayView)
            {
                this.Owner.VisualStateService.UpdateHoverDecoration(parameter as CalendarCellModel);
            }
        }
    }
}
