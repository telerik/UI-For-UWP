using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.Core;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal class ListViewItemCheckBoxService : ServiceBase<RadListView>
    {
        internal CheckBoxesContainerGenerator containerGenerator;
        internal CheckBoxGenerator generator;
        internal bool itemsAnimated = true;
        private IDictionary<int, GeneratedItemModel> generatedContainers = new Dictionary<int, GeneratedItemModel>();
        private ObservableCollection<object> selectedItems = new ObservableCollection<object>();
        private RadListViewItem tappedItem = null;

        public ListViewItemCheckBoxService(RadListView owner)
            : base(owner)
        {
            this.containerGenerator = new CheckBoxesContainerGenerator(this);
            this.generator = new CheckBoxGenerator(this.containerGenerator);
        }

        internal event EventHandler<CheckModeActiveChangedEventArgs> IsCheckModeActiveChanged;

#pragma warning disable 0067

        internal event EventHandler<ItemCheckedStateChangedEventArgs> ItemCheckedStateChanged;

#pragma warning restore 0067

        public bool IsCheckModeActive
        {
            get
            {
                return this.Owner.IsCheckModeActive;
            }
        }

        internal double VisualLength { get; set; }

        public FrameworkElement GenerateContainer()
        {
            return this.Owner.checkBoxLayerCache.GenerateContainer();
        }
        
        public void ClearContainerForItem(GeneratedItemModel model)
        {
            this.Owner.checkBoxLayerCache.Collapse(model);
        }

        public void PrepareContainerForItem(GeneratedItemModel element)
        {
            var control = element.Container as ItemCheckBoxControl;

            if (control != null)
            {
                this.Owner.checkBoxLayerCache.MakeVisible(element);
                control.DataContext = element.ItemInfo.Item;
                if (this.selectedItems.Contains(element.ItemInfo.Item))
                {
                    control.SetIsChecked(true);
                }
                else
                {
                    control.SetIsChecked(false);
                }

                control.CheckBoxStyle = this.Owner.ItemCheckBoxStyle;
            }
        }

        public void GenerateVisuals()
        {
            this.RecycleLocally();
            if (this.Owner.SelectionMode == DataControlsSelectionMode.MultipleWithCheckBoxes && this.IsCheckModeActive && this.Owner.LayoutDefinition.GetType().Equals(typeof(StackLayoutDefinition)))
            {
                foreach (var item in this.Owner.Model.ForEachDisplayedElement())
                {
                    GeneratedItemModel decorator = null;
                    if (item.ContainerType.Equals(typeof(RadListViewItem)))
                    {
                        decorator = this.generator.GenerateContainer(item.ItemInfo, item);
                        this.PrepareContainerForItem(decorator);
                        this.generatedContainers[item.ItemInfo.Id] = decorator;
                    }
                }
            }
            this.RecycleUnused();
        }
        
        public void ArrangeVisuals()
        {
            var orientation = this.Owner.Orientation;
            foreach (var item in this.Owner.Model.ForEachDisplayedElement())
            {
                GeneratedItemModel model = null;
                this.generatedContainers.TryGetValue(item.ItemInfo.Id, out model);

                if (model != null && model.Container != null)
                {
                    RadRect arrangeRect = new RadRect();
                    var control = model.Container as ItemCheckBoxControl;
                    control.Height = orientation == Orientation.Vertical ? item.LayoutSlot.Height : control.DesiredSize.Height;
                    control.Width = orientation == Orientation.Vertical ? control.DesiredSize.Width : item.LayoutSlot.Width;
                    arrangeRect.Width = orientation == Orientation.Vertical ? control.DesiredSize.Width : item.LayoutSlot.Width;
                    arrangeRect.Height = orientation == Orientation.Vertical ? item.LayoutSlot.Height : control.DesiredSize.Height;
                    var beforeX = item.LayoutSlot.X;
                    var afterX = orientation == Orientation.Vertical ? item.LayoutSlot.Right - control.DesiredSize.Width : item.LayoutSlot.X;
                    var beforeY = item.LayoutSlot.Y;
                    var afterY = orientation == Orientation.Vertical ? item.LayoutSlot.Y : item.LayoutSlot.Bottom - control.DesiredSize.Height;
                    arrangeRect.X = this.Owner.ItemCheckBoxPosition == CheckBoxPosition.BeforeItem ? beforeX : afterX;
                    arrangeRect.Y = this.Owner.ItemCheckBoxPosition == CheckBoxPosition.BeforeItem ? beforeY : afterY;
                    this.Owner.checkBoxLayerCache.ArrangeElement(model.Container as FrameworkElement, arrangeRect);
                    this.VisualLength = orientation == Orientation.Vertical ? arrangeRect.Width : arrangeRect.Height;
                }

                var listView = this.Owner;
                if (!this.itemsAnimated && this.Owner.checkBoxLayerCache.GetGeneratedContainersCount() > 0)
                {
                    listView.animationSurvice.PlayCheckModeAnimation(listView.childrenPanel, listView.IsCheckModeActive, listView.ItemCheckBoxPosition == CheckBoxPosition.BeforeItem, this.VisualLength);
                    listView.animationSurvice.PlayCheckBoxLayerAnimation(listView.checkBoxLayerCache.VisualElement, listView.IsCheckModeActive, listView.ItemCheckBoxPosition == CheckBoxPosition.BeforeItem, this.VisualLength);
                    this.itemsAnimated = true;
                }
            }
        }

        internal void OnIsCheckModeActiveChanged()
        {
            var handler = this.IsCheckModeActiveChanged;
            if (handler != null)
            {
                handler(this.Owner, new CheckModeActiveChangedEventArgs(this.IsCheckModeActive, this.tappedItem, this.selectedItems));
            }
            this.tappedItem = null;
            this.Owner.updateService.RegisterUpdate((int)UpdateFlags.AffectsContent);
        }

        internal void RefreshSelection()
        {
            this.selectedItems = this.Owner.SelectedItems;
        }

        internal void OnItemTap(RadListViewItem listViewItem)
        {
            this.tappedItem = listViewItem;
            this.Owner.IsCheckModeActive = true;
        }

        internal void SelectItem(object item, bool select)
        {
            bool needsUpdate = false;
            if (select && !this.selectedItems.Contains(item))
            {
                needsUpdate = true;
                this.selectedItems.Add(item);
            }
            else if (!select && this.selectedItems.Contains(item))
            {
                needsUpdate = true;
                this.selectedItems.Remove(item);
            }

            if (needsUpdate)
            {
                this.OnItemSelected(item, select);
            }
        }

        internal void FullyRecycle()
        {
            foreach (var decorator in this.generatedContainers)
            {
                this.Recycle(decorator.Value);
            }
            this.generatedContainers.Clear();
        }

        internal void OnItemSelected(object item, bool selected)
        {
            var model = this.generatedContainers.Where((pair) =>
            {
                if (pair.Value.ItemInfo.Item.Equals(item))
                {
                    return true;
                }
                return false;
            }).FirstOrDefault();

            if (model.Value != null)
            {
                var checkBox = model.Value.Container as ItemCheckBoxControl;
                if (checkBox != null)
                {
                    checkBox.SetIsChecked(selected);
                }
            }
        }

        internal void OnCheckBoxChecked(object sender)
        {
            var checkBox = sender as CheckBox;
            if (checkBox.IsChecked == true)
            {
                if (!this.selectedItems.Contains(checkBox.DataContext))
                {
                    this.selectedItems.Add(checkBox.DataContext);
                    this.Owner.selectionService.SelectItem(checkBox.DataContext, true, true);
                }
            }
            else
            {
                this.selectedItems.Remove(checkBox.DataContext);
                this.Owner.selectionService.SelectItem(checkBox.DataContext, false, true);
            }
        }

        /// <summary>
        /// Gets a value indicating whether an item is selected. Exposed for testing purposes.
        /// </summary>
        internal bool IsItemSelected(object item)
        {
            return this.selectedItems.Contains(item);
        }

        /// <summary>
        /// Gets a value indicating whether a checkbox is selected. Exposed for testing purposes.
        /// </summary>
        internal bool IsCheckBoxSelected(object item)
        {
            bool selected = true;

            var model = this.generatedContainers.Where((pair) =>
            {
                if (pair.Value.ItemInfo.Item.Equals(item))
                {
                    return true;
                }
                return false;
            }).FirstOrDefault();

            if (model.Value != null)
            {
                var checkBox = model.Value.Container as ItemCheckBoxControl;
                if (checkBox != null)
                {
                    selected = selected && checkBox.checkBox.IsChecked.Value;
                }
                else
                {
                    selected = false;
                }
            }
            else
            {
                selected = selected && this.selectedItems.Contains(item);
            }

            return selected;
        }

        internal void ClearSelection()
        {
            foreach (var model in this.generatedContainers.Values)
            {
                var checkbox = model.Container as ItemCheckBoxControl;
                if (checkbox != null)
                {
                    checkbox.SetIsChecked(false);
                }
            }

            this.selectedItems.Clear();
        }
        
        private void RecycleLocally()
        {
            foreach (var decorator in this.generatedContainers)
            {
                this.generator.RecycleDecorator(decorator.Value);
            }
            this.generatedContainers.Clear();
        }

        private void RecycleUnused()
        {
            this.generator.FullyRecycleDecorators();
        }

        private void Recycle(GeneratedItemModel decorator)
        {
            this.generator.RecycleDecorator(decorator);
        }
    }
}
