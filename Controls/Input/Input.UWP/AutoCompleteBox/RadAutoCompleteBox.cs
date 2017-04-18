using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls.Input.AutoCompleteBox;
using Windows.Foundation;
using Windows.System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation;
using Windows.UI.Xaml.Automation.Peers;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;

namespace Telerik.UI.Xaml.Controls.Input
{
    /// <summary>
    /// Represents an input control that can use a collection of items to display suggestions while the user types. Essentially, this is
    /// a text box control with extended functionality to support customized suggestion items.
    /// </summary>
    [TemplatePart(Name = "PART_TextBox", Type = typeof(AutoCompleteTextBox))]
    [TemplatePart(Name = "PART_Popup", Type = typeof(Popup))]
    [TemplatePart(Name = "PART_SuggestionsControl", Type = typeof(SuggestionItemsControl))]
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "WatermarkUnfocused", GroupName = "FocusStates")]
    [TemplateVisualState(Name = "WatermarkFocused", GroupName = "FocusStates")]
    public class RadAutoCompleteBox : RadHeaderedControl, INotifyPropertyChanged
    {
        /// <summary>
        /// Identifies the <see cref="Text"/> property.
        /// </summary>
        public static readonly DependencyProperty TextProperty =
            DependencyProperty.Register(nameof(Text), typeof(string), typeof(RadAutoCompleteBox), new PropertyMetadata(string.Empty, OnAutoCompleteBoxTextChanged));

        /// <summary>
        /// Identifies the <see cref="FilterMode"/> property.
        /// </summary>
        public static readonly DependencyProperty FilterModeProperty =
            DependencyProperty.Register(nameof(FilterMode), typeof(AutoCompleteBoxFilterMode), typeof(RadAutoCompleteBox), new PropertyMetadata(AutoCompleteBoxFilterMode.StartsWith, OnFilterModeChanged));

        /// <summary>
        /// Identifies the <see cref="FilterComparisonMode"/> property.
        /// </summary>
        public static readonly DependencyProperty FilterComparisonModeProperty =
            DependencyProperty.Register(nameof(FilterComparisonMode), typeof(StringComparison), typeof(RadAutoCompleteBox), new PropertyMetadata(StringComparison.CurrentCultureIgnoreCase, OnFilterComparisonModeChanged));

        /// <summary>
        /// Identifies the <see cref="FilterMemberPath"/> property.
        /// </summary>
        public static readonly DependencyProperty FilterMemberPathProperty =
            DependencyProperty.Register(nameof(FilterMemberPath), typeof(string), typeof(RadAutoCompleteBox), new PropertyMetadata(null, OnFilterMemberPathChanged));

        /// <summary>
        /// Identifies the <see cref="DisplayMemberPath"/> property.
        /// </summary>
        public static readonly DependencyProperty DisplayMemberPathProperty =
            DependencyProperty.Register(nameof(DisplayMemberPath), typeof(string), typeof(RadAutoCompleteBox), new PropertyMetadata(null, OnDisplayMemberPathChanged));

        /// <summary>
        /// Identifies the <see cref="FilterStartThreshold"/> property.
        /// </summary>
        public static readonly DependencyProperty FilterStartThresholdProperty =
            DependencyProperty.Register(nameof(FilterStartThreshold), typeof(int), typeof(RadAutoCompleteBox), new PropertyMetadata(0, OnFilterStartThresholdChanged));

        /// <summary>
        /// Identifies the <see cref="FilterDelay"/> property.
        /// </summary>
        public static readonly DependencyProperty FilterDelayProperty =
            DependencyProperty.Register(nameof(FilterDelay), typeof(TimeSpan), typeof(RadAutoCompleteBox), new PropertyMetadata(TimeSpan.Zero, OnFilterDelayChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsSource"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(nameof(ItemsSource), typeof(IEnumerable), typeof(RadAutoCompleteBox), new PropertyMetadata(null, OnItemsSourceChanged));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(RadAutoCompleteBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemTemplateSelector"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(RadAutoCompleteBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="AutosuggestFirstItem"/> property.
        /// </summary>
        public static readonly DependencyProperty AutosuggestFirstItemProperty =
            DependencyProperty.Register(nameof(AutosuggestFirstItem), typeof(bool), typeof(RadAutoCompleteBox), new PropertyMetadata(true));

        /// <summary>
        /// Identifies the <see cref="DropDownPlacement"/> property.
        /// </summary>
        public static readonly DependencyProperty DropDownPlacementProperty =
            DependencyProperty.Register(nameof(DropDownPlacement), typeof(AutoCompleteBoxPlacementMode), typeof(RadAutoCompleteBox), new PropertyMetadata(AutoCompleteBoxPlacementMode.Auto, OnDropDownPlacementChanged));

        /// <summary>
        /// Identifies the <see cref="DropDownMaxHeight"/> property.
        /// </summary>
        public static readonly DependencyProperty DropDownMaxHeightProperty =
            DependencyProperty.Register(nameof(DropDownMaxHeight), typeof(double), typeof(RadAutoCompleteBox), new PropertyMetadata(double.PositiveInfinity, OnDropDownMaxHeightChanged));

        /// <summary>
        /// Identifies the <see cref="IsDropDownOpen"/> property.
        /// </summary>
        public static readonly DependencyProperty IsDropDownOpenProperty =
            DependencyProperty.Register(nameof(IsDropDownOpen), typeof(bool), typeof(RadAutoCompleteBox), new PropertyMetadata(false, OnIsDropDownOpenChanged));

        /// <summary>
        /// Identifies the RadAutoCompleteBox.TextMatchHighlightStyle attached property.
        /// </summary>
        public static readonly DependencyProperty TextMatchHighlightStyleProperty =
            DependencyProperty.RegisterAttached("TextMatchHighlightStyle", typeof(HighlightStyle), typeof(RadAutoCompleteBox), new PropertyMetadata(new HighlightStyle() { Foreground = new SolidColorBrush(Color.FromArgb(0xFF, 0x26, 0xA0, 0xDA)) }));

        /// <summary>
        /// Identifies the RadAutoCompleteBox.IsTextMatchHighlightEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty IsTextMatchHighlightEnabledProperty =
            DependencyProperty.RegisterAttached("IsTextMatchHighlightEnabled", typeof(bool), typeof(RadAutoCompleteBox), new PropertyMetadata(false));

        /// <summary>
        /// Identifies the <see cref="Watermark"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkProperty =
            DependencyProperty.Register(nameof(Watermark), typeof(object), typeof(RadAutoCompleteBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the WatermarkTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty WatermarkTemplateProperty =
            DependencyProperty.Register(nameof(WatermarkTemplate), typeof(DataTemplate), typeof(RadAutoCompleteBox), null);
            
        /// <summary>
        /// Identifies the <see cref="SelectedItem"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(nameof(SelectedItem), typeof(object), typeof(RadAutoCompleteBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="NoResultsContent"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NoResultsContentProperty =
            DependencyProperty.Register(nameof(NoResultsContent), typeof(object), typeof(RadAutoCompleteBox), new PropertyMetadata(null, OnNoResultsContentChanged));

        /// <summary>
        /// Identifies the <see cref="NoResultsContentTemplate"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty NoResultsContentTemplateProperty =
            DependencyProperty.Register(nameof(NoResultsContentTemplate), typeof(DataTemplate), typeof(RadAutoCompleteBox), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="IsNoResultsContentEnabled"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty IsNoResultsContentEnabledProperty =
            DependencyProperty.Register(nameof(IsNoResultsContentEnabled), typeof(bool), typeof(RadAutoCompleteBox), new PropertyMetadata(false));

        internal const double PopupOffsetFromTextBox = 2.0;

        internal Popup suggestionsPopup;
        internal SuggestionItemsControl suggestionsControl;
        internal TextBox textbox;
        internal ContentControl noResultsControl;

        internal ITextSearchProvider suggestionsProvider;
        internal bool isUserTyping;

        private const string TextBoxPartName = "PART_TextBox";
        private const string SuggestionsPopupPartName = "PART_Popup";
        private const string SuggestionsControlPartName = "PART_SuggestionsControl";
        private const string NoResultsControlPartName = "PART_NoResultsControl";
        private const string NoResultsFoundResourceKey = "AutoCompleteNoResultsFound";

        private readonly DispatcherTimer filterDelayTimer;

        private AutoCompleteBoxPlacementMode dropDownPlacementCache = AutoCompleteBoxPlacementMode.Auto;
        private double dropDownMaxHeightCache = double.PositiveInfinity;
        private TimeSpan filterDelayCache = TimeSpan.Zero;
        private int filterStartThresholdCache;
        private string filterMemberPathCache;
        private string textCache = string.Empty;
        private bool textChangedByPopupInteraction;
        private bool forceSuggestionsRefreshProgrammatically;
        private FocusState lastFocusState = FocusState.Unfocused;
        private bool shouldMarkText;
        private bool setProgrammaticFocus;
        private bool noResultsFound;
        private bool textChangedWhileDropDownClosed = false;
        /// <summary>
        /// Initializes a new instance of the <see cref="RadAutoCompleteBox" /> class.
        /// </summary>
        public RadAutoCompleteBox()
        {
            this.DefaultStyleKey = typeof(RadAutoCompleteBox);

            this.SizeChanged += this.OnSizeChanged;

            this.filterDelayTimer = new DispatcherTimer();
            this.filterDelayTimer.Tick += this.OnFilterDelayTimerTick;

            this.InitializeSuggestionsProvider(this.GetTextSearchProvider());
        }

        /// <summary>
        /// Occurs when a property of the <see cref="RadAutoCompleteBox"/> changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when the end user selects a suggestion item from the drop-down list of items.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event SelectionChangedEventHandler SelectionChanged;

        /// <summary>
        /// Occurs when the content of the <see cref="TextBox"/> part of the control changes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly")]
        public event TextChangedEventHandler TextChanged;

        /// <summary>
        /// Gets or sets the text content of the autocomplete box.
        /// </summary>
        /// <value>The default value is <see cref="String.Empty"/>.</value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox Text="California"/&gt;
        /// </code>
        /// </example>
        public string Text
        {
            get
            {
                return (string)this.GetValue(TextProperty);
            }
            set
            {
                this.SetValue(TextProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the mode used to filter the suggestion items based on the user input.
        /// </summary>
        /// <value>
        /// The default value is <see cref="AutoCompleteBoxFilterMode.StartsWith"/>.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox FilterMode="Contains"/&gt;
        /// </code>
        /// </example>
        /// <seealso cref="AutoCompleteBoxFilterMode"/>
        public AutoCompleteBoxFilterMode FilterMode
        {
            get
            {
                return (AutoCompleteBoxFilterMode)this.GetValue(FilterModeProperty);
            }
            set
            {
                this.SetValue(FilterModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value from the <see cref="StringComparison"/>
        /// enumeration that defines the way the candidate suggestion key is compared
        /// to the current user input.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox FilterComparisonMode="InvariantCultureIgnoreCase"/&gt;
        /// </code>
        /// </example>
        public StringComparison FilterComparisonMode
        {
            get
            {
                return (StringComparison)this.GetValue(FilterComparisonModeProperty);
            }
            set
            {
                this.SetValue(FilterComparisonModeProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a string representing a property path
        /// of a single object within the <see cref="RadAutoCompleteBox.ItemsSource"/>
        /// to be used when filtering the suggestion items.
        /// </summary>
        /// <remarks>
        /// If no <see cref="RadAutoCompleteBox.FilterMemberPath"/> value is set, the
        /// <see cref="RadAutoCompleteBox.DisplayMemberPath"/> value is used to filter
        /// the suggestion items.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox FilterMemberPath="PropertyName"/&gt;
        /// </code>
        /// </example>
        /// <seealso cref="RadAutoCompleteBox.DisplayMemberPath"/>
        public string FilterMemberPath
        {
            get
            {
                return this.filterMemberPathCache;
            }
            set
            {
                this.SetValue(FilterMemberPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a string representing a property path
        /// of a single object within the <see cref="RadAutoCompleteBox.ItemsSource"/>
        /// to be used to display the suggestion items.
        /// </summary>
        /// <remarks>
        /// If no <see cref="RadAutoCompleteBox.FilterMemberPath"/> value is set, this 
        /// value is also used to filter the suggestion items.
        /// </remarks>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox DisplayMemberPath="PropertyName"/&gt;
        /// </code>
        /// </example>
        /// <seealso cref="RadAutoCompleteBox.FilterMemberPath"/>
        public string DisplayMemberPath
        {
            get
            {
                return (string)this.GetValue(DisplayMemberPathProperty);
            }
            set
            {
                this.SetValue(DisplayMemberPathProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of symbols that have to be typed in <see cref="RadAutoCompleteBox"/>
        /// before the filtering procedure starts.
        /// </summary>
        /// <value>
        /// The default value is <c>0</c>.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox FilterStartThreshold="3"/&gt;
        /// </code>
        /// </example>
        public int FilterStartThreshold
        {
            get
            {
                return this.filterStartThresholdCache;
            }
            set
            {
                this.SetValue(FilterStartThresholdProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="TimeSpan"/> struct
        /// that represents the time interval between the last end user input action and
        /// a filter pass operation. 
        /// </summary>
        /// <value>
        /// The default value of this property is 0 milliseconds.
        /// </value>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox FilterDelay="1000"/&gt;
        /// </code>
        /// </example>
        public TimeSpan FilterDelay
        {
            get
            {
                return this.filterDelayCache;
            }
            set
            {
                this.SetValue(FilterDelayProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a function that acquires a property value of the suggestion
        /// item and returns a value based on which the suggestion is filtered.
        /// </summary>
        /// <example>
        /// <para>
        /// This example demonstrates how to filter items, that have a <see cref="DateTime"/> property, by
        /// year while the displayed text is the full date.
        /// </para>
        /// <para>
        /// First, create a sample class that has a property of type <see cref="DateTime"/>:
        /// </para>
        /// <code language="c#">
        ///     public class Birthday
        ///     {
        ///         public DateTime Date { get; set; }
        /// 
        ///         public override string ToString()
        ///         {
        ///             return this.Date.ToString("dd/mm/yyyy");
        ///         }
        ///     }
        /// </code>
        /// <para>
        /// Then define the <see cref="AutoCompleteBox"/>:
        /// </para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox x:Name="autoCompleteBox"/&gt;
        /// </code>
        /// Finally, set the FilterMemberProvider property:
        /// <code language="c#">
        /// this.autoCompleteBox.FilterMemberProvider = (object item) =>
        ///     {
        ///         return (item as Birthday).Date.Year.ToString();
        ///     };
        /// </code>
        /// </example>
        public Func<object, string> FilterMemberProvider
        {
            get
            {
                return this.suggestionsProvider.FilterMemberProvider;
            }
            set
            {
                this.suggestionsProvider.FilterMemberProvider = value;
            }
        }

        /// <summary>
        /// Gets an <see cref="IEnumerable"/> implementation that contains all suggestion items that
        /// have passed the filter based on the provided input.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox x:Name="autoCompleteBox"/&gt;
        /// </code>
        /// <code language="c#">
        /// var filteredItems = autoCompleteBox.FilteredItems;
        /// </code>
        /// </example>
        public IEnumerable FilteredItems
        {
            get
            {
                if (!this.suggestionsProvider.HasItems && string.IsNullOrEmpty(this.suggestionsProvider.InputString))
                {
                    return this.ItemsSource;
                }

                return this.suggestionsProvider.FilteredItems;
            }
        }

        /// <summary>
        /// Gets or sets an implementation of the <see cref="IEnumerable"/> interface that
        /// contains all items available for suggestion items in this instance of the <see cref="RadAutoCompleteBox"/> class.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox x:Name="autoCompleteBox"/&gt;
        /// </code>
        /// <code language="c#">
        /// autoCompleteBox.ItemsSource = new List&lt;string&gt; { "Apple", "Orange", "Banana", "Pineapple" };
        /// </code>
        /// </example>
        public IEnumerable ItemsSource
        {
            get
            {
                return (IEnumerable)this.GetValue(ItemsSourceProperty);
            }
            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets an instance of the <see cref="DataTemplate"/> class
        /// that defines the visual structure of a suggestion item.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox&gt;
        ///     &lt;telerikInput:RadAutoCompleteBox.ItemTemplate&gt;
        ///         &lt;DataTemplate&gt;
        ///             &lt;TextBlock Text="{Binding PropertyName}" Foreground="Orange" FontSize="20" /&gt;
        ///         &lt;/DataTemplate&gt;
        ///     &lt;/telerikInput:RadAutoCompleteBox.ItemTemplate&gt;
        /// &lt;/telerikInput:RadAutoCompleteBox&gt;
        /// </code>
        /// </example>
        public DataTemplate ItemTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(ItemTemplateProperty);
            }
            set
            {
                this.SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the custom logic for choosing the template used to display each suggestion item.
        /// </summary>
        /// <example>
        /// <para>
        /// This example demonstrates how to set custom template for the suggestion items depending on the
        /// length of their string representation.
        /// </para>
        /// <para>
        /// First, create a custom class that inherits the <see cref="Windows.UI.Xaml.Controls.DataTemplateSelector"/>
        /// class and override its <see cref="Windows.UI.Xaml.Controls.DataTemplateSelector.SelectTemplateCore(object, Windows.UI.Xaml.DependencyObject)"/>
        /// method.
        /// </para>
        /// <code language="c#">
        /// public class CustomItemTemplateSelector : DataTemplateSelector
        /// {
        ///     public DataTemplate Template1;
        /// 
        ///     public DataTemplate Template2;
        /// 
        ///     protected override Windows.UI.Xaml.DataTemplate SelectTemplateCore(object item, Windows.UI.Xaml.DependencyObject container)
        ///     {
        ///         var suggestionItem = item as string;
        ///         if (suggestionItem.Length > 5)
        ///         {
        ///             return Template1;
        ///         }
        /// 
        ///         return Template2;
        ///     }
        /// }
        /// </code>
        /// <para>
        /// Then define instance of this class in the static resources. It is easier to set the template styles in XAML.
        /// </para>
        /// <code language="xaml">
        /// &lt;Page.Resources&gt;
        ///     &lt;local:CustomItemTemplateSelector x:Key="templateSelector"&gt;
        ///         &lt;local:CustomItemTemplateSelector.Template1&gt;
        ///             &lt;DataTemplate&gt;
        ///                 &lt;TextBlock Text="{Binding}" Foreground="Red"/&gt;
        ///             &lt;/DataTemplate&gt;
        ///         &lt;/local:CustomItemTemplateSelector.Template1&gt;
        ///         &lt;local:CustomItemTemplateSelector.Template2&gt;
        ///             &lt;DataTemplate&gt;
        ///                 &lt;TextBlock Text="{Binding}" Foreground="Blue"/&gt;
        ///             &lt;/DataTemplate&gt;
        ///         &lt;/local:CustomItemTemplateSelector.Template2&gt;
        ///     &lt;/local:CustomItemTemplateSelector&gt;
        /// &lt;/Page.Resources&gt;
        /// </code>
        /// <para>
        /// Now you can define the <see cref="RadAutoCompleteBox"/> and set its ItemTemplateSelector property:
        /// </para>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox x:Name="autoCompleteBox" ItemTemplateSelector="{StaticResource templateSelector}"/&gt;
        /// </code>
        /// <code language="c#">
        /// autoCompleteBox.ItemsSource = new List&lt;string&gt; { "Apple", "Orange", "Pineapple", "Tomato", "Cucumber", "Patato", "Grapes", "Egg", "Cheese", "Bread", "Milk"};
        /// </code>
        /// </example>
        public DataTemplateSelector ItemTemplateSelector
        {
            get
            {
                return (DataTemplateSelector)this.GetValue(ItemTemplateSelectorProperty);
            }
            set
            {
                this.SetValue(ItemTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the first suggestion item 
        /// should be highlighted by default when the drop-down control is open.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox AutosuggestFirstItem="False"/&gt;
        /// </code>
        /// </example>
        public bool AutosuggestFirstItem
        {
            get
            {
                return (bool)this.GetValue(AutosuggestFirstItemProperty);
            }
            set
            {
                this.SetValue(AutosuggestFirstItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value that specifies the placement of the drop-down that holds the available suggestion items relative to the the input field.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox DropDownPlacement="Top"/&gt;
        /// </code>
        /// </example>
        public AutoCompleteBoxPlacementMode DropDownPlacement
        {
            get
            {
                return this.dropDownPlacementCache;
            }
            set
            {
                this.SetValue(DropDownPlacementProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the maximum height constraint of the drop-down list that holds the available suggestion items.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox DropDownMaxHeight="200"/&gt;</code>
        /// </example>
        public double DropDownMaxHeight
        {
            get
            {
                return this.dropDownMaxHeightCache;
            }
            set
            {
                this.SetValue(DropDownMaxHeightProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the drop-down suggestions list is currently open.
        /// </summary>
        public bool IsDropDownOpen
        {
            get
            {
                return (bool)this.GetValue(IsDropDownOpenProperty);
            }
            set
            {
                this.SetValue(IsDropDownOpenProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content to be displayed when the input field is empty and unfocused.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox Watermark="Write Here"/&gt;
        /// </code>
        /// </example>
        public object Watermark
        {
            get
            {
                return (object)this.GetValue(WatermarkProperty);
            }
            set
            {
                this.SetValue(WatermarkProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the data template applied to the watermark presenter when the TextBox is empty and unfocused.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox&gt;
        ///     &lt;telerikInput:RadAutoCompleteBox.WatermarkTemplate&gt;
        ///         &lt;DataTemplate&gt;
        ///             &lt;Ellipse Fill="Gray" Width="10" Height="10"/&gt;
        ///         &lt;/DataTemplate&gt;
        ///     &lt;/telerikInput:RadAutoCompleteBox.WatermarkTemplate&gt;
        /// &lt;/telerikInput:RadAutoCompleteBox&gt; 
        /// </code>
        /// </example>
        public DataTemplate WatermarkTemplate
        {
            get
            {
                return (DataTemplate)this.GetValue(WatermarkTemplateProperty);
            }
            set
            {
                this.SetValue(WatermarkTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets currently selected item in the RadAutoCompleteBox.
        /// </summary>
        /// <example>
        /// <code language="xaml">
        /// &lt;telerikInput:RadAutoCompleteBox x:Name="autoCompleteBox"/&gt;
        /// </code>
        /// <code language="c#">
        /// var selected = autoCompleteBox.SelectedItem;
        /// </code>
        /// </example>
        public object SelectedItem
        {
            get
            {
                return (object)this.GetValue(SelectedItemProperty);
            }
            private set
            {
                this.SetValue(SelectedItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the content which will be shown when no results are found after filtering has been applied.
        /// </summary>
        public object NoResultsContent
        {
            get { return (object)GetValue(NoResultsContentProperty); }
            set { SetValue(NoResultsContentProperty, value); }
        }


        /// <summary>
        /// Gets or sets a value which indicates whether the  <see cref="RadAutoCompleteBox.NoResultsContentProperty"/> will be shown when its criteria is matched.
        /// </summary>
        public bool IsNoResultsContentEnabled
        {
            get { return (bool)GetValue(IsNoResultsContentEnabledProperty); }
            set { SetValue(IsNoResultsContentEnabledProperty, value); }
        }

        /// <summary>
        /// Gets or sets the template of the  <see cref="RadAutoCompleteBox.NoResultsContentProperty"/>.
        /// </summary>
        public DataTemplate NoResultsContentTemplate
        {
            get { return (DataTemplate)GetValue(NoResultsContentTemplateProperty); }
            set { SetValue(NoResultsContentTemplateProperty, value); }
        }

        private double DropDownClampedHeight
        {
            get
            {
                if (this.suggestionsControl.DesiredHeight > this.DropDownMaxHeight)
                {
                    return this.DropDownMaxHeight;
                }

                return this.suggestionsControl.DesiredHeight;
            }
        }

        /// <summary>
        /// Gets the value of the attached <see cref="RadAutoCompleteBox.TextMatchHighlightStyleProperty"/> for the
        /// specified <see cref="TextBlock"/>.
        /// </summary>
        /// <param name="target">The target text block element.</param>
        /// <returns>The style of the text block element.</returns>
        public static HighlightStyle GetTextMatchHighlightStyle(DependencyObject target)
        {
            if (target == null)
            {
                return null;
            }

            return (HighlightStyle)target.GetValue(TextMatchHighlightStyleProperty);
        }

        /// <summary>
        /// Sets the value of the attached <see cref="RadAutoCompleteBox.TextMatchHighlightStyleProperty"/> on the
        /// specified <see cref="TextBlock"/>.
        /// </summary>
        /// <param name="target">The target text block element.</param>
        /// <param name="value">The style to be set.</param>
        public static void SetTextMatchHighlightStyle(DependencyObject target, HighlightStyle value)
        {
            if (target != null)
            {
                target.SetValue(TextMatchHighlightStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets the value of the attached <see cref="RadAutoCompleteBox.IsTextMatchHighlightEnabledProperty"/> for the
        /// specified <see cref="TextBlock"/>.
        /// </summary>
        /// <param name="target">The target text block element.</param>
        /// <returns>The value of the attached property.</returns>
        public static bool GetIsTextMatchHighlightEnabled(DependencyObject target)
        {
            if (target == null)
            {
                return false;
            }

            return (bool)target.GetValue(IsTextMatchHighlightEnabledProperty);
        }

        /// <summary>
        /// Sets the value of the attached <see cref="RadAutoCompleteBox.IsTextMatchHighlightEnabledProperty"/> on the
        /// specified <see cref="TextBlock"/>.
        /// </summary>
        /// <param name="target">The target text block.</param>
        /// <param name="value">The value to be set.</param>
        public static void SetIsTextMatchHighlightEnabled(DependencyObject target, bool value)
        {
            if (target != null)
            {
                target.SetValue(IsTextMatchHighlightEnabledProperty, value);
            }
        }

        /// <summary>
        /// Forces a refresh of the list of available suggestions (and opens the drop-down if necessary) based on specified text input string. 
        /// If no string parameter is specified, uses the current value of the text field as the text to search for.
        /// </summary>
        /// <param name="searchText">The text to search for.</param>
        public void Suggest(string searchText = "")
        {
            if (!string.IsNullOrEmpty(searchText) && searchText != this.Text)
            {
                this.forceSuggestionsRefreshProgrammatically = true;
                this.Text = searchText;
                this.InvokeAsync(
                    Windows.UI.Core.CoreDispatcherPriority.Low,
                    () =>
                    {
                        // Reconsider future review
                        this.forceSuggestionsRefreshProgrammatically = false;
                    });
            }
            else
            {
                this.IsDropDownOpen = true;
            }
        }

        /// <summary>
        /// Initializes the specific <see cref="ITextSearchProvider"/> implementation
        /// for this <see cref="RadAutoCompleteBox"/> instance.
        /// </summary>
        /// <param name="provider">The provider that the <see cref="RadAutoCompleteBox"/> will be initialized with.</param>
        public void InitializeSuggestionsProvider(ITextSearchProvider provider)
        {
            if (this.suggestionsProvider != null)
            {
                this.suggestionsProvider.PropertyChanged -= this.OnSuggestionsProviderPropertyChanged;
            }

            this.suggestionsProvider = provider;

            if (this.suggestionsProvider != null)
            {
                this.suggestionsProvider.FilterMemberPath = this.filterMemberPathCache;
                this.suggestionsProvider.ItemsSource = this.ItemsSource;
                this.suggestionsProvider.ComparisonMode = this.FilterComparisonMode;

                this.suggestionsProvider.PropertyChanged += this.OnSuggestionsProviderPropertyChanged;
            }
        }

        internal bool ProcessKeyDown(VirtualKey key)
        {
            if (!this.isUserTyping || !this.IsDropDownOpen)
            {
                return false;
            }

            bool handled = false;

            switch (key)
            {
                case VirtualKey.Enter:
                    {
                        if (this.suggestionsControl.SelectedItem != null)
                        {
                            this.UpdateTextFromPopupInteraction(this.suggestionsControl.SelectedItem);
                        }

                        this.IsDropDownOpen = false;

                        handled = true;
                        break;
                    }
                case VirtualKey.Tab:
                    {
                        if (this.suggestionsControl.SelectedItem != null)
                        {
                            this.UpdateTextFromPopupInteraction(this.suggestionsControl.SelectedItem);
                        }

                        this.IsDropDownOpen = false;

                        handled = true;
                        break;
                    }
                case VirtualKey.Up:
                    {
                        this.suggestionsControl.SelectPreviousItem();

                        handled = true;
                        break;
                    }
                case VirtualKey.Down:
                    {
                        this.suggestionsControl.SelectNextItem();

                        handled = true;
                        break;
                    }
                case VirtualKey.Escape:
                    {
                        this.IsDropDownOpen = false;

                        handled = true;
                        break;
                    }
            }

            return handled;
        }

        /// <summary>
        /// Called before the KeyDown event occurs.
        /// </summary>
        /// <param name="e">The data for the event.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        protected override void OnKeyDown(KeyRoutedEventArgs e)
        {
            base.OnKeyDown(e);

            e.Handled = this.ProcessKeyDown(e.Key);
        }

        /// <summary>
        /// Called when the Framework <see cref="T:OnApplyTemplate" /> is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied" /> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.textbox = this.GetTemplatePartField<TextBox>(TextBoxPartName);
            applied = applied && this.textbox != null;

            this.suggestionsPopup = this.GetTemplatePartField<Popup>(SuggestionsPopupPartName);
            applied = applied && this.suggestionsPopup != null;

            this.suggestionsControl = this.GetTemplatePartField<SuggestionItemsControl>(SuggestionsControlPartName);
            applied = applied && this.suggestionsControl != null;

            this.noResultsControl = this.GetTemplatePartField<ContentControl>(NoResultsControlPartName);
            applied = applied && this.noResultsControl != null;

            return applied;
        }

        /// <summary>
        /// Occurs when the <see cref="T:OnApplyTemplate" /> method has been called and the template is already successfully applied.
        /// </summary>
        protected override void OnTemplateApplied()
        {
            base.OnTemplateApplied();

            this.textbox.TextChanged += this.OnTextBoxTextChanged;
            this.textbox.GotFocus += this.OnTextBoxGotFocus;
            this.textbox.LostFocus += this.OnTextBoxLostFocus;

            this.textbox.Text = this.textCache ?? string.Empty;

            this.suggestionsControl.owner = this;
            this.suggestionsControl.MaxHeight = this.DropDownMaxHeight;

            this.suggestionsControl.SizeChanged += this.OnSuggestionsControlSizeChanged;
            this.suggestionsControl.ItemTapped += this.OnSuggestionsControlItemTapped;

            if(this.NoResultsContent != null)
            {
                this.noResultsControl.Content = this.NoResultsContent;
            }
            else
            {
                this.noResultsControl.Content = InputLocalizationManager.Instance.GetString(NoResultsFoundResourceKey);
            }

            this.InitializeFilteredItemsBinding();
        }

        /// <inheritdoc/>
        protected override void UnapplyTemplateCore()
        {
            base.UnapplyTemplateCore();

            this.textbox.TextChanged -= this.OnTextBoxTextChanged;
            this.textbox.GotFocus -= this.OnTextBoxGotFocus;
            this.textbox.LostFocus -= this.OnTextBoxLostFocus;

            this.suggestionsControl.owner = null;
            this.suggestionsControl.SizeChanged -= this.OnSuggestionsControlSizeChanged;
            this.suggestionsControl.ItemTapped -= this.OnSuggestionsControlItemTapped;
        }

        /// <summary>
        /// Called within the handler of the <see cref="E:Unloaded" /> event. Allows inheritors to provide their specific logic.
        /// </summary>
        protected override void UnloadCore()
        {
            base.UnloadCore();

            this.SelectedItem = null;

            this.filterDelayTimer.Stop();
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new RadAutoCompleteBoxAutomationPeer(this);
        }

        /// <summary>
        /// Raises the <see cref="E:PropertyChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnPropertyChanged(PropertyChangedEventArgs args)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:TextChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="TextChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnTextChanged(TextChangedEventArgs args)
        {
            if (this.TextChanged != null)
            {
                this.TextChanged(this, args);
            }
        }

        /// <summary>
        /// Raises the <see cref="E:SelectionChanged" /> event.
        /// </summary>
        /// <param name="args">The <see cref="SelectionChangedEventArgs" /> instance containing the event data.</param>
        protected virtual void OnSelectionChanged(SelectionChangedEventArgs args)
        {
            if (this.SelectionChanged != null)
            {
                this.SelectionChanged(this, args);
            }
        }

        /// <summary>
        /// Creates a new <see cref="SuggestionItem" /> when needed.
        /// </summary>
        protected internal virtual DependencyObject GetContainerForSuggestionItem()
        {
            return this.suggestionsControl.GetContainerForSuggestionItem();
        }

        /// <summary>
        /// Prepares the <see cref="SuggestionItem" /> for further usage.
        /// </summary>
        protected internal virtual void PrepareContainerForSuggestionItem(DependencyObject element, object item)
        {
            this.suggestionsControl.PrepareContainerForSuggestionItem(element, item);
        }

        /// <summary>
        /// Undoes the effects of the PrepareContainerForSuggestionItem method.
        /// </summary>
        protected internal virtual void ClearContainerForSuggestionItem(DependencyObject element, object item)
        {
            this.suggestionsControl.ClearContainerForSuggestionItem(element, item);
        }

        private static void OnAutoCompleteBoxTextChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;

            autoCompleteBox.textCache = (string)args.NewValue;

            if (autoCompleteBox.IsTemplateApplied)
            {
                autoCompleteBox.textbox.Text = autoCompleteBox.textCache;

                if (AutomationPeer.ListenerExists(AutomationEvents.PropertyChanged))
                {
                    var peer = FrameworkElementAutomationPeer.FromElement(autoCompleteBox.suggestionsControl) as ListBoxAutomationPeer;
                    if (peer != null)
                    {
                        peer.RaiseAutomationEvent(AutomationEvents.LiveRegionChanged);
                    }
                }
            }
        }

        private static void OnFilterModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;

            if (autoCompleteBox.IsDropDownOpen)
            {
                autoCompleteBox.IsDropDownOpen = false;

                autoCompleteBox.suggestionsProvider.Reset();
            }

            autoCompleteBox.InitializeSuggestionsProvider(autoCompleteBox.GetTextSearchProvider());
        }

        private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;
            autoCompleteBox.suggestionsProvider.ItemsSource = args.NewValue as IEnumerable;

            autoCompleteBox.SelectedItem = null;
        }

        private static void OnFilterComparisonModeChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;
            autoCompleteBox.suggestionsProvider.ComparisonMode = (StringComparison)args.NewValue;
        }

        private static void OnFilterMemberPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;

            autoCompleteBox.filterMemberPathCache = args.NewValue as string;
            autoCompleteBox.suggestionsProvider.FilterMemberPath = autoCompleteBox.filterMemberPathCache;
        }

        private static void OnDisplayMemberPathChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;

            if (string.IsNullOrEmpty(autoCompleteBox.FilterMemberPath))
            {
                autoCompleteBox.FilterMemberPath = autoCompleteBox.DisplayMemberPath;
            }
        }

        private static void OnFilterStartThresholdChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;
            autoCompleteBox.filterStartThresholdCache = (int)args.NewValue;
        }

        private static void OnFilterDelayChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;
            autoCompleteBox.filterDelayCache = (TimeSpan)args.NewValue;
        }

        private static void OnDropDownPlacementChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;

            autoCompleteBox.dropDownPlacementCache = (AutoCompleteBoxPlacementMode)args.NewValue;

            if (!autoCompleteBox.IsTemplateApplied)
            {
                return;
            }

            if (autoCompleteBox.dropDownPlacementCache == AutoCompleteBoxPlacementMode.None)
            {
                autoCompleteBox.IsDropDownOpen = false;
            }
            else
            {
                autoCompleteBox.PositionPopup();
            }
        }

        private static void OnDropDownMaxHeightChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;

            autoCompleteBox.dropDownMaxHeightCache = (double)args.NewValue;
        }

        private static void OnIsDropDownOpenChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            RadAutoCompleteBox autoCompleteBox = sender as RadAutoCompleteBox;

            if (!autoCompleteBox.IsTemplateApplied)
            {
                return;
            }
        
            if (autoCompleteBox.IsDropDownOpen)
            {
                // NOTE: Implicit style for the popup child not applied on time unless we invoke this asynchronously. 
                // We cannot use the Popup.Opened/Closed events to perform this logic instead as the width changes visibly (as the popup is actually already open).

                if (autoCompleteBox.IsNoResultsContentEnabled && autoCompleteBox.noResultsFound)
                {
                    autoCompleteBox.InvokeAsync(() =>
                    {
                        autoCompleteBox.noResultsControl.Visibility = Visibility.Visible;

                        if (double.IsNaN(autoCompleteBox.noResultsControl.Width))
                        {
                            autoCompleteBox.noResultsControl.Width = autoCompleteBox.textbox.ActualWidth;
                        }

                        autoCompleteBox.PositionPopup();
                    });
                }
                else
                {
                    autoCompleteBox.noResultsControl.Visibility = Visibility.Collapsed;

                    autoCompleteBox.InvokeAsync(() =>
                    {
                        if (double.IsNaN(autoCompleteBox.suggestionsControl.Width))
                        {
                            autoCompleteBox.suggestionsControl.Width = autoCompleteBox.textbox.ActualWidth;
                        }

                        autoCompleteBox.PositionPopup();
                    });

                    autoCompleteBox.suggestionsControl.SelectOrScrollToFirstItem();
                    autoCompleteBox.setProgrammaticFocus = true;
                    autoCompleteBox.shouldMarkText = false;
                    autoCompleteBox.textbox.Focus(FocusState.Programmatic);
                }
            }
            else
            {
                autoCompleteBox.textChangedWhileDropDownClosed = false;
                autoCompleteBox.suggestionsPopup.HorizontalOffset = 0;
                autoCompleteBox.suggestionsPopup.VerticalOffset = 0;

                autoCompleteBox.suggestionsControl.SelectedIndex = -1;
                autoCompleteBox.suggestionsControl.ClearValue(FrameworkElement.WidthProperty);

                autoCompleteBox.suggestionsControl.MaxHeight = autoCompleteBox.DropDownMaxHeight;
            }

            RadAutoCompleteBoxAutomationPeer autoCompletePeer = FrameworkElementAutomationPeer.FromElement(autoCompleteBox) as RadAutoCompleteBoxAutomationPeer;
            if (autoCompletePeer != null)
            {
                autoCompletePeer.RaisePropertyChangedEvent(ExpandCollapsePatternIdentifiers.ExpandCollapseStateProperty, args.OldValue, args.NewValue);
            }
        }

        private static void OnNoResultsContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var autoComplete = d as RadAutoCompleteBox;
            if (autoComplete != null && autoComplete.IsTemplateApplied)
            {
                autoComplete.noResultsControl.Content = e.NewValue;
            }
        }

        private ITextSearchProvider GetTextSearchProvider()
        {
            switch (this.FilterMode)
            {
                case AutoCompleteBoxFilterMode.StartsWith:
                    return new StartsWithTextSearchProvider();
                default:
                    return new ContainsTextSearchProvider();
            }
        }

        private void InitializeFilteredItemsBinding()
        {
            Binding b = new Binding();
            b.Source = this;
            b.Path = new PropertyPath("FilteredItems");
            b.Mode = BindingMode.OneWay;

            this.suggestionsControl.SetBinding(ItemsControl.ItemsSourceProperty, b);
        }

        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs args)
        {
            if(!this.IsDropDownOpen)
            {
                this.textChangedWhileDropDownClosed = true;
            }

            this.RefreshSuggestions();

            this.UpdateWatermarkVisibility();

            this.Text = this.textbox.Text;
            this.OnTextChanged(args);

            if (this.SelectedItem != null && !this.textChangedByPopupInteraction)
            {
                object removedItem = this.SelectedItem;
                this.SelectedItem = null;
                this.OnSelectionChanged(new SelectionChangedEventArgs(new List<object> { removedItem }, new List<object>()));
            }

            this.textChangedByPopupInteraction = false;
        }

        private void RefreshSuggestions()
        {
            if (this.forceSuggestionsRefreshProgrammatically)
            {
                this.suggestionsProvider.Input(0, this.suggestionsProvider.InputString.Length, this.Text);
            }
            else if (this.isUserTyping && !this.textChangedByPopupInteraction)
            {
                if (!this.IsRefreshSuggestionsActionDelayed())
                {
                    this.RefreshSuggestionsOnTextChanged();
                }
            }
            else
            {
                this.suggestionsProvider.Reset();
            }
        }

        private bool IsRefreshSuggestionsActionDelayed()
        {
            if (this.filterDelayCache == TimeSpan.Zero || string.IsNullOrEmpty(this.textbox.Text))
            {
                return false;
            }

            this.filterDelayTimer.Interval = this.filterDelayCache;
            this.filterDelayTimer.Stop();
            this.filterDelayTimer.Start();

            return true;
        }

        private void OnFilterDelayTimerTick(object sender, object e)
        {
            this.RefreshSuggestionsOnTextChanged();

            this.filterDelayTimer.Stop();
        }

        private void RefreshSuggestionsOnTextChanged()
        {
            string newText = this.textbox.Text;

            if (this.textbox.Text.Length > this.filterStartThresholdCache)
            {
                if (newText != this.suggestionsProvider.InputString)
                {
                    this.suggestionsProvider.Input(0, this.suggestionsProvider.InputString.Length, newText);
                }
            }
            else
            {
                this.suggestionsProvider.Reset();
            }
        }
         
        private void OnSuggestionsProviderPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (args.PropertyName == "FilteredItems")
            {
                this.OnPropertyChanged(args);

                if (!this.IsTemplateApplied || this.dropDownPlacementCache == AutoCompleteBoxPlacementMode.None || this.textChangedByPopupInteraction || (!this.IsDropDownOpen && !this.textChangedWhileDropDownClosed))
                {
                    return;
                }

                this.noResultsFound = false;

                if (this.suggestionsProvider.HasItems)
                {
                        if(this.IsDropDownOpen)
                        {
                            this.IsDropDownOpen = false;
                        }
                        this.IsDropDownOpen = true;

                    // NOTE: FilteredItems can be changed if popup is already open.
                    this.suggestionsControl.SelectOrScrollToFirstItem();

                    // NOTE: "Reset" MaxHeight value to a reasonable default (so UI virtualization can operate)
                    // as we need to recalculate the popup position on every FilteredItems change.
                    this.suggestionsControl.MaxHeight = this.DropDownMaxHeight;
                }
                else
                {
                    if(!String.IsNullOrEmpty(this.suggestionsProvider.InputString) && this.IsNoResultsContentEnabled)
                    {
                        this.IsDropDownOpen = false;
                        this.noResultsFound = true;
                        this.IsDropDownOpen = true;
                        this.noResultsControl.MaxHeight = this.DropDownMaxHeight;
                    }
                    else
                    {
                        this.IsDropDownOpen = false;
                    }
                }
            }
        }

        private void PositionPopup()
        {
            Point originLocation = this.textbox.TransformToVisual(Window.Current.Content).TransformPoint(new Point(0, 0));

            this.AdjustPopupHorizontalOffset(originLocation);
            this.AdjustPopupVerticalOffset(originLocation);
        }

        private void AdjustPopupHorizontalOffset(Point originLocation)
        {
            FrameworkElement child = this.noResultsFound ? (FrameworkElement)this.noResultsControl : this.suggestionsControl;

            // Width is set either as a local value (in our code), or as an implicit style setter (in user code)
            if (double.IsNaN(child.Width) || child.Width <= this.textbox.ActualWidth)
            {
                return;
            }

            double availablePopupWidth;
            double availableOffsetWidth;

            if (this.FlowDirection == FlowDirection.LeftToRight)
            {
                availableOffsetWidth = originLocation.X;
                availablePopupWidth = Window.Current.Bounds.Width - availableOffsetWidth;
            }
            else
            {
                availablePopupWidth = originLocation.X;
                availableOffsetWidth = Window.Current.Bounds.Width - availablePopupWidth;
            }

            double horizontalOffset = child.Width - availablePopupWidth;
            if (horizontalOffset > 0 && horizontalOffset <= availableOffsetWidth)
            {
                this.suggestionsPopup.HorizontalOffset = -horizontalOffset;
            }
        }

        private void AdjustPopupVerticalOffset(Point originLocation)
        {
            if (this.suggestionsControl.DesiredHeight == 0 && !this.noResultsFound)
            {
                return;
            }

            FrameworkElement child = this.noResultsFound ? (FrameworkElement)this.noResultsControl : this.suggestionsControl;

            Rect occludedRect = Windows.UI.ViewManagement.InputPane.GetForCurrentView().OccludedRect;
            double occludedRectStartY = Window.Current.Bounds.Bottom;
            if (occludedRect.Y > 0)
            {
                occludedRectStartY = occludedRect.Y;
            }

            double distanceToBottom = occludedRectStartY - originLocation.Y - this.ActualHeight;
            double distanceToTop = originLocation.Y;

            bool showPopupBelowTextBox = this.ShouldShowPopupBelowTextBox(distanceToBottom, distanceToTop);

            this.SetPopupClampedMaxHeight(showPopupBelowTextBox ? distanceToBottom : distanceToTop);

            if (showPopupBelowTextBox)
            {
                this.suggestionsPopup.VerticalOffset = this.ActualHeight - PopupOffsetFromTextBox;
            }
            else
            {
                this.suggestionsPopup.VerticalOffset = -child.ActualHeight + (this.ActualHeight - this.textbox.ActualHeight) + PopupOffsetFromTextBox;
            }
        }

        private bool ShouldShowPopupBelowTextBox(double distanceToBottom, double distanceToTop)
        {
            if (this.dropDownPlacementCache == AutoCompleteBoxPlacementMode.Bottom)
            {
                return true;
            }
            else if (this.dropDownPlacementCache == AutoCompleteBoxPlacementMode.Top)
            {
                return false;
            }
            else
            {
                double desiredMaxHeight = this.DropDownClampedHeight;

                if (desiredMaxHeight <= distanceToBottom ||
                    (desiredMaxHeight > distanceToTop && distanceToBottom >= distanceToTop))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        private void SetPopupClampedMaxHeight(double windowConstraint)
        {
            double maxHeight = this.DropDownMaxHeight;
            if (maxHeight > windowConstraint)
            {
                maxHeight = windowConstraint;
            }

            this.suggestionsControl.MaxHeight = maxHeight;
        }

        private void OnTextBoxGotFocus(object sender, RoutedEventArgs args)
        {
            var textBox = sender as TextBox;

            if (this.lastFocusState != textBox.FocusState && this.lastFocusState != FocusState.Unfocused)
            {
                this.shouldMarkText = false;
            }
            else if (this.lastFocusState == Windows.UI.Xaml.FocusState.Unfocused)
            {
                this.lastFocusState = textBox.FocusState;
            }

            if (textBox.FocusState != Windows.UI.Xaml.FocusState.Pointer)
            {
                this.setProgrammaticFocus = false;
            }

            this.isUserTyping = true;

            this.UpdateWatermarkVisibility();
            this.UpdateCaretPosition();
        }

        private void OnTextBoxLostFocus(object sender, RoutedEventArgs args)
        {
            var textBox = sender as TextBox;

            this.setProgrammaticFocus = false;
            this.shouldMarkText = true;
            this.lastFocusState = FocusState.Unfocused;

            this.isUserTyping = false;

            this.IsDropDownOpen = false;

            this.UpdateWatermarkVisibility();
        }

        private void UpdateWatermarkVisibility()
        {
            if (this.isUserTyping || !string.IsNullOrEmpty(this.textbox.Text))
            {
                VisualStateManager.GoToState(this, "WatermarkFocused", false);
            }
            else
            {
                VisualStateManager.GoToState(this, "WatermarkUnfocused", false);
            }
        }

        private void UpdateCaretPosition()
        {
            if (this.textbox.FocusState == FocusState.Keyboard && this.shouldMarkText)
            {
                this.textbox.SelectionStart = 0;
                this.textbox.SelectionLength = this.textbox.Text.Length;
            }
            else if (this.setProgrammaticFocus || (this.textbox.FocusState == FocusState.Keyboard && !this.shouldMarkText))
            {
                this.textbox.SelectionStart = this.textbox.Text.Length;
                this.textbox.SelectionLength = 0;
            }
        }

        private void OnSizeChanged(object sender, SizeChangedEventArgs args)
        {
            this.IsDropDownOpen = false;
        }

        private void OnSuggestionsControlSizeChanged(object sender, SizeChangedEventArgs args)
        {
            if (this.IsDropDownOpen)
            {
                this.PositionPopup();
            }
        }

        private void OnSuggestionsControlItemTapped(object sender, SuggestionItemTappedEventArgs e)
        {
            this.UpdateTextFromPopupInteraction(e.Item);

            this.IsDropDownOpen = false;

            this.setProgrammaticFocus = true;
            this.textbox.Focus(FocusState.Programmatic);
        }

        internal void UpdateTextFromPopupInteraction(object suggestionItem)
        {
            this.textChangedByPopupInteraction = true;
            this.textbox.Text = this.suggestionsProvider.GetFilterKey(suggestionItem);
            this.UpdateCaretPosition();

            this.SelectedItem = suggestionItem;
            this.OnSelectionChanged(new SelectionChangedEventArgs(new List<object>(), new List<object> { suggestionItem }));
        }
    }
}