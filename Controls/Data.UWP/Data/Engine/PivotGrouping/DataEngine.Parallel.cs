#if NETFX2 || NETFX35
#define LEGACY
#endif

#if !LEGACY
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Threading.Tasks;

namespace Telerik.Data.Core.Engine
{
    internal partial class ParallelDataEngine
    {
        private static object locker = new object();

        private Task currentResultTask;
        private ParallelState initalState;

        private void BeginParallelProcessing(ParallelState parallelInitalState)
        {
            this.initalState = parallelInitalState;

            this.currentResultTask = Task.Factory
                .StartNew(() => GenerateBottomLevelsFromSourceParallel(parallelInitalState), parallelInitalState.CancellationToken, TaskCreationOptions.LongRunning, parallelInitalState.TaskScheduler)
                .ContinueWith((task) => this.ProcessBottomLevelsParallel(task, parallelInitalState), TaskContinuationOptions.AttachedToParent);
        }

        private static GroupingResults GenerateBottomLevelsFromSourceParallel(ParallelState state)
        {
            HashSet<object>[] uniqueFilterItems;
            List<object> items;
            SortAndFilterItems(state, out uniqueFilterItems, out items);

            if (!state.HasDescriptions)
            {
                IGroupFactory groupFactory = state.ValueProvider.GetGroupFactory();
                Group rowRootGroup = CreateGrandTotal(groupFactory);
                rowRootGroup.SetItems(items);
                Group columnRootGroup = CreateGrandTotal(groupFactory);
                IDictionary<Coordinate, AggregateValue[]> aggregates = new Dictionary<Coordinate, AggregateValue[]>();
                return new GroupingResults(rowRootGroup, columnRootGroup, groupFactory) { Aggregates = aggregates, UniqueFilterItems = uniqueFilterItems };
            }

            int maxDegreeOfParallelism = Math.Max(1, state.MaxDegreeOfParallelism);
            int itemCount = state.ItemCount;
            int remainder = itemCount % maxDegreeOfParallelism;
            int multiplier = (itemCount / maxDegreeOfParallelism) + (remainder > 0 ? 1 : 0);

            List<Task<GroupingResults>> tasks = new List<Task<GroupingResults>>(maxDegreeOfParallelism);

            for (int i = 0; i < maxDegreeOfParallelism; i++)
            {
                int start = i * multiplier;
                int end = Math.Min((i + 1) * multiplier, itemCount);

                BottomLevelGroupingTaskState taskState = new BottomLevelGroupingTaskState() { ParallelState = state, Start = start, End = end };
                var task = Task.Factory.StartNew<GroupingResults>(ProcessItems, taskState, state.CancellationToken, TaskCreationOptions.LongRunning | TaskCreationOptions.AttachedToParent, state.TaskScheduler);

                tasks.Add(task);
            }

            Task.WaitAll(tasks.ToArray());
            GroupingResults result = tasks[0].Result;
            for (int i = 1; i < tasks.Count; i++)
            {
                result.Merge(tasks[i].Result);
            }

            result.UniqueFilterItems = uniqueFilterItems;
            return result;
        }

