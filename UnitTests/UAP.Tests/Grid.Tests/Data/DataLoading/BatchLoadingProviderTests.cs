using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Telerik.Core.Data;

namespace Telerik.UI.Xaml.Controls.Grid.Tests.Data.DataLoading
{
    [TestClass]
    public class BatchLoadingProviderTests
    {
        private IIncrementalBatchLoading loadingCollection;

        [TestMethod]
        public void SettingSource_WhenSourceIsIIncrementalBatchLoading_ShouldSetBatchSize()
        {
            loadingCollection = new IncrementalLoadingCollection<object>((c) =>new Task<IEnumerable<object>>(()=>Enumerable.Empty<object>())) { BatchSize = 10 };

            var provider = new BatchLoadingProvider<object>(loadingCollection, loadingCollection as ICollection<object>);

            Assert.AreEqual((uint)10, provider.BatchSize);
        }
    }
}
