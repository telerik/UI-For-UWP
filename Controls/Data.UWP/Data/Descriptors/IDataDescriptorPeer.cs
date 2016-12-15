using System;
using System.Linq;

namespace Telerik.Data.Core
{
    internal interface IDataDescriptorPeer
    {
        bool IsAssociatedWithDescriptor(IPropertyDescriptor descriptor);

        void OnDescriptorAssociated(DataDescriptor descriptor);

        void OnAssociatedDescriptorPropertyChanged(DataDescriptor descriptor, string propertyName);

        void OnAssociatedDescriptorRemoved(DataDescriptor descriptor);
    }
}