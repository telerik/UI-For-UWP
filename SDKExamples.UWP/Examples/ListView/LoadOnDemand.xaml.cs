using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Core.Data;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LoadOnDemand : ExamplePageBase
    {
        public LoadOnDemand()
        {
            this.InitializeComponent();

            this.DataContext = new ViewModel();

            this.loadingModeCombo.ItemsSource = Enum.GetValues(typeof(BatchLoadingMode));
            this.loadingModeCombo.SelectedIndex = 0;
        }

        public class ViewModel
        {
            private int currentCount = 0;

            public ViewModel()
            {
                this.Items = new IncrementalLoadingCollection<int>(this.GetMoreItems) { BatchSize = 5 };
            }

            public IncrementalLoadingCollection<int> Items { get; set; }

            private async Task<IEnumerable<int>> GetMoreItems(uint count)
            {
                await Task.Delay(200);
                var result = Enumerable.Range(this.currentCount, (int)count).Select(x => (int)x).ToList();
                currentCount += (int)count;
                return result;
            }
        }
         
        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.ListView.IncrementalLoadingMode = (BatchLoadingMode)(sender as ComboBox).SelectedItem;
        }
    }
}
