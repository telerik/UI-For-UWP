using System.ComponentModel;

namespace Telerik.UI.Xaml.Controls.Chart
{
    internal abstract class ValueIndicatorDataSourceBase : CategoricalSeriesDataSource
    {
        /// <summary>
        /// Called when a property of a bound object changes.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="System.ComponentModel.PropertyChangedEventArgs" /> instance containing the event data.</param>
        protected override void OnBoundItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var propertyNameValueBinding = (this.Owner as ValueIndicatorBase).ValueBinding as PropertyNameDataPointBinding;
            var propertyNameCategoryBinding = (this.Owner as ValueIndicatorBase).CategoryBinding as PropertyNameDataPointBinding;

            if (propertyNameValueBinding != null && propertyNameCategoryBinding != null &&
                e.PropertyName != propertyNameValueBinding.PropertyName && e.PropertyName != propertyNameCategoryBinding.PropertyName)
            {
                return;
            }

            base.OnBoundItemPropertyChanged(sender, e);
        }
    }
}
