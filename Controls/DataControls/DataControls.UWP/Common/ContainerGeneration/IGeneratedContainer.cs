using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Data.ContainerGeneration
{
    internal interface IGeneratedContainer : IAnimated
    {
        object ContainerType { get; set; }

        RadSize DesiredSize { get; set; }

        RadRect LayoutSlot { get; }

        ItemInfo ItemInfo { get; set; }
    }
}
