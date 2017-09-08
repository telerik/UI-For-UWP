using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class DateTimeFilterDescriptorTests
    {
        private static readonly DateTime DefaultDate = new DateTime(2013, 1, 1);
        private static readonly TimeSpan DefaultTimeSpan = TimeSpan.FromHours(1);

        [TestMethod]
        public void Test_Operator_EqualsTo()
        {
            var descriptor = new DateTimeFilterDescriptor();
            descriptor.Value = DefaultDate;

            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            var model = this.GetItems(1).First();

            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1);
            Assert.IsFalse(descriptor.PassesFilter(model));

            // DateTimeOffset
            descriptor.Value = new DateTimeOffset(DefaultDate, DefaultTimeSpan);
            memberAccess.IsDateOffset = true;
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate;
            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours - 1);
            Assert.IsTrue(descriptor.PassesFilter(model));
        }

        [TestMethod]
        public void Test_Operator_DoesNotEqualTo()
        {
            var descriptor = new DateTimeFilterDescriptor();
            descriptor.Value = DefaultDate;
            descriptor.Operator = NumericalOperator.DoesNotEqualTo;

            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            var model = this.GetItems(1).First();

            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1);
            Assert.IsTrue(descriptor.PassesFilter(model));

            // DateTimeOffset
            descriptor.Value = new DateTimeOffset(DefaultDate, DefaultTimeSpan);
            memberAccess.IsDateOffset = true;
            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate;
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours - 1);
            Assert.IsFalse(descriptor.PassesFilter(model));
        }

        [TestMethod]
        public void Test_Operator_IsGreaterThan()
        {
            var descriptor = new DateTimeFilterDescriptor();
            descriptor.Value = DefaultDate;
            descriptor.Operator = NumericalOperator.IsGreaterThan;

            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            var model = new ViewModel() 
            {
                Date = DefaultDate.AddDays(1),
                DateOffset = new DateTimeOffset(DefaultDate.AddDays(1), DefaultTimeSpan)
            };

            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1);
            Assert.IsFalse(descriptor.PassesFilter(model));

            // DateTimeOffset
            descriptor.Value = new DateTimeOffset(DefaultDate, DefaultTimeSpan);
            memberAccess.IsDateOffset = true;
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate;
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1).AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours - 1);
            Assert.IsFalse(descriptor.PassesFilter(model));
        }

        [TestMethod]
        public void Test_Operator_IsGreaterThanOrEqualTo()
        {
            var descriptor = new DateTimeFilterDescriptor();
            descriptor.Value = DefaultDate;
            descriptor.Operator = NumericalOperator.IsGreaterThanOrEqualTo;

            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            var model = new ViewModel()
            {
                Date = DefaultDate.AddDays(1),
                DateOffset = new DateTimeOffset(DefaultDate.AddDays(1), DefaultTimeSpan)
            };

            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1);
            Assert.IsTrue(descriptor.PassesFilter(model));

            // DateTimeOffset
            descriptor.Value = new DateTimeOffset(DefaultDate, DefaultTimeSpan);
            memberAccess.IsDateOffset = true;
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate;
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1).AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours - 1);
            Assert.IsTrue(descriptor.PassesFilter(model));
        }

                
        [TestMethod]
        public void Test_Operator_IsLessThan()
        {
            var descriptor = new DateTimeFilterDescriptor();
            descriptor.Value = DefaultDate;
            descriptor.Operator = NumericalOperator.IsLessThan;

            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            var model = new ViewModel()
            {
                Date = DefaultDate.AddDays(1),
                DateOffset = new DateTimeOffset(DefaultDate.AddDays(1), DefaultTimeSpan)
            };

            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1);
            Assert.IsFalse(descriptor.PassesFilter(model));

            // DateTimeOffset
            descriptor.Value = new DateTimeOffset(DefaultDate, DefaultTimeSpan);
            memberAccess.IsDateOffset = true;
            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate;
            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1).AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours - 1);
            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(2);
            Assert.IsTrue(descriptor.PassesFilter(model));
        }

        [TestMethod]
        public void Test_Operator_IsLessThanOrEqualTo()
        {
            var descriptor = new DateTimeFilterDescriptor();
            descriptor.Value = DefaultDate;
            descriptor.Operator = NumericalOperator.IsLessThanOrEqualTo;

            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            var model = new ViewModel()
            {
                Date = DefaultDate.AddDays(1),
                DateOffset = new DateTimeOffset(DefaultDate.AddDays(1), DefaultTimeSpan)
            };

            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1);
            Assert.IsTrue(descriptor.PassesFilter(model));

            // DateTimeOffset
            descriptor.Value = new DateTimeOffset(DefaultDate, DefaultTimeSpan);
            memberAccess.IsDateOffset = true;
            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate;
            Assert.IsFalse(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(1).AddHours(TimeZoneInfo.Local.BaseUtcOffset.Hours - 1);
            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = DefaultDate.AddDays(2);
            Assert.IsTrue(descriptor.PassesFilter(model));
        }

        [TestMethod]
        public void Test_ConvertValue_FromString()
        {
            var descriptor = new DateTimeFilterDescriptor();
            descriptor.Value = "1/1/2010";

            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            var model = new ViewModel() { Date = new DateTime(2010, 1, 1) };

            Assert.IsTrue(descriptor.PassesFilter(model));

            descriptor.Value = "1/2/2010";
            Assert.IsFalse(descriptor.PassesFilter(model));
        }

        private IEnumerable<ViewModel> GetItems(int count)
        {
            DateTime date = DefaultDate;

            for (int i = 0; i < count; i++)
            {
                var model = new ViewModel();
                model.Date = date;
                model.DateOffset = new DateTimeOffset(date, DefaultTimeSpan);

                yield return model;

                date = date.AddDays(1);
            }
        }

        private class ViewModelMemberAccess : IMemberAccess
        {
            public bool IsDateOffset
            {
                get;
                set;
            }

            public object GetValue(object item)
            {
                var model = item as ViewModel;
                if (this.IsDateOffset)
                {
                    return model.DateOffset;
                }

                return model.Date;
            }

            public void SetValue(object item, object fieldValue)
            {
                throw new NotImplementedException();
            }
        }

        private class ViewModel
        {
            public DateTime Date
            {
                get;
                set;
            }

            public DateTimeOffset DateOffset
            {
                get;
                set;
            }
        }
    }
}
