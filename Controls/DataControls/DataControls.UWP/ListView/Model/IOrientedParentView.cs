using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.ListView
{
    internal interface IOrientedParentView
    {
        Orientation Orientation { get; set; }
        
        double ScrollOffset { get; }

        RadSize Measure(IGeneratedContainer container, RadSize availableSize);

        void Arrange(IGeneratedContainer container);
    }
}
