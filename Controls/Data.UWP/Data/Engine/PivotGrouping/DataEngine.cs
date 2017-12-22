using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Threading;
using Telerik.Core;
using Telerik.Data.Core.Aggregates;
using Telerik.Data.Core.Totals;

namespace Telerik.Data.Core.Engine
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Will resolve in future.")]
    internal partial class ParallelDataEngine : IDataEngine
    {
        private IDictionary<Coordinate, AggregateValue[]> aggregates;
        private IDictionary<Coordinate, AggregateValue[]> summaries;
        private IDictionary<Coordinate, AggregateValue[]> formattedTotals;
        private IValueProvider valueProvider;

        private List<List<HashSet<object>>> uniqueGroupKeys;
        private HashSet<object>[] uniqueFilterItems;

        public ParallelDataEngine()
        {
            this.aggregates = new Dictionary<Coordinate, AggregateValue[]>();
            this.summaries = new Dictionary<Coordinate, AggregateValue[]>();
            this.formattedTotals = new Dictionary<Coordinate, AggregateValue[]>();
            this.RowGroupDescriptions = new ReadOnlyList<GroupDescription, GroupDescription>(new List<GroupDescription>());
            this.ColumnGroupDescriptions = new ReadOnlyList<GroupDescription, GroupDescription>(new List<GroupDescription>());
            this.AggregateDescriptions = new ReadOnlyList<IAggregateDescription, IAggregateDescription>(new List<IAggregateDescription>());
            this.FilterDescriptions = new ReadOnlyList<FilterDescription, FilterDescription>(new List<FilterDescription>());
        }

        public event EventHandler<DataEngineCompletedEventArgs> Completed;

        public Coordinate Root { get; private set; }

        public IReadOnlyList<GroupDescription> RowGroupDescriptions { get; private set; }

        public IReadOnlyList<GroupDescription> ColumnGroupDescriptions { get; private set; }

        public IReadOnlyList<IAggregateDescription> AggregateDescriptions { get; private set; }

        public IReadOnlyList<FilterDescription> FilterDescriptions { get; private set; }

        public static Tuple<Group, int> DepthFirstSearch(Group rootGroup, object item, IComparer<object> itemComparer)
        {
            int index = -1;
            Group bottomGroup = null;

            Stack<Group> groups = new Stack<Group>();
            groups.Push(rootGroup);

            while (groups.Count > 0)
            {
                var group = groups.Pop();
                if (group.IsBottomLevel)
                {
                    index = group.IndexOf(item, itemComparer);
                    if (index >= 0)
                    {
                        bottomGroup = group;
                        break;
                    }
                }
                else
                {
                    int count = group.Items.Count - 1;
                    while (count >= 0)
                    {
                        Group subGroup = group.Items[count--] as Group;
                        groups.Push(subGroup);
                    }
                }
            }

            return Tuple.Create(bottomGroup, index);
        }

        /// <inheritdoc />
        public IEnumerable<object> GetUniqueKeys(DataAxis axis, int groupDescriptionIndex)
        {
            if (this.uniqueGroupKeys != null)
            {
                if (axis == DataAxis.Rows && this.uniqueGroupKeys.Count >= 1)
                {
                    List<HashSet<object>> axisKeys = this.uniqueGroupKeys[0];
                    if (groupDescriptionIndex >= 0 && groupDescriptionIndex < axisKeys.Count)
                    {
                        // TODO: do not return the actual HashSet<object> but rather wrap it in read only view
                        return axisKeys[groupDescriptionIndex];
                    }
                }
                else if (axis == DataAxis.Columns && this.uniqueGroupKeys.Count >= 2)
                {
                    List<HashSet<object>> axisKeys = this.uniqueGroupKeys[1];
                    if (groupDescriptionIndex >= 0 && groupDescriptionIndex < axisKeys.Count)
                    {
                        // TODO: do not return the actual HashSet<object> but rather wrap it in read only view
                        return axisKeys[groupDescriptionIndex];
                    }
                }
            }

            return null;
        }

        /// <inheritdoc />
        public IEnumerable<object> GetUniqueFilterItems(int filterIndex)
        {
            if (this.uniqueFilterItems != null)
            {
                if (filterIndex >= 0 && filterIndex < this.uniqueFilterItems.Length)
                {
                    return this.uniqueFilterItems[filterIndex];
                }
            }

            return null;
        }

        public void AddUniqueFilterItems(int filterIndex, object item)
        {
            if (this.uniqueFilterItems != null)
            {
                if (filterIndex >= 0 && filterIndex < this.uniqueFilterItems.Length)
                {
                    this.uniqueFilterItems[filterIndex].Add(item);
                }
            }
        }

        public void RemoveUniqueFilterItems(int filterIndex, object item)
        {
            if (this.uniqueFilterItems != null)
            {
                if (filterIndex >= 0 && filterIndex < this.uniqueFilterItems.Length)
                {
                    this.uniqueFilterItems[filterIndex].Remove(item);
                }
            }
        }

        public AggregateValue GetAggregateResult(int aggregateIndex, IGroup row, IGroup column)
        {
            return this.GetAggregateResult(aggregateIndex, new Coordinate(row, column));
        }

        public AggregateValue GetAggregateResult(int aggregateIndex, Coordinate coordinate)
        {
            AggregateValue[] results;

            if (aggregateIndex >= 0 && aggregateIndex < this.AggregateDescriptions.Count)
            {
                var aggregateDescription = this.AggregateDescriptions[aggregateIndex];

                // TODO: add a list with info if an aggregate description had formatting...
                if (aggregateDescription.TotalFormat != null)
                {
                    // Expected a formatted value
                    if (this.formattedTotals.TryGetValue(coordinate, out results))
                    {
                        AggregateValue result = results[aggregateIndex];
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
                else
                {
                    // If not formatted return core summary or aggregate
                    if (this.aggregates.TryGetValue(coordinate, out results))
                    {
                        AggregateValue result = results[aggregateIndex];
                        if (result != null)
                        {
                            return result;
                        }
                    }
                    else if (this.summaries.TryGetValue(coordinate, out results))
                    {
                        AggregateValue result = results[aggregateIndex];
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }

            return null;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design choice.")]
        public void RebuildCube(ParallelState state)
        {
            var exceptions = new List<Exception>();
            var status = DataEngineStatus.Faulted;

            try
            {
                this.CancelCurrentProcessing(state);
                this.RaiseInProgress();
                state.CancellationTokenSource = new CancellationTokenSource();

                HashSet<object>[] uniqueFilterItemsResult;
                List<object> items;
                SortAndFilterItems(state, out uniqueFilterItemsResult, out items);

                GroupingResults bottomLevelResultsTask;
                if (!state.HasDescriptions)
                {
                    IGroupFactory groupFactory = state.ValueProvider.GetGroupFactory();
                    Group rowRootGroup = CreateGrandTotal(groupFactory);
                    rowRootGroup.SetItems(items);
                    Group columnRootGroup = CreateGrandTotal(groupFactory);
                    IDictionary<Coordinate, AggregateValue[]> emptyAggregates = new Dictionary<Coordinate, AggregateValue[]>();
                    bottomLevelResultsTask = new GroupingResults(rowRootGroup, columnRootGroup, groupFactory) { Aggregates = emptyAggregates, UniqueFilterItems = uniqueFilterItemsResult };
                }
                else
                {
                    BottomLevelGroupingTaskState taskState = new BottomLevelGroupingTaskState();
                    taskState.ParallelState = state;
                    taskState.Start = 0;
                    taskState.End = state.ItemCount;
                    bottomLevelResultsTask = ProcessItems(taskState);
                }

                bottomLevelResultsTask.UniqueFilterItems = uniqueFilterItemsResult;

                GroupingFinalizationTaskState finalizationTasksState = new GroupingFinalizationTaskState()
                {
                    ParallelState = state,
                    Results = bottomLevelResultsTask
                };

                this.FinalizeAggregations(finalizationTasksState);
                status = DataEngineStatus.Completed;
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }

            var innerExceptions = new ReadOnlyCollection<Exception>(exceptions);
            this.RaiseCompleted(new DataEngineCompletedEventArgs(innerExceptions, status));
        }

        public void WaitForParallel()
        {
            if (this.currentResultTask != null)
            {
                this.currentResultTask.Wait();
            }
        }

        public void Clear(ParallelState state)
        {
            this.CancelCurrentProcessing(state);
            this.RaiseCompleted(new DataEngineCompletedEventArgs(new ReadOnlyCollection<Exception>(new List<Exception>()), DataEngineStatus.Completed));
        }

        List<AddRemoveResult> IDataEngine.InsertItems(int index, System.Collections.IEnumerable items)
        {
            return this.Process(index, items, false, false, true);
        }

        List<AddRemoveResult> IDataEngine.RemoveItems(int index, System.Collections.IEnumerable items, bool removeFilteredItem, bool canUseComparer)
        {
            return this.Process(index, items, true, removeFilteredItem, canUseComparer);
        }

        internal static Group CreateGrandTotal(IGroupFactory groupFactory)
        {
            return groupFactory.CreateGroup(Group.GrandTotalName);
        }

        private static void ApplyStringFormats(GroupingFinalizationTaskState finalizationTasksState, AggregateResultProvider resultsProvider, IDictionary<Coordinate, AggregateValue[]> totalFormats)
        {
            var aggregateDescriptions = finalizationTasksState.ParallelState.AggregateDescriptions;

            var stringFormats = new List<string>(aggregateDescriptions.Count);
            for (int i = 0; i < aggregateDescriptions.Count; i++)
            {
                stringFormats.Add(finalizationTasksState.ParallelState.ValueProvider.GetAggregateStringFormat(i));
            }

            // TODO: These should have one format:
            ApplyStringFormatsToAggregateValues(resultsProvider.Aggregates, stringFormats);
            ApplyStringFormatsToAggregateValues(resultsProvider.Summaries, stringFormats);

            // TODO: And these should have another:
            ApplyStringFormatsToAggregateValues(totalFormats, stringFormats);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design choice.")]
        private static void ApplyStringFormatsToAggregateValues(IDictionary<Coordinate, AggregateValue[]> aggValues, List<string> stringFormats)
        {
            foreach (var aggregatePair in aggValues)
            {
                var aggregateValues = aggregatePair.Value;
                for (int i = 0; i < aggregateValues.Length; i++)
                {
                    var aggregateValue = aggregateValues[i];
                    var stringFormat = stringFormats[i];
                    if (aggregateValue != null && stringFormat != null)
                    {
                        try
                        {
                            var value = aggregateValue.GetValue() as IFormattable;
                            if (value != null)
                            {
                                aggregateValue.SetFormattedValue(value.ToString(stringFormat, CultureInfo.CurrentCulture));
                            }
                        }
                        catch
                        {
                            aggregateValue.RaiseError(); // TODO: Formatting Error
                        }
                    }
                }
            }
        }

        private static Dictionary<Coordinate, AggregateValue[]> GenerateFormattedTotals(GroupingFinalizationTaskState finalizationTasksState, IAggregateResultProvider summaryResults)
        {
            ParallelState state = finalizationTasksState.ParallelState;
            GroupingResults aggregateResults = finalizationTasksState.Results;
            var aggregates = finalizationTasksState.ParallelState.AggregateDescriptions;

            return GenerateFormattedTotals(summaryResults, aggregates);
        }

        private static Dictionary<Coordinate, AggregateValue[]> GenerateFormattedTotals(IAggregateResultProvider summaryResults, IReadOnlyList<IAggregateDescription> aggregates)
        {
            for (int aggregateIndex = 0; aggregateIndex < aggregates.Count; aggregateIndex++)
            {
                IAggregateDescription aggregateDescription = aggregates[aggregateIndex];

                // TODO: implement simplier formatter...
                TotalFormat totalFormat = aggregateDescription.TotalFormat;
                SingleTotalFormat formattedTotals = totalFormat as SingleTotalFormat;
                if (formattedTotals != null)
                {
                    return GenerateSimpleFormat(summaryResults, aggregateIndex, aggregates.Count, formattedTotals);
                }
                else
                {
                    SiblingTotalsFormat showValueAs = totalFormat as SiblingTotalsFormat;
                    if (showValueAs != null)
                    {
                        return GenerateRunningTotals(summaryResults, aggregates, aggregateIndex, showValueAs);
                    }
                }
            }

            return new Dictionary<Coordinate, AggregateValue[]>();
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design choice.")]
        private static Dictionary<Coordinate, AggregateValue[]> GenerateSimpleFormat(IAggregateResultProvider summaryResults, int aggregateIndex, int aggregatesCount, SingleTotalFormat formattedTotals)
        {
            Dictionary<Coordinate, AggregateValue[]> formatTotals = new Dictionary<Coordinate, AggregateValue[]>();

            Group rowRootGroup = summaryResults.Root.RowGroup as Group;
            Group columnRootGroup = summaryResults.Root.ColumnGroup as Group;

            IList<Group> rowGroups = Enumerable.ToList(GetChildrenGroups(rowRootGroup));
            IList<Group> columnGroups = Enumerable.ToList(GetChildrenGroups(columnRootGroup));

            foreach (var rowGroup in rowGroups)
            {
                foreach (var columnGroup in columnGroups)
                {
                    Coordinate coordinate = new Coordinate(rowGroup, columnGroup);

                    // TODO: Try/Catch here... as in the GenerateRunningTotals
                    AggregateValue value;
                    try
                    {
                        value = formattedTotals.FormatValue(coordinate, summaryResults, aggregateIndex);
                    }
                    catch
                    {
                        value = new ConstantValueAggregate(AggregateValue.Error);
                    }

                    SetRunningTotalValue(formatTotals, aggregatesCount, aggregateIndex, coordinate, value);
                }
            }

            return formatTotals;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design choice.")]
        private static Dictionary<Coordinate, AggregateValue[]> GenerateRunningTotals(IAggregateResultProvider summaryResults, IReadOnlyList<IAggregateDescription> aggregates, int aggregateIndex, SiblingTotalsFormat showValueAs)
        {
            Dictionary<Coordinate, AggregateValue[]> formatTotals = new Dictionary<Coordinate, AggregateValue[]>();

            Group rowRootGroup = summaryResults.Root.RowGroup as Group;
            Group columnRootGroup = summaryResults.Root.RowGroup as Group;

            IEqualityComparer<object[]> comparer = null;
            switch (showValueAs.SubVariation())
            {
                case RunningTotalSubGroupVariation.ParentAndSelfNames:
                    comparer = new ObjectArrayComparer();
                    break;
                case RunningTotalSubGroupVariation.GroupDescriptionAndName:
                default:
                    comparer = new CountAndLastArrayComparer();
                    break;
            }

            DataAxis runningAxis = showValueAs.Axis;
            DataAxis oppositeAxis = runningAxis == DataAxis.Rows ? DataAxis.Columns : DataAxis.Rows;
            IEnumerable<Group> oppositeAxisGroups = Enumerable.ToList(GetChildrenGroups(runningAxis == DataAxis.Rows ? columnRootGroup : rowRootGroup));
            IEnumerable<Group> domainGroups = ChildGroupsAtLevel(runningAxis == DataAxis.Rows ? rowRootGroup : columnRootGroup, showValueAs.Level);

            foreach (var domainGroup in domainGroups)
            {
                if (domainGroup.HasItems && !domainGroup.IsBottomLevel)
                {
                    IEnumerable<List<Group>> subNameTrees = Enumerable.ToList(GetUniqueSubNameTrees(domainGroup.Items, comparer));

                    foreach (var opositeAxisGroup in oppositeAxisGroups)
                    {
                        foreach (var subTree in subNameTrees)
                        {
                            List<TotalValue> totals = new List<TotalValue>();
                            foreach (var itterationGroup in subTree)
                            {
                                Coordinate coordinate = runningAxis == DataAxis.Rows ? new Coordinate(itterationGroup, opositeAxisGroup) : new Coordinate(opositeAxisGroup, itterationGroup);
                                totals.Add(new TotalValue(summaryResults, coordinate, aggregateIndex));
                            }

                            try
                            {
                                showValueAs.FormatTotals(new ReadOnlyList<TotalValue, TotalValue>(totals), summaryResults);

                                foreach (var total in totals)
                                {
                                    SetRunningTotalValue(formatTotals, aggregates.Count, aggregateIndex, total.Groups, total.FormattedValue);
                                }
                            }
                            catch
                            {
                                ConstantValueAggregate error = new ConstantValueAggregate(AggregateValue.Error);
                                foreach (var total in totals)
                                {
                                    SetRunningTotalValue(formatTotals, aggregates.Count, aggregateIndex, total.Groups, error);
                                }
                            }
                        }
                    }
                }
            }

            return formatTotals;
        }

        private static void SetRunningTotalValue(Dictionary<Coordinate, AggregateValue[]> runningTotals, int aggregatesCount, int aggregateIndex, Coordinate coordinate, AggregateValue aggregateValue)
        {
            if (aggregateValue != null)
            {
                AggregateValue[] aggregateValues = null;
                runningTotals.TryGetValue(coordinate, out aggregateValues);

                if (aggregateValues == null)
                {
                    aggregateValues = new AggregateValue[aggregatesCount];
                    runningTotals[coordinate] = aggregateValues;
                }

                aggregateValues[aggregateIndex] = aggregateValue;
            }
        }

        private static IEnumerable<List<Group>> GetUniqueSubNameTrees(IEnumerable<object> groups, IEqualityComparer<object[]> comparer)
        {
            List<object> parentNames = new List<object>();

            Dictionary<object[], List<Group>> subTreeNames = new Dictionary<object[], List<Group>>(comparer);

            foreach (var group in groups.OfType<Group>())
            {
                AddGroupToNamesSubTree(parentNames, subTreeNames, group);
                if (group.HasItems && !group.IsBottomLevel)
                {
                    GetUniqueSubNameTrees(group.Items, parentNames, subTreeNames);
                }
            }

            return Enumerable.Select(subTreeNames, s => s.Value);
        }

        private static void GetUniqueSubNameTrees(IEnumerable<object> groups, List<object> parentNames, Dictionary<object[], List<Group>> subTreeNames)
        {
            foreach (var group in groups.OfType<Group>())
            {
                parentNames.Add(group.Name);
                AddGroupToNamesSubTree(parentNames, subTreeNames, group);

                if (group.HasItems && !group.IsBottomLevel)
                {
                    GetUniqueSubNameTrees(group.Items, parentNames, subTreeNames);
                }

                parentNames.RemoveAt(parentNames.Count - 1);
            }
        }

        private static void AddGroupToNamesSubTree(IList<object> parentNames, Dictionary<object[], List<Group>> subTreeNames, Group group)
        {
            List<Group> subTreeGroups = null;
            object[] parentNamesArray = parentNames.ToArray();
            subTreeNames.TryGetValue(parentNamesArray, out subTreeGroups);
            if (subTreeGroups == null)
            {
                subTreeNames.Add(parentNamesArray, new List<Group>() { group });
            }
            else
            {
                subTreeGroups.Add(group);
            }
        }

        private static IEnumerable<Group> ChildGroupsAtLevel(Group root, int depth)
        {
            if (depth > 0)
            {
                if (root.HasItems && !root.IsBottomLevel)
                {
                    foreach (var group in root.Items.OfType<Group>())
                    {
                        foreach (var childGroup in ChildGroupsAtLevel(group, depth - 1))
                        {
                            yield return childGroup;
                        }
                    }
                }
            }
            else
            {
                yield return root;
            }
        }

        private static void GenerateAllKeys(GroupingFinalizationTaskState finalizationState, List<List<HashSet<object>>> uniqueKeys)
        {
            ParallelState state = finalizationState.ParallelState;
            GenerateAllKeysAxis(uniqueKeys, state, DataAxis.Rows, state.RowGroupDescriptions);
            GenerateAllKeysAxis(uniqueKeys, state, DataAxis.Columns, state.ColumnGroupDescriptions);
        }

        private static void GenerateAllKeysAxis(List<List<HashSet<object>>> uniqueKeys, ParallelState state, DataAxis axis, IReadOnlyList<GroupDescription> rd)
        {
            for (int l = 0; l < rd.Count; l++)
            {
                state.CancellationToken.ThrowIfCancellationRequested();

                var groupDescription = rd[l];
                HashSet<object> groupUniqueKeys = uniqueKeys[(int)axis][l];
                HashSet<object> groupAllKeys = new HashSet<object>();
                uniqueKeys[(int)axis][l] = groupAllKeys;

                // TODO: Where did my parents go?!?! This null is not expected here... We should find another way to get all names...
                // TODO: Provide only parents within the same GetMemberName()...
                IEnumerable<object> keys = groupDescription.GetAllNames(groupUniqueKeys, Enumerable.Empty<object>());
                if (keys != null)
                {
                    foreach (var key in keys)
                    {
                        groupAllKeys.Add(key);
                    }
                }
            }
        }

        private static List<List<HashSet<object>>> GenerateUniqueKeys(GroupingFinalizationTaskState finalizationState)
        {
            List<List<HashSet<object>>> uniqueKeys = new List<List<HashSet<object>>>();
            GenerateUniqueKeysForAxis(finalizationState, uniqueKeys, DataAxis.Rows);
            GenerateUniqueKeysForAxis(finalizationState, uniqueKeys, DataAxis.Columns);
            return uniqueKeys;
        }

        private static void GenerateUniqueKeysForAxis(GroupingFinalizationTaskState finalizationState, List<List<HashSet<object>>> uniqueKeys, DataAxis axis)
        {
            ParallelState state = finalizationState.ParallelState;
            GroupingResults results = finalizationState.Results;

            state.CancellationToken.ThrowIfCancellationRequested();
            uniqueKeys.Add(new List<HashSet<object>>());
            IReadOnlyList<GroupDescription> groupDescriptions = axis == DataAxis.Rows ? state.RowGroupDescriptions : state.ColumnGroupDescriptions;

            for (int l = 0; l < groupDescriptions.Count; l++)
            {
                uniqueKeys[(int)axis].Add(new HashSet<object>());
            }

            IGroup group = axis == DataAxis.Rows ? results.Root.RowGroup : results.Root.ColumnGroup;
            GetUniqueKeys(state.CancellationToken, group, 0, uniqueKeys[(int)axis]);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Not a real issue.")]
        private static void GetUniqueKeys(CancellationToken token, IGroup group, int level, List<HashSet<object>> keys)
        {
            if (group.HasItems && !group.IsBottomLevel)
            {
                int nextLevel = level + 1;
                var keySet = keys[level];
                foreach (IGroup childGroup in group.Items.OfType<IGroup>())
                {
                    token.ThrowIfCancellationRequested();
                    keySet.Add(childGroup.Name);
                    GetUniqueKeys(token, childGroup, nextLevel, keys);
                }
            }
        }

        private static void GenerateEmptyGroups(GroupingFinalizationTaskState finalizationState, List<List<HashSet<object>>> uniqueKeys)
        {
            GroupingResults results = finalizationState.Results;
            GenerateEmptyGroups(finalizationState, uniqueKeys, results.RowRootGroup, DataAxis.Rows, 0);
            GenerateEmptyGroups(finalizationState, uniqueKeys, results.ColumnRootGroup, DataAxis.Columns, 0);
        }

        private static void GenerateEmptyGroups(GroupingFinalizationTaskState finalizationState, List<List<HashSet<object>>> allKeys, Group group, DataAxis axis, int level)
        {
            ParallelState state = finalizationState.ParallelState;
            GroupingResults results = finalizationState.Results;

            state.CancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<GroupDescription> groupDescriptions = axis == DataAxis.Rows ? state.RowGroupDescriptions : state.ColumnGroupDescriptions;

            if (level < groupDescriptions.Count)
            {
                var groupDescription = groupDescriptions[level];
                if (groupDescription.ShowGroupsWithNoData)
                {
                    var groupUniqueKeys = allKeys[(int)axis][level];
                    string memberName = groupDescription.GetMemberName();
                    var allNames = groupDescription.GetAllNames(groupUniqueKeys, ParallelDataEngine.EnumerateParentNames(group, memberName, groupDescriptions, level - 1));
                    if (allNames != null)
                    {
                        foreach (object groupName in allNames)
                        {
                            state.CancellationToken.ThrowIfCancellationRequested();
                            group.CreateGroupByName(groupName, finalizationState.ParallelState.ValueProvider.GetGroupFactory());
                        }
                    }
                }

                if (group.HasItems && !group.IsBottomLevel)
                {
                    state.CancellationToken.ThrowIfCancellationRequested();

                    int nextLevel = level + 1;
                    foreach (Group childGroup in group.Items.OfType<Group>())
                    {
                        GenerateEmptyGroups(finalizationState, allKeys, childGroup, axis, nextLevel);
                    }
                }
            }
        }

        private static void SortGroups(GroupingFinalizationTaskState finalizationState, IAggregateResultProvider resultsProvider)
        {
            ParallelState state = finalizationState.ParallelState;
            GroupingResults results = finalizationState.Results;

            SortGroups(finalizationState, resultsProvider, results.RowRootGroup, DataAxis.Rows, 0);
            SortGroups(finalizationState, resultsProvider, results.ColumnRootGroup, DataAxis.Columns, 0);
        }

        private static void SortGroups(GroupingFinalizationTaskState finalizationState, IAggregateResultProvider resultsProvider, Group group, DataAxis axis, int level)
        {
            ParallelState state = finalizationState.ParallelState;
            GroupingResults results = finalizationState.Results;

            state.CancellationToken.ThrowIfCancellationRequested();

            IReadOnlyList<GroupDescription> groupDescriptions = axis == DataAxis.Rows ? state.RowGroupDescriptions : state.ColumnGroupDescriptions;

            if (level < groupDescriptions.Count)
            {
                var groupDescription = groupDescriptions[level];
                GroupComparer comparer = ((IGroupDescription)groupDescription).GroupComparer;

                if (comparer != null)
                {
                    SortOrder sortOrder = groupDescription.SortOrder;
                    GroupComparerDecorator groupComparer = new GroupComparerDecorator(comparer, sortOrder, resultsProvider, axis);
                    group.SortSubGroups(groupComparer);
                }

                level++;
                if (level < groupDescriptions.Count && !group.IsBottomLevel)
                {
                    for (int i = 0; i < group.Items.Count; i++)
                    {
                        state.CancellationToken.ThrowIfCancellationRequested();
                        SortGroups(finalizationState, resultsProvider, group.Items[i] as Group, axis, level);
                    }
                }
            }
        }

        private static void FilterGroups(GroupingFinalizationTaskState finalizationState, IAggregateResultProvider resultsProvider, ref Dictionary<Coordinate, AggregateValue[]> summaries)
        {
            ParallelState state = finalizationState.ParallelState;
            GroupingResults results = finalizationState.Results;

            state.CancellationToken.ThrowIfCancellationRequested();

            // Filter Groups
            var filteredGroups = new HashSet<IGroup>();
            FilterGroups(finalizationState, resultsProvider, results.RowRootGroup, filteredGroups, DataAxis.Rows, 0);
            FilterGroups(finalizationState, resultsProvider, results.ColumnRootGroup, filteredGroups, DataAxis.Columns, 0);

            // Update the Aggregates if any group have been filtered
            // TODO: try to preserve the filtered aggregates so they could be reused in case the filter is changed...
            if (filteredGroups.Count > 0)
            {
                IDictionary<Coordinate, AggregateValue[]> filteredAggregates = new Dictionary<Coordinate, AggregateValue[]>();

                foreach (var item in results.Aggregates)
                {
                    Coordinate itemCoord = item.Key;
                    if (!filteredGroups.Contains(itemCoord.RowGroup) && !filteredGroups.Contains(itemCoord.ColumnGroup))
                    {
                        filteredAggregates.Add(item);
                    }
                }

                results.Aggregates = filteredAggregates;
                summaries.Clear();

                summaries = GenerateSummaries(finalizationState);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Not a real issue.")]
        private static void FilterGroups(GroupingFinalizationTaskState finalizationState, IAggregateResultProvider resultsProvider, Group group, ICollection<IGroup> filteredGroups, DataAxis axis, int level)
        {
            ParallelState state = finalizationState.ParallelState;
            GroupingResults results = finalizationState.Results;

            if (group.HasItems && !group.IsBottomLevel)
            {
                var groups = group.Items;
                int groupsCount = groups.Count;
                int nextLevel = level + 1;

                var groupDescriptions = axis == DataAxis.Rows ? state.RowGroupDescriptions : state.ColumnGroupDescriptions;
                var groupDescription = groupDescriptions[level];

                GroupFilter groupFilter = groupDescription.GroupFilter;
                SingleGroupFilter singleGroupFilter = groupFilter as SingleGroupFilter;
                ICollection<IGroup> filteredSiblings = null;
                if (groupFilter != null && singleGroupFilter == null)
                {
                    SiblingGroupsFilter siblingGroupFilter = groupFilter as SiblingGroupsFilter;
                    filteredSiblings = siblingGroupFilter.Filter(group.Items, resultsProvider, axis, level);
                }

                for (int i = 0; i < groupsCount; i++)
                {
                    Group childGroup = groups[i] as Group;
                    bool filtered = false;

                    state.CancellationToken.ThrowIfCancellationRequested();

                    if (singleGroupFilter != null)
                    {
                        filtered = !singleGroupFilter.Filter(childGroup, resultsProvider, axis);
                    }
                    else if (filteredSiblings != null)
                    {
                        filtered = !filteredSiblings.Contains(childGroup);
                    }

                    if (!filtered)
                    {
                        if (childGroup.HasItems && !childGroup.IsBottomLevel)
                        {
                            FilterGroups(finalizationState, resultsProvider, childGroup, filteredGroups, axis, nextLevel);
                            if (!(childGroup.HasItems && !childGroup.IsBottomLevel))
                            {
                                filtered = true;
                            }
                        }
                    }

                    if (filtered)
                    {
                        AddChildGroupsToSet(filteredGroups, childGroup);
                        group.RemoveItem(i, null, null);
                        i--;
                        groupsCount--;
                    }
                }
            }
        }

        private static void AddChildGroupsToSet(ICollection<IGroup> filteredGroups, Group group)
        {
            foreach (var item in GetChildrenGroups(group))
            {
                filteredGroups.Add(item);
            }
        }

        private static IEnumerable<Group> GetChildrenGroups(Group group)
        {
            yield return group;
            if (group.HasItems && !group.IsBottomLevel)
            {
                foreach (var childGroup in group.Items.OfType<Group>())
                {
                    foreach (var grandChildGroup in GetChildrenGroups(childGroup))
                    {
                        yield return grandChildGroup;
                    }
                }
            }
        }

        private static Dictionary<Coordinate, AggregateValue[]> GenerateSummaries(GroupingFinalizationTaskState finalizationState)
        {
            ParallelState state = finalizationState.ParallelState;
            GroupingResults results = finalizationState.Results;

            state.CancellationToken.ThrowIfCancellationRequested();
            var valueProvider = state.ValueProvider;

            var aggregates = results.Aggregates;
            return GenerateSummariesImplementation(valueProvider, aggregates);
        }

        private static Dictionary<Coordinate, AggregateValue[]> GenerateSummariesImplementation(IValueProvider valueProvider, IDictionary<Coordinate, AggregateValue[]> aggregates)
        {
            Dictionary<Coordinate, AggregateValue[]> summaries = new Dictionary<Coordinate, AggregateValue[]>();

            Summarize(valueProvider, summaries, DataAxis.Rows, Append(summaries, aggregates));
            Summarize(valueProvider, summaries, DataAxis.Columns, Append(summaries, aggregates));

            return summaries;
        }

        private static void Summarize(IValueProvider valueProvider, Dictionary<Coordinate, AggregateValue[]> summaries, DataAxis axis, IEnumerable<KeyValuePair<Coordinate, AggregateValue[]>> children)
        {
            var aggregateDescriptions = valueProvider.GetAggregateDescriptions();
            IDictionary<Coordinate, AggregateValue[]> parents = new Dictionary<Coordinate, AggregateValue[]>();
            foreach (KeyValuePair<Coordinate, AggregateValue[]> pair in children)
            {
                var childCoordinate = pair.Key;
                if (axis == DataAxis.Rows ? childCoordinate.RowGroup.Parent != null : childCoordinate.ColumnGroup.Parent != null)
                {
                    Coordinate parentCoordinate;
                    if (axis == DataAxis.Rows)
                    {
                        parentCoordinate = new Coordinate(childCoordinate.RowGroup.Parent, childCoordinate.ColumnGroup);
                    }
                    else
                    {
                        parentCoordinate = new Coordinate(childCoordinate.RowGroup, childCoordinate.ColumnGroup.Parent);
                    }

                    IList<AggregateValue> summary = GetOrCreateAggregates(parents, ref parentCoordinate, aggregateDescriptions, valueProvider);
                    for (int i = 0; i < aggregateDescriptions.Count; i++)
                    {
                        AggregateValue parentAggregate = summary[i];
                        AggregateValue childAggregate = pair.Value[i];
                        parentAggregate.MergeCore(childAggregate);
                    }
                }
            }

            foreach (KeyValuePair<Coordinate, AggregateValue[]> pair in parents)
            {
                summaries.Add(pair.Key, pair.Value);
            }

            if (Enumerable.Any(parents))
            {
                Summarize(valueProvider, summaries, axis, parents);
            }
        }

        private static AggregateValue[] GetOrCreateAggregates(IDictionary<Coordinate, AggregateValue[]> dictionary, ref Coordinate coordinate, IReadOnlyList<IAggregateDescription> aggregateDescriptions, IValueProvider valueProvider)
        {
            AggregateValue[] summary;
            if (!dictionary.TryGetValue(coordinate, out summary))
            {
                summary = CreateAggregates(dictionary, ref coordinate, aggregateDescriptions, valueProvider);
            }

            return summary;
        }

        private static AggregateValue[] CreateAggregates(IDictionary<Coordinate, AggregateValue[]> dictionary, ref Coordinate coordinate, IReadOnlyList<IAggregateDescription> aggregateDescriptions, IValueProvider valueProvider)
        {
            int aggregatesCount = aggregateDescriptions.Count;
            AggregateValue[] summary = new AggregateValue[aggregatesCount];
            for (int a = 0; a < aggregatesCount; a++)
            {
                IAggregateDescription aggregate = aggregateDescriptions[a];
                summary[a] = valueProvider.CreateAggregateValue(a);
            }

            dictionary[coordinate] = summary;
            return summary;
        }

        private static IEnumerable<object> EnumerateParentNames(IGroup group, string memberName, IReadOnlyList<GroupDescription> groupDescriptions, int level)
        {
            while (group != null && level >= 0)
            {
                GroupDescription groupDescription = groupDescriptions[level];
                string currentMemberName = groupDescription.GetMemberName();
                if (currentMemberName == memberName)
                {
                    yield return group.Name;
                }

                group = group.Parent;
                level--;
            }
        }

        private static IEnumerable<T> Append<T>(params IEnumerable<T>[] enumerables)
        {
            foreach (var enumerable in enumerables)
            {
                foreach (var item in enumerable)
                {
                    yield return item;
                }
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design choice.")]
        private static GroupingResults ProcessItems(object state)
        {
            BottomLevelGroupingTaskState taskState = (BottomLevelGroupingTaskState)state;
            ParallelState parallelState = taskState.ParallelState;

            int start = taskState.Start;
            int end = taskState.End;

            IGroupFactory groupFactory = parallelState.ValueProvider.GetGroupFactory();

            Group rowRootGroup = CreateGrandTotal(groupFactory);
            Group columnRootGroup = CreateGrandTotal(groupFactory);

            IDictionary<Coordinate, AggregateValue[]> aggregates = new Dictionary<Coordinate, AggregateValue[]>();

            ////int filtersCount = parallelState.ValueProvider.GetFiltersCount();
            ////HashSet<object>[] uniqueFilterItems = new HashSet<object>[filtersCount];
            ////for (int i = 0; i < filtersCount; i++)
            ////{
            ////    uniqueFilterItems[i] = new HashSet<object>();
            ////}

            var cancellationToken = parallelState.CancellationToken;
            for (int index = start; index < end; index++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                object fact = parallelState.GetItem(index);

                ////object[] filterItems = parallelState.ValueProvider.GetFilterItems(fact);

                ////for (int i = 0; i < uniqueFilterItems.Length; i++)
                ////{
                ////    uniqueFilterItems[i].Add(filterItems[i]);
                ////}

                ////bool passesFilter = parallelState.ValueProvider.PassesFilter(filterItems);

                ////if (passesFilter)
                ////{
                //// IAggregateResultProvider is null because we will sort groups later.

                var coordinate = AddItem(parallelState, rowRootGroup, columnRootGroup, index);
                UpdateAggregates(parallelState.ValueProvider, aggregates, fact, ref coordinate);
                ////}
            }

            return new GroupingResults(rowRootGroup, columnRootGroup, groupFactory) { Aggregates = aggregates };
        }

        private static Coordinate AddItem(ParallelState parallelState, Group rowRootGroup, Group columnRootGroup, int index)
        {
            var valueProvider = parallelState.ValueProvider;
            var item = parallelState.GetItem(index);
            var groupFactory = valueProvider.GetGroupFactory();

            Group rowGroup = rowRootGroup;

            List<object> rowGroupNames = new List<object>();

            foreach (var groupName in valueProvider.GetRowGroupNames(item))
            {
                rowGroupNames.Add(groupName);
            }

            if (parallelState.DataView.SourceGroups.Count > 0)
            {
                rowGroupNames.Insert(0, parallelState.GetGroupFromIndex(index));
            }

            foreach (var groupName in rowGroupNames)
            {
                rowGroup = rowGroup.CreateGroupByName(groupName, groupFactory);
            }

            rowGroup.InsertItem(-1, item, null);

            // In one dimension grouping (e.g. not pivot) there is no need to store all the items (this reduce memory footprint).
            bool hasColumnDescriptions = false;
            Group columnGroup = columnRootGroup;

            var columnGroupNames = valueProvider.GetColumnGroupNames(item);
            foreach (var groupName in columnGroupNames)
            {
                hasColumnDescriptions = true;
                columnGroup = columnGroup.CreateGroupByName(groupName, groupFactory);
            }

            if (hasColumnDescriptions)
            {
                columnGroup.InsertItem(-1, item, null);
            }

            return new Coordinate(rowGroup, columnGroup);
        }

        private static AddRemoveItemResult AddItem(IAggregateResultProvider aggregateResultProvider, IValueProvider valueProvider, Group rowRootGroup, Group columnRootGroup, object item, int index)
        {
            AddRemoveItemResult result = new AddRemoveItemResult();
            IGroupFactory groupFactory = valueProvider.GetGroupFactory();

            // The first item is changed group and the second one is the new group (if any).
            var sortComparer = valueProvider.GetSortComparer();
            var rowGroupNames = valueProvider.GetRowGroupNames(item);

            Group aggregateRowGroup = rowRootGroup;
            Group changedRowGroup = rowRootGroup;
            Group addedRowGroup = null;

            int rowLevel = 0;
            foreach (var groupName in rowGroupNames)
            {
                Group group;
                if (aggregateRowGroup.TryGetGroup(groupName, out group))
                {
                    aggregateRowGroup = group;
                    changedRowGroup = group;
                }
                else
                {
                    var tuple = valueProvider.GetRowGroupNameComparerAndSortOrder(rowLevel);
                    var groupNameComparer = CreateGroupComparer(aggregateResultProvider, tuple, DataAxis.Rows);
                    int groupIndex = aggregateRowGroup.AddGroupByName(groupName, groupFactory, groupNameComparer);
                    aggregateRowGroup = (Group)aggregateRowGroup.Items[groupIndex];

                    // We get the first new group only so that we pass it as NewItem.
                    // Layout will enumerate all items of NewItem (if it is Group) recursively so we don't pass sub groups.
                    if (addedRowGroup == null)
                    {
                        // Take only the first new group and index.
                        addedRowGroup = aggregateRowGroup;
                        result.AddRemoveRowGroupIndex = groupIndex;
                    }
                }
                rowLevel++;
            }

            if (aggregateRowGroup.IsBottomLevel)
            {
                int itemIndex = aggregateRowGroup.InsertItem(index, item, sortComparer);

                // If we didn't create new group then we take the index of the item.
                // Else we use the index of the newly created group.
                if (addedRowGroup == null)
                {
                    result.AddRemoveRowGroupIndex = itemIndex;
                }

                result.ItemWasAddedOrRemoved = true;
            }

            // In one dimension grouping (e.g. not pivot) there is no need to store all the items (this reduce memory footprint).
            bool hasColumnDescriptions = false;
            Group aggregateColumnGroup = columnRootGroup;
            Group changedColumnGroup = columnRootGroup;
            Group addedColumnGroup = null;

            var columnGroupNames = valueProvider.GetColumnGroupNames(item);

            int columnLevel = 0;
            foreach (var groupName in columnGroupNames)
            {
                hasColumnDescriptions = true;

                Group group;
                if (aggregateColumnGroup.TryGetGroup(groupName, out group))
                {
                    aggregateColumnGroup = group;
                    changedColumnGroup = group;
                }
                else
                {
                    var tuple = valueProvider.GetColumnGroupNameComparerAndSortOrder(columnLevel);
                    var groupNameComparer = CreateGroupComparer(aggregateResultProvider, tuple, DataAxis.Columns);

                    int groupIndex = aggregateColumnGroup.AddGroupByName(groupName, groupFactory, groupNameComparer);
                    aggregateColumnGroup = (Group)aggregateColumnGroup.Items[groupIndex];

                    // We get the first new group only so that we pass it as NewItem.
                    // Layout will enumerate all items of NewItem (if it is Group) recursively so we don't pass sub groups.
                    if (addedColumnGroup == null)
                    {
                        addedColumnGroup = aggregateColumnGroup;
                        result.AddRemoveColumnGroupIndex = groupIndex;
                    }
                }
                columnLevel++;
            }

            if (hasColumnDescriptions && aggregateColumnGroup.IsBottomLevel)
            {
                int itemIndex = aggregateColumnGroup.InsertItem(index, item, sortComparer);

                // If we didn't create new group then we take the index of the item.
                // Else we use the index of the newly created group.
                if (addedColumnGroup == null)
                {
                    result.AddRemoveColumnGroupIndex = itemIndex;
                }

                result.ItemWasAddedOrRemoved = true;
            }

            result.AggregateCoordinate = new Coordinate(aggregateRowGroup, aggregateColumnGroup);
            result.ChangedCoordinate = new Coordinate(changedRowGroup, changedColumnGroup);
            result.AddedOrRemovedCoordinate = new Coordinate(addedRowGroup, addedColumnGroup);

            return result;
        }

        private static IComparer<object> CreateGroupComparer(IAggregateResultProvider aggregateResultProvider, Tuple<GroupComparer, SortOrder> tuple, DataAxis axis)
        {
            var groupNameComparer = tuple.Item1;
            if (groupNameComparer != null && aggregateResultProvider != null)
            {
                SortOrder sortOrder = tuple.Item2;
                return new GroupComparerDecorator(groupNameComparer, sortOrder, aggregateResultProvider, axis);
            }

            return null;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void RebuildAggregates(IValueProvider valueProvider, IDictionary<Coordinate, AggregateValue[]> aggregates, ref Coordinate coord)
        {
            var aggregateDescriptions = valueProvider.GetAggregateDescriptions();

            // When there are not aggregates than skip aggregates dictionary (this reduce memory footprint).
            if (aggregateDescriptions.Count > 0)
            {
                var items = GetItems(ref coord);
                bool hasItems = items.Any();

                if (hasItems)
                {
                    var summary = CreateAggregates(aggregates, ref coord, aggregateDescriptions, valueProvider);
                    for (int i = 0; i < summary.Length; i++)
                    {
                        object value = null;
                        object exception = null;

                        foreach (var item in items)
                        {
                            try
                            {
                                value = valueProvider.GetAggregateValue(i, item);
                            }
                            catch (Exception e)
                            {
                                exception = e;
                            }

                            if (exception == null)
                            {
                                summary[i].AccumulateCore(value);
                            }
                            else
                            {
                                summary[i].RaiseError();

                                // If there is an error then skip other items. 
                                break;
                            }
                        }
                    }
                }
                else
                {
                    aggregates.Remove(coord);
                }
            }
        }

        private static IEnumerable<object> GetItems(ref Coordinate coord)
        {
            bool hasRowItems = coord.RowGroup != null && coord.RowGroup.HasItems;
            bool hasColumnItems = coord.ColumnGroup != null && coord.ColumnGroup.HasItems;

            if (!hasRowItems && !hasColumnItems)
            {
                return Enumerable.Empty<object>();
            }
            else if (!hasColumnItems)
            {
                return coord.RowGroup.Items;
            }
            else if (!hasRowItems)
            {
                return coord.ColumnGroup.Items;
            }
            else
            {
                HashSet<object> items = new HashSet<object>(coord.RowGroup.Items);
                items.IntersectWith(coord.ColumnGroup.Items);
                return items;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void UpdateAggregates(IValueProvider valueProvider, IDictionary<Coordinate, AggregateValue[]> aggregates, object fact, ref Coordinate coord)
        {
            var aggregateDescriptions = valueProvider.GetAggregateDescriptions();

            // When there are not aggregates than skip aggregates dictionary (this reduce memory footprint).
            if (aggregateDescriptions.Count > 0)
            {
                var summary = GetOrCreateAggregates(aggregates, ref coord, aggregateDescriptions, valueProvider);
                for (int i = 0; i < summary.Length; i++)
                {
                    object value = null;
                    object exception = null;

                    try
                    {
                        value = valueProvider.GetAggregateValue(i, fact);
                    }
                    catch (Exception e)
                    {
                        exception = e;
                    }

                    if (exception == null)
                    {
                        summary[i].AccumulateCore(value);
                    }
                    else
                    {
                        summary[i].RaiseError();
                    }
                }
            }
        }

        private static AddRemoveItemResult RemoveItem(IAggregateResultProvider aggregateResultProvider, IValueProvider valueProvider, Group rowRootGroup, Group columnRootGroup, object item, int index, bool exhaustiveSearch, bool canUseComparer)
        {
            // NOTE: removeFilteredItem means this item has a property changed so we need to do exhaustive search to find it and remove it.
            // If we need better/faster result that we can index all items and their properties in initial processing.
            // canUseComparer == false means that property that we have sorted on has changed so we cannot use comparer for fast search.
            AddRemoveItemResult result = new AddRemoveItemResult();

            Group aggregateRowGroup = rowRootGroup;
            Group changedRowGroup = null;
            Group removedRowGroup = null;

            var itemComparer = valueProvider.GetSortComparer();

            Tuple<Group, int> searchResult = FindGroupAndItemIndex(valueProvider, rowRootGroup, item, index, exhaustiveSearch, canUseComparer);
            aggregateRowGroup = searchResult.Item1;
            index = searchResult.Item2;

            // We should remove items only from bottom level groups.
            if (aggregateRowGroup != null && aggregateRowGroup.IsBottomLevel)
            {
                // ChangedGroup is the topMost group that was changed because of this remove operation
                // or null when there was no change.
                int removedItemIndex = -1;

                changedRowGroup = aggregateRowGroup;
                removedRowGroup = null;

                if (exhaustiveSearch)
                {
                    // We have found the item index so no need to search it again.
                    itemComparer = null;
                }

                object itemReference = item;
                while (changedRowGroup != null)
                {
                    removedItemIndex = changedRowGroup.RemoveItem(index, itemReference, itemComparer);
                    if (removedItemIndex >= 0)
                    {
                        result.ItemWasAddedOrRemoved = true;
                        result.AddRemoveRowGroupIndex = removedItemIndex;
                    }
                    else
                    {
                        changedRowGroup = null;
                        result.AddRemoveRowGroupIndex = -1;
                    }

                    if (changedRowGroup != null && !changedRowGroup.HasItems && changedRowGroup.HasParent)
                    {
                        itemReference = changedRowGroup;
                        removedRowGroup = changedRowGroup;
                        changedRowGroup = changedRowGroup.InternalParent;

                        var tuple = valueProvider.GetRowGroupNameComparerAndSortOrder(changedRowGroup.Level);
                        itemComparer = CreateGroupComparer(aggregateResultProvider, tuple, DataAxis.Rows);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            Group aggregateColumnGroup = columnRootGroup;
            Group changedColumnGroup = null;
            Group removedColumnGroup = null;

            var columnGroupNames = valueProvider.GetColumnGroupNames(item);

            searchResult = FindGroupAndItemIndex(valueProvider, columnRootGroup, item, index, exhaustiveSearch, canUseComparer);
            aggregateColumnGroup = searchResult.Item1;
            index = searchResult.Item2;

            // We should remove items only from bottom level groups.
            if (aggregateColumnGroup != null && aggregateColumnGroup.IsBottomLevel)
            {
                int removedItemIndex = -1;
                changedColumnGroup = aggregateColumnGroup;
                removedColumnGroup = null;

                if (exhaustiveSearch)
                {
                    // We have found the item index so no need to search it again.
                    itemComparer = null;
                }

                object itemReference = item;
                while (changedColumnGroup != null)
                {
                    removedItemIndex = changedColumnGroup.RemoveItem(index, itemReference, itemComparer);

                    if (removedItemIndex >= 0)
                    {
                        result.ItemWasAddedOrRemoved = true;
                    }
                    else
                    {
                        changedColumnGroup = null;
                    }

                    if (changedColumnGroup != null && !changedColumnGroup.HasItems && changedColumnGroup.HasParent)
                    {
                        result.AddRemoveColumnGroupIndex = removedItemIndex;
                        result.ItemWasAddedOrRemoved = true;
                        itemReference = changedColumnGroup;
                        removedColumnGroup = changedColumnGroup;
                        changedColumnGroup = changedColumnGroup.InternalParent;
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (aggregateColumnGroup == null)
            {
                aggregateColumnGroup = columnRootGroup;
            }

            if (aggregateRowGroup == null)
            {
                aggregateRowGroup = rowRootGroup;
            }

            result.AggregateCoordinate = new Coordinate(aggregateRowGroup, aggregateColumnGroup);
            result.ChangedCoordinate = new Coordinate(changedRowGroup, changedColumnGroup);
            result.AddedOrRemovedCoordinate = new Coordinate(removedRowGroup, removedColumnGroup);

            return result;
        }

        private static Tuple<Group, int> FindGroupAndItemIndex(IValueProvider valueProvider, Group rowRootGroup, object item, int index, bool exhaustiveSearch, bool canUseComparer)
        {
            if (exhaustiveSearch)
            {
                return DepthFirstSearch(rowRootGroup, item, canUseComparer ? valueProvider.GetSortComparer() : null);
            }
            else
            {
                var aggregateRowGroup = rowRootGroup;
                var rowGroupNames = valueProvider.GetRowGroupNames(item);
                foreach (var groupName in rowGroupNames)
                {
                    Group group;
                    if (aggregateRowGroup.TryGetGroup(groupName, out group))
                    {
                        aggregateRowGroup = group;
                    }
                }

                index = aggregateRowGroup.IndexOf(item, valueProvider.GetSortComparer());

                return Tuple.Create(aggregateRowGroup, index);
            }
        }

        private List<AddRemoveResult> Process(int index, System.Collections.IEnumerable items, bool remove, bool removeFilteredItem, bool canUseComparer)
        {
            List<AddRemoveResult> changes = new List<AddRemoveResult>();

            var aggregateDescriptions = this.valueProvider.GetAggregateDescriptions();
            bool hasFormattedTotals = aggregateDescriptions.Any(ag => ag.TotalFormat != null);
            bool aggregatesChanged = false;

            bool hasFilterDescriptions = this.valueProvider.GetFiltersCount() > 0;

            var rowRootGroup = this.Root.RowGroup as Group;
            var columnRootGroup = this.Root.ColumnGroup as Group;

            int currentIndex = index;
            int itemIndex;

            foreach (var item in items)
            {
                object[] filterItems = this.UpdateUniqueFilterItems(remove, item);

                // NOTE: If using the same method for PropertyChanged based on Filtered property then we need different logic here.
                bool passesFilter = this.valueProvider.PassesFilter(filterItems);
                if (passesFilter || removeFilteredItem)
                {
                    Coordinate aggregateCoordinate = new Coordinate();

                    if (remove)
                    {
                        // NOTE: This is not complete implementation.
                        // We should save groups that are filtered from GroupFilter and update them when we add/remove items.
                        // We could also rebuild only summaries that are affected (not all) but this will require all modified not bottom level groups.
                        // When we have filter we don't know the real index so we use -1 which means list.Remove(item) - linear search.
                        itemIndex = hasFilterDescriptions ? -1 : currentIndex;

                        var result = RemoveItem(this, this.valueProvider, rowRootGroup, columnRootGroup, item, itemIndex, removeFilteredItem, canUseComparer);
                        if (result.ItemWasAddedOrRemoved)
                        {
                            aggregateCoordinate = result.AggregateCoordinate;
                            changes.Add(new AddRemoveResult()
                            {
                                ChangedCoordinate = result.ChangedCoordinate,
                                AddedOrRemovedCoordinate = result.AddedOrRemovedCoordinate,
                                AddRemoveRowGroupIndex = result.AddRemoveRowGroupIndex,
                                AddRemoveColumnGroupIndex = result.AddRemoveColumnGroupIndex
                            });
                            aggregatesChanged = true;
                        }
                    }
                    else
                    {
                        itemIndex = currentIndex;
                        var result = AddItem(this, this.valueProvider, rowRootGroup, columnRootGroup, item, itemIndex);
                        currentIndex++;
                        if (result.ItemWasAddedOrRemoved)
                        {
                            aggregateCoordinate = result.AggregateCoordinate;
                            changes.Add(new AddRemoveResult()
                            {
                                ChangedCoordinate = result.ChangedCoordinate,
                                AddedOrRemovedCoordinate = result.AddedOrRemovedCoordinate,
                                AddRemoveRowGroupIndex = result.AddRemoveRowGroupIndex,
                                AddRemoveColumnGroupIndex = result.AddRemoveColumnGroupIndex
                            });
                            aggregatesChanged = true;
                        }
                    }

                    if (aggregateDescriptions.Count > 0 && aggregatesChanged)
                    {
                        RebuildAggregates(this.valueProvider, this.aggregates, ref aggregateCoordinate);
                    }
                }
            }

            if (aggregateDescriptions.Count > 0 && aggregatesChanged)
            {
                this.summaries = GenerateSummariesImplementation(this.valueProvider, this.aggregates);
            }

            if (hasFormattedTotals && aggregatesChanged)
            {
                this.formattedTotals = GenerateFormattedTotals(this, this.valueProvider.GetAggregateDescriptions());
            }

            return changes;
        }

        private object[] UpdateUniqueFilterItems(bool remove, object item)
        {
            int filtersCount = this.valueProvider.GetFiltersCount();
            object[] filterItems = this.valueProvider.GetFilterItems(item);

            for (int j = 0; j < filtersCount; j++)
            {
                if (remove)
                {
                    this.RemoveUniqueFilterItems(j, filterItems[j]);
                }
                else
                {
                    this.AddUniqueFilterItems(j, filterItems[j]);
                }
            }
            return filterItems;
        }

        private void CancelCurrentProcessing(ParallelState state)
        {
            lock (locker)
            {
                if (this.initalState != null && this.initalState.CancellationTokenSource != null)
                {
                    this.initalState.CancellationTokenSource.Cancel();
                }

                this.initalState = null;

                // Clear
                this.aggregates.Clear();
                this.summaries.Clear();
                this.formattedTotals.Clear();

                this.uniqueGroupKeys = null;
                this.uniqueFilterItems = null;
                var groupFactory = state.ValueProvider.GetGroupFactory();
                this.Root = new Coordinate(CreateGrandTotal(groupFactory), CreateGrandTotal(groupFactory));

                this.valueProvider = null;
                this.RowGroupDescriptions = new ReadOnlyList<GroupDescription, GroupDescription>(new List<GroupDescription>());
                this.ColumnGroupDescriptions = new ReadOnlyList<GroupDescription, GroupDescription>(new List<GroupDescription>());
                this.AggregateDescriptions = new ReadOnlyList<IAggregateDescription, IAggregateDescription>(new List<IAggregateDescription>());
                this.FilterDescriptions = new ReadOnlyList<FilterDescription, FilterDescription>(new List<FilterDescription>());
            }
        }

        private void RaiseInProgress()
        {
            this.RaiseCompleted(new DataEngineCompletedEventArgs(new ReadOnlyCollection<Exception>(new List<Exception>()), DataEngineStatus.InProgress));
        }

        private void RaiseCompleted(DataEngineCompletedEventArgs args)
        {
            this.currentResultTask = null;
            if (this.Completed != null)
            {
                this.Completed(this, args);
            }
        }

        private void FinalizeAggregations(GroupingFinalizationTaskState finalizationTasksState)
        {
            var localUniqueGroupKeys = GenerateUniqueKeys(finalizationTasksState);
            GenerateEmptyGroups(finalizationTasksState, localUniqueGroupKeys);
            GenerateAllKeys(finalizationTasksState, localUniqueGroupKeys);

            var allKeys = localUniqueGroupKeys;

            // GroupingFinalizationTaskState -> should be AggregationTaskState and SummarizationTaskState should be introduced that has the resultsProvider below:
            var localSummaries = GenerateSummaries(finalizationTasksState);

            GroupingResults finalResult = finalizationTasksState.Results;
            AggregateResultProvider resultsProvider = new AggregateResultProvider()
            {
                Aggregates = finalResult.Aggregates,
                Summaries = localSummaries,
                Root = finalizationTasksState.Results.Root
            };

            SortGroups(finalizationTasksState, resultsProvider);
            FilterGroups(finalizationTasksState, resultsProvider, ref localSummaries);

            var formatTotals = GenerateFormattedTotals(finalizationTasksState, resultsProvider);
            ApplyStringFormats(finalizationTasksState, resultsProvider, formatTotals);

            // TODO: Group dependant summary computations
            this.aggregates = finalResult.Aggregates;
            this.summaries = localSummaries;
            this.formattedTotals = formatTotals;

            this.uniqueGroupKeys = allKeys;
            this.uniqueFilterItems = finalResult.UniqueFilterItems;
            this.Root = finalResult.Root;

            this.valueProvider = finalizationTasksState.ParallelState.ValueProvider;
            this.AggregateDescriptions = finalizationTasksState.ParallelState.AggregateDescriptions;
            this.RowGroupDescriptions = finalizationTasksState.ParallelState.RowGroupDescriptions;
            this.ColumnGroupDescriptions = finalizationTasksState.ParallelState.ColumnGroupDescriptions;
            this.FilterDescriptions = finalizationTasksState.ParallelState.FilterDescriptions;
        }

        private class AddRemoveItemResult
        {
            public int AddRemoveRowGroupIndex;
            public int AddRemoveColumnGroupIndex;
            public bool ItemWasAddedOrRemoved;

            public Coordinate ChangedCoordinate;
            public Coordinate AddedOrRemovedCoordinate;
            public Coordinate AggregateCoordinate;
        }

        private class CountAndLastArrayComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[] x, object[] y)
            {
                if (x == null && y == null)
                {
                    return true;
                }
                else if (x == null || y == null)
                {
                    return false;
                }

                if (x.Length != y.Length)
                {
                    return false;
                }

                if (x.Length > 0)
                {
                    int l = x.Length - 1;

                    if (!object.Equals(x[l], y[l]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(object[] obj)
            {
                if (obj == null)
                {
                    return 0;
                }
                int hash = obj.Length;

                if (obj.Length > 0)
                {
                    hash = (hash * 8831) + obj[obj.Length - 1].GetHashCode();
                }

                return hash;
            }
        }

        private class ObjectArrayComparer : IEqualityComparer<object[]>
        {
            public bool Equals(object[] x, object[] y)
            {
                if (x == null && y == null)
                {
                    return true;
                }
                else if (x == null || y == null)
                {
                    return false;
                }

                if (x.Length != y.Length)
                {
                    return false;
                }

                for (int i = 0; i < x.Length; i++)
                {
                    if (!object.Equals(x[0], y[0]))
                    {
                        return false;
                    }
                }

                return true;
            }

            public int GetHashCode(object[] obj)
            {
                if (obj == null)
                {
                    return 0;
                }

                int hash = obj.Length * 8831;
                for (int i = 0; i < obj.Length; i++)
                {
                    hash = (hash * 8831) + obj[i].GetHashCode();
                }

                return hash;
            }
        }

        private class BottomLevelGroupingTaskState
        {
            internal int Start { get; set; }

            internal int End { get; set; }

            internal ParallelState ParallelState { get; set; }
        }

        private class GroupingFinalizationTaskState
        {
            internal GroupingResults Results { get; set; }

            internal ParallelState ParallelState { get; set; }
        }

        private class AggregateResultProvider : IAggregateResultProvider
        {
            public Coordinate Root { get; internal set; }

            internal IDictionary<Coordinate, AggregateValue[]> Aggregates { get; set; }

            internal IDictionary<Coordinate, AggregateValue[]> Summaries { get; set; }

            public AggregateValue GetAggregateResult(int aggregate, Coordinate groups)
            {
                var values = this.GetAggregateResults(groups);

                if (values == null || aggregate < 0 || aggregate >= values.Length)
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
                else if (this.Summaries.TryGetValue(coordinate, out results))
                {
                    return results;
                }

                return null;
            }
        }

        private class GroupingResults
        {
            internal GroupingResults(Group rowRoot, Group columnRoot, IGroupFactory groupFactory)
            {
                this.RowRootGroup = rowRoot;
                this.ColumnRootGroup = columnRoot;
                this.Root = new Coordinate(this.RowRootGroup, this.ColumnRootGroup);

                this.GroupFactory = groupFactory;
            }

            public IGroupFactory GroupFactory { get; private set; }

            public Coordinate Root { get; private set; }

            public IDictionary<Coordinate, AggregateValue[]> Aggregates { get; internal set; }

            public HashSet<object>[] UniqueFilterItems { get; set; }

            internal Group RowRootGroup { get; private set; }

            internal Group ColumnRootGroup { get; private set; }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Not called for NET20 only.")]
            internal void Merge(GroupingResults result)
            {
                foreach (var pair in result.Aggregates)
                {
                    AggregateValue[] parentAggregates;
                    if (this.Aggregates.TryGetValue(pair.Key, out parentAggregates))
                    {
                        for (int i = 0; i < parentAggregates.Length; i++)
                        {
                            AggregateValue parentAggregate = parentAggregates[i];
                            AggregateValue childAggregate = pair.Value[i];
                            parentAggregate.MergeCore(childAggregate);
                        }
                    }
                    else
                    {
                        this.Aggregates.Add(pair);
                    }
                }

                ////for (int i = 0; i < this.UniqueFilterItems.Length; i++)
                ////{
                ////    var sourceUniqueItems = this.UniqueFilterItems[i];
                ////    var mergeUniqueItems = result.UniqueFilterItems[i];

                ////    foreach (var item in mergeUniqueItems)
                ////    {
                ////        sourceUniqueItems.Add(item);
                ////    }
                ////}

                this.MergeChildGroups(this.RowRootGroup, result.RowRootGroup);
                this.MergeChildGroups(this.ColumnRootGroup, result.ColumnRootGroup);
            }

            [SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Not called for NET20 only.")]
            private void MergeChildGroups(Group group, Group group2)
            {
                if (group2.HasItems)
                {
                    foreach (var subItem in group2.Items)
                    {
                        Group childGroup = subItem as Group;
                        if (childGroup != null)
                        {
                            Group rootChildGroup = group.CreateGroupByName(childGroup.Name, this.GroupFactory);
                            this.MergeChildGroups(rootChildGroup, childGroup);
                        }
                        else
                        {
                            group.InsertItem(-1, subItem, null);
                        }
                    }
                }
            }
        }
    }
}
