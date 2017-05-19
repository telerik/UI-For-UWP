using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.Core
{
    /// <summary>
    /// Scale animation for showing/hiding elements.
    /// </summary>
    public class RadScaleAnimation : RadAnimationGroup
    {
        private RadScaleXAnimation scaleXAnimation = new RadScaleXAnimation();
        private RadScaleYAnimation scaleYAnimation = new RadScaleYAnimation();

        /// <summary>
        /// Initializes a new instance of the <see cref="RadScaleAnimation"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadScaleAnimation()
        {
            this.Duration = new Duration(TimeSpan.FromSeconds(.2));
            this.Children.Add(this.scaleXAnimation);
            this.Children.Add(this.scaleYAnimation);
        }

        /// <summary>
        /// Gets or sets the duration of the scale animation. The value of
        /// this property will be set as the value of DurationX and DurationY.
        /// </summary>
        public override Duration Duration
        {
            get
            {
                return base.Duration;
            }

            set
            {
                base.Duration = value;
                this.DurationX = value;
                this.DurationY = value;
            }
        }

        /// <summary>
        /// Gets or sets the render transform origin of the animated element.
        /// </summary>
        public override Point AnimationOrigin
        {
            get
            {
                return base.AnimationOrigin;
            }

            set
            {
                base.AnimationOrigin = value;
                this.scaleXAnimation.AnimationOrigin = value;
                this.scaleYAnimation.AnimationOrigin = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration of the X animation.
        /// </summary>
        public Duration DurationX
        {
            get
            {
                return this.scaleXAnimation.Duration;
            }

            set
            {
                this.scaleXAnimation.Duration = value;
            }
        }

        /// <summary>
        /// Gets or sets the duration of the Y animation.
        /// </summary>
        public Duration DurationY
        {
            get
            {
                return this.scaleYAnimation.Duration;
            }

            set
            {
                this.scaleYAnimation.Duration = value;
            }
        }

        /// <summary>
        /// Gets or sets the start scale on the X axis.
        /// </summary>
        public double? StartScaleX
        {
            get
            {
                return this.scaleXAnimation.StartScaleX;
            }

            set
            {
                this.scaleXAnimation.StartScaleX = value;
            }
        }

        /// <summary>
        /// Gets or sets the end scale on the X axis.
        /// </summary>
        public double? EndScaleX
        {
            get
            {
                return this.scaleXAnimation.EndScaleX;
            }

            set
            {
                this.scaleXAnimation.EndScaleX = value;
            }
        }

        /// <summary>
        /// Gets or sets the start scale on the Y axis.
        /// </summary>
        public double? StartScaleY
        {
            get
            {
                return this.scaleYAnimation.StartScaleY;
            }

            set
            {
                this.scaleYAnimation.StartScaleY = value;
            }
        }

        /// <summary>
        /// Gets or sets the end scale on the Y axis.
        /// </summary>
        public double? EndScaleY
        {
            get
            {
                return this.scaleYAnimation.EndScaleY;
            }

            set
            {
                this.scaleYAnimation.EndScaleY = value;
            }
        }
    }
}
