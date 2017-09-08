using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Telerik.Data.Core.Fields
{
    /// <summary>
    /// A base class for various FieldInfo classes presenting local sources. An implementation of <see cref="IFieldDescriptionProvider"/>.
    /// </summary>
    internal abstract class LocalFieldDescriptionsProviderBase : FieldDescriptionProviderBase
    {
        private IFieldInfoData data;

        /// <summary>
        /// Gets the object which FieldDescriptions are generated.
        /// </summary>
        public object CurrentState { get; private set; }

        /// <inheritdoc />
        public override void GetDescriptionsDataAsync(object state)
        {
            if (this.CurrentState != state || this.data == null)
            {
                if (!this.IsReady || (state == null && this.IsDynamic) || state != null)
                {
                    this.CurrentState = state;
                    this.GetDescriptionsData();
                }
            }
            else if (!this.IsBusy)
            {
                this.OnDescriptionsDataCompleted(new GetDescriptionsDataCompletedEventArgs(null, this.CurrentState, this.data));
            }
        }

        /// <summary>
        /// Retrieves the DescriptionsData for data source.
        /// </summary>
        /// <returns>DescriptionsData instance.</returns>
        protected abstract IFieldInfoData GenerateDescriptionsData();

        /// <summary>
        /// Gets the field description hierarchy.
        /// </summary>
        /// <param name="fieldInfos">Collection of <see cref="IDataFieldInfo"/> instances.</param>
        protected virtual ContainerNode GetFieldDescriptionHierarchy(IEnumerable<IDataFieldInfo> fieldInfos)
        {
            if (fieldInfos == null)
            {
                throw new ArgumentNullException(nameof(fieldInfos));
            }

            var root = ContainerNode.CreateRootNode();

            foreach (var fieldInfoItem in fieldInfos)
            {
                var fieldDescriptionNode = new FieldInfoNode(fieldInfoItem);
                root.Children.Add(fieldDescriptionNode);
            }

            return root;
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Design choice.")]
        private void GetDescriptionsData()
        {
            Exception error = null;

            try
            {
                this.IsBusy = true;
                this.data = this.GenerateDescriptionsData();
            }
            catch (Exception e)
            {
                error = e;
                this.data = new EmptyFieldInfoData();
            }
            finally
            {
                this.OnDescriptionsDataCompleted(new GetDescriptionsDataCompletedEventArgs(error, this.CurrentState, this.data));
            }
        }
    }
}