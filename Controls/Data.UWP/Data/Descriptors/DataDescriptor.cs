using System;
using Telerik.Core;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Represents an abstraction that is related to a data operation within a data component.
    /// Data operations for example are sorting, grouping and filtering.
    /// </summary>
    public abstract class DataDescriptor : ViewModelBase
    {
        private WeakReference<IDataDescriptorPeer> descriptorPeer;

        internal IDataDescriptorsHost Host { get; set; }

        internal virtual DataChangeFlags UpdateFlags
        {
            get
            {
                return DataChangeFlags.None;
            }
        }

        internal abstract DescriptionBase EngineDescription { get; }

        /// <summary>
        /// Gets a value indicating whether the current instance is DelegateDataDescriptor - that is to have a user-specified member access.
        /// </summary>
        internal virtual bool IsDelegate
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IDataDescriptorPeer"/> object associated with this descriptor.
        /// This property is updated when the descriptor is added to the corresponding Descriptors collection of the owning data component.
        /// </summary>
        internal IDataDescriptorPeer DescriptorPeer
        {
            get
            {
                if (this.descriptorPeer == null)
                {
                    return null;
                }

                IDataDescriptorPeer column;
                if (this.descriptorPeer.TryGetTarget(out column))
                {
                    return column;
                }

                return null;
            }
            set
            {
                this.descriptorPeer = new WeakReference<IDataDescriptorPeer>(value);
            }
        }

        internal abstract bool Equals(DescriptionBase description);

        internal void Attach(IDataDescriptorsHost model)
        {
            this.Host = model;
            this.UpdateAssociatedPeer();
            this.AttachOverride();
        }

        internal void Detach()
        {
            this.Host = null;
            this.DetachAssociatedPeer();
        }

        internal virtual bool IsAssociatedPeer(IDataDescriptorPeer peer)
        {
            var propertyDescriptor = this as IPropertyDescriptor;
            if (propertyDescriptor == null)
            {
                return false;
            }

            if (peer != null)
            {
                return peer.IsAssociatedWithDescriptor(propertyDescriptor);
            }

            return false;
        }

        internal virtual void AttachOverride()
        {
        }

        /// <summary>
        /// Resolves the object that is considered as "Associated" with the current instance.
        /// </summary>
        internal void UpdateAssociatedPeer(IDataDescriptorPeer peer = null)
        {
            this.DetachAssociatedPeer();

            if (peer == null)
            {
                peer = this.FindAssociatedPeer();
            }

            this.DescriptorPeer = peer;

            if (peer != null)
            {
                peer.OnDescriptorAssociated(this);
            }
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        protected override void PropertyChangedOverride(string changedPropertyName)
        {
            base.PropertyChangedOverride(changedPropertyName);

            var peer = this.DescriptorPeer;
            if (peer != null)
            {
                peer.OnAssociatedDescriptorPropertyChanged(this, changedPropertyName);
            }
        }

        private void DetachAssociatedPeer()
        {
            var column = this.DescriptorPeer;
            if (column != null)
            {
                column.OnAssociatedDescriptorRemoved(this);
            }
        }

        private IDataDescriptorPeer FindAssociatedPeer()
        {
            if (this.Host == null)
            {
                return null;
            }

            foreach (var column in this.Host.DescriptorPeers)
            {
                if (this.IsAssociatedPeer(column))
                {
                    return column;
                }
            }

            return null;
        }
    }
}