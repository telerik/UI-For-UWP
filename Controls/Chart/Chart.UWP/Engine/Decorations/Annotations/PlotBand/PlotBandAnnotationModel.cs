using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class PlotBandAnnotationModel : SingleAxisAnnotationModel
    {
        internal static readonly int FromPropertyKey = PropertyKeys.Register(typeof(PlotBandAnnotationModel), "From", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int ToPropertyKey = PropertyKeys.Register(typeof(PlotBandAnnotationModel), "To", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal object from;
        internal object to;
        internal AxisPlotInfo firstPlotInfo;
        internal AxisPlotInfo secondPlotInfo;
        internal bool isFirstPlotUpdated;
        internal bool isSecondPlotUpdated;

        public override bool IsUpdated
        {
            get
            {
                return this.isFirstPlotUpdated && this.isSecondPlotUpdated;
            }
        }

        public object From
        {
            get
            {
                return this.GetValue(FromPropertyKey);
            }
            set
            {
                this.SetValue(FromPropertyKey, value);
            }
        }

        public object To
        {
            get
            {
                return this.GetValue(ToPropertyKey);
            }
            set
            {
                this.SetValue(ToPropertyKey, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == FromPropertyKey)
            {
                this.from = e.NewValue;

                this.UpdateFirstPlot();
            }
            else if (e.Key == ToPropertyKey)
            {
                this.to = e.NewValue;

                this.UpdateSecondPlot();
            }

            base.OnPropertyChanged(e);
        }

        internal override void ResetState()
        {
            this.isFirstPlotUpdated = false;
            this.isSecondPlotUpdated = false;
        }

        protected override void UpdateCore()
        {
            this.UpdateFirstPlot();
            this.UpdateSecondPlot();
        }

        private void UpdateFirstPlot()
        {
            this.isFirstPlotUpdated = ChartAnnotationModel.TryCreatePlotInfo(this.axis, this.from, out this.firstPlotInfo);
        }

        private void UpdateSecondPlot()
        {
            this.isSecondPlotUpdated = ChartAnnotationModel.TryCreatePlotInfo(this.axis, this.to, out this.secondPlotInfo);
        }
    }
}
