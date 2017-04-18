using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    /// <summary>
    /// Test ResultProvider used to check whether GroupFilter give correct Cooridnate to GetAggregateResult method.
    /// </summary>
    internal class FakeAggregatesResultProviderMock : IAggregateResultProvider
    {
        public DataAxis Axis { get; set; }

        public FakeAggregatesResultProviderMock()
        {
            Group rowRoot = new Group("Grand Total");
            Group colRoot = new Group("Grand Total");

            this.Root = new Coordinate(rowRoot, colRoot);
        }

        public AggregateValue GetAggregateResult(int aggregate, Coordinate groups)
        {
            if (this.Axis == DataAxis.Rows && groups.ColumnGroup != this.Root.ColumnGroup
                || this.Axis == DataAxis.Columns && groups.RowGroup != this.Root.RowGroup)
            {
                Assert.Fail("Wrong RootGroup specified");
            }

            return null;
        }

        public Coordinate Root
        {
            get;
            private set;
        }
    }
}