using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class CustomAnnotationModel : MultipleAxesAnnotationModel
    {
        internal static readonly int FirstValuePropertyKey = PropertyKeys.Register(typeof(CustomAnnotationModel), "FirstValue", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int SecondValuePropertyKey = PropertyKeys.Register(typeof(CustomAnnotationModel), "SecondValue", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal object firstValue;
        internal object secondValue;
        internal AxisPlotInfo firstPlotInfo;
        internal AxisPlotInfo secondPlotInfo;
        internal bool isFirstPlotUpdated;
        internal bool isSecondPlotUpdated;
        internal RadSize desiredSize;

        public CustomAnnotationModel()
        {
            this.desiredSize = RadSize.Invalid;
        }

        public object FirstValue
        {
            get
            {
                return this.GetValue(FirstValuePropertyKey);
            }
            set
            {
                this.SetValue(FirstValuePropertyKey, value);
            }
        }

        public object SecondValue
        {
            get
            {
                return this.GetValue(SecondValuePropertyKey);
            }
            set
            {
                this.SetValue(SecondValuePropertyKey, value);
            }
        }

        public override bool IsUpdated
        {
            get
            {
                return this.isFirstPlotUpdated && this.isSecondPlotUpdated;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == FirstValuePropertyKey)
            {
                this.firstValue = e.NewValue;

                this.UpdateFirstPlot();
            }
            else if (e.Key == SecondValuePropertyKey)
            {
                this.secondValue = e.NewValue;

                this.UpdateSecondPlot();
            }

            base.OnPropertyChanged(e);
        }

        internal override void ResetState()
        {
            this.isFirstPlotUpdated = false;
            this.isSecondPlotUpdated = false;
        }

        internal RadSize Measure()
        {
            if (this.desiredSize == RadSize.Invalid)
            {
                this.desiredSize = this.Presenter.MeasureContent(this, this);
            }

            return this.desiredSize;
        }

        protected override void UpdateCore()
        {
            this.UpdateFirstPlot();
            this.UpdateSecondPlot();
        }

        protected override void OnFirstAxisChanged()
        {
            base.OnFirstAxisChanged();

            this.UpdateFirstPlot();
        }

        protected override void OnSecondAxisChanged()
        {
            base.OnSecondAxisChanged();

            this.UpdateSecondPlot();
        }

        private void UpdateFirstPlot()
        {
            this.isFirstPlotUpdated = ChartAnnotationModel.TryCreatePlotInfo(this.firstAxis, this.firstValue, out this.firstPlotInfo);
        }

        private void UpdateSecondPlot()
        {
            this.isSecondPlotUpdated = ChartAnnotationModel.TryCreatePlotInfo(this.secondAxis, this.secondValue, out this.secondPlotInfo);
        }
    }
}
