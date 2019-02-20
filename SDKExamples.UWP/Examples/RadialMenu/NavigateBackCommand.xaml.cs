using Telerik.UI.Xaml.Controls.Primitives.Menu.Commands;

namespace SDKExamples.UWP.Examples.RadialMenu
{
    public sealed partial class NavigateBackCommand : ExamplePageBase
    {
        public NavigateBackCommand()
        {
            this.InitializeComponent();
        }

        private void NavigateBackClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.radialMenu.CommandService.ExecuteCommand(CommandId.NavigateBack, null);
        }
    }
}