using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using Telerik.Charting;
using Telerik.UI.Xaml.Controls.Chart;
using Windows.UI.Composition;

namespace SDKExamples.UWP.Chart
{
    public class AnimationContainerVisualsFactory : ContainerVisualsFactory
    {
        private int slideDelay = 0;
        private int moveOffsetChange = 400;
        private int moveDelay = 50;
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

        protected override bool CanDrawContainerVisual(PresenterBase visualElement)
        {
            if (visualElement is LineSeries || visualElement is ScatterLineSeries 
                || visualElement is BollingerBandsIndicator)
            {
                return false;
            }

            return base.CanDrawContainerVisual(visualElement);
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
                        return this.TriggerMoveAnimation(pointTemplate, 100);
                    }

                    return this.TriggerSlideAnimation(pointTemplate, moveDelay);
                }
            }
            else if (pointTemplate.Orientation.X == 1)
            {
                pointTemplate.Orientation = new Quaternion(0, 0, 0, 1);
            }

            return pointTemplate;
        }

        private ContainerVisual TriggerMoveAnimation(ContainerVisual colorVisual, int delay, int duration = 500)
        {
            Compositor compositor = colorVisual.Compositor;
            var animation = compositor.CreateVector3KeyFrameAnimation();

            var oldOffset = colorVisual.Offset;
            colorVisual.Offset = new Vector3(oldOffset.X, oldOffset.Y + moveOffsetChange, 0);
            animation.DelayTime = TimeSpan.FromMilliseconds(slideDelay);
            animation.InsertKeyFrame(1f, new Vector3(oldOffset.X, oldOffset.Y, 0));
            animation.Duration = TimeSpan.FromMilliseconds(duration);
            colorVisual.StartAnimation("Offset", animation);
            slideDelay += delay;

            return colorVisual;
        }

        private ContainerVisual TriggerSlideAnimation(ContainerVisual colorVisual, int delay, int duration = 500)
        {
            var size = colorVisual.Size;
            Compositor compositor = colorVisual.Compositor;
            ScalarKeyFrameAnimation slideAnimation = compositor.CreateScalarKeyFrameAnimation();

            colorVisual.Offset = new Vector3(colorVisual.Offset.X, colorVisual.Offset.Y + colorVisual.Size.Y, 0);
            colorVisual.Orientation = new Quaternion(1, 0, 0, 0);
            colorVisual.Size = new Vector2(size.X, 0);

            if (this.previousSpriteVisual != null && this.previousSpriteVisual.Offset.X == colorVisual.Offset.X)
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
            this.previousSpriteVisual = colorVisual;
            
            return colorVisual;
        }
    }
}
