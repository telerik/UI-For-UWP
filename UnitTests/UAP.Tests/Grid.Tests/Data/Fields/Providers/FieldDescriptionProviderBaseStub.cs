using System;
using Telerik.Data.Core.Fields;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    internal class FieldDescriptionProviderBaseStub : FieldDescriptionProviderBase
    {
        public FieldDescriptionProviderBaseStub()
        {
            this.DataToReturn = new EmptyFieldInfoData();
            this.ActionOnGetDescriptionsDataAsync = (object state) =>
            {
                var args = new GetDescriptionsDataCompletedEventArgs(this.ErrorToReturn, state, this.DataToReturn);

                this.OnDescriptionsDataCompleted(args);
            };
        }

        public override bool IsReady
        {
            get
            {
                return true;
            }
        }

        public Action<object> ActionOnGetDescriptionsDataAsync { get; set; }

        public IFieldInfoData DataToReturn { get; set; }

        public Exception ErrorToReturn { get; set; }

        internal override bool IsDynamic
        {
            get
            {
                return false                ;
            }
        }

        public override void GetDescriptionsDataAsync(object state)
        {
            this.ActionOnGetDescriptionsDataAsync(state);
        }
    }
}