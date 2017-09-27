using System;
using System.Linq;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class XamlGridRowGenerator : IUIContainerGenerator<GridRowModel, RowGenerationContext>
    {
        private const int MaxGroupCount = 20;

        private static Type loadingDataControlType = typeof(DataGridLoadDataControl);
        private static Type groupHeaderType = typeof(DataGridGroupHeader);
        private static object frozenHeader = new object();
        private static Type placeholderModelType = typeof(PlaceholderInfo);

        private static Type rowDetailType = typeof(DataGridRowDetailsControl);

        private RadDataGrid owner;

        public XamlGridRowGenerator(RadDataGrid owner)
        {
            this.owner = owner;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void PrepareContainerForItem(GridRowModel decorator)
        {
            this.PrepareLoadingDataControl(decorator.Container as DataGridLoadDataControl);
            this.PrepareExpandedRowDetailsControl(decorator, decorator.Container as DataGridRowDetailsControl);
            this.PrepareGroupRow(decorator, decorator.Container as DataGridGroupHeader);
        }

        private void PrepareExpandedRowDetailsControl(GridRowModel decorator, DataGridRowDetailsControl control)
        {
            if (control != null)
            {
                control.Content = decorator.ItemInfo.Item;

                if (this.owner.RowDetailsTemplate != control.ContentTemplate)
                {
                    control.ContentTemplate = this.owner.RowDetailsTemplate;
                }
            }
        }

        private void PrepareLoadingDataControl(DataGridLoadDataControl control)
        {
            if (control != null)
            {
                control.Owner = this.owner;
                this.owner.visualStateService.RegisterDataLoadingListener((IDataStatusListener)control);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void ClearContainerForItem(GridRowModel decorator)
        {
            DataGridLoadDataControl control = decorator.Container as DataGridLoadDataControl;

            if (control != null)
            {
                control.Owner = null;
                this.owner.visualStateService.UnregisterDataLoadingListener((IDataStatusListener)control);
            }

            DataGridGroupHeader groupHeader = decorator.Container as DataGridGroupHeader;
            if (groupHeader != null)
            {
                groupHeader.Owner = null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public object GetContainerTypeForItem(RowGenerationContext context)
        {
            var item = context.Info;
            bool isFrozen = context.IsFrozen;

            if (item.IsCollapsible)
            {
                if (isFrozen)
                {
                    return frozenHeader;
                }

                return groupHeaderType;
            }

            if (item.Item.GetType() == placeholderModelType)
            {
                return loadingDataControlType;
            }

            //TODO: detach this if possible from the owner
            if (this.owner.rowDetailsService.HasExpandedRowDetails(context.Info.Item))
            {
                return rowDetailType;
            }

            return typeof(object);
        }

        public object GenerateContainerForItem(RowGenerationContext info, object containerType)
        {
            DataGridGroupHeader groupHeader = null;

            object generatedContent = null;

            if (groupHeaderType.Equals(containerType))
            {
                groupHeader = new DataGridGroupHeader();
            }
            else if (object.Equals(frozenHeader, containerType))
            {
                // Frozen Headers are placed in different panel (not in cells panel).
                groupHeader = new DataGridGroupHeader(true);
            }

            if (groupHeader != null)
            {
                if (groupHeader.IsFrozen)
                {
                    this.owner.FrozenGroupHeadersHost.Children.Add(groupHeader);
                }
                else
                {
                    var contentLayer = this.owner.GroupHeadersContentLayer;
                    if (contentLayer != null)
                    {
                        contentLayer.AddVisualChild(groupHeader);
                    }
                }

                generatedContent = groupHeader;
            }

            if (containerType != null && containerType.Equals(loadingDataControlType))
            {
                var control = new DataGridLoadDataControl();

                var contentLayer = this.owner.GetContentLayerForColumn(null);
                if (contentLayer != null)
                {
                    contentLayer.AddVisualChild(control);
                }

                generatedContent = control;
            }

            if (rowDetailType.Equals(containerType))
            {
                generatedContent = this.owner.rowDetailsService.DetailsPresenter;
                this.owner.rowDetailsService.DetailsPresenter.Visibility = Visibility.Visible;
            }

            return generatedContent;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void MakeVisible(GridRowModel decorator)
        {
            MakeVisibleCore(decorator.Container as UIElement);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        public void MakeHidden(GridRowModel decorator)
        {
            MakeHiddenCore(decorator.Container as UIElement);
        }

        public void SetOpacity(GridRowModel decorator, byte opacity)
        {
            if (decorator != null)
            {
                UIElement container = decorator.Container as UIElement;
                if (container != null && !(container is DataGridRowDetailsControl))
                {
                    container.Opacity = opacity;
                }
            }
        }

        private static void MakeVisibleCore(UIElement container)
        {
            if (container != null)
            {
                container.ClearValue(UIElement.VisibilityProperty);
            }
        }

        private static void MakeHiddenCore(UIElement container)
        {
            if (container != null)
            {
                container.Visibility = Visibility.Collapsed;
                container.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            }
        }

        private void PrepareGroupRow(GridRowModel decorator, DataGridGroupHeader groupHeader)
        {
            if (groupHeader != null)
            {
                groupHeader.Owner = this.owner;

                var context = new GroupHeaderContext()
                {
                    Level = decorator.ItemInfo.Level,
                    Descriptor = this.owner.GroupDescriptors[decorator.ItemInfo.Level],
                    Group = decorator.ItemInfo.Item as IDataGroup,
                    IsExpanded = !decorator.ItemInfo.IsCollapsed,
                    Grid = this.owner
                };
                groupHeader.DataContext = context;
                groupHeader.IsExpanded = context.IsExpanded;

                this.UpdateGroupHeaderStyleAndTemplate(groupHeader, context);

                groupHeader.IndentWidth = this.owner.IndentWidth * decorator.ItemInfo.Level;

                if (groupHeader.IsFrozen)
                {
                    // We assume that no more that 20 GroupDescriptions will be added.
                    // If needed this number can be increased.
                    Canvas.SetZIndex(groupHeader, MaxGroupCount - decorator.ItemInfo.Level);
                }
            }
        }

        private void UpdateGroupHeaderStyleAndTemplate(DataGridGroupHeader groupHeader, GroupHeaderContext context)
        {
            var style = this.owner.GroupHeaderStyle;
            if (style == null)
            {
                if (this.owner.GroupHeaderStyleSelector != null)
                {
                    style = this.owner.GroupHeaderStyleSelector.SelectStyle(context, groupHeader);
                }
            }

            if (style != null)
            {
                groupHeader.Style = style;
            }
            else
            {
                groupHeader.ClearValue(DataGridGroupHeader.StyleProperty);
            }

            var dataTemplate = this.owner.GroupHeaderTemplate;
            if (dataTemplate == null)
            {
                if (this.owner.GroupHeaderTemplateSelector != null)
                {
                    dataTemplate = this.owner.GroupHeaderTemplateSelector.SelectTemplate(context, groupHeader);
                }
            }

            if (dataTemplate != null)
            {
                groupHeader.ContentTemplate = dataTemplate;
            }
            else
            {
                groupHeader.ClearValue(DataGridGroupHeader.ContentTemplateProperty);
            }
        }
    }
}