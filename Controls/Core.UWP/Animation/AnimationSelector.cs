using System.Collections;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;

namespace Telerik.Core
{
    /// <summary>
    /// Animation Selector, used for easier definition of animations in XAML.
    /// </summary>
    [ContentProperty(Name = "Animations")]
    public class RadAnimationSelector : AnimationSelectorBase
    {
        private List<RadAnimation> animations;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadAnimationSelector"/> class.
        /// </summary>
        public RadAnimationSelector()
        {
            this.animations = new List<RadAnimation>();
        }

        /// <summary>
        /// Gets the list of animations in that this selector will choose from.
        /// </summary>
        public IList Animations
        {
            get
            {
                return this.animations;
            }
        }

        /// <summary>
        /// Selects an animation based on its AnimationName.
        /// </summary>
        /// <remarks>
        ///        <para>
        ///            The AnimationSelector will return the animation with matching name from
        ///            its <strong>Animations</strong> list.
        ///        </para>
        /// </remarks>
        /// <param name="control">The control the animation is needed for.</param>
        /// <param name="name">The name of the animation. Often it is a change of state, result of a user action.</param>
        /// <returns>The RadAnimation object.</returns>
        /// <seealso cref="RadAnimationSelector.Animations"/>
        public override RadAnimation SelectAnimation(UIElement control, string name)
        {
            foreach (var animation in this.animations)
            {
                if (animation.AnimationName == name)
                {
                    return animation;
                }
            }

            return null;
        }
    }
}