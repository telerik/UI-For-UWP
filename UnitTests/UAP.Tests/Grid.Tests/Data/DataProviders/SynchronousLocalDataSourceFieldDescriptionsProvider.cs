using System;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal class SynchronousLocalDataSourceFieldDescriptionsProvider : LocalDataSourceFieldDescriptionsProvider
    {
        public SynchronousLocalDataSourceFieldDescriptionsProvider(LocalDataSourceProvider provider)
            : base()
        {

        }
    }
}
