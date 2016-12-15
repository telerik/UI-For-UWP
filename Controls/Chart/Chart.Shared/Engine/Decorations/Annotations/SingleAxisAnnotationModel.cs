using System;
using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class SingleAxisAnnotationModel : ChartAnnotationModel, IStrokedAnnotationModel
    {
        internal static readonly int AxisPropertyKey = PropertyKeys.Register(typeof(SingleAxisAnnotationModel), "Axis", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int StrokeThicknessPropertyKey = PropertyKeys.Register(typeof(SingleAxisAnnotationModel), "StrokeThickness", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal static readonly int DashPatternLengthPropertyKey = PropertyKeys.Register(typeof(SingleAxisAnnotationModel), "DashPatternLength", ChartAreaInvalidateFlags.InvalidateAnnotations);

        internal AxisModel axis;

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

        public AxisModel Axis
        {
            get
            {
                return this.GetTypedValue<AxisModel>(AxisPropertyKey, null);
            }
            set
            {
                this.SetValue(AxisPropertyKey, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == AxisPropertyKey)
            {
                this.axis = (AxisModel)e.NewValue;

                this.UpdateCore();
            }

            base.OnPropertyChanged(e);
        }
    }
}