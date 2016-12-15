using System.Collections;
using System.Collections.Generic;

namespace Telerik.Core
{
    internal class ReadOnlyList<V, T> : IReadOnlyList<T>
        where V : T
    {
        private IList<V> source;

        public ReadOnlyList(IList<V> source)
        {
            this.source = source;
        }

        public int Count
        {
            get
            {
                return this.source.Count;
            }
        }

        public T this[int index]
        {
            get
            {
                return this.source[index];
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
#if NETFX2
            foreach (var item in this.source)
            {
                yield return item;
            }
#else
            IEnumerator<T> enumerator = this.source.GetEnumerator() as IEnumerator<T>;
            System.Diagnostics.Debug.Assert(enumerator != null, "Valid state check.");
            return enumerator;
#endif
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.source.GetEnumerator();
        }
    }
}
