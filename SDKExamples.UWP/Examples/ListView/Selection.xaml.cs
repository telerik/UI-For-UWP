using System;
using System.Collections;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Data.ListView;
using System.Linq;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Selection : ExamplePageBase
    {
        public Selection()
        {
            this.InitializeComponent();

            this.DataContext = new List<int>() { 1, 2, 3, 4, 5, 6 };
            this.selectionCombo.ItemsSource = Enum.GetValues(typeof(DataControlsSelectionMode));
            this.selectionCombo.SelectedIndex = 0;
        }

        private void SelectionViewSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.radListView.SelectionMode = (DataControlsSelectionMode)(sender as ComboBox).SelectedItem;
        }

        private void SelectFirstItemClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.radListView.SelectItem((this.radListView.ItemsSource as List<int>).FirstOrDefault());
        }

        private void DeselectItemClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.radListView.DeselectItem(this.radListView.SelectedItem);
        }

        private void SelectAllClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.radListView.SelectAll();
        }

        private void DeselectAllClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.radListView.DeselectAll();
        }

        private void TurnONOFFCheckboxesClick(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            this.radListView.IsCheckModeActive = !this.radListView.IsCheckModeActive;
        }
    }
}
