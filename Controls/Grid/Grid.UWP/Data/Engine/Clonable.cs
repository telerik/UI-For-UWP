using System;
using System.Globalization;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Defines an object that has a modifiable state and a read-only state. Classes that derive from <see cref="Cloneable"/> can clone themselves. 
    /// </summary>
    internal abstract class Cloneable
    {
        /// <summary>
        /// Creates a new instance of the <see cref="Telerik.Data.Core.Cloneable"/>, making deep copies of the object's values.
        /// </summary>
        /// <returns>A clone of the current object.</returns>
        public Cloneable Clone()
        {
            var clone = this.CreateInstance();
            clone.CloneCore(this);

            return clone;
        }

        /// <summary>
        /// If source is null - returns default(<typeparamref name="T"/>).
        /// If source is not null makes a copy of type <typeparamref name="T"/>.
        /// If the copy is from a different type throws appropriate exception.
        /// </summary>
        /// <typeparam name="T">The expected copy type.</typeparam>
        /// <param name="source">The source that is about to be copied.</param>
        /// <returns>Clone of <paramref name="source"/> of type <typeparamref name="T"/>. If source is null - default(<typeparamref name="T"/>).</returns>
        internal static T CloneOrDefault<T>(T source) where T : Cloneable
        {
            if (source == null)
            {
                return default(T);
            }

            var clone = source.Clone();
            if (clone == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Clone of {0} returned null. Type of {1} or derived type expected.", source.GetType(), typeof(T)));
            }

            T stronglyTypedClone = clone as T;
            if (stronglyTypedClone == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.InvariantCulture, "Cloneable of type {0} resulted into a clone of type {1}. Type of {2} or derived type expected.", source.GetType(), clone.GetType(), typeof(T)));
            }

            return stronglyTypedClone;
        }

        /// <summary>
        /// When implemented in a derived class, creates a new instance of the <see cref="Telerik.Data.Core.Cloneable"/> derived class. 
        /// </summary>
        /// <returns>New instance for cloning.</returns>
        /// <remarks>Do not call this method directly (except when calling base in an implementation). This method is called internally by the <see cref="Clone()"/> method whenever a new instance of the <see cref="Telerik.Data.Core.Cloneable"/> is created.
        /// Notes to Inheritors.
        /// Every <see cref="Telerik.Data.Core.Cloneable"/> derived class must implement this method. A typical implementation is to simply call the default constructor and return the result. 
        /// </remarks>
        protected abstract Cloneable CreateInstanceCore();

        /// <summary>
        /// Makes the instance a clone (deep copy) of the specified <see cref="Telerik.Data.Core.Cloneable"/>.
        /// </summary>
        /// <param name="source">The object to clone.</param>
        /// <remarks>Notes to Inheritors
        /// If you derive from <see cref="Telerik.Data.Core.Cloneable"/>, you may need to override this method to copy all properties.
        /// It is essential that all implementations call the base implementation of this method (if you don't call base you should manually copy all needed properties including base properties).
        /// </remarks>
        protected abstract void CloneCore(Cloneable source);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "CreateInstance", Justification = "Design choice.")]
        private static void VerifyInstance(Cloneable original, Cloneable newInstance)
        {
            if (newInstance == null)
            {
                throw new ArgumentNullException("newInstance");
            }

            if (original == newInstance)
            {
                throw new InvalidOperationException("CreateInstance should not return the same instance as the original.");
            }
        }

        private Cloneable CreateInstance()
        {
            Cloneable newInstance = this.CreateInstanceCore();
            VerifyInstance(this, newInstance);
            return newInstance;
        }
    }
}