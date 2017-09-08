using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class PropertyGroupDescriptionTests
    {
        #region DefaultValues
        [TestMethod]
        public void PropertyGroupDescription_DefaultValues()
        {
            var gd = new PropertyGroupDescription();
            Assert.IsNull(gd.CustomName, "CustomName default value is wrong.");
            Assert.IsNotNull(((IGroupDescription)gd).GroupComparer, "GroupComparer default value is wrong.");
            Assert.IsNull(gd.GroupFilter, "GroupFilter default value is wrong.");
            Assert.IsNull(gd.PropertyName, "PropertyName default value is wrong.");
            Assert.AreEqual(false, gd.ShowGroupsWithNoData, "ShowGroupsWithNoData default value is wrong.");
            Assert.AreEqual(SortOrder.Ascending, gd.SortOrder, "SortOrder default value is wrong.");
        }
        #endregion

        #region PropertyChangedNotifications
        [TestMethod]
        public void PropertyGroupDescription_PropertyChangedNotifications_CustomName()
        {
            var gd = new PropertyGroupDescription() { CustomName = string.Empty };
            gd.AssertPropertyChanged("Setting CustomName", () => gd.CustomName = "UserSetCustomName", "CustomName", "DisplayName");
        }

        [TestMethod]
        public void PropertyGroupDescription_PropertyChangedNotifications_GroupComparer()
        {
            var gd = new PropertyGroupDescription();// { GroupComparer = null };
            gd.AssertPropertyChanged("Setting GroupComparer", () => ((IGroupDescription)gd).GroupComparer = new GrandTotalComparer(), "GroupComparer");
        }

        //[TestMethod]
        //public void PropertyGroupDescription_PropertyChangedNotifications_GroupFilter()
        //{
        //    var gd = new PropertyGroupDescription() { GroupFilter = null };
        //    gd.AssertPropertyChanged("Setting GroupFilter", () => gd.GroupFilter = new ValueGroupFilter(), "GroupFilter");
        //}

        [TestMethod]
        public void PropertyGroupDescription_PropertyChangedNotifications_PropertyName()
        {
            var gd = new PropertyGroupDescription() { PropertyName = string.Empty };
            gd.AssertPropertyChanged("Setting PropertyName", () => gd.PropertyName = "CustomProperty", "PropertyName", "DisplayName");
        }

        [TestMethod]
        public void PropertyGroupDescription_PropertyChangedNotifications_ShowGroupsWithNoData()
        {
            var gd = new PropertyGroupDescription() { ShowGroupsWithNoData = false };
            gd.AssertPropertyChanged("Setting ShowGroupsWithNoData", () => gd.ShowGroupsWithNoData = true, "ShowGroupsWithNoData");
        }

        [TestMethod]
        public void PropertyGroupDescription_PropertyChangedNotifications_SortOrder()
        {
            var gd = new PropertyGroupDescription() { SortOrder = SortOrder.Ascending };
            gd.AssertPropertyChanged("Setting SortOrder", () => gd.SortOrder = SortOrder.Descending, "SortOrder");
        }
        #endregion

        #region Proper DisplayName
        [TestMethod]
        public void PropertyGroupDescription_DisplayName_From_PropertyName()
        {
            var gd = new PropertyGroupDescription() { PropertyName = "PropertyName" };
            Assert.IsNotNull(gd.DisplayName);
            Assert.AreEqual("PropertyName", gd.DisplayName);
        }

        [TestMethod]
        public void PropertyGroupDescription_DisplayName_From_CustomName()
        {
            var gd = new PropertyGroupDescription() { CustomName = "CustomName" };
            Assert.IsNotNull(gd.DisplayName);
            Assert.AreEqual("CustomName", gd.DisplayName);
        }

        [TestMethod]
        public void PropertyGroupDescription_DisplayName_From_PropertyName_and_CustomName()
        {
            var gd = new PropertyGroupDescription() { CustomName = "CustomName", PropertyName = "PropertyName" };
            Assert.IsNotNull(gd.DisplayName);
            Assert.AreEqual("CustomName", gd.DisplayName);
        }

        [TestMethod]
        public void PropertyGroupDescription_DisplayName_CorrectOnCleanUp()
        {
            var gd = new PropertyGroupDescription() { CustomName = "CustomName", PropertyName = "PropertyName" };
            Assert.IsNotNull(gd.DisplayName);
            Assert.AreEqual("CustomName", gd.DisplayName);
            gd.CustomName = null;
            Assert.AreEqual("PropertyName", gd.DisplayName);
            gd.PropertyName = null;
            Assert.IsNull(null, gd.DisplayName);
        }
        #endregion

        #region GroupNameFromItem
        [TestMethod]
        public void PropertyGroupDescription_GroupNameFromItem_ByString()
        {
            var groupByStringProperty = new PropertyGroupDescription() { PropertyName = "StringProperty", MemberAccess = new DelegateMemberAccess<ValuesItem>((o) => o.StringProperty) };
            var item = new ValuesItem() { StringProperty = "Hello World!" };
            var groupName = groupByStringProperty.GroupNameFromItem(item, 0);
            Assert.AreEqual("Hello World!", groupName);
        }

        [TestMethod]
        public void PropertyGroupDescription_GroupNameFromItem_ByDouble()
        {
            var groupByStringProperty = new PropertyGroupDescription() { PropertyName = "DoubleProperty", MemberAccess = new DelegateMemberAccess<ValuesItem>((o) => o.DoubleProperty) };
            var item = new ValuesItem() { DoubleProperty = 101.5d };
            var groupName = groupByStringProperty.GroupNameFromItem(item, 0);
            Assert.AreEqual(101.5d, groupName);
        }

        [TestMethod]
        public void PropertyGroupDescription_GroupNameFromItem_ByBoolean()
        {
            var groupByStringProperty = new PropertyGroupDescription() { PropertyName = "BooleanProperty", MemberAccess = new DelegateMemberAccess<ValuesItem>((o) => o.BooleanProperty) };
            var item = new ValuesItem() { BooleanProperty = true };
            var groupName = groupByStringProperty.GroupNameFromItem(item, 0);
            Assert.AreEqual(true, groupName);
        }

        [TestMethod]
        public void PropertyGroupDescription_GroupNameFromItem_ByObject_CustomObject()
        {
            var groupByStringProperty = new PropertyGroupDescription() { PropertyName = "ObjectProperty", MemberAccess = new DelegateMemberAccess<ValuesItem>((o) => o.ObjectProperty) };
            var obj = new ValuesItem();
            var item = new ValuesItem() { ObjectProperty = obj };
            var groupName = groupByStringProperty.GroupNameFromItem(item, 0);
            Assert.AreEqual(obj, groupName);
        }

        [TestMethod]
        public void PropertyGroupDescription_GroupNameFromItem_ByObject_BoxedObject()
        {
            var groupByStringProperty = new PropertyGroupDescription() { PropertyName = "ObjectProperty", MemberAccess = new DelegateMemberAccess<ValuesItem>((o) => o.ObjectProperty) };
            var item = new ValuesItem() { ObjectProperty = 12.5d };
            var groupName = groupByStringProperty.GroupNameFromItem(item, 0);
            Assert.AreEqual(12.5d, groupName);
        }

        [TestMethod]
        public void PropertyGroupDescription_GroupNameFromItem_ByObject_Null()
        {
            var groupByStringProperty = new PropertyGroupDescription() { PropertyName = "ObjectProperty", MemberAccess = new DelegateMemberAccess<ValuesItem>((o) => o.ObjectProperty) };
            var item = new ValuesItem() { ObjectProperty = null };
            var groupName = groupByStringProperty.GroupNameFromItem(item, 0);
            Assert.AreEqual(null, groupName);
        }

        [TestMethod]
        public void PropertyGroupDescription_GroupNameFromItem_ShouldNotHandleOrRethrowExceptions()
        {
            var groupByErrorProperty = new PropertyGroupDescription() { PropertyName = "ErrorPrperty", MemberAccess = new DelegateMemberAccess<ValuesItem>((o) => o.ErrorProperty) };
            var item = new ValuesItem();
            try
            {
                var groupName = groupByErrorProperty.GroupNameFromItem(item, 0);
                Assert.Fail("Exception handeld.");
            }
            catch (Exception e)
            {
                Assert.IsInstanceOfType(e, typeof(ErrorPropertyException), "Expected exception type {0}, thrown {1}", typeof(ErrorPropertyException), e.GetType());
            }
        }
        #endregion

        #region Clone
        [TestMethod]
        public void PropertyGroupDescription_Clone()
        {
            var original = new PropertyGroupDescription();
            PropertyGroupDescriptionTests.SetPropertiesToCopySource(original);

            var clone = original.Clone() as PropertyGroupDescription;
            Assert.IsNotNull(clone);
            Assert.IsInstanceOfType(clone, typeof(PropertyGroupDescription));

            PropertyGroupDescriptionTests.AssertPropertiesAreCloned(original, clone);
        }

        internal static void SetPropertiesToCopySource(PropertyGroupDescriptionBase source)
        {
            source.CustomName = "CustomName";
            IGroupDescription igd = source as IGroupDescription;
            igd.GroupComparer = new GrandTotalComparer() { AggregateIndex = 1 };
            //IntervalCondition condition = new IntervalCondition() { From = 100, To = 200, Condition = IntervalComparison.IsBetween };
            //ValueGroupFilter groupFilter = new ValueGroupFilter() { Condition = condition, AggregateIndex = 1 };
            //source.GroupFilter = groupFilter;
            source.MemberAccess = new DelegateMemberAccess<ValuesItem>((i) => i.DoubleProperty);
            source.PropertyName = "DoubleProperty";
            source.ShowGroupsWithNoData = true;
            source.SortOrder = SortOrder.Descending;
        }

        internal static void AssertPropertiesAreCloned(PropertyGroupDescriptionBase source, PropertyGroupDescriptionBase clone)
        {
            // CustomName
            Assert.AreEqual("CustomName", clone.CustomName);

            IGroupDescription igd = source as IGroupDescription;
            IGroupDescription igdClone = clone as IGroupDescription;
            // GroupComparer
            Assert.AreNotSame(igd.GroupComparer, igdClone.GroupComparer);
            Assert.IsInstanceOfType(igdClone.GroupComparer, typeof(GrandTotalComparer));
            GrandTotalComparer comparerClone = (GrandTotalComparer)igdClone.GroupComparer;
            Assert.AreEqual(1, comparerClone.AggregateIndex);

            // GroupFilter
            //Assert.AreNotSame(source.GroupFilter, clone.GroupFilter);
            //Assert.IsInstanceOfType(clone.GroupFilter, typeof(ValueGroupFilter));

            //// GroupFilter.Condition
            //ValueGroupFilter groupFilterClone = (ValueGroupFilter)clone.GroupFilter;
            //Assert.AreNotSame(groupFilterClone.Condition, ((ValueGroupFilter)source.GroupFilter).Condition);
            //Assert.IsInstanceOfType(groupFilterClone.Condition, typeof(IntervalCondition));

            //IntervalCondition conditionClone = (IntervalCondition)groupFilterClone.Condition;
            //Assert.AreEqual(100, conditionClone.From);
            //Assert.AreEqual(200, conditionClone.To);
            //Assert.AreEqual(IntervalComparison.IsBetween, conditionClone.Condition);
            //Assert.AreEqual(1, groupFilterClone.AggregateIndex);

            // MemberAccess
            Assert.AreSame(source.MemberAccess, clone.MemberAccess);

            // PropertyName
            Assert.AreEqual("DoubleProperty", clone.PropertyName);

            // ShowGroupsWithNoData
            Assert.AreEqual(true, clone.ShowGroupsWithNoData);

            // SortOrder
            Assert.AreEqual(SortOrder.Descending, clone.SortOrder);
        }
        #endregion
    }
}
