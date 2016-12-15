using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.Data.Core
{
    internal interface IDataDescriptor : INotifyPropertyChanged
    {
        DescriptionBase EngineDescription { get; }

        void DetachFromHost();

        void AttachToHost(IDataDescriptorsHost host);
    }
}
