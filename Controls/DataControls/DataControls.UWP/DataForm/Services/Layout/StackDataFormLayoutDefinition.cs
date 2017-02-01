using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.DataForm
{
    public class StackDataFormLayoutDefinition : DataFormLayoutDefinition
    {
        public Orientation Orientation
        {
            get { return (Orientation)GetValue(OrientationProperty); }
            set { SetValue(OrientationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Orientation.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register("Orientation", typeof(Orientation), typeof(StackDataFormLayoutDefinition), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

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

        protected internal override Panel CreateGroupLayoutPanel(string groupKey)
        {
            return new StackPanel() { Orientation = this.Orientation };
        }


        protected internal override Panel CreateDataFormPanel()
        {
            return new StackPanel() { Orientation = this.Orientation };
        }

        protected internal override void SetEditorArrangeMetadata(EntityPropertyControl editorElement, Telerik.Data.Core.EntityProperty entityProperty, Panel parentPanel)
        {
            var stackPanel = parentPanel as StackPanel;

            if (stackPanel != null)
            {
                stackPanel.Orientation = this.Orientation;
            }

            base.SetEditorArrangeMetadata(editorElement, entityProperty, parentPanel);
        }


        protected internal override void SetEditorElementsArrangeMetadata(EntityPropertyControl editorElement, Telerik.Data.Core.EntityProperty entityProperty)
        {
            editorElement.ColumnCount = 0;
            editorElement.RowCount = 3;
            editorElement.LabelRow = 0;
            editorElement.ViewRow = 1;
            editorElement.ErrorViewRow = 2;
            editorElement.PositiveMessageViewRow = 2;
        }
    }
}
