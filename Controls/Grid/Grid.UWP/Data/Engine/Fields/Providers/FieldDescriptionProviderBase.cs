using System;
using System.Collections.Generic;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// A base class for various implementations of <see cref="IFieldDescriptionProvider"/>.
    /// </summary>
    internal abstract class FieldDescriptionProviderBase : IFieldDescriptionProvider
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FieldDescriptionProviderBase"/> class.
        /// </summary>
        protected FieldDescriptionProviderBase()
        {
        }

        /// <inheritdoc />
        public event EventHandler<GetDescriptionsDataCompletedEventArgs> GetDescriptionsDataAsyncCompleted;

        /// <inheritdoc />
        public bool IsBusy
        {
            get;
            protected set;
        }

        /// <summary>
        /// Determines whether the provider is ready to be used. That is, to not be Busy and to be properly initialized.
        /// </summary>
        public abstract bool IsReady
        {
            get;
        }

        /// <inheritdoc />
        public abstract void GetDescriptionsDataAsync(object state);

        /// <summary>
        /// Raise GetDescriptionsDataAsyncCompleted event.
        /// </summary>
        /// <param name="args">The event args used to invoke the event.</param>
        protected virtual void OnDescriptionsDataCompleted(GetDescriptionsDataCompletedEventArgs args)
        {
            this.IsBusy = false;

            if (this.GetDescriptionsDataAsyncCompleted != null)
            {
                this.GetDescriptionsDataAsyncCompleted(this, args);
            }
        }
    }
}