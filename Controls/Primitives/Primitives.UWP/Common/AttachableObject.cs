using System.Diagnostics.CodeAnalysis;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents an object that is attached to an owning object.
    /// </summary>
    /// <typeparam name="T">The type that owns this object.</typeparam>
    public abstract class AttachableObject<T> : RadDependencyObject where T : class
    {
        private T owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachableObject{T}" /> class.
        /// </summary>
        /// <param name="owner">The object instance that owns this one.</param>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        internal AttachableObject(T owner)
            : this()
        {
            this.Owner = owner;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AttachableObject{T}" /> class.
        /// </summary>
        protected AttachableObject()
        {
        }

        /// <summary>
        /// Gets or sets the object instance that owns this service.
        /// </summary>
        public T Owner
        {
            get
            {
                return this.owner;
            }
            protected internal set
            {
                if (this.owner == value)
                {
                    return;
                }

                if (this.owner != null)
                {
                    this.Detach();
                }

                this.Attach(value);
            }
        }

        /// <summary>
        /// Performs the core logic behind the Detach routine. Allows inheritors to provide additional implementation.
        /// </summary>
        protected virtual void OnDetached(T previousOwner)
        {
        }

        /// <summary>
        /// Performs the core logic behind the Attach routine. Allows inheritors to provide additional implementation.
        /// </summary>
        protected virtual void OnAttached()
        {
        }

        private void Detach()
        {
            var prevOwner = this.owner;
            this.owner = null;
            this.OnDetached(prevOwner);
        }

        private void Attach(T newOwner)
        {
            this.owner = newOwner;
            this.OnAttached();
        }
    }
}
