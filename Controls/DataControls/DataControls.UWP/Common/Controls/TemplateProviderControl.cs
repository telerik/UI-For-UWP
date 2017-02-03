using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    /// <summary>
    /// Encapsulates common properties like <see cref="P:TemplateProviderControl.ItemTemplate"/> and adds support for notifications when some of these properties changes.
    /// </summary>
    public abstract class TemplateProviderControl : RadControl
    {
        /// <summary>
        /// Identifies the ItemTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(TemplateProviderControl), new PropertyMetadata(null, OnItemTemplateChanged));

        /// <summary>
        /// Identifies the ItemTemplateSelector dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateSelectorProperty =
            DependencyProperty.Register(nameof(ItemTemplateSelector), typeof(DataTemplateSelector), typeof(TemplateProviderControl), new PropertyMetadata(null, OnItemTemplateSelectorChanged));

        /// <summary>
        /// Identifies the ItemContainerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleProperty =
            DependencyProperty.Register(nameof(ItemContainerStyle), typeof(Style), typeof(TemplateProviderControl), new PropertyMetadata(null, OnItemContainerStyleChanged));

        /// <summary>
        /// Identifies the ItemContainerStyleSelector dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemContainerStyleSelectorProperty =
            DependencyProperty.Register(nameof(ItemContainerStyleSelector), typeof(StyleSelector), typeof(TemplateProviderControl), new PropertyMetadata(null, OnItemContainerStyleSelectorChanged));

        internal DataTemplate itemTemplateCache;
        internal DataTemplateSelector itemTemplateSelectorCache;
        internal StyleSelector itemContainerStyleSelectorCache;
        internal Style itemContainerStyleCache;

        /// <summary>
        /// Initializes a new instance of the <see cref="TemplateProviderControl"/> class.
        /// </summary>
        protected TemplateProviderControl()
        {
        }

        /// <summary>
        /// Gets or sets a <see cref="DataTemplate"/> that represents the template applied to each visual item in the control.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get
            {
                return this.itemTemplateCache;
            }
            set
            {
                this.SetValue(ItemTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template selector that is used to select a template for an item container.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get
            {
                return this.itemTemplateSelectorCache;
            }
            set
            {
                this.SetValue(ItemTemplateSelectorProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the style that is applied to each item container.
        /// </summary>
        public Style ItemContainerStyle
        {
            get
            {
                return this.itemContainerStyleCache;
            }
            set
            {
                this.SetValue(ItemContainerStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a selector that is used when applying the style to an item container.
        /// </summary>
        public StyleSelector ItemContainerStyleSelector
        {
            get
            {
                return this.itemContainerStyleSelectorCache;
            }
            set
            {
                this.SetValue(ItemContainerStyleSelectorProperty, value);
            }
        }

        internal virtual Style GetItemContainerStyle(UIElement container, object dataItem)
        {
            if (this.itemContainerStyleSelectorCache != null)
            {
                return this.itemContainerStyleSelectorCache.SelectStyle(dataItem, container);
            }

            return this.itemContainerStyleCache;
        }

        internal virtual DataTemplate GetItemTemplate(UIElement container, object dataItem)
        {
            if (this.itemTemplateSelectorCache != null)
            {
                return this.itemTemplateSelectorCache.SelectTemplate(dataItem, container);
            }

            return this.itemTemplateCache;
        }

        /// <summary>
        /// Invalidates the visual representation of the control.
        /// </summary>
        protected virtual void InvalidateUI()
        {
        }

        /// <summary>
        /// Occurs when the <see cref="ItemTemplate"/> property has changed.
        /// </summary>
        protected virtual void OnItemTemplateChanged(DataTemplate oldTemplate)
        {
        }

        /// <summary>
        /// Occurs when the <see cref="ItemTemplateSelector"/> property has changed.
        /// </summary>
        protected virtual void OnItemTemplateSelectorChanged(DataTemplateSelector oldSelector)
        {
        }

        /// <summary>
        /// Occurs when the <see cref="ItemContainerStyle"/> property has changed.
        /// </summary>
        protected virtual void OnItemContainerStyleChanged(Style oldStyle)
        {
        }

        /// <summary>
        /// Occurs when the <see cref="ItemContainerStyleSelector"/> property has changed.
        /// </summary>
        protected virtual void OnItemContainerStyleSelectorChanged(StyleSelector oldSelector)
        {
        }

        private static void OnItemTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TemplateProviderControl control = sender as TemplateProviderControl;
            control.itemTemplateCache = e.NewValue as DataTemplate;
            control.OnItemTemplateChanged(e.OldValue as DataTemplate);
            control.InvalidateUI();
        }

        private static void OnItemTemplateSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TemplateProviderControl control = sender as TemplateProviderControl;
            control.itemTemplateSelectorCache = e.NewValue as DataTemplateSelector;
            control.OnItemTemplateSelectorChanged(e.OldValue as DataTemplateSelector);
            control.InvalidateUI();
        }

        private static void OnItemContainerStyleChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TemplateProviderControl control = sender as TemplateProviderControl;
            control.itemContainerStyleCache = e.NewValue as Style;
            control.OnItemContainerStyleChanged(e.OldValue as Style);
            control.InvalidateUI();
        }

        private static void OnItemContainerStyleSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            TemplateProviderControl control = sender as TemplateProviderControl;
            control.itemContainerStyleSelectorCache = e.NewValue as StyleSelector;
            control.OnItemContainerStyleSelectorChanged(e.OldValue as StyleSelector);
            control.InvalidateUI();
        }
    }
}
