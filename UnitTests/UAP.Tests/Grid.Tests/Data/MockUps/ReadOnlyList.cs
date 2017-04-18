using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public class ReadOnlyList<T> : List<T>, IReadOnlyList<T>
    {
    }
}
