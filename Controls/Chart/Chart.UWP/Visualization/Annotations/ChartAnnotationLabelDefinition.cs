using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Describes the appearance of the labels within a <see cref="ChartAnnotation"/>.
    /// </summary>
    [Bindable]
    public class ChartAnnotationLabelDefinition : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="Format"/> property.
        /// </summary>
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(string), typeof(ChartAnnotationLabelDefinition), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Template"/> property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(nameof(Template), typeof(DataTemplate), typeof(ChartAnnotationLabelDefinition), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DefaultVisualStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty DefaultVisualStyleProperty =
            DependencyProperty.Register(nameof(DefaultVisualStyle), typeof(Style), typeof(ChartAnnotationLabelDefinition), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Location"/> property.
        /// </summary>
        public static readonly DependencyProperty LocationProperty =
            DependencyProperty.Register(nameof(Location), typeof(ChartAnnotationLabelLocation), typeof(ChartAnnotationLabelDefinition), new PropertyMetadata(ChartAnnotationLabelLocation.Left));

        /// <summary>
        /// Identifies the <see cref="HorizontalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(HorizontalAlignment), typeof(HorizontalAlignment), typeof(ChartAnnotationLabelDefinition), new PropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Identifies the <see cref="VerticalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(nameof(VerticalAlignment), typeof(VerticalAlignment), typeof(ChartAnnotationLabelDefinition), new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Identifies the <see cref="HorizontalOffset"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalOffsetProperty =
            DependencyProperty.Register(nameof(HorizontalOffset), typeof(double), typeof(ChartAnnotationLabelDefinition), new PropertyMetadata(0d));

        /// <summary>
        /// Identifies the <see cref="VerticalOffset"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalOffsetProperty =
            DependencyProperty.Register(nameof(VerticalOffset), typeof(double), typeof(ChartAnnotationLabelDefinition), new PropertyMetadata(0d));

        /// <summary>
        /// Gets or sets the string used to format the label content, using the <see cref="M:String.Format"/> method.
        /// </summary>
        public string Format
        {
            get
            {
                return (string)this.GetValue(FormatProperty);
            }
            set 
            { 
                this.SetValue(FormatProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplate"/> instance that may be used to define custom-looking label.
        /// </summary>
        public DataTemplate Template
        {
            get 
            { 
                return (DataTemplate)this.GetValue(TemplateProperty); 
            }
            set 
            { 
                this.SetValue(TemplateProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that defines the appearance of the default label visual - <see cref="TextBlock"/> instance.
        /// </summary>
        public Style DefaultVisualStyle
        {
            get
            { 
                return (Style)this.GetValue(DefaultVisualStyleProperty); 
            }
            set 
            { 
                this.SetValue(DefaultVisualStyleProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartAnnotationLabelLocation"/> value that defines the primary location of the annotation label visual.
        /// </summary>
        /// <remarks>Annotations provide a mechanism for label positioning that consists of three layers -- primary (Location), secondary (Horizontal/VerticalAlignment),
        /// and tertiary (Horizontal/VerticalOffset).</remarks>
        public ChartAnnotationLabelLocation Location
        {
            get 
            { 
                return (ChartAnnotationLabelLocation)this.GetValue(LocationProperty); 
            }
            set 
            {
                this.SetValue(LocationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment value that can be used as a secondary mechanism in conjunction with the primary Location property value.
        /// </summary>
        /// <remarks>
        /// Annotations provide a mechanism for label positioning that consists of three layers -- primary (Location), secondary (Horizontal/VerticalAlignment),
        /// and tertiary (Horizontal/VerticalOffset).
        /// </remarks>
        /// <value>HorizontalAlignment property value is not applicable if Location property is set to either ChartAnnotationLabelLocation.Left, 
        /// or ChartAnnotationLabelLocation.Right.</value>
        public HorizontalAlignment HorizontalAlignment
        {
            get 
            { 
                return (HorizontalAlignment)this.GetValue(HorizontalAlignmentProperty);
            }
            set
            { 
                this.SetValue(HorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment value that can be used in conjunction with the primary Location property value.
        /// </summary>
        /// <remarks>
        /// Annotations provide a mechanism for label positioning that consists of three layers -- primary (Location), secondary (Horizontal/VerticalAlignment),
        /// and tertiary (Horizontal/VerticalOffset).
        /// </remarks>
        /// <value>
        /// VerticalAlignment property value is not applicable if Location property is set to either ChartAnnotationLabelLocation.Top, or ChartAnnotationLabelLocation.Bottom.
        /// </value>
        public VerticalAlignment VerticalAlignment
        {
            get 
            { 
                return (VerticalAlignment)this.GetValue(VerticalAlignmentProperty); 
            }
            set 
            { 
                this.SetValue(VerticalAlignmentProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets the horizontal offset in pixels that can be specified besides the primary Location and the secondary HorizontalAlignment properties.
        /// </summary>
        /// <remarks>
        /// Annotations provide a mechanism for label positioning that consists of three layers -- primary (Location), secondary (Horizontal/VerticalAlignment),
        /// and tertiary (Horizontal/VerticalOffset).
        /// </remarks>
        public double HorizontalOffset
        {
            get 
            { 
                return (double)this.GetValue(HorizontalOffsetProperty); 
            }
            set 
            { 
                this.SetValue(HorizontalOffsetProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical offset in pixels that can be specified besides the primary Location and the secondary VerticalAlignment properties.
        /// </summary>
        /// <remarks>
        /// Annotations provide a mechanism for label positioning that consists of three layers -- primary (Location), secondary (Horizontal/VerticalAlignment),
        /// and tertiary (Horizontal/VerticalOffset).
        /// </remarks>
        public double VerticalOffset
        {
            get 
            { 
                return (double)this.GetValue(VerticalOffsetProperty); 
            }
            set 
            { 
                this.SetValue(VerticalOffsetProperty, value);
            }
        }
    }
}
