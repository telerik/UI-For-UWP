using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class DelegateAggregateDescription : PropertyAggregateDescriptionBase
    {
        protected override void CloneOverride(Cloneable source)
        {
        }

        protected override Cloneable CreateInstanceCore()
        {
            return new DelegateAggregateDescription();
        }
    }
}
