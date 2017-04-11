using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class TextFilterDescriptorTests
    {
        [TestMethod]
        public void Test_Operator_EqualsTo()
        {
            TextFilterDescriptor descriptor = new TextFilterDescriptor();
            descriptor.Operator = TextOperator.EqualsTo;
            descriptor.Value = "Sofia";
            descriptor.MemberAccess = new CityMemberAccess();

            var viewModel = new TestViewModel() { City = "Sofia", Name = "Tsvyatko" };
            Assert.IsTrue(descriptor.PassesFilter(viewModel));

            // test case-sensitivity
            viewModel.City = "sofia";
            Assert.IsFalse(descriptor.PassesFilter(viewModel));

            descriptor.IsCaseSensitive = false;
            Assert.IsTrue(descriptor.PassesFilter(viewModel));
        }

        [TestMethod]
        public void Test_Operator_DoesNotEqualTo()
        {
            TextFilterDescriptor descriptor = new TextFilterDescriptor();
            descriptor.Operator = TextOperator.DoesNotEqualTo;
            descriptor.Value = "Sofia";
            descriptor.MemberAccess = new CityMemberAccess();

            var viewModel = new TestViewModel() { City = "Sofia", Name = "Tsvyatko" };
            Assert.IsFalse(descriptor.PassesFilter(viewModel));
        }

        [TestMethod]
        public void Test_Operator_Contains()
        {
            TextFilterDescriptor descriptor = new TextFilterDescriptor();
            descriptor.Operator = TextOperator.Contains;
            descriptor.Value = "fi";
            descriptor.MemberAccess = new CityMemberAccess();

            var viewModel = new TestViewModel() { City = "Sofia", Name = "Tsvyatko" };
            Assert.IsTrue(descriptor.PassesFilter(viewModel));

            descriptor.Value = "ffi";
            Assert.IsFalse(descriptor.PassesFilter(viewModel));
        }

        [TestMethod]
        public void Test_Operator_DoesNotContain()
        {
            TextFilterDescriptor descriptor = new TextFilterDescriptor();
            descriptor.Operator = TextOperator.DoesNotContain;
            descriptor.Value = "Sof";
            descriptor.MemberAccess = new CityMemberAccess();

            var viewModel = new TestViewModel() { City = "Sofia", Name = "Tsvyatko" };
            Assert.IsFalse(descriptor.PassesFilter(viewModel));

            descriptor.Value = "Soff";
            Assert.IsTrue(descriptor.PassesFilter(viewModel));
        }

        [TestMethod]
        public void Test_Operator_StartsWith()
        {
            TextFilterDescriptor descriptor = new TextFilterDescriptor();
            descriptor.Operator = TextOperator.StartsWith;
            descriptor.Value = "Sof";
            descriptor.MemberAccess = new CityMemberAccess();

            var viewModel = new TestViewModel() { City = "Sofia", Name = "Tsvyatko" };
            Assert.IsTrue(descriptor.PassesFilter(viewModel));

            descriptor.Value = "Soff";
            Assert.IsFalse(descriptor.PassesFilter(viewModel));
        }

        [TestMethod]
        public void Test_Operator_EndsWith()
        {
            TextFilterDescriptor descriptor = new TextFilterDescriptor();
            descriptor.Operator = TextOperator.EndsWith;
            descriptor.Value = "fia";
            descriptor.MemberAccess = new CityMemberAccess();

            var viewModel = new TestViewModel() { City = "Sofia", Name = "Tsvyatko" };
            Assert.IsTrue(descriptor.PassesFilter(viewModel));

            descriptor.Value = "fi";
            Assert.IsFalse(descriptor.PassesFilter(viewModel));
        }

        [TestMethod]
        public void Test_NullValue()
        {
            TextFilterDescriptor descriptor = new TextFilterDescriptor();
            descriptor.Operator = TextOperator.StartsWith;
            descriptor.Value = "Sof";
            descriptor.MemberAccess = new CityMemberAccess();

            var viewModel = new TestViewModel() { City = null, Name = "Tsvyatko" };
            Assert.IsFalse(descriptor.PassesFilter(viewModel));

            descriptor.Value = null;
            Assert.IsTrue(descriptor.PassesFilter(viewModel));
        }

        private class CityMemberAccess : IMemberAccess
        {
            public object GetValue(object item)
            {
                var model = item as TestViewModel;
                return model.City;
            }

            public void SetValue(object item, object fieldValue)
            {
                throw new NotImplementedException();
            }
        }
    }
}
