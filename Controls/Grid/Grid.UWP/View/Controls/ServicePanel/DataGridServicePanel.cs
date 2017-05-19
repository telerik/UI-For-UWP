using System;
using System.Collections.Specialized;
using System.Diagnostics;
using Telerik.Data.Core;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Primitives.DragDrop;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the service UI that is located on the left side of a <see cref="RadDataGrid"/> component and is used to control the grouping state of the component through the user interface.
    /// </summary>
    [TemplatePart(Name = "PART_GroupFlyout", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_GroupFlyoutContent", Type = typeof(DataGridServicePanelGroupingFlyout))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "NormalGrouped", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Expanded", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "DragHinting", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "DropPossible", GroupName = "CommonStates")]
    public partial class DataGridServicePanel : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Position"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.Register(nameof(Position), typeof(GroupPanelPosition), typeof(DataGridServicePanel), new PropertyMetadata(GroupPanelPosition.Left, OnPositionChanged));

        private TextBlock verticalText;
        private Popup groupFlyout;
        private DataGridServicePanelGroupingFlyout groupFlyoutContent;
        private bool isGroupFlyoutOpen;
        private bool isColumnDragging;
        private RadDataGrid owner;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridServicePanel" /> class.
        /// </summary>
        public DataGridServicePanel()
        {
            this.DefaultStyleKey = typeof(DataGridServicePanel);
            DragDrop.SetAllowDrop(this, true);
        }

        /// <summary>
        /// Gets or sets the position of the Group's panel.
        /// </summary>
        public GroupPanelPosition Position
        {
            get { return (GroupPanelPosition)GetValue(PositionProperty); }
            set { this.SetValue(PositionProperty, value); }
        }
        
        /// <summary>
        /// Gets the Popup instance used to display the grouping flyout content. Exposed for testing purposes, do not use elsewhere but in test projects.
        /// </summary>
        internal Popup GroupFlyout
        {
            get
            {
                return this.groupFlyout;
            }
        }

        /// <summary>
        /// Gets the DataGridServicePanelGroupingFlyout instance displayed within the GroupFlyout. Exposed for testing purposes, do not use elsewhere but in test projects.
        /// </summary>
        internal DataGridServicePanelGroupingFlyout GroupFlyoutContent
        {
            get
            {
                return this.groupFlyoutContent;
            }
        }

        /// <summary>
        /// Gets the TextBlock instance that displays the "Drag here to group hint". Exposed for testing purposes, do not use elsewhere but in test projects.
        /// </summary>
        internal TextBlock VerticalTextBlock
        {
            get
            {
                return this.verticalText;
            }
        }

        internal bool IsColumnDragging
        {
            get
            {
                return this.isColumnDragging;
            }
            set
            {
                this.isColumnDragging = value;
                this.UpdateVisualState(this.IsTemplateApplied);
            }
        }

        internal RadDataGrid Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                if (this.owner != null)
                {
                    this.owner.GroupDescriptors.CollectionChanged -= this.OnGroupDescriptorsCollectionChanged;
                }

                this.owner = value;

                if (this.owner != null)
                {
                    this.owner.GroupDescriptors.CollectionChanged += this.OnGroupDescriptorsCollectionChanged;
                }
            }
        }
        
        internal void OnPositionChanged(GroupPanelPosition position)
        {
            switch (position)
            {
                case GroupPanelPosition.Left:
                    this.HorizontalAlignment = HorizontalAlignment.Left;
                    this.VerticalAlignment = VerticalAlignment.Stretch;
                    this.Width = 40;
                    this.Height = double.NaN;
                    break;
                case GroupPanelPosition.Bottom:
                    this.HorizontalAlignment = HorizontalAlignment.Stretch;
                    this.VerticalAlignment = VerticalAlignment.Top;
                    this.Height = 40;
                    this.Width = double.NaN;
                    break;
                default:
                    break;
            }
        }

        internal void HandleGroupFlyoutHeaderClosed(object sender, EventArgs e)
        {
            var descriptor = (sender as FrameworkElement).DataContext as GroupDescriptorBase;
            if (descriptor == null)
            {
                Debug.Assert(false, "Invalid HandleGroupFlyoutHeaderClosed event sender");
                return;
            }

            if (descriptor is CollectionViewGroupDescriptor)
            {
                Debug.Assert(false, "CollectionViewGroupDescriptor cannot be removed by the user.");
                return;
            }

            int previousCount = this.Owner.GroupDescriptors.Count;

            // raise the FlyoutGroupHeaderTap command and see whether the descriptor count has changed
            var context = new FlyoutGroupHeaderTapContext()
            {
                Action = DataGridFlyoutGroupHeaderTapAction.RemoveDescriptor,
                Descriptor = descriptor
            };

            this.Owner.commandService.ExecuteCommand(CommandId.FlyoutGroupHeaderTap, context);

            if (previousCount == this.Owner.GroupDescriptors.Count)
            {
                // the user has prevented the removal of the associated descriptor, so do nothing
                return;
            }

            if (this.Owner.GroupDescriptors.Count > 0)
            {
                this.groupFlyoutContent.ClearUI();
                this.groupFlyoutContent.PrepareUI();
            }
            else
            {
                this.groupFlyout.IsOpen = false;
            }
        }

        internal void HandleGroupFlyoutDescriptorContentTap(object sender, EventArgs e)
        {
            var descriptor = (sender as FrameworkElement).DataContext as GroupDescriptorBase;
            if (descriptor == null)
            {
                Debug.Assert(false, "Invalid HandleGroupFlyoutHeaderClosed event sender");
                return;
            }

            // raise the FlyoutGroupHeaderTap command
            var context = new FlyoutGroupHeaderTapContext()
            {
                Action = DataGridFlyoutGroupHeaderTapAction.ChangeSortOrder,
                Descriptor = descriptor
            };

            this.Owner.commandService.ExecuteCommand(CommandId.FlyoutGroupHeaderTap, context);
        }

        internal void OpenGroupingFlyout()
        {
            this.groupFlyoutContent.PrepareUI();
            this.PositionGroupFlyout();

            this.groupFlyout.IsOpen = true;

            // Hide any other flyout in RadDataGrid.
            // TODO: GroupFlyout should also use the shared ContentFlyout in RadDataGrid.
            this.owner.ContentFlyout.Hide(DataGridFlyoutId.All);
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.verticalText = this.GetTemplateChild("DragHereText") as TextBlock;
            if (this.verticalText != null)
            {
                this.verticalText.Text = GridLocalizationManager.Instance.GetString("DragToGroup");
            }

            this.groupFlyout = this.GetTemplatePartField<Popup>("PART_GroupFlyout");
            applied = applied && this.groupFlyout != null;

            this.groupFlyoutContent = this.GetTemplatePartField<DataGridServicePanelGroupingFlyout>("PART_GroupFlyoutContent");
            applied = applied && this.groupFlyoutContent != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.groupFlyout.Opened += this.OnGroupFlyoutOpened;
            this.groupFlyout.Closed += this.OnGroupFlyoutClosed;
            this.groupFlyoutContent.SizeChanged += this.OnGroupFlyoutSizeChanged;

            this.groupFlyoutContent.Owner = this;

            this.OnPositionChanged(this.Position);
            this.UpdatePositionVisualState(this.Position);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new DataGridServicePanelAutomationPeer(this);
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.groupFlyout.Opened -= this.OnGroupFlyoutOpened;
            this.groupFlyout.Closed -= this.OnGroupFlyoutClosed;
            this.groupFlyout.SizeChanged -= this.OnGroupFlyoutSizeChanged;

            this.groupFlyoutContent.Owner = null;
        }

        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.Owner == null)
            {
                return base.ComposeVisualStateName();
            }

            if (this.DropPossible)
            {
                return "DropPossible";
            }

            if (this.isColumnDragging)
            {
                return "DragHinting";
            }

            if (this.isGroupFlyoutOpen)
            {
                return "Expanded";
            }

            if (this.Owner.GroupDescriptors.Count > 0)
            {
                return "NormalGrouped";
            }

            return "Normal";
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (e == null || this.Owner == null || this.Owner.GroupDescriptors.Count == 0)
            {
                return;
            }

            this.OpenGroupingFlyout();
        }

        private static void OnPositionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as DataGridServicePanel;
            var position = (GroupPanelPosition)e.NewValue;

            if (panel != null)
            {
                if (panel.IsTemplateApplied)
                {
                    panel.OnPositionChanged(position);
                    panel.UpdatePositionVisualState(position);
                    panel.PositionGroupFlyout();
                }
            }
        }

        private void PositionGroupFlyout()
        {
            if (this.Position == GroupPanelPosition.Left)
            {
                this.groupFlyoutContent.Height = this.ActualHeight;
                this.groupFlyoutContent.Width = double.NaN;
              
                this.groupFlyout.HorizontalOffset = this.ActualWidth;
                this.groupFlyout.VerticalOffset = 0;
            }
            else
            {
                this.groupFlyoutContent.Width = this.ActualWidth;
                this.groupFlyoutContent.Height = double.NaN;

                this.groupFlyout.VerticalOffset = -this.groupFlyoutContent.ActualHeight;
                this.groupFlyout.HorizontalOffset = 0;
            }
         }

        private void UpdatePositionVisualState(GroupPanelPosition position)
        {
            var stateName = position.ToString();
            this.SetVisualState(stateName, false);
        }

        private void OnGroupFlyoutSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.PositionGroupFlyout();
        }
        
        private void OnGroupFlyoutOpened(object sender, object e)
        {
            this.isGroupFlyoutOpen = true;
            this.UpdateVisualState(this.IsTemplateApplied);

            var dataGridServicePanelPeer = FrameworkElementAutomationPeer.FromElement(this) as DataGridServicePanelAutomationPeer;
            if (dataGridServicePanelPeer != null)
            {
                dataGridServicePanelPeer.RaiseExpandCollapseAutomationEvent(false, true);
            }
        }

        private void OnGroupFlyoutClosed(object sender, object e)
        {
            this.isGroupFlyoutOpen = false;
            this.groupFlyoutContent.ClearUI();
            this.UpdateVisualState(this.IsTemplateApplied);

            var dataGridServicePanelPeer = FrameworkElementAutomationPeer.FromElement(this) as DataGridServicePanelAutomationPeer;
            if (dataGridServicePanelPeer != null)
            {
                dataGridServicePanelPeer.RaiseExpandCollapseAutomationEvent(true, false);
            }
        }

        private void OnGroupDescriptorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.UpdateVisualState(this.IsTemplateApplied);
        }
    }
}
