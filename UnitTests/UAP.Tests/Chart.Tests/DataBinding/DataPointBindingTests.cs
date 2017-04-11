using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Dynamic;

namespace Telerik.UI.Xaml.Controls.Chart.Tests
{
    [TestClass]
    public class DataPointBindingTests
    {
        [TestMethod]
        public void Test_PropertyNameBinding()
        {
            PropertyNameDataPointBinding binding = new PropertyNameDataPointBinding();
            binding.PropertyName = "Value";

            BusinessObject obj = new BusinessObject() { Value = 1 };
            Assert.AreEqual<double>(1, (double)binding.GetValue(obj), "Value not looked-up correctly");
        }

        [TestMethod]
        public void Test_PropertyNameBinding_ExpandoObject()
        {
            PropertyNameDataPointBinding binding = new PropertyNameDataPointBinding();
            binding.PropertyName = "Value";

            dynamic obj = new ExpandoObject();
            obj.Value = 1;

            Assert.AreEqual<double>(1, (double)binding.GetValue(obj), "ExpandoObject Value not looked-up correctly");
        }

        [TestMethod]
        public void Test_PropertyNameBinding_DynamicObject()
        {
            PropertyNameDataPointBinding binding = new PropertyNameDataPointBinding();
            binding.PropertyName = "Value";

            dynamic obj = new MyDynamicObject();
            obj.Value = 1;

            Assert.AreEqual<double>(1, (double)binding.GetValue(obj), "DynamicObject Value not looked-up correctly");
        }

        [TestMethod]
        public void Test_PropertyNameBinding_EmptyPropertyName()
        {
            Assert.ThrowsException<ArgumentException>(() => {
                PropertyNameDataPointBinding binding = new PropertyNameDataPointBinding();
                binding.PropertyName = null;
            });
        }

        [TestMethod]
        public void Test_PropertyNameBinding_MissingProperty()
        {
            Assert.ThrowsException<NullReferenceException>(() => {
                PropertyNameDataPointBinding binding = new PropertyNameDataPointBinding();
                binding.PropertyName = "Wrong";
                binding.GetValue(new BusinessObject());
            });
        }

        [TestMethod]
        public void Test_GenericBinding()
        {
            GenericDataPointBinding<BusinessObject, double> binding = new GenericDataPointBinding<BusinessObject, double>();
            binding.ValueSelector = (o) => o.Value.Value;

            BusinessObject obj = new BusinessObject() { Value = 1 };
            Assert.AreEqual<double>(1, (double)binding.GetValue(obj), "Value not looked-up correctly");
        }

        [TestMethod]
        public void Test_GenericBinding_NullValueSelector()
        {
            Assert.ThrowsException<ArgumentNullException>(() => {
                GenericDataPointBinding<BusinessObject, double> binding = new GenericDataPointBinding<BusinessObject, double>();
                binding.ValueSelector = null;
            });
        }

        [TestMethod]
        public void Test_GenericBinding_UnspecifiedValueSelector()
        {
            Assert.ThrowsException<InvalidOperationException>(() => {
                GenericDataPointBinding<int, int> binding = new GenericDataPointBinding<int, int>();
                binding.GetValue(0);
            });
        }

        [TestMethod]
        public void Test_GenericBinding_UnexpectedInstance()
        {
            Assert.ThrowsException<ArgumentNullException>(() => {
                GenericDataPointBinding<BusinessObject, double> binding = new GenericDataPointBinding<BusinessObject, double>();
                binding.ValueSelector = (o) => o.Value.Value;
                binding.GetValue(null);
            });
        }

        [TestMethod]
        public void Test_PropertyNameBinding_PropertyChanged()
        {
            bool propertyChangedFired = false;

            PropertyNameDataPointBinding binding = new PropertyNameDataPointBinding();
            binding.PropertyChanged += (sender, args) =>
                {
                    propertyChangedFired = true;
                    Assert.AreEqual<string>("PropertyName", args.PropertyName);
                };

            binding.PropertyName = "Value";

            Assert.IsTrue(propertyChangedFired);
        }

        [TestMethod]
        public void Test_GenericBinding_PropertyChanged()
        {
            GenericDataPointBinding<BusinessObject, double> binding = new GenericDataPointBinding<BusinessObject, double>();
            
            bool propertyChangedCalled = false;
            binding.PropertyChanged += (sender, args) => 
                {
                    propertyChangedCalled = true;
                    Assert.AreEqual<string>("ValueSelector", args.PropertyName);
                };

            binding.ValueSelector = (o) => o.Value.Value;

            Assert.IsTrue(propertyChangedCalled);
        }
    }

    public class MyDynamicObject : DynamicObject
    {
        private Dictionary<string, object> propertyBag = new Dictionary<string, object>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return this.propertyBag.TryGetValue(binder.Name, out result);
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            this.propertyBag[binder.Name] = value;
            return true;
        }
    }
}
