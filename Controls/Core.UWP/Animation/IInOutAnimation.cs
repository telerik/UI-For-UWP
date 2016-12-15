namespace Telerik.Core
{
    /// <summary>
    /// Specifies that an animation has in and out modes.
    /// For example an animation can be different depending whether
    /// the user is navigating to some page or from some page. The new
    /// page is animated with an "in" animation while the old page
    /// is animated with an "out" animation.
    /// </summary>
    public interface IInOutAnimation
    {
        /// <summary>
        /// Gets or sets a value which indicates if an animation supports in and out modes.
        /// </summary>
        InOutAnimationMode InOutAnimationMode
        {
            get;
            set;
        }
    }
}