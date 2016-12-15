using System.Collections.Generic;
using Telerik.Core;

namespace Telerik.UI.Xaml.Controls.Grid
{
    internal abstract class ItemModelGenerator<T, K>
        where T : Node, IGridNode
        where K : IGenerationContext
    {
        private Dictionary<object, Queue<T>> recycledDecoratorsType = new Dictionary<object, Queue<T>>();
        private Dictionary<object, Queue<T>> fullyRecycledDecoratorsType = new Dictionary<object, Queue<T>>();
        private HashSet<T> displayedDecorators = new HashSet<T>();
        private Dictionary<object, Queue<T>> recycledFrozenDecoratorsType = new Dictionary<object, Queue<T>>();
        private Dictionary<object, Queue<T>> fullyRecycledFrozenDecoratorsType = new Dictionary<object, Queue<T>>();
        private HashSet<T> displayedFrozenDecorators = new HashSet<T>();

        public ItemModelGenerator(IUIContainerGenerator<T, K> owner)
        {
            this.Owner = owner;
        }

        internal IUIContainerGenerator<T, K> Owner { get; private set; }

        /// <summary>
        /// Gets the recycled decorators. Exposed for testing purposes, do not use in the codebase.
        /// </summary>
        internal Dictionary<object, Queue<T>> RecycledDecorators
        {
            get
            {
                return this.fullyRecycledDecoratorsType;
            }
        }

        internal void FullyRecycleDecorators()
        {
            this.FullyRecycleFrozenDecorators();
            this.FullyRecycleUnfrozenDecorators();
        }

        internal void RecycleDecorator(T decorator)
        {
            var currentRecycledDecoratorType = this.recycledDecoratorsType;

            if (!this.displayedDecorators.Remove(decorator))
            {
                this.displayedFrozenDecorators.Remove(decorator);
                currentRecycledDecoratorType = this.recycledFrozenDecoratorsType;
            }

            Queue<T> recycledDecorators;
            object containerType = decorator.ContainerType;

            if (!currentRecycledDecoratorType.TryGetValue(containerType, out recycledDecorators))
            {
                recycledDecorators = new Queue<T>();
                currentRecycledDecoratorType.Add(containerType, recycledDecorators);
            }

            recycledDecorators.Enqueue(decorator);
            this.ClearContainerForItem(decorator);
        }

        internal virtual T GenerateContainer(K context)
        {
            object containerType = this.ContainerTypeForItem(context);
            T decorator = this.GetRecycledContainer(containerType, context.IsFrozen);
            if (decorator == null)
            {
                decorator = this.GenerateContainerForItem(context, containerType);

                if (context.IsFrozen)
                {
                    this.displayedFrozenDecorators.Add(decorator);
                }
                else
                {
                    this.displayedDecorators.Add(decorator);
                }
            }

            return decorator;
        }

        internal abstract object ContainerTypeForItem(K context);

        internal abstract void PrepareContainerForItem(T decorator);

        protected abstract void ClearContainerForItem(T decorator);

        protected abstract T GenerateContainerForItem(K context, object containerType);

        private void FullyRecycleUnfrozenDecorators()
        {
            foreach (var pair in this.recycledDecoratorsType)
            {
                var recycledDecorators = pair.Value;
                if (recycledDecorators.Count > 0)
                {
                    Queue<T> fullyRecycledDecorators = null;
                    if (!this.fullyRecycledDecoratorsType.TryGetValue(pair.Key, out fullyRecycledDecorators))
                    {
                        fullyRecycledDecorators = new Queue<T>();
                        this.fullyRecycledDecoratorsType.Add(pair.Key, fullyRecycledDecorators);
                    }

                    while (recycledDecorators.Count > 0)
                    {
                        var container = recycledDecorators.Dequeue();
                        this.Owner.MakeHidden(container);
                        fullyRecycledDecorators.Enqueue(container);
                    }
                }
            }
        }

        private void FullyRecycleFrozenDecorators()
        {
            foreach (var pair in this.recycledFrozenDecoratorsType)
            {
                var recycledFrozenDecorators = pair.Value;
                if (recycledFrozenDecorators.Count > 0)
                {
                    Queue<T> fullyRecycledFrozenDecorators = null;
                    if (!this.fullyRecycledFrozenDecoratorsType.TryGetValue(pair.Key, out fullyRecycledFrozenDecorators))
                    {
                        fullyRecycledFrozenDecorators = new Queue<T>();
                        this.fullyRecycledFrozenDecoratorsType.Add(pair.Key, fullyRecycledFrozenDecorators);
                    }

                    while (recycledFrozenDecorators.Count > 0)
                    {
                        var container = recycledFrozenDecorators.Dequeue();
                        this.Owner.MakeHidden(container);
                        fullyRecycledFrozenDecorators.Enqueue(container);
                    }
                }
            }
        }

        private T GetRecycledContainer(object containerType, bool isFrozen)
        {
            T decorator = default(T);

            var currentRecycledDecoratorsType = isFrozen ? this.recycledFrozenDecoratorsType : this.recycledDecoratorsType;
            var currentFullyRecycledDecoratorsType = isFrozen ? this.fullyRecycledFrozenDecoratorsType : this.fullyRecycledDecoratorsType;

            Queue<T> recycledDecorators;
            Queue<T> fullyRecycledDecorators;
            if (currentRecycledDecoratorsType.TryGetValue(containerType, out recycledDecorators) && recycledDecorators.Count > 0)
            {
                decorator = recycledDecorators.Dequeue();
            }
            else if (currentFullyRecycledDecoratorsType.TryGetValue(containerType, out fullyRecycledDecorators) && fullyRecycledDecorators.Count > 0)
            {
                decorator = fullyRecycledDecorators.Dequeue();
                this.Owner.MakeVisible(decorator);
            }

            if (decorator != null)
            {
                if (isFrozen)
                {
                    this.displayedFrozenDecorators.Add(decorator);
                }
                else
                {
                    this.displayedDecorators.Add(decorator);
                }
            }

            return decorator;
        }

        private void RemoveContainers(IEnumerable<T> decorators)
        {
            foreach (var decorator in decorators)
            {
                this.ClearContainerForItem(decorator);
            }
        }
    }
}