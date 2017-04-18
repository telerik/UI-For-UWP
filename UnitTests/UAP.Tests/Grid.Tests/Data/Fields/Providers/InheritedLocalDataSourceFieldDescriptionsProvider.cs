using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal class InheritedLocalDataSourceFieldDescriptionsProvider : LocalDataSourceFieldDescriptionsProvider
    {
        private LocalDataSourceProvider localProvider;

        public InheritedLocalDataSourceFieldDescriptionsProvider(LocalDataSourceProvider provider) : base()
        {
            this.localProvider = provider;
        }

        public IFieldInfoData ExposedGenerateDescriptionsData()
        {
            this.GetDescriptionsDataAsync(this.localProvider.ItemsSource);
            return this.GenerateDescriptionsData();
        }
    }
}