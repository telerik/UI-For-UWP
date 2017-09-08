using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Telerik.Core;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data.Core.Aggregates;
using Telerik.Data.Core.Engine;
using Telerik.Data.Core.Totals;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class ParallelDataEngineTests
    {
        #region MockUps
        private class OrdersFilterDescription : FilterDescription
        {
            public OrderFields Field { get; set; }

            public Predicate<object> Filter { get; set; }

            internal object GetFilterItem(object item)
            {
                return this.Field.GetField(item);
            }

            internal bool PassesFilter(object value)
            {
                return this.Filter == null ? true : this.Filter(value);
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new OrdersFilterDescription();
            }

            protected override void CloneCore(Cloneable source)
            {
                var original = source as OrdersFilterDescription;
                if (original != null)
                {
                    this.Field = original.Field;
                    this.Filter = original.Filter;
                }
                base.CloneCore(source);
            }
        }

        private class OrdersGroupDescription : GroupDescription
        {
            public bool IsSlowRunning { get; set; }
            private int sleepItteration = 0;

            public OrderFields Field { get; set; }
            public IEnumerable<object> WellKnownGroupNames { get; set; }

            public GroupComparer GroupComparer
            {
                get
                {
                    return ((IGroupDescription)this).GroupComparer;
                }
                set
                {
                    ((IGroupDescription)this).GroupComparer = value;
                }
            }

            public virtual object GroupNameFromItem(object item)
            {
                if (this.IsSlowRunning)
                {
                    this.sleepItteration++;
                    if (this.sleepItteration > 10)
                    {
                        new System.Threading.ManualResetEvent(false).WaitOne(1);
                        this.sleepItteration = 0;
                    }
                }
                return this.Field.GetField(item);
            }

            protected override string GetDisplayName()
            {
                return this.Field.ToString();
            }

            protected internal override string GetMemberName()
            {
                return this.Field.ToString();
            }

            protected internal override IEnumerable<object> GetAllNames(IEnumerable<object> uniqueNames, IEnumerable<object> parentGroupNames)
            {
                if (this.WellKnownGroupNames != null)
                {
                    HashSet<object> names = new HashSet<object>(uniqueNames);
                    names.UnionWith(this.WellKnownGroupNames);
                    return names;
                }
                else
                {
                    return base.GetAllNames(uniqueNames, parentGroupNames);
                }
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new OrdersGroupDescription();
            }

            protected override void CloneCore(Cloneable source)
            {
                OrdersGroupDescription ogd = source as OrdersGroupDescription;
                if (ogd != null)
                {
                    this.Field = ogd.Field;
                }
                base.CloneCore(source);
            }
        }

        private abstract class OrdersAggregateDescription : Cloneable, IAggregateDescription
        {
            public OrderFields Field { get; set; }
            public object GetField(object item)
            {
                return this.Field.GetField(item);
            }

            public string DisplayName
            {
                // TODO: The GroupDescription obviously has GetDisplayName while the IAggregateDescription DisplayName property. We could make the APIs the same and turn the IAggregateDescription into an abstract base class with GetDisplayName method.
                get { return this.Field.ToString(); }
            }

            public TotalFormat TotalFormat { get; set; }

            public string GetMemberName()
            {
                return this.Field.ToString();
            }

            protected override void CloneCore(Cloneable source)
            {
                OrdersAggregateDescription oad = source as OrdersAggregateDescription;
                if (oad != null)
                {
                    this.Field = oad.Field;
                }
            }

            public abstract AggregateValue CreateAggregate();

            IDescriptionBase IDescriptionBase.Clone()
            {
                return this.Clone() as IDescriptionBase;
            }

            public string StringFormat
            {
                get
                {
                    // throw new NotImplementedException();
                    return "0.00лв\\.";
                }
            }
        }

        private class OrdersAggregateDescription<T> : OrdersAggregateDescription
            where T : AggregateValue, new()
        {
            protected override Cloneable CreateInstanceCore()
            {
                return new OrdersAggregateDescription<T>();
            }

            public override AggregateValue CreateAggregate()
            {
                return new T();
            }
        }

        private class OrdersValueProvider : IValueProvider
        {
            public OrdersValueProvider()
            {
                this.RowGroupDescriptions = new List<OrdersGroupDescription>();
                this.ColumnGroupDescriptions = new List<OrdersGroupDescription>();
                this.AggregateDescriptions = new List<OrdersAggregateDescription>();
                this.FilterDescriptions = new List<OrdersFilterDescription>();
            }
            public IReadOnlyList<OrdersGroupDescription> RowGroupDescriptions { get; set; }
            public IReadOnlyList<OrdersGroupDescription> ColumnGroupDescriptions { get; set; }
            public IReadOnlyList<OrdersAggregateDescription> AggregateDescriptions { get; set; }
            public IReadOnlyList<OrdersFilterDescription> FilterDescriptions { get; set; }

            // public IFilter Filter { get; set; } // we now have FilterDescriptions...

            public IGroupFactory GroupFactory { get; set; }

            System.Collections.IEnumerable IValueProvider.GetRowGroupNames(object item)
            {
                for (int level = 0; level < this.RowGroupDescriptions.Count; level++)
                {
                    yield return this.RowGroupDescriptions[level].GroupNameFromItem(item);
                }
            }

            System.Collections.IEnumerable IValueProvider.GetColumnGroupNames(object item)
            {
                for (int level = 0; level < this.ColumnGroupDescriptions.Count; level++)
                {
                    yield return this.ColumnGroupDescriptions[level].GroupNameFromItem(item);
                }
            }

            AggregateValue IValueProvider.CreateAggregateValue(int index)
            {
                return this.AggregateDescriptions[index].CreateAggregate();
            }

            bool IValueProvider.PassesFilter(object[] values)
            {
                int count = this.FilterDescriptions == null ? 0 : this.FilterDescriptions.Count;
                for (int i = 0; i < count; i++)
                {
                    if (!this.FilterDescriptions[i].PassesFilter(values[i]))
                    {
                        return false;
                    }
                }
                return true;
            }

            public object GetAggregateValue(int aggregateDescriptionIndex, object item)
            {
                return this.AggregateDescriptions[aggregateDescriptionIndex].GetField(item);
            }

            public string GetAggregateStringFormat(int aggregateDescriptionIndex)
            {
                return null;
            }

            public object[] GetFilterItems(object fact)
            {
                object[] filterItems;
                if (this.FilterDescriptions == null)
                {
                    filterItems = new object[0];
                }
                else
                {
                    filterItems = new object[this.FilterDescriptions.Count];
                    for (int i = 0; i < this.FilterDescriptions.Count; i++)
                    {
                        filterItems[i] = this.FilterDescriptions[i].GetFilterItem(fact);
                    }
                }
                return filterItems;
            }

            public int GetFiltersCount()
            {
                return this.FilterDescriptions == null ? 0 : this.FilterDescriptions.Count;
            }

            public IReadOnlyList<IAggregateDescription> GetAggregateDescriptions()
            {
                return this.AggregateDescriptions;
            }

            Tuple<GroupComparer, SortOrder> IValueProvider.GetRowGroupNameComparerAndSortOrder(int level)
            {
                IGroupDescription description = (IGroupDescription)this.RowGroupDescriptions[level];
                return Tuple.Create(description.GroupComparer, description.SortOrder);
            }

            Tuple<GroupComparer, SortOrder> IValueProvider.GetColumnGroupNameComparerAndSortOrder(int level)
            {
                IGroupDescription description = (IGroupDescription)this.ColumnGroupDescriptions[level];
                return Tuple.Create(description.GroupComparer, description.SortOrder);
            }

            public IGroupFactory GetGroupFactory()
            {
                return this.GroupFactory;
            }

            public IComparer<object> GetSortComparer()
            {
                return null;
            }
        }

        private void AssertIGroupAreEqual(string displayName, IGroup expected, IGroup actual)
        {
            Assert.IsNotNull(expected, "Expected group in AssertIGroupAreEqual was null. Actual is: {0}", actual);
            Assert.IsNotNull(actual, "Actual group in AssertIGroupAreEqual was null. Expected is: {0}", expected);
            bool equal = GroupTestsHelper.AreGroupsEqual(expected, actual);
            if (!equal)
            {
                Assert.Fail(String.Format("{0}{3}{3}Expected:{3}{1}{3}{3}Actual:{3}{2}", displayName, PrintTree(expected), PrintTree(actual), System.Environment.NewLine));
            }
        }

        private string PrintTree(IGroup group)
        {
            return PrintTree(new ReadOnlyList<IGroup, IGroup>(new List<IGroup>() { group }), String.Empty);
        }

        private string PrintTree(IReadOnlyList<IGroup> iReadOnlyList, string ident)
        {
            string result = String.Empty;
            for (int i = 0; i < iReadOnlyList.Count; i++)
            {
                bool isLast = i == iReadOnlyList.Count - 1;
                IGroup item = iReadOnlyList[i];
                result += ident + (isLast ? "└─● " : "├─● ") + item.Name + System.Environment.NewLine;
                if (item.HasItems && !item.IsBottomLevel)
                {
                    result += PrintTree(item.Items.OfType<IGroup>().ToList(), ident + (isLast ? "  " : "│ "));
                }
            }
            return result;
        }

        private class ListItems : AggregateValue
        {
            private List<object> items;

            public ListItems()
            {
                this.items = new List<object>();
            }

            protected override object GetValueOverride()
            {
                return items;
            }

            protected override void AccumulateOverride(object value)
            {
                this.items.Add(value);
            }

            protected override void MergeOverride(AggregateValue childAggregate)
            {
                ListItems accumulatedOrders = childAggregate as ListItems;
                foreach (var item in accumulatedOrders.items)
                {
                    this.items.Add(item);
                }
            }
        }

        private class ConstValueAggregate : AggregateValue
        {
            public static readonly object Value = new Object();

            protected override object GetValueOverride()
            {
                return Value;
            }

            protected override void AccumulateOverride(object value)
            {
            }

            protected override void MergeOverride(AggregateValue childAggregate)
            {
            }
        }

        private class GroupNameComparer : GroupComparer
        {
            public override int CompareGroups(IAggregateResultProvider results, IGroup x, IGroup y, DataAxis axis)
            {
                if (x.Name is NullValue && y.Name is NullValue)
                {
                    return 0;
                }
                else if (x.Name is NullValue)
                {
                    return -1;
                }
                else if (y.Name is NullValue)
                {
                    return 1;
                }
                return (x.Name as IComparable).CompareTo(y.Name);
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new GroupNameComparer();
            }

            protected override void CloneCore(Cloneable source)
            {
            }
        }

        private class LevelName
        {
            public object Name { get; set; }
            public int Level { get; set; }

            public override bool Equals(object obj)
            {
                LevelName other = obj as LevelName;
                if (other == null)
                {
                    return false;
                }
                return Object.Equals(this.Name, other.Name) && Object.Equals(this.Level, other.Level);
            }

            public override int GetHashCode()
            {
                return this.Name.GetHashCode() * 17 + this.Level;
            }
        }

        private class HierarchicalOrdersGroupDescription : OrdersGroupDescription
        {
            public int Level { get; set; }

            public Action<IEnumerable<object>, IEnumerable<object>> GetAllNamesCallback { get; set; }

            public override object GroupNameFromItem(object item)
            {
                var name = base.GroupNameFromItem(item);
                return new LevelName() { Level = this.Level, Name = name };
            }

            protected internal override IEnumerable<object> GetAllNames(IEnumerable<object> uniqueNames, IEnumerable<object> parentGroupNames)
            {
                if (this.GetAllNamesCallback != null)
                {
                    this.GetAllNamesCallback(uniqueNames, parentGroupNames);
                }

                return base.GetAllNames(uniqueNames, parentGroupNames);
            }
        }

        private class GroupNameFilter : SingleGroupFilter
        {
            private HashSet<object> allowedGroupNames;

            public GroupNameFilter(IEnumerable<object> allowedGroupNames)
            {
                this.allowedGroupNames = new HashSet<object>(allowedGroupNames);
            }

            internal override bool Filter(IGroup group, IAggregateResultProvider results, DataAxis axis)
            {
                return allowedGroupNames.Contains(group.Name);
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new GroupNameFilter(this.allowedGroupNames);
            }

            protected override void CloneCore(Cloneable source)
            {
            }
        }

        private class GroupValueFilter : SingleGroupFilter
        {
            internal override bool Filter(IGroup group, IAggregateResultProvider results, DataAxis axis)
            {
                Coordinate coordinate;
                if (axis == DataAxis.Rows)
                {
                    coordinate = new Coordinate(group, results.Root.ColumnGroup);
                }
                else
                {
                    coordinate = new Coordinate(results.Root.RowGroup, group);
                }
                var result = (double)results.GetAggregateResult(0, coordinate).GetValue();
                return result > 1500;
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new GroupValueFilter();
            }

            protected override void CloneCore(Cloneable source)
            {
            }
        }

        private class TopGroupFilter : SiblingGroupsFilter
        {
            protected internal override ICollection<IGroup> Filter(IReadOnlyList<object> groups, IAggregateResultProvider results, DataAxis axis, int level)
            {
                IGroup group;
                if (axis == DataAxis.Rows)
                {
                    group = groups.Aggregate(groups.OfType<IGroup>().First(), (g, c) => (double)results.GetAggregateResult(0, new Coordinate(g, results.Root.ColumnGroup)).GetValue() > (double)results.GetAggregateResult(0, new Coordinate(c as IGroup, results.Root.ColumnGroup)).GetValue() ? g : c as IGroup);
                }
                else
                {
                    group = groups.Aggregate(groups.OfType<IGroup>().First(), (g, c) => (double)results.GetAggregateResult(0, new Coordinate(results.Root.RowGroup, g)).GetValue() > (double)results.GetAggregateResult(0, new Coordinate(results.Root.RowGroup, c as IGroup)).GetValue() ? g : c as IGroup);
                }
                return new List<IGroup>() { group };
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new TopGroupFilter();
            }

            protected override void CloneCore(Cloneable source)
            {
            }
        }

        private class RankGroupTotals : SiblingTotalsFormat
        {
            public RankGroupTotals()
            {
                this.Variation = RunningTotalSubGroupVariation.ParentAndSelfNames;
            }

            private class DoubleTotalValueComparer : IComparer<TotalValue>
            {
                public int Compare(TotalValue x, TotalValue y)
                {
                    if (x.Value == null && y.Value == null)
                    {
                        return 0;
                    }
                    if (x.Value == null)
                    {
                        return -1;
                    }
                    else if (y.Value == null)
                    {
                        return 1;
                    }

                    return ((double)x.Value.GetValue()).CompareTo((double)y.Value.GetValue());
                }
            }

            public RunningTotalSubGroupVariation Variation { get; set; }

            protected internal override RunningTotalSubGroupVariation SubVariation()
            {
                return this.Variation;
            }

            protected internal override void FormatTotals(IReadOnlyList<TotalValue> valueFormatters, IAggregateResultProvider results)
            {
                var sortedlist = valueFormatters.ToList();
                sortedlist.Sort(new DoubleTotalValueComparer());
                for (int i = 0; i < sortedlist.Count; i++)
                {
                    sortedlist[i].FormattedValue = new ConstantValueAggregate(i);
                }
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new RankGroupTotals();
            }
        }

        private class PercentOfGrandTotal : SingleTotalFormat
        {
            protected internal override AggregateValue FormatValue(Coordinate groups, IAggregateResultProvider results, int aggregateIndex)
            {
                var grandTotal = (double)results.GetAggregateResult(0, results.Root).GetValue();
                var currentValue = (double)results.GetAggregateResult(0, groups).GetValue();
                return new ConstantValueAggregate(currentValue / grandTotal);
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new PercentOfGrandTotal();
            }

            protected override void CloneCore(Cloneable source)
            {
            }

            //protected internal override FormatType FormatType
            //{
            //    get { throw new NotImplementedException(); }
            //}
        }

        private class ExceptionSingleTotalFormat : SingleTotalFormat
        {
            protected internal override AggregateValue FormatValue(Coordinate groups, IAggregateResultProvider results, int aggregateIndex)
            {
                throw new Exception("FormatValue throws exception");
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new ExceptionSingleTotalFormat();
            }

            protected override void CloneCore(Cloneable source)
            {
            }
        }

        private class ExceptionSiblingTotalFormat : SiblingTotalsFormat
        {
            protected internal override void FormatTotals(IReadOnlyList<TotalValue> valueFormatters, IAggregateResultProvider results)
            {
                throw new Exception("FormatValue throws exception");
            }

            protected override Cloneable CreateInstanceCore()
            {
                return new ExceptionSiblingTotalFormat();
            }
        }

        private static void SetupEngineForParallel(bool isRunningSlow, out ParallelDataEngine engine, out ParallelState state)
        {
            engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer(), IsSlowRunning = isRunningSlow });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer(), IsSlowRunning = isRunningSlow });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer(), IsSlowRunning = isRunningSlow });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Net });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,

                MaxDegreeOfParallelism = 4,
                TaskScheduler = TaskScheduler.Default,
            };
        }

        private void CheckParallelResults(ParallelDataEngine engine)
        {
            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Direct mail")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Magazine")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Newspaper")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
                new TestGroup("Printer stand")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root.");
            Assert.IsNotNull(engine.Root.RowGroup, "Engine RowGroup was null. Should have been GrandTotal");
            Assert.IsNotNull(engine.Root.ColumnGroup, "Engine ColumnGroup was null. Should have been GrandTotal");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            List<List<List<double>>> expectedResults = new List<List<List<double>>>()
            {
                new List<List<double>>()
                {
                    new List<double>() { 7851, 73736.32 }, new List<double>() { 1439, 10469.3 }, new List<double>() { 1708, 23985.88 }, new List<double>() { 3360, 24354.92 }, new List<double>() { 1344, 14926.22 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1814, 17193.27 }, new List<double>() { 322, 2327.22 }, new List<double>() { 402, 5653.94 }, new List<double>() { 752, 5461.36 }, new List<double>() { 338, 3750.75 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 935, 9552.24 }, new List<double>() { 154, 1196.58 }, new List<double>() { 220, 3293.4 }, new List<double>() { 385, 2991.45 }, new List<double>() { 176, 2070.81 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 879, 7641.03 }, new List<double>() { 168, 1130.64 }, new List<double>() { 182, 2360.54 }, new List<double>() { 367, 2469.91 }, new List<double>() { 162, 1679.94 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 3416, 31977.38 }, new List<double>() { 555, 4050.94 }, new List<double>() { 719, 10234.55 }, new List<double>() { 1596, 11579.44 }, new List<double>() { 546, 6112.45 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1793, 17448.03 }, new List<double>() { 341, 2610.72 }, new List<double>() { 352, 5224.56 }, new List<double>() { 836, 6464.64 }, new List<double>() { 264, 3148.11 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1623, 14529.35 }, new List<double>() { 214, 1440.22 }, new List<double>() { 367, 5009.99 }, new List<double>() { 760, 5114.8 }, new List<double>() { 282, 2964.34 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 2621, 24565.67 }, new List<double>() { 562, 4091.14 }, new List<double>() { 587, 8097.39 }, new List<double>() { 1012, 7314.12 }, new List<double>() { 460, 5063.02 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1221, 12037.19 }, new List<double>() { 297, 2307.69 }, new List<double>() { 242, 3622.74 }, new List<double>() { 484, 3760.68 }, new List<double>() { 198, 2346.08 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1400, 12528.48 }, new List<double>() { 265, 1783.45 }, new List<double>() { 345, 4474.65 }, new List<double>() { 528, 3553.44 }, new List<double>() { 262, 2716.94 }
                }
            };

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int a = 0; a < 2; a++)
                    {
                        IGroup rowGroup = rowGroups[x];
                        IGroup columnGroup = columnGroups[y];
                        var actualResult = (double)engine.GetAggregateResult(a, rowGroup, columnGroup).GetValue();
                        var expectedResult = expectedResults[x][y][a];
                        Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                        Assert.IsTrue(Math.Abs(actualResult - expectedResult) < 0.1, "Results for: {0}, {1} aggregate index {2}{5}Expected: {3}{5}Actual: {4}", x, y, a, expectedResult, actualResult, System.Environment.NewLine);
                    }
                }
            }
        }

        private static IList<Order> GenerateRandomOrdersSource()
        {
            IList<Order> source = new List<Order>();

            List<string> Advertisements = new List<string>() { "Advertisement1", "Advertisement2" };
            List<string> Promotions = new List<string>() { "Promotion1", "Promotion2" };
            List<string> Products = new List<string>() { "Product1", "Product2" };
            List<DateTime> Dates = new List<DateTime>() { new DateTime(2012, 1, 1), new DateTime(2012, 2, 1) };

            Random pseudoRandom = new Random(123);

            for (int i = 0; i < 16; i++)
            {
                source.Add(new Order()
                {
                    Advertisement = Advertisements[(i & 1) / 1],
                    Promotion = Promotions[(i & 2) / 2],
                    Product = Products[(i & 4) / 4],
                    Date = Dates[(i & 8) / 8],
                    Quantity = i,
                    Net = Math.Floor(pseudoRandom.NextDouble() * 10000) / 100
                });
            }

            return source;
        }

        #endregion

        private void GetEngineWithResults(out ParallelDataEngine engine, out ParallelState state)
        {
            engine = new ParallelDataEngine();
            var source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },
                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,
            };

            engine.RebuildCube(state);
        }

        [TestMethod]
        public void ParallelDataEngine_RaiseCompleted_With_Status_InProgress_In_Rebuild()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            DataEngineStatus status = DataEngineStatus.Faulted;

            bool firstRun = false;
            engine.Completed += (o, e) =>
            {
                status = e.Status;
                if (!firstRun)
                {
                    firstRun = true;
                    Assert.AreEqual(DataEngineStatus.InProgress, status);
                }
                else
                {
                    Assert.AreEqual(DataEngineStatus.Completed, status);
                }
            };

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            OrdersSource source = new OrdersSource();
            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);
        }

        [TestMethod]
        public void Rebuild_While_InProgress_Cause_Previous_Operation_To_Be_Ignored()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            DataEngineStatus status = DataEngineStatus.Faulted;

            int fired = 0;
            engine.Completed += (o, e) =>
            {
                status = e.Status;
                if (fired < 2)
                {
                    Assert.AreEqual(DataEngineStatus.InProgress, status);
                }
                else
                {
                    Assert.AreEqual(DataEngineStatus.Completed, status);
                }

                fired++;
                Assert.IsTrue(fired < 4);
            };

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            OrdersSource source = new OrdersSource();
            var newSource = source.Take(3);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,
                TaskScheduler = TaskScheduler.Default,
            };

            ParallelState state2 = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(newSource),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,
                TaskScheduler = TaskScheduler.Default,
            };

            engine.RebuildCubeParallel(state);
            engine.RebuildCubeParallel(state2);
            engine.WaitForParallel();

            Assert.AreEqual(3, engine.Root.RowGroup.Items.Count);
        }

        [TestMethod]
        public void ParallelDataEngine_GroupingOnly()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Direct mail")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Magazine")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Newspaper")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
                new TestGroup("Printer stand")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);
        }

        [TestMethod]
        public void ParallelDataEngine_BasicGrouping_SummariesFromSingleBottomLevelItems()
        {
            IList<Order> source = GenerateRandomOrdersSource();

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Date, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<ListItems>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Advertisement1")
                {
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2")
                },
                new TestGroup("Advertisement2")
                {
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2")
                },
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Product1")
                {
                    new TestGroup(new DateTime(2012, 1, 1)),
                    new TestGroup(new DateTime(2012, 2, 1))
                },
                new TestGroup("Product2")
                {
                    new TestGroup(new DateTime(2012, 1, 1)),
                    new TestGroup(new DateTime(2012, 2, 1))
                }
            };

            engine.RebuildCube(state);

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            // Bottom level results:
            // _  _  _  _  _  _  _
            // _  _  _  _  _  _  _
            // _  _  0  2  _  1  3
            // _  _  8 10  _  9 11
            // _  _  _  _  _  _  _
            // _  _  4  6  _  5  7
            // _  _ 12 14  _ 13 15

            List<List<HashSet<object>>> expectedResults = new List<List<HashSet<object>>>()
            {
                new List<HashSet<object>>
                {
                    new HashSet<object>(new List<object>() {0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15 }),
                    new HashSet<object>(new List<object>() {0, 1, 2, 3, 8, 9, 10, 11 }),
                    new HashSet<object>(new List<object>() {0, 1, 2, 3 }),
                    new HashSet<object>(new List<object>() {8, 9, 10, 11 }),
                    new HashSet<object>(new List<object>() {4, 5, 6, 7, 12, 13, 14, 15 }),
                    new HashSet<object>(new List<object>() {4, 5, 6, 7 }),
                    new HashSet<object>(new List<object>() {12, 13, 14, 15 }),
                },
                new List<HashSet<object>>
                {
                    new HashSet<object>(new List<object>() { 0, 2, 4, 6, 8, 10, 12, 14 } ),
                    new HashSet<object>(new List<object>() { 0, 2, 8, 10 }),
                    new HashSet<object>(new List<object>() { 0, 2 }),
                    new HashSet<object>(new List<object>() { 8, 10 }),
                    new HashSet<object>(new List<object>() { 4, 6, 12, 14 }),
                    new HashSet<object>(new List<object>() { 4, 6, }),
                    new HashSet<object>(new List<object>() { 12, 14 })
                },
                new List<HashSet<object>>
                {
                    new HashSet<object>(new List<object>() { 0, 4, 8, 12 } ),
                    new HashSet<object>(new List<object>() { 0, 8 } ),
                    new HashSet<object>(new List<object>() { 0 } ),
                    new HashSet<object>(new List<object>() { 8 } ),
                    new HashSet<object>(new List<object>() { 4, 12 } ),
                    new HashSet<object>(new List<object>() { 4 } ),
                    new HashSet<object>(new List<object>() { 12 } ),
                },
                new List<HashSet<object>>
                {
                    new HashSet<object>(new List<object>() { 2, 6, 10, 14 } ),
                    new HashSet<object>(new List<object>() { 2, 10 } ),
                    new HashSet<object>(new List<object>() { 2 } ),
                    new HashSet<object>(new List<object>() { 10 } ),
                    new HashSet<object>(new List<object>() { 6, 14 } ),
                    new HashSet<object>(new List<object>() { 6 } ),
                    new HashSet<object>(new List<object>() { 14 } ),
                },
                new List<HashSet<object>>
                {
                    new HashSet<object>(new List<object>() { 1, 3, 5, 7, 9, 11, 13, 15 }),
                    new HashSet<object>(new List<object>() { 1, 3, 9, 11 }),
                    new HashSet<object>(new List<object>() { 1, 3 }),
                    new HashSet<object>(new List<object>() { 9, 11 }),
                    new HashSet<object>(new List<object>() { 5, 7, 13, 15 }),
                    new HashSet<object>(new List<object>() { 5, 7 }),
                    new HashSet<object>(new List<object>() { 13, 15 })
                },
                new List<HashSet<object>>
                {
                    new HashSet<object>(new List<object>() { 1, 5, 9, 13 }),
                    new HashSet<object>(new List<object>() { 1, 9 }),
                    new HashSet<object>(new List<object>() { 1 }),
                    new HashSet<object>(new List<object>() { 9 }),
                    new HashSet<object>(new List<object>() { 5, 13 }),
                    new HashSet<object>(new List<object>() { 5 }),
                    new HashSet<object>(new List<object>() { 13 })
                },
                new List<HashSet<object>>
                {
                    new HashSet<object>(new List<object>() { 3, 7, 11, 15 }),
                    new HashSet<object>(new List<object>() { 3, 11 }),
                    new HashSet<object>(new List<object>() { 3 }),
                    new HashSet<object>(new List<object>() { 11 }),
                    new HashSet<object>(new List<object>() { 7, 15 }),
                    new HashSet<object>(new List<object>() { 7 }),
                    new HashSet<object>(new List<object>() { 15 })
                }
            };

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = engine.GetAggregateResult(0, rowGroup, columnGroup).GetValue() as IEnumerable<object>;
                    var expectedResult = expectedResults[x][y];
                    Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                    Assert.IsTrue(expectedResult.SetEquals(actualResult), "Results for: {0}, {1}{4}Expected: {2}{4}Actual: {3}", x, y, string.Join(", ", expectedResult), string.Join(", ", actualResult), System.Environment.NewLine);
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_GroupingSortingAndAggregates()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Direct mail")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Magazine")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Newspaper")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
                new TestGroup("Printer stand")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            List<List<double>> expectedResults = new List<List<double>>()
            {
                new List<double>() { 7851, 1439, 1708, 3360, 1344 },
                new List<double>() { 1814, 322, 402, 752, 338 },
                new List<double>() { 935, 154, 220, 385, 176 },
                new List<double>() { 879, 168, 182, 367, 162 },
                new List<double>() { 3416, 555, 719, 1596, 546 },
                new List<double>() { 1793, 341, 352, 836, 264 },
                new List<double>() { 1623, 214, 367, 760, 282 },
                new List<double>() { 2621, 562, 587, 1012, 460 },
                new List<double>() { 1221, 297, 242, 484, 198 },
                new List<double>() { 1400, 265, 345, 528, 262 },
            };

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = (double)engine.GetAggregateResult(0, rowGroup, columnGroup).GetValue();
                    var expectedResult = expectedResults[x][y];
                    Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                    Assert.IsTrue(Math.Abs(actualResult - expectedResult) < 0.1, "Results for: {0}, {1}{4}Expected: {2}{4}Actual: {3}", x, y, string.Join(", ", expectedResult), string.Join(", ", actualResult), System.Environment.NewLine);
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_GroupingSortingAndAggregates_Parallel()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,
                TaskScheduler = TaskScheduler.Default,
                MaxDegreeOfParallelism = 4
            };

            engine.RebuildCubeParallel(state);
            engine.WaitForParallel();

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Direct mail")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Magazine")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Newspaper")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
                new TestGroup("Printer stand")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            List<List<double>> expectedResults = new List<List<double>>()
            {
                new List<double>() { 7851, 1439, 1708, 3360, 1344 },
                new List<double>() { 1814, 322, 402, 752, 338 },
                new List<double>() { 935, 154, 220, 385, 176 },
                new List<double>() { 879, 168, 182, 367, 162 },
                new List<double>() { 3416, 555, 719, 1596, 546 },
                new List<double>() { 1793, 341, 352, 836, 264 },
                new List<double>() { 1623, 214, 367, 760, 282 },
                new List<double>() { 2621, 562, 587, 1012, 460 },
                new List<double>() { 1221, 297, 242, 484, 198 },
                new List<double>() { 1400, 265, 345, 528, 262 },
            };

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = (double)engine.GetAggregateResult(0, rowGroup, columnGroup).GetValue();
                    var expectedResult = expectedResults[x][y];
                    Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                    Assert.IsTrue(Math.Abs(actualResult - expectedResult) < 0.1, "Results for: {0}, {1}{4}Expected: {2}{4}Actual: {3}", x, y, string.Join(", ", expectedResult), string.Join(", ", actualResult), System.Environment.NewLine);
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_EmptyGrouping()
        {
            IList<Order> source = new OrdersSource();
            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            //Assert.IsNull(engine.Root, "Engine grouping Root expected to be null.");

            Assert.IsNotNull(engine.Root);
            Assert.IsNotNull(engine.Root.ColumnGroup);
            Assert.IsNotNull(engine.Root.RowGroup);

            Assert.IsFalse(engine.Root.ColumnGroup.HasItems);
            Assert.IsTrue(engine.Root.RowGroup.HasItems);
        }

        [TestMethod]
        public void ParallelDataEngine_EmptyGroupingWithAggregate()
        {
            IList<Order> source = new OrdersSource();
            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            Group expectedRowGroup = new Group("Grand Total");
            expectedRowGroup.SetItems(source.OfType<object>().ToList());
            IGroup expectedColumnGroup = new Group("Grand Total");

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            var result = engine.GetAggregateResult(0, engine.Root);
            Assert.IsNotNull(result, "Expected an AggregateValue for the engine's Root");
            var doubleValue = (double)result.GetValue();
            double expectedDoubleValue = 7851;
            Assert.IsTrue(Math.Abs(doubleValue - expectedDoubleValue) < 0.1, "Actual root result: {0}{2}Expected: {1}", doubleValue, expectedDoubleValue, System.Environment.NewLine);
        }

        [TestMethod]
        public void ParallelDataEngine_GroupingWithMultyAggregates()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Net });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Direct mail")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Magazine")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Newspaper")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
                new TestGroup("Printer stand")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            List<List<List<double>>> expectedResults = new List<List<List<double>>>()
            {
                new List<List<double>>()
                {
                    new List<double>() { 7851, 73736.32 }, new List<double>() { 1439, 10469.3 }, new List<double>() { 1708, 23985.88 }, new List<double>() { 3360, 24354.92 }, new List<double>() { 1344, 14926.22 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1814, 17193.27 }, new List<double>() { 322, 2327.22 }, new List<double>() { 402, 5653.94 }, new List<double>() { 752, 5461.36 }, new List<double>() { 338, 3750.75 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 935, 9552.24 }, new List<double>() { 154, 1196.58 }, new List<double>() { 220, 3293.4 }, new List<double>() { 385, 2991.45 }, new List<double>() { 176, 2070.81 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 879, 7641.03 }, new List<double>() { 168, 1130.64 }, new List<double>() { 182, 2360.54 }, new List<double>() { 367, 2469.91 }, new List<double>() { 162, 1679.94 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 3416, 31977.38 }, new List<double>() { 555, 4050.94 }, new List<double>() { 719, 10234.55 }, new List<double>() { 1596, 11579.44 }, new List<double>() { 546, 6112.45 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1793, 17448.03 }, new List<double>() { 341, 2610.72 }, new List<double>() { 352, 5224.56 }, new List<double>() { 836, 6464.64 }, new List<double>() { 264, 3148.11 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1623, 14529.35 }, new List<double>() { 214, 1440.22 }, new List<double>() { 367, 5009.99 }, new List<double>() { 760, 5114.8 }, new List<double>() { 282, 2964.34 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 2621, 24565.67 }, new List<double>() { 562, 4091.14 }, new List<double>() { 587, 8097.39 }, new List<double>() { 1012, 7314.12 }, new List<double>() { 460, 5063.02 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1221, 12037.19 }, new List<double>() { 297, 2307.69 }, new List<double>() { 242, 3622.74 }, new List<double>() { 484, 3760.68 }, new List<double>() { 198, 2346.08 }
                },
                new List<List<double>>()
                {
                    new List<double>() { 1400, 12528.48 }, new List<double>() { 265, 1783.45 }, new List<double>() { 345, 4474.65 }, new List<double>() { 528, 3553.44 }, new List<double>() { 262, 2716.94 }
                }
            };

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            for (int x = 0; x < 10; x++)
            {
                for (int y = 0; y < 5; y++)
                {
                    for (int a = 0; a < 2; a++)
                    {
                        IGroup rowGroup = rowGroups[x];
                        IGroup columnGroup = columnGroups[y];
                        var actualResult = (double)engine.GetAggregateResult(a, rowGroup, columnGroup).GetValue();
                        var expectedResult = expectedResults[x][y][a];
                        Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                        Assert.IsTrue(Math.Abs(actualResult - expectedResult) < 0.1, "Results for: {0}, {1} aggregate index {2}{5}Expected: {3}{5}Actual: {4}", x, y, a, expectedResult, actualResult, System.Environment.NewLine);
                    }
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_ShowEmptyGroups_CustomGroups_FromWellKnownGroupNames()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer(), ShowGroupsWithNoData = true, WellKnownGroupNames = new List<object>() { "CG1", "CG2" } });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer(), ShowGroupsWithNoData = true, WellKnownGroupNames = new List<object>() { "CG3", "CG4" } });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Net });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("CG1"),
                new TestGroup("CG2"),
                new TestGroup("Direct mail")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Magazine")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Newspaper")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("CG3"),
                new TestGroup("CG4"),
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
                new TestGroup("Printer stand")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);
        }

        [TestMethod]
        public void ParallelDataEngine_NullGroupKeys()
        {
            IList<Order> source = new List<Order>();

            List<object> Advertisements = new List<object>() { NullValue.Instance, "Advertisement2" };
            List<string> Promotions = new List<string>() { "Promotion1", "Promotion2" };
            List<string> Products = new List<string>() { "Product1", null };
            List<DateTime> Dates = new List<DateTime>() { new DateTime(2012, 1, 1), new DateTime(2012, 2, 1) };

            for (int i = 0; i < 16; i++)
            {
                source.Add(new Order()
                {
                    Advertisement = Advertisements[(i & 1) / 1],
                    Promotion = Promotions[(i & 2) / 2],
                    Product = Products[(i & 4) / 4],
                    Date = Dates[(i & 8) / 8],
                    Quantity = i
                });
            }

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Date, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<ListItems>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup(NullValue.Instance)
                {
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2")
                },
                new TestGroup("Advertisement2")
                {
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2")
                },
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup(NullValue.Instance)
                {
                    new TestGroup(new DateTime(2012, 1, 1)),
                    new TestGroup(new DateTime(2012, 2, 1))
                },
                new TestGroup("Product1")
                {
                    new TestGroup(new DateTime(2012, 1, 1)),
                    new TestGroup(new DateTime(2012, 2, 1))
                }
            };

            engine.RebuildCube(state);

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);
        }

        [TestMethod]
        public void ParallelDataEngine_ShowEmptyGroups_CheckParentNamesFromSameMember()
        {
            IList<Order> source = new List<Order>();
            source.Add(new Order() { Product = "Product", Promotion = "Promotion" });

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new HierarchicalOrdersGroupDescription() { Field = OrderFields.Product, Level = 1 });
            rowList.Add(new HierarchicalOrdersGroupDescription() { Field = OrderFields.Promotion, Level = 2 });

            int callbackCalls3 = 0;
            rowList.Add(new HierarchicalOrdersGroupDescription()
            {
                Field = OrderFields.Product,
                Level = 3,
                ShowGroupsWithNoData = true,
                GetAllNamesCallback = (uniqueNames, parentGroupNames) =>
                {
                    callbackCalls3++;
                    // TODO: Probably this should be done in two different methods as the omited parentGroupNames somehow is hard to test and probably would cause further misunderstandings....
                    if (callbackCalls3 == 1)
                    {
                        IList<object> expectedParentNames = new List<object>() { new LevelName() { Level = 1, Name = "Product" } };
                        Assert.IsTrue(expectedParentNames.ItemsEqual(parentGroupNames), "Unexpected parent names.");
                    }
                    else if (callbackCalls3 == 2)
                    {
                        Assert.IsFalse(parentGroupNames.Any(), "The second time GetAllNames is called should be with empty parents to collect all group names.");
                    }
                    else
                    {
                        Assert.Fail("The GetAllNames should have been called just twice - once to generate the empty groups and the second time to get all group names.");
                    }
                }
            });

            int callbackCalls4 = 0;
            rowList.Add(new HierarchicalOrdersGroupDescription()
            {
                Field = OrderFields.Promotion,
                Level = 4,
                ShowGroupsWithNoData = true,
                GetAllNamesCallback = (uniqueNames, parentGroupNames) =>
                {
                    callbackCalls4++;
                    // TODO: Probably this should be done in two different methods as the omited parentGroupNames somehow is hard to test and probably would cause further misunderstandings....
                    if (callbackCalls4 == 1)
                    {
                        IList<object> expectedParentNames = new List<object>() { new LevelName() { Level = 2, Name = "Promotion" } };
                        Assert.IsTrue(expectedParentNames.ItemsEqual(parentGroupNames), "Unexpected parent names.");
                    }
                    else if (callbackCalls4 == 2)
                    {
                        Assert.IsFalse(parentGroupNames.Any(), "The second time GetAllNames is called should be with empty parents to collect all group names.");
                    }
                    else
                    {
                        Assert.Fail("The GetAllNames should have been called just twice - once to generate the empty groups and the second time to get all group names.");
                    }
                }
            });

            int callbackCalls5 = 0;
            rowList.Add(new HierarchicalOrdersGroupDescription()
            {
                Field = OrderFields.Product,
                Level = 5,
                ShowGroupsWithNoData = true,
                GetAllNamesCallback = (uniqueNames, parentGroupNames) =>
                {
                    callbackCalls5++;
                    // TODO: Probably this should be done in two different methods as the omited parentGroupNames somehow is hard to test and probably would cause further misunderstandings....
                    if (callbackCalls5 == 1)
                    {
                        IList<object> expectedParentNames = new List<object>() { new LevelName() { Level = 3, Name = "Product" }, new LevelName() { Level = 1, Name = "Product" } };
                        Assert.IsTrue(expectedParentNames.ItemsEqual(parentGroupNames), "Unexpected parent names.");
                    }
                    else if (callbackCalls5 == 2)
                    {
                        Assert.IsFalse(parentGroupNames.Any(), "The second time GetAllNames is called should be with empty parents to collect all group names.");
                    }
                    else
                    {
                        Assert.Fail("The GetAllNames should have been called just twice - once to generate the empty groups and the second time to get all group names.");
                    }
                }
            });

            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup(new LevelName() { Name = "Product", Level = 1 })
                {
                    new TestGroup(new LevelName() { Name = "Promotion", Level = 2 })
                    {
                        new TestGroup(new LevelName() { Name = "Product", Level = 3 })
                        {
                            new TestGroup(new LevelName() { Name = "Promotion", Level = 4 })
                            {
                                new TestGroup(new LevelName() { Name = "Product", Level = 5 })
                            }
                        }
                    }
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total");

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);
        }
        
        [TestMethod]
        public void ParallelDataEngine_AggregatesErrors_ExeptionOnGet()
        {
            IList<Order> source = new List<Order>();

            source.Add(new Order() { Promotion = "Promotion1", Advertisement = "Advertisement1", Quantity = 1 });
            source.Add(new Order() { Promotion = "Promotion2", Advertisement = "Advertisement1", Quantity = 2, InvalidQuantity = true });
            source.Add(new Order() { Promotion = "Promotion1", Advertisement = "Advertisement2", Quantity = 3 });
            source.Add(new Order() { Promotion = "Promotion2", Advertisement = "Advertisement2", Quantity = 4 });

            source.Add(new Order() { Promotion = "Promotion1", Advertisement = "Advertisement1", Quantity = 5 });
            source.Add(new Order() { Promotion = "Promotion2", Advertisement = "Advertisement1", Quantity = 6 });
            source.Add(new Order() { Promotion = "Promotion1", Advertisement = "Advertisement2", Quantity = 7, InvalidQuantity = true });
            source.Add(new Order() { Promotion = "Promotion2", Advertisement = "Advertisement2", Quantity = 8 });

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<ConstValueAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,
            };

            engine.RebuildCube(state);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            List<List<object>> expectedResults = new List<List<object>>()
            {
                new List<object>() { ConstValueAggregate.Error, ConstValueAggregate.Error, ConstValueAggregate.Error },
                new List<object>() { ConstValueAggregate.Error, ConstValueAggregate.Value, ConstValueAggregate.Error },
                new List<object>() { ConstValueAggregate.Error, ConstValueAggregate.Error, ConstValueAggregate.Value },
            };

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = engine.GetAggregateResult(0, rowGroup, columnGroup).GetValue();
                    var expectedResult = expectedResults[x][y];
                    Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                    Assert.IsTrue(Object.Equals(actualResult, expectedResult), "Results for: {0}, {1}{4}Expected: {2}{4}Actual: {3}", x, y, expectedResult, actualResult, System.Environment.NewLine);
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_GroupFiltering_NameFilter()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer(), GroupFilter = new GroupNameFilter(new List<object>() { "Direct mail", "Magazine" }) });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer(), GroupFilter = new GroupNameFilter(Enumerable.Empty<object>()) });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer(), GroupFilter = new GroupNameFilter(new List<object>() { "Copy holder", "Glare filter" }) });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                // All at level 2 should be filtered so their parents at level 1 should be removed too...
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                // 2 filtered
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);
        }

        [TestMethod]
        public void ParallelDataEngine_GroupFiltering_ValueFilter()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer(), GroupFilter = new GroupValueFilter() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer(), GroupFilter = new GroupValueFilter() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Magazine")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            List<List<double>> expectedResults = new List<List<double>>()
            {
                new List<double>() { 2315, 719, 1596 },
                new List<double>() { 2315, 719,  1596 },
                new List<double>() { 1188, 352,  836  },
                new List<double>() { 1127, 367,  760  },
            };

            for (int x = 0; x < 4; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = (double)engine.GetAggregateResult(0, rowGroup, columnGroup).GetValue();
                    var expectedResult = expectedResults[x][y];
                    Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                    Assert.IsTrue(Math.Abs(actualResult - expectedResult) < 0.1, "Results for: {0}, {1}{4}Expected: {2}{4}Actual: {3}", x, y, string.Join(", ", expectedResult), string.Join(", ", actualResult), System.Environment.NewLine);
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_GroupFiltering_TopGroup()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer(), GroupFilter = new TopGroupFilter() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer(), GroupFilter = new TopGroupFilter() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Direct mail")
                {
                    new TestGroup("1 Free with 10"),
                },
                new TestGroup("Magazine")
                {
                    new TestGroup("1 Free with 10"),
                },
                new TestGroup("Newspaper")
                {
                    new TestGroup("Extra Discount")
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Mouse pad")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            List<List<double>> expectedResults = new List<List<double>>()
            {
                new List<double>() { 1749, 1749 },
                new List<double>() { 385,  385  },
                new List<double>() { 385,  385  },
                new List<double>() { 836,  836  },
                new List<double>() { 836,  836  },
                new List<double>() { 528,  528  },
                new List<double>() { 528,  528  },
            };

            for (int x = 0; x < 6; x++)
            {
                for (int y = 0; y < 2; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = (double)engine.GetAggregateResult(0, rowGroup, columnGroup).GetValue();
                    var expectedResult = expectedResults[x][y];
                    Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                    Assert.IsTrue(Math.Abs(actualResult - expectedResult) < 0.1, "Results for: {0}, {1}{4}Expected: {2}{4}Actual: {3}", x, y, string.Join(", ", expectedResult), string.Join(", ", actualResult), System.Environment.NewLine);
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_GroupFiltering_CorrectlyRemovesChildren_CorrectAggregateValues()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer(), GroupFilter = new GroupNameFilter(new List<object>() { "Direct mail", "Magazine" }) });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Direct mail")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                },
                new TestGroup("Magazine")
                {
                    new TestGroup("1 Free with 10"),
                    new TestGroup("Extra Discount")
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
                new TestGroup("Printer stand"),
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            // TODO: assert GrandTotal updated correctly:
            var grandtotal = (double)engine.GetAggregateResult(0, engine.Root).GetValue();
            Assert.IsTrue(Math.Abs(grandtotal - 5230.0) < 0.001, "Grand Total expected close to 5230.0. Actual: {0}.", grandtotal);
        }

        [TestMethod]
        public void ParallelDataEngine_TotalsFormat_RunningTotals_RankValues_ParentAndSelfNames()
        {
            IList<Order> source = new List<Order>();

            List<string> Advertisements = new List<string>() { "Advertisement1", "Advertisement2", "Advertisement3", "Advertisement4" };
            List<string> Promotions = new List<string>() { "Promotion1", "Promotion2", "Promotion3", "Promotion4" };
            List<string> Products = new List<string>() { "Product1", "Product2", "Product3", "Product4" };

            for (int i = 0; i < 64; i++)
            {
                source.Add(new Order()
                {
                    Advertisement = Advertisements[(i & 3) / 1],
                    Promotion = Promotions[(i & 12) / 4],
                    Product = Products[(i & 48) / 16],
                    Quantity = i
                });
            }

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity, TotalFormat = new RankGroupTotals() { Axis = DataAxis.Rows, Level = 1 } });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            #region expectedRowGroup
            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Advertisement1")
                {
                    new TestGroup("Promotion1")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion2")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion3")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion4")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    }
                },
                new TestGroup("Advertisement2")
                {
                    new TestGroup("Promotion1")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion2")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion3")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion4")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    }
                },
                new TestGroup("Advertisement3")
                {
                    new TestGroup("Promotion1")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion2")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion3")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion4")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    }
                },
                new TestGroup("Advertisement4")
                {
                    new TestGroup("Promotion1")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion2")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion3")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion4")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    }
                }
            };
            #endregion

            IGroup expectedColumnGroup = new TestGroup("Grand Total");

            engine.RebuildCube(state);

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            var expectedResults = new List<object>() { 
                null,
                null, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3,
                null, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3,
                null, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3,
                null, 0, 0, 0, 0, 0, 1, 1, 1, 1, 1, 2, 2, 2, 2, 2, 3, 3, 3, 3, 3,
            };

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            for (int x = 0; x < 85; x++)
            {
                IGroup rowGroup = rowGroups[x];
                IGroup columnGroup = engine.Root.ColumnGroup;
                var aggregate = engine.GetAggregateResult(0, rowGroup, columnGroup);
                var actualResult = aggregate != null ? aggregate.GetValue() : null;
                var expectedResult = expectedResults[x];
                Assert.IsTrue(Object.Equals(actualResult, expectedResult), "Results for: {0}, {1} aggregate index {2}{5}Expected: {3}{5}Actual: {4}", x, 0, 0, expectedResult, actualResult, System.Environment.NewLine);
            }
        }

        [TestMethod]
        public void ParallelDataEngine_TotalsFormat_RunningTotals_RankValues_GroupDescriptionAndName()
        {
            IList<Order> source = new List<Order>();

            List<string> Advertisements = new List<string>() { "Advertisement1", "Advertisement2", "Advertisement3", "Advertisement4" };
            List<string> Promotions = new List<string>() { "Promotion1", "Promotion2", "Promotion3", "Promotion4" };
            List<string> Products = new List<string>() { "Product1", "Product2", "Product3", "Product4" };

            for (int i = 0; i < 64; i++)
            {
                source.Add(new Order()
                {
                    Advertisement = Advertisements[(i & 3) / 1],
                    Promotion = Promotions[(i & 12) / 4],
                    Product = Products[(i & 48) / 16],
                    Quantity = i
                });
            }

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity, TotalFormat = new RankGroupTotals() { Axis = DataAxis.Rows, Level = 0, Variation = RunningTotalSubGroupVariation.GroupDescriptionAndName } });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            #region expectedRowGroup
            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Advertisement1")
                {
                    new TestGroup("Promotion1")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion2")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion3")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion4")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    }
                },
                new TestGroup("Advertisement2")
                {
                    new TestGroup("Promotion1")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion2")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion3")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion4")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    }
                },
                new TestGroup("Advertisement3")
                {
                    new TestGroup("Promotion1")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion2")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion3")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion4")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    }
                },
                new TestGroup("Advertisement4")
                {
                    new TestGroup("Promotion1")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion2")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion3")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    },
                    new TestGroup("Promotion4")
                    {
                        new TestGroup("Product1"),
                        new TestGroup("Product2"),
                        new TestGroup("Product3"),
                        new TestGroup("Product4")
                    }
                }
            };
            #endregion

            IGroup expectedColumnGroup = new TestGroup("Grand Total");

            engine.RebuildCube(state);

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            var expectedResults = new List<object>() { 
                null,
                0, 0, 0, 0, 0, 0, 0, 4, 4, 4, 4, 0, 8, 8, 8, 8, 0, 12, 12, 12, 12,
                1, 1, 1, 1, 1, 1, 1, 5, 5, 5, 5, 1, 9, 9, 9, 9, 1, 13, 13, 13, 13,
                2, 2, 2, 2, 2, 2, 2, 6, 6, 6, 6, 2, 10, 10, 10, 10, 2, 14, 14, 14, 14,
                3, 3, 3, 3, 3, 3, 3, 7, 7, 7, 7, 3, 11, 11, 11, 11, 3, 15, 15, 15, 15
            };

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            for (int x = 0; x < 85; x++)
            {
                IGroup rowGroup = rowGroups[x];
                IGroup columnGroup = engine.Root.ColumnGroup;
                var aggregate = engine.GetAggregateResult(0, rowGroup, columnGroup);
                var actualResult = aggregate != null ? aggregate.GetValue() : null;
                var expectedResult = expectedResults[x];
                Assert.IsTrue(Object.Equals(actualResult, expectedResult), "Results for: {0}, {1} aggregate index {2}{5}Expected: {3}{5}Actual: {4}", x, 0, 0, expectedResult, actualResult, System.Environment.NewLine);
            }
        }

        [TestMethod]
        public void ParallelDataEngine_TotalsFormat_SingleTotalFormat()
        {
            IList<Order> source = GenerateRandomOrdersSource();

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Date, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity, TotalFormat = new PercentOfGrandTotal() });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Advertisement1")
                {
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2")
                },
                new TestGroup("Advertisement2")
                {
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2")
                },
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Product1")
                {
                    new TestGroup(new DateTime(2012, 1, 1)),
                    new TestGroup(new DateTime(2012, 2, 1))
                },
                new TestGroup("Product2")
                {
                    new TestGroup(new DateTime(2012, 1, 1)),
                    new TestGroup(new DateTime(2012, 2, 1))
                }
            };

            engine.RebuildCube(state);

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            // Bottom level results:
            // 120  58  24  32  64  28  36
            //  44  20   8  12  24  10  14
            //   6   2   0   2   4   1   3
            //  38  18   8  10  20   9  11
            //  75  36  16  20  39  18  22
            //  22  10   4   6  12   5   7
            //  53  26  12  14  27  13  15

            // Percent of total:
            //  1.00	0.48	0.20	0.27	0.53	0.23	0.30
            //  0.37	0.17	0.07	0.10	0.20	0.08	0.12
            //  0.05	0.02	0.00	0.02	0.03	0.01	0.03
            //  0.32	0.15	0.07	0.08	0.17	0.08	0.09
            //  0.63	0.30	0.13	0.17	0.33	0.15	0.18
            //  0.18	0.08	0.03	0.05	0.10	0.04	0.06
            //  0.44	0.22	0.10	0.12	0.23	0.11	0.13

            List<List<double>> expectedResults = new List<List<double>>()
            {
                new List<double>() { 1.00, 0.48, 0.20, 0.27, 0.53, 0.23, 0.30 },
                new List<double>() { 0.37, 0.17, 0.07, 0.10, 0.20, 0.08, 0.12 },
                new List<double>() { 0.05, 0.02, 0.00, 0.02, 0.03, 0.01, 0.03 },
                new List<double>() { 0.32, 0.15, 0.07, 0.08, 0.17, 0.08, 0.09 },
                new List<double>() { 0.63, 0.30, 0.13, 0.17, 0.33, 0.15, 0.18 },
                new List<double>() { 0.18, 0.08, 0.03, 0.05, 0.10, 0.04, 0.06 },
                new List<double>() { 0.44, 0.22, 0.10, 0.12, 0.23, 0.11, 0.13 }
            };

            for (int x = 0; x < 7; x++)
            {
                for (int y = 0; y < 7; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = (double)engine.GetAggregateResult(0, rowGroup, columnGroup).GetValue();
                    var expectedResult = expectedResults[y][x]; // NOTE: The results in the list above had been flipped!
                    Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                    Assert.IsTrue(Math.Abs(actualResult - expectedResult) < 0.015, "Results for: {0}, {1}{4}Expected: {2}{4}Actual: {3}", x, y, string.Join(", ", expectedResult), string.Join(", ", actualResult), System.Environment.NewLine);
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_TotalFormat_SingleTotalFormat_Exception()
        {
            IList<Order> source = new OrdersSource();
            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity, TotalFormat = new ExceptionSingleTotalFormat() });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total");
            IGroup expectedColumnGroup = new TestGroup("Grand Total");

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            var result = engine.GetAggregateResult(0, engine.Root);
            Assert.IsNotNull(result, "Expected an AggregateValue for the engine's Root");
            var actualValue = result.GetValue();
            object expectedValue = AggregateValue.Error;
            Assert.AreEqual(actualValue, expectedValue, "The total aggregate should have been formated as error.");
        }

        [TestMethod]
        public void ParallelDataEngine_TotalFormat_SibblingsTotalFormat_Exception()
        {
            IList<Order> source = new OrdersSource();
            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity, TotalFormat = new ExceptionSiblingTotalFormat() { Level = 0, Axis = DataAxis.Rows } });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Copy holder"),
                new TestGroup("Glare filter"),
                new TestGroup("Mouse pad"),
                new TestGroup("Printer stand")
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total");

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();

            List<object> expectedResults = new List<object>() { null, AggregateValue.Error, AggregateValue.Error, AggregateValue.Error, AggregateValue.Error };

            for (int x = 0; x < 5; x++)
            {
                IGroup rowGroup = rowGroups[x];
                IGroup columnGroup = engine.Root.ColumnGroup;
                var resultAggregate = engine.GetAggregateResult(0, rowGroup, columnGroup);
                var actualResult = resultAggregate != null ? resultAggregate.GetValue() : null;
                var expectedResult = expectedResults[x];
                Assert.AreEqual(actualResult, expectedResult, "The expected results mismatch for row group {0}", rowGroup.Name);
            }
        }

        [TestMethod]
        public void ParallelDataEngine_Parallel_GroupingWithMultyAggregates_CompletedEvent()
        {
            ParallelDataEngine engine;
            ParallelState state;
            SetupEngineForParallel(false, out engine, out state);

            int timeLimit = 500;
            ManualResetEvent mre = new ManualResetEvent(false);
            engine.Completed += (s, e) =>
                {
                    if (e.Status != DataEngineStatus.InProgress)
                    {
                        mre.Set();
                    }
                };
            engine.RebuildCubeParallel(state);
            if (!mre.WaitOne(timeLimit))
            {
                Assert.IsNotNull(engine.Root, "Timeout. Engine could not completed in {0} milliseconds. Consider extending the timelimit.", timeLimit);
            }

            CheckParallelResults(engine);
        }

        [TestMethod]
        public void ParallelDataEngine_Parallel_Cancel_SlowGroupDescriptions_CompletedEvent()
        {
            ParallelDataEngine engine;
            ParallelState state;
            SetupEngineForParallel(true, out engine, out state);

            DataEngineCompletedEventArgs args = null;

            int sleepTime = 10;
            int timeLimit = 500;

            ManualResetEvent mre = new ManualResetEvent(false);
            engine.Completed += (s, e) =>
                {
                    if (e.Status != DataEngineStatus.InProgress)
                    {
                        args = e;
                        mre.Set();
                    }
                };
            engine.RebuildCubeParallel(state);

            new System.Threading.ManualResetEvent(false).WaitOne(sleepTime);
            engine.Clear(state);

            if (!mre.WaitOne(timeLimit))
            {
                Assert.Fail("The engine was Canceled after {0} milliseconds sleep but failed to invoke Completed in {1} milliseconds.", sleepTime, timeLimit);
            }

            Assert.IsNotNull(args.InnerExceptions, "There must be no inner exceptions");
            Assert.AreEqual(DataEngineStatus.Completed, args.Status, "The Completed event args's Status should be 'Completed'.");
            // Assert.IsNull(engine.Root, "The engine should store no results after Cancel.");

            Assert.IsNotNull(engine.Root);
            Assert.IsNotNull(engine.Root.ColumnGroup);
            Assert.IsNotNull(engine.Root.RowGroup);
        }

        [TestMethod]
        public void ParallelDataEngine_Parallel_GroupingWithMultyAggregates_WaitForParallel()
        {
            ParallelDataEngine engine;
            ParallelState state;
            SetupEngineForParallel(false, out engine, out state);

            engine.RebuildCubeParallel(state);
            engine.WaitForParallel();

            CheckParallelResults(engine);
        }

        [TestMethod]
        public void ParallelDataEngine_GetAggregateValue_EmptyCoordinate()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            List<Order> source = new List<Order>();
            source.Add(new Order() { Advertisement = "A1", Product = "P1", Quantity = 1 });
            source.Add(new Order() { Advertisement = "A2", Product = "P2", Quantity = 1 });

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("A1"),
                new TestGroup("A2")
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("P1"),
                new TestGroup("P2")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            List<List<bool>> nonNullAggregates = new List<List<bool>>()
            {
                new List<bool>() { true, true, true },
                new List<bool>() { true, true, false },
                new List<bool>() { true, false, true }
            };

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = engine.GetAggregateResult(0, rowGroup, columnGroup);
                    bool shouldBeNonNull = nonNullAggregates[x][y];
                    if (shouldBeNonNull)
                    {
                        Assert.IsNotNull(actualResult, "The GetAggregateResult() at {0}, {1} was expected to return an non null AggregateValue.");
                    }
                    else
                    {
                        Assert.IsNull(actualResult, "The GetAggregateResult() at {0}, {1} was expected to return null.");
                    }
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_ResultsHaveTheSameGroupDescriptions()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            OrdersSource source = new OrdersSource();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer() });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            Assert.IsTrue(engine.RowGroupDescriptions.ItemsEqual(rowList), "Row group descriptions in the Engine differ the settings in the ParallelState");
            Assert.IsTrue(engine.ColumnGroupDescriptions.ItemsEqual(columnList), "Coulmn group descriptions in the Engine differ the settings in the ParallelState");
            Assert.IsTrue(engine.AggregateDescriptions.ItemsEqual(aggregateList), "Aggregate descriptions in the Engine differ the settings in the ParallelState");
        }

        [TestMethod]
        public void ParallelDataEngine_GetAggregateValue_UnavailableIndex()
        {
            IList<Order> source = new OrdersSource();
            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total");
            IGroup expectedColumnGroup = new TestGroup("Grand Total");

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            var result = engine.GetAggregateResult(-1, engine.Root);
            Assert.IsNull(result, "Expected an AggregateValue for the engine's Root with aggregate -1 was null");
            Assert.IsNull(result, "Expected an AggregateValue for the engine's Root with aggregate 1 was null");
        }

        [TestMethod]
        public void ParallelDataEngine_ValueFilter()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            List<Order> source = new List<Order>();

            source.Add(new Order() { Product = "P1", Promotion = "P1", Quantity = 0 });
            source.Add(new Order() { Product = "P1", Promotion = "P1", Quantity = 1 });
            source.Add(new Order() { Product = "P1", Promotion = "P2", Quantity = 0 });
            source.Add(new Order() { Product = "P1", Promotion = "P2", Quantity = 1 });
            source.Add(new Order() { Product = "P2", Promotion = "P1", Quantity = 0 });
            source.Add(new Order() { Product = "P2", Promotion = "P1", Quantity = 1 });
            source.Add(new Order() { Product = "P2", Promotion = "P2", Quantity = 0 });
            source.Add(new Order() { Product = "P2", Promotion = "P2", Quantity = 1 });

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            IList<OrdersFilterDescription> filterList = new List<OrdersFilterDescription>();
            filterList.Add(new OrdersFilterDescription() { Field = OrderFields.Quantity, Filter = (q) => ((int)q) != 0 });
            IReadOnlyList<OrdersFilterDescription> filterReadOnlyList = new ReadOnlyList<OrdersFilterDescription, OrdersFilterDescription>(filterList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    FilterDescriptions = filterReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,
                FilterDescriptions = filterReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("P1"),
                new TestGroup("P2")
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("P1"),
                new TestGroup("P2")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            List<List<double>> expectedResults = new List<List<double>>()
            {
                new List<double>() { 4, 2, 2 },
                new List<double>() { 2, 1, 1 },
                new List<double>() { 2, 1, 1 }
            };

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = (double)engine.GetAggregateResult(0, rowGroup, columnGroup).GetValue();
                    var expectedResult = expectedResults[x][y];
                    Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                    Assert.IsTrue(Math.Abs(actualResult - expectedResult) < 0.1, "Results for: {0}, {1}{4}Expected: {2}{4}Actual: {3}", x, y, string.Join(", ", expectedResult), string.Join(", ", actualResult), System.Environment.NewLine);
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_FilterDescriptions()
        {
            ParallelDataEngine engine = new ParallelDataEngine();
            List<Order> source = new List<Order>();

            source.Add(new Order() { Product = "P1", Promotion = "P1", Quantity = 0 });
            source.Add(new Order() { Product = "P1", Promotion = "P1", Quantity = 1 });
            source.Add(new Order() { Product = "P1", Promotion = "P2", Quantity = 0 });
            source.Add(new Order() { Product = "P1", Promotion = "P2", Quantity = 1 });
            source.Add(new Order() { Product = "P2", Promotion = "P1", Quantity = 0 });
            source.Add(new Order() { Product = "P2", Promotion = "P1", Quantity = 1 });
            source.Add(new Order() { Product = "P2", Promotion = "P2", Quantity = 0 });
            source.Add(new Order() { Product = "P2", Promotion = "P2", Quantity = 1 });

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<SumAggregate>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);


            IList<OrdersFilterDescription> filterList = new List<OrdersFilterDescription>();
            filterList.Add(new OrdersFilterDescription() { Field = OrderFields.Quantity, Filter = (q) => ((int)q) != 0 });
            IReadOnlyList<OrdersFilterDescription> filterReadOnlyList = new ReadOnlyList<OrdersFilterDescription, OrdersFilterDescription>(filterList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    FilterDescriptions = filterReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,
                FilterDescriptions = filterReadOnlyList
            };

            engine.RebuildCube(state);

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("P1"),
                new TestGroup("P2")
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup("P1"),
                new TestGroup("P2")
            };

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            IList<IGroup> rowGroups = engine.Root.RowGroup.Flatten().ToList();
            IList<IGroup> columnGroups = engine.Root.ColumnGroup.Flatten().ToList();

            List<List<double>> expectedResults = new List<List<double>>()
            {
                new List<double>() { 4, 2, 2 },
                new List<double>() { 2, 1, 1 },
                new List<double>() { 2, 1, 1 }
            };

            for (int x = 0; x < 3; x++)
            {
                for (int y = 0; y < 3; y++)
                {
                    IGroup rowGroup = rowGroups[x];
                    IGroup columnGroup = columnGroups[y];
                    var actualResult = (double)engine.GetAggregateResult(0, rowGroup, columnGroup).GetValue();
                    var expectedResult = expectedResults[x][y];
                    Assert.IsNotNull(actualResult, "Expected non-null result for: {0}, {1}", x, y);
                    Assert.IsTrue(Math.Abs(actualResult - expectedResult) < 0.1, "Results for: {0}, {1}{4}Expected: {2}{4}Actual: {3}", x, y, string.Join(", ", expectedResult), string.Join(", ", actualResult), System.Environment.NewLine);
                }
            }
        }

        [TestMethod]
        public void ParallelDataEngine_AllKeys()
        {
            IList<Order> source = GenerateRandomOrdersSource();

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement, GroupComparer = new GroupNameComparer(), ShowGroupsWithNoData = true, WellKnownGroupNames = new List<object>() { "Advertisement0", "Advertisement1", "Advertisement2", "Advertisement3" } });
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Promotion, GroupComparer = new GroupNameComparer(), ShowGroupsWithNoData = true, WellKnownGroupNames = new List<object>() { "Promotion0", "Promotion1", "Promotion2", "Promotion3" } });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer(), ShowGroupsWithNoData = true, WellKnownGroupNames = new List<object>() { NullValue.Instance } });
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Date, GroupComparer = new GroupNameComparer(), ShowGroupsWithNoData = true });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<ListItems>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList
            };

            IGroup expectedRowGroup = new TestGroup("Grand Total")
            {
                new TestGroup("Advertisement0")
                {
                    new TestGroup("Promotion0"),
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2"),
                    new TestGroup("Promotion3")
                },
                new TestGroup("Advertisement1")
                {
                    new TestGroup("Promotion0"),
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2"),
                    new TestGroup("Promotion3")
                },
                new TestGroup("Advertisement2")
                {
                    new TestGroup("Promotion0"),
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2"),
                    new TestGroup("Promotion3"),
                },
                new TestGroup("Advertisement3")
                {
                    new TestGroup("Promotion0"),
                    new TestGroup("Promotion1"),
                    new TestGroup("Promotion2"),
                    new TestGroup("Promotion3")
                }
            };

            IGroup expectedColumnGroup = new TestGroup("Grand Total")
            {
                new TestGroup(NullValue.Instance)
                {
                    new TestGroup(new DateTime(2012, 1, 1)),
                    new TestGroup(new DateTime(2012, 2, 1))
                },
                new TestGroup("Product1")
                {
                    new TestGroup(new DateTime(2012, 1, 1)),
                    new TestGroup(new DateTime(2012, 2, 1))
                },
                new TestGroup("Product2")
                {
                    new TestGroup(new DateTime(2012, 1, 1)),
                    new TestGroup(new DateTime(2012, 2, 1))
                }
            };

            engine.RebuildCube(state);

            Assert.IsNotNull(engine.Root, "Engine grouping produced null Root");
            AssertIGroupAreEqual("engine.Root.RowGroup returned wrong groups", expectedRowGroup, engine.Root.RowGroup);
            AssertIGroupAreEqual("engine.Root.ColumnGroup returned wrong groups", expectedColumnGroup, engine.Root.ColumnGroup);

            var r0 = engine.GetUniqueKeys(DataAxis.Rows, 0);
            var r1 = engine.GetUniqueKeys(DataAxis.Rows, 1);
            var c0 = engine.GetUniqueKeys(DataAxis.Columns, 0);
            var c1 = engine.GetUniqueKeys(DataAxis.Columns, 1);

            var expectedR0 = new HashSet<object>(new List<object>() { "Advertisement0", "Advertisement1", "Advertisement2", "Advertisement3" });
            var expectedR1 = new HashSet<object>(new List<object>() { "Promotion0", "Promotion1", "Promotion2", "Promotion3" });
            var expectedC0 = new HashSet<object>(new List<object>() { NullValue.Instance, "Product1", "Product2" });
            var expectedC1 = new HashSet<object>(new List<object>() { new DateTime(2012, 1, 1), new DateTime(2012, 2, 1) });

            Assert.IsTrue(expectedR0.SetEquals(r0), "GetUniqueKeys for Rows 0 mismatched.");
            Assert.IsTrue(expectedR1.SetEquals(r1), "GetUniqueKeys for Rows 1 mismatched.");

            Assert.IsTrue(expectedC0.SetEquals(c0), "GetUniqueKeys for Columns 0 mismatched.");
            Assert.IsTrue(expectedC1.SetEquals(c1), "GetUniqueKeys for Columns 1 mismatched.");
        }

        [TestMethod]
        public void ParallelDataEngine_GetUniqueFilterItems_RandomNet()
        {
            IList<Order> source = GenerateRandomOrdersSource();

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<ListItems>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            IList<OrdersFilterDescription> filterList = new List<OrdersFilterDescription>();
            filterList.Add(new OrdersFilterDescription() { Field = OrderFields.Net, Filter = (object d) => ((double)d) < 50 });
            IReadOnlyList<OrdersFilterDescription> filterReadOnlyList = new ReadOnlyList<OrdersFilterDescription, OrdersFilterDescription>(filterList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    FilterDescriptions = filterReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,

                FilterDescriptions = filterReadOnlyList
            };

            engine.RebuildCube(state);

            var filterItems = engine.GetUniqueFilterItems(0);

            var filterNetItems = engine.GetUniqueFilterItems(0);
            var expectedNetFilterItems = new HashSet<double>(new List<double>() { 98.45, 90.78, 74.35, 81.16, 73.87, 4.83, 1.7, 14.93, 19.47, 63, 90.97, 49.51, 18.91, 46.09, 7.86, 84.64 });
            Assert.IsTrue(expectedNetFilterItems.SetEquals(filterNetItems.OfType<double>()), "Unique Net items does not match.");
        }

        [TestMethod]
        public void ParallelDataEngine_GetUniqueFilterItems_RandomNetAndAdvertisement()
        {
            IList<Order> source = GenerateRandomOrdersSource();

            ParallelDataEngine engine = new ParallelDataEngine();

            IList<OrdersGroupDescription> rowList = new List<OrdersGroupDescription>();
            rowList.Add(new OrdersGroupDescription() { Field = OrderFields.Advertisement });
            IReadOnlyList<OrdersGroupDescription> rowReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(rowList);

            IList<OrdersGroupDescription> columnList = new List<OrdersGroupDescription>();
            columnList.Add(new OrdersGroupDescription() { Field = OrderFields.Product, GroupComparer = new GroupNameComparer() });
            IReadOnlyList<OrdersGroupDescription> columnReadOnlyList = new ReadOnlyList<OrdersGroupDescription, OrdersGroupDescription>(columnList);

            IList<OrdersAggregateDescription> aggregateList = new List<OrdersAggregateDescription>();
            aggregateList.Add(new OrdersAggregateDescription<ListItems>() { Field = OrderFields.Quantity });
            IReadOnlyList<OrdersAggregateDescription> aggregateReadOnlyList = new ReadOnlyList<OrdersAggregateDescription, OrdersAggregateDescription>(aggregateList);

            IList<OrdersFilterDescription> filterList = new List<OrdersFilterDescription>();
            filterList.Add(new OrdersFilterDescription() { Field = OrderFields.Net, Filter = (object d) => ((double)d) < 50 });
            filterList.Add(new OrdersFilterDescription() { Field = OrderFields.Advertisement });
            IReadOnlyList<OrdersFilterDescription> filterReadOnlyList = new ReadOnlyList<OrdersFilterDescription, OrdersFilterDescription>(filterList);

            ParallelState state = new ParallelState()
            {
                DataView = new EnumerableDataSourceView(source),
                ValueProvider = new OrdersValueProvider()
                {
                    RowGroupDescriptions = rowReadOnlyList,
                    ColumnGroupDescriptions = columnReadOnlyList,
                    AggregateDescriptions = aggregateReadOnlyList,
                    FilterDescriptions = filterReadOnlyList,
                    GroupFactory = new DataGroupFactory()
                },

                RowGroupDescriptions = rowReadOnlyList,
                ColumnGroupDescriptions = columnReadOnlyList,
                AggregateDescriptions = aggregateReadOnlyList,

                FilterDescriptions = filterReadOnlyList
            };

            engine.RebuildCube(state);

            var filterNetItems = engine.GetUniqueFilterItems(0);
            var expectedNetFilterItems = new HashSet<double>(new List<double>() { 98.45, 90.78, 74.35, 81.16, 73.87, 4.83, 1.7, 14.93, 19.47, 63, 90.97, 49.51, 18.91, 46.09, 7.86, 84.64 });
            Assert.IsTrue(expectedNetFilterItems.SetEquals(filterNetItems.OfType<double>()), "Unique filter items does not match.");

            var filterAdvertisementItems = engine.GetUniqueFilterItems(1);
            var expectedAdvertisementFilterItems = new HashSet<string>(new List<string>() { "Advertisement1", "Advertisement2" });
            Assert.IsTrue(expectedAdvertisementFilterItems.SetEquals(filterAdvertisementItems.OfType<string>()), "Unique Net items does not match.");
        }

        [TestMethod]
        public void Clear_WhenEngineHasResults_CreatesNewRootCoordinateWithEmptyRowandColumnGroups()
        {
            ParallelDataEngine engine;
            ParallelState state;
            GetEngineWithResults(out engine, out state);

            engine.Clear(state);

            Assert.IsNotNull(engine.Root.ColumnGroup);
            Assert.IsNotNull(engine.Root.RowGroup);
        }

        [TestMethod]
        public void Clear_WhenEngineHasResults_ClearsAllDescriptionCollections()
        {
            ParallelDataEngine engine;
            ParallelState state;
            GetEngineWithResults(out engine, out state);

            engine.Clear(state);

            Assert.AreEqual(0, engine.AggregateDescriptions.Count);
            Assert.AreEqual(0, engine.ColumnGroupDescriptions.Count);
            Assert.AreEqual(0, engine.FilterDescriptions.Count);
            Assert.AreEqual(0, engine.RowGroupDescriptions.Count);
        }

        [TestMethod]
        public void Clear_WhenEngineHasResults_ClearsAllUniqueKeys()
        {
            ParallelDataEngine engine;
            ParallelState state;
            GetEngineWithResults(out engine, out state);

            engine.Clear(state);

            Assert.AreEqual(null, engine.GetUniqueKeys(DataAxis.Rows, 0));
        }

        [TestMethod]
        public void Clear_WhenEngineHasResults_ClearsAllUniqueFilterItems()
        {
            ParallelDataEngine engine;
            ParallelState state;
            GetEngineWithResults(out engine, out state);

            engine.Clear(state);

            Assert.AreEqual(null, engine.GetUniqueFilterItems(0));
        }

        [TestMethod]
        public void Clear_WhenEngineHasResults_RaisesCompletedEvent()
        {
            ParallelDataEngine engine;
            ParallelState state;
            GetEngineWithResults(out engine, out state);

            var engineCompleted = false;
            engine.Completed += (s, a) =>
            {
                engineCompleted = true;
            };

            engine.Clear(state);

            Assert.IsTrue(engineCompleted);
        }
    }
}