using System;
using System.Globalization;
using System.Windows;
using Telerik.UI.Automation.Peers;
using Telerik.UI.Xaml.Controls;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Automation.Peers;

namespace Telerik.UI.Xaml.Controls.Primitives.Pagination
{
    /// <summary>
    /// A special control, that displays the CurrentIndex and ItemCount information in a <see cref="RadPaginationControl"/>.
    /// </summary>
    [TemplateVisualState(Name = "Normal", GroupName = "CommonStates")]
    [TemplateVisualState(Name = "Disabled", GroupName = "CommonStates")]
    public class PaginationIndexLabelControl : RadControl
    {
        /// <summary>
        /// Identifies the <see cref="Separator"/> dependency property.
        /// </summary>  
        public static readonly DependencyProperty SeparatorProperty =
            DependencyProperty.Register(nameof(Separator), typeof(string), typeof(PaginationIndexLabelControl), new PropertyMetadata(" / "));

        /// <summary>
        /// Identifies the <see cref="ItemCountFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCountFormatProperty =
            DependencyProperty.Register(nameof(ItemCountFormat), typeof(string), typeof(PaginationIndexLabelControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CurrentIndexFormat"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentIndexFormatProperty =
            DependencyProperty.Register(nameof(CurrentIndexFormat), typeof(string), typeof(PaginationIndexLabelControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemCountDisplayValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemCountDisplayValueProperty =
            DependencyProperty.Register(nameof(ItemCountDisplayValue), typeof(string), typeof(PaginationIndexLabelControl), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="CurrentIndexDisplayValue"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentIndexDisplayValueProperty =
            DependencyProperty.Register(nameof(CurrentIndexDisplayValue), typeof(string), typeof(PaginationIndexLabelControl), new PropertyMetadata(null));

        /// <summary>
        /// Initializes a new instance of the <see cref="PaginationIndexLabelControl"/> class.
        /// </summary>
        public PaginationIndexLabelControl()
        {
            this.DefaultStyleKey = typeof(PaginationIndexLabelControl);

            this.IsEnabledChanged += this.OnIsEnabledChanged;
        }

        /// <summary>
        /// Gets or sets the string that separates the CurrentIndex and ItemCount values. Defaults to " / ".
        /// </summary>
        public string Separator
        {
            get
            {
                return (string)this.GetValue(SeparatorProperty);
            }
            set
            {
                this.SetValue(SeparatorProperty, value);
            }
        }

        /// <summary>
        ///  Gets or sets the format that defines how the item count part of the control is displayed.
        /// </summary>
        public string ItemCountFormat
        {
            get
            {
                return (string)this.GetValue(ItemCountFormatProperty);
            }
            set
            {
                this.SetValue(ItemCountFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the format that defines how the current index part of the control is displayed.
        /// </summary>
        public string CurrentIndexFormat
        {
            get
            {
                return (string)this.GetValue(CurrentIndexFormatProperty);
            }
            set
            {
                this.SetValue(CurrentIndexFormatProperty, value);
            }
        }

        /// <summary>
        /// Gets the string representation of the current index.
        /// </summary>
        public string CurrentIndexDisplayValue
        {
            get
            {
                return (string)this.GetValue(CurrentIndexDisplayValueProperty);
            }
        }

        /// <summary>
        /// Gets the string representation of the item count.
        /// </summary>
        public string ItemCountDisplayValue
        {
            get
            {
                return (string)this.GetValue(ItemCountDisplayValueProperty);
            }
        }

        internal void SetValues(int count, int index)
        {
            string countFormat = this.ItemCountFormat;
            string countValue;
            if (!string.IsNullOrEmpty(countFormat))
            {
                countValue = string.Format(CultureInfo.CurrentUICulture, countFormat, count);
            }
            else
            {
                countValue = count.ToString(CultureInfo.CurrentUICulture);
            }

            this.SetValue(ItemCountDisplayValueProperty, countValue);

            string indexFormat = this.CurrentIndexFormat;
            string indexValue;
            if (!string.IsNullOrEmpty(indexFormat))
            {
                indexValue = string.Format(CultureInfo.CurrentUICulture, indexFormat, index);
            }
            else
            {
                indexValue = index.ToString(CultureInfo.CurrentUICulture);
            }

            this.SetValue(CurrentIndexDisplayValueProperty, indexValue);
        }

        /// <inheritdoc />
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new PaginationIndexLabelControlAutomationPeer(this);
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            this.UpdateVisualState(true);
        }
    }
}
