using Telerik.UI.Xaml.Controls.Input.Calendar;
using Telerik.UI.Xaml.Controls.Input.Calendar.Commands;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Calendar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RemoveNavigationAnimations : ExamplePageBase
    {
        public RemoveNavigationAnimations()
        {
            this.InitializeComponent();
        }

        private void NavigateToUpperViewClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.calendar.MoveToUpperView();
        }

        private void NavigateToLowerViewClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.calendar.MoveToLowerView(this.calendar.CurrentDate);
        }
    }

    public class CustomMoveToUpperViewCommand : CalendarCommand
    {
        public CustomMoveToUpperViewCommand()
        {
            this.Id = CommandId.MoveToUpperView;
        }
        public override bool CanExecute(object parameter)
        {
            return true;
        }
        public override void Execute(object parameter)
        {
            (parameter as CalendarViewChangeContext).AnimationStoryboard = null;
            this.Owner.CommandService.ExecuteDefaultCommand(CommandId.MoveToUpperView, parameter);
        }
    }

}
