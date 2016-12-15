using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Primitives.Scale
{
    internal class AxisLabelModel : Node
    {
        internal static readonly int ContentPropertyKey = PropertyKeys.Register(typeof(AxisLabelModel), "Content");

        internal RadSize desiredSize = RadSize.Empty;

        public object Content
        {
            get
            {
                return this.GetValue(ContentPropertyKey);
            }
            set
            {
                this.SetValue(ContentPropertyKey, value);
            }
        }

        internal override void OnPropertyChanged(RadPropertyEventArgs e)
        {
            if (e != null && e.Key == ContentPropertyKey)
            {
                this.desiredSize = RadSize.Empty;
            }

            base.OnPropertyChanged(e);
        }
    }
}
