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
    public sealed partial class FilterSettings : ExamplePageBase
    {
        public FilterSettings()
        {
            this.InitializeComponent();

            this.filterModeCombo.ItemsSource = Enum.GetValues(typeof(AutoCompleteBoxFilterMode));
            this.autoComplete.ItemsSource = new List<string>() { "Alon", "John", "Ivo", "Ivaylo", "Iva", "Yasen", "Yavor" };
        }

        private void filterModeCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.autoComplete.FilterMode = (AutoCompleteBoxFilterMode)(sender as ComboBox).SelectedItem;
        }
    }
}
