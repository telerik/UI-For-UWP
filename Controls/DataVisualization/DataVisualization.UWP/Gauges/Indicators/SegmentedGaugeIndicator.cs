using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Telerik.UI.Automation.Peers;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.DataVisualization
{
    /// <summary>
    /// This is the base class for the segmented indicators.
    /// It defines a collection of segments and provides
    /// a grid panel for the layout of the segments.
    /// </summary>
    [TemplatePart(Name = "PART_Layout", Type = typeof(Grid))]
    [ContentProperty(Name = "Segments")]
    public abstract class SegmentedGaugeIndicator : GaugeIndicator
    {
        internal IndicatorSegmentCollection segments = new IndicatorSegmentCollection();
        private const string LayoutPartName = "PART_Layout";
        private List<SegmentInfo> segmentInfos = new List<SegmentInfo>();

        private Grid layout;
        private double totalSegmentLength;
        private Size lastAvailableSize = Size.Empty;

        /// <summary>
        /// Initializes a new instance of the SegmentedGaugeIndicator class.
        /// </summary>
        protected SegmentedGaugeIndicator()
        {
            this.segments.CollectionChanged += this.SegmentsCollectionChanged;
        }

        /// <summary>
        /// Gets the segments collection.
        /// </summary>
        /// <remarks>
        /// Segments can be added and removed from this collection.
        /// Every such action triggers state resets and will be slow if
        /// done often.
        /// </remarks>
        public IndicatorSegmentCollection Segments
        {
            get
            {
                return this.segments;
            }
        }

        /// <summary>
        /// Gets a list of Segment info. These info are 
        /// used during the creation of the visual representation of
        /// the segments.
        /// </summary>
        internal List<SegmentInfo> SegmentInfos
        {
            get
            {
                return this.segmentInfos;
            }
        }

        /// <summary>
        /// Gets the total length of the currently added segments.
        /// </summary>
        protected double TotalSegmentLength
        {
            get
            {
                return this.totalSegmentLength;
            }
        }

        /// <summary>
        /// Gets the layout panel for this segmented indicator.
        /// </summary>
        protected Grid Layout
        {
            get
            {
                return this.layout;
            }
        }

        /// <summary>
        /// This is a virtual method that resets the state of the indicator.
        /// The parent range is responsible to (indirectly) call this method when
        /// a property of importance changes.
        /// </summary>
        /// <param name="availableSize">A size which can be used by the update logic.</param>
        /// <remarks>
        /// The linear range for example triggers the UpdateOverride() method
        /// of its indicators when its Orientation property changes.
        /// </remarks>
        internal override void UpdateOverride(Size availableSize)
        {
            base.UpdateOverride(availableSize);

            this.totalSegmentLength = 0;
            foreach (BarIndicatorSegment segment in this.segments)
            {
                this.totalSegmentLength += segment.Length;
            }

            this.lastAvailableSize = availableSize;

            this.ResetSegments(availableSize);
            this.SyncWithValue(this.ActualValue);
        }

        /// <summary>
        /// A virtual method that is called when the value of this indicator changes.
        /// </summary>
        /// <param name="newValue">The new value.</param>
        /// <param name="oldValue">The old value.</param>
        internal override void OnValueChanged(double newValue, double oldValue)
        {
            base.OnValueChanged(newValue, oldValue);

            if (!this.lastAvailableSize.IsEmpty)
            {
                this.ResetSegments(this.lastAvailableSize);
            }

            this.SyncWithValue(newValue);
        }

        /// <summary>
        /// This method is called whenever the segments
        /// need to recreate their visual representation.
        /// </summary>
        /// <param name="availableSize">The available size which the visual parts can occupy.</param>
        internal abstract void ResetSegments(Size availableSize);

        /// <summary>
        /// This method is called so that a segmented indicator can synchronize
        /// its visual state with its current value.
        /// </summary>
        /// <param name="value">The value to synchronize with.</param>
        internal abstract void SyncWithValue(double value);

        /// <summary>
        /// Initializes the template parts of the indicator (see the TemplatePart attributes for more information).
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            base.ApplyTemplateCore();

            this.layout = this.GetTemplatePartField<Grid>(LayoutPartName);
            bool applied = this.layout != null;

            if (applied)
            {
                foreach (BarIndicatorSegment segment in this.segments)
                {
                    segment.Owner = this;
                    this.layout.Children.Add(segment);
                }
            }

            return applied;
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SegmentedGaugeIndicatorAutomationPeer(this);
        }

        private void SegmentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            BarIndicatorSegment segment;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    this.layout.Children.Clear();
                    foreach (BarIndicatorSegment newSegment in this.segments)
                    {
                        this.layout.Children.Add(newSegment);
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    segment = e.NewItems[0] as BarIndicatorSegment;
                    segment.Owner = this;
                    this.layout.Children.Add(segment);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    segment = e.OldItems[0] as BarIndicatorSegment;
                    segment.Owner = null;
                    this.layout.Children.Remove(segment);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    segment = e.OldItems[0] as BarIndicatorSegment;
                    segment.Owner = null;
                    this.layout.Children.Remove(segment);

                    segment = e.NewItems[0] as BarIndicatorSegment;
                    segment.Owner = this;
                    this.layout.Children.Add(segment);
                    break;
            }

            this.ScheduleUpdate();
        }

        /// <summary>
        /// This class is internally created during
        /// segment reset in order to provide information
        /// during value synchronization.
        /// </summary>
        internal class SegmentInfo
        {
            /// <summary>
            /// Gets or sets a path segment.
            /// </summary>
            public PathSegment PathSegment
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the start of the path segment.
            /// </summary>
            public double Start
            {
                get;
                set;
            }

            /// <summary>
            /// Gets or sets the end of the part segment.
            /// </summary>
            public double End
            {
                get;
                set;
            }
        }
    }
}
