using System;
using System.Linq;
using Telerik.UI.Automation.Peers;
using Windows.ApplicationModel;
using Windows.Foundation;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents a content control that consists of a main content presenter and
    /// an expandable content presenter that can be collapsed/expanded by the end users.
    /// </summary>
    public class RadExpanderControl : RadContentControl
    {
        /// <summary>
        /// Identifies the <see cref="AnimatedIndicatorContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimatedIndicatorContentProperty =
            DependencyProperty.Register(nameof(AnimatedIndicatorContent), typeof(object), typeof(RadExpanderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="AnimatedIndicatorContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimatedIndicatorContentTemplateProperty =
            DependencyProperty.Register(nameof(AnimatedIndicatorContentTemplate), typeof(DataTemplate), typeof(RadExpanderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="AnimatedIndicatorStyle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AnimatedIndicatorStyleProperty =
            DependencyProperty.Register(nameof(AnimatedIndicatorStyle), typeof(Style), typeof(RadExpanderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ExpandedStateContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandedStateContentProperty =
            DependencyProperty.Register(nameof(ExpandedStateContent), typeof(object), typeof(RadExpanderControl), new PropertyMetadata(null, OnExpandedStateContentChanged));

        /// <summary>
        /// Identifies the <see cref="ExpandedStateContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandedStateContentTemplateProperty =
            DependencyProperty.Register(nameof(ExpandedStateContentTemplate), typeof(DataTemplate), typeof(RadExpanderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ExpandableContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandableContentProperty =
            DependencyProperty.Register(nameof(ExpandableContent), typeof(object), typeof(RadExpanderControl), new PropertyMetadata(null, OnExpandableContentChanged));

        /// <summary>
        /// Identifies the <see cref="ExpandableContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ExpandableContentTemplateProperty =
            DependencyProperty.Register(nameof(ExpandableContentTemplate), typeof(DataTemplate), typeof(RadExpanderControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsExpanded"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandedProperty =
            DependencyProperty.Register(nameof(IsExpanded), typeof(bool), typeof(RadExpanderControl), new PropertyMetadata(false, OnIsExpandedChanged));

        /// <summary>
        /// Identifies the <see cref="IsExpandable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsExpandableProperty =
            DependencyProperty.Register(nameof(IsExpandable), typeof(bool), typeof(RadExpanderControl), new PropertyMetadata(true, OnIsExpandableChanged));

        /// <summary>
        /// Identifies the <see cref="HideIndicatorWhenNotExpandable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HideIndicatorWhenNotExpandableProperty =
            DependencyProperty.Register(nameof(HideIndicatorWhenNotExpandable), typeof(bool), typeof(RadExpanderControl), new PropertyMetadata(false, OnHideIndicatorWhenNotExpandableChanged));
        
        /// <summary>
        /// Identifies the <see cref="HeaderBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderBackgroundProperty =
            DependencyProperty.Register(nameof(HeaderBackground), typeof(Brush), typeof(RadExpanderControl), new PropertyMetadata(null));
        
        internal ContentPresenter mainContentPresenter;
        
        private static readonly DependencyProperty DataContextPrivateProperty =
            DependencyProperty.Register("DataContextPrivate", typeof(object), typeof(RadExpanderControl), new PropertyMetadata(null, OnDataContextPrivateChanged));

        private bool useTransitions = false;
        private bool isPropertySetSilently = false;
        private bool contextBinding = false;
        private Canvas contentHolder;
        private Panel expanderHeaderLayoutRoot;
        private ContentPresenter expandableContent;
        private ContentPresenter animatedIndicator;
        private DoubleAnimation expandAnimation;
        private DoubleAnimation expandContentHolderAnimation;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadExpanderControl"/> class.
        /// </summary>
        public RadExpanderControl()
        {
            this.UseOptimizedMeasure = false;
            this.DefaultStyleKey = typeof(RadExpanderControl);
        }

        /// <summary>
        /// Fires when the <see cref="IsExpanded"/> property is about to change.
        /// </summary>
        public event EventHandler<ExpandedStateChangingEventArgs> ExpandedStateChanging;

        /// <summary>
        /// Fires when the <see cref="IsExpanded"/> property is has changed.
        /// </summary>
        public event EventHandler<ExpandedStateChangedEventArgs> ExpandedStateChanged;

        /// <summary>
        /// Gets or sets a value indicating whether the expandable content is measured
        /// only when the control is being expanded or at the initial layout pass for the whole control. When
        /// an optimized measure pass is used, some delays in the expansion may occur depending on the complexity of 
        /// the expandable content's template.
        /// </summary>
        public bool UseOptimizedMeasure
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the expandable content is visible or not.
        /// </summary>
        public bool IsExpanded
        {
            get
            {
                return (bool)this.GetValue(IsExpandedProperty);
            }
            set
            {
                this.SetValue(IsExpandedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the user can expand/collapse the control through the UI.
        /// </summary>
        public bool IsExpandable
        {
            get { return (bool)GetValue(IsExpandableProperty); }
            set { this.SetValue(IsExpandableProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the animated indicator(arrow) will be hidden when the control is not expandable.
        /// </summary>
        public bool HideIndicatorWhenNotExpandable
        {
            get { return (bool)GetValue(HideIndicatorWhenNotExpandableProperty); }
            set { this.SetValue(HideIndicatorWhenNotExpandableProperty, value); }
        }
        
        /// <summary>
        /// Gets or sets the Background Brush of the Header(Content and Indicator).
        /// </summary>
        public Brush HeaderBackground
        {
            get { return (Brush)GetValue(HeaderBackgroundProperty); }
            set { this.SetValue(HeaderBackgroundProperty, value); }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="Style"/> class
        /// used to define the visual appearance of the animated indicator.
        /// </summary>
        public Style AnimatedIndicatorStyle
        {
            get
            {
                return this.GetValue(AnimatedIndicatorStyleProperty) as Style;
            }
            set
            {
                this.SetValue(AnimatedIndicatorStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content of the animated indicator.
        /// </summary>
        /// <value>The content of the animated indicator.</value>
        public object AnimatedIndicatorContent
        {
            get
            {
                return this.GetValue(AnimatedIndicatorContentProperty);
            }
            set
            {
                this.SetValue(AnimatedIndicatorContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="DataTemplate"/> class that defines the visual appearance
        /// of the content defined by the <see cref="AnimatedIndicatorContent"/> property..
        /// </summary>
        /// <value>The content template of the animated indicator.</value>
        public DataTemplate AnimatedIndicatorContentTemplate
        {
            get
            {
                return this.GetValue(AnimatedIndicatorContentTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(AnimatedIndicatorContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content that will be displayed in the <see cref="RadExpanderControl"/>'s
        /// header when it is in its expanded state.
        /// </summary>
        public object ExpandedStateContent
        {
            get
            {
                return this.GetValue(ExpandedStateContentProperty);
            }
            set
            {
                this.SetValue(ExpandedStateContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="DataTemplate"/> class that defines the visual appearance
        /// of the object set to the <see cref="ExpandedStateContent"/> property.
        /// </summary>
        public DataTemplate ExpandedStateContentTemplate
        {
            get
            {
                return this.GetValue(ExpandedStateContentTemplateProperty) as DataTemplate;
            }
            set
            {
                this.SetValue(ExpandedStateContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the additional content for <see cref="RadExpanderControl"/> that can be
        /// expanded/collapsed.
        /// </summary>
        public object ExpandableContent
        {
            get
            {
                return this.GetValue(ExpandableContentProperty);
            }
            set
            {
                this.SetValue(ExpandableContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the data template for the additional content 
        /// for <see cref="RadExpanderControl"/> that can be
        /// expanded/collapsed.
        /// </summary>
        public object ExpandableContentTemplate
        {
            get
            {
                return this.GetValue(ExpandableContentTemplateProperty);
            }
            set
            {
                this.SetValue(ExpandableContentTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets a boolean value that indicates whether the control template parts
        /// have been successfully acquired after the OnApplyTemplate call.
        /// </summary>
        protected internal override bool IsProperlyTemplated
        {
            get
            {
                return this.expandableContent != null && this.contentHolder != null && this.expandAnimation != null && this.expandContentHolderAnimation != null && this.animatedIndicator != null;
            }
        }

        internal virtual bool ShouldToggleExpandOnTap(Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            if (ElementTreeHelper.IsElementRendered(this.expanderHeaderLayoutRoot))
            {
                Point position = e.GetPosition(this.expanderHeaderLayoutRoot);

                var rect = new Rect(new Point(), this.expanderHeaderLayoutRoot.RenderSize);

                return rect.Contains(position);
            }

            return false;
        }

#pragma warning disable 4014
        /// <summary>
        /// When overridden in a derived class, is invoked whenever application code or internal processes (such as a rebuilding layout pass) call <see cref="M:System.Windows.Controls.Control.ApplyTemplate" />. In simplest terms, this means the method is called just before a UI element displays in an application. For more information, see Remarks.
        /// </summary>
        protected override void OnApplyTemplate()
        {
            // For some reason GetTemplateChild does not find animation declared in a VisualState.
            var layoutRoot = this.GetTemplateChild("PART_LayoutRoot") as Border;
            this.contentHolder = this.GetTemplateChild("PART_ExpandableContentHolder") as Canvas;
            var expandedVisualState = VisualStateManager.GetVisualStateGroups(layoutRoot)[0].States.First(state => state.Name == "Expanded");
            if (expandedVisualState != null && expandedVisualState.Storyboard != null)
            {
                var animation = new DoubleAnimation();
                animation.Duration = new Duration(TimeSpan.FromSeconds(0));
                animation.EnableDependentAnimation = true;
                Storyboard.SetTarget(animation, this.contentHolder);
                animation.SetValue(Storyboard.TargetPropertyProperty, "Height");
                this.expandContentHolderAnimation = animation;

                expandedVisualState.Storyboard.Children.Add(this.expandContentHolderAnimation);
            }

            base.OnApplyTemplate();

            this.expanderHeaderLayoutRoot = this.GetTemplateChild("PART_ExpanderHeaderLayoutRoot") as Panel;
            this.mainContentPresenter = this.GetTemplateChild("PART_MainContentPresenter") as ContentPresenter;
            this.expandableContent = this.GetTemplateChild("PART_ExpandableContentPresenter") as ContentPresenter;
            this.expandableContent.SizeChanged += this.OnExpandableContentPresenter_SizeChanged;
            this.contentHolder.SizeChanged += this.OnContentHolder_SizeChanged;
            this.expandAnimation = this.GetTemplateChild("PART_ExpandAnimation") as DoubleAnimation;
            this.animatedIndicator = this.GetTemplateChild("PART_AnimatedIndicator") as ContentPresenter;

            Binding b = new Binding();
            b.Source = this;
            b.Path = new PropertyPath("DataContext");
            this.contextBinding = true;
            this.SetBinding(DataContextPrivateProperty, b);
            this.contextBinding = false;

            if (this.IsProperlyTemplated)
            {
                if (!this.IsExpandable && this.HideIndicatorWhenNotExpandable)
                {
                    this.animatedIndicator.Visibility = Visibility.Collapsed;
                }
                else
                {
                    this.animatedIndicator.Visibility = Visibility.Visible;
                }

                if (DesignMode.DesignModeEnabled)
                {
                    this.SetInitialControlState(false);
                }
                else
                {
                    this.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal,
                        () =>
                        {
                            this.SetInitialControlState(false);
                        });
                }
            }
        }
#pragma warning restore 4014

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadExpanderControlAutomationPeer(this);
        }

        /// <summary>
        /// Called before the <see cref="E:System.Windows.UIElement.Tap" /> event
        /// occurs.
        /// </summary>
        /// <param name="e">Event data for the event.</param>
        protected override void OnTapped(Windows.UI.Xaml.Input.TappedRoutedEventArgs e)
        {
            base.OnTapped(e);

            if (e.Handled)
            {
                return;
            }

            if (this.ShouldToggleExpandOnTap(e))
            {
                this.useTransitions = true;
                this.ToggleExpandedOnTap();
                this.useTransitions = false;
            }
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            if (e.OriginalSource is RadExpanderControl)
            {
                e.Handled = this.HandleKeyDown(e.Key);
            }
           
            base.OnKeyDown(e);
        }
        
        /// <summary>
        /// Builds the current visual state for this instance.
        /// </summary>
        protected override string ComposeVisualStateName()
        {
            if (this.IsExpanded)
            {
                return "Expanded";
            }

            return "Collapsed";
        }

        /// <summary>
        /// Provides the behavior for the Measure pass of Windows Phone layout. Classes can override this method to define their own Measure pass behavior.
        /// </summary>
        /// <param name="availableSize">The available size that this object can give to child objects. Infinity (<see cref="F:System.Double.PositiveInfinity" />) can be specified as a value to indicate that the object will size to whatever content is available.</param>
        /// <returns>
        /// The size that this object determines it needs during layout, based on its calculations of the allocated sizes for child objects; or based on other considerations, such as a fixed container size.
        /// </returns>
        protected override Size MeasureOverride(Size availableSize)
        {
            if (!this.UseOptimizedMeasure || this.IsExpanded)
            {
                this.expandableContent.Measure(new Size(availableSize.Width, double.PositiveInfinity));
            }

            Size measuredResult = base.MeasureOverride(availableSize);

            return measuredResult;
        }

#pragma warning disable 4014
        /// <summary>
        /// Provides the behavior for the Arrange pass of Silverlight layout. Classes
        /// can override this method to define their own Arrange pass behavior.
        /// </summary>
        /// <param name="finalSize">The final area within the parent that this object should
        /// use to arrange itself and its children.</param>
        /// <returns>
        /// The actual size that is used after the element is arranged in layout.
        /// </returns>
        protected override Size ArrangeOverride(Size finalSize)
        {
            Size result = base.ArrangeOverride(finalSize);

            if (!this.UseOptimizedMeasure || this.IsExpanded)
            {
                Action action = () =>
                {
                    if (!this.UseOptimizedMeasure || this.IsExpanded)
                    {
                        this.contentHolder.Width = result.Width;
                        this.expandableContent.Width = result.Width;
                    }
                };

                if (DesignMode.DesignModeEnabled)
                {
                    action();
                }
                else
                {
                    this.Dispatcher.RunAsync(
                        CoreDispatcherPriority.Normal, 
                        () =>
                        {
                            action();
                        });
                }
            }

            return result;
        }
#pragma warning restore 4014

        /// <summary>
        /// Called when the value of the <see cref="P:System.Windows.Controls.ContentControl.Content" /> property changes.
        /// </summary>
        /// <param name="oldContent">The old value of the <see cref="P:System.Windows.Controls.ContentControl.Content" /> property.</param>
        /// <param name="newContent">The new value of the <see cref="P:System.Windows.Controls.ContentControl.Content" /> property.</param>
        protected override void OnContentChanged(object oldContent, object newContent)
        {
            base.OnContentChanged(oldContent, newContent);
            if (this.IsProperlyTemplated)
            {
                this.SetInitialControlState(false);
            }
        }

        private static void OnDataContextPrivateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadExpanderControl typedSender = sender as RadExpanderControl;

            if (!typedSender.IsProperlyTemplated || typedSender.contextBinding)
            {
                return;
            }
            typedSender.expandableContent.Visibility = Visibility.Collapsed;
            typedSender.SetInitialControlState(false);
        }

        private static void OnExpandedStateContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadExpanderControl typedSender = sender as RadExpanderControl;

            if (!typedSender.IsProperlyTemplated)
            {
                return;
            }
            typedSender.expandableContent.Visibility = Visibility.Collapsed;
            typedSender.SetInitialControlState(false);
        }

        private static void OnExpandableContentChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadExpanderControl typedSender = sender as RadExpanderControl;

            if (!typedSender.IsTemplateApplied)
            {
                return;
            }
            typedSender.expandableContent.Visibility = Visibility.Collapsed;
            typedSender.SetInitialControlState(false);
        }

        private static void OnIsExpandedChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadExpanderControl typedSender = sender as RadExpanderControl;

            if (!typedSender.isPropertySetSilently)
            {
                if (!typedSender.HandleExpandedChange(typedSender.IsExpanded))
                {
                    typedSender.isPropertySetSilently = true;
                    typedSender.IsExpanded = !typedSender.IsExpanded;
                    typedSender.isPropertySetSilently = false;
                }
            }

            var expanderControlPeer = FrameworkElementAutomationPeer.FromElement(typedSender) as RadExpanderControlAutomationPeer;
            if (expanderControlPeer != null)
            {
                expanderControlPeer.RaiseExpandCollapseAutomationEvent((bool)args.OldValue, (bool)args.NewValue);
                expanderControlPeer.RaiseAutomationEvent(AutomationEvents.AutomationFocusChanged);
            }
        }

        private static void OnIsExpandableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadExpanderControl;
            bool isExpandable = (bool)e.NewValue;

            if (control != null && control.IsTemplateApplied)
            {
                if (!isExpandable && control.HideIndicatorWhenNotExpandable)
                {
                    control.animatedIndicator.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.animatedIndicator.Visibility = Visibility.Visible;
                }
            }
        }

        private static void OnHideIndicatorWhenNotExpandableChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = d as RadExpanderControl;
            bool shouldHideIndicator = (bool)e.NewValue;

            if (control != null && control.IsTemplateApplied)
            {
                if (shouldHideIndicator && !control.IsExpandable)
                {
                    control.animatedIndicator.Visibility = Visibility.Collapsed;
                }
                else
                {
                    control.animatedIndicator.Visibility = Visibility.Visible;
                }
            }
        }

        private void OnExpandableContentPresenter_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.expandContentHolderAnimation != null)
            {
                this.expandContentHolderAnimation.To = e.NewSize.Height;
            }
           
            if (e.PreviousSize.Height != e.NewSize.Height)
            {
                if (this.IsExpanded)
                {
                    this.contentHolder.Height = e.NewSize.Height;
                }
            }
        }

        private void OnContentHolder_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RectangleGeometry rectangle = new RectangleGeometry();
            rectangle.Rect = new Rect(new Point(), e.NewSize);
            this.contentHolder.Clip = rectangle;
        }

        private void ToggleExpandedOnTap()
        {
            if (this.IsExpandable)
            {
                this.IsExpanded = !this.IsExpanded;
            }
        }

        private bool HandleExpandedChange(bool isExpanded)
        {
            ExpandedStateChangingEventArgs changingArgs = new ExpandedStateChangingEventArgs(isExpanded);
            EventHandler<ExpandedStateChangingEventArgs> changingHandler = this.ExpandedStateChanging;
            if (changingHandler != null)
            {
                changingHandler(this, changingArgs);
            }

            if (changingArgs.Cancel)
            {
                return false;
            }

            ExpandedStateChangedEventArgs changedArgs = new ExpandedStateChangedEventArgs(isExpanded);
            EventHandler<ExpandedStateChangedEventArgs> changedHandler = this.ExpandedStateChanged;

            if (changedHandler != null)
            {
                changedHandler(this, changedArgs);
            }

            if (this.IsTemplateApplied)
            {
                this.SetInitialControlState(this.useTransitions);
            }

            return true;
        }

        private void SetInitialControlState(bool useTransitions)
        {
            if (this.IsExpanded)
            {
                if (this.ExpandedStateContent != null)
                {
                    this.mainContentPresenter.Content = this.ExpandedStateContent;
                }
                else
                {
                    this.mainContentPresenter.Content = this.Content;
                }

                if (this.ExpandedStateContentTemplate != null)
                {
                    this.mainContentPresenter.ContentTemplate = this.ExpandedStateContentTemplate;
                }
                else
                {
                    this.mainContentPresenter.ContentTemplate = this.ContentTemplate;
                }
            }
            else
            {
                this.mainContentPresenter.Content = this.Content;
                this.mainContentPresenter.ContentTemplate = this.ContentTemplate;
            }

            this.UpdateElementsVisibility();

            this.UpdateVisualState(useTransitions);
        }

        private void UpdateElementsVisibility()
        {
            if (this.expandableContent.Visibility == Visibility.Collapsed)
            {
                this.expandableContent.Visibility = Visibility.Visible;
            }

            if (this.IsExpanded)
            {
                this.expandableContent.InvalidateMeasure();
                this.expandableContent.InvalidateArrange();
                this.expandableContent.UpdateLayout();
                this.expandAnimation.To = this.expandableContent.ActualHeight;
                if (this.expandContentHolderAnimation != null)
                {
                    this.expandContentHolderAnimation.To = this.expandableContent.ActualHeight;
                }
            }
        }

        private bool HandleKeyDown(VirtualKey key)
        {
            if (key == VirtualKey.Enter || key == VirtualKey.Space)
            {
                this.ToggleExpandedOnTap();
                return true;
            }

            return false;
        }
    }
}
