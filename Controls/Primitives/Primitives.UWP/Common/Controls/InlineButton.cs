using System;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;

namespace Telerik.UI.Xaml.Controls.Primitives.Common
{
    /// <summary>
    /// Represents a custom Button control that is part of a composite UI.  This type supports Telerik's infrastructure and is not intended to be used outside the composite UI.
    /// </summary>
    public class InlineButton : Button
    {
        /// <summary>
        /// Identifies the <see cref="PressedBackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PressedBackgroundBrushProperty =
            DependencyProperty.Register(nameof(PressedBackgroundBrush), typeof(Brush), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PressedForegroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PressedForegroundBrushProperty =
            DependencyProperty.Register(nameof(PressedForegroundBrush), typeof(Brush), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PressedBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PressedBorderBrushProperty =
            DependencyProperty.Register(nameof(PressedBorderBrush), typeof(Brush), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PointerOverBackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerOverBackgroundBrushProperty =
            DependencyProperty.Register(nameof(PointerOverBackgroundBrush), typeof(Brush), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PointerOverForegroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerOverForegroundBrushProperty =
            DependencyProperty.Register(nameof(PointerOverForegroundBrush), typeof(Brush), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PointerOverBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerOverBorderBrushProperty =
            DependencyProperty.Register(nameof(PointerOverBorderBrush), typeof(Brush), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DisabledBackgroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledBackgroundBrushProperty =
            DependencyProperty.Register(nameof(DisabledBackgroundBrush), typeof(Brush), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DisabledForegroundBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledForegroundBrushProperty =
            DependencyProperty.Register(nameof(DisabledForegroundBrush), typeof(Brush), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DisabledBorderBrush"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledBorderBrushProperty =
            DependencyProperty.Register(nameof(DisabledBorderBrush), typeof(Brush), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="RepeatDelay"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RepeatDelayProperty =
            DependencyProperty.Register(nameof(RepeatDelay), typeof(int), typeof(InlineButton), new PropertyMetadata(250, OnRepeatDelayChanged));

        /// <summary>
        /// Identifies the <see cref="RepeatInterval"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty RepeatIntervalProperty =
            DependencyProperty.Register(nameof(RepeatInterval), typeof(int), typeof(InlineButton), new PropertyMetadata(50, OnRepeatIntervalChanged));

        /// <summary>
        /// Identifies the <see cref="IsRepeatingEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsRepeatingEnabledProperty =
            DependencyProperty.Register(nameof(IsRepeatingEnabled), typeof(bool), typeof(InlineButton), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="IconSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconSourceProperty =
          DependencyProperty.Register(nameof(IconSource), typeof(BitmapImage), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PressedIconSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PressedIconSourceProperty =
            DependencyProperty.Register(nameof(PressedIconSource), typeof(BitmapImage), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="PointerOverIconSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty PointerOverIconSourceProperty =
            DependencyProperty.Register(nameof(PointerOverIconSource), typeof(BitmapImage), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DisabledIconSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DisabledIconSourceProperty =
           DependencyProperty.Register(nameof(DisabledIconSource), typeof(BitmapImage), typeof(InlineButton), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IconStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconStyleProperty =
            DependencyProperty.Register(nameof(IconStyle), typeof(Style), typeof(InlineButton), new PropertyMetadata(null));

        private DispatcherTimer repeatTimer;
        private bool currentlyRepeating;
        private bool privatePropertyChange;
        private ButtonAutomationPeer automationPeer;

        /// <summary>
        /// Initializes a new instance of the <see cref="InlineButton" /> class.
        /// </summary>
        public InlineButton()
        {
            this.DefaultStyleKey = typeof(InlineButton);

            this.Unloaded += this.OnUnloaded;

            this.repeatTimer = new DispatcherTimer();
            this.repeatTimer.Tick += this.OnRepeatTimerTick;

            this.automationPeer = new ButtonAutomationPeer(this);
        }

        /// <summary>
        /// Gets or sets a value indicating whether the button will act like a RepeatButton when a pointer is pressed over it.
        /// </summary>
        public bool IsRepeatingEnabled
        {
            get
            {
                return (bool)this.GetValue(IsRepeatingEnabledProperty);
            }
            set
            {
                this.SetValue(IsRepeatingEnabledProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the time, in milliseconds, that the button waits when it is pressed before it starts repeating the click action. Defaults to 250 milliseconds.
        /// </summary>
        public int RepeatDelay
        {
            get
            {
                return (int)this.GetValue(RepeatDelayProperty);
            }
            set
            {
                this.SetValue(RepeatDelayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the time, in milliseconds, between repetitions of the click action, as soon as repeating starts. Defaults to 50 milliseconds.
        /// </summary>
        public int RepeatInterval
        {
            get
            {
                return (int)this.GetValue(RepeatIntervalProperty);
            }
            set
            {
                this.SetValue(RepeatIntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the <see cref="P:Background"/> value in the Pressed visual state.
        /// </summary>
        public Brush PressedBackgroundBrush
        {
            get
            {
                return this.GetValue(PressedBackgroundBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(PressedBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the <see cref="P:Foreground"/> value in the Pressed visual state.
        /// </summary>
        public Brush PressedForegroundBrush
        {
            get
            {
                return this.GetValue(PressedForegroundBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(PressedForegroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the <see cref="P:Border"/> value in the Pressed visual state.
        /// </summary>
        public Brush PressedBorderBrush
        {
            get
            {
                return this.GetValue(PressedBorderBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(PressedBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the <see cref="P:Background"/> value in the PointerOver visual state.
        /// </summary>
        public Brush PointerOverBackgroundBrush
        {
            get
            {
                return this.GetValue(PointerOverBackgroundBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(PointerOverBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the <see cref="P:Foreground"/> value in the PointerOver visual state.
        /// </summary>
        public Brush PointerOverForegroundBrush
        {
            get
            {
                return this.GetValue(PointerOverForegroundBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(PointerOverForegroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the <see cref="P:Border"/> value in the PointerOver visual state.
        /// </summary>
        public Brush PointerOverBorderBrush
        {
            get
            {
                return this.GetValue(PointerOverBorderBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(PointerOverBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the <see cref="P:Background"/> value in the Disabled visual state.
        /// </summary>
        public Brush DisabledBackgroundBrush
        {
            get
            {
                return this.GetValue(DisabledBackgroundBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(DisabledBackgroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the <see cref="P:Foreground"/> value in the Disabled visual state.
        /// </summary>
        public Brush DisabledForegroundBrush
        {
            get
            {
                return this.GetValue(DisabledForegroundBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(DisabledForegroundBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> instance that defines the <see cref="P:Border"/> value in the Disabled visual state.
        /// </summary>
        public Brush DisabledBorderBrush
        {
            get
            {
                return this.GetValue(DisabledBorderBrushProperty) as Brush;
            }
            set
            {
                this.SetValue(DisabledBorderBrushProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> used as an Icon by the <see cref="InlineButton"/>.
        /// </summary>
        public BitmapImage IconSource
        {
            get
            {
                return (BitmapImage)GetValue(IconSourceProperty);
            }
            set
            {
                this.SetValue(IconSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> for the Pressed visual state of the <see cref="InlineButton"/>.
        /// </summary>
        public BitmapImage PressedIconSource
        {
            get
            {
                return (BitmapImage)GetValue(PressedIconSourceProperty);
            }
            set
            {
                this.SetValue(PressedIconSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> for the PointerOver visual state of the <see cref="InlineButton"/>.
        /// </summary>
        public BitmapImage PointerOverIconSource
        {
            get
            {
                return (BitmapImage)GetValue(PointerOverIconSourceProperty);
            }
            set
            {
                this.SetValue(PointerOverIconSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="BitmapImage"/> for the Disabled visual state of the <see cref="InlineButton"/>.
        /// </summary>
        public BitmapImage DisabledIconSource
        {
            get
            {
                return (BitmapImage)GetValue(DisabledIconSourceProperty);
            }
            set
            {
                this.SetValue(DisabledIconSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the icon content of the <see cref="InlineButton"/>.
        /// </summary>
        public Style IconStyle
        {
            get
            {
                return (Style)GetValue(IconStyleProperty);
            }
            set
            {
                this.SetValue(IconStyleProperty, value);
            }
        }

        /// <summary>
        /// Called before the PointerPressed event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerPressed(PointerRoutedEventArgs e)
        {
            base.OnPointerPressed(e);

            if (e == null || !this.IsRepeatingEnabled)
            {
                return;
            }

            this.RestartRepeatTimer(this.RepeatDelay);
        }

        /// <summary>
        /// Called before the PointerCaptureLost event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerCaptureLost(PointerRoutedEventArgs e)
        {
            base.OnPointerCaptureLost(e);

            this.StopRepeatTimer();
        }

        /// <summary>
        /// Called before the PointerReleased event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerReleased(PointerRoutedEventArgs e)
        {
            base.OnPointerReleased(e);

            this.StopRepeatTimer();
        }

        /// <summary>
        /// Called before the PointerMoved event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerMoved(PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            if (e == null || !this.currentlyRepeating)
            {
                return;
            }

            var point = e.GetCurrentPoint(this);
            var rect = new Rect(0, 0, this.ActualWidth, this.ActualHeight);

            if (rect.Contains(point.Position))
            {
                if (!this.repeatTimer.IsEnabled)
                {
                    this.repeatTimer.Start();
                }
            }
            else
            {
                this.repeatTimer.Stop();
            }
        }

        private static void OnRepeatIntervalChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineButton button = d as InlineButton;
            if (button.privatePropertyChange)
            {
                return;
            }

            int newValue = (int)e.NewValue;
            if (newValue < 0)
            {
                button.ChangePropertyPrivately(RepeatIntervalProperty, 0);
                newValue = 0;
            }

            button.repeatTimer.Interval = TimeSpan.FromMilliseconds(newValue);
        }

        private static void OnRepeatDelayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            InlineButton button = d as InlineButton;
            if (button.privatePropertyChange)
            {
                return;
            }

            int newValue = (int)e.NewValue;
            if (newValue < 0)
            {
                button.ChangePropertyPrivately(RepeatDelayProperty, 0);
            }
        }

        private void OnRepeatTimerTick(object sender, object e)
        {
            if (this.currentlyRepeating)
            {
#if !WINDOWS_UWP
                this.automationPeer.Invoke();
#endif
            }
            else
            {
                this.currentlyRepeating = true;
                this.RestartRepeatTimer(this.RepeatInterval);
            }
        }

        private void ChangePropertyPrivately(DependencyProperty property, object value)
        {
            this.privatePropertyChange = true;
            this.SetValue(property, value);
            this.privatePropertyChange = false;
        }

        private void RestartRepeatTimer(int milliseconds)
        {
            this.repeatTimer.Stop();
            this.repeatTimer.Interval = TimeSpan.FromMilliseconds(milliseconds);
            this.repeatTimer.Start();
        }

        private void StopRepeatTimer()
        {
            this.repeatTimer.Stop();
            this.currentlyRepeating = false;
        }

        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.StopRepeatTimer();
        }
    }
}
