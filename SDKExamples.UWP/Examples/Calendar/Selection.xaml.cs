using System;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Calendar
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Selection : ExamplePageBase
    {
        public Selection()
        {
            this.InitializeComponent();

            this.DataContext = Enum.GetValues(typeof(CalendarSelectionMode));
        }

        private void ModeSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.calendar.SelectionMode = (CalendarSelectionMode)(sender as ListView).SelectedItem;
        }
    }
}
