using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class ItemSwipe : ExamplePageBase
    {
        public ItemSwipe()
        {
            this.InitializeComponent();
            this.DataContext = new List<int>() { 1, 2, 4, 5, 6, 7, 8 };
        }
    }

        public class ItemSwipeCommand : ListViewCommand
        {
            public override bool CanExecute(object parameter)
            {
                return parameter is ItemSwipeActionCompleteContext;
            }

            public override void Execute(object parameter)
            {
                // do some work
                var context = parameter as ItemSwipeActionCompleteContext;

                this.Owner.CommandService.ExecuteDefaultCommand(CommandId.ItemSwipeActionComplete, parameter);
            }
        }

        public class ItemActionTapCommand : ListViewCommand
        {
            public override bool CanExecute(object parameter)
            {
                return parameter is ItemActionTapContext;
            }

            public override void Execute(object parameter)
            {
                 // do some work
                 var context = parameter as ItemActionTapContext;
                 
                 this.Owner.CommandService.ExecuteDefaultCommand(CommandId.ItemActionTap, context);
             }
         }
}
