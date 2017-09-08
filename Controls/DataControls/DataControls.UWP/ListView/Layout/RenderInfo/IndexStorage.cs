using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace Telerik.Data.Core.Layouts
{
    internal class IndexStorage : ICollection, IEnumerable<long>, IRenderInfo
    {
        public const double UnknownItemLength = 0.001;

        public const int PrecisionMultiplier = 1000;

        private const long DefaultAverageItemLength = 40000;

        private long itemDefaultValue;
        private bool[] indicesWithValue;

        // Indexer will return average item length if IndexStorage.UnknownItemLength is found. 
        // this.storage will return the actual stored value. 
        // Use indexer to approximate the entire length.
        private long[] storage;

        private long[] aggregateInfo;
        private int size;
        private int count;
        private object syncRoot;
        private bool aggregateInfoUpdateInProgress;

        private long averageItemLength;
        private int knownSizeItemsCount;

        internal IndexStorage(int capacity)
            : this(capacity, 0)
        {
        }

        internal IndexStorage(int capacity, double defaultVal)
        {
            this.itemDefaultValue = IndexStorage.DoubleToLong(defaultVal, IndexStorage.PrecisionMultiplier);

            IndexStorage.CheckValue(this.itemDefaultValue);

            this.count = capacity;
            this.size = 8;

            if (defaultVal != IndexStorage.UnknownItemLength)
            {
                this.knownSizeItemsCount = Math.Max(capacity, this.size);
            }

            while (this.size < capacity)
            {
                this.size <<= 1;
            }

            this.indicesWithValue = new bool[this.size];
            this.storage = new long[this.size];
            this.aggregateInfo = new long[this.size];
            this.averageItemLength = this.itemDefaultValue;

            this.Initialize(null, capacity, this.itemDefaultValue);
        }

        public int Count
        {
            get
            {
                return this.count;
            }
        }

        public bool HasUpdatedValues
        {
            get;
            private set;
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.syncRoot ?? (this.syncRoot = new object());
            }
        }

        // Indexer will return average item length if IndexStorage.UnknownItemLength is found. 
        // this.storage will return the actual stored value. 
        // Use indexer to approximate the entire length.
        private long this[int index]
        {
            get
            {
                IndexStorage.CheckIndex(index, this.size);
                long val = this.storage[index];
                return val != IndexStorage.DoubleToLong(IndexStorage.UnknownItemLength, IndexStorage.PrecisionMultiplier) ? val : this.averageItemLength;
            }
            set
            {
                IndexStorage.CheckIndex(index, this.size);
                IndexStorage.CheckValue(value);
                long item = this.storage[index];
                if (item == value)
                {
                    return;
                }

                if (LongToDouble(item, IndexStorage.PrecisionMultiplier) == IndexStorage.UnknownItemLength)
                {
                    this.knownSizeItemsCount++;
                }

                if (this.knownSizeItemsCount == 0)
                {
                    this.averageItemLength = IndexStorage.DefaultAverageItemLength;
                }
                else
                {
                    this.averageItemLength = (long)(this.averageItemLength * (this.knownSizeItemsCount - 1) + value) / this.knownSizeItemsCount;
                }

                this.Set(index, value);
                this.RefreshAggregateInfo();
            }
        }

        public double OffsetFromIndex(int endIndex)
        {
            if (this.aggregateInfoUpdateInProgress)
            {
                Debugger.Break();
            }

            var index = endIndex;

            // Debug.Assert is not enough!
            // in some cases like continuous adding of a new item (ShowInsertRow = true)
            // without these two checks exception is thrown
            if (index < 0)
            {
                return this[0];
            }

            if (index >= this.storage.Length)
            {
                return this[this.storage.Length - 1];
            }

            long result = this[index];

            if ((index % 2) == 1)
            {
                result += this[index - 1];
            }

            index = (index + this.size) >> 1;

            while (index != 1)
            {
                bool comeFromRight = index % 2 == 1;
                if (comeFromRight)
                {
                    result += this.aggregateInfo[index - 1];
                }
                index >>= 1;
            }

            return IndexStorage.LongToDouble(result, IndexStorage.PrecisionMultiplier);
        }

        public int IndexFromOffset(double offset)
        {
            if (this.aggregateInfoUpdateInProgress)
            {
                Debugger.Break();
            }

            long value = IndexStorage.DoubleToLong(offset, IndexStorage.PrecisionMultiplier);
            if (value > this.aggregateInfo[1])
            {
                return this.count - 1;
            }
            int index = 1;
            while (index * 2 < this.size)
            {
                if (this.aggregateInfo[index * 2] < value)
                {
                    value -= this.aggregateInfo[index * 2];
                    index = (index * 2) + 1;
                }
                else
                {
                    index *= 2;
                }
            }

            index = (index * 2) - this.size;

            while (index < this.size)
            {
                if (this[index] < value)
                {
                    value -= this[index];
                    index++;
                }
                else
                {
                    break;
                }
            }

            return Math.Min(index, this.count - 1);
        }

        public void Clear()
        {
            Array.Clear(this.storage, 0, this.storage.Length);
            Array.Clear(this.indicesWithValue, 0, this.indicesWithValue.Length);

            this.HasUpdatedValues = false;
        }

        public void Insert(int index, double value)
        {
            long val = IndexStorage.DoubleToLong(value, IndexStorage.PrecisionMultiplier);

            IndexStorage.CheckValue(val);

            if (this.Count == this.size)
            {
                this.ExtendCapacity(this.size * 2);
            }

            Array.Copy(this.storage, index, this.storage, index + 1, this.size - index - 1);
            Array.Copy(this.indicesWithValue, index, this.indicesWithValue, index + 1, this.size - index - 1);

            this.RefreshAggregateInfo();

            this.count++;
            this[index] = val;
        }

        public void ResetToDefaultValues(IRenderInfoState loadState, double defaultValue)
        {
            this.Clear();

            long itemLength = double.IsNaN(defaultValue) ? this.itemDefaultValue : IndexStorage.DoubleToLong(defaultValue, IndexStorage.PrecisionMultiplier);

            this.Initialize(loadState, this.Count, itemLength);
        }

        public void InsertRange(int index, double? value, int insertItemsCount)
        {
            long val = 0;
            if (value.HasValue)
            {
                val = IndexStorage.DoubleToLong(value.Value, IndexStorage.PrecisionMultiplier);

                IndexStorage.CheckValue(val);
            }

            int newSize = this.size;

            while (this.Count + insertItemsCount >= newSize)
            {
                newSize *= 2;
            }

            if (newSize != this.size)
            {
                this.ExtendCapacity(newSize);
            }

            Array.Copy(this.storage, index, this.storage, index + insertItemsCount, this.size - index - insertItemsCount);
            Array.Copy(this.indicesWithValue, index, this.indicesWithValue, index + insertItemsCount, this.size - index - insertItemsCount);

            this.count += insertItemsCount;

            for (int i = 0; i < insertItemsCount; i++)
            {
                if (value.HasValue)
                {
                    this[index + i] = val;
                    this.indicesWithValue[index + i] = true;
                    this.HasUpdatedValues = true;
                }
            }

            this.RefreshAggregateInfo();
        }

        public void Add(double value)
        {
            long val = IndexStorage.DoubleToLong(value, IndexStorage.PrecisionMultiplier);

            IndexStorage.CheckValue(val);

            if (this.Count == this.size)
            {
                this.ExtendCapacity(this.size * 2);
                this.RefreshAggregateInfo();
            }

            this.count++;
            this[this.Count - 1] = val;
        }

        public void Update(int index, double value)
        {
            this[index] = IndexStorage.DoubleToLong(value, IndexStorage.PrecisionMultiplier);
        }

        public double ValueForIndex(int index, bool approximate = true)
        {
            if (approximate || this.indicesWithValue[index])
            {
                return IndexStorage.LongToDouble(this[index], IndexStorage.PrecisionMultiplier);
            }

            return 0.0;
        }

        /// <summary>
        /// Removes the element at the given index. Worst complexity is
        /// (N-InsertIndex) + Log(N)*NonDefaultsInRange(N-InsertIndex).
        /// </summary>
        /// <param name="index">The index at which to remove the item.</param>
        public void RemoveAt(int index)
        {
            IndexStorage.CheckIndex(index, this.size);

            Array.Copy(this.storage, index + 1, this.storage, index, this.storage.Length - index - 1);
            Array.Copy(this.indicesWithValue, index + 1, this.indicesWithValue, index, this.indicesWithValue.Length - index - 1);

            if (this.count > 0)
            {
                this[this.count - 1] = 0;
                this.count--;
            }

            this.RefreshAggregateInfo();
        }

        public void RemoveRange(int index, int removeItesCount)
        {
            IndexStorage.CheckIndex(index, this.size);
            IndexStorage.CheckIndex(index + removeItesCount, this.size);

            Array.Copy(this.storage, index + removeItesCount, this.storage, index, this.storage.Length - index - removeItesCount);
            Array.Copy(this.indicesWithValue, index + removeItesCount, this.indicesWithValue, index, this.indicesWithValue.Length - index - removeItesCount);

            for (int i = this.storage.Length - 1; i >= this.storage.Length - removeItesCount; i--)
            {
                this[i] = 0;
                this.indicesWithValue[i] = false;
            }

            this.count -= removeItesCount;

            this.RefreshAggregateInfo();
        }

        public IEnumerator<long> GetEnumerator()
        {
            for (int i = this.size; i < this.size + this.Count; i++)
            {
                yield return this[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (this as IEnumerable<int>).GetEnumerator();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotImplementedException();
        }

        internal static long DoubleToLong(double value, int precision)
        {
            return double.IsInfinity(value) ? long.MaxValue : (long)Math.Ceiling(value * precision);
        }

        internal static double LongToDouble(long value, int precision)
        {
            return value / (double)precision;
        }

        [Conditional("DEBUG")]
        private static void CheckValue(long value)
        {
            Debug.Assert(value >= 0, "The values stored in an index tree need to be non-negative.");
        }

        [Conditional("DEBUG")]
        private static void CheckIndex(int index, int capacity)
        {
            Debug.Assert(index >= 0, "Index should not be less than 0.");
            Debug.Assert(index < capacity, "Index should be smaller than the current capacity.");
        }

        private void Initialize(IRenderInfoState loadState, int capacity, long defaultValue)
        {
            if (defaultValue != 0)
            {
                var currentValue = defaultValue;

                for (int i = 0; capacity > 0; i++)
                {
                    if (loadState != null)
                    {
                        var loadValue = loadState.GetValueAt(i);

                        currentValue = loadValue.HasValue ? IndexStorage.DoubleToLong(loadValue.Value, IndexStorage.PrecisionMultiplier) : defaultValue;
                    }

                    this[i] = currentValue;
                    capacity--;
                }

                this.RefreshAggregateInfo();
            }
        }

        private void ExtendCapacity(int newSize)
        {
            var newStorage = new long[newSize];
            Array.Copy(this.storage, 0, newStorage, 0, this.count);
            this.storage = newStorage;

            var newIndices = new bool[newSize];
            Array.Copy(this.indicesWithValue, 0, newIndices, 0, this.count);
            this.indicesWithValue = newIndices;

            this.aggregateInfo = new long[newSize];
            this.size = newSize;
        }

        private void Set(int index, long value)
        {
            if (index < 0 || index >= this.storage.Length)
            {
                return;
            }

            this.indicesWithValue[index] = true;
            this.HasUpdatedValues = true;

            var oldValue = this.storage[index];
            var dif = value - oldValue;

            if (dif == 0)
            {
                return;
            }

            this.storage[index] += dif;

            index += this.size;
            index >>= 1;

            while (index > 0)
            {
                this.aggregateInfo[index] += dif;
                index >>= 1;
            }
        }

        private void RefreshAggregateInfo()
        {
            this.aggregateInfoUpdateInProgress = true;
            int index = this.size - 1;
            int aggregateInfoIndex = index;

            while (index > 0)
            {
                this.aggregateInfo[aggregateInfoIndex] = this[index] + this[index - 1];
                index -= 2;
                aggregateInfoIndex -= 1;
            }

            aggregateInfoIndex = this.aggregateInfo.Length - 1;

            int half = aggregateInfoIndex >> 1;
            while (aggregateInfoIndex != 1)
            {
                this.aggregateInfo[half] = this.aggregateInfo[aggregateInfoIndex] + this.aggregateInfo[aggregateInfoIndex - 1];
                aggregateInfoIndex -= 2;
                half = aggregateInfoIndex >> 1;
            }
            this.aggregateInfoUpdateInProgress = false;
        }
    }
}
