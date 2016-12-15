using Telerik.Data.Core;

namespace Telerik.Data.Core
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
