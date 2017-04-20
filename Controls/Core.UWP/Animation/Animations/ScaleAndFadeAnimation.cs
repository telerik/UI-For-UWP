using System;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;

namespace Telerik.Core
{
    /// <summary>
    /// Provides a pre-defined scale and fade animation group.
    /// </summary>
    public class RadScaleAndFadeAnimation : RadAnimationGroup
    {
        private RadScaleAnimation scale;
        private RadFadeAnimation fade;

        /// <summary>
        /// Initializes a new instance of the RadScaleAndFadeAnimation class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadScaleAndFadeAnimation()
        {
            this.scale = this.CreateScaleAnimation();
            this.fade = this.CreateFadeAnimation();

            this.Children.Add(this.scale);
            this.Children.Add(this.fade);
        }

        /// <summary>
        /// Gets the scale animation instance.
        /// </summary>
        public RadScaleAnimation ScaleAnimation
        {
            get
            {
                return this.scale;
            }
        }

        /// <summary>
        /// Gets the fade animation instance.
        /// </summary>
        public RadFadeAnimation FadeAnimation
        {
            get
            {
                return this.fade;
            }
        }

        /// <summary>
        /// Called in the constructor to create the default scale animation.
        /// </summary>
        /// <returns>Returns a new instance of RadScaleAnimation.</returns>
        protected virtual RadScaleAnimation CreateScaleAnimation()
        {
            RadScaleAnimation result = new RadScaleAnimation();
            result.Duration = new Duration(TimeSpan.FromSeconds(0.5));

            return result;
        }

        /// <summary>
        /// Called in the constructor to create the default fade animation.
        /// </summary>
        /// <returns>Returns a new instance of RadFadeAnimation.</returns>
        protected virtual RadFadeAnimation CreateFadeAnimation()
        {
            RadFadeAnimation result = new RadFadeAnimation();
            result.Duration = new Duration(TimeSpan.FromSeconds(0.3));

            return result;
        }
    }
}
