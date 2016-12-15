using System;
using Telerik.Data.Core;

namespace Telerik.Core.Data
{
    internal class DataGroupFactory : IGroupFactory
    {
        public Group CreateGroup(object key)
        {
            return new Telerik.Data.Core.DataGroup(key);
        }
    }
}
