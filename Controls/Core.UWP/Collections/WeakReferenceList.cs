using System;
using System.Collections;
using System.Collections.Generic;

namespace Telerik.Core
{
    /// <summary>
    /// Wraps instances of type T in WeakReference and stores them in a List.
    /// </summary>
    public class WeakReferenceList<T> : IEnumerable<T> where T : class
    {
        private List<WeakReference> list;
        private bool autoCleanNonAlive;
        private bool trackResurrection;

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReferenceList&lt;T&gt;"/> class.
        /// </summary>
        public WeakReferenceList()
        {
            this.list = new List<WeakReference>();
            this.autoCleanNonAlive = false;
            this.trackResurrection = false;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReferenceList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="cleanNonAlive">True to remove any non-alive instances automatically, false otherwise.</param>
        public WeakReferenceList(bool cleanNonAlive)
            : this()
        {
            this.autoCleanNonAlive = cleanNonAlive;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="WeakReferenceList&lt;T&gt;"/> class.
        /// </summary>
        /// <param name="cleanNonAlive">True to remove any non-alive instances automatically, false otherwise.</param>
        /// <param name="trackResurrection">True to track object resurrection, false otherwise.</param>
        public WeakReferenceList(bool cleanNonAlive, bool trackResurrection)
            : this(cleanNonAlive)
        {
            this.trackResurrection = trackResurrection;
        }

        /// <summary>
        /// Determines whether the list will automatically remove any contained non-alive weak reference.
        /// </summary>
        public bool AutoCleanNonAlive
        {
            get
            {
                return this.autoCleanNonAlive;
            }
            set
            {
                this.autoCleanNonAlive = value;
            }
        }

        /// <summary>
        /// Determines the WeakReference.TrackResurrection property for all T instances added.
        /// </summary>
        public bool TrackResurrection
        {
            get
            {
                return this.trackResurrection;
            }
            set
            {
                this.trackResurrection = value;
            }
        }

        /// <summary>
        /// Gets the count of the list.
        /// </summary>
        public int Count
        {
            get
            {
                return this.list.Count;
            }
        }

        /// <summary>
        /// Gets the internal List used to store all instances.
        /// </summary>
        protected List<WeakReference> List
        {
            get
            {
                return this.list;
            }
        }

        /// <summary>
        /// Gets or sets the item at the specified index.
        /// </summary>
        /// <value></value>
        public T this[int index]
        {
            get
            {
                if (index < 0 || index >= this.list.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                return (T)this.list[index].Target;
            }
            set
            {
                if (index < 0 || index >= this.list.Count)
                {
                    throw new IndexOutOfRangeException();
                }

                this.list[index] = new WeakReference(value, this.trackResurrection);
            }
        }

        /// <summary>
        /// Adds the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Add(T value)
        {
            this.InsertCore(this.Count, value);
        }

        /// <summary>
        /// Inserts an item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        public void Insert(int index, T value)
        {
            this.InsertCore(index, value);
        }

        /// <summary>
        /// Gets the index of the specified value within the list.
        /// </summary>
        /// <param name="value">The value to check for.</param>
        /// <returns></returns>
        public int IndexOf(T value)
        {
            int count = this.list.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                WeakReference reference = this.list[i];
                if (reference.IsAlive)
                {
                    if (reference.Target == value)
                    {
                        return i;
                    }
                }
                else if (this.autoCleanNonAlive)
                {
                    this.list.RemoveAt(i);
                }
            }

            return -1;
        }

        /// <summary>
        /// Removes the specified value.
        /// </summary>
        /// <param name="value">The value.</param>
        public void Remove(T value)
        {
            int index = this.IndexOf(value);
            if (index >= 0)
            {
                this.list.RemoveAt(index);
            }
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
            this.list.Clear();
        }

        /// <summary>
        /// Removes the value at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        public void RemoveAt(int index)
        {
            if (index < 0 || index >= this.list.Count)
            {
                throw new IndexOutOfRangeException();
            }

            this.list.RemoveAt(index);
        }

        /// <summary>
        /// Cleans all targets that are no longer alive.
        /// </summary>
        public void CleanNonAlive()
        {
            int count = this.list.Count;
            for (int i = count - 1; i >= 0; i--)
            {
                WeakReference reference = this.list[i];
                if (!reference.IsAlive)
                {
                    this.list.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Gets the enumerator that iterates through all items.
        /// </summary>
        /// <returns></returns>
        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                WeakReference reference = this.list[i];
                if (!reference.IsAlive)
                {
                    if (this.autoCleanNonAlive)
                    {
                        this.list.RemoveAt(i--);
                    }
                    continue;
                }

                yield return (T)reference.Target;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            for (int i = 0; i < this.list.Count; i++)
            {
                WeakReference reference = this.list[i];
                if (!reference.IsAlive)
                {
                    if (this.autoCleanNonAlive)
                    {
                        this.list.RemoveAt(i--);
                    }
                    continue;
                }

                yield return (T)reference.Target;
            }
        }

        /// <summary>
        /// Core insert implementation. Allows inheritors to provide their own implementation.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="value">The value.</param>
        protected virtual void InsertCore(int index, T value)
        {
            WeakReference reference = new WeakReference(value, this.trackResurrection);
            this.list.Insert(index, reference);
        }
    }
}