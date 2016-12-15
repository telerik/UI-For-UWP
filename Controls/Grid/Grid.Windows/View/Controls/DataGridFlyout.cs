using System;
using System.Linq;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a UI element, utilized to pop over the grid.
    /// </summary>
    public partial class DataGridFlyout : RadControl
    {
        private DataGridFlyoutPanel container;
        internal Canvas adorner;

        internal DataGridFlyoutPanel Container
        {
            get
            {
                return this.container;
            }
        }

        internal virtual void PrepareUI()
        {

        }

        internal virtual void ClearUI()
        {

        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            // prepare the UI initially
            this.PrepareUI();
        }

        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.container = this.GetTemplatePartField<DataGridFlyoutPanel>("PART_HeadersContainer");
            applied = applied && this.Container != null;

            if (this.Container != null)
                this.Container.Flyout = this;

            this.adorner = this.GetTemplatePartField<Canvas>("PART_AdornerHost");
            applied = applied && this.adorner != null;

            this.InitializeDragDrop();

            return applied;
        }
    }
}
