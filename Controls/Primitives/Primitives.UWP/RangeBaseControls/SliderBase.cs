using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Telerik.UI.Automation.Peers;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines a basic slider control properties.
    /// </summary>
    public abstract class SliderBase : RangeInputBase
    {
        /// <summary>
        /// Identifies the <see cref="TickFrequency"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TickFrequencyProperty =
            DependencyProperty.Register(nameof(TickFrequency), typeof(double), typeof(SliderBase), new PropertyMetadata(1d, OnTickFrequencyChanged));

        /// <summary>
        /// Identifies the <see cref="Orientation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(nameof(Orientation), typeof(Orientation), typeof(SliderBase), new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        /// <summary>
        /// Identifies the <see cref="SelectionStart"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionStartProperty =
            DependencyProperty.Register(nameof(SelectionStart), typeof(double), typeof(SliderBase), new PropertyMetadata(4d, OnSelectionStartChanged));

        /// <summary>
        /// Identifies the <see cref="SelectionEnd"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionEndProperty =
            DependencyProperty.Register(nameof(SelectionEnd), typeof(double), typeof(SliderBase), new PropertyMetadata(6d, OnSelectionEndChanged));

        /// <summary>
        /// Identifies the <see cref="IsDeferredDraggingEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsDeferredDraggingEnabledProperty =
            DependencyProperty.Register(nameof(IsDeferredDraggingEnabled), typeof(bool), typeof(SliderBase), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="SnapsTo"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SnapsToProperty =
            DependencyProperty.Register(nameof(SnapsTo), typeof(SnapsTo), typeof(SliderBase), new PropertyMetadata(SnapsTo.None, OnSnapsToPropertyChanged));

        // TODO: Rethink the naming and consider whether the property is needed

        /// <summary>
        /// Identifies the <see cref="TrackTapMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TrackTapModeProperty =
            DependencyProperty.Register(nameof(TrackTapMode), typeof(RangeSliderTrackTapMode), typeof(SliderBase), new PropertyMetadata(RangeSliderTrackTapMode.IncrementByLargeChange));

        /// <summary>
        /// Identifies the <see cref="ShowValueToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowValueToolTipProperty =
           DependencyProperty.Register(nameof(ShowValueToolTip), typeof(bool), typeof(SliderBase), new PropertyMetadata(true, OnShowValueToolTipPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ShowRangeToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowRangeToolTipProperty =
           DependencyProperty.Register(nameof(ShowRangeToolTip), typeof(bool), typeof(SliderBase), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="ToolTipFormat"/> property.
        /// </summary>
        public static readonly DependencyProperty ToolTipFormatProperty =
            DependencyProperty.Register(nameof(ToolTipFormat), typeof(string), typeof(SliderBase), new PropertyMetadata("{0:0.00}"));

        /// <summary>
        /// Gets or sets the logical tick frequency of the scale.
        /// </summary>
        public double TickFrequency
        {
            get
            {
                return (double)this.GetValue(TickFrequencyProperty);
            }
            set
            {
                this.SetValue(TickFrequencyProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the control.
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
        /// Gets or sets the desired start value of the control. It determines the visual position of the SelectionStartThumb.
        /// </summary>
        public double SelectionStart
        {
            get
            {
                return (double)this.GetValue(SelectionStartProperty);
            }
            set
            {
                this.SetValue(SelectionStartProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the desired end value of the control. It determines the visual position of the SelectionEndThumb.
        /// </summary>
        public double SelectionEnd
        {
            get
            {
                return (double)this.GetValue(SelectionEndProperty);
            }
            set
            {
                this.SetValue(SelectionEndProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SelectionStart/SelectionEnd values will be differed while dragging.
        /// </summary>
        public bool IsDeferredDraggingEnabled
        {
            get
            {
                return (bool)this.GetValue(IsDeferredDraggingEnabledProperty);
            }
            set
            {
                this.SetValue(IsDeferredDraggingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the SelectionStart/SelectionEnd values will be snapped after drag complete.
        /// </summary>
        public SnapsTo SnapsTo
        {
            get
            {
                return (SnapsTo)this.GetValue(SnapsToProperty);
            }
            set
            {
                this.SetValue(SnapsToProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the value that indicates with what step the selection thumb position will be updated when a tap/click event occurs on the track <see cref="RangeSlider.RangeTrackPrimitive"/>
        /// </summary>
        public RangeSliderTrackTapMode TrackTapMode
        {
            get
            {
                return (RangeSliderTrackTapMode)this.GetValue(TrackTapModeProperty);
            }
            set
            {
                this.SetValue(TrackTapModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a tool tip, displaying the current pointer value, will be displayed.
        /// </summary>
        public bool ShowValueToolTip
        {
            get
            {
                return (bool)this.GetValue(ShowValueToolTipProperty);
            }
            set
            {
                this.SetValue(ShowValueToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether a tool tip, displaying the current value range, will be displayed.
        /// </summary>
        public bool ShowRangeToolTip
        {
            get
            {
                return (bool)this.GetValue(ShowRangeToolTipProperty);
            }
            set
            {
                this.SetValue(ShowRangeToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the string that is used to format the tooltip text of the component. 
        /// </summary>
        public string ToolTipFormat
        {
            get
            {
                return (string)this.GetValue(ToolTipFormatProperty);
            }
            set
            {
                this.SetValue(ToolTipFormatProperty, value);
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new SliderBaseAutomationPeer(this);
        }

        internal virtual void OnSelectionStartChanged(double oldSelectionStart, double newSelectionStart)
        {
        }

        internal virtual void OnSelectionEndChanged(double oldSelectionEnd, double newSelectionEnd)
        {
        }

        internal virtual void OnSnapsToPropertyChanged(SnapsTo oldSnapsTo, SnapsTo newSnapsTo)
        {
        }

        internal virtual void OnTickFrequencyChanged(double oldTickFrequency, double newTickFrequency)
        {
        }

        internal virtual void OnOrientationChanged(Orientation oldOrientation, Orientation newOrientation)
        {
        }

        internal virtual void OnShowValueToolTipPropertyChanged(bool oldShowValueToolTip, bool newShowValueToolTip)
        {
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SliderBase;
            if (sender != null)
            {
                sender.OnOrientationChanged((Orientation)e.OldValue, (Orientation)e.NewValue);
            }
        }

        private static void OnTickFrequencyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SliderBase;
            if (sender != null)
            {
                sender.OnTickFrequencyChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private static void OnSelectionStartChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SliderBase;
            if (sender != null)
            {
                sender.OnSelectionStartChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private static void OnSelectionEndChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SliderBase;
            if (sender != null)
            {
                sender.OnSelectionEndChanged((double)e.OldValue, (double)e.NewValue);
            }
        }

        private static void OnSnapsToPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SliderBase;
            if (sender != null)
            {
                sender.OnSnapsToPropertyChanged((SnapsTo)e.OldValue, (SnapsTo)e.NewValue);
            }
        }

        private static void OnShowValueToolTipPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as SliderBase;
            if (sender != null)
            {
                sender.OnShowValueToolTipPropertyChanged((bool)e.OldValue, (bool)e.NewValue);
            }
        } 
    }
}
