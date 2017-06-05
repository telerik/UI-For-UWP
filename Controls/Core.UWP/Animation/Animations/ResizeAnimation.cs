using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.UI.Xaml;

namespace Telerik.Core
{
    /// <summary>
    /// Animation for smooth resizing elements.
    /// </summary>
    public class RadResizeAnimation : RadAnimationGroup
    {
        private RadResizeWidthAnimation resizeWidthAnimation = new RadResizeWidthAnimation();
        private RadResizeHeightAnimation resizeHeightAnimation = new RadResizeHeightAnimation();

        /// <summary>
        /// Initializes a new instance of the RadResizeAnimation class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadResizeAnimation()
        {
            this.Duration = new Duration(TimeSpan.FromSeconds(.4));

            this.Children.Add(this.resizeWidthAnimation);
            this.Children.Add(this.resizeHeightAnimation);
        }

        /// <summary>
        /// Gets or sets the Size structure that defines the initial size. If no value is applied current element size is used.
        /// </summary>
        public Size? StartSize
        {
            get
            {
                if (!this.resizeHeightAnimation.StartHeight.HasValue)
                {
                    return null;
                }

                if (!this.resizeWidthAnimation.StartWidth.HasValue)
                {
                    return null;
                }

                return new Size(this.resizeWidthAnimation.StartWidth.Value, this.resizeHeightAnimation.StartHeight.Value);
            }

            set
            {
                if (value.HasValue)
                {
                    this.resizeWidthAnimation.StartWidth = value.Value.Width;
                    this.resizeHeightAnimation.StartHeight = value.Value.Height;
                }
                else
                {
                    this.resizeWidthAnimation.StartWidth = null;
                    this.resizeHeightAnimation.StartHeight = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the final size of the animated element. If no value is applied current element size is used.
        /// </summary>
        public Size? EndSize
        {
            get
            {
                if (!this.resizeHeightAnimation.EndHeight.HasValue)
                {
                    return null;
                }

                if (!this.resizeWidthAnimation.EndWidth.HasValue)
                {
                    return null;
                }

                return new Size(this.resizeWidthAnimation.EndWidth.Value, this.resizeHeightAnimation.EndHeight.Value);
            }

            set
            {
                if (value.HasValue)
                {
                    this.resizeWidthAnimation.EndWidth = value.Value.Width;
                    this.resizeHeightAnimation.EndHeight = value.Value.Height;
                }
                else
                {
                    this.resizeWidthAnimation.EndWidth = null;
                    this.resizeHeightAnimation.EndHeight = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the duration of the animation. Defaults to (0:0:.4) - 400 milliseconds.
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
                this.resizeWidthAnimation.Duration = value;
                this.resizeHeightAnimation.Duration = value;
            }
        }
    }
}
