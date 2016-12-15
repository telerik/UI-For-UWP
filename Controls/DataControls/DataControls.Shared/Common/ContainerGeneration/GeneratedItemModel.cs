using Telerik.Core;
using Telerik.Data.Core.Layouts;

namespace Telerik.UI.Xaml.Controls.Data.ContainerGeneration
{
    internal class GeneratedItemModel : IGeneratedContainer
    {
        public object ContainerType { get; set; }

        public object Container { get; set; }

        public RadSize DesiredSize { get; set; }

        public ItemInfo ItemInfo { get; set; }

        public RadRect LayoutSlot { get; set; }

        public bool IsAnimating { get; set; }
    }
}
