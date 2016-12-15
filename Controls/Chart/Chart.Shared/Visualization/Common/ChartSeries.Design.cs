using System;

namespace Telerik.UI.Xaml.Controls.Chart
{
    public partial class ChartSeries
    {
        private static readonly int[] SampleData = new int[] { 5, 10, 30, 20, 25 };

        // TODO: Consider real design-time data, with notation for different series types - currently financial series raise exception.
        // TODO: Additionally, having such dummy data initialized under the hood at design-time may bring additional confusion to the user.
        ////partial void PrepareDesignTimeData()
        ////{
        ////    if (this.ItemsSource == null && this.Model.DataPointsInternal.Count == 0)
        ////    {
        ////        this.ItemsSource = SampleData;
        ////    }
        ////}
    }
}
