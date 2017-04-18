using System;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Defines the UI that represents a <see cref="DataGridFlyoutGroupHeader"/> within the Grouping Flyout in a <see cref="RadDataGrid"/> instance.
    /// </summary>
    
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(InlineButton))]   
    public partial class DataGridFlyoutGroupHeader : DataGridFlyoutHeader
    {
        /// <summary>
        /// Identifies the <see cref="BottomGlyphOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty BottomGlyphOpacityProperty =
            DependencyProperty.Register(nameof(BottomGlyphOpacity), typeof(double), typeof(DataGridFlyoutGroupHeader), new PropertyMetadata(1));
        private Button closeButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridFlyoutGroupHeader" /> class.
        /// </summary>
        public DataGridFlyoutGroupHeader()
        {
            this.DefaultStyleKey = typeof(DataGridFlyoutGroupHeader);
        }

        /// <summary>
        /// Gets or sets the visibility of the Glyph at the bottom of the control. This glyph visually emphasizes the order of the headers within the flyout control.
        /// </summary>
        public double BottomGlyphOpacity
        {
            get
            {
                return (double) this.GetValue(BottomGlyphOpacityProperty);
            }
            set
            {
                this.SetValue(BottomGlyphOpacityProperty, value);
            }
        }

        internal event EventHandler CloseButtonClick;

        /// <summary>
        /// Raises the <see cref="CloseButtonClick"/> event. Exposed for testing purposes, do not call elsewhere but in test projects.
        /// </summary>
        internal void RaiseCloseButtonClick()
        {
            var eh = this.CloseButtonClick;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.closeButton = this.GetTemplatePartField<Button>("PART_CloseButton");
            applied = applied && this.closeButton != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.closeButton.Click += this.OnCloseButtonClick;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();
            this.closeButton.Click -= this.OnCloseButtonClick;
        }

        private void OnContentTapped(object sender, TappedRoutedEventArgs e)
        {
            this.RaiseDescriptorContentTap();
        }

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            this.RaiseCloseButtonClick();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridFlyoutGroupHeaderAutomationPeer(this);
        }
    }
}
