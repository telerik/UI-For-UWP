using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Telerik.UI.Xaml.Controls.Primitives.Menu
{
    internal class CompositeAction : ActionBase
    {
        private Queue<ActionBase> actionsToExecute;
        private int pendingActionsCount;
        private bool executingAction;

        private List<ActionBase> currentExecutingActions;

        public CompositeAction(params ActionBase[] actions)
            : this((IEnumerable<ActionBase>)actions)
        {
        }

        public CompositeAction(IEnumerable<ActionBase> actions)
        {
            this.actionsToExecute = new Queue<ActionBase>(actions);

            this.currentExecutingActions = new List<ActionBase>();

            this.pendingActionsCount = this.actionsToExecute.Count;
        }

        public override void Execute()
        {
            if (this.actionsToExecute.Count > 0)
            {
                var action = this.actionsToExecute.Peek();

                if (!this.executingAction || !action.IsDependant)
                {
                    this.executingAction = true;
                    this.actionsToExecute.Dequeue();

                    this.currentExecutingActions.Add(action);
                    action.Completed += this.Action_Completed;

                    action.Execute();

                    if (this.actionsToExecute.Count > 0)
                    {
                        var nextAction = this.actionsToExecute.Peek();
                        if (nextAction != null && !nextAction.IsDependant)
                        {
                            this.Execute();
                        }
                    }
                }
            }
        }

        public override void ForceCompletion()
        {
            foreach (var action in this.currentExecutingActions.ToArray())
            {
                if (!action.IsCompleted)
                {
                    action.ForceCompletion();
                }
            }

            if (this.actionsToExecute.Count > 0)
            {
                this.Execute();
                this.ForceCompletion();
            }
        }

        private void Action_Completed(object sender, EventArgs e)
        {
            this.pendingActionsCount--;

            var action = sender as ActionBase;
            action.Completed -= this.Action_Completed;
            this.currentExecutingActions.Remove(action);

            this.executingAction = false;

            if (this.actionsToExecute.Count == 0 && this.pendingActionsCount == 0)
            {
                this.OnCompleted();
            }
            else
            {
                this.Execute();
            }
        }
    }
}
