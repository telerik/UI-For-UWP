using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Grid.Primitives;
using Telerik.UI.Xaml.Controls.Input;
using Windows.UI.Xaml;

namespace SDKExamples.UWP.DataGrid
{
    [TemplatePart(Name = "PART_Rating", Type = typeof(RadRating))]
    public sealed class DataGridCustomFilteringControl : DataGridFilterControlBase
    {
        private RadRating rating;

        public DataGridCustomFilteringControl()
        {
            this.DefaultStyleKey = typeof(DataGridCustomFilteringControl);
        }

        public string PropertyName
        {
            get;
            set;
        }

        public override FilterDescriptorBase BuildDescriptor()
        {
            var descriptor = new NumericalFilterDescriptor();
            descriptor.PropertyName = this.PropertyName;
            descriptor.Value = this.rating.Value;

            return descriptor;
        }

        protected override bool ApplyTemplateCore()
        {
            bool applied = base.ApplyTemplateCore();

            this.rating = this.GetTemplateChild("PART_Rating") as RadRating;
            applied = applied && this.rating != null;

            return applied;
        }

        protected override void Initialize()
        {
            var descriptor = this.ActualAssociatedDescriptor as NumericalFilterDescriptor;
            if (descriptor != null)
            {
                double value;
                if (double.TryParse(descriptor.Value.ToString(), out value))
                {
                    this.rating.Value = value;
                }
            }
        }
    }
}
