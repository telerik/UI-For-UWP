using Telerik.Core.Data;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Represents a visual item that is used in the <see cref="RadDataBoundListBox"/> control.
    /// </summary>
    [TemplatePart(Name = "PART_CheckBox", Type = typeof(CheckBox))]
    [TemplatePart(Name = "PART_ContentContainer", Type = typeof(ContentControl))]
    public partial class RadDataBoundListBoxItem : RadVirtualizingDataControlItem
    {
        internal bool checkBoxVisible = false;
        internal bool isItemCheckable = false;
        internal ContentControl containerHolder;
        internal CheckBox checkBox;
        internal bool isCheckBoxEnabled;
        internal RadDataBoundListBox typedOwner;

        private bool isSelected;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadDataBoundListBoxItem"/> class.
        /// </summary>
        public RadDataBoundListBoxItem()
        {
            this.DefaultStyleKey = typeof(RadDataBoundListBoxItem);
            this.InitManipulation();
        }

        /// <summary>
        /// Gets a value indicating whether the item is currently in Selected visual state.
        /// </summary>
        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            internal set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    this.UpdateVisualState(this.IsLoaded);
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether the item
        /// will appear as checked when the list control shows
        /// check boxes next to each visual item.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                if (this.associatedDataItem != null)
                {
                    return this.associatedDataItem.IsChecked;
                }
                return false;
            }
        }

        internal override void PerformSpecialItemAction(object itemAction, object argument, ref object result)
        {
            base.PerformSpecialItemAction(itemAction, argument, ref result);

            if (itemAction.ToString() == "SynchCheckBoxState")
            {
                this.SynchItemCheckBoxState();
            }
            else if (itemAction.ToString() == "PositionCheckBox")
            {
                this.PositionCheckBoxForItem();
            }
            else if (itemAction.ToString() == "GetCheckIndicatorRectangle")
            {
                result = this.GetCheckBoxesIndicatorRectangleForItem(this.typedOwner.checkBoxesPressIndicator.Margin);
            }
            else if (itemAction.ToString() == "GetIsPointInCheckArea")
            {
                Point p = (Point)argument;
                result = this.IsPointInCheckModeAreaForItem(p);
            }
        }

        internal virtual void SynchItemCheckBoxState()
        {
            if (this.typedOwner == null || this.checkBox == null)
            {
                return;
            }

            bool visible = this.typedOwner.isCheckModeActive && this.isItemCheckable;

            this.checkBoxVisible = visible;

            if (this.checkBoxVisible)
            {
                if (this.typedOwner.checkBoxStyleCache != null && this.checkBox.Style != this.typedOwner.checkBoxStyleCache)
                {
                    this.checkBox.Style = this.typedOwner.checkBoxStyleCache;
                }
                else if (this.typedOwner.checkBoxStyleCache == null && this.checkBox.ReadLocalValue(CheckBox.StyleProperty) != DependencyProperty.UnsetValue)
                {
                    this.checkBox.ClearValue(CheckBox.StyleProperty);
                }
            }

            this.SynchCheckBoxEnabledState();

            if (this.containerHolder == null)
            {
                return;
            }

            this.PositionCheckBoxForItem();
        }

        internal bool IsPointInCheckModeAreaForItem(Point point)
        {
            if (this.typedOwner.virtualizationStrategy is WrapVirtualizationStrategy ||
                this.typedOwner.virtualizationStrategy is DynamicGridVirtualizationStrategy)
            {
                return this.IsPointInCheckAreaForWrapMode(point);
            }
            else if (this.typedOwner.virtualizationStrategy is StackVirtualizationStrategy)
            {
                return this.IsPointInCheckAreaForStackMode(point);
            }

            return false;
        }

        internal Rect GetCheckBoxesIndicatorRectangleForItem(Thickness indicatorMargin)
        {
            if (this.typedOwner.virtualizationStrategy is WrapVirtualizationStrategy ||
                this.typedOwner.virtualizationStrategy is DynamicGridVirtualizationStrategy)
            {
                return this.GetCheckBoxIndicatorRectangleForWrapMode(indicatorMargin);
            }
            else if (this.typedOwner.virtualizationStrategy is StackVirtualizationStrategy)
            {
                return this.GetCheckBoxIndicatorRectangleForStackMode(indicatorMargin);
            }
            return new Rect();
        }

        internal override void InvalidateCachedSize(Size newSize)
        {
            bool invalidate = false;
            if (this.typedOwner != null)
            {
                invalidate = this.typedOwner.virtualizationStrategy.orientationCache == Orientation.Vertical ? this.width != newSize.Width : this.height != newSize.Height;
            }

            base.InvalidateCachedSize(newSize);

            if (invalidate && this.typedOwner != null)
            {
                this.SynchItemCheckBoxState();
            }
        }

        internal override void BindToDataItem(IDataSourceItem item)
        {
            base.BindToDataItem(item);

            this.UpdateCheckedState();
        }

        internal void UpdateCheckedState()
        {
            if (this.checkBox != null && this.associatedDataItem != null)
            {
                this.checkBox.IsChecked = this.associatedDataItem.IsChecked;
            }
        }

        internal override void Attach(RadVirtualizingDataControl owner)
        {
            base.Attach(owner);
            this.typedOwner = owner as RadDataBoundListBox;
            this.isItemCheckable = this.typedOwner.IsItemCheckable(this);
            this.SynchItemCheckBoxState();
        }

        internal void SynchCheckBoxEnabledState()
        {
            this.isCheckBoxEnabled = this.typedOwner.listSourceFactory.sourceItemCheckedPathWritable ?? true;
            if (!this.isCheckBoxEnabled)
            {
                this.UpdateCheckBoxVisualState("Disabled");
            }
            else
            {
                this.UpdateCheckBoxVisualState("Normal");
            }
        }

        internal void UpdateCheckBoxVisualState(string newState)
        {
            if (!this.checkBoxVisible)
            {
                return;
            }

            if (this.isCheckBoxEnabled)
            {
                VisualStateManager.GoToState(this.checkBox, newState, false);
            }
            else
            {
                VisualStateManager.GoToState(this.checkBox, "Disabled", false);
            }
        }

        internal override void Detach()
        {
            base.Detach();

            if (this.typedOwner != null)
            {
                this.typedOwner.holdTimer.Tick -= this.HoldTimer_Tick;
            }

            this.typedOwner = null;
            this.isItemCheckable = false;
        }

        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate"/>. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.checkBox = this.GetTemplateChild("PART_CheckBox") as CheckBox;
            this.containerHolder = this.GetTemplateChild("PART_ContainerHolder") as ContentControl;

            if (this.checkBox != null)
            {
                this.UpdateCheckedState();
                this.UpdateCheckBoxVisualState(this.isCheckBoxEnabled ? "Normal" : "Disabled");

                if (this.ItemState == ItemState.Realized)
                {
                    this.SynchItemCheckBoxState();
                }
            }

            this.UpdateVisualState(false);
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            return this.IsSelected ? "Selected" : "Unselected";
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadDataBoundListBoxItemAutomationPeer(this);
        }

        partial void InitManipulation();

        private void PositionCheckBoxForItem()
        {
            if (this.typedOwner.virtualizationStrategy is WrapVirtualizationStrategy ||
                this.typedOwner.virtualizationStrategy is DynamicGridVirtualizationStrategy)
            {
                this.PositionCheckBoxForWrapLayout();
            }
            else
            {
                this.PositionCheckBoxForStackLayout();
            }
        }

        private void PositionCheckBoxForWrapLayout()
        {
            if (!this.checkBoxVisible)
            {
                if (this.typedOwner.virtualizationStrategy.orientationCache == Orientation.Vertical)
                {
                    VisualStateManager.GoToState(this, "CheckBoxNotVisibleVertical", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "CheckBoxNotVisibleHorizontal", false);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, "CheckBoxVisibleOverlay", false);
            }
        }

        private void PositionCheckBoxForStackLayout()
        {
            bool containerHolderVisible = this.isItemCheckable && this.typedOwner.isCheckModeEnabled && !this.typedOwner.isCheckModeActive;
            Orientation orientationCache = this.typedOwner.virtualizationStrategy.orientationCache;
            if (this.checkBoxVisible)
            {
                if (orientationCache == Orientation.Vertical)
                {
                    VisualStateManager.GoToState(this, "CheckBoxVisibleVertical", false);
                }
                else
                {
                    VisualStateManager.GoToState(this, "CheckBoxVisibleHorizontal", false);
                }
            }
            else
            {
                if (!containerHolderVisible)
                {
                    if (orientationCache == Orientation.Vertical)
                    {
                        VisualStateManager.GoToState(this, "CheckBoxNotVisibleVertical", false);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "CheckBoxNotVisibleHorizontal", false);
                    }
                }
                else
                {
                    if (orientationCache == Orientation.Vertical)
                    {
                        VisualStateManager.GoToState(this, "CheckBoxNotVisibleVerticalSpace", false);
                    }
                    else
                    {
                        VisualStateManager.GoToState(this, "CheckBoxNotVisibleHorizontalSpace", false);
                    }
                }
            }
        }

        private bool IsPointInCheckAreaForStackMode(Point point)
        {
            double tapOffsetFromItemStart = this.typedOwner.virtualizationStrategy.orientationCache == Orientation.Vertical ? point.X : point.Y;
            bool isCheckModeActivationArea = tapOffsetFromItemStart <= RadDataBoundListBox.ShowCheckBoxesThreshold;

            if (!this.checkBoxVisible && this.typedOwner.isCheckModeEnabled)
            {
                return isCheckModeActivationArea;
            }

            if (!this.typedOwner.isCheckModeActive && !this.typedOwner.isCheckModeEnabled)
            {
                return false;
            }

            Point checkBoxLocationInItem = this.checkBox.TransformToVisual(this).TransformPoint(new Point(0, 0));
            Size checkBoxSize = this.checkBox.DesiredSize;
            Rect checkArea = new Rect(new Point(0, 0), new Size(checkBoxLocationInItem.X + checkBoxSize.Width, checkBoxLocationInItem.Y + checkBoxSize.Height));

            return checkArea.Contains(point) || isCheckModeActivationArea;
        }

        private bool IsPointInCheckAreaForWrapMode(Point point)
        {
            if (!this.checkBoxVisible)
            {
                return false;
            }

            Point checkBoxLocation = this.typedOwner.TransformToVisual(this).TransformPoint(point);
            Rect checkArea = new Rect(new Point(0, 0), new Size(this.width - this.containerHolder.Width, this.height));

            return checkArea.Contains(checkBoxLocation);
        }

        private Rect GetCheckBoxIndicatorRectangleForWrapMode(Thickness indicatorMargin)
        {
            return new Rect();
        }

        private Rect GetCheckBoxIndicatorRectangleForStackMode(Thickness indicatorMargin)
        {
            Orientation orientationCache = this.typedOwner.virtualizationStrategy.orientationCache;
            double lengthMargin = orientationCache == Orientation.Vertical ? indicatorMargin.Top + indicatorMargin.Bottom : indicatorMargin.Left + indicatorMargin.Right;
            double itemLength = this.typedOwner.virtualizationStrategy.GetItemLength(this) - lengthMargin;

            double xCoord = 0;
            double yCoord = 0;
            double width = 0;
            double height = 0;

            if (orientationCache == Orientation.Vertical)
            {
                yCoord = this.CurrentOffset;
                width = RadDataBoundListBox.CheckBoxesIndicatorLength;
                height = itemLength;
            }
            else
            {
                width = itemLength;
                height = RadDataBoundListBox.CheckBoxesIndicatorLength;
                yCoord = indicatorMargin.Top;
                xCoord = this.CurrentOffset + indicatorMargin.Left;
            }

            return new Rect(xCoord, yCoord, width, height);
        }
    }
}
