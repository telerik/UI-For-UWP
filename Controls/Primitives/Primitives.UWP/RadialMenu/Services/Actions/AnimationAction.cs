using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Media.Animation;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class AnimationAction : ActionBase
    {
        private Storyboard storyboard;
        private bool actionInProgress;

        public AnimationAction(Storyboard storyboard)
        {
            this.storyboard = storyboard;
        }

        public override void Execute()
        {
            if (this.storyboard != null && !this.actionInProgress)
            {
                this.actionInProgress = true;
                this.storyboard.Completed += this.Storyboard_Completed;
                this.storyboard.Begin();
            }
            else
            {
                this.OnCompleted();
            }
        }

        public override void ForceCompletion()
        {
            this.storyboard.SkipToFill();
            this.actionInProgress = false;
            this.OnCompleted();
        }

        private void Storyboard_Completed(object sender, object e)
        {
            this.storyboard.Completed -= this.Storyboard_Completed;

            if (this.actionInProgress)
            {
                this.OnCompleted();
                this.actionInProgress = false;
            }
        }
    }
}
