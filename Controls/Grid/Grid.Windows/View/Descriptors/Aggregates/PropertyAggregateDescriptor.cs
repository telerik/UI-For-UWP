using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    /// <summary>
    /// Represents an <see cref="AggregateDescriptorBase"/> that is associated with a particular property in the underlying ViewModel.
    /// </summary>
    public class PropertyAggregateDescriptor : AggregateDescriptorBase, IPropertyDescriptor
    {
        private string propertyName;
        private PropertyAggregateDescription engineDescription;
        private KnownFunction function;

        /// <summary>
        /// Gets or sets the <see cref="KnownFunction"/> value to be applied to this aggregate.
        /// </summary>
        public KnownFunction Function
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

        /// <summary>
        /// Gets or sets the name of the property that is used to compute the aggregate value.
        /// </summary>
        public string PropertyName
        {
            get
            {
                return this.propertyName;
            }
            set
            {
                if (this.propertyName == value)
                {
                    return;
                }

                this.propertyName = value;
                this.OnPropertyChanged();
            }
        }

        internal override DescriptionBase EngineDescription
        {
            get
            {
                if (this.engineDescription == null)
                {
                    this.engineDescription = new PropertyAggregateDescription()
                    {
                        PropertyName = this.propertyName,
                        AggregateFunction = AggregateFunctionFromKnownFunction(this.Function),
                        StringFormat = this.Format
                    };
                }

                return this.engineDescription;
            }
        }

        internal override bool Equals(DescriptionBase description)
        {
            var aggregate = description as PropertyAggregateDescription;
            if (aggregate == null)
            {
                return false;
            }

            return aggregate.PropertyName == this.propertyName && 
                aggregate.StringFormat == this.Format && 
                AggregateFunctionEqualsKnownFunction(aggregate.AggregateFunction, this.Function);
        }

        /// <summary>
        /// Provides an entry point for inheritors to provide additional logic over the PropertyChanged routine.
        /// </summary>
        /// <param name="changedPropertyName"></param>
        protected override void PropertyChangedOverride(string changedPropertyName)
        {
            if (this.engineDescription != null)
            {
                if (changedPropertyName == "Format")
                {
                    this.engineDescription.StringFormat = this.Format;
                }
                else if (changedPropertyName == "Function")
                {
                    this.engineDescription.AggregateFunction = AggregateFunctionFromKnownFunction(this.Function);
                }
                else if (changedPropertyName == "PropertyName")
                {
                    this.engineDescription.PropertyName = this.propertyName;
                }
            }

            base.PropertyChangedOverride(changedPropertyName);
        }

        private static bool AggregateFunctionEqualsKnownFunction(AggregateFunction aggregateFunction, KnownFunction function)
        {
            switch (function)
            {
                case KnownFunction.Average:
                    return aggregateFunction is AverageAggregateFunction;
                case KnownFunction.Count:
                    return aggregateFunction is CountAggregateFunction;
                case KnownFunction.Max:
                    return aggregateFunction is MaxAggregateFunction;
                case KnownFunction.Min:
                    return aggregateFunction is MinAggregateFunction;
                case KnownFunction.Product:
                    return aggregateFunction is ProductAggregateFunction;
                case KnownFunction.StdDev:
                    return aggregateFunction is StdDevAggregateFunction;
                case KnownFunction.StdDevP:
                    return aggregateFunction is StdDevPAggregateFunction;
                case KnownFunction.Var:
                    return aggregateFunction is VarAggregateFunction;
                case KnownFunction.VarP:
                    return aggregateFunction is VarPAggregateFunction;
                default:
                    return aggregateFunction is SumAggregateFunction;
            }
        }

        private static AggregateFunction AggregateFunctionFromKnownFunction(KnownFunction function)
        {
            switch (function)
            {
                case KnownFunction.Average:
                    return AggregateFunctions.Average;
                case KnownFunction.Count:
                    return AggregateFunctions.Count;
                case KnownFunction.Max:
                    return AggregateFunctions.Max;
                case KnownFunction.Min:
                    return AggregateFunctions.Min;
                case KnownFunction.Product:
                    return AggregateFunctions.Product;
                case KnownFunction.StdDev:
                    return AggregateFunctions.StdDev;
                case KnownFunction.StdDevP:
                    return AggregateFunctions.StdDevP;
                case KnownFunction.Var:
                    return AggregateFunctions.Var;
                case KnownFunction.VarP:
                    return AggregateFunctions.VarP;
                default:
                    return AggregateFunctions.Sum;
            }
        }
    }
}
