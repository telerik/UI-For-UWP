using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Telerik.Data.Core;

namespace Telerik.UI.Xaml.Controls.Grid.Tests
{
    [DebuggerDisplay("Name = {Name}, Count = {Count}")]
    internal class TestGroup : Collection<TestGroup>, IGroup
    {
        public TestGroup(object name)
        {
            this.Name = this.TransforToExpectedGroupName(name);
            this.Type = GroupType.BottomLevel;
        }

        protected virtual object TransforToExpectedGroupName(object groupName)
        {
            return groupName;
        }

        public new IReadOnlyList<object> Items
        {
            get
            {
                return this;
            }
        }

        public object Name { get; private set; }

        public bool HasItems
        {
            get { return this.Count > 0; }
        }

        public IGroup Parent
        {
            get;
            private set;
        }

        public GroupType Type
        {
            get;
            set;
        }

        public int Level
        {
            get
            {
                return Telerik.Data.Core.IGroupExtensions.GetLevel(this);
            }
        }

        protected override void InsertItem(int index, TestGroup item)
        {
            item.Parent = this;
            base.InsertItem(index, item);
        }

        protected override void ClearItems()
        {
            base.ClearItems();
            foreach (var item in this)
            {
                item.Parent = null;
            }
        }

        protected override void RemoveItem(int index)
        {
            var group = this[index];
            group.Parent = null;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TestGroup item)
        {
            var group = this[index];
            group.Parent = null;
            item.Parent = this;
            base.SetItem(index, item);
        }

        public bool IsBottomLevel
        {
            get { return this.Count == 0; }
        }
    }
}