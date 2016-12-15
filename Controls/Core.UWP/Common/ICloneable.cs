namespace Telerik.Core
{
    /// <summary>
    /// Supports cloning, which creates a new instance of a class with the same value as an existing instance.
    /// </summary>
    /// <typeparam name="T">The concrete type of the clone instance.</typeparam>
    public interface ICloneable<T>
    {
        /// <summary>
        /// Creates a new object that is a copy of the current instance.
        /// </summary>
        T Clone();
    }
}
