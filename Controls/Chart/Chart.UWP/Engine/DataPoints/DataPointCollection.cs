using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Telerik.Core;

namespace Telerik.Charting
{
    /// <summary>
    /// Represents a typed element collection which accepts <see cref="DataPoint"/> instances only.
    /// </summary>
    /// <typeparam name="T">Instances of type <see cref="DataPoint"/>.</typeparam>
    public class DataPointCollection<T> : ElementCollection<T>, IList<DataPoint> where T : DataPoint
    {
        internal DataPointCollection(ChartSeriesModel owner)
            : base(owner)
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        int ICollection<DataPoint>.Count
        {
            get
            {
                return this.Count;
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        bool ICollection<DataPoint>.IsReadOnly
        {
            get
            {
                return false;
            }
        }

        DataPoint IList<DataPoint>.this[int index]
        {
            get
            {
                return this[index] as DataPoint;
            }
            set
            {
                this[index] = value as T;
            }
        }

        int IList<DataPoint>.IndexOf(DataPoint item)
        {
            return this.IndexOf(item as T);
        }

        void IList<DataPoint>.Insert(int index, DataPoint item)
        {
            this.Insert(index, item as T);
        }

        void IList<DataPoint>.RemoveAt(int index)
        {
            this.RemoveAt(index);
        }

        void ICollection<DataPoint>.Add(DataPoint item)
        {
            this.Add(item as T);
        }

        void ICollection<DataPoint>.Clear()
        {
            this.Clear();
        }

        bool ICollection<DataPoint>.Contains(DataPoint item)
        {
            return this.Contains(item as T);
        }

        void ICollection<DataPoint>.CopyTo(DataPoint[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }

        bool ICollection<DataPoint>.Remove(DataPoint item)
        {
            return this.Remove(item as T);
        }

        IEnumerator<DataPoint> IEnumerable<DataPoint>.GetEnumerator()
        {
            foreach (DataPoint point in this)
            {
                yield return point;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            foreach (DataPoint point in this)
            {
                yield return point;
            }
        }
    }
}
