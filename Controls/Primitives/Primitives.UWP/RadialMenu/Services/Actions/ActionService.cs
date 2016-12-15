using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class ActionService
    {
        private Queue<ActionBase> actionsToExecute = new Queue<ActionBase>();

        private bool actionInProgress;
        private ActionBase currentAction;

        public static void PushActionAsync(ActionBase item)
        {
            item.Execute();
        }

        public void PushAction(ActionBase item)
        {
            if (!item.IsDependant)
            {
                throw new Exception("Service does not support independent actions!");
            }

            this.actionsToExecute.Enqueue(item);

            this.ExecuteAction();
        }

        public void ForceCompletion()
        {
            if (this.currentAction != null)
            {
                this.currentAction.ForceCompletion();
            }

            if (this.actionsToExecute.Count > 0)
            {
                this.ExecuteAction();
                this.ForceCompletion();
            }
        }

        private void ExecuteAction()
        {
            if (!this.actionInProgress && this.actionsToExecute.Count > 0)
            {
                this.currentAction = this.actionsToExecute.Dequeue();
                this.currentAction.Completed += this.OnItemCompleted;
                this.actionInProgress = true;

                this.currentAction.Execute();
            }
        }

        private void OnItemCompleted(object sender, EventArgs e)
        {
            var item = sender as ActionBase;

            item.Completed -= this.OnItemCompleted;
            this.currentAction = null;
            this.actionInProgress = false;

            this.ExecuteAction();
        }
    }
}
