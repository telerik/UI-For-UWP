using System;
using System.Collections.Generic;
using System.Text;
using Telerik.UI.Xaml.Controls.Data.ListView;
using Windows.UI.Xaml;

namespace Telerik.UI.Xaml.Controls.Data
{
   public partial class RadListView
    {
        /// <summary>
        /// Identifies the <see cref="IsCheckModeActive"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsCheckModeActiveProperty =
            DependencyProperty.Register(nameof(IsCheckModeActive), typeof(bool), typeof(RadListView), new PropertyMetadata(false, OnIsCheckModeActiveChanged));



        public CheckBoxPosition ItemCheckBoxPosition
        {
            get { return (CheckBoxPosition)GetValue(ItemCheckBoxPositionProperty); }
            set { SetValue(ItemCheckBoxPositionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemCheckBoxPosition.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemCheckBoxPositionProperty =
            DependencyProperty.Register(nameof(ItemCheckBoxPosition), typeof(CheckBoxPosition), typeof(RadListView), new PropertyMetadata(CheckBoxPosition.BeforeItem));

        

        public Style ItemCheckBoxStyle
        {
            get { return (Style)GetValue(ItemCheckBoxStyleProperty); }
            set { SetValue(ItemCheckBoxStyleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemCheckBoxStyle.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemCheckBoxStyleProperty =
            DependencyProperty.Register(nameof(ItemCheckBoxStyle), typeof(Style), typeof(RadListView), new PropertyMetadata(null));
    
        internal readonly ListViewItemCheckBoxService itemCheckBoxService;

        /// <summary>
        /// Occurs when the <see cref="IsCheckModeActiveChanged"/> property has changed.
        /// </summary>
        public event EventHandler<CheckModeActiveChangedEventArgs> IsCheckModeActiveChanged
        {
            add
            {
                this.itemCheckBoxService.IsCheckModeActiveChanged += value;
            }
            remove
            {
                this.itemCheckBoxService.IsCheckModeActiveChanged -= value;
            }
        }

        public event EventHandler<ItemCheckedStateChangedEventArgs> ItemCheckedStateChanged
        {
            add
            {
                this.itemCheckBoxService.ItemCheckedStateChanged += value;
            }
            remove
            {
                this.itemCheckBoxService.ItemCheckedStateChanged -= value;
            }
        }

        private static void OnIsCheckModeActiveChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var listView = d as RadListView;

            if (listView.IsTemplateApplied)
            {
                listView.OnIsCheckModeActiveChanged((bool)e.NewValue);
            }
        }

        private void OnIsCheckModeActiveChanged(bool isActive, bool force = false)
        {
            var listView = this;

            if (listView != null && listView.LayoutDefinition.GetType().Equals(typeof(StackLayoutDefinition)) && (listView.SelectionMode == DataControlsSelectionMode.MultipleWithCheckBoxes || force))
            {
                listView.itemCheckBoxService.itemsAnimated = false;
                if (isActive)
                {
                    listView.itemCheckBoxService.OnIsCheckModeActiveChanged();

                }
                else
                {
                    listView.animationSurvice.PlayCheckModeAnimation(listView.childrenPanel, isActive, listView.ItemCheckBoxPosition == CheckBoxPosition.BeforeItem, listView.itemCheckBoxService.VisualLength);
                    listView.animationSurvice.PlayCheckBoxLayerAnimation(listView.checkBoxLayerCache.VisualElement, isActive, listView.ItemCheckBoxPosition == CheckBoxPosition.BeforeItem, listView.itemCheckBoxService.VisualLength);

                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether checkboxes are be shown
        /// next to the visual items allowing multiple selection of items.
        /// </summary>
        public bool IsCheckModeActive
        {
            get
            {
                 return (bool)this.GetValue(IsCheckModeActiveProperty);
            }
            set
            {
                this.SetValue(IsCheckModeActiveProperty, value);
            }
        }

        ListViewItemCheckBoxService IItemCheckBoxService.ItemCheckBoxService
        {
            get { return this.itemCheckBoxService; }
        }

        internal void OnCheckBoxChecked(object sender)
        {
            this.itemCheckBoxService.OnCheckBoxChecked(sender);
        }
    }
}
