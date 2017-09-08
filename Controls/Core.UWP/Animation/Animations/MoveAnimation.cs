using System;
using System.Diagnostics.CodeAnalysis;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.Core
{
    /// <summary>
    /// Moves the target element from a start point to an end point.
    /// </summary>
    public class RadMoveAnimation : RadAnimationGroup
    {
        private MoveAnimationPointMode pointMode;

        private Point? startPoint;
        private Point? endPoint;
        private MoveDirection? moveDirection;

        private RadMoveXAnimation xAnimation = new RadMoveXAnimation();
        private RadMoveYAnimation yAnimation = new RadMoveYAnimation();

        /// <summary>
        /// Initializes a new instance of the <see cref="RadMoveAnimation"/> class.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors", Justification = "These virtual calls do not rely on uninitialized base state.")]
        public RadMoveAnimation()
        {
            this.Duration = new Duration(TimeSpan.FromSeconds(.4));

            this.Children.Add(this.xAnimation);
            this.Children.Add(this.yAnimation);
        }

        /// <summary>
        /// Gets or sets the middle points of the X animation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public DoubleKeyFrameCollection MiddlePointsXAxis
        {
            get
            {
                return this.xAnimation.MiddlePointsAxis;
            }

            set
            {
                this.xAnimation.MiddlePointsAxis = value;
            }
        }

        /// <summary>
        /// Gets or sets the middle points of the Y animation.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public DoubleKeyFrameCollection MiddlePointsYAxis
        {
            get
            {
                return this.yAnimation.MiddlePointsAxis;
            }

            set
            {
                this.yAnimation.MiddlePointsAxis = value;
            }
        }

        /// <summary>
        /// Gets or sets a value that describes from to where the animated element should move.
        /// For more precise control use the StartPoint and EndPoint and set MoveDirection to null.
        /// Move direction has precedence over StartPoint and EndPoint.
        /// </summary>
        public MoveDirection? MoveDirection
        {
            get
            {
                return this.moveDirection;
            }

            set
            {
                this.moveDirection = value;
            }
        }

        /// <summary>
        /// Gets or sets the start position for the animation.
        /// If not set, the current element TranslateTransform (if any) is used or an empty Point - that is [0,0].
        /// If MoveDirection is set this property is disregarded.
        /// </summary>
        public Point? StartPoint
        {
            get
            {
                return this.startPoint;
            }

            set
            {
                this.startPoint = value;
                if (value.HasValue)
                {
                    this.xAnimation.StartX = value.Value.X;
                    this.yAnimation.StartY = value.Value.Y;
                }
                else
                {
                    this.xAnimation.StartX = null;
                    this.yAnimation.StartY = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets the end position of the animation.
        /// If not set, the current element TranslateTransform (if any) is used or an empty Point - that is [0,0].
        /// If MoveDirection is set this property is disregarded.
        /// </summary>
        public Point? EndPoint
        {
            get
            {
                return this.endPoint;
            }

            set
            {
                this.endPoint = value;
                if (value.HasValue)
                {
                    this.xAnimation.EndX = value.Value.X;
                    this.yAnimation.EndY = value.Value.Y;
                }
                else
                {
                    this.xAnimation.EndX = null;
                    this.yAnimation.EndY = null;
                }
            }
        }

        /// <summary>
        /// Gets or sets a value that determines how the move animation's start and end points will be interpreted.
        /// Absolute means that they will be interpreted as pixels and Relative means that they will interpreted as
        /// points in the [-1, 1] range.
        /// </summary>
        public MoveAnimationPointMode PointMode
        {
            get
            {
                return this.pointMode;
            }

            set
            {
                this.pointMode = value;
                this.xAnimation.PointMode = value;
                this.yAnimation.PointMode = value;
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
                this.xAnimation.Duration = value;
                this.yAnimation.Duration = value;
            }
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            RadMoveAnimation reverse = base.CreateOpposite() as RadMoveAnimation;
            if (this.MoveDirection.HasValue)
            {
                reverse.MoveDirection = GetOppositeDirection(this.MoveDirection.Value);
            }

            return reverse;
        }

        /// <summary>
        /// Gets the value of a nullable point if it is not null. If it is null it returns a point that represents the X and Y
        /// components of the target's TranslateTransform.
        /// </summary>
        /// <param name="target">The target from which to obtain a point if the provided point is null.</param>
        /// <param name="point">A nullable point.</param>
        /// <returns>Returns the value of a nullable point if it is not null. If it is null it returns a point that represents the X and Y
        /// components of the target's TranslateTransform.</returns>
        protected static Point GetPoint(UIElement target, Point? point)
        {
            if (point.HasValue)
            {
                return point.Value;
            }

            TranslateTransform transform = target.GetTranslateTransform();
            return new Point(transform.X, transform.Y);
        }

        /// <summary>
        /// Gets the move direction value, opposite (mirrored) to the specified one.
        /// </summary>
        /// <param name="direction">The direction which opposite value should be calculated.</param>
        protected static MoveDirection GetOppositeDirection(MoveDirection direction)
        {
            switch (direction)
            {
                case Telerik.Core.MoveDirection.BottomIn:
                    return Telerik.Core.MoveDirection.TopOut;

                case Telerik.Core.MoveDirection.BottomOut:
                    return Telerik.Core.MoveDirection.TopIn;

                case Telerik.Core.MoveDirection.LeftIn:
                    return Telerik.Core.MoveDirection.RightOut;

                case Telerik.Core.MoveDirection.LeftOut:
                    return Telerik.Core.MoveDirection.RightIn;

                case Telerik.Core.MoveDirection.RightIn:
                    return Telerik.Core.MoveDirection.LeftOut;

                case Telerik.Core.MoveDirection.RightOut:
                    return Telerik.Core.MoveDirection.LeftIn;

                case Telerik.Core.MoveDirection.TopIn:
                    return Telerik.Core.MoveDirection.BottomOut;

                case Telerik.Core.MoveDirection.TopOut:
                    return Telerik.Core.MoveDirection.BottomIn;

                default:
                    throw new ArgumentException("Unknown MoveDirection");
            }
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

            if (this.MoveDirection.HasValue)
            {
                Pair<Point, Point> points = this.GetStartEndPoints(context.Target as FrameworkElement);
                this.xAnimation.StartX = points.First.X;
                this.yAnimation.StartY = points.First.Y;
                this.xAnimation.EndX = points.Second.X;
                this.yAnimation.EndY = points.Second.Y;
            }

            base.UpdateAnimationOverride(context);
        }

        /// <summary>
        /// Sets the start and end point arguments based on the current MoveDirection value.
        /// </summary>
        /// <param name="element">The target which is required in order to determine the move length. The move length is equal to the render size of target.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1702:CompoundWordsShouldBeCasedCorrectly", MessageId = "EndPoints")]
        protected Pair<Point, Point> GetStartEndPoints(FrameworkElement element)
        {
            if (element == null)
            {
                return new Pair<Point, Point>(new Point(), new Point());
            }

            Point start;
            Point end;

            switch (this.MoveDirection.Value)
            {
                case Telerik.Core.MoveDirection.BottomIn:
                    start = new Point(0, -element.ActualHeight);
                    end = new Point(0, 0);
                    break;

                case Telerik.Core.MoveDirection.BottomOut:
                    start = new Point(0, 0);
                    end = new Point(0, element.ActualHeight);
                    break;

                case Telerik.Core.MoveDirection.LeftIn:
                    start = new Point(element.ActualWidth, 0);
                    end = new Point(0, 0);
                    break;

                case Telerik.Core.MoveDirection.LeftOut:
                    start = new Point(0, 0);
                    end = new Point(-element.ActualWidth, 0);
                    break;

                case Telerik.Core.MoveDirection.RightIn:
                    start = new Point(-element.ActualWidth, 0);
                    end = new Point(0, 0);
                    break;

                case Telerik.Core.MoveDirection.RightOut:
                    start = new Point(0, 0);
                    end = new Point(element.ActualWidth, 0);
                    break;

                case Telerik.Core.MoveDirection.TopIn:
                    start = new Point(0, element.ActualHeight);
                    end = new Point(0, 0);
                    break;

                case Telerik.Core.MoveDirection.TopOut:
                    start = new Point(0, 0);
                    end = new Point(0, -element.ActualHeight);
                    break;

                default:
                    start = new Point();
                    end = new Point();
                    break;
            }

            return new Pair<Point, Point>(start, end);
        }
    }
}