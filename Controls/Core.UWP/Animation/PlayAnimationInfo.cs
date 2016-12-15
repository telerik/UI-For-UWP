using System;
using System.Collections.Generic;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// Encapsulates information for an RadAnimationManager.Play pass.
    /// </summary>
    public class PlayAnimationInfo
    {
        private Storyboard storyboard;
        private RadAnimation animation;
        private WeakReference target;
        private Dictionary<RadAnimation, object[]> animatedValues;
        private int targetHashCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayAnimationInfo"/> class.
        /// </summary>
        /// <param name="storyboard">The storyboard.</param>
        /// <param name="animation">The animation.</param>
        /// <param name="target">The target.</param>
        public PlayAnimationInfo(Storyboard storyboard, RadAnimation animation, UIElement target)
        {
            if (target == null)
            {
                return;
            }

            this.storyboard = storyboard;
            this.animation = animation;
            this.target = new WeakReference(target);
            this.animatedValues = new Dictionary<RadAnimation, object[]>();
            this.targetHashCode = target.GetHashCode();
        }

        /// <summary>
        /// Gets the <see cref="Storyboard"/> instance associated with this play pass.
        /// </summary>
        public Storyboard Storyboard
        {
            get
            {
                return this.storyboard;
            }
        }

        /// <summary>
        /// Gets the <see cref="RadAnimation"/> instance associated with this play pass.
        /// </summary>
        public RadAnimation Animation
        {
            get
            {
                return this.animation;
            }
        }

        /// <summary>
        /// Gets the <see cref="FrameworkElement"/> instance associated with this play pass.
        /// </summary>
        public UIElement Target
        {
            get
            {
                if (this.target.IsAlive)
                {
                    return this.target.Target as UIElement;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the hash code of the animation target.
        /// </summary>
        /// <remarks>
        /// Since the target is a weak reference it can be garbage collected before we
        /// get its hash code for resource cleanup. See AnimationManager.RemoveAnimationInfo().
        /// </remarks>
        internal int TargetHashCode
        {
            get
            {
                return this.targetHashCode;
            }
        }

        /// <summary>
        /// Gets previously stored array of values for the specified animation.
        /// </summary>
        /// <param name="targetAnimation">The animation which values are to be retrieved.</param>
        /// <returns>An array of values, stored by the specified animation.</returns>
        public object[] GetAnimatedValues(RadAnimation targetAnimation)
        {
            if (targetAnimation == null)
            {
                return null;
            }

            object[] values;

            this.animatedValues.TryGetValue(targetAnimation, out values);

            return values;
        }

        /// <summary>
        /// Records an array of values for the specified animation.
        /// </summary>
        /// <param name="targetAnimation">The animation which provides the values.</param>
        /// <param name="values">The values to be stored.</param>
        public void SetAnimatedValues(RadAnimation targetAnimation, object[] values)
        {
            if (targetAnimation != null)
            {
                this.animatedValues[targetAnimation] = values;
            }
        }
    }
}