using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Telerik.Core
{
    /// <summary>
    /// This enumeration can be used to obtain a partially reversed perspective animation.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue")]
    [Flags]
    public enum ReverseMode
    {
        /// <summary>
        /// Reverse start and end angles.
        /// </summary>
        StartEndAngles,

        /// <summary>
        /// Reverse direction.
        /// </summary>
        RotationDirection,
    }

    /// <summary>
    /// Predefined possible axes in the perspective animation.
    /// </summary>
    [Flags]
    public enum PerspectiveAnimationAxis
    {
        /// <summary>
        /// The X axis.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "X")]
        X = 1,

        /// <summary>
        /// The Y axis.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Y")]
        Y = X << 1,

        /// <summary>
        /// The Z axis.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Z")]
        Z = Y << 1,

        /// <summary>
        /// Both X and Y axis.
        /// </summary>
        XY = X | Y,

        /// <summary>
        /// Both X and Z axis.
        /// </summary>
        XZ = X | Z,

        /// <summary>
        /// Both Y and Z axis.
        /// </summary>
        YZ = Y | Z,

        /// <summary>
        /// All axes.
        /// </summary>
        All = X | Y | Z,
    }

    /// <summary>
    /// Defines the direction of a perspective animation.
    /// </summary>
    public enum PerspectiveAnimationDirection
    {
        /// <summary>
        /// Animation is clock-wise orientated.
        /// </summary>
        Clockwise,

        /// <summary>
        /// Animation is counter clock-wise orientated.
        /// </summary>
        CounterClockwise,
    }

    /// <summary>
    /// Defines animation that changes the perspective of the target element.
    /// </summary>
    public class RadPlaneProjectionAnimation : RadAnimation
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RadPlaneProjectionAnimation"/> class.
        /// </summary>
        public RadPlaneProjectionAnimation()
        {
            this.Axes = PerspectiveAnimationAxis.Y;
            this.Direction = PerspectiveAnimationDirection.CounterClockwise;

            this.StartAngleX = 90;
            this.StartAngleY = 90;
            this.StartAngleZ = 90;
            this.EndAngleX = 0;
            this.EndAngleY = 0;
            this.EndAngleZ = 0;
        }

        /// <summary>
        /// Gets or sets the starting angle of the perspective rotation along the X axis.
        /// </summary>
        public double StartAngleX { get; set; }

        /// <summary>
        /// Gets or sets the end angle of the perspective rotation along the X axis.
        /// </summary>
        public double EndAngleX { get; set; }

        /// <summary>
        /// Gets or sets the starting angle of the perspective rotation along the Y axis.
        /// </summary>
        public double StartAngleY { get; set; }

        /// <summary>
        /// Gets or sets the end angle of the perspective rotation along the Y axis.
        /// </summary>
        public double EndAngleY { get; set; }

        /// <summary>
        /// Gets or sets the starting angle of the perspective rotation along the Z axis.
        /// </summary>
        public double StartAngleZ { get; set; }

        /// <summary>
        /// Gets or sets the end angle of the perspective rotation along the Z axis.
        /// </summary>
        public double EndAngleZ { get; set; }

        /// <summary>
        /// Gets or sets the center of the rotation along the X axis.
        /// </summary>
        public double? CenterX { get; set; }

        /// <summary>
        /// Gets or sets the center of the rotation along the Y axis.
        /// </summary>
        public double? CenterY { get; set; }

        /// <summary>
        /// Gets or sets the center of the rotation along the Z axis.
        /// </summary>
        public double? CenterZ { get; set; }

        /// <summary>
        /// Gets or sets the axes along which rotation should occur.
        /// </summary>
        public PerspectiveAnimationAxis Axes { get; set; }

        /// <summary>
        /// Gets or sets the direction of the animation.
        /// That is the rotation direction - clockwise or counter-clockwise.
        /// </summary>
        public PerspectiveAnimationDirection Direction { get; set; }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <returns>A new instance of this animation that is the reverse of this instance.</returns>
        public override RadAnimation CreateOpposite()
        {
            return this.CreateOpposite(ReverseMode.StartEndAngles);
        }

        /// <summary>
        /// Creates a new instance of this animation that is the reverse of this instance.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns>
        /// A new instance of this animation that is the reverse of this instance.
        /// </returns>
        public RadAnimation CreateOpposite(ReverseMode mode)
        {
            RadPlaneProjectionAnimation reverse = base.CreateOpposite() as RadPlaneProjectionAnimation;
            if ((mode & ReverseMode.StartEndAngles) == ReverseMode.StartEndAngles)
            {
                ReverseAngles(reverse);
            }

            if ((mode & ReverseMode.RotationDirection) == ReverseMode.RotationDirection)
            {
                ReverseDirection(reverse);
            }

            return reverse;
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
            if (target != null)
            {
                target.Projection = null;
            }
        }

        /// <inheritdoc/>
        public override void ApplyInitialValues(UIElement target)
        {
            base.ApplyInitialValues(target);

            AnimationContext context = new AnimationContext(target);
            if (this.CenterX.HasValue)
            {
                context.InitializeCenterOfRotationX(this.CenterX.Value);
            }

            if (this.CenterY.HasValue)
            {
                context.InitializeCenterOfRotationY(this.CenterY.Value);
            }

            if (this.CenterZ.HasValue)
            {
                context.InitializeCenterOfRotationZ(this.CenterZ.Value);
            }

            context.InitializeRotationX(this.StartAngleX);
            context.InitializeRotationY(this.StartAngleY);
            context.InitializeRotationZ(this.StartAngleZ);
        }

        /// <inheritdoc/>
        protected internal override void ApplyAnimationValues(PlayAnimationInfo info)
        {
            if (info == null)
            {
                return;
            }

            PlaneProjection projection = info.Target.Projection as PlaneProjection;
            if (projection == null)
            {
                return;
            }

            double direction = this.Direction == PerspectiveAnimationDirection.Clockwise ? -1 : 1;
            double rotation;

            PerspectiveAnimationAxis axes = this.Axes;
            bool autoReverse = this.GetAutoReverse();

            if ((axes & PerspectiveAnimationAxis.X) == PerspectiveAnimationAxis.X)
            {
                rotation = autoReverse ? this.StartAngleX : this.EndAngleX;
                projection.RotationX = rotation * direction;
            }

            if ((axes & PerspectiveAnimationAxis.Y) == PerspectiveAnimationAxis.Y)
            {
                rotation = autoReverse ? this.StartAngleY : this.EndAngleY;
                projection.RotationY = rotation * direction;
            }

            if ((axes & PerspectiveAnimationAxis.Z) == PerspectiveAnimationAxis.Z)
            {
                rotation = autoReverse ? this.StartAngleZ : this.EndAngleZ;
                projection.RotationZ = rotation * direction;
            }
        }

        /// <summary>
        /// Core update routine.
        /// </summary>
        /// <param name="context">The context that holds information about the animation.</param>
        protected override void UpdateAnimationOverride(AnimationContext context)
        {
            this.SetPlaneProjection(context);
            this.AnimateAxes(context);
            base.UpdateAnimationOverride(context);
        }

        private static void ReverseDirection(RadPlaneProjectionAnimation animation)
        {
            if (animation.Direction == PerspectiveAnimationDirection.Clockwise)
            {
                animation.Direction = PerspectiveAnimationDirection.CounterClockwise;
            }
            else
            {
                animation.Direction = PerspectiveAnimationDirection.Clockwise;
            }
        }

        private static void ReverseAngles(RadPlaneProjectionAnimation animation)
        {
            double tmp = animation.StartAngleX;
            animation.StartAngleX = animation.EndAngleX;
            animation.EndAngleX = tmp;

            tmp = animation.StartAngleY;
            animation.StartAngleY = animation.EndAngleY;
            animation.EndAngleY = tmp;

            tmp = animation.StartAngleZ;
            animation.StartAngleZ = animation.EndAngleZ;
            animation.EndAngleZ = tmp;
        }

        private void AnimateAxes(AnimationContext context)
        {
            double from;
            double to;

            PerspectiveAnimationAxis axes = this.Axes;
            double duration = this.Duration.TimeSpan.TotalSeconds;

            if ((axes & PerspectiveAnimationAxis.X) == PerspectiveAnimationAxis.X)
            {
                // animate the X-axis
                from = this.StartAngleX;
                to = this.EndAngleX;
                this.UpdateStartEnd(ref from, ref to);
                context.RotationX(new double[] { 0, from, duration, to });
            }

            if ((axes & PerspectiveAnimationAxis.Y) == PerspectiveAnimationAxis.Y)
            {
                // animate the Y-axis
                from = this.StartAngleY;
                to = this.EndAngleY;
                this.UpdateStartEnd(ref from, ref to);
                context.RotationY(new double[] { 0, from, duration, to });
            }

            if ((axes & PerspectiveAnimationAxis.Z) == PerspectiveAnimationAxis.Z)
            {
                // animate the Y-axis
                from = this.StartAngleZ;
                to = this.EndAngleZ;
                this.UpdateStartEnd(ref from, ref to);
                context.RotationZ(new double[] { 0, from, duration, to });
            }
        }

        private void UpdateStartEnd(ref double from, ref double to)
        {
            if (this.Direction == PerspectiveAnimationDirection.Clockwise)
            {
                from *= -1;
                to *= -1;
            }
        }

        private void SetPlaneProjection(AnimationContext context)
        {
            if (context.Target.Projection == null)
            {
                PlaneProjection projection = new PlaneProjection();
                context.Target.Projection = projection;
            }

            if (this.CenterX.HasValue)
            {
                context.InitializeCenterOfRotationX(this.CenterX.Value);
            }

            if (this.CenterY.HasValue)
            {
                context.InitializeCenterOfRotationY(this.CenterY.Value);
            }

            if (this.CenterZ.HasValue)
            {
                context.InitializeCenterOfRotationZ(this.CenterZ.Value);
            }
        }
    }
}