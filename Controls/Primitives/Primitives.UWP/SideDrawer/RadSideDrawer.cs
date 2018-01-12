using System;
using System.Linq;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.SideDrawer.Commands;
using Windows.Foundation;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a control that enables a user to show a drawer from any side of a content.
    /// </summary>
    [TemplatePart(Name = "PART_ShowDrawerButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_SideDrawer", Type = typeof(Canvas))]
    public partial class RadSideDrawer : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="MainContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MainContentProperty =
            DependencyProperty.Register(nameof(MainContent), typeof(object), typeof(RadSideDrawer), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="MainContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty MainContentTemplateProperty =
            DependencyProperty.Register(nameof(MainContentTemplate), typeof(DataTemplate), typeof(RadSideDrawer), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DrawerContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerContentProperty =
            DependencyProperty.Register(nameof(DrawerContent), typeof(object), typeof(RadSideDrawer), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DrawerContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerContentTemplateProperty =
            DependencyProperty.Register(nameof(DrawerContentTemplate), typeof(DataTemplate), typeof(RadSideDrawer), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DrawerTransition"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerTransitionProperty =
            DependencyProperty.Register(nameof(DrawerTransition), typeof(DrawerTransition), typeof(RadSideDrawer), new PropertyMetadata(DrawerTransition.SlideInOnTop, OnDrawerTransitionChanged));

        /// <summary>
        /// Identifies the <see cref="DrawerLocation"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerLocationProperty =
            DependencyProperty.Register(nameof(DrawerLocation), typeof(DrawerLocation), typeof(RadSideDrawer), new PropertyMetadata(DrawerLocation.Left, OnDrawerLocationChagned));

        /// <summary>
        /// Identifies the <see cref="DrawerButtonStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerButtonStyleProperty =
            DependencyProperty.Register(nameof(DrawerButtonStyle), typeof(Style), typeof(RadSideDrawer), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="DrawerButtonVerticalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerButtonVerticalAlignmentProperty =
            DependencyProperty.Register(nameof(DrawerButtonVerticalAlignment), typeof(VerticalAlignment), typeof(RadSideDrawer), new PropertyMetadata(VerticalAlignment.Top));

        /// <summary>
        /// Identifies the <see cref="DrawerButtonHorizontalAlignment"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerButtonHorizontalAlignmentProperty =
            DependencyProperty.Register(nameof(DrawerButtonHorizontalAlignment), typeof(HorizontalAlignment), typeof(RadSideDrawer), new PropertyMetadata(HorizontalAlignment.Left));

        /// <summary>
        /// Identifies the <see cref="AnimationDuration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimationDurationProperty =
            DependencyProperty.Register(nameof(AnimationDuration), typeof(Duration), typeof(RadSideDrawer), new PropertyMetadata(new Duration(TimeSpan.FromMilliseconds(300))));

        /// <summary>
        /// Identifies the <see cref="DrawerState"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerStateProperty =
            DependencyProperty.Register(nameof(DrawerState), typeof(DrawerState), typeof(RadSideDrawer), new PropertyMetadata(DrawerState.Closed, OnDrawerStateChanged));

        /// <summary>
        /// Identifies the <see cref="TapOutsideToClose"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TapOutsideToCloseProperty =
            DependencyProperty.Register(nameof(TapOutsideToClose), typeof(bool), typeof(RadSideDrawer), new PropertyMetadata(true));
        
        /// <summary>
        /// Identifies the <see cref="DrawerLength"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerLengthProperty =
            DependencyProperty.Register(nameof(DrawerLength), typeof(double), typeof(RadSideDrawer), new PropertyMetadata(double.NaN, OnDrawerLengthChanged));

        /// <summary>
        /// Identifies the <see cref="DrawerTransitionFadeOpacity"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DrawerTransitionFadeOpacityProperty =
            DependencyProperty.Register(nameof(DrawerTransitionFadeOpacity), typeof(double), typeof(RadSideDrawer), new PropertyMetadata(0.5, OnDrawerTransitionFadeOpacityChanged));
        
        /// <summary>
        /// Identifies the <see cref="IsOpen"/> dependency property.
        /// </summary>
        internal static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(RadSideDrawer), new PropertyMetadata(false, OnIsOpenChanged));

        internal Grid drawer;

        /// <summary>
        /// Identifies the <see cref="DrawerClip"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty DrawerClipProperty =
            DependencyProperty.Register(nameof(DrawerClip), typeof(double), typeof(RadSideDrawer), new PropertyMetadata(0d, OnDrawerClipChanged));

        /// <summary>
        /// Identifies the <see cref="SlideoutSeekedAnimationClip"/> dependency property.
        /// </summary>
        private static readonly DependencyProperty SlideoutSeekedAnimationClipProperty =
            DependencyProperty.Register(nameof(SlideoutSeekedAnimationClip), typeof(double), typeof(RadSideDrawer), new PropertyMetadata(0d, OnSlideoutSeekedAnimationClipChanged));

        private static double swipeAreaDefaultLength = 20d;
        private Button showDrawerButton;
        private Border swipeAreaElement;
        private Canvas sideDrawerRoot;
        private SideOutPanel mainContent;
        private DrawerManipulationMode drawerManipulationMode = DrawerManipulationMode.Both;
        private CommandService commandService;
        private bool isAnimationUpdateScheduled;
        private bool closeDrawer = true;
        private bool openInitially = false;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RadSideDrawer"/> class.
        /// </summary>
        public RadSideDrawer()
        {
            this.DefaultStyleKey = typeof(RadSideDrawer);

            this.commandService = new CommandService(this);

            this.SizeChanged += this.RadSideDrawer_SizeChanged;
        }

        /// <summary>
        /// Gets or sets the  length of the drawer length (height of width) depending on the drawer location.
        /// </summary>
        public double DrawerLength
        {
            get { return (double)GetValue(DrawerLengthProperty); }
            set { this.SetValue(DrawerLengthProperty, value); }
        }

        /// <summary>
        /// Gets or sets the  Maximum opacity to which the of the drawer will animate its main content when opened.
        /// </summary>
        public double DrawerTransitionFadeOpacity
        {
            get { return (double)GetValue(DrawerTransitionFadeOpacityProperty); }
            set { this.SetValue(DrawerTransitionFadeOpacityProperty, value); }
        }

        /// <summary>
        /// Gets or sets the main content of the control.
        /// </summary>
        public object MainContent
        {
            get
            {
                return (object)this.GetValue(MainContentProperty);
            }
            set
            {
                this.SetValue(MainContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template of the <see cref="MainContent"/>.
        /// </summary>
        public DataTemplate MainContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(MainContentTemplateProperty);
            }
            set
            {
                this.SetValue(MainContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of drawer.
        /// </summary>
        public object DrawerContent
        {
            get
            {
                return (object)this.GetValue(DrawerContentProperty);
            }
            set
            {
                this.SetValue(DrawerContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template of the <see cref="DrawerContentTemplate"/>.
        /// </summary>
        public DataTemplate DrawerContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(DrawerContentTemplateProperty);
            }
            set
            {
                this.SetValue(DrawerContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the transition(set of animations) used to show/hide the drawer.
        /// </summary>
        public DrawerTransition DrawerTransition
        {
            get
            {
                return (DrawerTransition)this.GetValue(DrawerTransitionProperty);
            }
            set
            {
                this.SetValue(DrawerTransitionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the orientation of the drawer i.e. from which side of the MainContent to show(left, right, top or bottom). 
        /// </summary>
        public DrawerLocation DrawerLocation
        {
            get
            {
                return (DrawerLocation)this.GetValue(DrawerLocationProperty);
            }
            set
            {
                this.SetValue(DrawerLocationProperty, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="DrawerState"/> of the drawer. 
        /// </summary>
        public DrawerState DrawerState
        {
            get
            {
                return (DrawerState)this.GetValue(DrawerStateProperty);
            }
            private set
            {
                this.SetValue(DrawerStateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style of the DrawerButton which is used to show/hide the drawer.
        /// </summary>
        public Style DrawerButtonStyle
        {
            get
            {
                return (Style)this.GetValue(DrawerButtonStyleProperty);
            }
            set
            {
                this.SetValue(DrawerButtonStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the vertical alignment of the DrawerButton.
        /// </summary>
        public VerticalAlignment DrawerButtonVerticalAlignment
        {
            get
            {
                return (VerticalAlignment)this.GetValue(DrawerButtonVerticalAlignmentProperty);
            }
            set
            {
                this.SetValue(DrawerButtonVerticalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the horizontal alignment of the DrawerButton.
        /// </summary>
        public HorizontalAlignment DrawerButtonHorizontalAlignment
        {
            get
            {
                return (HorizontalAlignment)this.GetValue(DrawerButtonHorizontalAlignmentProperty);
            }
            set
            {
                this.SetValue(DrawerButtonHorizontalAlignmentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the duration of the animations used to show/hide the drawer.
        /// </summary>
        public Duration AnimationDuration
        {
            get
            {
                return (Duration)this.GetValue(AnimationDurationProperty);
            }
            set
            {
                this.SetValue(AnimationDurationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DrawerManipulationMode used to show/hide the drawer.
        /// </summary>
        public DrawerManipulationMode DrawerManipulationMode
        {
            get
            {
                return this.drawerManipulationMode;
            }
            set
            {
                this.drawerManipulationMode = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="CommandService"/> instance that manages the commanding behavior of this instance.
        /// </summary>
        public CommandService CommandService
        {
            get
            {
                return this.commandService;
            }
        }

        /// <summary>
        /// Gets the collection with all the custom commands registered with the <see cref="CommandService"/>. 
        /// Custom commands have higher priority than the built-in (default) ones.
        /// </summary>
        public CommandCollection<RadSideDrawer> Commands
        {
            get
            {
                return this.CommandService.UserCommands;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to close the drawer if a user taps outside the drawer.
        /// </summary>
        public bool TapOutsideToClose
        {
            get
            {
                return (bool)this.GetValue(TapOutsideToCloseProperty);
            }
            set
            {
                this.SetValue(TapOutsideToCloseProperty, value);
            }
        }

        internal bool IsOpen
        {
            get
            {
                return (bool)this.GetValue(IsOpenProperty);
            }
            set
            {
                this.SetValue(IsOpenProperty, value);
            }
        }

        private double DrawerClip
        {
            get
            {
                return (double)this.GetValue(DrawerClipProperty);
            }
            set
            {
                this.SetValue(DrawerClipProperty, value);
            }
        }

        private double SlideoutSeekedAnimationClip
        {
            get
            {
                return (double)this.GetValue(SlideoutSeekedAnimationClipProperty);
            }
            set
            {
                this.SetValue(SlideoutSeekedAnimationClipProperty, value);
            }
        }

        /// <summary>
        /// Shows the drawer, i.e. makes it in DrawerState.Opened.
        /// </summary>
        public void ShowDrawer()
        {
            if (!this.IsTemplateApplied || !this.Context.IsGenerated)
            {
                this.openInitially = true;
                return;
            }

            if (this.DrawerState == DrawerState.Closed)
            {
                this.Context.MainContentStoryBoard.Begin();
                this.Context.DrawerStoryBoard.Begin();

                this.IsOpen = true;
            }
            else if (this.DrawerState == DrawerState.Moving)
            {
                if (!this.IsOpen)
                {
                    this.Context.MainContentStoryBoard.Resume();
                    this.Context.DrawerStoryBoard.Resume();
                    this.IsOpen = true;
                }
                else
                {
                    this.shouldAnimate = false;
                    this.Context.MainContentStoryBoardReverse.Seek(this.AnimationDuration.TimeSpan);
                    this.Context.DrawerStoryBoardReverse.Seek(this.AnimationDuration.TimeSpan);
                    this.Context.MainContentStoryBoardReverse.Resume();
                    this.Context.DrawerStoryBoardReverse.Resume();

                    this.Context.MainContentStoryBoard.Begin();
                    this.Context.DrawerStoryBoard.Begin();

                    this.IsOpen = true;
                }
            }

            // Workaround the missing previewpointerpressed in case this action is called to open the drawer from button inside main content
            this.closeDrawer = false;
            this.InvokeAsync(() => this.closeDrawer = true);
        }

        /// <summary>
        /// Hides the drawer, i.e. makes it in DrawerState.Closed.
        /// </summary>
        public void HideDrawer()
        {
            if (!this.IsTemplateApplied || !this.Context.IsGenerated)
            {
                this.openInitially = false;
                return;
            }

            if (this.DrawerState == DrawerState.Opened)
            {
                this.Context.MainContentStoryBoardReverse.Begin();
                this.Context.DrawerStoryBoardReverse.Begin();

                this.IsOpen = false;
            }
            else if (this.DrawerState == DrawerState.Moving)
            {
                if (this.IsOpen)
                {
                    this.Context.MainContentStoryBoardReverse.Resume();
                    this.Context.DrawerStoryBoardReverse.Resume();
                    this.IsOpen = false;
                }
                else
                {
                    this.shouldAnimate = false;
                    this.Context.MainContentStoryBoard.Seek(this.AnimationDuration.TimeSpan);
                    this.Context.DrawerStoryBoard.Seek(this.AnimationDuration.TimeSpan);
                    this.Context.MainContentStoryBoard.Resume();
                    this.Context.DrawerStoryBoard.Resume();

                    this.Context.MainContentStoryBoardReverse.Begin();
                    this.Context.DrawerStoryBoardReverse.Begin();

                    this.IsOpen = false;
                }
            }
        }

        internal void ToggleDrawer()
        {
            if (this.IsOpen)
            {
                this.Context.MainContentStoryBoardReverse.Begin();
                this.Context.DrawerStoryBoardReverse.Begin();
                this.IsOpen = false;
            }
            else
            {
                this.Context.MainContentStoryBoard.Begin();
                this.Context.DrawerStoryBoard.Begin();
                this.IsOpen = true;
                this.closeDrawer = false;
            }
        }

        /// <summary>
        /// Called in the arrange pass of the layout system.
        /// </summary>
        /// <param name="finalSize">The final size that was given by the layout system.</param>
        /// <returns>The final size of the panel.</returns>
        protected override Windows.Foundation.Size ArrangeOverride(Windows.Foundation.Size finalSize)
        {
            this.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(0, 0, finalSize.Width, finalSize.Height) };

            if (this.isAnimationUpdateScheduled)
            {
                this.CommandService.ExecuteCommand(CommandId.GenerateAnimations, this.GetAnimations());
                this.isAnimationUpdateScheduled = false;
            }

            var swipeAreaLeft = this.DrawerLocation == DrawerLocation.Left ? 0 : finalSize.Width - this.swipeAreaElement.Width;
            var swipeAreaTop = this.DrawerLocation == DrawerLocation.Top ? 0 : finalSize.Height - this.swipeAreaElement.Height;

            Canvas.SetLeft(this.swipeAreaElement, swipeAreaLeft);
            Canvas.SetTop(this.swipeAreaElement, swipeAreaTop);

            return base.ArrangeOverride(finalSize);
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of the layout cycle. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects or based on other considerations such as a fixed container size.
        /// </returns>
        protected override Windows.Foundation.Size MeasureOverride(Windows.Foundation.Size availableSize)
        {
            if (this.drawer == null || this.mainContent == null)
            {
                return base.MeasureOverride(availableSize);
            }
            
            this.drawer.Measure(new Size(availableSize.Width, availableSize.Height));

            this.mainContent.Measure(new Size(availableSize.Width, availableSize.Height));

            if (this.DrawerLocation == DrawerLocation.Left || this.DrawerLocation == DrawerLocation.Right)
            {
                this.drawer.Width = double.IsNaN(this.DrawerLength) ? this.drawer.DesiredSize.Width : this.DrawerLength;
                this.drawer.Height = this.mainContent.DesiredSize.Height;

                this.swipeAreaElement.Width = swipeAreaDefaultLength;
                this.swipeAreaElement.Height = this.drawer.Height;
            }
            else
            {
                this.drawer.Width = this.mainContent.DesiredSize.Width;
                this.drawer.Height = double.IsNaN(this.DrawerLength) ? this.drawer.DesiredSize.Height : this.DrawerLength;

                this.swipeAreaElement.Height = swipeAreaDefaultLength;
                this.swipeAreaElement.Width = this.drawer.Width;
            }

            var largestSize = base.MeasureOverride(availableSize);
            largestSize.Width = Math.Max(largestSize.Width, this.mainContent.DesiredSize.Width);
            largestSize.Height = Math.Max(largestSize.Height, this.mainContent.DesiredSize.Height);

            return largestSize;
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate"/> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied"/> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.showDrawerButton = this.GetTemplatePartField<Button>("PART_SideDrawerButton");
            applied = applied && this.showDrawerButton != null;

            this.sideDrawerRoot = this.GetTemplatePartField<Canvas>("PART_SideDrawer");
            applied = applied && this.sideDrawerRoot != null;

            this.drawer = this.GetTemplatePartField<Grid>("PART_Drawer");
            applied = applied && this.drawer != null;

            this.mainContent = this.GetTemplatePartField<SideOutPanel>("PART_MainContent");
            applied = applied && this.mainContent != null;

            this.swipeAreaElement = new Border
            {
                Background = new SolidColorBrush(Colors.Transparent),
                ManipulationMode = Windows.UI.Xaml.Input.ManipulationModes.All
            };

            this.sideDrawerRoot.Children.Add(this.swipeAreaElement);

            Canvas.SetZIndex(this.drawer, 99999);
            Canvas.SetZIndex(this.swipeAreaElement, 900);

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.showDrawerButton.Click += this.ShowDrawerButton_Click;

            if (this.DrawerManipulationMode != Primitives.DrawerManipulationMode.Button)
            {
                this.SubscribeToManipulationEvents();
            }
            if (this.DrawerManipulationMode == Primitives.DrawerManipulationMode.Gestures)
            {
                this.showDrawerButton.Visibility = Visibility.Collapsed;
            }
            this.mainContent.Tapped += this.MainContent_Tapped;
            this.mainContent.Owner = this;
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            if (this.mainContent != null)
            {
                this.mainContent.ManipulationStarted -= this.MainContent_ManipulationStarted;
                this.mainContent.ManipulationDelta -= this.MainContent_ManipulationDelta;
                this.mainContent.ManipulationCompleted -= this.MainContent_ManipulationCompleted;
                this.mainContent.Tapped -= this.MainContent_Tapped;
                this.mainContent.Owner = null;
            }

            if (this.drawer != null)
            {
                this.drawer.ManipulationStarted -= this.Drawer_ManipulationStarted;
                this.drawer.ManipulationDelta -= this.Drawer_ManipulationDelta;
                this.drawer.ManipulationCompleted -= this.Drawer_ManipulationCompleted;
            }

            base.UnapplyTemplateCore();
        }

        /// <inheritdoc/>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadSideDrawerAutomationPeer(this);
        }
        
        private static void OnDrawerTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sideDrawer = d as RadSideDrawer;
            if (sideDrawer.drawer != null)
            {
                sideDrawer.ResetDrawer();
            }
        }

        private static void OnSlideoutSeekedAnimationClipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            (d as RadSideDrawer).drawer.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(0, 0, 0, 0) };
        }

        private static void OnDrawerLengthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sideDrawer = d as RadSideDrawer;

            if (sideDrawer.IsTemplateApplied)
            {
                sideDrawer.InvalidateMeasure();
            }
        }

        private static void OnDrawerStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sideDrawer = d as RadSideDrawer;
            var mainContent = sideDrawer.MainContent as FrameworkElement;

            if (mainContent != null)
            {
                mainContent.IsHitTestVisible = (DrawerState)e.NewValue == DrawerState.Closed;
            }

            sideDrawer.CommandService.ExecuteCommand(CommandId.DrawerStateChanged, e.NewValue);

            if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
            {
                var peer = FrameworkElementAutomationPeer.FromElement(sideDrawer) as RadSideDrawerAutomationPeer;
                if (peer != null)
                {
                    peer.RaiseExpandCollapseAutomationEvent((DrawerState)e.OldValue, (DrawerState)e.NewValue);
                }
            }
        }

        private static void OnDrawerLocationChagned(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sideDrawer = d as RadSideDrawer;
            if (sideDrawer.drawer != null)
            {
                sideDrawer.ResetDrawer();
            }
        }

        private static void OnDrawerTransitionFadeOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sideDrawer = d as RadSideDrawer;

            if (((double)e.NewValue) > 1 || ((double)e.NewValue) < 0)
            {
                throw new ArgumentException("DrawerTransitionFadeOpacity should be between 0 and 1.", "DrawerTransitionFadeOpacity");
            }

            if (sideDrawer.IsTemplateApplied)
            {
                sideDrawer.InvalidateMeasure();
            }
        }
        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sideDrawer = d as RadSideDrawer;

            if (sideDrawer.DrawerManipulationMode != DrawerManipulationMode.Button)
            {
                sideDrawer.SubscribeToManipulationEvents();

                sideDrawer.UnSubscribeToManipulationEvents();
            }
        }

        private static void OnDrawerClipChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sideDrawer = d as RadSideDrawer;

            sideDrawer.OnDrawerClipChanged();
        }

        private void OnDrawerClipChanged()
        {
            double clipOffset = 0;
            if (this.DrawerTransition == DrawerTransition.SlideAlong)
            {
                if (this.DrawerLocation == DrawerLocation.Left)
                {
                    clipOffset = Canvas.GetLeft(this.drawer);
                }
                else if (this.DrawerLocation == DrawerLocation.Top)
                {
                    clipOffset = Canvas.GetTop(this.drawer);
                }
            }

            double currentLength;

            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:

                    currentLength = Math.Max(this.DrawerClip - clipOffset, 0);

                    this.drawer.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(0, 0, currentLength, this.drawer.Height) };
                    break;
                case DrawerLocation.Right:

                    this.drawer.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(this.DrawerClip, 0, this.drawer.Width, this.drawer.Height) };
                    break;
                case DrawerLocation.Top:
                    currentLength = Math.Max(this.DrawerClip - clipOffset, 0);

                    this.drawer.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(0, 0, this.drawer.Width, currentLength) };
                    break;
                case DrawerLocation.Bottom:
                    this.drawer.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(0, this.DrawerClip, this.drawer.Width, this.drawer.Height) };
                    break;
            }
        }

        private void UpdateDrawerClip(bool refresh, RectangleGeometry oldGeometry)
        {
            double clipOffset = 0;
            if (this.DrawerTransition == DrawerTransition.SlideAlong)
            {
                if (this.DrawerLocation == DrawerLocation.Left)
                {
                    clipOffset = Canvas.GetLeft(this.drawer);
                }
                else if (this.DrawerLocation == DrawerLocation.Top)
                {
                    clipOffset = Canvas.GetTop(this.drawer);
                }
            }

            double currentLength;

            switch (this.DrawerLocation)
            {
                case DrawerLocation.Left:
                    if (oldGeometry == null)
                    {
                        return;
                    }
                    else
                    {
                        currentLength = refresh ? oldGeometry.Bounds.Width : Math.Max(this.DrawerClip - clipOffset, 0);
                    }

                    this.drawer.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(0, 0, currentLength, this.drawer.Height) };
                    break;
                case DrawerLocation.Right:

                    this.drawer.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(this.DrawerClip, 0, this.drawer.Width, this.drawer.Height) };
                    break;
                case DrawerLocation.Top:
                    if (oldGeometry == null)
                    {
                        return;
                    }
                    else
                    {
                        currentLength = refresh ? oldGeometry.Bounds.Height : Math.Max(this.DrawerClip - clipOffset, 0);
                    }
                    this.drawer.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(0, 0, this.drawer.Width, currentLength) };
                    break;
                case DrawerLocation.Bottom:
                    this.drawer.Clip = new RectangleGeometry() { Rect = new Windows.Foundation.Rect(0, this.DrawerClip, this.drawer.Width, this.drawer.Height) };
                    break;
            }
        }
        
        private void RadSideDrawer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (e.NewSize != e.PreviousSize)
            {
                var oldClip = this.drawer != null ? this.drawer.Clip : null;
                this.CommandService.ExecuteCommand(CommandId.GenerateAnimations, this.GetAnimations(true));

                if (this.DrawerState == DrawerState.Opened)
                {
                    this.UpdateDrawerClip(true, oldClip);

                    this.Context.MainContentStoryBoard.Begin();
                    this.Context.DrawerStoryBoard.Begin();
                    this.Context.MainContentStoryBoard.Seek(this.AnimationDuration.TimeSpan);
                    this.Context.DrawerStoryBoard.Seek(this.AnimationDuration.TimeSpan);
                    this.Context.MainContentStoryBoard.Resume();
                    this.Context.DrawerStoryBoard.Resume();
                }

                if (this.openInitially)
                {
                    this.openInitially = false;
                    this.ShowDrawer();
                }
            }
        }

        private void MainContent_Tapped(object sender, Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (this.IsOpen && this.closeDrawer && this.TapOutsideToClose)
            {
                this.Context.MainContentStoryBoardReverse.Begin();
                this.Context.DrawerStoryBoardReverse.Begin();
                this.IsOpen = false;
            }

            this.closeDrawer = true;
        }

        private void ShowDrawerButton_Click(object sender, RoutedEventArgs e)
        {
            this.ToggleDrawer();
        }

        private void ResetDrawer()
        {
            if (this.DrawerState != Primitives.DrawerState.Closed)
            {
                if (this.IsOpen)
                {
                    this.Context.MainContentStoryBoardReverse.Begin();
                    this.Context.MainContentStoryBoardReverse.Pause();
                    this.Context.DrawerStoryBoardReverse.Begin();
                    this.Context.DrawerStoryBoardReverse.Pause();

                    this.Context.MainContentStoryBoardReverse.Seek(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds));
                    this.Context.DrawerStoryBoardReverse.Seek(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds));

                    this.Context.DrawerStoryBoardReverse.Resume();
                    this.Context.MainContentStoryBoardReverse.Resume();
                }
                else
                {
                    this.Context.MainContentStoryBoard.Begin();
                    this.Context.MainContentStoryBoard.Pause();
                    this.Context.DrawerStoryBoard.Begin();
                    this.Context.DrawerStoryBoard.Pause();

                    this.Context.MainContentStoryBoard.Seek(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds));
                    this.Context.DrawerStoryBoard.Seek(TimeSpan.FromMilliseconds(this.AnimationDuration.TimeSpan.Milliseconds));

                    this.Context.DrawerStoryBoard.Resume();
                    this.Context.MainContentStoryBoard.Resume();
                }

                this.IsOpen = false;
                this.DrawerState = Primitives.DrawerState.Closed;
            }

            this.isAnimationUpdateScheduled = true;
            this.drawer.ClearValue(Grid.HeightProperty);
            this.drawer.ClearValue(Grid.WidthProperty);
            this.InvalidateMeasure();
        }
    }
}
