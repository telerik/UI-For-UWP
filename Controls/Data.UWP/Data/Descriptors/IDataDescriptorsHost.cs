using System;
using System.Collections.Generic;
using System.Linq;

namespace Telerik.Data.Core
{
    internal interface IDataDescriptorsHost
    {
        IDataProvider CurrentDataProvider { get; }

        IEnumerable<IDataDescriptorPeer> DescriptorPeers { get; }

        void OnDataDescriptorPropertyChanged(DataDescriptor descriptor);
    }
}