using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides an attribute that specifies that the attributed member value will not be edited by the UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ReadOnlyAttribute : Attribute
    {
    }
}