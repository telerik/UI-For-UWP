using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Markup;

namespace Telerik.UI.Xaml.Controls.Primitives
{
    /// <summary>
    /// Represents legend control that displays the series from <see cref="ILegendInfoProvider"/> instance.
    /// </summary>
    [TemplatePart(Name = "PART_LegendPresenter", Type = typeof(ItemsControl))]
    [ContentProperty(Name = "LegendItems")]
    public class RadLegendControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="LegendProvider"/> property.
        /// </summary>
        public static readonly DependencyProperty LegendProviderProperty =
            DependencyProperty.Register(nameof(LegendProvider), typeof(object), typeof(RadLegendControl), new PropertyMetadata(null, OnProviderChanged));

        /// <summary>
        /// Identifies the <see cref="ItemsPanel"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register(nameof(ItemsPanel), typeof(ItemsPanelTemplate), typeof(RadLegendControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemTemplate"/> property.
        /// </summary>
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register(nameof(ItemTemplate), typeof(DataTemplate), typeof(RadLegendControl), new PropertyMetadata(null));

        private string presenterName = "PART_LegendPresenter";
        private FrameworkElement presenter;
        private LegendItemCollection localLegendItems;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadLegendControl"/> class.
        /// </summary>
        public RadLegendControl()
        {
            this.DefaultStyleKey = typeof(RadLegendControl);

            this.localLegendItems = new LegendItemCollection();
        }

        /// <summary>
        /// Gets or sets the items panel that will be used for the legend presenter to display data.
        /// </summary>
        public ItemsPanelTemplate ItemsPanel
        {
            get { return (ItemsPanelTemplate)this.GetValue(ItemsPanelProperty); }
            set { this.SetValue(ItemsPanelProperty, value); }
        }

        /// <summary>
        /// Gets or sets the DataTemplate used to display each legend item.
        /// </summary>
        public DataTemplate ItemTemplate
        {
            get { return (DataTemplate)this.GetValue(ItemTemplateProperty); }
            set { this.SetValue(ItemTemplateProperty, value); }
        }

        /// <summary>
        /// Gets or sets the provider that this <see cref="RadLegendControl"/> will use to display its items.
        /// </summary>
        public ILegendInfoProvider LegendProvider
        {
            get { return (ILegendInfoProvider)this.GetValue(LegendProviderProperty); }
            set { this.SetValue(LegendProviderProperty, value); }
        }

        /// <summary>
        /// Gets a collection of the <see cref="LegendItem"/> items that will be displayed.
        /// </summary>
        public virtual LegendItemCollection LegendItems
        {
            get { return this.LegendProvider != null ? this.LegendProvider.LegendInfos : this.localLegendItems; }
        }

        /// <summary>
        /// Called when the Framework OnApplyTemplate is called. Inheritors should override this method should they have some custom template-related logic.
        /// This is done to ensure that the <see cref="P:IsTemplateApplied"/> property is properly initialized.
        /// </summary>
        protected override bool ApplyTemplateCore()
        {
            var templateApplied = base.ApplyTemplateCore();

            this.presenter = this.GetTemplatePartField<FrameworkElement>(this.presenterName);

            templateApplied = templateApplied && this.presenter != null;

            if (templateApplied)
            {
                this.presenter.DataContext = this.LegendItems;
            }

            return templateApplied;
        }

        private static void OnProviderChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var legend = (RadLegendControl)d;

            if (e.NewValue != null)
            {
                legend.localLegendItems = null;
            }

            if (legend.presenter != null)
            {
                legend.presenter.DataContext = legend.LegendItems;
            }
        }
    }
}
