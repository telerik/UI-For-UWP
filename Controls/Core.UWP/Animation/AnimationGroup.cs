using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics.CodeAnalysis;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Markup;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// Represents composite animation, used for declaratively creating composite animations.
    /// </summary>
    [ContentProperty(Name = "Children")]
    public class RadAnimationGroup : RadAnimation
    {
        private ObservableCollection<RadAnimation> children;

        /// <summary>
        /// Initializes a new instance of the <see cref="RadAnimationGroup"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadAnimationGroup()
        {
            this.Duration = new Duration(TimeSpan.FromSeconds(1));
            this.children = new ObservableCollection<RadAnimation>();
            this.children.CollectionChanged += this.OnChildrenChanged;
        }

        /// <summary>
        /// Gets a list of the children animation objects of this composite animation.
        /// </summary>
        public Collection<RadAnimation> Children
        {
            get
            {
                return this.children;
            }
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            return this.CloneGroup((child) => child.CreateOpposite());
        }

        /// <summary>
        /// Removes any property modifications, applied to the specified element by this instance.
        /// </summary>
        /// <param name="target">The element which property values are to be cleared.</param>
        /// <remarks>
        /// It is assumed that the element has been previously animated by this animation.
        /// </remarks>
        public override void ClearAnimation(UIElement target)
        {
            foreach (RadAnimation animation in this.Children)
            {
                animation.ClearAnimation(target);
            }

            base.ClearAnimation(target);
        }

        /// <summary>
        /// Sets the initial animation values to the provided target element.
        /// </summary>
        /// <param name="target">The target element.</param>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);

            foreach (RadAnimation animation in this.children)
            {
                animation.ApplyInitialValues(target);
            }
        }

        /// <summary>
        /// Applies already stored (if any) animated values.
        /// </summary>
        /// <param name="info">The animation info.</param>
        protected internal override void ApplyAnimationValues(PlayAnimationInfo info)
        {
            foreach (RadAnimation animation in this.Children)
            {
                animation.ApplyAnimationValues(info);
            }
        }

        /// <summary>
        /// Creates a clone animation of this instance.
        /// </summary>
        /// <returns>Returns a clone of this animation.</returns>
        protected override RadAnimation CloneCore()
        {
            return this.CloneGroup((child) => child.Clone());
        }

        /// <summary>
        /// Core create routine.
        /// </summary>
        /// <param name="target">The targeted element of the animation.</param>
        /// <returns>The newly created animation.</returns>
        protected override Storyboard CreateStoryboardOverride(UIElement target)
        {
            Storyboard result = base.CreateStoryboardOverride(target);

            foreach (RadAnimation animation in this.Children)
            {
                Storyboard child = animation.CreateStoryboard(target);
                result.Children.Add(child);
            }

            return result;
        }

        /// <summary>
        /// Core update routine.
        /// </summary>
        /// <param name="context">The context that holds information about the animation.</param>
        protected override void UpdateAnimationOverride(AnimationContext context)
        {
            if (context == null)
            {
                return;
            }

            int currentIndex = 0;
            foreach (RadAnimation animation in this.Children)
            {
                if (animation.Duration > this.Duration)
                {
                    // Clamp durations which are greater than that of the group so that we can be sure that all animations
                    // will stop at their end values.
                    animation.Duration = this.Duration;
                }

                Storyboard childBoard = context.Storyboard.Children[currentIndex++] as Storyboard;
                animation.UpdateAnimation(context.Target, childBoard, context.Arguments);
            }
        }

        private RadAnimationGroup CloneGroup(Func<RadAnimation, RadAnimation> cloneGenerator)
        {
            RadAnimationGroup cloneGroup = base.CloneCore() as RadAnimationGroup;
            cloneGroup.children = new ObservableCollection<RadAnimation>();
            foreach (RadAnimation child in this.children)
            {
                cloneGroup.children.Add(cloneGenerator(child));
            }

            return cloneGroup;
        }

        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (RadAnimation added in e.NewItems)
                {
                    added.Parent = this;
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (RadAnimation added in e.OldItems)
                {
                    added.Parent = null;
                }
            }
        }
    }
}
