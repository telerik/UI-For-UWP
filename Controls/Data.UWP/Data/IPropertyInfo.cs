using System;

namespace Telerik.Data.Core
{
    internal interface IPropertyInfo
    {
        Type PropertyType { get; }

        string Name { get; }
    }
}