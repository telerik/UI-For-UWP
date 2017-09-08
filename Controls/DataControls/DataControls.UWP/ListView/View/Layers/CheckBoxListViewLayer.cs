using System;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class CheckBoxListViewLayer : ListViewLayer
    {
        private Canvas visualPanel;

        public CheckBoxListViewLayer()
        {
            this.visualPanel = new Canvas();
            this.visualPanel.SetValue(Canvas.ZIndexProperty, 10);
        }

        protected internal override UIElement VisualElement
        {
            get
            {
                return this.visualPanel;
            }
        }

        public void ArrangeElement(FrameworkElement container, RadRect rect)
        {
            Canvas.SetTop(container, rect.Y);
            Canvas.SetLeft(container, rect.X);
        }

        internal ItemCheckBoxControl GenerateContainer()
        {
            ItemCheckBoxControl control = new ItemCheckBoxControl();

            control.IsCheckedChanged += this.OnCheckBoxChecked;

            Canvas.SetZIndex(control, 10);
            this.AddVisualChild(control);
            return control;
        }

        internal void MakeVisible(GeneratedItemModel model)
        {
            var visual = model.Container as ItemCheckBoxControl;
            if (visual != null)
            {
                visual.ClearValue(CheckBox.VisibilityProperty);
                visual.IsCheckedChanged += this.OnCheckBoxChecked;
            }
        }

        internal void Collapse(GeneratedItemModel model)
        {
            var visual = model.Container as ItemCheckBoxControl;
            if (visual != null)
            {
                visual.IsCheckedChanged -= this.OnCheckBoxChecked;
                visual.Visibility = Visibility.Collapsed;
            }
        }

        internal int GetGeneratedContainersCount()
        {
            int count = 0;
            if (this.visualPanel != null)
            {
                count = this.visualPanel.Children.Count;
            }

            return count;
        }

        protected internal override void AttachUI(Panel parent)
        {
            parent.Children.Add(this.VisualElement);
        }

        protected internal override void DetachUI(Panel parent)
        {
            parent.Children.Remove(this.VisualElement);
        }

        private void OnCheckBoxChecked(object sender, EventArgs e)
        {
            this.Owner.OnCheckBoxChecked(sender);
        }
    }
}
