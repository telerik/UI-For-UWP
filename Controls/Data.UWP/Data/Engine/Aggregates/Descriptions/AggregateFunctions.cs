namespace Telerik.Data.Core
{
    /// <summary>
    /// Describes the supported aggregate functions available for LocalDataSourceProvider.
    /// </summary>
    internal static class AggregateFunctions
    {
        /// <summary>
        /// Gets an aggregate function that computes the sum.
        /// </summary>
        public static AggregateFunction Sum
        {
            get
            {
                return new SumAggregateFunction();
            }
        }

        /// <summary>
        /// Gets an aggregate function that counts items.
        /// </summary>
        public static AggregateFunction Count
        {
            get
            {
                return new CountAggregateFunction();
            }
        }

        /// <summary>
        /// Gets an aggregate function that computes the average.
        /// </summary>
        public static AggregateFunction Average
        {
            get
            {
                return new AverageAggregateFunction();
            }
        }

        /// <summary>
        /// Gets an aggregate function that computes the maximum.
        /// </summary>
        public static AggregateFunction Max
        {
            get
            {
                return new MaxAggregateFunction();
            }
        }

        /// <summary>
        /// Gets an aggregate function that computes the minimum.
        /// </summary>
        public static AggregateFunction Min
        {
            get
            {
                return new MinAggregateFunction();
            }
        }

        /// <summary>
        /// Gets an aggregate function that computes the product.
        /// </summary>
        public static AggregateFunction Product
        {
            get
            {
                return new ProductAggregateFunction();
            }
        }

        /// <summary>
        /// Gets an aggregate function that estimates the standard deviation of a population based on a sample.
        /// </summary>
        public static AggregateFunction StdDev
        {
            get
            {
                return new StdDevAggregateFunction();
            }
        }

        /// <summary>
        /// Gets an aggregate function that estimates the standard deviation of a population based on the entire population.
        /// </summary>
        public static AggregateFunction StdDevP
        {
            get
            {
                return new StdDevPAggregateFunction();
            }
        }

        /// <summary>
        /// Gets an aggregate function that estimates the variance based on a sample.
        /// </summary>
        public static AggregateFunction Var
        {
            get
            {
                return new VarAggregateFunction();
            }
        }

        /// <summary>
        /// Gets an aggregate function that estimates the variance based on the entire population.
        /// </summary>
        public static AggregateFunction VarP
        {
            get
            {
                return new VarPAggregateFunction();
            }
        }
    }
}