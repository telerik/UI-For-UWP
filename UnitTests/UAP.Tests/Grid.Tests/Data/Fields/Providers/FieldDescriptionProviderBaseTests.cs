using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [TestClass]
    public class FieldDescriptionProviderBaseTests
    {
        private bool completedEventRaised;
        private GetDescriptionsDataCompletedEventArgs latestCompletedEventArgs;
        private InheritedFieldDescriptionProviderBase provider;

        [TestInitialize]
        public void TestInitialize()
        {
            this.provider = new InheritedFieldDescriptionProviderBase();
            this.provider.GetDescriptionsDataAsyncCompleted += this.provider_GetDescriptionsDataAsyncCompleted;
        }

        [TestCleanup]
        public void TestCleanup()
        {
            this.provider.GetDescriptionsDataAsyncCompleted -= this.provider_GetDescriptionsDataAsyncCompleted;
            this.latestCompletedEventArgs = null;
            this.completedEventRaised = false;
        }

        [TestMethod]
        public void IsBusy_WhenInstanceIsCreated_IsSetToFalse()
        {
            var expectedIsBusy = false;

            var actualIsBusy = this.provider.IsBusy;

            Assert.AreEqual(expectedIsBusy, actualIsBusy);
        }

        [TestMethod]
        public void IsBusy_WhenAsyncOperationHasFinished_IsSetToFalse()
        {
            this.provider.GetDescriptionsDataAsync();
            var expectedIsBusy = false;
            var actualIsBusy = this.provider.IsBusy;

            Assert.AreEqual(expectedIsBusy, actualIsBusy);
        }

        [TestMethod]
        public void GetDescriptionsDataAsyncCompletedEvent_IsRaisedWhenAsyncOperationCompletes()
        {
            this.provider.GetDescriptionsDataAsync();

            Assert.IsTrue(this.completedEventRaised);
        }

        [TestMethod]
        public void GetDescriptionsDataAsyncCompletedEventArgs_ContainsTheResultReturnedByGenerateDescriptionsData()
        {
            var expectedDescriptionsData = new FieldInfoData(new ContainerNode("TestName", ContainerNodeRole.Other));
            this.provider.ActionToExecuteAsync = () => expectedDescriptionsData;
            this.provider.GetDescriptionsDataAsync();

            var actualDescriptionsData = this.latestCompletedEventArgs.DescriptionsData;

            Assert.AreSame(expectedDescriptionsData, actualDescriptionsData);
        }

        [TestMethod]
        public void GetDescriptionsDataAsyncCompletedEventArgs_WhenAsyncOperationHasFailedDueToException_HasItsErrorPropertySetToTheException()
        {
            var expectedException = new ArgumentNullException("");
            this.provider.ActionToExecuteAsync = () => { throw expectedException; };
            this.provider.GetDescriptionsDataAsync();

            var actualException = this.latestCompletedEventArgs.Error;

            Assert.AreSame(expectedException, actualException);
        }

        [TestMethod]
        public void GetDescriptionsDataAsyncCompletedEventArgs_WhenAsyncOperationHasFailedDueToException_WhenDescriptionsDataPropertyIsAccessed_RaisesException()
        {
            var expectedException = new ArgumentNullException("myException");
            this.provider.ActionToExecuteAsync = () => { throw expectedException; };
            this.provider.GetDescriptionsDataAsync();

            try
            {
                var data = this.latestCompletedEventArgs.DescriptionsData;

                Assert.Fail("Exception expected");
            }
            catch (Exception)
            {
            }
        }

        private void provider_GetDescriptionsDataAsyncCompleted(object sender, GetDescriptionsDataCompletedEventArgs e)
        {
            this.completedEventRaised = true;
            this.latestCompletedEventArgs = e;
        }

        internal class InheritedFieldDescriptionProviderBase : FieldDescriptionProviderBase
        {
            private IFieldInfoData data;

            public InheritedFieldDescriptionProviderBase()
            {
                this.ActionToExecuteAsync = () =>
                {
                    return new FieldInfoData(new ContainerNode("TestName", ContainerNodeRole.Other));
                };
            }

            public override bool IsReady
            {
                get
                {
                    return true;
                }
            }

            public Func<IFieldInfoData> ActionToExecuteAsync { get; set; }

            internal override bool IsDynamic
            {
                get
                {
                    return false;
                }
            }

            public override void GetDescriptionsDataAsync(object state)
            {
                this.data = this.ActionToExecuteAsync();
            }

            public void GetDescriptionsDataAsync()
            {
                Exception ex = null;
                try
                {
                    this.GetDescriptionsDataAsync(null);
                }
                catch (Exception e)
                {
                    ex = e;
                    this.data = new EmptyFieldInfoData();
                }

                this.OnDescriptionsDataCompleted(new GetDescriptionsDataCompletedEventArgs(ex, null, this.data));
            }
        }
    }
}