        private static void SortAndFilterItems(ParallelState state, out HashSet<object>[] uniqueFilterItems, out List<object> items)
        {
            int filtersCount = state.ValueProvider.GetFiltersCount();
            uniqueFilterItems = new HashSet<object>[filtersCount];
            for (int i = 0; i < filtersCount; i++)
            {
                uniqueFilterItems[i] = new HashSet<object>();
            }

            if (filtersCount > 0)
            {
                items = new List<object>();

                if (state.DataView.SourceGroups.Count > 0)
                {
                    foreach (var group in state.DataView.SourceGroups)
                    {
                        List<object> filteredGroupedItems = new List<object>();

                        for (int g = group.Item2; g < group.Item3; g++)
                        {
                            object[] filterItems = state.ValueProvider.GetFilterItems(state.DataView.InternalList[g]);
                            for (int i = 0; i < uniqueFilterItems.Length; i++)
                            {
                                uniqueFilterItems[i].Add(filterItems[i]);
                            }

                            bool passesFilter = state.ValueProvider.PassesFilter(filterItems);
                            if (passesFilter)
                            {
                                filteredGroupedItems.Add(state.DataView.InternalList[g]);
                            }
                        }

                        var sortComparer = state.ValueProvider.GetSortComparer();
                        if (sortComparer != null)
                        {
                            ParallelAlgorithms.Sort(filteredGroupedItems, sortComparer);
                        }

                        items.AddRange(filteredGroupedItems);

                        state.SourceGroups.Add(new Tuple<object, int>(group.Item1, items.Count));
                    }
                }
                else
                {
                    foreach (var item in state.DataView.InternalList)
                    {
                        object[] filterItems = state.ValueProvider.GetFilterItems(item);
                        for (int i = 0; i < uniqueFilterItems.Length; i++)
                        {
                            uniqueFilterItems[i].Add(filterItems[i]);
                        }

                        bool passesFilter = state.ValueProvider.PassesFilter(filterItems);
                        if (passesFilter)
                        {
                            items.Add(item);
                        }
                    }

                    var sortComparer = state.ValueProvider.GetSortComparer();
                    if (sortComparer != null)
                    {
                        ParallelAlgorithms.Sort(items, sortComparer);
                    }
                }
            }
            else
            {
                items = new List<object>();

                if (state.DataView.SourceGroups.Count > 0)
                {
                    List<object> groupedItems = new List<object>();

                    foreach (var group in state.DataView.SourceGroups)
                    {      
                        groupedItems = new List<object>();
                        for (int g = group.Item2; g < group.Item3; g++)
                        {
                            groupedItems.Add(state.DataView.InternalList[g]);

                        }

                        var sortComparer = state.ValueProvider.GetSortComparer();
                        if (sortComparer != null)
                        {
                            ParallelAlgorithms.Sort(groupedItems, sortComparer);
                        }
                        items.AddRange(groupedItems);


                        state.SourceGroups.Add(new Tuple<object, int>(group.Item1, items.Count));
                    }
                }
                else
                {
                    items = new List<object>(state.DataView.InternalList);

                    var sortComparer = state.ValueProvider.GetSortComparer();
                    if (sortComparer != null)
                    {
                        ParallelAlgorithms.Sort(items, sortComparer);
                    }
                }
            }

            state.Items = items;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Scope = "member", Target = "Telerik.Data.Core.Engine.ParallelDataEngine.#ProcessBottomLevelsParallel(System.Threading.Tasks.Task`1<Telerik.Data.Core.Engine.ParallelDataEngine+GroupingResults>,Telerik.Data.Core.Engine.ParallelState)", Justification = "Catching all excpetions from different threads.")]
        private void ProcessBottomLevelsParallel(Task<GroupingResults> bottomLevelResultsTask, ParallelState parallelState)
        {
            AggregateException exception = null;
            var status = DataEngineStatus.Faulted;

            try
            {
                
                lock (locker)
                {
                    if (this.initalState == parallelState)
                    {
                        var finalizationTasksState = new GroupingFinalizationTaskState()
                        {
                            ParallelState = parallelState,
                            Results = bottomLevelResultsTask.Result
                        };

                        this.FinalizeAggregations(finalizationTasksState);
                        status = DataEngineStatus.Completed;
                    }
                }
            }
            catch (OperationCanceledException)
            {
                status = DataEngineStatus.Completed;
            }
            catch (AggregateException ae)
            {
                // Filter out the exception caused by cancellation itself.
                var flattedAggregateException = ae.Flatten();

                List<Exception> innerExceptions = null;
                for (int i = 0; i < flattedAggregateException.InnerExceptions.Count; i++)
                {
                    var innerException = flattedAggregateException.InnerExceptions[i];
                    if (!(innerException is OperationCanceledException))
                    {
                        if (innerExceptions == null)
                        {
                            innerExceptions = new List<Exception>();
                        }

                        innerExceptions.Add(innerException);
                    }
                }

                if (this.initalState == parallelState)
                {
                    if (innerExceptions != null && innerExceptions.Count > 0)
                    {
                        exception = new AggregateException(flattedAggregateException.ToString(), innerExceptions);
                    }
                    else
                    {
                        status = DataEngineStatus.Completed;
                    }
                }
            }
            catch (Exception ex)
            {
                exception = new AggregateException(ex.ToString(), new List<Exception>() { ex });
            }
            finally
            {
                if (this.initalState == parallelState)
                {
                    this.currentResultTask = null;

                    if (parallelState != null && parallelState.CancellationTokenSource != null)
                    {
                        parallelState.CancellationTokenSource.Dispose();
                        parallelState.CancellationTokenSource = null;
                    }

                    this.initalState = null;

                    ReadOnlyCollection<Exception> innerExceptions;
                    if (exception != null)
                    {
                        innerExceptions = exception.InnerExceptions;
                    }
                    else
                    {
                        innerExceptions = new ReadOnlyCollection<Exception>(new List<Exception>());
                    }

                    this.RaiseCompleted(new DataEngineCompletedEventArgs(innerExceptions, status));
                }
            }
        }

        public void RebuildCubeParallel(ParallelState state)
        {
            lock (locker)
            {
                this.CancelCurrentProcessing(state);
                state.CancellationTokenSource = new CancellationTokenSource();

                this.RaiseInProgress();
                this.BeginParallelProcessing(state);
            }
        }
    }
}
#endif
