using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class UserAggregateFunction : AggregateFunction
    {
        private IAggregateFunction function;

        public IAggregateFunction Function
        {
            get
            {
                return this.function;
            }
            set
            {
                if (this.function == value)
                {
                    return;
                }

                this.function = value;
                this.OnPropertyChanged("Function");
                this.OnSettingsChanged(new SettingsChangedEventArgs());
            }
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return 19;
        }

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            return obj is UserAggregateFunction;
        }

        protected internal override AggregateValue CreateAggregate(Type dataType)
        {
            if (this.function == null)
            {
                throw new InvalidOperationException("No IAggregateFunction implementation specified.");
            }

            return new UserAggregateValue() { Function = this.function.Clone() };
        }

        protected override Cloneable CreateInstanceCore()
        {
            return new UserAggregateFunction() { function = this.function };
        }

        protected override void CloneCore(Cloneable source)
        {
        }
    }
}
