// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Diagnostics;

namespace Telerik.Data.Core
{
    internal class IndexToValueTable<T> : IEnumerable<Range<T>>
    {
        private List<Range<T>> list;

        public IndexToValueTable()
        {
            this.list = new List<Range<T>>();
        }

        /// <summary>
        /// Total number of indices represented in the table.
        /// </summary>
        public int IndexCount
        {
            get
            {
                int indexCount = 0;

                foreach (Range<T> range in this.list)
                {
                    indexCount += range.Count;
                }

                return indexCount;
            }
        }

        /// <summary>
        /// Returns true if the table is empty.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this.list.Count == 0;
            }
        }

        /// <summary>
        /// Returns the number of index ranges in the table.
        /// </summary>
        public int RangeCount
        {
            get
            {
                return this.list.Count;
            }
        }

        /// <summary>
        /// Add a value with an associated index to the table.
        /// </summary>
        /// <param name="index">Index where the value is to be added or updated.</param>
        /// <param name="value">Value to add.</param>
        public void AddValue(int index, T value)
        {
            this.AddValues(index, 1, value);
        }

        /// <summary>
        /// Add multiples values with an associated start index to the table.
        /// </summary>
        /// <param name="startIndex">Index where first value is added.</param>
        /// <param name="count">Total number of values to add (must be greater than 0).</param>
        /// <param name="value">Value to add.</param>
        public void AddValues(int startIndex, int count, T value)
        {
            Debug.Assert(count > 0, "Valid state check.");

            this.AddValuesPrivate(startIndex, count, value, null);
        }

        /// <summary>
        /// Clears the index table.
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// Returns true if the given index is contained in the table.
        /// </summary>
        /// <param name="index">Index to search for.</param>
        /// <returns>True if the index is contained in the table.</returns>
        public bool Contains(int index)
        {
            return this.IsCorrectRangeIndex(this.FindRangeIndex(index), index);
        }

        /// <summary>
        /// Returns true if the entire given index range is contained in the table.
        /// </summary>
        /// <param name="startIndex">Beginning of the range.</param>
        /// <param name="endIndex">End of the range.</param>
        /// <returns>True if the entire index range is present in the table.</returns>
        public bool ContainsAll(int startIndex, int endIndex)
        {
            int start = -1;
            int end = -1;

            foreach (Range<T> range in this.list)
            {
                if (start == -1 && range.UpperBound >= startIndex)
                {
                    if (startIndex < range.LowerBound)
                    {
                        return false;
                    }

                    start = startIndex;
                    end = range.UpperBound;

                    if (end >= endIndex)
                    {
                        return true;
                    }
                }
                else if (start != -1)
                {
                    if (range.LowerBound > end + 1)
                    {
                        return false;
                    }

                    end = range.UpperBound;
                    if (end >= endIndex)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Returns true if the given index is contained in the table with the the given value.
        /// </summary>
        /// <param name="index">Index to search for.</param>
        /// <param name="value">Value expected.</param>
        /// <returns>True if the given index is contained in the table with the the given value.</returns>
        public bool ContainsIndexAndValue(int index, T value)
        {
            int lowerRangeIndex = this.FindRangeIndex(index);

            return this.IsCorrectRangeIndex(lowerRangeIndex, index) && this.list[lowerRangeIndex].ContainsValue(value);
        }

        /// <summary>
        /// Returns a copy of this IndexToValueTable.
        /// </summary>
        /// <returns>Copy of this IndexToValueTable.</returns>
        public IndexToValueTable<T> Copy()
        {
            IndexToValueTable<T> copy = new IndexToValueTable<T>();

            foreach (Range<T> range in this.list)
            {
                copy.list.Add(range.Copy());
            }

            return copy;
        }

        // TODO: This method need unittests.
        public int CountNextNotIncludedIndexes(int index, int count)
        {
            int result = index;
            int visibleCount = 0;

            int rangeIndex = this.FindRangeIndex(index);
            if (this.list.Count > 0)
            {
                Range<T> range;
                if (rangeIndex >= 0)
                {
                    range = this.list[rangeIndex++];
                    if (range.ContainsIndex(index))
                    {
                        result = range.UpperBound + 1;
                    }
                }
                else
                {
                    rangeIndex++;
                }

                while (rangeIndex < this.list.Count)
                {
                    range = this.list[rangeIndex++];
                    if (range.LowerBound - 1 - result >= count - visibleCount)
                    {
                        return result + count - visibleCount;
                    }

                    visibleCount += range.LowerBound - result;
                    result = range.UpperBound + 1;
                }

                return result + (count - visibleCount);
            }
            else
            {
                return index + count;
            }
        }

        // TODO: This method need unittests.
        public int CountPreviousNotIncludedIndexes(int index, int count)
        {
            int result = index;
            int visibleCount = 0;

            int rangeIndex = this.FindRangeIndex(index);
            if (this.list.Count > 0)
            {
                Range<T> range;
                if (rangeIndex >= 0)
                {
                    range = this.list[rangeIndex];
                    if (range.ContainsIndex(index))
                    {
                        result = range.LowerBound - 1;
                    }
                }
                else
                {
                    rangeIndex--;
                }

                while (rangeIndex >= 0)
                {
                    range = this.list[rangeIndex--];
                    if (result - (range.UpperBound + 1) >= count - visibleCount)
                    {
                        return result - (count - visibleCount);
                    }

                    visibleCount += result - range.UpperBound;
                    result = range.LowerBound - 1;
                }

                return result - (count - visibleCount);
            }
            else
            {
                return index - count;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Not a real issue.")]
        public int GetNextGap(int index)
        {
            int targetIndex = index + 1;
            int rangeIndex = this.FindRangeIndex(targetIndex);

            if (this.IsCorrectRangeIndex(rangeIndex, targetIndex))
            {
                while (rangeIndex < this.list.Count - 1 && this.list[rangeIndex].UpperBound == this.list[rangeIndex + 1].LowerBound - 1)
                {
                    rangeIndex++;
                }

                return this.list[rangeIndex].UpperBound + 1;
            }
            else
            {
                return targetIndex;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Not a real issue.")]
        public int GetNextIndex(int index)
        {
            int targetIndex = index + 1;
            int rangeIndex = this.FindRangeIndex(targetIndex);

            if (this.IsCorrectRangeIndex(rangeIndex, targetIndex))
            {
                return targetIndex;
            }
            else
            {
                rangeIndex++;
                return rangeIndex < this.list.Count ? this.list[rangeIndex].LowerBound : -1;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Not a real issue.")]
        public int GetPreviousGap(int index)
        {
            int targetIndex = index - 1;
            int rangeIndex = this.FindRangeIndex(targetIndex);

            if (this.IsCorrectRangeIndex(rangeIndex, targetIndex))
            {
                while (rangeIndex > 0 && this.list[rangeIndex].LowerBound == this.list[rangeIndex - 1].UpperBound + 1)
                {
                    rangeIndex--;
                }

                return this.list[rangeIndex].LowerBound - 1;
            }
            else
            {
                return targetIndex;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2233:OperationsShouldNotOverflow", Justification = "Not a real issue.")]
        public int GetPreviousIndex(int index)
        {
            int targetIndex = index - 1;
            int rangeIndex = this.FindRangeIndex(targetIndex);

            if (this.IsCorrectRangeIndex(rangeIndex, targetIndex))
            {
                return targetIndex;
            }
            else
            {
                return rangeIndex >= 0 && rangeIndex < this.list.Count ? this.list[rangeIndex].UpperBound : -1;
            }
        }

        /// <summary>
        /// Returns the inclusive index count between lowerBound and upperBound of all indexes with the given value.
        /// </summary>
        /// <param name="lowerBound">LowerBound criteria.</param>
        /// <param name="upperBound">UpperBound criteria.</param>
        /// <param name="value">Value to look for.</param>
        /// <returns>Number of indexes contained in the table between lowerBound and upperBound (inclusive).</returns>
        public int GetIndexCount(int lowerBound, int upperBound, T value)
        {
            Debug.Assert(upperBound >= lowerBound, "Integrity check.");

            if (this.list.Count == 0)
            {
                return 0;
            }

            int count = 0;
            int index = this.FindRangeIndex(lowerBound);

            if (this.IsCorrectRangeIndex(index, lowerBound) && this.list[index].ContainsValue(value))
            {
                count += this.list[index].UpperBound - lowerBound + 1;
            }

            index++;
            while (index < this.list.Count && this.list[index].UpperBound <= upperBound)
            {
                if (this.list[index].ContainsValue(value))
                {
                    count += this.list[index].Count;
                }

                index++;
            }

            if (index < this.list.Count && this.IsCorrectRangeIndex(index, upperBound) && this.list[index].ContainsValue(value))
            {
                count += upperBound - this.list[index].LowerBound;
            }

            return count;
        }

        /// <summary>
        /// Returns the inclusive index count between lowerBound and upperBound.
        /// </summary>
        /// <param name="lowerBound">LowerBound criteria.</param>
        /// <param name="upperBound">UpperBound criteria.</param>
        /// <returns>Number of indexes contained in the table between lowerBound and upperBound (inclusive).</returns>
        public int GetIndexCount(int lowerBound, int upperBound)
        {
            if (upperBound < lowerBound || this.list.Count == 0)
            {
                return 0;
            }

            int count = 0;
            int index = this.FindRangeIndex(lowerBound);

            if (this.IsCorrectRangeIndex(index, lowerBound))
            {
                count += this.list[index].UpperBound - lowerBound + 1;
            }

            index++;

            while (index < this.list.Count && this.list[index].UpperBound <= upperBound)
            {
                count += this.list[index].Count;
                index++;
            }

            if (index < this.list.Count && this.IsCorrectRangeIndex(index, upperBound))
            {
                count += upperBound - this.list[index].LowerBound;
            }

            return count;
        }

        /// <summary>
        /// Returns the number indexes in this table after a given startingIndex but before.
        /// reaching a gap of indexes of a given size.
        /// </summary>
        /// <param name="startingIndex">Index to start at.</param>
        /// <param name="gapSize">Size of index gap.</param>
        /// <returns></returns>
        public int GetIndexCountBeforeGap(int startingIndex, int gapSize)
        {
            if (this.list.Count == 0)
            {
                return 0;
            }

            int count = 0;
            int currentIndex = startingIndex;
            int rangeIndex = 0;
            int gap = 0;
            while (gap <= gapSize && rangeIndex < this.list.Count)
            {
                gap += this.list[rangeIndex].LowerBound - currentIndex;
                if (gap <= gapSize)
                {
                    count += this.list[rangeIndex].UpperBound - this.list[rangeIndex].LowerBound + 1;
                    currentIndex = this.list[rangeIndex].UpperBound + 1;
                    rangeIndex++;
                }
            }

            return count;
        }

        /// <summary>
        /// Returns an enumerator that goes through the indexes present in the table.
        /// </summary>
        /// <returns>An enumerator that enumerates the indexes present in the table.</returns>
        public IEnumerable<int> GetIndexes()
        {
            Debug.Assert(this.list != null, "Integrity check.");

            foreach (Range<T> range in this.list)
            {
                for (int i = range.LowerBound; i <= range.UpperBound; i++)
                {
                    yield return i;
                }
            }
        }

        /// <summary>
        /// Returns all the indexes on or after a starting index.
        /// </summary>
        /// <param name="startIndex">Start index.</param>
        public IEnumerable<int> GetIndexes(int startIndex)
        {
            Debug.Assert(this.list != null, "Integrity check.");

            int rangeIndex = this.FindRangeIndex(startIndex);
            if (rangeIndex == -1)
            {
                rangeIndex++;
            }

            while (rangeIndex < this.list.Count)
            {
                for (int i = this.list[rangeIndex].LowerBound; i <= this.list[rangeIndex].UpperBound; i++)
                {
                    if (i >= startIndex)
                    {
                        yield return i;
                    }
                }

                rangeIndex++;
            }
        }

        /// <summary>
        /// Return the index of the Nth element in the table.
        /// </summary>
        /// <param name="n">N.</param>
        public int GetNthIndex(int n)
        {
            Debug.Assert(n >= 0 && n < this.IndexCount, "Valid state check.");

            int cumulatedEntries = 0;
            foreach (Range<T> range in this.list)
            {
                if (cumulatedEntries + range.Count > n)
                {
                    return range.LowerBound + n - cumulatedEntries;
                }
                else
                {
                    cumulatedEntries += range.Count;
                }
            }

            return -1;
        }

        /// <summary>
        /// Returns the value at a given index or the default value if the index is not in the table.
        /// </summary>
        /// <param name="index">Index to search for.</param>
        /// <returns>The value at the given index or the default value if index is not in the table.</returns>
        public T GetValueAt(int index)
        {
            bool found;
            return this.GetValueAt(index, out found);
        }

        /// <summary>
        /// Returns the value at a given index or the default value if the index is not in the table.
        /// </summary>
        /// <param name="index">Index to search for.</param>
        /// <param name="found">Set to true by the method if the index was found; otherwise, false.</param>
        /// <returns>The value at the given index or the default value if index is not in the table.</returns>
        public T GetValueAt(int index, out bool found)
        {
            int rangeIndex = this.FindRangeIndex(index);
            if (this.IsCorrectRangeIndex(rangeIndex, index))
            {
                found = true;
                return this.list[rangeIndex].Value;
            }
            else
            {
                found = false;
                return default(T);
            }
        }

        /// <summary>
        /// Returns an index's index within this table.
        /// </summary>
        public int IndexOf(int index)
        {
            int cumulatedIndexes = 0;
            foreach (Range<T> range in this.list)
            {
                if (range.UpperBound >= index)
                {
                    cumulatedIndexes += index - range.LowerBound;
                    break;
                }
                else
                {
                    cumulatedIndexes += range.Count;
                }
            }

            return cumulatedIndexes;
        }

        /// <summary>
        /// Inserts an index at the given location. This does not alter values in the table.
        /// </summary>
        /// <param name="index">Index location to insert an index.</param>
        public void InsertIndex(int index)
        {
            this.InsertIndexes(index, 1);
        }

        /// <summary>
        /// Inserts an index into the table with the given value .
        /// </summary>
        /// <param name="index">Index to insert.</param>
        /// <param name="value">Value for the index.</param>
        public void InsertIndexAndValue(int index, T value)
        {
            this.InsertIndexesAndValues(index, 1, value);
        }

        /// <summary>
        /// Inserts multiple indexes into the table. This does not alter Values in the table.
        /// </summary>
        /// <param name="startIndex">First index to insert.</param>
        /// <param name="count">Total number of indexes to insert.</param>
        public void InsertIndexes(int startIndex, int count)
        {
            Debug.Assert(count > 0, "Valid state check.");

            this.InsertIndexesPrivate(startIndex, count, this.FindRangeIndex(startIndex));
        }

        /// <summary>
        /// Inserts multiple indexes into the table with the given value.
        /// </summary>
        /// <param name="startIndex">Index to insert first value.</param>
        /// <param name="count">Total number of values to insert. (must be greater than 0).</param>
        /// <param name="value">Value to insert.</param>
        public void InsertIndexesAndValues(int startIndex, int count, T value)
        {
            Debug.Assert(count > 0, "Valid state check.");

            int lowerRangeIndex = this.FindRangeIndex(startIndex);
            this.InsertIndexesPrivate(startIndex, count, lowerRangeIndex);

            if ((lowerRangeIndex >= 0) && (this.list[lowerRangeIndex].LowerBound > startIndex))
            {
                // Because of the insert, the original range no longer contains the startIndex
                lowerRangeIndex--;
            }

            this.AddValuesPrivate(startIndex, count, value, lowerRangeIndex);
        }

        /// <summary>
        /// Removes an index from the table. This does not alter Values in the table.
        /// </summary>
        /// <param name="index">Index to remove.</param>
        public void RemoveIndex(int index)
        {
            this.RemoveIndexes(index, 1);
        }

        /// <summary>
        /// Removes a value and its index from the table.
        /// </summary>
        /// <param name="index">Index to remove.</param>
        public void RemoveIndexAndValue(int index)
        {
            this.RemoveIndexesAndValues(index, 1);
        }

        /// <summary>
        /// Removes multiple indexes from the table.  This does not alter Values in the table.
        /// </summary>
        /// <param name="startIndex">First index to remove.</param>
        /// <param name="count">Total number of indexes to remove.</param>
        public void RemoveIndexes(int startIndex, int count)
        {
            int lowerRangeIndex = this.FindRangeIndex(startIndex);
            if (lowerRangeIndex < 0)
            {
                lowerRangeIndex = 0;
            }

            int i = lowerRangeIndex;
            while (i < this.list.Count)
            {
                Range<T> range = this.list[i];
                if (range.UpperBound >= startIndex)
                {
                    if (range.LowerBound >= startIndex + count)
                    {
                        // Both bounds will remain after the removal
                        range.LowerBound -= count;
                        range.UpperBound -= count;
                    }
                    else
                    {
                        int currentIndex = i;
                        if (range.LowerBound <= startIndex)
                        {
                            // Range gets split up
                            if (range.UpperBound >= startIndex + count)
                            {
                                i++;
                                this.list.Insert(i, new Range<T>(startIndex, range.UpperBound - count, range.Value));
                            }

                            range.UpperBound = startIndex - 1;
                        }
                        else
                        {
                            range.LowerBound = startIndex;
                            range.UpperBound -= count;
                        }

                        if (this.RemoveRangeIfInvalid(range, currentIndex))
                        {
                            i--;
                        }
                    }
                }

                i++;
            }

            if (!this.Merge(lowerRangeIndex))
            {
                this.Merge(lowerRangeIndex + 1);
            }
        }

        /// <summary>
        /// Removes multiple values and their indexes from the table.
        /// </summary>
        /// <param name="startIndex">First index to remove.</param>
        /// <param name="count">Total number of indexes to remove.</param>
        public void RemoveIndexesAndValues(int startIndex, int count)
        {
            this.RemoveValues(startIndex, count);
            this.RemoveIndexes(startIndex, count);
        }

        /// <summary>
        /// Removes a value from the table at the given index. This does not alter other indexes in the table.
        /// </summary>
        /// <param name="index">Index where value should be removed.</param>
        public void RemoveValue(int index)
        {
            this.RemoveValues(index, 1);
        }

        /// <summary>
        /// Removes multiple values from the table. This does not alter other indexes in the table.
        /// </summary>
        /// <param name="startIndex">First index where values should be removed.</param>
        /// <param name="count">Total number of values to remove.</param>
        public void RemoveValues(int startIndex, int count)
        {
            Debug.Assert(count > 0, "Valid state check.");

            int lowerRangeIndex = this.FindRangeIndex(startIndex);
            if (lowerRangeIndex < 0)
            {
                lowerRangeIndex = 0;
            }

            while ((lowerRangeIndex < this.list.Count) && (this.list[lowerRangeIndex].UpperBound < startIndex))
            {
                lowerRangeIndex++;
            }

            if (lowerRangeIndex >= this.list.Count || this.list[lowerRangeIndex].LowerBound > startIndex + count - 1)
            {
                // If all the values are above our below our values, we have nothing to remove
                return;
            }

            if (this.list[lowerRangeIndex].LowerBound < startIndex)
            {
                // Need to split this up
                this.list.Insert(lowerRangeIndex, new Range<T>(this.list[lowerRangeIndex].LowerBound, startIndex - 1, this.list[lowerRangeIndex].Value));
                lowerRangeIndex++;
            }

            this.list[lowerRangeIndex].LowerBound = startIndex + count;
            if (!this.RemoveRangeIfInvalid(this.list[lowerRangeIndex], lowerRangeIndex))
            {
                lowerRangeIndex++;
            }

            while ((lowerRangeIndex < this.list.Count) && (this.list[lowerRangeIndex].UpperBound < startIndex + count))
            {
                this.list.RemoveAt(lowerRangeIndex);
            }

            if ((lowerRangeIndex < this.list.Count) && (this.list[lowerRangeIndex].UpperBound >= startIndex + count) &&
                (this.list[lowerRangeIndex].LowerBound < startIndex + count))
            {
                // Chop off the start of the remaining Range if it contains values that we're removing
                this.list[lowerRangeIndex].LowerBound = startIndex + count;
                this.RemoveRangeIfInvalid(this.list[lowerRangeIndex], lowerRangeIndex);
            }
        }

        public IEnumerator<Range<T>> GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.list.GetEnumerator();
        }

        internal bool TryGetValue(int slot, out T group, out int lowerBound)
        {
            bool found;
            int rangeIndex = this.FindRangeIndex(slot, out found);
            if (rangeIndex == -1)
            {
                group = default(T);
                lowerBound = -1;

                return false;
            }

            var range = this.list[rangeIndex];

            lowerBound = range.LowerBound;
            group = range.Value;
            return true;
        }

        private void AddValuesPrivate(int startIndex, int count, T value, int? startRangeIndex)
        {
            Debug.Assert(count > 0, "Valid state check.");

            int endIndex = startIndex + count - 1;
            Range<T> newRange = new Range<T>(startIndex, endIndex, value);

            if (this.list.Count == 0)
            {
                this.list.Add(newRange);
            }
            else
            {
                int lowerRangeIndex = startRangeIndex.HasValue ? startRangeIndex.Value : this.FindRangeIndex(startIndex);
                Range<T> lowerRange = (lowerRangeIndex < 0) ? null : this.list[lowerRangeIndex];
                if (lowerRange == null)
                {
                    if (lowerRangeIndex < 0)
                    {
                        lowerRangeIndex = 0;
                    }

                    this.list.Insert(lowerRangeIndex, newRange);
                }
                else
                {
                    if (!lowerRange.Value.Equals(value) && (lowerRange.UpperBound >= startIndex))
                    {
                        // Split up the range
                        if (lowerRange.UpperBound > endIndex)
                        {
                            this.list.Insert(lowerRangeIndex + 1, new Range<T>(endIndex + 1, lowerRange.UpperBound, lowerRange.Value));
                        }

                        lowerRange.UpperBound = startIndex - 1;
                        if (!this.RemoveRangeIfInvalid(lowerRange, lowerRangeIndex))
                        {
                            lowerRangeIndex++;
                        }

                        this.list.Insert(lowerRangeIndex, newRange);
                    }
                    else
                    {
                        this.list.Insert(lowerRangeIndex + 1, newRange);
                        if (!this.Merge(lowerRangeIndex))
                        {
                            lowerRangeIndex++;
                        }
                    }
                }

                // At this point the newRange has been inserted in the correct place, now we need to remove
                // any subsequent ranges that no longer make sense and possibly update the one at newRange.UpperBound
                int upperRangeIndex = lowerRangeIndex + 1;
                while ((upperRangeIndex < this.list.Count) && (this.list[upperRangeIndex].UpperBound < endIndex))
                {
                    this.list.RemoveAt(upperRangeIndex);
                }

                if (upperRangeIndex < this.list.Count)
                {
                    Range<T> upperRange = this.list[upperRangeIndex];
                    if (upperRange.LowerBound <= endIndex)
                    {
                        // Update the range
                        upperRange.LowerBound = endIndex + 1;
                        this.RemoveRangeIfInvalid(upperRange, upperRangeIndex);
                    }

                    this.Merge(lowerRangeIndex);
                }
            }
        }

        private int FindRangeIndex(int index)
        {
            bool found;
            return this.FindRangeIndex(index, out found);
        }

        // Returns the index of the range that contains the input or the range before if the input is not found
        private int FindRangeIndex(int index, out bool found)
        {
            found = false;
            if (this.list.Count == 0)
            {
                return -1;
            }

            //// Do a binary search for the index
            int front = 0;
            int end = this.list.Count - 1;
            Range<T> range = null;
            while (end > front)
            {
                int median = (front + end) / 2;
                range = this.list[median];
                if (range.UpperBound < index)
                {
                    front = median + 1;
                }
                else if (range.LowerBound > index)
                {
                    end = median - 1;
                }
                else
                {
                    // we found it
                    found = true;
                    return median;
                }
            }

            if (front == end)
            {
                range = this.list[front];
                if (range.ContainsIndex(index))
                {
                    found = true;
                    
                    // we found it or the index isn't there and we're one range before
                    return front;
                }
                else if (range.UpperBound < index)
                {
                    return front;
                }
                else
                {
                    // not found and we're one range after
                    return front - 1;
                }
            }
            else
            {
                // end is one index before front in this case so it's the range before
                return end;
            }
        }

        private bool Merge(int lowerRangeIndex)
        {
            int upperRangeIndex = lowerRangeIndex + 1;
            if ((lowerRangeIndex >= 0) && (upperRangeIndex < this.list.Count))
            {
                Range<T> lowerRange = this.list[lowerRangeIndex];
                Range<T> upperRange = this.list[upperRangeIndex];
                if ((lowerRange.UpperBound + 1 >= upperRange.LowerBound) && lowerRange.Value.Equals(upperRange.Value))
                {
                    lowerRange.UpperBound = System.Math.Max(lowerRange.UpperBound, upperRange.UpperBound);
                    this.list.RemoveAt(upperRangeIndex);
                    return true;
                }
            }

            return false;
        }

        private void InsertIndexesPrivate(int startIndex, int count, int lowerRangeIndex)
        {
            Debug.Assert(count > 0, "Valid state check.");

            // Same as AddRange after we fix the indicies affected by the insertion
            int startRangeIndex = (lowerRangeIndex >= 0) ? lowerRangeIndex : 0;
            for (int i = startRangeIndex; i < this.list.Count; i++)
            {
                Range<T> range = this.list[i];
                if (range.LowerBound >= startIndex)
                {
                    range.LowerBound += count;
                }
                else
                {
                    if (range.UpperBound >= startIndex)
                    {
                        // Split up this range
                        i++;
                        this.list.Insert(i, new Range<T>(startIndex, range.UpperBound + count, range.Value));
                        range.UpperBound = startIndex - 1;
                        continue;
                    }
                }

                if (range.UpperBound >= startIndex)
                {
                    range.UpperBound += count;
                }
            }
        }

        private bool IsCorrectRangeIndex(int rangeIndex, int index)
        {
            return (-1 != rangeIndex) && this.list[rangeIndex].ContainsIndex(index);
        }

        private bool RemoveRangeIfInvalid(Range<T> range, int rangeIndex)
        {
            if (range.UpperBound < range.LowerBound)
            {
                this.list.RemoveAt(rangeIndex);

                return true;
            }

            return false;
        }
    }
}