using System;
using System.Collections.Generic;
using Telerik.Core.Data;

namespace Telerik.Core
{
    /// <summary>
    /// Represents an object that implements the <see cref="IDisposable"/> interface and automatically releases any events, attached to this instance.
    /// </summary>
    public abstract class DisposableObject : SuspendableObject, IDisposable
    {
        internal const uint DisposableObjectStateKey = IsDisposedStateKey;

        private const uint IsDisposingStateKey = 1;
        private const uint IsDisposedStateKey = IsDisposingStateKey << 1;

        private EventHandler disposingEvent;
        private EventHandler disposedEvent;

        private BitVector32 bitState;

        /// <summary>
        /// Initializes a new instance of the <see cref="DisposableObject"/> class.
        /// </summary>
        protected DisposableObject()
        {
            this.bitState = new BitVector32(0);
        }

        /// <summary>
        /// Raised when the object enters its Dispose routine.
        /// </summary>
        public event EventHandler Disposing
        {
            add
            {
                this.disposingEvent += value;
            }
            remove
            {
                this.disposingEvent -= value;
            }
        }

        /// <summary>
        /// Raised when the object has finished its Dispose routine.
        /// </summary>
        public event EventHandler Disposed
        {
            add
            {
                this.disposedEvent += value;
            }
            remove
            {
                this.disposedEvent -= value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object is currently in a Dispose cycle.
        /// </summary>
        public bool IsDisposing
        {
            get
            {
                return this.bitState[IsDisposingStateKey];
            }
        }

        /// <summary>
        /// Gets a value indicating whether the object is already disposed.
        /// </summary>
        public bool IsDisposed
        {
            get
            {
                return this.bitState[IsDisposedStateKey];
            }
        }

        /// <summary>
        /// Directly gets or sets the bit, determined by the specified key.
        /// Use the SetBitState method to receive a Changed notification as appropriate.
        /// </summary>
        protected internal bool this[uint key]
        {
            get
            {
                return this.bitState[key];
            }
            set
            {
                this.bitState[key] = value;
            }
        }

        /// <summary>
        /// Releases all resources, used by this instance.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly")]
        public void Dispose()
        {
            this.DisposeCore(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Updates the specified bit, determined by the supplied key, depending on the value.
        /// </summary>
        /// <param name="key">The key.</param>
        /// <param name="value">The value.</param>
        protected internal void SetBitState(uint key, bool value)
        {
            bool oldValue = this.bitState[key];
            if (oldValue == value)
            {
                return;
            }

            this.bitState[key] = value;
            this.OnBitStateChanged(new BitStateChangedEventArgs(key, oldValue, value));
        }

        /// <summary>
        /// Notifies for a change in the current bit state of this instance.
        /// </summary>
        /// <param name="e">The arguments, associated with the event.</param>
        protected virtual void OnBitStateChanged(BitStateChangedEventArgs e)
        {
        }

        /// <summary>
        /// Disposes all managed resources allocated by this instance.
        /// </summary>
        protected virtual void DisposeManagedResources()
        {
            // collect all events
            List<Delegate> events = new List<Delegate>(8);
            this.CollectEvents(events);

            // clean-up events subscription
            int count = events.Count;
            for (int i = 0; i < count; i++)
            {
                Delegate del = events[i];
                if (del == null)
                {
                    continue;
                }

                Delegate[] listeners = del.GetInvocationList();
                foreach (Delegate listener in listeners)
                {
                    del = Delegate.Remove(del, listener);
                }
            }
        }

        /// <summary>
        /// Gets all the events, exposed by this instance. Used to clean-up event subscriptions upon disposal.
        /// </summary>
        /// <param name="events">The events.</param>
        protected virtual void CollectEvents(List<Delegate> events)
        {
            if (events == null)
            {
                return;
            }

            events.Add(this.disposingEvent);
            events.Add(this.disposedEvent);
        }

        /// <summary>
        /// Override this method if any unmanaged resources such as file handles are allocated.
        /// </summary>
        protected virtual void DisposeUnmanagedResources()
        {
        }

        private void DisposeCore(bool disposing)
        {
            if (this.IsDisposed)
            {
                return;
            }

            if (this.IsDisposing)
            {
                throw new InvalidOperationException("Object is currently disposing its resources.");
            }

            this.bitState[IsDisposingStateKey] = true;

            if (disposing)
            {
                this.DisposeManagedResources();
            }

            this.DisposeUnmanagedResources();

            this.bitState[IsDisposingStateKey] = false;
            this.bitState[IsDisposedStateKey] = true;
        }
    }
}