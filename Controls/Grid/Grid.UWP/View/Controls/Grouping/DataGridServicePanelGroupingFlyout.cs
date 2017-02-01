using System;
using System.Linq;
using Telerik.Data.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the UI on the left edge of a <see cref="RadDataGrid"/> instance. Used to access the Grouping Flyout.
    /// </summary>
    [TemplatePart(Name = "PART_HeadersContainer", Type = typeof(DataGridFlyoutPanel))]
    [TemplatePart(Name = "PART_AdornerHost", Type = typeof(Canvas))]
    public partial class DataGridServicePanelGroupingFlyout : DataGridFlyout
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridServicePanelGroupingFlyout" /> class.
        /// </summary>
        public DataGridServicePanelGroupingFlyout()
        {
            this.DefaultStyleKey = typeof(DataGridServicePanelGroupingFlyout);
        }

        internal DataGridServicePanel Owner
        {
            get;
            set;
        }

        internal override void PrepareUI()
        {
            if (this.Owner == null || !this.IsTemplateApplied)
            {
                return;
            }

            this.Container.Owner = this.Owner.Owner;

            for (int i = 0; i < this.Owner.Owner.GroupDescriptors.Count; i++)
            {
                GroupDescriptorBase descriptor = this.Owner.Owner.GroupDescriptors[i];

                DataGridFlyoutGroupHeader header = new DataGridFlyoutGroupHeader();

                this.SetupDragDropProperties(header, i);

                header.DataContext = descriptor;
                header.CloseButtonClick += this.Owner.HandleGroupFlyoutHeaderClosed;
                header.DragSurfaceRequested += this.HandleDragSurfaceRequested;
                header.DescriptorContentTap += this.Owner.HandleGroupFlyoutDescriptorContentTap;
                header.ParentGrid = this.Owner.Owner;

                this.Container.Elements.Add(header);
            }

            int childCount = this.Container.Elements.Count;
            if (childCount > 0)
            {
                var lastHeader = this.Container.Elements[childCount - 1] as DataGridFlyoutGroupHeader;
                lastHeader.BottomGlyphOpacity = 0.0;
            }
        }

        internal override void ClearUI()
        {
            foreach (DataGridFlyoutGroupHeader header in this.Container.Children)
            {
                header.CloseButtonClick -= this.Owner.HandleGroupFlyoutHeaderClosed;
                header.DragSurfaceRequested -= this.HandleDragSurfaceRequested;
                header.DescriptorContentTap -= this.Owner.HandleGroupFlyoutDescriptorContentTap;
            }

            this.Container.ClearItems(); 
        }
    }
}
