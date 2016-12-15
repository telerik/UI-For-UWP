using System;
using Telerik.Data.Core;

namespace Telerik.Data.Core
{
    internal interface IGroupFactory
    {
        Group CreateGroup(object key);
    }
}
