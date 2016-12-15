using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal struct ChartAnnotationLabelUpdateContext
    {
        public RadPoint Location;
        public RadRect LayoutSlot;
        public ChartAnnotationLabelDefinition Definition;

        public ChartAnnotationLabelUpdateContext(RadRect layoutSlot)
        {
            this.LayoutSlot = layoutSlot;
            this.Location = layoutSlot.Location;

            this.Definition = null;
        }
    }
}
