using System;
using System.Collections.ObjectModel;
using System.Linq;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using Telerik.UI.Xaml.Controls.Primitives.Menu.Commands;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    [ContentProperty(Name = "Items")]
    [Bindable]
    public partial class RadRadialMenu : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="ShowToolTip"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowToolTipProperty =
            DependencyProperty.Register(nameof(ShowToolTip), typeof(bool), typeof(RadRadialMenu), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="StartAngle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty StartAngleProperty =
            DependencyProperty.Register(nameof(StartAngle), typeof(double), typeof(RadRadialMenu), new PropertyMetadata(67.5d, OnStartAnglePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="InnerNavigationRadiusFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerNavigationRadiusFactorProperty =
            DependencyProperty.Register(nameof(InnerNavigationRadiusFactor), typeof(double), typeof(RadRadialMenu), new PropertyMetadata(0.85d, OnInnerNavigationRadiusFactorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="InnerRadiusFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty InnerRadiusFactorProperty =
            DependencyProperty.Register(nameof(InnerRadiusFactor), typeof(double), typeof(RadRadialMenu), new PropertyMetadata(0.2d, OnInnerRadiusFactorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="OuterRadiusFactor"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty OuterRadiusFactorProperty =
            DependencyProperty.Register(nameof(OuterRadiusFactor), typeof(double), typeof(RadRadialMenu), new PropertyMetadata(1d, OnOuterRadiusFactorPropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ContentMenuBackgroundStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentMenuBackgroundStyleProperty =
            DependencyProperty.Register(nameof(ContentMenuBackgroundStyle), typeof(Style), typeof(RadRadialMenu), new PropertyMetadata(null, OnContentMenuStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="ContentMenuBackgroundStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NavigationMenuBackgroundStyleProperty =
            DependencyProperty.Register(nameof(NavigationMenuBackgroundStyle), typeof(Style), typeof(RadRadialMenu), new PropertyMetadata(null, OnNavigationMenuStylePropertyChanged));

        /// <summary>
        /// Identifies the <see cref="IsOpen"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(nameof(IsOpen), typeof(bool), typeof(RadRadialMenu), new PropertyMetadata(false, OnIsOpenChanged));

        internal Style contentMenuBackgroundStyleCache;
        internal Style navigationMenuBackgroundStyleCache;
        internal double innerRadiusCache;
        internal double innerNavigationRadiusCache;
        internal double outerRadiusCache;
        internal double startAngleCache = 67.5d;
        internal RadialMenuModel model;
        internal HitTestService hitTestService;
        internal VisualStateService visualstateService;
        internal RadialMenuButton menuButton;

        internal Popup tooltip;
        internal MenuToolTip menuToolTipContent;

        private RadialPanel panel;
        private CommandService commandService;
        private FrameworkElement targetElement;

        private bool openRequested;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadRadialMenu"/> class.
        /// </summary>
        public RadRadialMenu()
        {
            this.DefaultStyleKey = typeof(RadRadialMenu);

            this.model = new RadialMenuModel(this);
            this.hitTestService = new HitTestService(this);
            this.visualstateService = new VisualStateService(this);
            this.commandService = new CommandService(this);
            this.SizeChanged += this.OnRadRadialMenuSizeChanged;
        }

        /// <summary>
        /// Occurs when menu item is selected/deselected.
        /// </summary>
        public event EventHandler<MenuSelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Gets or sets a value indicating whether a tool tip, displaying the current selected <see cref="RadialMenuItem"/> header text, will be displayed.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu ShowToolTip="True"/&gt;
        /// </code>
        /// </example>
        public bool ShowToolTip
        {
            get
            {
                return (bool)this.GetValue(ShowToolTipProperty);
            }
            set
            {
                this.SetValue(ShowToolTipProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RadRadialMenu"/> is open.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu IsOpen="True"/&gt;
        /// </code>
        /// </example>
        public bool IsOpen
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

        /// <summary>
        /// Gets or sets the start angle for ordering the <see cref="RadialMenuItem"/> components.
        /// </summary>
        /// <value>
        /// The angle is measured in radians.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu StartAngle="45"/&gt;
        /// </code>
        /// </example>
        public double StartAngle
        {
            get
            {
                return (double)this.GetValue(StartAngleProperty);
            }

            set
            {
                this.SetValue(StartAngleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the factor that defines the inner radius of the panel holding the <see cref="NavigationItemButton"/> items as a fraction of the size of the <see cref="RadRadialMenu"/> control. 
        /// </summary>
        /// <value>
        /// The value should be between 0 and 1. If the passed value lies outside this range, it is automatically set to the nearest boundary value.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu InnerNavigationRadiusFactor="0.3"/&gt;
        /// </code>
        /// </example>
        public double InnerNavigationRadiusFactor
        {
            get
            {
                return (double)this.GetValue(InnerNavigationRadiusFactorProperty);
            }

            set
            {
                this.SetValue(InnerNavigationRadiusFactorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the factor that defines the inner radius of the panel holding the <see cref="RadialMenuItemControl"/> items as a fraction of the size of the <see cref="RadRadialMenu"/> control.
        /// </summary>
        /// <value>
        /// The value should be between 0 and 1. If the passed value lies outside this range, it is automatically set to the nearest boundary value. 
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu InnerRadiusFactor="0.3"/&gt;
        /// </code>
        /// </example>
        public double InnerRadiusFactor
        {
            get
            {
                return (double)this.GetValue(InnerRadiusFactorProperty);
            }

            set
            {
                this.SetValue(InnerRadiusFactorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the factor that defines the outer radius of the panel holding the <see cref="NavigationItemButton"/> items as a fraction of the size of the <see cref="RadRadialMenu"/> control.
        /// </summary>
        /// <value>
        /// The value should be between 0 and 1. If the passed value lies outside this range, it is automatically set to the nearest boundary value.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu OuterRadiusFactor="0.9"/&gt;
        /// </code>
        /// </example>
        public double OuterRadiusFactor
        {
            get
            {
                return (double)this.GetValue(OuterRadiusFactorProperty);
            }
            set
            {
                this.SetValue(OuterRadiusFactorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> value that defines the appearance of the menu items panel.
        /// </summary>
        /// <remarks>
        /// The <see cref="Style"/> should target the <see cref="Windows.UI.Xaml.Shapes.Rectangle"/> type.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu&gt;
        ///     &lt;telerikPrimitives:RadRadialMenu.ContentMenuBackgroundStyle&gt;
        ///         &lt;Style TargetType="Rectangle"&gt;
        ///             &lt;Setter Property="Fill" Value="LightGreen"/&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikPrimitives:RadRadialMenu.ContentMenuBackgroundStyle&gt;
        /// &lt;/telerikPrimitives:RadRadialMenu&gt;
        /// </code>
        /// </example>
        public Style ContentMenuBackgroundStyle
        {
            get
            {
                return this.contentMenuBackgroundStyleCache;
            }
            set
            {
                this.SetValue(ContentMenuBackgroundStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Style"/> value that defines the appearance of the navigation buttons panel.
        /// </summary>
        /// <remarks>
        /// The <see cref="Style"/> should target the <see cref="Windows.UI.Xaml.Shapes.Rectangle"/> type.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu&gt;
        ///     &lt;telerikPrimitives:RadRadialMenu.NavigationMenuBackgroundStyle&gt;
        ///         &lt;Style TargetType="Rectangle"&gt;
        ///             &lt;Setter Property="Fill" Value="LightGreen"/&gt;
        ///         &lt;/Style&gt;
        ///     &lt;/telerikPrimitives:RadRadialMenu.NavigationMenuBackgroundStyle&gt;
        /// &lt;/telerikPrimitives:RadRadialMenu&gt;
        /// </code>
        /// </example>
        public Style NavigationMenuBackgroundStyle
        {
            get
            {
                return this.navigationMenuBackgroundStyleCache;
            }
            set
            {
                this.SetValue(NavigationMenuBackgroundStyleProperty, value);
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
        /// </summary>
        /// <remarks>
        /// Custom commands have higher priority than the built-in (default) ones.
        /// </remarks>
        public CommandCollection<RadRadialMenu> Commands
        {
            get
            {
                return this.CommandService.UserCommands;
            }
        }

        /// <summary>
        /// Gets the collection of all <see cref="RadialMenuItem"/>.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu x:Name="radialMenu"&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem Header="Item 1"/&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem Header="Item 2"/&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem Header="Item 3"/&gt;
        /// &lt;/telerikPrimitives:RadRadialMenu&gt;
        /// </code>
        /// <code language="c#">
        /// var items = this.radialMenu.Items;
        /// </code>
        /// </example>
        public ObservableCollection<RadialMenuItem> Items
        {
            get
            {
                return this.model.MenuItems;
            }
        }

        /// <summary>
        /// Gets the target <see cref="FrameworkElement"/> instance that <see cref="RadRadialMenu"/> is assigned to.
        /// </summary>
        /// <remarks>
        /// Before the menu is attached to the element via the <see cref="RadialMenuTriggerBehavior"/> this property has <c>null</c> value.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;TextBlock Text="Some Text"&gt;
        ///     &lt;telerikPrimitives:RadRadialContextMenu.Menu&gt;
        ///         &lt;telerikPrimitives:RadRadialMenu x:Name="radialMenu"/&gt;
        ///     &lt;/telerikPrimitives:RadRadialContextMenu.Menu&gt;
        ///     &lt;telerikPrimitives:RadRadialContextMenu.Behavior&gt;
        ///         &lt;telerikPrimitives:RadialMenuTriggerBehavior AttachTriggers="PointerOver"/&gt;
        ///     &lt;/telerikPrimitives:RadRadialContextMenu.Behavior&gt;
        /// &lt;/TextBlock&gt;
        /// </code>
        /// <para>After you perform the action that will attach the menu to the target element, you can use the <see cref="RadRadialMenu.TargetElement"/> property:</para>
        /// <code language="c#">
        /// var textBlock = this.radialMenu.TargetElement;
        /// </code>
        /// </example>
        public FrameworkElement TargetElement
        {
            get
            {
                return this.targetElement;
            }

            internal set
            {
                if (this.targetElement != value)
                {
                    this.model.actionService.ForceCompletion();

                    if (this.targetElement != null)
                    {
                        this.targetElement.Unloaded -= this.TargetElementUnloaded;
                    }

                    this.targetElement = value;

                    if (this.targetElement != null)
                    {
                        this.targetElement.Unloaded += this.TargetElementUnloaded;
                    }

                    this.model.InvalidateRequerySuggested(this.Items);
                }
            }
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal static double GetNextLevelStartAngle(double currentStartAngle, int childrenCount)
        {
            int itemsOffset = childrenCount / 2;

            if (itemsOffset > 0 && childrenCount % 2 == 0)
            {
                itemsOffset--;
            }

            if (currentStartAngle - itemsOffset * 45 < 0)
            {
                return (currentStartAngle + 360) - itemsOffset * 45;
            }
            else
            {
                return currentStartAngle - itemsOffset * 45;
            }
        }

        /// <summary>
        /// Opens the current instance of the <see cref="RadRadialMenu"/>.
        /// </summary>
        internal void Open()
        {
            if (this.TargetElement != null)
            {
                var behavior = RadRadialContextMenu.GetBehavior(this.TargetElement);

                if (behavior != null)
                {
                    behavior.AttachToTargetElement();
                }
            }

            this.model.ShowView();

            if (this.TargetElement != null)
            {
                PopupService.DisplayOverlay();
            }

            this.CalculateRingsAspectRatio();

            if (this.model.viewState.StartAngleLevels.Count > 0)
            {
                this.model.Layout.StartAngle = this.model.viewState.StartAngleLevels[0];
                this.model.UpdateRingsRadius();
            }
        }

        /// <summary>
        /// Closes the current instance of the <see cref="RadRadialMenu"/>.
        /// </summary>
        internal void Close()
        {
            this.model.HideView();

            if (this != null)
            {
                PopupService.HideOverlay();
            }

            this.menuToolTipContent.HideToolTip();
            this.model.ResetViewState();
        }

        internal void InitializeLayer(LayerBase layer)
        {
            layer.AttachToPanel(this.panel);
            layer.Owner = this;
        }

        internal void DestroyLayer(LayerBase layer)
        {
            layer.DetachFromPanel(this.panel);
            layer.Owner = null;
        }

        internal Point GetPositionPoint()
        {
            // TODO: raise: position command
            var position = this.TargetElement.TransformToVisual(null).TransformPoint(new Point(this.TargetElement.ActualWidth, 0));

            this.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
            var adjustedPosition = new Point(position.X, position.Y - this.DesiredSize.Height / 2);

            var windowBounds = Window.Current.Bounds;

            var x = Math.Min(adjustedPosition.X + this.DesiredSize.Width, windowBounds.Width) - this.DesiredSize.Width;

            x = Math.Max(adjustedPosition.X, 0);

            var y = Math.Min(adjustedPosition.Y + this.DesiredSize.Height, windowBounds.Height) - this.DesiredSize.Height;

            y = Math.Max(adjustedPosition.Y, 0);

            var calculatedPosition = new Point(x, y);

            return calculatedPosition;
        }

        /// <summary>
        /// Exposed for testing purposes.
        /// </summary>
        internal void HandleOnBackButtonPressed()
        {
            var menuLevels = this.model.viewState.MenuLevels;

            if (menuLevels.Count > 0)
            {
                var sourceItem = menuLevels.Pop();
                var targetLevel = menuLevels.FirstOrDefault();
                var startAngleLevels = this.model.viewState.StartAngleLevels;

                startAngleLevels.RemoveAt(startAngleLevels.Count - 1);
                var startAngle = startAngleLevels.LastOrDefault();

                this.RaiseNavigateCommand(targetLevel, sourceItem, startAngle, true);

                if (targetLevel == null)
                {
                    this.menuButton.TransformToNormal();
                }
            }
        }

        /// <summary>
        /// Called when selection is changed .
        /// </summary>
        /// <param name="item">The changed menu item.</param>
        protected internal virtual void OnSelectionChanged(RadialMenuItem item)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, new MenuSelectionChangedEventArgs { Item = item });
            }
        }

        /// <summary>
        /// Called when the Framework <see cref="M:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            this.panel = this.GetTemplatePartField<RadialPanel>("PART_Panel");
            this.menuButton = this.GetTemplatePartField<RadialMenuButton>("PART_Button");
            this.menuButton.Click += this.OnMainButtonPressed;

            return base.ApplyTemplateCore() && this.panel != null && this.menuButton != null;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.CalculateRingsAspectRatio();

            this.model.InitializeLayers();

            this.model.UpdateItemsSelection(this.Items);

            if (this.IsOpen)
            {
                this.openRequested = true;
            }

            this.tooltip = new Popup();
            this.menuToolTipContent = new MenuToolTip();
            this.tooltip.Child = this.menuToolTipContent;

            this.panel.Children.Add(this.tooltip);
            this.menuToolTipContent.Transitions = new TransitionCollection();

            this.menuToolTipContent.Transitions.Add(new PopupThemeTransition() { FromHorizontalOffset = 0, FromVerticalOffset = 0 });

            this.menuToolTipContent.Owner = this;
        }

        /// <summary>
        /// Called before the PointerMoved event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerMoved(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerMoved(e);

            if (e != null)
            {
                this.OnPointerMoved(e.GetCurrentPoint(this).Position);
            }
        }

        /// <summary>
        /// Called before the PointerExited event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnPointerExited(Windows.UI.Xaml.Input.PointerRoutedEventArgs e)
        {
            base.OnPointerExited(e);

            if (e != null)
            {
                this.visualstateService.UpdateItemHoverState(null);
            }
        }

        /// <summary>
        /// Called before the Tapped event occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (e != null)
            {
                this.OnPointerTapped(e.GetPosition(this));
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadRadialMenuAutomationPeer(this);
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menu = d as RadRadialMenu;

            if (e.OldValue == e.NewValue || menu.IsInternalPropertyChange || !menu.IsTemplateApplied)
            {
                return;
            }

            if (menu.IsOpen)
            {
                menu.Open();
            }
            else
            {
                if (menu.menuButton != null)
                {
                    menu.menuButton.TransformToNormal();
                }

                menu.Close();
            }

            RadRadialMenuAutomationPeer peer = FrameworkElementAutomationPeer.FromElement(menu) as RadRadialMenuAutomationPeer;
            if (peer != null)
            {
                peer.RaiseExpandCollapseAutomationEvent(!((bool)e.NewValue), (bool)e.NewValue);
            }
        }

        private static void OnInnerNavigationRadiusFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as RadRadialMenu;

            if (sender != null)
            {
                double radius = (double)e.NewValue;
                double clampedRadius = radius;

                if (clampedRadius < 0)
                {
                    clampedRadius = 0;
                }
                else if (clampedRadius > 1)
                {
                    clampedRadius = 1;
                }

                if (clampedRadius > sender.OuterRadiusFactor)
                {
                    clampedRadius = sender.OuterRadiusFactor;
                }

                if (clampedRadius < sender.InnerRadiusFactor)
                {
                    clampedRadius = sender.InnerRadiusFactor;
                }

                if (clampedRadius != radius)
                {
                    sender.SetValue(InnerNavigationRadiusFactorProperty, clampedRadius);
                    return;
                }

                sender.CalculateRingsAspectRatio();
            }
        }

        private static void OnInnerRadiusFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as RadRadialMenu;

            if (sender != null)
            {
                double radius = (double)e.NewValue;
                double clampedRadius = radius;

                if (clampedRadius < 0)
                {
                    clampedRadius = 0;
                }
                else if (clampedRadius > 1)
                {
                    clampedRadius = 1;
                }

                if (clampedRadius > sender.InnerNavigationRadiusFactor)
                {
                    clampedRadius = sender.InnerNavigationRadiusFactor;
                }

                if (clampedRadius != radius)
                {
                    sender.SetValue(InnerRadiusFactorProperty, clampedRadius);
                    return;
                }

                sender.CalculateRingsAspectRatio();
            }
        }

        private static void OnOuterRadiusFactorPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as RadRadialMenu;

            if (sender != null)
            {
                double radius = (double)e.NewValue;
                double clampedRadius = radius;

                if (clampedRadius < 0)
                {
                    clampedRadius = 0;
                }
                else if (clampedRadius > 1)
                {
                    clampedRadius = 1;
                }

                if (clampedRadius < sender.InnerNavigationRadiusFactor)
                {
                    clampedRadius = sender.InnerNavigationRadiusFactor;
                }

                if (clampedRadius != radius)
                {
                    sender.SetValue(OuterRadiusFactorProperty, clampedRadius);
                    return;
                }

                sender.CalculateRingsAspectRatio();
            }
        }

        private static void OnContentMenuStylePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadRadialMenu radialMenu = (RadRadialMenu)target;
            radialMenu.contentMenuBackgroundStyleCache = args.NewValue as Style;

            if (radialMenu.IsTemplateApplied)
            {
                radialMenu.model.UpdateRingsVisualPanel();
            }
        }

        private static void OnNavigationMenuStylePropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadRadialMenu radialMenu = (RadRadialMenu)target;
            radialMenu.navigationMenuBackgroundStyleCache = args.NewValue as Style;

            if (radialMenu.IsTemplateApplied)
            {
                radialMenu.model.UpdateRingsVisualPanel();
            }
        }

        private static void OnStartAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var sender = d as RadRadialMenu;

            if (sender != null)
            {
                double angle = (double)e.NewValue;
                double clampedAngle = angle;

                if (clampedAngle < 0)
                {
                    clampedAngle = 0;
                }
                else if (clampedAngle > 360)
                {
                    clampedAngle = 360;
                }

                if (clampedAngle != angle)
                {
                    sender.SetValue(StartAngleProperty, clampedAngle);
                    return;
                }

                // TODO : Calculate radiuse by factor.
                var segmentLaylout = sender.model.Layout as FixedSegmentLayout;
                sender.startAngleCache = angle;
                sender.model.UpdateStartAngle(angle);

                if (segmentLaylout != null)
                {
                    segmentLaylout.StartAngle = angle;
                    sender.model.UpdateRingsRadius();
                }
            }
        }

        private void TargetElementUnloaded(object sender, RoutedEventArgs e)
        {
            this.IsOpen = false;
            PopupService.Detach();
        }

        private void CalculateRingsAspectRatio()
        {
            if (!this.IsTemplateApplied)
            {
                return;
            }

            double maxRadius = Math.Min(this.ActualWidth, this.ActualHeight);
            double minRadius = Math.Max(this.menuButton.ActualWidth, this.menuButton.ActualHeight);

            this.outerRadiusCache = Math.Max(minRadius, (maxRadius * this.OuterRadiusFactor) / 2);
            this.innerNavigationRadiusCache = Math.Max(minRadius, (maxRadius * this.InnerNavigationRadiusFactor) / 2);
            this.innerRadiusCache = Math.Max(minRadius / 2, (maxRadius * this.InnerRadiusFactor) / 2);

            this.model.UpdateRingsRadius();
        }

        private void OnMainButtonPressed(object sender, RoutedEventArgs e)
        {
            if (!this.IsOpen)
            {
                this.IsOpen = this.CommandService.ExecuteCommand(CommandId.Open, null);
            }
            else
            {
                if (!this.menuButton.DisplayBackContent)
                {
                    this.IsOpen = !this.CommandService.ExecuteCommand(CommandId.Close, null);
                }
                else
                {
                    this.HandleOnBackButtonPressed();
                }
            }
        }

        private void OnRadRadialMenuSizeChanged(object sender, SizeChangedEventArgs e)
        {
            this.CalculateRingsAspectRatio();

            if (this.openRequested)
            {
                this.openRequested = false;

                this.Open();
            }
        }
    }
}