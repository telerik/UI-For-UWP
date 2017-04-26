using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Defines the common UI that represents a header within flyout in a <see cref="RadDataGrid"/> instance.
    /// </summary>
    [TemplatePart(Name = "PART_DescriptorContent", Type = typeof(Border))]
    public partial class DataGridFlyoutHeader : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="OuterBorderVisibility"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OuterBorderVisibilityProperty =
            DependencyProperty.Register(nameof(OuterBorderVisibility), typeof(Visibility), typeof(DataGridFlyoutHeader), new PropertyMetadata(Visibility.Collapsed));

        internal RadDataGrid ParentGrid;
        private FrameworkElement descriptorContent;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridFlyoutHeader"/> class.
        /// </summary>
        public DataGridFlyoutHeader()
        {
            this.DefaultStyleKey = typeof(DataGridFlyoutHeader);
        }

        internal event EventHandler DescriptorContentTap;

        /// <summary>
        /// Gets or sets the visibility of the outer border of the control. This border is displayed when the control is being dragged to reorder within the flyout.
        /// </summary>
        public Visibility OuterBorderVisibility
        {
            get
            {
                return (Visibility)this.GetValue(OuterBorderVisibilityProperty);
            }
            set
            {
                this.SetValue(OuterBorderVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets the FrameworkElement instance that represents the content area of the header. Exposed for testing purposes, do not call elsewhere but in test projects.
        /// </summary>
        internal FrameworkElement DescriptorContent
        {
            get
            {
                return this.descriptorContent;
            }
        }

        /// <summary>
        /// Raises the <see cref="DescriptorContentTap"/> event. Exposed for testing purposes, do not call elsewhere but in test projects.
        /// </summary>
        internal void RaiseDescriptorContentTap()
        {
            var eh = this.DescriptorContentTap;
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

            this.descriptorContent = this.GetTemplatePartField<FrameworkElement>("PART_DescriptorContent");
            applied = applied && this.descriptorContent != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();
            this.descriptorContent.Tapped += this.OnContentTapped;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();
            this.descriptorContent.Tapped -= this.OnContentTapped;
        }

        private void OnContentTapped(object sender, TappedRoutedEventArgs e)
        {
            this.RaiseDescriptorContentTap();
        }
    }
}
