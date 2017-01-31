using System;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a component that does not have a desired size - such as RadChart.
    /// In this case the entire UI update happens in the Arrange pass and virtualizing components like RadDataGrid need an entry point to invalidate control's current state.
    /// </summary>
    internal interface INoDesiredSizeControl
    {
        void InvalidateUI();
    }
}
