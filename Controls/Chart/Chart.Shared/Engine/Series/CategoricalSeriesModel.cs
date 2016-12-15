using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class CategoricalSeriesModel : SeriesModelWithAxes<CategoricalDataPoint>, ISupportCombineMode
    {
        internal static readonly int CombineModePropertyKey = PropertyKeys.Register(typeof(CategoricalSeriesModel), "CombineMode", ChartAreaInvalidateFlags.All);
        internal static readonly int StackGroupKeyPropertyKey = PropertyKeys.Register(typeof(CategoricalSeriesModel), "StackGroupKey", ChartAreaInvalidateFlags.All);

        public CategoricalSeriesModel()
        {
            this.TrackPropertyChanged = true;
        }

        /// <summary>
        /// Gets or sets the <see cref="ChartSeriesCombineMode"/> value that specifies whether this instance should be combined with other instances of same type.
        /// </summary>
        public ChartSeriesCombineMode CombineMode
        {
            get
            {
                return this.GetTypedValue<ChartSeriesCombineMode>(CombineModePropertyKey, ChartSeriesCombineMode.None);
            }
            set
            {
                this.SetValue(CombineModePropertyKey, value);
            }
        }

        /// <summary>
        /// Gets or sets the key that identifies the stack this instance should be put into.
        /// </summary>
        public object StackGroupKey
        {
            get
            {
                return this.GetValue(StackGroupKeyPropertyKey);
            }
            set
            {
                this.SetValue(StackGroupKeyPropertyKey, value);
            }
        }

        internal override AxisPlotMode DefaultPlotMode
        {
            get
            {
                return AxisPlotMode.BetweenTicks;
            }
        }

        internal override ModifyChildrenResult CanAddChild(Node child)
        {
            if (child is CategoricalDataPoint)
            {
                return ModifyChildrenResult.Accept;
            }

            return base.CanAddChild(child);
        }
    }
}
