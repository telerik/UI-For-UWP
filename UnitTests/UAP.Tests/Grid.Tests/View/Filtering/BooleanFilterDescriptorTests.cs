using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class BooleanFilterDescriptorTests
    {
        [TestMethod]
        public void Test_BooleanValue()
        {
            BooleanFilterDescriptor descriptor = new BooleanFilterDescriptor();
            descriptor.Value = true;
            descriptor.MemberAccess = new ValueMemberAccess();

            var model = new BooleanViewModel() { Value = true };
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = false;
            Assert.IsFalse(descriptor.PassesFilter(model));

            model.Value = false;
            Assert.IsTrue(descriptor.PassesFilter(model));
        }

        [TestMethod]
        public void Test_ConvertValue_FromString()
        {
            BooleanFilterDescriptor descriptor = new BooleanFilterDescriptor();
            descriptor.Value = "True";
            descriptor.MemberAccess = new ValueMemberAccess();

            var model = new BooleanViewModel() { Value = true };
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = "false";
            Assert.IsFalse(descriptor.PassesFilter(model));

            model.Value = false;
            Assert.IsTrue(descriptor.PassesFilter(model));
        }

        [TestMethod]
        public void Test_NullableBooleanValue()
        {
            BooleanFilterDescriptor descriptor = new BooleanFilterDescriptor();
            descriptor.MemberAccess = new NullableValueMemberAccess();

            var model = new BooleanViewModel() { NullableValue = null };
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = true;
            Assert.IsFalse (descriptor.PassesFilter(model));

            model.NullableValue = new Nullable<bool>(true);
            Assert.IsTrue(descriptor.PassesFilter(model));
        }

        private class ValueMemberAccess : IMemberAccess
        {
            public object GetValue(object item)
            {
                BooleanViewModel model = item as BooleanViewModel;
                return model.Value;
            }

            public void SetValue(object item, object fieldValue)
            {
                throw new NotImplementedException();
            }
        }

        private class NullableValueMemberAccess : IMemberAccess
        {
            public object GetValue(object item)
            {
                BooleanViewModel model = item as BooleanViewModel;
                return model.NullableValue;
            }

            public void SetValue(object item, object fieldValue)
            {
                throw new NotImplementedException();
            }
        }

        public class BooleanViewModel
        {
            public bool Value
            {
                get;
                set;
            }

            public bool? NullableValue
            {
                get;
                set;
            }
        }
    }
}
