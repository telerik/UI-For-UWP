using System;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents a custom <see cref="AggregateDescriptorBase"/> implementation that allows custom aggregate function definition.
    /// </summary>
    public class DelegateAggregateDescriptor : AggregateDescriptorBase
    {
        private IAggregateFunction function;
        private IKeyLookup valueLookup;
        private DelegateAggregateDescription engineDescription;

        /// <summary>
        /// Gets or sets the <see cref="IKeyLookup"/> instance used to retrieve the value from the underlying ViewModel, used for aggregated value computation.
        /// </summary>
        public IKeyLookup ValueLookup
        {
            get
            {
                return this.valueLookup;
            }
            set
            {
                if (this.valueLookup == value)
                {
                    return;
                }

                this.valueLookup = value;
                this.OnPropertyChanged();
            }
        }

        /// <summary>
        /// Gets or sets the <see cref="IAggregateFunction"/> instance that performs the aggregation of the values as specified by the <see cref="ValueLookup"/> property.
        /// </summary>
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
                this.OnPropertyChanged();
            }
        }

        internal override DescriptionBase EngineDescription
        {
            get
            {
                if (this.engineDescription == null)
                {
                    if (this.valueLookup == null)
                    {
                        throw new ArgumentNullException("ValueLookup member should be initialized.");
                    }
                    if (this.function == null)
                    {
                        throw new ArgumentNullException("Function member should be initialized.");
                    }

                    this.engineDescription = new DelegateAggregateDescription()
                    {
                        AggregateFunction = new UserAggregateFunction() { Function = this.function },
                        MemberAccess = new DelegateMemberAccess() { Getter = this.valueLookup.GetKey },
                    };
                }

                return this.engineDescription;
            }
        }

        internal override bool Equals(DescriptionBase description)
        {
            if (this.engineDescription == null)
            {
                return false;
            }

            var delegateDescription = description as DelegateAggregateDescription;
            if (delegateDescription == null)
            {
                return false;
            }

            return delegateDescription.MemberAccess == this.engineDescription.MemberAccess &&
                delegateDescription.AggregateFunction == this.engineDescription.AggregateFunction;
        }
    }
}
