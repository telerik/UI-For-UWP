using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Shapes;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This class represents a segment in a segmented bar indicator.
    /// A segment can have a distinctive color, length and thickness.
    /// </summary>
    [TemplatePart(Name = "PART_Layout", Type = typeof(Canvas))]
    public class BarIndicatorSegment : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Length"/> property.
        /// </summary>
        public static readonly DependencyProperty LengthProperty =
            DependencyProperty.Register(nameof(Length), typeof(double), typeof(BarIndicatorSegment), new PropertyMetadata(1d, OnLengthPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Thickness"/> property.
        /// </summary>
        public static readonly DependencyProperty ThicknessProperty =
            DependencyProperty.Register(nameof(Thickness), typeof(double), typeof(BarIndicatorSegment), new PropertyMetadata(1d, OnThicknessPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="Stroke"/> property.
        /// </summary>
        public static readonly DependencyProperty StrokeProperty =
            DependencyProperty.Register(nameof(Stroke), typeof(Brush), typeof(BarIndicatorSegment), new PropertyMetadata(null, OnStrokePropertyChanged));

        private const string LayoutPartName = "PART_Layout";

        private Path path = new Path();
        private double lengthCache = 1;
        private double thicknessCache = 1;
        private Brush strokeCache;
        private SegmentedGaugeIndicator owner;
        private Panel layoutRoot;

        /// <summary>
        /// Initializes a new instance of the <see cref="BarIndicatorSegment"/> class.
        /// </summary>
        public BarIndicatorSegment()
        {
            this.DefaultStyleKey = typeof(BarIndicatorSegment);
        }

        /// <summary>
        /// Gets or sets the length of the current segment.
        /// </summary>
        /// <remarks>
        /// The length does not need to be an absolute value.
        /// Internally a ratio is calculated that determines the
        /// actual length of each segment, relative to the actual value range of the owning <see cref="GaugePanel"/> instance.
        /// For example if a segmented indicator has three segments
        /// of length 1, they will be of evenly placed over the available indicator space.
        /// This property is somewhat similar to the relative size of the Grid panel's columns and rows.
        /// </remarks>
        public double Length
        {
            get
            {
                return this.lengthCache;
            }
            set
            {
                this.SetValue(LengthProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the color of the segment.
        /// </summary>
        public Brush Stroke
        {
            get
            {
                return this.strokeCache;
            }
            set
            {
                this.SetValue(StrokeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the thickness of the segment.
        /// </summary>
        public double Thickness
        {
            get
            {
                return this.thicknessCache;
            }
            set
            {
                this.SetValue(ThicknessProperty, value);
            }
        }

        internal Path Path
        {
            get
            {
                return this.path;
            }
        }

        internal SegmentedGaugeIndicator Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
            }
        }

        /// <summary>
        /// Initializes the template parts of the control (see the TemplatePart attributes for more information).
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            if (this.layoutRoot != null)
            {
                this.layoutRoot.Children.Remove(this.path);
            }

            this.layoutRoot = this.GetTemplatePartField<Panel>(LayoutPartName);
            bool applied = this.layoutRoot != null;

            if (applied)
            {
                this.layoutRoot.Children.Add(this.path);
                this.UpdatePath();
            }

            return applied;
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new BarIndicatorSegmentAutomationPeer(this);
        }

        private static void OnLengthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BarIndicatorSegment segment = d as BarIndicatorSegment;
            segment.lengthCache = (double)e.NewValue;
            segment.UpdateOwner();
        }

        private static void OnThicknessPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BarIndicatorSegment segment = d as BarIndicatorSegment;
            segment.thicknessCache = (double)e.NewValue;
            segment.UpdatePath();
        }

        private static void OnStrokePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            BarIndicatorSegment segment = d as BarIndicatorSegment;
            segment.strokeCache = (Brush)e.NewValue;
            segment.UpdatePath();
        }

        private void UpdateOwner()
        {
            if (this.owner == null)
            {
                return;
            }

            this.owner.ScheduleUpdate();
        }

        private void UpdatePath()
        {
            this.path.Stroke = this.strokeCache;
            this.path.StrokeThickness = this.thicknessCache;
        }
    }
}
