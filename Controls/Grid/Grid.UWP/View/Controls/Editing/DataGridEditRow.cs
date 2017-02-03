using Telerik.Core;
using Telerik.UI.Xaml.Controls.Grid.Commands;
using Telerik.UI.Xaml.Controls.Primitives.Common;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Grid.Primitives
{
    /// <summary>
    /// Represents the User Interface that is displayed whenever a <see cref="RadDataGrid"/> row enters edit mode.
    /// </summary>
    public class DataGridEditRow : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="CancelButtonStyle"/> dependency property. 
        /// </summary>    
        public static readonly DependencyProperty CancelButtonStyleProperty =
            DependencyProperty.Register(nameof(CancelButtonStyle), typeof(Style), typeof(DataGridEditRow), new PropertyMetadata(null, OnCloseButtonStyleChanged));

        private Button cancelButton;

        /// <summary>
        /// Initializes a new instance of the <see cref="DataGridEditRow" /> class.
        /// </summary>
        public DataGridEditRow()
        {
            this.DefaultStyleKey = typeof(DataGridEditRow);
        }

        /// <summary>
        /// Gets or sets the cancel button style.
        /// </summary>
        /// <value>The cancel button style.</value>
        public Style CancelButtonStyle
        {
            get
            {
                return (Style)this.GetValue(CancelButtonStyleProperty);
            }
            set
            {
                this.SetValue(CancelButtonStyleProperty, value);
            }
        }

        internal Button CancelButton
        {
            get
            {
                if (this.cancelButton == null)
                {
                    this.cancelButton = new InlineButton();
                    this.cancelButton.Click += this.OnCancelButtonClick;
                }

                return this.cancelButton;
            }
        }

        internal RadDataGrid Owner { get; set; }

        internal void UpdateCloseButtonVisibility(bool isVisible)
        {
            this.cancelButton.Visibility = isVisible ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void PositionCloseButton(RadRect layoutSlot, bool displayOnTop, bool isFrozen)
        {
            if (this.CancelButton != null && this.Owner.editService.IsEditing)
            {
                var buttonOffset = displayOnTop ? -this.CancelButton.ActualHeight : this.ActualHeight;

                var buttonSlot = new RadRect(layoutSlot.X, layoutSlot.Y + buttonOffset, this.CancelButton.ActualWidth, this.CancelButton.ActualHeight);

                this.CancelButton.Arrange(buttonSlot.ToRect());

                if (!isFrozen)
                {
                    if (this.Owner.EditRowLayer.EditorLayoutSlots.ContainsKey(this))
                    {
                        this.Owner.EditRowLayer.EditorLayoutSlots[this] = buttonSlot;
                    }
                    else
                    {
                        this.Owner.EditRowLayer.EditorLayoutSlots.Add(this, buttonSlot);
                    }
                }
                else
                {
                    if (this.Owner.FrozenEditRowLayer.EditorLayoutSlots.ContainsKey(this))
                    {
                        this.Owner.FrozenEditRowLayer.EditorLayoutSlots[this] = buttonSlot;
                    }
                    else
                    {
                        this.Owner.FrozenEditRowLayer.EditorLayoutSlots.Add(this, buttonSlot);
                    }
                }
            }
        }

        private static void OnCloseButtonStyleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var row = d as DataGridEditRow;

            row.CancelButton.Style = e.NewValue as Style;
        }

        private void OnCancelButtonClick(object sender, RoutedEventArgs e)
        {
            if (this.Owner != null)
            {
                this.Owner.CancelEdit(ActionTrigger.Programmatic, null);
            }
        }
    }
}