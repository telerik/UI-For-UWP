using System;
using System.Diagnostics;
using System.Linq;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Input;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the control which holds the column headers in the column chooser.
    /// </summary>
    [TemplatePart(Name = "PART_HeadersContainer", Type = typeof(DataGridFlyoutPanel))]
    [TemplatePart(Name = "PART_AdornerHost", Type = typeof(Canvas))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Image))]
    
    public partial class DataGridColumnsFlyout : DataGridFlyout 
    {
        /// <summary>
        /// Identifies the <see cref="HeaderStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderStyleProperty =
            DependencyProperty.Register(nameof(HeaderStyle), typeof(Style), typeof(DataGridColumnsFlyout), new PropertyMetadata(null));

        private Image closeButton;
        private bool isCloseButtonPointerOver;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridColumnsFlyout" /> class.
        /// </summary>
        public DataGridColumnsFlyout()
        {
            this.DefaultStyleKey = typeof(DataGridColumnsFlyout);
        }

        /// <summary>
        /// Gets or sets the style of the flyout header./>.
        /// </summary>
        public Style HeaderStyle
        {
            get
            {
                return (Style)this.GetValue(HeaderStyleProperty);
            }
            set
            {
                this.SetValue(HeaderStyleProperty, value);
            }
        }

        internal DataGridColumnReorderServicePanel Owner
        {
            get;
            set;
        }

        internal override void ClearUI()
        {
            if (this.Container != null)
            {
                foreach (DataGridFlyoutColumnHeader header in this.Container.Children)
                {
                    header.SelectionCheck -= this.Header_SelectionCheck;
                    header.SelectionUncheck -= this.Header_SelectionUncheck;
                    header.DragSurfaceRequested -= this.HandleDragSurfaceRequested;
                }

                this.Container.ClearItems();
            }
        }

        internal override void PrepareUI()
        {
            if (this.Owner == null || !this.IsTemplateApplied)
            {
                return;
            }

            this.Container.Owner = this.Owner.Owner;

            for (int i = 0; i < this.Owner.Owner.Columns.Count; i++)
            {
                DataGridColumn column = this.Owner.Owner.Columns[i];

                DataGridFlyoutColumnHeader header = new DataGridFlyoutColumnHeader();

                this.SetupDragDropProperties(header, i);

                header.DataContext = column;
                header.SelectionCheck += this.Header_SelectionCheck;
                header.SelectionUncheck += this.Header_SelectionUncheck;

                header.DragSurfaceRequested += this.HandleDragSurfaceRequested;
                header.ParentGrid = this.Owner.Owner;

                this.Container.Elements.Add(header);
            }
        }

        /// <inheritdoc/>
        protected override string ComposeVisualStateName()
        {
            string state = string.Empty;
            if (this.isCloseButtonPointerOver)
            {
                state = "NormalPointerOver";
            }
            else
            {
                state = "Normal";
            }
            if (this.Owner.Owner.ActualHeight > this.Owner.Owner.ActualWidth)
            {
                state = state + RadControl.VisualStateDelimiter + "BottomBorder";
            }
            else
            {
                state = state + RadControl.VisualStateDelimiter + "LeftBorder";
            }

            return state;
        }
        
        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate"/> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied"/> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.closeButton = this.GetTemplatePartField<Image>("PART_CloseButton");
            applied = applied && this.closeButton != null;

            this.closeButton.PointerEntered += this.CloseButton_PointerEntered;
            this.closeButton.PointerExited += this.CloseButton_PointerExited;
            this.closeButton.Tapped += this.CloseButton_Tapped;

            return applied;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            this.closeButton.PointerEntered -= this.CloseButton_PointerEntered;
            this.closeButton.PointerExited -= this.CloseButton_PointerExited;
            this.closeButton.Tapped -= this.CloseButton_Tapped;
            base.UnapplyTemplateCore();
        }

        private void CloseButton_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            this.Owner.CloseColumnsFlyout();
        }

        private void Header_SelectionUncheck(object sender, EventArgs e)
        {
            DataGridColumn column = (sender as DataGridFlyoutColumnHeader).DataContext as DataGridColumn;

            Debug.Assert(column != null, "A column should be present as DataContext");
            if (column == null)
            {
                return;
            }

            this.Owner.Owner.CommandService.ExecuteCommand(CommandId.ToggleColumnVisibility, new ToggleColumnVisibilityContext() { Column = column, IsColumnVisible = false });
        }

        private void Header_SelectionCheck(object sender, EventArgs e)
        {
            DataGridColumn column = (sender as DataGridFlyoutColumnHeader).DataContext as DataGridColumn;

            Debug.Assert(column != null, "A column should be present as DataContext");
            if (column == null)
            {
                return;
            }

            this.Owner.Owner.CommandService.ExecuteCommand(CommandId.ToggleColumnVisibility, new ToggleColumnVisibilityContext() { Column = column, IsColumnVisible = true });
        }

        private void CloseButton_PointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.OnPointerEntered(e);
            this.isCloseButtonPointerOver = false;
            this.UpdateVisualState(false);
        }

        private void CloseButton_PointerEntered(object sender, PointerRoutedEventArgs e)
        {
            this.OnPointerEntered(e);
            this.isCloseButtonPointerOver = true;
            this.UpdateVisualState(false);
        }
    }
}
