using System;

namespace Telerik.Data.Core
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
                this.OnPropertyChanged(nameof(this.Function));
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