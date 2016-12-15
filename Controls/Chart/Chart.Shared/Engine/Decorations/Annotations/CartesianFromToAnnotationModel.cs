using System;
using System.Linq;
using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class CartesianFromToAnnotationModel : MultipleAxesAnnotationModel, IStrokedAnnotationModel
    {
        internal static readonly int HorizontalFromPropertyKey = PropertyKeys.Register(typeof(GridLineAnnotationModel), "HorizontalFrom", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int HorizontalToPropertyKey = PropertyKeys.Register(typeof(GridLineAnnotationModel), "HorizontalTo", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int VerticalFromPropertyKey = PropertyKeys.Register(typeof(GridLineAnnotationModel), "VerticalFrom", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int VerticalToPropertyKey = PropertyKeys.Register(typeof(GridLineAnnotationModel), "VerticalTo", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int StrokeThicknessPropertyKey = PropertyKeys.Register(typeof(GridLineAnnotationModel), "StrokeThickness", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int DashPatternLengthPropertyKey = PropertyKeys.Register(typeof(GridLineAnnotationModel), "DashPatternLength", ChartAreaInvalidateFlags.InvalidateAnnotations);
      
        internal AxisPlotInfo horizontalFromPlotInfo;
        internal AxisPlotInfo horizontalToPlotInfo;
        internal AxisPlotInfo verticalFromPlotInfo;
        internal AxisPlotInfo verticalToPlotInfo;

        private object horizontalFrom;
        private object horizontalTo;
        private object verticalFrom;
        private object verticalTo;

        private bool isHorizontalFromPlotValid;
        private bool isHorizontalToPlotValid;
        private bool isVerticalFromPlotValid;
        private bool isVerticalToPlotValid;

        public double StrokeThickness
        {
            get
            {
                return this.GetTypedValue<double>(StrokeThicknessPropertyKey, 0d);
            }
            set
            {
                this.SetValue(StrokeThicknessPropertyKey, value);
            }
        }

        public double DashPatternLength
        {
            get
            {
                return this.GetTypedValue<double>(DashPatternLengthPropertyKey, 0d);
            }
            set
            {
                this.SetValue(DashPatternLengthPropertyKey, value);
            }
        }

        public override bool IsUpdated
        {
            get
            {
                return this.isHorizontalFromPlotValid && this.isHorizontalToPlotValid && this.isVerticalFromPlotValid && this.isVerticalToPlotValid;
            }
        }

        public object HorizontalFrom
        {
            get
            {
                return this.GetValue(HorizontalFromPropertyKey);
            }
            set
            {
                this.SetValue(HorizontalFromPropertyKey, value);
            }
        }

        public object HorizontalTo
        {
            get
            {
                return this.GetValue(HorizontalToPropertyKey);
            }
            set
            {
                this.SetValue(HorizontalToPropertyKey, value);
            }
        }

        public object VerticalFrom
        {
            get
            {
                return this.GetValue(VerticalFromPropertyKey);
            }
            set
            {
                this.SetValue(VerticalFromPropertyKey, value);
            }
        }

        public object VerticalTo
        {
            get
            {
                return this.GetValue(VerticalToPropertyKey);
            }
            set
            {
                this.SetValue(VerticalToPropertyKey, value);
            }
        }

        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (e == null)
            {
                return;
            }

            if (e.Key == HorizontalFromPropertyKey)
            {
                this.horizontalFrom = e.NewValue;
                this.UpdateX1Plot();
            }
            if (e.Key == HorizontalToPropertyKey)
            {
                this.horizontalTo = e.NewValue;
                this.UpdateX2Plot();
            }
            if (e.Key == VerticalFromPropertyKey)
            {
                this.verticalFrom = e.NewValue;
                this.UpdateY1Plot();
            }
            if (e.Key == VerticalToPropertyKey)
            {
                this.verticalTo = e.NewValue;
                this.UpdateY2Plot();
            }

            base.OnPropertyChanged(e);
        }

        internal override void ResetState()
        {
            this.isHorizontalFromPlotValid = false;
            this.isHorizontalToPlotValid = false;
            this.isVerticalFromPlotValid = false;
            this.isVerticalToPlotValid = false;
        }

        protected override void UpdateCore()
        {
            this.UpdateX1Plot();
            this.UpdateX2Plot();
            this.UpdateY1Plot();
            this.UpdateY2Plot();
        }

        protected override void OnFirstAxisChanged()
        {
            this.UpdateX1Plot();
            this.UpdateX2Plot();
        }

        protected override void OnSecondAxisChanged()
        {
            this.UpdateY1Plot();
            this.UpdateY2Plot();
        }

        private void UpdateX1Plot()
        {
            this.isHorizontalFromPlotValid = ChartAnnotationModel.TryCreatePlotInfo(this.firstAxis, this.horizontalFrom, out this.horizontalFromPlotInfo);
        }

        private void UpdateX2Plot()
        {
            this.isHorizontalToPlotValid = ChartAnnotationModel.TryCreatePlotInfo(this.firstAxis, this.horizontalTo, out this.horizontalToPlotInfo);
        }

        private void UpdateY1Plot()
        {
            this.isVerticalFromPlotValid = ChartAnnotationModel.TryCreatePlotInfo(this.secondAxis, this.verticalFrom, out this.verticalFromPlotInfo);
        }

        private void UpdateY2Plot()
        {
            this.isVerticalToPlotValid = ChartAnnotationModel.TryCreatePlotInfo(this.secondAxis, this.verticalTo, out this.verticalToPlotInfo);
        }
    }
}