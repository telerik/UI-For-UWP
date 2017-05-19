using System;
using System.Collections.Generic;
using Telerik.Data.Core;
using Telerik.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Telerik.UI.Xaml.Controls.Data.DataForm.View
{
    internal class EntityEditorGenerator
    {
        private readonly IDataFormView owner;

        private HashSet<IEntityPropertyEditor> elements = new HashSet<IEntityPropertyEditor>();
        private Dictionary<string, object> groups = new Dictionary<string, object>();

        public EntityEditorGenerator(IDataFormView view)
        {
            if (view == null)
            {
                throw new ArgumentNullException(nameof(this.owner));
            }

            this.owner = view;

            this.EditorFactory = new EditorFactory();
        }

        internal EditorFactory EditorFactory { get; set; }

        public object CreateContainer(EntityProperty entityProperty)
        {
            var element = this.EditorFactory.CreateEditor(entityProperty);

            this.elements.Add(element);

            return element;
        }

        public void PrepareContainer(object editor, EntityProperty entityProperty)
        {
            object groupOwner = null;

            if (entityProperty.GroupKey != null)
            {
                this.groups.TryGetValue(entityProperty.GroupKey, out groupOwner);
            }

            this.owner.PrepareEditor(editor, entityProperty, groupOwner);
        }

        public void RemoveAll()
        {
            this.groups.Clear();
            this.elements.Clear();
            this.owner.ClearEditors();
        }

        internal void OnCommitModeChanged(CommitMode newValue, CommitMode oldValue)
        {
            foreach (var element in this.elements)
            {
                this.owner.UnSubscribeFromEditorEvents(element, element.Property);
                this.owner.SubscribeToEditorEvents(element, element.Property);
            }
        }

        internal object GetGroupContainer(string groupKey)
        {
            object container;

            this.groups.TryGetValue(groupKey, out container);

            return container;
        }

        internal object CreateGroupContainer(string groupKey)
        {
            object groupVisual;

            if (!this.groups.TryGetValue(groupKey, out groupVisual))
            {
                groupVisual = this.owner.CreateGroupContainer(groupKey);
                this.owner.AddEditor(groupVisual);
                this.groups[groupKey] = groupVisual;
            }

            return groupVisual;
        }

        internal void PrepareGroupContainer(object editor, string groupKey)
        {
            this.owner.PrepareGroupEditor(editor, groupKey);
        }

        internal void AddViewToGroupContainer(object groupContainer, object child)
        {
            var groupHeader = groupContainer as RadExpanderControl;

            (groupHeader.ExpandableContent as Panel).Children.Add(child as FrameworkElement);
        }

        internal void Refresh()
        {
            foreach (var item in this.groups)
            {
                this.owner.PrepareGroupEditor(item.Value, item.Key);
            }

            foreach (var item in this.elements)
            {
                var group = this.GetGroupContainer(item.Property.GroupKey);

                this.owner.PrepareEditor(item, group);
            }
        }
    }
}
