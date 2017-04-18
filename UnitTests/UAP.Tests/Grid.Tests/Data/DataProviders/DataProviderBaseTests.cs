using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Telerik.Data.Core;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class DataProviderBaseTests
    {
        private class DataProviderBaseStub : DataProviderBase
        {
            public DataProviderBaseStub(IDataSettings settings)
                : base(settings)
            {
            }

            public int RefreshCallCount
            {
                get;
                private set;
            }

            public override void SuspendPropertyChanges(object item)
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public override void ResumePropertyChanges(object item)
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public override void BeginEditOperation(object item)
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public override void CancelEditOperation(object item)
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public override void CommitEditOperation(object item)
            {
                // TODO: Implement this method
                throw new NotImplementedException();
            }

            public override Collection<PropertyAggregateDescriptionBase> AggregateDescriptions
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
            }

            public override IGroupFactory GroupFactory
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
                set
                {
                    // TODO: Implement this property setter
                    throw new NotImplementedException();
                }
            }

            public override IValueProvider ValueProvider
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
            }

            public override IFieldInfoData FieldDescriptions
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
            }

            public override Collection<SortDescription> SortDescriptions
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
            }

            public override bool IsSingleThreaded
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
                set
                {
                    // TODO: Implement this property setter
                    throw new NotImplementedException();
                }
            }

            public override object ItemsSource
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
                set
                {
                    // TODO: Implement this property setter
                    throw new NotImplementedException();
                }
            }

            public override IDataSourceView DataView
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
                set
                {
                    // TODO: Implement this property setter
                    throw new NotImplementedException();
                }
            }

            public override Collection<PropertyFilterDescriptionBase> FilterDescriptions
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
            }

            public override Collection<PropertyGroupDescriptionBase> GroupDescriptions
            {
                get
                {
                    // TODO: Implement this property getter
                    throw new NotImplementedException();
                }
            }

            internal override IDataResults Results
            {
                get { throw new NotImplementedException(); }
            }

            public override object State
            {
                get { throw new NotImplementedException(); }
            }

            public override void BlockUntilRefreshCompletes()
            {
                //throw new NotImplementedException();
            }

            protected override void RefreshOverride(DataChangeFlags dataChangeFlags)
            {
                this.RefreshCallCount++;
            }

            public void AddPendingChangeToProvider()
            {
                this.Invalidate();
            }

            internal override IFieldDescriptionProvider CreateFieldDescriptionsProvider()
            {
                throw new NotImplementedException();
            }

            internal override IAggregateDescription GetAggregateDescriptionForFieldDescription(IDataFieldInfo description)
            {
                throw new NotImplementedException();
            }

            internal override IGroupDescription GetGroupDescriptionForFieldDescription(IDataFieldInfo description)
            {
                throw new NotImplementedException();
            }

            internal override FilterDescription GetFilterDescriptionForFieldDescription(IDataFieldInfo description)
            {
                throw new NotImplementedException();
            }

            internal override SortDescription GetSortDescriptionForFieldDescription(IDataFieldInfo description)
            {
                throw new NotImplementedException();
            }

            internal override IEnumerable<object> GetAggregateFunctionsForAggregateDescription(IAggregateDescription aggregateDescription)
            {
                throw new NotImplementedException();
            }

            internal override void SetAggregateFunctionToAggregateDescription(IAggregateDescription aggregateDescription, object aggregateFunction)
            {
                throw new NotImplementedException();
            }
        }

        private DataProviderBaseStub provider;
        private bool completedEventRaised;

        private void dataProvider_Completed(object sender, EventArgs e)
        {
            this.completedEventRaised = true;
        }

        [TestInitialize]
        public void TestInitialize()
        {
            this.provider = new DataProviderBaseStub(new DataSettings<PropertyFilterDescriptionBase, PropertyGroupDescriptionBase, PropertyAggregateDescriptionBase, PropertySortDescription>());
            this.provider.StatusChanged += dataProvider_Completed;
            this.completedEventRaised = false;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.provider.StatusChanged -= dataProvider_Completed;
            this.provider.BlockUntilRefreshCompletes();
            this.completedEventRaised = false;
        }

        [TestMethod]
        public void Calling_BeginInit_Twice_Throw_InvalidOperationException()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                isi.BeginInit();
                isi.BeginInit();
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNotNull(ex);
            Assert.AreEqual(typeof(InvalidOperationException), ex.GetType());
            Assert.IsFalse(completedEventRaised);
        }

        [TestMethod]
        public void Calling_EndInit_Before_BeginInit_Throw_InvalidOperationException()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                isi.EndInit();
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNotNull(ex);
            Assert.AreEqual(typeof(InvalidOperationException), ex.GetType());
            Assert.IsFalse(completedEventRaised);
        }

        [TestMethod]
        public void Calling_BeginInit_EndInit_Calls_Refresh_When_NoDefer_WhenProviderIsChanged()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                isi.BeginInit();
                this.provider.AddPendingChangeToProvider();
                isi.EndInit();
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.AreEqual(1, this.provider.RefreshCallCount);
        }

        [TestMethod]
        public void Calling_BeginInit_EndInit_DoesNotCall_Refresh_When_NoDefer_WhenProviderIsNotChanged()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                isi.BeginInit();
                isi.EndInit();
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.AreEqual(0, this.provider.RefreshCallCount);
        }

        [TestMethod]
        public void Calling_DeferRefresh_Calls_Refresh_When_BeginInit_AndProviderLocalStateIsChanged()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                using (provider.DeferRefresh())
                {
                    this.provider.AddPendingChangeToProvider();
                }
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.AreEqual(1, this.provider.RefreshCallCount);
        }

        [TestMethod]
        public void Calling_DeferRefresh_After_BeginInit_Does_Not_Calls_Refresh()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                isi.BeginInit();
                using (provider.DeferRefresh())
                {
                    //provider.ItemsSource = new List<Order>();
                }

                provider.BlockUntilRefreshCompletes();
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.IsFalse(completedEventRaised);
        }

        [TestMethod]
        public void Calling_BeginInit_EndInit_Inside_DeferRefresh_DoesNotCall_Refresh_WhenProviderStateIsChanged()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                using (provider.DeferRefresh())
                {
                    isi.BeginInit();
                    this.provider.AddPendingChangeToProvider();
                    isi.EndInit();

                    Assert.AreEqual(0, this.provider.RefreshCallCount);
                }
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.AreEqual(1, this.provider.RefreshCallCount);
        }

        [TestMethod]
        public void Calling_DeferRefresh_Inside_BeginInit_EndInite_Does_Not_Calls_Refresh_WhenProviderStateIsUpdated()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                isi.BeginInit();

                using (provider.DeferRefresh())
                {
                    this.provider.AddPendingChangeToProvider();
                    provider.BlockUntilRefreshCompletes();
                    Assert.AreEqual(0, this.provider.RefreshCallCount);
                }

                provider.BlockUntilRefreshCompletes();
                Assert.AreEqual(0, this.provider.RefreshCallCount);

                isi.EndInit();
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.AreEqual(1, this.provider.RefreshCallCount);
        }

        [TestMethod]
        public void Nested_DeferRefresh_Does_Not_Calls_Refresh_WhenProvideHasPendingUpdates()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                using (provider.DeferRefresh())
                {
                    using (provider.DeferRefresh())
                    {
                        this.provider.AddPendingChangeToProvider();
                        Assert.AreEqual(0, this.provider.RefreshCallCount);
                    }

                    Assert.AreEqual(0, this.provider.RefreshCallCount);
                }

                provider.BlockUntilRefreshCompletes();
                Assert.AreEqual(1, this.provider.RefreshCallCount);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.AreEqual(1, this.provider.RefreshCallCount);
        }

        [TestMethod]
        public void Setting_New_FieldDescriptions_Calls_Refresh()
        {
            ISupportInitialize isi = this.provider;
            Exception ex = null;
            try
            {
                (this.provider as IDataProvider).FieldDescriptionsProvider = new LocalDataSourceFieldDescriptionsProvider();

                Assert.AreEqual(1, this.provider.RefreshCallCount);
            }
            catch (Exception e)
            {
                ex = e;
            }

            Assert.IsNull(ex);
            Assert.AreEqual(1, this.provider.RefreshCallCount);
        }
    }
}