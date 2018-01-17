using System;
using Telerik.UI.Xaml.Controls.Grid;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace SDKExamples.UWP.DataGrid
{
    public class DataGridRatingColumn : DataGridTypedColumn
    {
        private static Type ratingType = typeof(RadRating);
        private static Type iconsPanel = typeof(StackPanel);

        public override object GetContainerType(object rowItem)
        {
            return iconsPanel;
        }

        public override object GetEditorType(object item)
        {
            if (!string.IsNullOrEmpty(this.PropertyName) && this.CanUserEdit)
            {
                return iconsPanel;
            }

            return ratingType;
        }

        public override object CreateContainer(object rowItem)
        {
            return new StackPanel() { Orientation = Orientation.Horizontal };
        }

        public override FrameworkElement CreateEditorContentVisual()
        {
            var rating = new RadRating();
            return rating;
        }

        public override void PrepareCell(object container, object value, object item)
        {
            StackPanel ratingPanel = container as StackPanel;
            if (ratingPanel == null)
            {
                return;
            }

            if (value == null)
            {
                ratingPanel.Children.Clear();
                return;
            }

            int starsCount;
            if (int.TryParse(value.ToString(), out starsCount))
            {
                ratingPanel.Children.Clear();
                for (int i = 0; i < starsCount; i++)
                {
                    var symbolIcon = new SymbolIcon(Symbol.SolidStar);
                    ratingPanel.Children.Add(symbolIcon);
                }
            }
        }

        public override void PrepareEditorContentVisual(FrameworkElement editorContent, Binding binding)
        {
            editorContent.SetBinding(RadRating.ValueProperty, binding);
        }

        public override void ClearEditorContentVisual(FrameworkElement editorContent)
        {
            editorContent.ClearValue(RadRating.ValueProperty);
        }

        protected override DataGridFilterControlBase CreateFilterControl()
        {
            return new DataGridCustomFilteringControl()
            {
                PropertyName = this.PropertyName
            };
        }
    }
}
