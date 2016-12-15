using System;

namespace Telerik.Charting
{
    /// <summary>
    /// Represents a list of delegates.
    /// </summary>
    public class EventHandlerList
    {
        private EventEntry head;

        /// <summary>
        /// Retrieves the delegate, associated with the provided key.
        /// </summary>
        /// <param name="key">The key that the delegate is associated with.</param>
        public Delegate this[object key]
        {
            get
            {
                EventEntry entry = this.Find(key);
                if (entry != null)
                {
                    return entry.handler;
                }

                return null;
            }
        }

        /// <summary>
        /// Clears the list.
        /// </summary>
        public void Clear()
        {
            this.head = null;
        }

        /// <summary>
        /// Adds the specified delegate associated with the provided key.
        /// </summary>
        /// <param name="key">The key the delegate to be associated with.</param>
        /// <param name="handler">The delegate.</param>
        public void AddHandler(object key, Delegate handler)
        {
            EventEntry entry = this.Find(key);
            if (entry != null)
            {
                entry.handler = Delegate.Combine(entry.handler, handler);
            }
            else
            {
                this.head = new EventEntry(key, handler, this.head);
            }
        }

        /// <summary>
        /// Removes the specified delegate associated with the provided key.
        /// </summary>
        /// <param name="key">The key that the delegate is associated with.</param>
        /// <param name="handler">The delegate.</param>
        public void RemoveHandler(object key, Delegate handler)
        {
            EventEntry entry = this.Find(key);
            if (entry != null)
            {
                entry.handler = Delegate.Remove(entry.handler, handler);
            }
        }

        private EventEntry Find(object key)
        {
            EventEntry current = this.head;
            while (current != null)
            {
                if (current.key == key)
                {
                    return current;
                }

                current = current.next;
            }

            return null;
        }

        private class EventEntry
        {
            public Delegate handler;
            public object key;
            public EventEntry next;

            public EventEntry(object key, Delegate handler, EventEntry next)
            {
                this.key = key;
                this.handler = handler;
                this.next = next;
            }
        }
    }
}
