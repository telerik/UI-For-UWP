using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// Static manager class used for dynamic animations of controls.
    /// </summary>
    public static class RadAnimationManager
    {
        /// <summary>
        /// Represents the AnimationSelector attached property.
        /// </summary>
        public static readonly DependencyProperty AnimationSelectorProperty =
            DependencyProperty.RegisterAttached("AnimationSelector", typeof(AnimationSelectorBase), typeof(RadAnimationManager), new PropertyMetadata(null, OnAnimationSelectorChanged));

        /// <summary>
        /// Identifies the IsAnimationEnabled attached property.
        /// </summary>
        public static readonly DependencyProperty IsAnimationEnabledProperty =
            DependencyProperty.RegisterAttached("IsAnimationEnabled", typeof(bool), typeof(RadAnimationManager), new PropertyMetadata(true));

        private static readonly DependencyProperty CallbacksProperty =
            DependencyProperty.RegisterAttached("Callbacks", typeof(ICollection<Action>), typeof(RadAnimationManager), null);

        /// <summary>
        /// Identifies the Animation attached property.
        /// </summary>
        /// <remarks>
        ///        <para>
        ///            This property is used to bind the corresponding RadAnimation to a storyboard.
        ///        </para>
        /// </remarks>
        private static readonly DependencyProperty AnimationInfoProperty =
            DependencyProperty.RegisterAttached("AnimationInfo", typeof(PlayAnimationInfo), typeof(RadAnimationManager), null);

        private static readonly Dictionary<int, List<PlayAnimationInfo>> RunningAnimations = new Dictionary<int, List<PlayAnimationInfo>>();
        private static bool isAnimationEnabled = true;
        private static double globalSpeedRatio = 1;

        /// <summary>
        /// Gets or sets a value indicating whether the Animation for the whole application will be enabled.
        /// This value overrides all other properties.
        /// </summary>
        public static bool IsGlobalAnimationEnabled
        {
            get
            {
                return isAnimationEnabled;
            }

            set
            {
                isAnimationEnabled = value;
            }
        }

        /// <summary>
        /// Gets or sets the global animation speed ration that will be used if no local speed ratio is set.
        /// </summary>
        public static double SpeedRatio
        {
            get
            {
                return globalSpeedRatio;
            }

            set
            {
                globalSpeedRatio = value;
            }
        }

        /// <summary>
        /// Gets the AnimationSelector for the given DependencyObject, normally a control.
        /// </summary>
        /// <param name="obj">The target animated object, normally a control.</param>
        /// <returns>The animation selector for the object.</returns>
        public static AnimationSelectorBase GetAnimationSelector(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (AnimationSelectorBase)obj.GetValue(AnimationSelectorProperty);
        }

        /// <summary>
        /// Sets the Animation selector for the given DependencyObject, normally a Control.
        /// </summary>
        /// <param name="obj">The target animated object, normally a control.</param>
        /// <param name="value">The AnimationSelector to assign.</param>
        public static void SetAnimationSelector(DependencyObject obj, AnimationSelectorBase value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(AnimationSelectorProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether animation is enabled for the given Control.
        /// </summary>
        /// <param name="obj">The dependency object for which to check the value, normally a control.</param>
        /// <returns>True if animation is enabled, false otherwise.</returns>
        public static bool GetIsAnimationEnabled(DependencyObject obj)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            return (bool)obj.GetValue(IsAnimationEnabledProperty);
        }

        /// <summary>
        /// Sets a value indicating whether animation is enabled for the given Control.
        /// </summary>
        /// <param name="obj">The dependency object for which to check the value, normally a control.</param>
        /// <param name="value">True if animation should be enabled, false otherwise.</param>
        public static void SetIsAnimationEnabled(DependencyObject obj, bool value)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            obj.SetValue(IsAnimationEnabledProperty, value);
        }

        /// <summary>
        /// Determines whether animation is supported for the specified element.
        /// </summary>
        public static bool CanAnimate(DependencyObject target)
        {
            return GetIsAnimationEnabled(target) && IsGlobalAnimationEnabled;
        }

        /// <summary>
        /// Stops an animation if it is currently active or filling.
        /// </summary>
        /// <param name="target">The control to stop the animation for.</param>
        /// <param name="animation">The animation to stop.</param>
        public static void Stop(UIElement target, RadAnimation animation)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Storyboard storyboard = GetStoryboard(target, info => info.Target == target && info.Animation == animation);

            if (storyboard != null && storyboard.GetCurrentState() != ClockState.Stopped)
            {
                OnStoryboardCompleted(storyboard, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Stops an animation if it is currently active or filling.
        /// </summary>
        /// <param name="target">The target that is being animated.</param>
        /// <param name="animation">The animation to stop if running.</param>
        public static void StopIfRunning(UIElement target, RadAnimation animation)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Storyboard storyboard = GetStoryboard(target, info => info.Target == target && info.Animation == animation);
            if (storyboard != null && storyboard.GetCurrentState() == ClockState.Active)
            {
                OnStoryboardCompleted(storyboard, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Determines if the provided animation is running on the specified target.
        /// </summary>
        /// <param name="target">The target of the animation.</param>
        /// <param name="animation">The animation to check if it is running or not.</param>
        /// <returns>Returns true if the animation is animating the target and false otherwise.</returns>
        public static bool IsAnimationRunning(UIElement target, RadAnimation animation)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            Storyboard storyboard = GetStoryboard(target, info => info.Target == target && info.Animation == animation);
            return storyboard != null && storyboard.GetCurrentState() == ClockState.Active;
        }

        /// <summary>
        /// Plays the provides animation on the specified target.
        /// </summary>
        /// <param name="target">The <see cref="T:System.Windows.UIElement"/> instance to be animated.</param>
        /// <param name="animation">The <see cref="RadAnimation"/> instance that describes the animation process.</param>
        /// <returns>True if the animation has been successfully executed, false otherwise.</returns>
        public static bool Play(UIElement target, RadAnimation animation)
        {
            return Play(target, animation, null);
        }

        /// <summary>
        /// Plays the provides animation on the specified target.
        /// </summary>
        /// <param name="target">The <see cref="T:System.Windows.UIElement"/> instance to be animated.</param>
        /// <param name="animation">The <see cref="RadAnimation"/> instance that describes the animation process.</param>
        /// <param name="completedCallback">Optional callback to notify the caller for animation completion.</param>
        /// <returns>True if the animation has been successfully executed, false otherwise.</returns>
        public static bool Play(UIElement target, RadAnimation animation, Action completedCallback)
        {
            if (target == null)
            {
                throw new ArgumentNullException(nameof(target));
            }

            if (animation == null)
            {
                throw new ArgumentNullException(nameof(animation));
            }

            PlayAnimationInfo info = GetAnimationInfo(arg => arg.Target == target && arg.Animation == animation);
            if (info != null)
            {
                StopStoryboard(info);
            }

            Storyboard storyboard = animation.CreateStoryboard(target);
            info = CreatePlayAnimationInfo(target, animation, storyboard);

            return PlayCore(info, completedCallback);
        }

        internal static void ForceStop(UIElement element, RadAnimation animation)
        {
            if (element == null)
            {
                throw new ArgumentNullException(nameof(element));
            }

            Storyboard storyboard = GetStoryboard(element, info => info.Target == element && info.Animation == animation);

            if (storyboard != null)
            {
                OnStoryboardCompleted(storyboard, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Returns a boolean value determining whether an animation has been scheduled for the given <see cref="UIElement"/>.
        /// </summary>
        /// <param name="element">The <see cref="UIElement"/> instance for which to check whether the given <see cref="RadAnimation"/> is scheduled.</param>
        /// <param name="animation">The <see cref="RadAnimation"/> instance for which to check whether it has been scheduled for the given <see cref="UIElement"/>.</param>
        /// <returns>True if an animation is scheduled for the element, otherwise false.</returns>
        internal static bool IsAnimationScheduled(UIElement element, RadAnimation animation)
        {
            return GetStoryboard(element, info => info.Target == element && info.Animation == animation) != null;
        }

        private static bool PlayCore(PlayAnimationInfo animationInfo, Action completeCallback, params object[] args)
        {
            if (!CanAnimate(animationInfo.Target))
            {
                BeginInvokeCallback(completeCallback, animationInfo.Target);
                return false;
            }

            Storyboard storyboard = animationInfo.Storyboard;

            // associate the animation info with the storyboard
            storyboard.SetValue(AnimationInfoProperty, animationInfo);
            storyboard.Completed += OnStoryboardCompleted;

            if (completeCallback != null)
            {
                AddCallback(storyboard, completeCallback);
            }

            animationInfo.Animation.UpdateAnimation(animationInfo.Target, storyboard, args);
            storyboard.Begin();
            animationInfo.Animation.OnStarted(animationInfo);

            return true;
        }

        private static PlayAnimationInfo CreatePlayAnimationInfo(UIElement target, RadAnimation animation, Storyboard storyboard)
        {
            int hash = target.GetHashCode();

            List<PlayAnimationInfo> animations = null;

            PlayAnimationInfo result = new PlayAnimationInfo(storyboard, animation, target);
            if (!RunningAnimations.TryGetValue(hash, out animations))
            {
                animations = new List<PlayAnimationInfo>();
                animations.Add(result);
                RunningAnimations[hash] = animations;
            }
            else
            {
                animations.Add(result);
            }

            return result;
        }

        private static Storyboard GetStoryboard(UIElement target, Func<PlayAnimationInfo, bool> predicate)
        {
            List<PlayAnimationInfo> infos = null;
            if (!RunningAnimations.TryGetValue(target.GetHashCode(), out infos))
            {
                return null;
            }

            PlayAnimationInfo result = infos.SingleOrDefault<PlayAnimationInfo>(arg => predicate(arg));

            return result == null ? null : result.Storyboard;
        }

        private static PlayAnimationInfo GetAnimationInfo(Func<PlayAnimationInfo, bool> predicate)
        {
            foreach (List<PlayAnimationInfo> playPasses in RunningAnimations.Values)
            {
                foreach (PlayAnimationInfo info in playPasses)
                {
                    if (predicate(info))
                    {
                        return info;
                    }
                }
            }

            return null;
        }

        private static void RemoveAnimationInfo(PlayAnimationInfo info)
        {
            int targetHash = info.TargetHashCode;
            List<PlayAnimationInfo> playPasses = RunningAnimations[targetHash];
            Debug.Assert(playPasses != null && playPasses.Count > 0, "PlayInfo must be registered.");
            playPasses.Remove(info);

            if (playPasses.Count == 0)
            {
                RunningAnimations.Remove(targetHash);
            }

            info.Storyboard.ClearValue(AnimationInfoProperty);
        }

        private static void AddCallback(Storyboard storyboard, Action callback)
        {
            var callbacks = GetCallbacks(storyboard);
            if (callbacks == null)
            {
                SetCallbacks(storyboard, new List<Action>() { callback });
            }
            else
            {
                callbacks.Add(callback);
            }
        }

        private static void InvokeCallbacks(Storyboard storyboard)
        {
            if (storyboard == null)
            {
                return;
            }

            ICollection<Action> callbacks = GetCallbacks(storyboard);

            foreach (Timeline child in storyboard.Children)
            {
                Storyboard childStoryboard = child as Storyboard;
                if (childStoryboard != null)
                {
                    InvokeCallbacks(childStoryboard);
                }
            }

            if (callbacks != null)
            {
                foreach (Action callback in callbacks)
                {
                    callback();
                }

                callbacks.Clear();
            }
        }

        private static async void BeginInvokeCallback(Action callback, DependencyObject target)
        {
            if (callback != null)
            {
                await target.Dispatcher.RunAsync(
                      Windows.UI.Core.CoreDispatcherPriority.Normal,
                      () =>
                      {
                          callback.Invoke();
                      });
            }
        }

        private static void OnStoryboardCompleted(object sender, object e)
        {
            Storyboard storyboard = sender as Storyboard;
            PlayAnimationInfo info = storyboard.GetValue(AnimationInfoProperty) as PlayAnimationInfo;
            Debug.Assert(info != null, "Must have PlayAnimationInfo associated.");
            StopStoryboard(info);
        }

        private static void StopStoryboard(PlayAnimationInfo info)
        {
            // try to notify the associated animation for stopping the storyboard
            // this is needed to record the final animation values as Local if needed
            info.Animation.OnStopping(info);

            info.Storyboard.Stop();
            info.Animation.OnStopped(info);

            info.Storyboard.Children.Clear();
            info.Storyboard.Completed -= OnStoryboardCompleted;

            RemoveAnimationInfo(info);
            InvokeCallbacks(info.Storyboard);
        }

        private static ICollection<Action> GetCallbacks(DependencyObject obj)
        {
            return obj.GetValue(CallbacksProperty) as ICollection<Action>;
        }

        private static void SetCallbacks(DependencyObject obj, ICollection<Action> value)
        {
            obj.SetValue(CallbacksProperty, value);
        }

        private static void OnAnimationSelectorChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            // Make sure property is set to the children as well:
            ItemsControl itemsControl = sender as ItemsControl;
            if (itemsControl != null)
            {
                foreach (object item in itemsControl.Items)
                {
                    UIElement container = itemsControl.ContainerFromItem(item) as UIElement;
                    if (container != null)
                    {
                        SetAnimationSelector(container, e.NewValue as AnimationSelectorBase);
                    }
                }
            }
        }
    }
}