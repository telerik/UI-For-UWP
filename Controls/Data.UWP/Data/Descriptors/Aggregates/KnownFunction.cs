namespace Telerik.Data.Core
{
    /// <summary>
    /// The predefined mathematical functions known to data component's aggregate API.
    /// </summary>
    public enum KnownFunction
    {
        /// <summary>
        /// The SUM function.
        /// </summary>
        Sum,

        /// <summary>
        /// The MINIMUM function.
        /// </summary>
        Min,

        /// <summary>
        /// The MAXIMUM function.
        /// </summary>
        Max,

        /// <summary>
        /// The AVERAGE function.
        /// </summary>
        Average,

        /// <summary>
        /// The COUNT function.
        /// </summary>
        Count,

        /// <summary>
        /// The PRODUCT function.
        /// </summary>
        Product,

        /// <summary>
        /// The STANDARD DEVIATION, based on a sample function.
        /// </summary>
        StdDev,

        /// <summary>
        /// The STANDARD DEVIATION, based on the entire population function.
        /// </summary>
        StdDevP,

        /// <summary>
        /// The VARIANCE, based on a sample function.
        /// </summary>
        Var,

        /// <summary>
        /// The VARIANCE, based on the entire population function.
        /// </summary>
        VarP
    }
}