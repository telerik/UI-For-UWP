using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    internal static partial class ParallelAlgorithms
    {
        /// <summary>Sorts an array in parallel.</summary>
        /// <typeparam name="T">Specifies the type of data in the array.</typeparam>
        /// <param name="array">The array to be sorted.</param>
        /// <param name="comparer">The comparer used to compare two elements during the sort operation.</param>
        public static void Sort<T>(List<T> array, IComparer<T> comparer)
        {
            if (array == null)
            {
                throw new ArgumentNullException(nameof(array));
            }

            Sort<T, object>(array, null, 0, array.Count, comparer);
        }

        /// <summary>Sorts key/value arrays in parallel.</summary>
        /// <typeparam name="TKey">Specifies the type of the data in the keys array.</typeparam>
        /// <typeparam name="TValue">Specifies the type of the data in the items array.</typeparam>
        /// <param name="keys">The keys to be sorted.</param>
        /// <param name="items">The items to be sorted based on the corresponding keys.</param>
        /// <param name="index">The index at which to start the sort, inclusive.</param>
        /// <param name="length">The number of elements to be sorted, starting at the start index.</param>
        /// <param name="comparer">The comparer used to compare two elements during the sort operation.</param>
        internal static void Sort<TKey, TValue>(List<TKey> keys, TValue[] items, int index, int length, IComparer<TKey> comparer)
        {
            if (keys == null)
            {
                throw new ArgumentNullException(nameof(keys));
            }

            if ((index < 0) || (length < 0))
            {
                throw new ArgumentOutOfRangeException(length < 0 ? "length" : "index");
            }

            if (((keys.Count - index) < length) || ((items != null) && (index > (items.Length - length))))
            {
                throw new ArgumentException(nameof(index));
            }

            // Run the core sort operation
            new Sorter<TKey, TValue>(keys, items, comparer).QuickSort(index, index + length - 1);
        }

        // Stores the data necessary for the sort, and provides the core sorting method
        private sealed class Sorter<TKey, TItem>
        {
            ////private TKey[] _keys;
            private List<TKey> keys;
            private TItem[] items;
            private IComparer<TKey> comparer;

            public Sorter(List<TKey> keys, TItem[] items, IComparer<TKey> comparer)
            {
                if (comparer == null)
                {
                    comparer = Comparer<TKey>.Default;
                }

                this.keys = keys;
                this.items = items;
                this.comparer = comparer;
            }

            // Swaps the items at the two specified indexes if they need to be swapped
            internal void SwapIfGreaterWithItems(int a, int b)
            {
                if (a != b)
                {
                    if (this.comparer.Compare(this.keys[a], this.keys[b]) > 0)
                    {
                        TKey temp = this.keys[a];
                        this.keys[a] = this.keys[b];
                        this.keys[b] = temp;
                        if (this.items != null)
                        {
                            TItem item = this.items[a];
                            this.items[a] = this.items[b];
                            this.items[b] = item;
                        }
                    }
                }
            }

            // Does a quicksort of the stored data, between the positions (inclusive specified by left and right)
            internal void QuickSort(int left, int right)
            {
                this.QuickSort(left, right, 0, GetMaxDepth());
            }

            // Does a quicksort of the stored data, between the positions (inclusive specified by left and right).
            // Depth specifies the current recursion depth, while maxDepth specifies the maximum depth
            // we should recur to until we switch over to sequential.
            internal void QuickSort(int left, int right, int depth, int maxDepth)
            {
                const int SEQUENTIAL_THRESHOLD = 0x1000;

                // If the max depth has been reached or if we've hit the sequential
                // threshold for the input array size, run sequential.
                if (depth >= maxDepth || (right - left + 1) <= SEQUENTIAL_THRESHOLD)
                {
                    ////Array.Sort(_keys, _items, left, right - left + 1, _comparer);
                    ////Array.Sort(_keys, left, right - left + 1, _comparer);
                    this.keys.Sort(left, right - left + 1, this.comparer);
                    return;
                }

                // Store all tasks generated to process subarrays
                List<Task> tasks = new List<Task>();

                // Run the same basic algorithm used by Array.Sort, but spawning Tasks for all recursive calls
                do
                {
                    int i = left;
                    int j = right;

                    // Pre-sort the low, middle (pivot), and high values in place.
                    int middle = GetMiddle(i, j);
                    this.SwapIfGreaterWithItems(i, middle); // swap the low with the mid point
                    this.SwapIfGreaterWithItems(i, j);      // swap the low with the high
                    this.SwapIfGreaterWithItems(middle, j); // swap the middle with the high

                    // Get the pivot
                    TKey x = this.keys[middle];

                    // Move all data around the pivot value
                    do
                    {
                        while (this.comparer.Compare(this.keys[i], x) < 0)
                        {
                            i++;
                        }

                        while (this.comparer.Compare(x, this.keys[j]) < 0)
                        {
                            j--;
                        }

                        if (i > j)
                        {
                            break;
                        }

                        if (i < j)
                        {
                            TKey key = this.keys[i];
                            this.keys[i] = this.keys[j];
                            this.keys[j] = key;
                            if (this.items != null)
                            {
                                TItem item = this.items[i];
                                this.items[i] = this.items[j];
                                this.items[j] = item;
                            }
                        }

                        i++;
                        j--;
                    }
                    while (i <= j);

                    if (j - left <= right - i)
                    {
                        if (left < j)
                        {
                            int leftcopy = left, jcopy = j;
                            tasks.Add(Task.Factory.StartNew(() => this.QuickSort(leftcopy, jcopy, depth + 1, maxDepth)));
                        }

                        left = i;
                    }
                    else
                    {
                        if (i < right)
                        {
                            int icopy = i, rightcopy = right;
                            tasks.Add(Task.Factory.StartNew(() => this.QuickSort(icopy, rightcopy, depth + 1, maxDepth)));
                        }

                        right = j;
                    }
                }
                while (left < right);

                // Wait for all of this level's tasks to complete
                Task.WaitAll(tasks.ToArray());
            }

            // Gets a recommended depth for recursion.  This assumes that every level will
            // spawn two child tasks, which isn't actually the case with the algorithm, but
            // it's a "good enough" approximation.
            private static int GetMaxDepth()
            {
                return (int)Math.Log(Environment.ProcessorCount, 2);
            }

            // Gets the middle value between the provided low and high
            private static int GetMiddle(int low, int high)
            {
                return low + ((high - low) >> 1);
            }
        }
    }
}