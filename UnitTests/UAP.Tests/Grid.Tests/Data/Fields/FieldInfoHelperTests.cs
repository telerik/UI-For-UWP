using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class FieldInfoHelperTests
    {
        [TestMethod]
        public void GetRoleForType_WhenTypeIsSingle_ReturnsValueRole()
        {
            var type = typeof(Single);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsDouble_ReturnsValueRole()
        {
            var type = typeof(Double);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsDecimal_ReturnsValueRole()
        {
            var type = typeof(Decimal);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsInt16_ReturnsValueRole()
        {
            var type = typeof(Int16);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsInt32_ReturnsValueRole()
        {
            var type = typeof(Int32);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsInt64_ReturnsValueRole()
        {
            var type = typeof(Int64);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsByte_ReturnsValueRole()
        {
            var type = typeof(byte);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsSByte_ReturnsValueRole()
        {
            var type = typeof(sbyte);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsUInt32_ReturnsValueRole()
        {
            var type = typeof(UInt32);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsUInt16_ReturnsValueRole()
        {
            var type = typeof(UInt16);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsUInt64_ReturnsValueRole()
        {
            var type = typeof(UInt64);
            var expectedRole = FieldRole.Value;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsEnum_ReturnsColumnRole()
        {
            var type = typeof(Enum);
            var expectedRole = FieldRole.Column;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsChar_ReturnsColumnRole()
        {
            var type = typeof(Char);
            var expectedRole = FieldRole.Column;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsString_ReturnsRowRole()
        {
            var type = typeof(string);
            var expectedRole = FieldRole.Row;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsDateTime_ReturnsRowRole()
        {
            var type = typeof(DateTime);
            var expectedRole = FieldRole.Row;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void GetRoleForType_WhenTypeIsBool_ReturnsRowRole()
        {
            var type = typeof(bool);
            var expectedRole = FieldRole.Row;

            var actualRole = FieldInfoHelper.GetRoleForType(type);

            Assert.AreEqual(expectedRole, actualRole);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsSingle_ReturnsTrue()
        {
            var type = typeof(Single);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableSingle_ReturnsTrue()
        {
            var type = typeof(Nullable<Single>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsDouble_ReturnsTrue()
        {
            var type = typeof(Double);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableDouble_ReturnsTrue()
        {
            var type = typeof(Nullable<Double>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsDecimal_ReturnsTrue()
        {
            var type = typeof(Decimal);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableDecimal_ReturnsTrue()
        {
            var type = typeof(Nullable<Decimal>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsInt16_ReturnsTrue()
        {
            var type = typeof(Int16);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableInt16_ReturnsTrue()
        {
            var type = typeof(Nullable<Int16>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsInt32_ReturnsTrue()
        {
            var type = typeof(Int32);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableInt32_ReturnsTrue()
        {
            var type = typeof(Nullable<Int32>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsInt64_ReturnsTrue()
        {
            var type = typeof(Int64);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableInt64_ReturnsTrue()
        {
            var type = typeof(Nullable<Int64>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsByte_ReturnsTrue()
        {
            var type = typeof(byte);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableByte_ReturnsTrue()
        {
            var type = typeof(Nullable<byte>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsSByte_ReturnsTrue()
        {
            var type = typeof(sbyte);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableSByte_ReturnsTrue()
        {
            var type = typeof(Nullable<sbyte>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsUInt32_ReturnsTrue()
        {
            var type = typeof(UInt32);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableUInt32_ReturnsTrue()
        {
            var type = typeof(Nullable<UInt32>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsUInt16_ReturnsTrue()
        {
            var type = typeof(UInt16);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableUInt16_ReturnsTrue()
        {
            var type = typeof(Nullable<UInt16>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsUInt64_ReturnsTrue()
        {
            var type = typeof(UInt64);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsNullableUInt64_ReturnsTrue()
        {
            var type = typeof(Nullable<UInt64>);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsTrue(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsEnum_ReturnsFalse()
        {
            var type = typeof(Enum);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsFalse(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsChar_ReturnsFalse()
        {
            var type = typeof(Char);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsFalse(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsString_ReturnsFalse()
        {
            var type = typeof(string);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsFalse(isNumeric);
        }

        [TestMethod]
        public void IsNumericType_WhenTypeIsDateTime_ReturnsFalse()
        {
            var type = typeof(DateTime);

            var isNumeric = FieldInfoHelper.IsNumericType(type);

            Assert.IsFalse(isNumeric);
        }
    }
}
