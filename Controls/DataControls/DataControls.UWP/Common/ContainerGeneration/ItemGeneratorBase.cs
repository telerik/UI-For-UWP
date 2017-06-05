using System.Collections.Generic;

namespace Telerik.UI.Xaml.Controls.Data.ContainerGeneration
{
    internal abstract class ItemGeneratorBase<T, K>
        where T : IGeneratedContainer
    {
        private Dictionary<object, Queue<T>> recycledDecoratorsType = new Dictionary<object, Queue<T>>();
        private Dictionary<object, Queue<T>> fullyRecycledDecoratorsType = new Dictionary<object, Queue<T>>();
        private Dictionary<object, T> recycledAnimatingDecorators = new Dictionary<object, T>();
        private HashSet<T> displayedDecorators = new HashSet<T>();
        private Dictionary<object, Queue<T>> recycledFrozenDecoratorsType = new Dictionary<object, Queue<T>>();
        private Dictionary<object, Queue<T>> fullyRecycledFrozenDecoratorsType = new Dictionary<object, Queue<T>>();
        private HashSet<T> displayedFrozenDecorators = new HashSet<T>();

        public ItemGeneratorBase(IUIContainerGenerator<T, K> owner)
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

        internal void RecycleAnimatedDecorator(T decorator)
        {
            T animatedDecorator;
            this.recycledAnimatingDecorators.TryGetValue(decorator.ItemInfo.Item, out animatedDecorator);
            if (animatedDecorator != null && !decorator.IsAnimating)
            {
                this.recycledAnimatingDecorators.Remove(animatedDecorator.ItemInfo.Item);
                this.RecycleDecorator(animatedDecorator);
            }
        }

        internal void RecycleDecorator(T decorator)
        {
            if (decorator.IsAnimating)
            {
                T animatedDecorator;
                if (!this.recycledAnimatingDecorators.TryGetValue(decorator.ItemInfo.Item, out animatedDecorator))
                {
                    this.recycledAnimatingDecorators.Add(decorator.ItemInfo.Item, decorator);
                }

                this.displayedDecorators.Remove(decorator);

                return;
            }

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

        internal virtual T GenerateContainer(K context, object key)
        {
            object containerType = this.ContainerTypeForItem(context);
            T decorator = this.GetRecycledAnimatingContainer(key);

            if (decorator == null)
            {
                decorator = this.GetRecycledContainer(containerType, false);
                if (decorator == null)
                {
                    decorator = this.GenerateContainerForItem(context, containerType);
                    this.displayedDecorators.Add(decorator);
                }
            }

            return decorator;
        }

        internal virtual object ContainerTypeForItem(K context)
        {
            return this.Owner.GetContainerTypeForItem(context);
        }

        internal virtual void PrepareContainerForItem(T decorator)
        {
            this.Owner.PrepareContainerForItem(decorator);
        }

        protected virtual void ClearContainerForItem(T decorator)
        {
            this.Owner.ClearContainerForItem(decorator);
        }

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

        private T GetRecycledAnimatingContainer(object key)
        {
            T decorator = default(T);

            this.recycledAnimatingDecorators.TryGetValue(key, out decorator);

            return decorator;
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