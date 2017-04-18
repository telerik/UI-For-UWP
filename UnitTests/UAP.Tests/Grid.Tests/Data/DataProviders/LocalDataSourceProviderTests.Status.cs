using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Collections.Generic;
using Telerik.Core.Data;
using Telerik.Data.Core;
using Telerik.Data.Core.Engine;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    public partial class LocalDataSourceProviderTests
    {
        [TestMethod]
        public void Status_WhenInstanceIsCreated_IsSetToUninitialized()
        {
            var expectedStatus = DataProviderStatus.Uninitialized;

            var actualStatus = this.provider.Status;

            Assert.AreEqual(expectedStatus, actualStatus);
        }

        [TestMethod]
        public void Status_WhenEngineCompletedEventIsRaisedWithCompleted_IsSetToReady()
        {
            var engine = new PivotEngineMock();
            this.provider = new LocalDataSourceProvider(engine);
            this.provider.GroupFactory = new DataGroupFactory();
            var expectedStatus = DataProviderStatus.Ready;

            engine.RaiseCompletedEvent(new DataEngineCompletedEventArgs(null, DataEngineStatus.Completed));
            var actualStatus = this.provider.Status;

            Assert.AreEqual(expectedStatus, actualStatus);
        }

        [TestMethod]
        public void Status_WhenEngineCompletedEventIsRaisedWithFaulted_IsSetToFaulted()
        {
            var engine = new PivotEngineMock();
            this.provider = new LocalDataSourceProvider(engine);
            this.provider.GroupFactory = new DataGroupFactory();
            var expectedStatus = DataProviderStatus.Faulted;

            engine.RaiseCompletedEvent(new DataEngineCompletedEventArgs(null, DataEngineStatus.Faulted));
            var actualStatus = this.provider.Status;

            Assert.AreEqual(expectedStatus, actualStatus);
        }

        [TestMethod]
        public void Status_WhenSettingItemsSourceToNull_IsSetToUninitialized()
        {
            this.InitializeProviderWithData();

            this.provider.ItemsSource = null;

            var expectedStatus = DataProviderStatus.Uninitialized;
            Assert.AreEqual(expectedStatus, this.provider.Status);
        }

        [TestMethod]
        public void Status_WhenRefreshIsCalled_WhenItemsSourceIsNull_ShouldRemainUninitialized()
        {
            LocalDataSourceProvider provider = new LocalDataSourceProvider();
            provider.AggregateDescriptions.Add(new ListAggregateDescription() { PropertyName = "Item1" });

            this.provider.ItemsSource = Order.GetData();
            provider.Refresh();
            provider.BlockUntilRefreshCompletes();

            Assert.AreEqual(DataProviderStatus.Uninitialized, provider.Status);
        }

        [TestMethod]
        public void CompletedEvent_WhenEngineCompletedEventIsRaised_IsRaised()
        {
            var engine = new PivotEngineMock();
            this.provider = new LocalDataSourceProvider(engine);
            this.provider.GroupFactory = new DataGroupFactory();
            this.provider.StatusChanged += this.OnProviderStatusChanged;

            engine.RaiseCompletedEvent(new DataEngineCompletedEventArgs(null, DataEngineStatus.Completed));

            Assert.IsTrue(this.StatuschangedEventWasRaised());
        }

        [TestMethod]
        public void Status_WhenRefreshIsCalled_WhenFieldInfosCannotBeRetrieved_RemainsUninitialized()
        {
            var expectedStatus = DataProviderStatus.Uninitialized;
            this.InitializeProviderThatCannotRetrieveFieldInfosOnRefresh();

            this.provider.ItemsSource = Order.GetData();
            this.provider.Refresh();

            Assert.AreEqual(expectedStatus, this.provider.Status);
        }

        [TestMethod]
        public void Status_WhenRefreshIsCalled_WhenFieldInfosAreBeingRetrieved_IsSetToInitializing()
        {
            var expectedStatus = DataProviderStatus.Initializing;
            this.InitializeProviderThatIsGettingFieldInfosOnRefresh();

            this.provider.ItemsSource = Order.GetData();
            this.provider.Refresh();

            Assert.AreEqual(expectedStatus, this.provider.Status);
        }

        [TestMethod]
        public void Status_WhenRefreshIsCalled_WhenFieldInfosHaveBeenRetrievedCorrectly_IsSetToRetrievingData()
        {
            var expectedStatus = DataProviderStatus.ProcessingData;
            this.InitializeProviderThatHasReceivedFieldInfosOnRefresh();

            this.provider.ItemsSource = Order.GetData();
            this.provider.Refresh();

            Assert.AreEqual(expectedStatus, this.provider.Status);
        }

        [TestMethod]
        public void Status_WhenRefreshIsCalledAndResultsHaveBeenProcessedCorrectly_IsSetToReady()
        {
            var expectedStatus = DataProviderStatus.Ready;
            this.InitializeProviderThatWillRetrieveDataOnRefresh();

            this.provider.ItemsSource = Order.GetData();
            this.provider.Refresh();

            Assert.AreEqual(expectedStatus, this.provider.Status);
        }

        [TestMethod]
        public void StatusChangedEvents_WhenRefreshIsCalled_WhenFieldInfosAreOkay_WhenDataIsReceived_HaveCorrectSequenceAndState()
        {
            this.InitializeProviderThatWillRetrieveDataOnRefresh();
            var expectedEventSequence = new List<DataProviderStatusChangedEventArgs>()
            {
                new DataProviderStatusChangedEventArgs(DataProviderStatus.Initializing, false, null),
                new DataProviderStatusChangedEventArgs(DataProviderStatus.DescriptionsReady, false, null),
                new DataProviderStatusChangedEventArgs(DataProviderStatus.ProcessingData, false, null),
                new DataProviderStatusChangedEventArgs(DataProviderStatus.Ready, true, null)
            };

            this.provider.ItemsSource = Order.GetData();
            this.provider.Refresh();

            this.AssertStatusChangedEventSequence(expectedEventSequence);
        }

        [TestMethod]
        public void StatusChangedEvent_HoldsTheExceptionObject_WhenXmlaClientReturnExceptionOnSendRequestCompleted()
        {
            this.InitializeProviderWithXmlaClientThatReturnsErrorOnSendRequestCompleted();

            this.provider.ItemsSource = Order.GetData();
            this.provider.Refresh();
            var lastEvent = this.GetLastStatusChangedEvent();

            Assert.IsNotNull(lastEvent.Error);
        }

        private void InitializeProviderWithData()
        {
            this.ConfigureProviderToProduceResults();
            this.provider.Refresh();
            this.provider.BlockUntilRefreshCompletes();
        }
    }
}