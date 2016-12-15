using System;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal interface IPropertyInfo
    {
        Type PropertyType { get; }
        string Name { get; }
    }
}