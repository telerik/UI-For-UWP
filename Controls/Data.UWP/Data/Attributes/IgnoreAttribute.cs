using System;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Provides an attribute that specifies that the attributed member will be ignored by the UI.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class IgnoreAttribute : Attribute
    {
    }
}