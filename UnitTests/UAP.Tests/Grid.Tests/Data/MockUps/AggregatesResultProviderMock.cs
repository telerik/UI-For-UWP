using System.Collections.Generic;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Aggregates;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    /// <summary>
    /// Based on PivotTable04.xlsm.
    /// RowLabels -> Product, Advertisements, Promotion.
    /// ColumnLabels -> Date, Values.
    /// Aggregates/Values -> Sum of Quantity, Count of Net $
    /// </summary>
    internal class AggregatesResultProviderMock : IAggregateResultProvider
    {
        public IDictionary<Coordinate, AggregateValue[]> Aggregates { get; private set; }

        public AggregatesResultProviderMock()
        {
            this.Aggregates = new Dictionary<Coordinate, AggregateValue[]>();

            Group rowRoot = new Group("Grand Total");
            Group colRoot = new Group("Grand Total");

            this.Root = new Coordinate(rowRoot, colRoot);

            var june = new Group("Jun");
            var july = new Group("Jul");
            var august = new Group("Aug");
            colRoot.AddGroup(june);
            colRoot.AddGroup(july);
            colRoot.AddGroup(august);

            var parent = new Group("Copy holder");
            rowRoot.AddGroup(parent);

            this.Add(parent, colRoot, 615, 17, 509, 6, 315, 6, 1439, 29);

            var child = new Group("Direct mail");
            parent.AddGroup(child);

            var oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            var extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 136, 3, 115, 2, 71, 2, 322, 7);
            this.Add(oneFree, colRoot, 66, 1, 55, 1, 33, 1, 154, 3);
            this.Add(extraDi, colRoot, 70, 2, 60, 1, 38, 1, 168, 4);

            child = new Group("Magazine");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 247, 8, 174, 2, 134, 2, 555, 12);
            this.Add(oneFree, colRoot, 165, 6, 99, 1, 77, 1, 341, 8);
            this.Add(extraDi, colRoot, 82, 2, 75, 1, 57, 1, 214, 4);

            child = new Group("Newspaper");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 232, 6, 220, 2, 110, 2, 562, 10);
            this.Add(oneFree, colRoot, 121, 1, 132, 1, 44, 1, 297, 3);
            this.Add(extraDi, colRoot, 111, 5, 88, 1, 66, 1, 265, 7);



            parent = new Group("Glare filter");
            rowRoot.AddGroup(parent);

            this.Add(parent, colRoot, 730, 19, 583, 6, 395, 6, 1708, 31);

            child = new Group("Direct mail");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 168, 2, 142, 2, 92, 2, 402, 6);
            this.Add(oneFree, colRoot, 99, 1, 77, 1, 44, 1, 220, 3);
            this.Add(extraDi, colRoot, 69, 1, 65, 1, 48, 1, 182, 3);

            child = new Group("Magazine");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 310, 15, 234, 2, 175, 2, 719, 19);
            this.Add(oneFree, colRoot, 154, 7, 110, 1, 88, 1, 352, 9);
            this.Add(extraDi, colRoot, 156, 8, 124, 1, 87, 1, 367, 10);

            child = new Group("Newspaper");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 252, 2, 207, 2, 128, 2, 587, 6);
            this.Add(oneFree, colRoot, 132, 1, 77, 1, 33, 1, 242, 3);
            this.Add(extraDi, colRoot, 120, 1, 130, 1, 95, 1, 345, 3);



            parent = new Group("Mouse pad");
            rowRoot.AddGroup(parent);

            this.Add(parent, colRoot, 1354, 20, 1153, 6, 853, 6, 3360, 32);

            child = new Group("Direct mail");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 287, 2, 262, 2, 203, 2, 752, 6);
            this.Add(oneFree, colRoot, 165, 1, 121, 1, 99, 1, 385, 3);
            this.Add(extraDi, colRoot, 122, 1, 141, 1, 104, 1, 367, 3);

            child = new Group("Magazine");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 662, 13, 549, 2, 385, 2, 1596, 17);
            this.Add(oneFree, colRoot, 374, 8, 275, 1, 187, 1, 836, 10);
            this.Add(extraDi, colRoot, 288, 5, 274, 1, 198, 1, 760, 7);

            child = new Group("Newspaper");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 405, 5, 342, 2, 265, 2, 1012, 9);
            this.Add(oneFree, colRoot, 187, 1, 176, 1, 121, 1, 484, 3);
            this.Add(extraDi, colRoot, 218, 4, 166, 1, 144, 1, 528, 6);



            parent = new Group("Printer stand");
            rowRoot.AddGroup(parent);

            this.Add(parent, colRoot, 560, 17, 451, 6, 333, 6, 1344, 29);

            child = new Group("Direct mail");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 170, 5, 104, 2, 64, 2, 338, 9);
            this.Add(oneFree, colRoot, 88, 3, 44, 1, 44, 1, 176, 5);
            this.Add(extraDi, colRoot, 82, 2, 60, 1, 20, 1, 162, 4);

            child = new Group("Magazine");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 200, 6, 190, 2, 156, 2, 546, 10);
            this.Add(oneFree, colRoot, 110, 2, 88, 1, 66, 1, 264, 4);
            this.Add(extraDi, colRoot, 90, 4, 102, 1, 90, 1, 282, 6);

            child = new Group("Newspaper");
            parent.AddGroup(child);

            oneFree = new Group("1 Free with 10");
            child.AddGroup(oneFree);

            extraDi = new Group("Extra Discount");
            child.AddGroup(extraDi);

            this.Add(child, colRoot, 190, 6, 157, 2, 113, 2, 460, 10);
            this.Add(oneFree, colRoot, 88, 5, 77, 1, 33, 1, 198, 7);
            this.Add(extraDi, colRoot, 102, 1, 80, 1, 80, 1, 262, 3);

            this.Add(rowRoot, colRoot, 3259, 73, 2696, 24, 1896, 24, 7851, 121);
        }

        private void Add(Group oneFree, Group colRoot, params int[] values)
        {
            int count = 0;
            AggregateValue[] aggregateValues = null;
            foreach (var group in colRoot.Items.OfType<IGroup>())
            {
                aggregateValues = new ConstantValueAggregate[2];
                aggregateValues[0] = new ConstantValueAggregate(values[count++]);
                aggregateValues[1] = new ConstantValueAggregate(values[count++]);
                this.Aggregates[new Coordinate(oneFree, group)] = aggregateValues;
            }

            aggregateValues = new ConstantValueAggregate[2];
            aggregateValues[0] = new ConstantValueAggregate(values[count++]);
            aggregateValues[1] = new ConstantValueAggregate(values[count++]);
            this.Aggregates[new Coordinate(oneFree, colRoot)] = aggregateValues;
        }

        public Coordinate Root
        {
            get;
            private set;
        }

        public AggregateValue GetAggregateResult(int aggregate, Coordinate groups)
        {
            AggregateValue[] values = GetAggregateResults(groups);
            if (values == null)
            {
                return null;
            }
            return values[aggregate];
        }

        private AggregateValue[] GetAggregateResults(Coordinate coordinate)
        {
            AggregateValue[] results;
            if (this.Aggregates.TryGetValue(coordinate, out results))
            {
                return results;
            }

            return null;
        }
    }
}
