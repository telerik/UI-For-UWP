using Telerik.Core;

namespace Telerik.Charting
{
    internal abstract class GridLineAnnotationModel : SingleAxisAnnotationModel
    {
        internal static readonly int ValuePropertyKey = PropertyKeys.Register(typeof(GridLineAnnotationModel), "Value", ChartAreaInvalidateFlags.InvalidateAnnotations);
        internal AxisPlotInfo plotInfo;
        internal bool isUpdated;
        internal object value;

        public override bool IsUpdated
        {
            get
            {
                return this.isUpdated;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1500:VariableNamesShouldNotMatchFieldNames", MessageId = "value")]
        public object Value
        {
            get
            {
                return this.GetValue(ValuePropertyKey);
            }
            set
            {
                this.SetValue(ValuePropertyKey, value);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0")]
        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            // update local value first and then call base to raise the PropertyChanged event (if needed)
            if (e.Key == ValuePropertyKey)
            {
                this.value = e.NewValue;
                
                this.UpdateCore();
            }

            base.OnPropertyChanged(e);
        }

        internal override void ResetState()
        {
            this.isUpdated = false;
        }

        protected override void UpdateCore()
        {
            this.isUpdated = ChartAnnotationModel.TryCreatePlotInfo(this.axis, this.value, out this.plotInfo);
        }
    }
}
