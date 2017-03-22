using System;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Rating
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Selection : ExamplePageBase
    {
        public Selection()
        {
            this.InitializeComponent();

            this.selectionCombo.ItemsSource = Enum.GetValues(typeof(RatingSelectionMode));
            this.selectionCombo.SelectedIndex = 0;
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.rating.RatingSelectionMode = (RatingSelectionMode)(sender as ComboBox).SelectedItem;
        }
    }
}
