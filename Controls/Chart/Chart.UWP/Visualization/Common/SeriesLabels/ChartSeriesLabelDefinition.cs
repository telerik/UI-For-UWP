using System;
using Telerik.Charting;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Describes the appearance of the labels within a <see cref="ChartSeries"/>.
    /// A chart series can have multiple definitions, allowing for multiple labels per data point.
    /// </summary>
    public class ChartSeriesLabelDefinition : DependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="Binding"/> property.
        /// </summary>
        public static readonly DependencyProperty BindingProperty =
            DependencyProperty.Register(nameof(Binding), typeof(DataPointBinding), typeof(ChartSeriesLabelDefinition), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Format"/> property.
        /// </summary>
        public static readonly DependencyProperty FormatProperty =
            DependencyProperty.Register(nameof(Format), typeof(string), typeof(ChartSeriesLabelDefinition), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Margin"/> property.
        /// </summary>
        public static readonly DependencyProperty MarginProperty =
            DependencyProperty.Register(nameof(Margin), typeof(Thickness), typeof(ChartSeriesLabelDefinition), new PropertyMetadata(new Thickness(0)));

        /// <summary>
        /// Identifies the <see cref="HorizontalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty HorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(HorizontalAlignment), typeof(HorizontalAlignment), typeof(ChartSeriesLabelDefinition), new PropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Identifies the <see cref="VerticalAlignment"/> property.
        /// </summary>
        public static readonly DependencyProperty VerticalAlignmentProperty =
            DependencyProperty.Register(nameof(VerticalAlignment), typeof(VerticalAlignment), typeof(ChartSeriesLabelDefinition), new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Identifies the <see cref="Template"/> property.
        /// </summary>
        public static readonly DependencyProperty TemplateProperty =
            DependencyProperty.Register(nameof(Template), typeof(DataTemplate), typeof(ChartSeriesLabelDefinition), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TemplateSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty TemplateSelectorProperty =
            DependencyProperty.Register(nameof(TemplateSelector), typeof(DataTemplateSelector), typeof(ChartSeriesLabelDefinition), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DefaultVisualStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty DefaultVisualStyleProperty =
            DependencyProperty.Register(nameof(DefaultVisualStyle), typeof(Style), typeof(ChartSeriesLabelDefinition), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DefaultVisualStyle"/> property.
        /// </summary>
        public static readonly DependencyProperty StrategyProperty =
            DependencyProperty.Register(nameof(Strategy), typeof(ChartSeriesLabelStrategy), typeof(ChartSeriesLabelDefinition), new PropertyMetadata(null));

        /// <summary>
        /// Gets or sets the <see cref="DataPointBinding"/> instance that will retrieve the content of each label.
        /// Valid when the owning <see cref="ChartSeries"/> is data-bound.
        /// </summary>
        public DataPointBinding Binding
        {
            get
            {
                return this.GetValue(BindingProperty) as DataPointBinding;
            }
            set
            {
                this.SetValue(BindingProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the string used to format the label content, using the <see cref="M:String.Format"/> method.
        /// </summary>
        public string Format
        {
            get
            {
                return this.GetValue(FormatProperty) as string;
            }
            set
            {
                this.SetValue(FormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Windows.UI.Xaml.Thickness(double)"/> that defines the offset of each label from the four box edges.
        /// </summary>
        public Thickness Margin
        {
            get
            {
                return (Thickness)this.GetValue(MarginProperty);
            }
            set
            {
                this.SetValue(MarginProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the alignment along the X-axis of each label relative to the <see cref="DataPoint"/> it is associated to.
        /// </summary>
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
        /// Gets or sets the alignment along the Y-axis of each label relative to the <see cref="DataPoint"/> it is associated to.
        /// </summary>
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
        /// Gets or sets the <see cref="DataTemplate"/> instance that may be used to define custom-looking labels.
        /// </summary>
        public DataTemplate Template
        {
            get
            {
                return this.GetValue(TemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(TemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="DataTemplateSelector"/> instance that may be used to provide context-specific data templates, depending on the provided <see cref="DataPoint"/>.
        /// </summary>
        public DataTemplateSelector TemplateSelector
        {
            get
            {
                return this.GetValue(TemplateSelectorProperty) as DataTemplateSelector;
            }
            set
            {
                this.SetValue(TemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> instance that defines the appearance of the default label visuals - <see cref="TextBlock"/> instances.
        /// </summary>
        public Style DefaultVisualStyle
        {
            get
            {
                return this.GetValue(DefaultVisualStyleProperty) as Style;
            }
            set
            {
                this.SetValue(DefaultVisualStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a custom <see cref="ChartSeriesLabelStrategy"/> instance that may be used to completely override labels appearance, content and layout.
        /// </summary>
        public ChartSeriesLabelStrategy Strategy
        {
            get
            {
                return this.GetValue(StrategyProperty) as ChartSeriesLabelStrategy;
            }
            set
            {
                this.SetValue(StrategyProperty, value);
            }
        }
    }
}
