using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class MultipleAxesAnnotationModel : ChartAnnotationModel, IPlotAreaElementModelWithAxes
    {
        internal static readonly int FirstAxisPropertyKey = PropertyKeys.Register(typeof(MultipleAxesAnnotationModel), "FirstAxis", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int SecondAxisPropertyKey = PropertyKeys.Register(typeof(MultipleAxesAnnotationModel), "SecondAxis", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal AxisModel firstAxis;
        internal AxisModel secondAxis;

        public AxisModel FirstAxis
        {
            get
            {
                return this.GetTypedValue<AxisModel>(FirstAxisPropertyKey, null);
            }
            set
            {
                this.SetValue(FirstAxisPropertyKey, value);
            }
        }

        public AxisModel SecondAxis
        {
            get
            {
                return this.GetTypedValue<AxisModel>(SecondAxisPropertyKey, null);
            }
            set
            {
                this.SetValue(SecondAxisPropertyKey, value);
            }
        }

        public virtual void AttachAxis(AxisModel axis, AxisType type)
        {
            if (type == AxisType.First)
            {
                this.FirstAxis = axis;
            }
            else
            {
                this.SecondAxis = axis;
            }
        }

        public void DetachAxis(AxisModel axis)
        {
            if (this.FirstAxis == axis)
            {
                this.FirstAxis = null;
            }
            else if (this.SecondAxis == axis)
            {
                this.SecondAxis = null;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == FirstAxisPropertyKey)
            {
                this.firstAxis = (AxisModel)e.NewValue;

                this.OnFirstAxisChanged();
            }
            else if (e.Key == SecondAxisPropertyKey)
            {
                this.secondAxis = (AxisModel)e.NewValue;

                this.OnSecondAxisChanged();
            }

            base.OnPropertyChanged(e);
        }

        protected virtual void OnFirstAxisChanged()
        {
        }

        protected virtual void OnSecondAxisChanged()
        {
        }
    }
}