namespace Telerik.Core
{
    /// <summary>
    /// A utility class that holds a pair of objects of arbitrary types.
    /// </summary>
    public class Pair<T1, T2>
    {
        /// <summary>
        /// Initializes a new instance of the Pair class.
        /// </summary>
        /// <param name="first">The object for First.</param>
        /// <param name="second">The object for Second.</param>
        public Pair(T1 first, T2 second)
        {
            this.First = first;
            this.Second = second;
        }

        /// <summary>
        /// Gets or sets the first object.
        /// </summary>
        public T1 First
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets the second object.
        /// </summary>
        public T2 Second
        {
            get;
            set;
        }
    }
}