namespace Telerik.Data.Core
{
    /// <summary>
    /// Describes the supported aggregate functions available for LocalDataSourceProvider.
    /// </summary>
    internal static class AggregateFunctions
    {
        /// <summary>
        /// Computes the sum.
        /// </summary>
        public static AggregateFunction Sum
        {
            get
            {
                return new SumAggregateFunction();
            }
        }

        /// <summary>
        /// Counts items.
        /// </summary>
        public static AggregateFunction Count
        {
            get
            {
                return new CountAggregateFunction();
            }
        }

        /// <summary>
        /// Computes the average.
        /// </summary>
        public static AggregateFunction Average
        {
            get
            {
                return new AverageAggregateFunction();
            }
        }

        /// <summary>
        /// Computes the maximum.
        /// </summary>
        public static AggregateFunction Max
        {
            get
            {
                return new MaxAggregateFunction();
            }
        }

        /// <summary>
        /// Computes the minimum.
        /// </summary>
        public static AggregateFunction Min
        {
            get
            {
                return new MinAggregateFunction();
            }
        }

        /// <summary>
        /// Computes the product.
        /// </summary>
        public static AggregateFunction Product
        {
            get
            {
                return new ProductAggregateFunction();
            }
        }

        /// <summary>
        /// Estimates the standard deviation of a population based on a sample.
        /// </summary>
        public static AggregateFunction StdDev
        {
            get
            {
                return new StdDevAggregateFunction();
            }
        }

        /// <summary>
        /// Estimates the standard deviation of a population based on the entire population.
        /// </summary>
        public static AggregateFunction StdDevP
        {
            get
            {
                return new StdDevPAggregateFunction();
            }
        }

        /// <summary>
        /// Estimates the variance based on a sample.
        /// </summary>
        public static AggregateFunction Var
        {
            get
            {
                return new VarAggregateFunction();
            }
        }

        /// <summary>
        /// Estimates the variance based on the entire population.
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