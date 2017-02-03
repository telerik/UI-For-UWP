using System;
using System.ComponentModel;

namespace Telerik.Data.Core
{
    /// <summary>
    /// Base class that support <see cref="Cloneable.Clone"/> Clone and <see cref="INotifyPropertyChanged"/>.
    /// </summary>
    internal abstract class SettingsNode : Cloneable, INotifyPropertyChanged, ISupportInitialize
    {
        private bool isInitializing;
        private int editScopeLevel;

        // TODO: create a SettingsEventArgs that contains nested SettinsEventArgs and collect the notifications while in change scope.
        private SettingsChangedEventArgs accumulatedChanges;

        /// <summary>
        /// Invoked when this or one of the children is changed.
        /// </summary>
        public event EventHandler<SettingsChangedEventArgs> SettingsChanged;

        /// <summary>
        /// Invoked when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Gets or sets the <see cref="SettingsNode"/> this <see cref="SettingsNode"/> is used in.
        /// </summary>
        public SettingsNode Parent { get; internal set; }

        private bool IsInEditScope
        {
            get
            {
                return this.editScopeLevel != 0 || this.isInitializing;
            }
        }

        /// <summary>
        /// Enters the <see cref="SettingsNode"/> in a new editing scope. Use when applying multiple changes.
        /// If child <see cref="SettingsNode"/> are changed, notifications will be accumulated in this <see cref="SettingsNode"/>.
        /// <example>
        /// using(settingsNode.BeginEdit())
        /// {
        ///     // Apply multiple changes here.
        /// }
        /// </example>
        /// </summary>
        /// <returns>An edit scope token that you must <see cref="IDisposable.Dispose"/> when you are done with the editing.</returns>
        public IDisposable BeginEdit()
        {
            return new EditScope(this);
        }

        /// <inheritdoc />
        public void BeginInit()
        {
            if (this.isInitializing)
            {
                throw new InvalidOperationException("Can not start new initialization while already in initialization.");
            }

            this.isInitializing = true;
        }

        /// <inheritdoc />
        public void EndInit()
        {
            if (!this.isInitializing)
            {
                throw new InvalidOperationException("Can not end initialization because initialization was not started.");
            }

            this.isInitializing = false;
        }

        internal static void ValidateChildForAssignment(SettingsNode child)
        {
            if (child.Parent != null)
            {
                throw new InvalidOperationException("Node already is a child of another node.");
            }
        }

        internal void ChangeSettingsProperty<T>(ref T childProperty, T newChild) where T : SettingsNode
        {
            if (childProperty != null)
            {
                childProperty.Parent = null;
            }

            if (newChild != null)
            {
                ValidateChildForAssignment(newChild);
                newChild.Parent = this;
            }

            childProperty = newChild;
        }

        /// <summary>
        /// Will recursively notify all <see cref="SettingsNode"/> for a settings change.
        /// </summary>
        /// <param name="args"><see cref="SettingsChangedEventArgs"/> that contain information about the change.</param>
        protected internal void NotifyChange(SettingsChangedEventArgs args)
        {
            if (args == null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            args.OriginalSource = this;
            SettingsNode target = this;
            while (target != null && !target.IsInEditScope)
            {
                target.RaiseSettingsChanged(args);
                target = target.Parent;
            }

            if (target != null)
            {
                target.AccumulateChanges();
            }
        }        

        /// <summary>
        /// Unsets the parent initiated with <see cref="AddSettingsChild"/>.
        /// This <see cref="SettingsNode"/> will no longer receive change notifications from the <paramref name="child"/>.
        /// </summary>
        /// <param name="child">The nested <see cref="SettingsNode"/>.</param>
        protected internal void RemoveSettingsChild(SettingsNode child)
        {
            if (this != child.Parent)
            {
                throw new InvalidOperationException("Trying to remove child node from parent that is not the actual parent of the child.");
            }

            child.Parent = null;
        }

        /// <summary>
        /// Set this <see cref="SettingsNode"/> as parent of the <paramref name="child"/> and becomes a target for the <paramref name="child"/>'s change notifications.
        /// </summary>
        /// <param name="child">The nested <see cref="SettingsNode"/>.</param>
        protected internal void AddSettingsChild(SettingsNode child)
        {
            ValidateChildForAssignment(child);
            child.Parent = this;
        }

        /// <summary>
        /// Invoked when a SettingsChangedEventArgs reaches the <see cref="SettingsNode"/>.
        /// </summary>
        /// <param name="args">The <see cref="SettingsChangedEventArgs" /> that contains the event data.</param>
        protected virtual void OnSettingsChanged(SettingsChangedEventArgs args)
        {
        }

        /// <summary>
        /// Raises this object's <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="propertyName">The property that has a new value.</param>
        protected void OnPropertyChanged(string propertyName)
        {
            if (this.PropertyChanged != null)
            {
                this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private void EnterEditScope()
        {
            this.editScopeLevel++;
        }

        private void ExitEditScope()
        {
            this.editScopeLevel--;
            if (!this.IsInEditScope)
            {
                this.OnExitEditScope();
            }
        }

        private void OnExitEditScope()
        {
            if (this.accumulatedChanges != null)
            {
                this.NotifyChange(this.accumulatedChanges);
                this.accumulatedChanges = null;
            }
        }

        private void AccumulateChanges()
        {
            if (this.accumulatedChanges == null)
            {
                this.accumulatedChanges = new SettingsChangedEventArgs();
            }
        }

        private void RaiseSettingsChanged(SettingsChangedEventArgs args)
        {
            this.OnSettingsChanged(args);
            if (this.SettingsChanged != null)
            {
                this.SettingsChanged(this, args);
            }
        }

        private class EditScope : IDisposable
        {
            private SettingsNode settingsNode;

            public EditScope(SettingsNode settingsNode)
            {
                this.settingsNode = settingsNode;
                this.settingsNode.EnterEditScope();
            }

            public void Dispose()
            {
                if (this.settingsNode == null)
                {
                    throw new InvalidOperationException("Already disposed.");
                }

                var node = this.settingsNode;
                this.settingsNode = null;
                node.ExitEditScope();
            }
        }
    }
}