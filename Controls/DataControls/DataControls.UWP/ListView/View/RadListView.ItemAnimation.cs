using System;
using System.Collections.Generic;
using System.Text;
using Telerik.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data
{
    public partial class RadListView
    {
        /// <summary>
        /// Identifies the ItemRemovedAnimation dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemRemovedAnimationProperty =
            DependencyProperty.Register("ItemRemovedAnimation", typeof(RadAnimation), typeof(RadListView), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the ItemAddedAnimation dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemAddedAnimationProperty =
            DependencyProperty.Register("ItemAddedAnimation", typeof(RadAnimation), typeof(RadListView), new PropertyMetadata(null));

        /// <summary>
        /// Identifies the <see cref="ItemAnimationMode"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemAnimationModeProperty =
            DependencyProperty.Register("ItemAnimationMode", typeof(ItemAnimationMode), typeof(RadListView), new PropertyMetadata(ItemAnimationMode.PlayAll));

        /// <summary>
        /// Gets or sets an animation that is played when an item is removed.
        /// </summary>
        public RadAnimation ItemRemovedAnimation
        {
            get { return (RadAnimation)GetValue(ItemRemovedAnimationProperty); }
            set { SetValue(ItemRemovedAnimationProperty, value); }
        }

        /// <summary>
        /// Gets or sets an animation that is played when an item is added.
        /// </summary>
        public RadAnimation ItemAddedAnimation
        {
            get { return (RadAnimation)GetValue(ItemAddedAnimationProperty); }
            set { SetValue(ItemAddedAnimationProperty, value); }
        }

        /// <summary>
        /// Gets or sets a value that specifies how the item animations are played.
        /// </summary>
        public ItemAnimationMode ItemAnimationMode
        {
            get { return (ItemAnimationMode)GetValue(ItemAnimationModeProperty); }
            set { SetValue(ItemAnimationModeProperty, value); }
        }

        Panel ISupportItemAnimation.AnimatingChildrenPanel
        {
            get
            {
                return this.animatingChildrenPanel;
            }
        }

        ListViewAnimationService IAnimatingService.AnimatingService
        {
            get { return this.animationSurvice; }
        }
    }
}
