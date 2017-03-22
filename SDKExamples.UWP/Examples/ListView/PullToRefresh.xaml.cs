using System;
using System.Collections.ObjectModel;
using Telerik.UI.Xaml.Controls.Data;
using Windows.UI.Xaml.Controls;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace SDKExamples.UWP.Listview
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class PullToRefresh : ExamplePageBase
    {
        ObservableCollection<int> source = new ObservableCollection<int>() { 1, 2, 3, 4, 5, 6 };

        public PullToRefresh()
        {
            this.InitializeComponent();

            this.DataContext = this.source;
        }

        private void RefreshRequested(object sender, EventArgs e)
        {
            this.source.Add(this.source.Count + 1);
            this.source.Add(this.source.Count + 1);
            this.source.Add(this.source.Count + 1);
            this.source.Add(this.source.Count + 1);

           (sender as RadListView).IsPullToRefreshActive = false;
        }
    }
}
