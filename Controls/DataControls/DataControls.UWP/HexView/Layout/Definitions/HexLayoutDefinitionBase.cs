using System.ComponentModel;
using Telerik.UI.Xaml.Controls.Data.ContainerGeneration;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.HexView
{
    /// <summary>
    /// A base class for all layout definitions of the <see cref="RadHexView"/> control.
    /// </summary>
    public abstract class HexLayoutDefinitionBase : DependencyObject, INotifyPropertyChanged
    {
        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
             DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(HexLayoutDefinitionBase), new PropertyMetadata(Orientation.Vertical, OnOrientationChanged));

        /// <summary>
        /// Identifies the <see cref="ItemLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemLengthProperty =
            DependencyProperty.Register(nameof(ItemLength), typeof(double), typeof(HexLayoutDefinitionBase), new PropertyMetadata(171d, OnItemLengthChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsSpacing"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSpacingProperty =
            DependencyProperty.Register(nameof(ItemsSpacing), typeof(double), typeof(HexLayoutDefinitionBase), new PropertyMetadata(5d, OnItemsSpacingChanged));

        /// <summary>
        /// Identifies the <see cref="ViewPortExtension"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ViewPortExtensionProperty =
            DependencyProperty.Register(nameof(ViewPortExtension), typeof(double), typeof(HexLayoutDefinitionBase), new PropertyMetadata(0.5d, OnViewPortExtensionChanged));

        /// <summary>
        /// Occurs when a property is changed.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the orientation of the layout.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }
            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Telerik.UI.Xaml.Controls.Primitives.RadHexHubTile.Length"/> property of the items.
        /// </summary>
        public double ItemLength
        {
            get
            {
                return (double)this.GetValue(ItemLengthProperty);
            }
            set
            {
                this.SetValue(ItemLengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the spacing between the items.
        /// </summary>
        public double ItemsSpacing
        {
            get
            {
                return (double)this.GetValue(ItemsSpacingProperty);
            }
            set
            {
                this.SetValue(ItemsSpacingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the amount of space added to the top and the bottom of the viewport,
        /// to be filled with items. The value is in relative units. For example: if you set it to 0.5, then
        /// half viewport will be added to the top and to the bottom of the visible area.
        /// </summary>
        public double ViewPortExtension
        {
            get
            {
                return (double)this.GetValue(ViewPortExtensionProperty);
            }
            set
            {
                this.SetValue(ViewPortExtensionProperty, value);
            }
        }

        internal abstract HexLayoutStrategyBase CreateStrategy(HexItemModelGenerator generator, RadHexView view);

        private static void OnItemLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as HexLayoutDefinitionBase;
            definition.OnPropertyChanged(nameof(HexLayoutDefinitionBase.ItemLength));
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as HexLayoutDefinitionBase;
            definition.OnPropertyChanged(nameof(HexLayoutDefinitionBase.Orientation));
        }

        private static void OnItemsSpacingChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as HexLayoutDefinitionBase;
            definition.OnPropertyChanged(nameof(HexLayoutDefinitionBase.ItemsSpacing));
        }

        private static void OnViewPortExtensionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var definition = d as HexLayoutDefinitionBase;
            definition.OnPropertyChanged(nameof(HexLayoutDefinitionBase.ViewPortExtension));
        }

        private void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
