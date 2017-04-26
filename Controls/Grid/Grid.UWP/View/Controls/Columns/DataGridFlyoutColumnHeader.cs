using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents a DataGridFlyoutColumnHeader control.
    /// </summary>
    [TemplatePart(Name = "PART_SelectCheckBox", Type = typeof(CheckBox))]
    public partial class DataGridFlyoutColumnHeader : DataGridFlyoutHeader
    {
        private CheckBox selectCheckBox;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridFlyoutColumnHeader"/> class.
        /// </summary>
        public DataGridFlyoutColumnHeader()
        {
            this.DefaultStyleKey = typeof(DataGridFlyoutColumnHeader);
        }

        internal event EventHandler SelectionCheck;
        internal event EventHandler SelectionUncheck;

        /// <summary>
        /// Raises the <see cref="SelectionCheck"/> event. Exposed for testing purposes, do not call elsewhere but in test projects.
        /// </summary>
        internal void RaiseSelectionCheck()
        {
            var eh = this.SelectionCheck;
            if (eh != null)
            {
                eh(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Raises the <see cref="SelectionUncheck"/> event. Exposed for testing purposes, do not call elsewhere but in test projects.
        /// </summary>
        internal void RaiseSelectionUncheck()
        {
            var eh = this.SelectionUncheck;
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

            this.selectCheckBox = this.GetTemplatePartField<CheckBox>("PART_SelectCheckBox");
            applied = applied && this.selectCheckBox != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.selectCheckBox.Checked += this.SelectCheckBox_Checked;
            this.selectCheckBox.Unchecked += this.SelectCheckBox_Unchecked;
        }
        
        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();
            this.selectCheckBox.Checked -= this.SelectCheckBox_Checked;
            this.selectCheckBox.Unchecked -= this.SelectCheckBox_Unchecked;
        }

        /// <inheritdoc/>
        protected override DataGridFlyoutHeader CreateHeader()
        {
            DataGridFlyoutColumnHeader header = new DataGridFlyoutColumnHeader();
            header.Width = this.ActualWidth;
            header.OuterBorderVisibility = Visibility.Visible;
            return header;
        }

        private void SelectCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            this.RaiseSelectionUncheck();
        }

        private void SelectCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            this.RaiseSelectionCheck();
        }

        private void OnContentTapped(object sender, TappedRoutedEventArgs e)
        {
            this.RaiseDescriptorContentTap();
        }
    }
}
