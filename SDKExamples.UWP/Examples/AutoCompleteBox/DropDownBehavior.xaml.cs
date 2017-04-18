using System;
using System.Collections.Generic;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.AutoCompleteBox
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class DropDownBehavior : ExamplePageBase
    {
        private bool isOpen = false;

        public DropDownBehavior()
        {
            this.InitializeComponent();

            this.combo.ItemsSource = Enum.GetValues(typeof(AutoCompleteBoxPlacementMode));
            this.autoComplete.ItemsSource = new List<string>() { "Ivo", "Ivaylo", "Iva", "Yasen", "Yavor" };
        }

        private void combo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.autoComplete.DropDownPlacement = (AutoCompleteBoxPlacementMode)(sender as ComboBox).SelectedItem;
        }


        private void Button_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
            isOpen = !isOpen;
            this.autoComplete.IsDropDownOpen = isOpen;
        }
    }
}
