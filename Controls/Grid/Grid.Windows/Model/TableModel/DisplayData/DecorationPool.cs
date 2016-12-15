using System.Collections.Generic;
using Telerik.Core;
using Telerik.Data.Core.Layouts;
using Telerik.UI.Xaml.Controls.Grid;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal class DecorationPool<T> where T : GridElement
    {
        private List<T> recycledElements = new List<T>();
        private List<T> fullyRecycledElements = new List<T>();
        private List<T> displayedElements = new List<T>();

        private IDecorationPresenter<T> visibilityAdapter;

        public DecorationPool(IDecorationPresenter<T> visibilityAdapter)
        {
            this.visibilityAdapter = visibilityAdapter;
        }

        internal T GetRecycledElement()
        {
            if (this.recycledElements.Count > 0)
            {
                T decorator = this.recycledElements[0];
                this.recycledElements.RemoveAt(0);
                return decorator;
            }
            else if (this.fullyRecycledElements.Count > 0)
            {
                T decorator = this.fullyRecycledElements[0];
                this.fullyRecycledElements.RemoveAt(0);

                this.visibilityAdapter.MakeVisible(decorator);
                return decorator;
            }

            return null;
        }

        internal void FullyRecycleElements()
        {
            foreach (T item in this.recycledElements)
            {
                this.fullyRecycledElements.Add(item);
                this.visibilityAdapter.Collapse(item);
            }

            this.recycledElements.Clear();
        }

        internal void Recycle(T element)
        {
            this.recycledElements.Add(element);
            this.displayedElements.Remove(element);
        }

        internal void AddToDisplayedElements(T container)
        {
            this.displayedElements.Add(container);
        }

        internal List<T> GetDisplayedElements()
        {
            return this.displayedElements;
        }

        internal List<T> GetRecycledElements()
        {
            return this.recycledElements;
        }

        internal List<T> GetFullyRecycledElements()
        {
            return this.fullyRecycledElements;
        }

        internal void RecycleAll()
        {
            foreach (var item in this.displayedElements)
            {
                this.recycledElements.Add(item);
            }

            this.displayedElements.Clear();
        }
    }
}