using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class NumericalFilterDescriptorTests
    {
        [TestMethod]
        public void Test_Operator_EqualsTo()
        {
            var descriptor = new NumericalFilterDescriptor();
            descriptor.Operator = NumericalOperator.EqualsTo;

            var model = CreateViewModel(10);
            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            foreach (object value in this.GetNumericValues(10))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(5))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsFalse(descriptor.PassesFilter(model));
                }
            }
        }

        [TestMethod]
        public void Test_Operator_DoesNotEqualTo()
        {
            var descriptor = new NumericalFilterDescriptor();
            descriptor.Operator = NumericalOperator.DoesNotEqualTo;

            var model = CreateViewModel(10);
            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            foreach (object value in this.GetNumericValues(5))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(10))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsFalse(descriptor.PassesFilter(model));
                }
            }
        }

        [TestMethod]
        public void Test_Operator_IsGreaterThan()
        {
            var descriptor = new NumericalFilterDescriptor();
            descriptor.Operator = NumericalOperator.IsGreaterThan;

            var model = CreateViewModel(10);
            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            foreach (object value in this.GetNumericValues(8))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(10))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsFalse(descriptor.PassesFilter(model));
                }
            }
        }

        [TestMethod]
        public void Test_Operator_IsGreaterThanOrEqualTo()
        {
            var descriptor = new NumericalFilterDescriptor();
            descriptor.Operator = NumericalOperator.IsGreaterThanOrEqualTo;

            var model = CreateViewModel(10);
            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            foreach (object value in this.GetNumericValues(8))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(10))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(11))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsFalse(descriptor.PassesFilter(model));
                }
            }
        }

        [TestMethod]
        public void Test_Operator_IsLessThan()
        {
            var descriptor = new NumericalFilterDescriptor();
            descriptor.Operator = NumericalOperator.IsLessThan;

            var model = CreateViewModel(10);
            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            foreach (object value in this.GetNumericValues(8))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsFalse(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(10))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsFalse(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(12))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }
        }

        [TestMethod]
        public void Test_Operator_IsLessThanOrEqualTo()
        {
            var descriptor = new NumericalFilterDescriptor();
            descriptor.Operator = NumericalOperator.IsLessThanOrEqualTo;

            var model = CreateViewModel(10);
            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            foreach (object value in this.GetNumericValues(8))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsFalse(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(10))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(12))
            {
                descriptor.Value = value;
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }
        }

        [TestMethod]
        public void Test_ConvertValue_FromString()
        {
            var descriptor = new NumericalFilterDescriptor();
            descriptor.Operator = NumericalOperator.IsLessThanOrEqualTo;

            var model = CreateViewModel(10);
            var memberAccess = new ViewModelMemberAccess();
            descriptor.MemberAccess = memberAccess;

            foreach (object value in this.GetNumericValues(8.5))
            {
                descriptor.Value = value.ToString();
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsFalse(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(10))
            {
                descriptor.Value = value.ToString();
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }

            foreach (object value in this.GetNumericValues(12.22))
            {
                descriptor.Value = value.ToString();
                foreach (NumericalType type in Enum.GetValues(typeof(NumericalType)))
                {
                    memberAccess.Type = type;
                    Assert.IsTrue(descriptor.PassesFilter(model));
                }
            }
        }

        private ViewModel CreateViewModel(double value)
        {
            ViewModel model = new ViewModel();

            model.ByteValue = (byte)value;
            model.SByteValue = (sbyte)value;
            model.ShortValue = (short)value;
            model.UShortValue = (ushort)value;
            model.IntValue = (int)value;
            model.UIntValue = (uint)value;
            model.LongValue = (long)value;
            model.ULongValue = (ulong)value;
            model.FloatValue = (float)value;
            model.DoubleValue = (double)value;
            model.DecimalValue = (decimal)value;

            return model;
        }

        private IEnumerable GetNumericValues(double value)
        {
            yield return (byte)value;
            yield return (sbyte)value;
            yield return (short)value;
            yield return (ushort)value;
            yield return (int)value;
            yield return (uint)value;
            yield return (long)value;
            yield return (ulong)value;
            yield return (float)value;
            yield return (double)value;
            yield return (decimal)value;
        }

        private IEnumerable<ViewModel> GetItemsSource(int count)
        {
            for (int i = 0; i < count; i++)
            {
                yield return CreateViewModel(i);
            }
        }

        private enum NumericalType
        {
            Byte,
            SByte,
            Short,
            UShort,
            Int,
            UInt,
            Long,
            ULong,
            Float,
            Double,
            Decimal
        }

        private class ViewModelMemberAccess : IMemberAccess
        {
            public NumericalType Type
            {
                get;
                set;
            }

            public object GetValue(object item)
            {
                var model = item as ViewModel;
                object value = null;

                switch (this.Type)
                {
                    case NumericalType.Byte:
                        value = model.ByteValue;
                        break;
                    case NumericalType.SByte:
                        value = model.SByteValue;
                        break;
                    case NumericalType.Short:
                        value = model.ShortValue;
                        break;
                    case NumericalType.UShort:
                        value = model.UShortValue;
                        break;
                    case NumericalType.Int:
                        value = model.IntValue;
                        break;
                    case NumericalType.UInt:
                        value = model.UIntValue;
                        break;
                    case NumericalType.Long:
                        value = model.LongValue;
                        break;
                    case NumericalType.ULong:
                        value = model.ULongValue;
                        break;
                    case NumericalType.Float:
                        value = model.FloatValue;
                        break;
                    case NumericalType.Double:
                        value = model.DoubleValue;
                        break;
                    case NumericalType.Decimal:
                        value = model.DecimalValue;
                        break;
                }

                return value;
            }

            public void SetValue(object item, object fieldValue)
            {
                throw new NotImplementedException();
            }
        }

        private class ViewModel
        {
            public byte ByteValue
            {
                get;
                set;
            }

            public sbyte SByteValue
            {
                get;
                set;
            }

            public short ShortValue
            {
                get;
                set;
            }

            public ushort UShortValue
            {
                get;
                set;
            }

            public int IntValue
            {
                get;
                set;
            }

            public uint UIntValue
            {
                get;
                set;
            }

            public long LongValue
            {
                get;
                set;
            }

            public ulong ULongValue
            {
                get;
                set;
            }

            public float FloatValue
            {
                get;
                set;
            }

            public double DoubleValue
            {
                get;
                set;
            }

            public decimal DecimalValue
            {
                get;
                set;
            }
        }
    }
}
