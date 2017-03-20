using System;
using System.Windows.Input;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives.HubTile;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Base class for all hub tiles.
    /// </summary>
    public abstract class HubTileBase : RadControl
    {
        /// <summary>
        /// Identifies the Title dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(nameof(Title), typeof(object), typeof(HubTileBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="TitleTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TitleTemplateProperty =
            DependencyProperty.Register(nameof(TitleTemplate), typeof(DataTemplate), typeof(HubTileBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the UpdateInterval dependency property.
        /// </summary>
        public static readonly DependencyProperty UpdateIntervalProperty =
            DependencyProperty.Register(nameof(UpdateInterval), typeof(TimeSpan), typeof(HubTileBase), new PropertyMetadata(TimeSpan.FromSeconds(3), OnUpdateIntervalChanged));

        /// <summary>
        /// Identifies the IsFrozen dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFrozenProperty =
           DependencyProperty.Register(nameof(IsFrozen), typeof(bool), typeof(HubTileBase), new PropertyMetadata(false, OnIsFrozenChanged));

        /// <summary>
        /// Identifies the BackContent dependency property.
        /// </summary>
        public static readonly DependencyProperty BackContentProperty =
            DependencyProperty.Register(nameof(BackContent), typeof(object), typeof(HubTileBase), new PropertyMetadata(null, OnBackContentChanged));

        /// <summary>
        /// Identifies the BackContentTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty BackContentTemplateProperty =
            DependencyProperty.Register(nameof(BackContentTemplate), typeof(DataTemplate), typeof(HubTileBase), new PropertyMetadata(null, OnBackContentChanged));

        /// <summary>
        /// Identifies the Command dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(HubTileBase), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the IsFlipped dependency property.
        /// </summary>
        public static readonly DependencyProperty IsFlippedProperty =
            DependencyProperty.Register(nameof(IsFlipped), typeof(bool), typeof(HubTileBase), new PropertyMetadata(false, OnIsFlippedChanged));

        /// <summary>
        /// Identifies the CommandParameter dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(HubTileBase), new PropertyMetadata(null));

        private DispatcherTimer updateTimer = new DispatcherTimer();
        private UIElement layoutRoot;
        private FlipControl flipControl;
        private TimeSpan updateIntervalChache;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "The tilt effect allowed types may not be initialized outside the constructor.")]
        static HubTileBase()
        {
            InteractionEffectManager.AllowedTypes.Add(typeof(HubTileBase));
        }

        /// <summary>
        /// Initializes a new instance of the HubTileBase class.
        /// </summary>
        protected HubTileBase()
        {
            this.DefaultStyleKey = typeof(HubTileBase);
            HubTileService.Tiles.Add(this);

            this.updateTimer.Tick += this.OnUpdateTimerTick;
            this.updateIntervalChache = (TimeSpan)this.GetValue(UpdateIntervalProperty);
            this.UpdateTimer.Interval = this.updateIntervalChache;
            
            this.SizeChanged += this.OnSizeChanged;

            InteractionEffectManager.SetIsInteractionEnabled(this, true);
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="HubTileBase"/> class. Cleans up any used resources when the object is destroyed.
        /// </summary>
        ~HubTileBase()
        {
            int index = HubTileService.Tiles.IndexOf(this);
            if (index != -1)
            {
                HubTileService.Tiles.RemoveAt(index);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the control is flipped.
        /// </summary>
        public bool IsFlipped
        {
            get
            {
                return (bool)this.GetValue(IsFlippedProperty);
            }
            set
            {
                this.SetValue(IsFlippedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a command that will be executed when the hub tile is tapped.
        /// </summary>
        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(HubTileBase.CommandProperty);
            }

            set
            {
                this.SetValue(HubTileBase.CommandProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command parameter that will be passed to the command assigned to the Command property.
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return this.GetValue(HubTileBase.CommandParameterProperty);
            }

            set
            {
                this.SetValue(HubTileBase.CommandParameterProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the Title.
        /// </summary>
        /// <remarks>The Title property works similar to the ContentControl.Content property. As it is of type <c>object</c>,
        /// there are no restrictions of its value - string, UI element, Panel or etc.</remarks>
        public object Title
        {
            get
            {
                return this.GetValue(HubTileBase.TitleProperty);
            }

            set
            {
                this.SetValue(HubTileBase.TitleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate that is used to visualize the <see cref="Title"/> property.
        /// </summary>
        /// <seealso cref="Title"/>
        public DataTemplate TitleTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(TitleTemplateProperty);
            }

            set
            {
                this.SetValue(TitleTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the hub tile is frozen. Freezing a hub tile means that it will cease to
        /// periodically update itself. For example when it is off-screen.
        /// </summary>
        public bool IsFrozen
        {
            get
            {
                return (bool)this.GetValue(HubTileBase.IsFrozenProperty);
            }

            set
            {
                this.SetValue(HubTileBase.IsFrozenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the UpdateInterval. This interval determines how often the tile will
        /// update its visual states when it is not frozen. The default value is <c>new TimeSpan(0, 0, 3)</c>
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadHubTile UpdateInterval="0:0:1"/&gt;
        /// </code>
        /// </example>
        public TimeSpan UpdateInterval
        {
            get
            {
                return this.updateIntervalChache;
            }

            set
            {
                this.SetValue(HubTileBase.UpdateIntervalProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the back content of the tile. When the back content is set,
        /// the tile flips with a swivel animation to its back side and periodically
        /// flips between its back and front states.
        /// </summary>
        public object BackContent
        {
            get
            {
                return this.GetValue(HubTileBase.BackContentProperty);
            }

            set
            {
                this.SetValue(HubTileBase.BackContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the DataTemplate that is used to visualize the BackContent
        /// property.
        /// </summary>
        public DataTemplate BackContentTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(HubTileBase.BackContentTemplateProperty);
            }

            set
            {
                this.SetValue(HubTileBase.BackContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the timer internally used to perform the control's transitions. Exposed for testing purposes.
        /// </summary>
        internal DispatcherTimer UpdateTimer
        {
            get
            {
                return this.updateTimer;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the update timer can be started - meaning the control is completely loaded in the visual tree and not frozen.
        /// </summary>
        internal bool CanStartTimer
        {
            get
            {
                if (!this.IsTemplateApplied || this.IsFrozen)
                {
                    return false;
                }

                if (!this.IsLoaded && !this.IsLoading)
                {
                    return false;
                }

                return true;
            }
        }

        /// <summary>
        /// Gets the LayoutRoot of the control template.
        /// </summary>
        protected internal UIElement LayoutRoot
        {
            get
            {
                return this.layoutRoot;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a rectangle clip is set on the LayoutRoot.
        /// </summary>
        protected virtual bool ShouldClip
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the update timer used to update the tile's VisualState needs to be started.
        /// </summary>
        protected virtual bool IsUpdateTimerNeeded
        {
            get
            {
                return this.BackContent != null || this.BackContentTemplate != null;
            }
        }

        /// <summary>
        /// Performs the Command execution logic. Exposed for testing purposes.
        /// </summary>
        internal void ExecuteCommand()
        {
            if (AutomationPeer.ListenerExists(AutomationEvents.InvokePatternOnInvoked))
            {
                AutomationPeer peer = FrameworkElementAutomationPeer.CreatePeerForElement(this);
                if (peer != null)
                    peer.RaiseAutomationEvent(AutomationEvents.InvokePatternOnInvoked);
            }

            if (this.Command == null)
            {
                return;
            }

            if (!this.Command.CanExecute(this.CommandParameter))
            {
                return;
            }

            this.Command.Execute(this.CommandParameter);
        }

        /// <summary>
        /// Calls the OnUpdateTimerTick method. Exposed for testing purposes.
        /// </summary>
        internal void CallTimerTick()
        {
            this.OnUpdateTimerTick(null, null);
        }

        /// <summary>
        /// A virtual callback that is called periodically when the tile is not frozen. It can be used to
        /// update the tile visual states or other necessary operations.
        /// </summary>
        protected internal virtual void Update(bool animate, bool updateIsFlipped)
        {
            if (updateIsFlipped)
            {
                this.UpdateIsFlipped(animate);
            }

            this.UpdateVisualState(animate);
        }

        /// <summary>
        /// Updates the value of the <see cref="IsFlipped"/> property.
        /// </summary>
        protected virtual void UpdateIsFlipped(bool animate)
        {
            if (this.flipControl != null)
            {
                this.flipControl.UpdateVisualState(animate);
            }
        }

        /// <summary>
        /// Resolves the ControlTemplate parts.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.layoutRoot = this.GetTemplatePartField<UIElement>("PART_LayoutRoot");
            applied = applied && this.layoutRoot != null;

            this.flipControl = this.GetTemplatePartField<FlipControl>("PART_FlipControl");
            applied = applied && this.flipControl != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="M:OnApplyTemplate"/> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.UpdateTimerState();
        }

        /// <summary>
        /// Called before the <see cref="E:Tapped"/> event occurs.
        /// </summary>
        protected override void OnTapped(TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            var peer = FrameworkElementAutomationPeer.FromElement(this);
            if (peer != null)
            {
                peer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            }
            this.ExecuteCommand();
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Loaded"/> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void LoadCore()
        {
            base.LoadCore();

            this.UpdateTimerState();
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Stops the update timer (if currently running).
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.UpdateTimerState();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new HubTileBaseAutomationPeer(this);
        }
        
        /// <summary>
        /// A virtual method that resets the timer.
        /// </summary>
        protected void ResetTimer()
        {
            this.updateTimer.Stop();
            this.UpdateTimerState();
        }

        /// <summary>
        /// A virtual callback that is called when the BackContent property changes.
        /// </summary>
        /// <param name="newBackContent">The new BackContent value.</param>
        /// <param name="oldBackContent">The old BackContent value.</param>
        protected virtual void OnBackContentChanged(object newBackContent, object oldBackContent)
        {
            if (newBackContent == null)
            {
                this.OnBackStateDeactivated();
            }
            else
            {
                this.OnBackStateActivated();
            }
        }

        /// <summary>
        /// This callback is invoked when BackContent is set to a non-null value.
        /// </summary>
        protected virtual void OnBackStateActivated()
        {
            this.ResetTimer();
            this.UpdateVisualState(this.IsLoaded);
        }

        /// <summary>
        /// This callback is invoked when BackContent is set to a null value.
        /// </summary>
        protected virtual void OnBackStateDeactivated()
        {
            this.ResetTimer();
            this.UpdateVisualState(this.IsLoaded);
        }

        /// <summary>
        /// This callback is invoked when <see cref="IsFlipped"/> is changed.
        /// </summary>
        protected virtual void OnIsFlippedChanged(bool newValue, bool oldValue)
        {
        }

        /// <summary>
        /// Checks whether the timer should be running or stopped and updates it accordingly.
        /// </summary>
        protected void UpdateTimerState()
        {
            if (!this.CanStartTimer || !this.IsUpdateTimerNeeded)
            {
                this.updateTimer.Stop();
            }
            else
            {
                this.updateTimer.Start();
            }
        }

        private static void OnIsFrozenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            HubTileBase hubTile = (HubTileBase)sender;
            hubTile.UpdateTimerState();

            var peer = FrameworkElementAutomationPeer.CreatePeerForElement(hubTile) as HubTileBaseAutomationPeer;
            if (peer != null)
            {
                peer.RaiseToggleStatePropertyChangedEvent((bool)args.OldValue, (bool)args.NewValue);
            }
        }

        private static void OnIsFlippedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            HubTileBase tile = sender as HubTileBase;
            tile.OnIsFlippedChanged((bool)args.NewValue, (bool)args.OldValue);
        }

        private static void OnUpdateIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            HubTileBase hubTile = sender as HubTileBase;
            var newInterval = (TimeSpan)args.NewValue;

            if (hubTile.updateIntervalChache != newInterval)
            {
                hubTile.updateIntervalChache = newInterval;
                hubTile.updateTimer.Interval = newInterval;
            }
        }

        private static void OnBackContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            HubTileBase tile = sender as HubTileBase;
            tile.OnBackContentChanged(args.NewValue, args.OldValue);
        }

        private void OnUpdateTimerTick(object sender, object args)
        {
            if (!this.IsLoaded || !this.IsTemplateApplied)
            {
                return;
            }

            this.Update(this.IsLoaded, true);
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!this.ShouldClip || !this.IsTemplateApplied)
            {
                return;
            }

            this.layoutRoot.Clip = new RectangleGeometry() { Rect = new Rect(new Point(), e.NewSize) };
        }
    }
}
