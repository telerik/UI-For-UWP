using System.Linq;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Data.DataForm;
using Telerik.UI.Xaml.Controls.Data.DataForm.View;

namespace Telerik.UI.Xaml.Controls.Data
{
    internal partial class DataFormModel
    {
        private Entity entity;

        private EntityEditorGenerator containerGenerator;

        internal DataFormModel(IDataFormView view)
        {
            this.View = view;
            this.containerGenerator = new EntityEditorGenerator(view);
        }

        internal IDataFormView View { get; set; }
        internal EntityProvider entityProvider { get; set; }

        internal EditorFactory EditorFactory
        {
            get
            {
                return this.containerGenerator.EditorFactory;
            }
        }

        internal ITransactionService TransactionService
        {
            get
            {
                return this.View.TransactionService;
            }
        }

        internal Entity Entity
        {
            get
            {
                return this.entity;
            }
            set
            {
                this.OnEntityChanged(value, this.entity);
                this.entity = value;
            }
        }

        internal void OnEditorFactoryChanged(EditorFactory factory)
        {
            this.entity.IsReadOnly = factory.RestrictEditableControls;
            this.containerGenerator.EditorFactory = factory;

            if (this.entity != null)
            {
                this.containerGenerator.RemoveAll();
                this.BuildEditors();
            }
        }

        internal void OnEntityChanged(Entity entity, Entity oldEntity)
        {
            if (entity != null && entity.Validator != null)
            {
                entity.Validator.ErrorsChanged += this.Validator_ErrorsChanged;
            }

            if (oldEntity != null && oldEntity.Validator != null)
            {
                oldEntity.Validator.ErrorsChanged -= this.Validator_ErrorsChanged;
            }
        }

        internal void OnCommitModeChanged(CommitMode mode, CommitMode previousMode)
        {
            this.containerGenerator.OnCommitModeChanged(mode, previousMode);
        }

        internal void OnItemChanged(object newItem)
        {
            if (this.entityProvider == null)
            {
                this.CreateDefaultEntityProvider();
            }

            this.entityProvider.OnItemChanged(newItem);
            this.containerGenerator.RemoveAll();
            this.Entity = this.entityProvider.GenerateEntity();
            this.Entity.IsReadOnly = this.View.IsReadOnly;
            this.BuildEditors();
        }

        internal void OnIsReadOnlyChanged(bool oldValue, bool newValue)
        {
            if (this.Entity != null)
            {
                this.Entity.IsReadOnly = newValue;
                this.containerGenerator.EditorFactory.RestrictEditableControls = newValue;
                this.containerGenerator.RemoveAll();
                this.BuildEditors();
            }
        }

        internal void OnIterationModeChanged(PropertyIteratorMode iterationMode)
        {
            this.entityProvider.IteratorMode = iterationMode;
        }

        internal void OnEntityProviderChanged(EntityProvider provider)
        {
            this.entityProvider = provider;
        }

        internal void RefreshLayout()
        {
            this.containerGenerator.RemoveAll();
            this.BuildEditors();
        }

        private void Validator_ErrorsChanged(object sender, System.ComponentModel.DataErrorsChangedEventArgs e)
        {
            this.TransactionService.ErrorsChanged(sender, e.PropertyName);
        }

        private void CreateDefaultEntityProvider()
        {
            this.entityProvider = new RuntimeEntityProvider();
        }

        private void BuildEditors()
        {
            var properties = this.entity.Properties.OrderBy((property) => property.Index);

            foreach (var entityProperty in properties)
            {
                object groupContainer = null;

                if (entityProperty.GroupKey != null)
                {
                    groupContainer = this.GetOrCreateGroupContainer(entityProperty.GroupKey);
                }

                var editor = this.containerGenerator.CreateContainer(entityProperty);

                if (editor != null)
                {
                    this.containerGenerator.PrepareContainer(editor, entityProperty);

                    if (entityProperty.GroupKey == null)
                    {
                        this.View.AddEditor(editor);
                    }
                    else
                    {
                        this.containerGenerator.AddViewToGroupContainer(groupContainer, editor);
                    }
                }
            }
        }

        private object GetOrCreateGroupContainer(string groupKey)
        {
            var container = this.containerGenerator.GetGroupContainer(groupKey);

            if (container == null)
            {
                container = this.containerGenerator.CreateGroupContainer(groupKey);
                this.containerGenerator.PrepareGroupContainer(container, groupKey);
            }

            return container;
        }
    }
}
