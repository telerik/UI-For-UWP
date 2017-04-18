using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Primitives;
using Telerik.UI.Xaml.Controls.Primitives.Menu;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Defines an abstraction of a menu item that is used to visualize command item along with its children within a <see cref="RadRadialMenu"/> component.
    /// </summary>
    [ContentProperty(Name = "ChildItems")]
    public class RadialMenuItem : RadDependencyObject
    {
        /// <summary>
        /// Identifies the <see cref="ToolTipContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ToolTipContentProperty =
            DependencyProperty.Register(nameof(ToolTipContent), typeof(object), typeof(RadialMenuItemControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Command"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandProperty =
    DependencyProperty.Register(nameof(Command), typeof(ICommand), typeof(RadialMenuItem), new PropertyMetadata(null, OnCommandChanged));

        /// <summary>
        /// Identifies the <see cref="CommandParameter"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register(nameof(CommandParameter), typeof(object), typeof(RadialMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.Register(nameof(IsEnabled), typeof(bool), typeof(RadialMenuItem), new PropertyMetadata(true, OnIsEnabledChanged));

        /// <summary>
        /// Identifies the <see cref="IsSelected"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsSelectedProperty =
            DependencyProperty.Register(nameof(IsSelected), typeof(bool), typeof(RadialMenuItem), new PropertyMetadata(false, OnIsSelectedChanged));

        /// <summary>
        /// Identifies the <see cref="GroupName"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupNameProperty =
            DependencyProperty.Register(nameof(GroupName), typeof(string), typeof(RadialMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="Selectable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectableProperty =
            DependencyProperty.Register(nameof(Selectable), typeof(bool), typeof(RadialMenuItem), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="Deselectable"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DeselectableProperty =
            DependencyProperty.Register(nameof(Deselectable), typeof(bool), typeof(RadialMenuItem), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="Header"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(nameof(Header), typeof(object), typeof(RadialMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IconContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IconContentProperty =
            DependencyProperty.Register(nameof(IconContent), typeof(object), typeof(RadialMenuItem), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ContentSectorBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ContentSectorBackgroundProperty =
            DependencyProperty.Register(nameof(ContentSectorBackground), typeof(Brush), typeof(RadialMenuItem), new PropertyMetadata(null, OnContentSectorBackgroundPropertyChanged));

        internal Brush contentSectorBackgroundCache;

        private RadialMenuModel owner;

        private ObservableCollection<RadialMenuItem> childItems;

        /// <summary>
        /// Gets or sets the tooltip content of the current <see cref="RadialMenuItem"/>.
        /// </summary>
        /// <remarks>
        /// If this value is not set, the tooltip will display the <see cref="RadialMenuItem.Header"/> content.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadialMenuItem ToolTipContent="Home"/&gt;
        /// </code>
        /// </example>
        public object ToolTipContent
        {
            get
            {
                return (object)GetValue(ToolTipContentProperty);
            }
            set
            {
                this.SetValue(ToolTipContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets command parameter that will be used by the <see cref="RadialMenuItem.Command"/> associated with the <see cref="RadialMenuItem"/>. 
        /// </summary>
        public object CommandParameter
        {
            get { return (object)this.GetValue(CommandParameterProperty); }
            set { this.SetValue(CommandParameterProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RadialMenuItem"/> will be enabled.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadialMenuItem Header="Item" IconContent="&amp;#xE113;" IsEnabled="True"/&gt;
        /// </code>
        /// </example>
        public bool IsEnabled
        {
            get { return (bool)this.GetValue(IsEnabledProperty); }
            set { this.SetValue(IsEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the <see cref="ICommand"/> associated the current menu item.
        /// </summary>
        public ICommand Command
        {
            get { return (ICommand)this.GetValue(CommandProperty); }
            set { this.SetValue(CommandProperty, value); }
        }

        /// <summary>
        /// Gets the parent <see cref="RadialMenuItem"/> of the current menu item.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadialMenuItem x:Name="menuItem"/&gt;
        /// </code>
        /// <code language="c#">
        /// var parent = this.menuItem;
        /// </code>
        /// </example>
        public RadialMenuItem ParentItem
        {
            get;
            internal set;
        }

        /// <summary>
        /// Gets or sets a value indicating whether the menu item is selected.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadialMenuItem Header="Item" IconContent="&amp;#xE113;" IsSelected="True"/&gt;
        /// </code>
        /// </example>
        public bool IsSelected
        {
            get
            {
                return (bool)GetValue(IsSelectedProperty);
            }
            set
            {
                this.SetValue(IsSelectedProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value specifying the name of the group this item belongs to. The default value is null. 
        /// </summary>
        /// <remarks>
        /// All items in a group behave like radio buttons when selected.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem GroupName="first" Header="first 1"/&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem GroupName="first" Header="first 2"/&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem GroupName="first" Header="first 3"/&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem GroupName="second" Header="second 1"/&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem GroupName="second" Header="second 2"/&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem GroupName="second" Header="second 3"/&gt;
        /// &lt;/telerikPrimitives:RadRadialMenu&gt;
        /// </code>
        /// </example>
        public string GroupName
        {
            get
            {
                return (string)GetValue(GroupNameProperty);
            }
            set
            {
                this.SetValue(GroupNameProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RadialMenuItem"/> can be selected.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadialMenuItem Header="Item" IconContent="&amp;#xE113;" Selectable="True"/&gt;
        /// </code>
        /// </example>
        public bool Selectable
        {
            get
            {
                return (bool)GetValue(SelectableProperty);
            }
            set
            {
                this.SetValue(SelectableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="RadialMenuItem"/> can be deselected.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadialMenuItem Header="Item" IconContent="&amp;#xE113;" Deselectable="True"/&gt;
        /// </code>
        /// </example>
        public bool Deselectable
        {
            get
            {
                return (bool)this.GetValue(DeselectableProperty);
            }
            set
            {
                this.SetValue(DeselectableProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value specifying the visual representation of the title of the <see cref="RadialMenuItem"/>.
        /// </summary> 
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadialMenuItem Header="Item" IconContent="&amp;#xE113;"/&gt;
        /// </code>
        /// </example>
        public object Header
        {
            get
            {
                return (object)this.GetValue(HeaderProperty);
            }
            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value specifying the visual representation of the icon associated with the <see cref="RadialMenuItem"/>.
        /// </summary> 
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadialMenuItem Header="Item" IconContent="&amp;#xE113;"/&gt;
        /// </code>
        /// </example>
        public object IconContent
        {
            get
            {
                return (object)this.GetValue(IconContentProperty);
            }
            set
            {
                this.SetValue(IconContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="Brush"/> value that defines the background of the <see cref="RadialMenuItem"/>.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadialMenuItem ContentSectorBackground="Violet"/&gt;
        /// </code>
        /// </example>
        public Brush ContentSectorBackground
        {
            get
            {
                return this.contentSectorBackgroundCache;
            }
            set
            {
                this.SetValue(ContentSectorBackgroundProperty, value);
            }
        }

        /// <summary>
        /// Gets the <see cref="RadialMenuItem"/> collection associated with the current <see cref="RadialMenuItem"/>. 
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikPrimitives:RadRadialMenu x:Name="radialMenu"&gt;
        ///     &lt;telerikPrimitives:RadialMenuItem Header="Item" IconContent="&amp;#xE113;" x:Name="menuItem"&gt;
        ///         &lt;telerikPrimitives:RadialMenuItem Header="SubItem 1" IconContent="&amp;#xE113;"/&gt;
        ///         &lt;telerikPrimitives:RadialMenuItem Header="SubItem 2" IconContent="&amp;#xE113;"/&gt;
        ///     &lt;/telerikPrimitives:RadialMenuItem&gt;
        /// &lt;/telerikPrimitives:RadRadialMenu&gt;
        /// </code>
        /// <code language="c#">
        /// var item = this.radialMenu.Items[0];
        /// var children = item.ChildItems;
        /// </code>
        /// </example>
        public ObservableCollection<RadialMenuItem> ChildItems
        {
            get
            {
                if (this.childItems == null)
                {
                    this.childItems = new MenuItemCollection<RadialMenuItem>() { Owner = this.Owner, ParentItem = this };
                }

                return this.childItems;
            }
        }

        internal bool HasChildren
        {
            get
            {
                return this.childItems != null && this.childItems.Count > 0;
            }
        }

        internal RadialMenuModel Owner
        {
            get
            {
                return this.owner;
            }
            set
            {
                this.owner = value;
                ((MenuItemCollection<RadialMenuItem>)this.ChildItems).Owner = value;
                this.UpdateIsEnabled();
            }
        }

        internal int Index { get; set; }

        internal bool CanNavigate
        {
            get
            {
                return this.ChildItems != null && this.ChildItems.Count > 0;
            }
        }

        internal void ExecuteCommand()
        {
            RadialMenuItemContext context = this.GetCommandContext();

            if (this.Command != null && this.Command.CanExecute(context))
            {
                this.Command.Execute(context);
            }
        }

        internal void UpdateIsEnabled()
        {
            if (this.Owner != null && this.Command != null)
            {
                RadialMenuItemContext context = this.GetCommandContext();

                this.IsEnabled = this.Command.CanExecute(context);
            }
        }

        private static void OnContentSectorBackgroundPropertyChanged(DependencyObject target, DependencyPropertyChangedEventArgs args)
        {
            RadialMenuItem item = (RadialMenuItem)target;
            item.contentSectorBackgroundCache = args.NewValue as Brush;

            if (item.owner != null)
            {
                item.Owner.UpdateRingsVisualItems();
            }
        }

        private static void OnIsSelectedChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as RadialMenuItem;
            var newValue = (bool)e.NewValue;
            var oldValue = (bool)e.OldValue;

            if (item.IsInternalPropertyChange)
            {
                return;
            }

            if (oldValue == newValue || !RadialMenuModel.CanChangeSelection(item, newValue))
            {
                item.ChangePropertyInternally(RadialMenuItem.IsSelectedProperty, oldValue);
            }

            if (item.Owner != null)
            {
                item.Owner.OnSelectionChanged(item);

                var contentSegment = item.Owner.GetContentSegment(item);
                if (contentSegment != null)
                {
                    var radialMenuItemControl = contentSegment.Visual as RadialMenuItemControl;
                    if (radialMenuItemControl != null)
                    {
                        var peer = FrameworkElementAutomationPeer.CreatePeerForElement(radialMenuItemControl) as RadialMenuItemControlAutomationPeer;
                        if (peer != null)
                        {
                            peer.RaiseToggleStatePropertyChangedEvent((bool)e.OldValue, (bool)e.NewValue);
                            if (newValue)
                            {
                                peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementAddedToSelection);
                                peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementSelected);
                            }
                            else
                            {
                                peer.RaiseAutomationEvent(AutomationEvents.SelectionItemPatternOnElementRemovedFromSelection);
                            }
                        }
                    }
                }
            }
        }

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as RadialMenuItem;

            if (item.Owner != null)
            {
                item.Owner.OnIsEnabledChanged(item);
            }
        }

        private static void OnCommandChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var item = d as RadialMenuItem;

            var oldCommand = e.OldValue as ICommand;
            var newCommand = e.NewValue as ICommand;

            if (oldCommand != null)
            {
                oldCommand.CanExecuteChanged -= item.Command_CanExecuteChanged;
            }

            if (newCommand != null)
            {
                newCommand.CanExecuteChanged += item.Command_CanExecuteChanged;
            }

            item.UpdateIsEnabled();
        }

        private void Command_CanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateIsEnabled();
        }

        private RadialMenuItemContext GetCommandContext()
        {
            RadialMenuItemContext context = this.Owner.GetCommandContext();
            context.MenuItem = this;
            context.CommandParameter = this.CommandParameter;

            return context;
        }
    }
}
