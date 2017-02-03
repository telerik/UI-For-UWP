using System;
using System.Reflection;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// Provides data for the <see cref="IFieldDescriptionProvider.GetDescriptionsDataAsyncCompleted"/> event.
    /// </summary>
    internal class GetDescriptionsDataCompletedEventArgs : EventArgs
    {
        private IFieldInfoData descriptionsData;

        /// <summary>
        /// Initializes a new instance of the <see cref="GetDescriptionsDataCompletedEventArgs"/> class.
        /// </summary>
        public GetDescriptionsDataCompletedEventArgs(Exception error, object userState, IFieldInfoData data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            this.DescriptionsData = data;
            this.Error = error;
            this.State = userState;
        }

        /// <summary>
        /// Gets a value indicating which error occurred during an operation.
        /// </summary>
        /// <value>The error.</value>
        public Exception Error
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the unique identifier for the asynchronous operation.
        /// </summary>
        /// <value>Identifier.</value>
        public object State
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets information about available fields/properties. 
        /// </summary>
        public IFieldInfoData DescriptionsData
        {
            get
            {
                this.RaiseExceptionIfNecessary();

                return this.descriptionsData;
            }

            private set
            {
                this.descriptionsData = value;
            }
        }

        private void RaiseExceptionIfNecessary()
        {
            if (this.Error != null)
            {
                throw new TargetInvocationException("Cannot access data because an error has occurred.", this.Error);
            }
        }
    }
}