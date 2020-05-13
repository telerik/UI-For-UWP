using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Telerik.Core.Data;
using Telerik.UI.Xaml.Controls.Data.ListView.Commands;
using Windows.UI.Xaml.Controls;

namespace SDKExamples.UWP.Listview
{
    public sealed partial class LoadOnDemandCommand : ExamplePageBase
    {
        public LoadOnDemandCommand()
        {
            this.InitializeComponent();

            this.loadingModeCombo.ItemsSource = Enum.GetValues(typeof(BatchLoadingMode));
            this.loadingModeCombo.SelectedIndex = 0;

            this.ListView.Commands.Add(new CustomLoadOnDemandCommand());

            this.DataContext = new ViewModel();
        }

        public class ViewModel
        {
            public ViewModel()
            {
                this.Items = new ObservableCollection<int>();
                this.AddItems(40);
            }

            public ObservableCollection<int> Items { get; }

            public void AddItems(int count)
            {
                for (int i = 0; i < count; i++)
                {
                    this.Items.Add(this.Items.Count);
                }
            }
        }

        private void SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox comboBox = (ComboBox)sender;
            this.ListView.IncrementalLoadingMode = (BatchLoadingMode)comboBox.SelectedItem;
        }
    }

    public class CustomLoadOnDemandCommand : LoadMoreDataCommand
    {
        private int lodCounter;

        public CustomLoadOnDemandCommand()
            :base()
        {
            this.Id = CommandId.LoadMoreData;
        }

        public override bool CanExecute(object parameter)
        {
            LoadMoreDataContext context = (LoadMoreDataContext)parameter;
            LoadOnDemandCommand.ViewModel viewModel = (LoadOnDemandCommand.ViewModel)context.DataContext;

            bool canExecute = viewModel.Items.Count < 100;
            return canExecute;
        }

        public async override void Execute(object parameter)
        {
            base.Execute(parameter);

            LoadMoreDataContext context = (LoadMoreDataContext)parameter;
            LoadOnDemandCommand.ViewModel viewModel = (LoadOnDemandCommand.ViewModel)context.DataContext;
            this.lodCounter++;

            if (this.lodCounter % 3 == 0)
            {
                // If we do not need to get new data asynchronously, we can add the new items right away.
                viewModel.AddItems(15);
            }
            else
            {
                // If we need to get new data asynchronously, we must manually update the loading status.

                this.Owner.ChangeDataLoadingStatus(BatchLoadingStatus.ItemsRequested);

                // Mimic getting data from server asynchronously.
                await Task.Delay(2000);

                viewModel.AddItems(15);

                this.Owner.ChangeDataLoadingStatus(BatchLoadingStatus.ItemsLoaded);
            }
        }
    }
}
