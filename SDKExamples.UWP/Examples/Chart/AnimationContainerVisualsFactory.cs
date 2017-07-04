using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Telerik.Charting;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.Foundation;
using Windows.UI.Composition;
using Windows.UI.Xaml.Media;

namespace SDKExamples.UWP.Chart
{
    public class AnimationContainerVisualsFactory : ContainerVisualsFactory
    {
        private static int slideDelay = 0;
        private const int minDelay = 5;
        private const int maxDelay = 150;
        private const int moveOffsetChange = 400;
        private const int moveDuration = 900;
        private const int moveDelay = 50;
        private bool isAnimationEnabled;
        private ContainerVisual previousSpriteVisual;
       
        public AnimationContainerVisualsFactory()
        {
            this.IsAnimationEnabled = true;
            this.AnimationsRun = new ObservableCollection<ContainerVisual>();
        }

        public ObservableCollection<ContainerVisual> AnimationsRun { get; set; }

        public bool IsAnimationEnabled
        {
            get
            {
                return this.isAnimationEnabled;
            }
            set
            {
                this.isAnimationEnabled = value;
            }
        }

        public void TriggerOrderedVisualsAnimation(int delay, int duration)
        {
            if (this.AnimationsRun.Count > 0)
            {
                var orderedVisualContainers = this.AnimationsRun.OrderBy(a => a.Offset.X);
                foreach (var spriteVisual in orderedVisualContainers)
                {
                    this.TriggerSlideAnimation(spriteVisual, delay, duration);
                }
            }
        }

        protected override ContainerVisual PreparePointTemplateSeriesVisual(ContainerVisual containerVisual, DataPoint dataPoint)
        {
            var pointTemplate = base.PreparePointTemplateSeriesVisual(containerVisual, dataPoint);

            if (!this.AnimationsRun.Contains(pointTemplate))
            {
                this.AnimationsRun.Add(pointTemplate);
                if (this.IsAnimationEnabled)
                {
                    if (dataPoint is OhlcDataPoint)
                    {
                        return this.TriggerMoveAnimation(pointTemplate);
                    }

                    return this.TriggerSlideAnimation(pointTemplate, 50);
                }
            }
            else if(pointTemplate.Orientation.X == 1)
            {
                pointTemplate.Orientation = new Quaternion(0, 0, 0, 1);
            }

            return pointTemplate;
        }

        protected override ContainerVisual PrepareLineRenderVisual(ContainerVisual containerVisual, IEnumerable<Point> points, Brush stroke, double strokeThickness)
        {
            var lineSeries = base.PrepareLineRenderVisual(containerVisual, points, stroke, strokeThickness);
            if (!this.AnimationsRun.Contains(lineSeries))
            {
                this.AnimationsRun.Add(lineSeries);
                if (this.IsAnimationEnabled)
                {
                    return this.TriggerLineSlideAnimation(lineSeries);
                }
            }

            return lineSeries;
        }

        private ContainerVisual TriggerMoveAnimation(ContainerVisual colorVisual)
        {
            var spriteVisual = colorVisual as SpriteVisual;
            if (spriteVisual != null)
            {
                Compositor compositor = colorVisual.Compositor;
                var animation = compositor.CreateVector3KeyFrameAnimation();
                var oldOffset = spriteVisual.Offset;
                spriteVisual.Offset = new Vector3(oldOffset.X, oldOffset.Y + moveOffsetChange, 0);

                animation.DelayTime = TimeSpan.FromMilliseconds(slideDelay);
                animation.InsertKeyFrame(1f, new Vector3(oldOffset.X, oldOffset.Y, 0));
                animation.Duration = TimeSpan.FromMilliseconds(moveDuration);
                spriteVisual.StartAnimation("Offset", animation);
                slideDelay += moveDelay;
            }

            return spriteVisual;
        }

        private ContainerVisual TriggerSlideAnimation(ContainerVisual colorVisual, int delay, int duration = 500)
        {
            var spriteVisual = colorVisual as SpriteVisual;
            if (spriteVisual != null)
            {
                var size = spriteVisual.Size;
                Compositor compositor = colorVisual.Compositor;
                ScalarKeyFrameAnimation slideAnimation = compositor.CreateScalarKeyFrameAnimation();

                spriteVisual.Offset = new Vector3(spriteVisual.Offset.X, spriteVisual.Offset.Y + spriteVisual.Size.Y, 0);
                spriteVisual.Orientation = new Quaternion(1, 0, 0, 0);
                spriteVisual.Size = new Vector2(size.X, 0);

                if (this.previousSpriteVisual != null && this.previousSpriteVisual.Offset.X == spriteVisual.Offset.X)
                {
                    slideDelay += (duration - delay);
                }
                else
                {
                    slideDelay += delay;
                }

                slideAnimation.DelayTime = TimeSpan.FromMilliseconds(slideDelay);
                slideAnimation.InsertKeyFrame(1f, size.Y);
                slideAnimation.Duration = TimeSpan.FromMilliseconds(duration);
                colorVisual.StartAnimation("Size.Y", slideAnimation);
                this.previousSpriteVisual = spriteVisual;
                
            }
            
            return spriteVisual;
        }

        private ContainerVisual TriggerLineSlideAnimation(ContainerVisual lineSeries)
        {
            var spriteVisual = lineSeries as SpriteVisual;
            if (spriteVisual != null && spriteVisual.Children.Count > 0)
            {
                int delay = lineSeries.Children.Count > 50 ? minDelay : maxDelay;
                int animationDuration = delay;
                for (int i = spriteVisual.Children.Count - 1; i >= 0; i--)
                {
                    var childSpriteVisual = spriteVisual.Children.ElementAt(i);
                    if (childSpriteVisual != null)
                    {
                        var size = childSpriteVisual.Size;
                        Compositor compositor = lineSeries.Compositor;
                        ScalarKeyFrameAnimation slideAnimation = compositor.CreateScalarKeyFrameAnimation();
                        childSpriteVisual.Size = new Vector2(0, size.Y);

                        slideAnimation.InsertKeyFrame(1f, size.X);
                        if (i < (spriteVisual.Children.Count - 1))
                        {
                            slideAnimation.DelayTime = TimeSpan.FromMilliseconds(delay);
                            delay += animationDuration;
                        }

                        slideAnimation.Duration = TimeSpan.FromMilliseconds(animationDuration);
                        childSpriteVisual.StartAnimation("Size.X", slideAnimation);
                    }
                }
            }

            return spriteVisual;
        }
    }
}
