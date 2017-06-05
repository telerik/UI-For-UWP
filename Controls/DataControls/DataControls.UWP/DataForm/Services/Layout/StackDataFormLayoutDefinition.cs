using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    /// <summary>
    /// Represents an StackDataFormLayoutDefinition layout definition.
    /// </summary>
    public class StackDataFormLayoutDefinition : DataFormLayoutDefinition
    {
        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(StackDataFormLayoutDefinition), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        /// <summary>
        /// Gets or sets the orientation of the StackDataFrom definition.
        /// </summary>
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { this.SetValue(OrientationProperty, value); }
        }

        /// <inheritdoc/>
        protected internal override Panel CreateGroupLayoutPanel(string groupKey)
        {
            return new StackPanel() { Orientation = this.Orientation };
        }

        /// <inheritdoc/>
        protected internal override Panel CreateDataFormPanel()
        {
            return new StackPanel() { Orientation = this.Orientation };
        }

        /// <inheritdoc/>
        protected internal override void SetEditorArrangeMetadata(EntityPropertyControl editorElement, Telerik.Data.Core.EntityProperty entityProperty, Panel parentPanel)
        {
            var stackPanel = parentPanel as StackPanel;

            if (stackPanel != null)
            {
                stackPanel.Orientation = this.Orientation;
            }

            base.SetEditorArrangeMetadata(editorElement, entityProperty, parentPanel);
        }

        /// <inheritdoc/>
        protected internal override void SetEditorElementsArrangeMetadata(EntityPropertyControl editorElement, Telerik.Data.Core.EntityProperty entityProperty)
        {
            editorElement.ColumnCount = 0;
            editorElement.RowCount = 3;
            editorElement.LabelRow = 0;
            editorElement.ViewRow = 1;
            editorElement.ErrorViewRow = 2;
            editorElement.PositiveMessageViewRow = 2;
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as StackDataFormLayoutDefinition;

            if (definition.Owner == null)
            {
                return;
            }

            var panel = definition.Owner.GetRootPanel() as StackPanel;

            if (panel != null)
            {
                panel.Orientation = definition.Orientation;
                definition.Owner.RefreshFormLayout();
            }
        }
    }
}
