using Telerik.Core;

namespace Telerik.Charting
{
    public class TestPropertyBagObject : PropertyBagObject
    {
        internal static readonly int TestDoublePropertyKey = PropertyKeys.Register(typeof(TestPropertyBagObject), "TestDouble");
    }
}
