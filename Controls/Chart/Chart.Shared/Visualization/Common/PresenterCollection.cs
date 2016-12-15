using System;
using System.Collections;
using System.Collections.ObjectModel;

namespace Telerik.UI.Xaml.Controls.Chart
{
    /// <summary>
    /// Represents a strongly typed collection of <see cref="ChartElementPresenter"/> instances.
    /// </summary>
    /// <typeparam name="T">Must be <see cref="ChartElementPresenter"/>.</typeparam>
    public class PresenterCollection<T> : Collection<T> where T : ChartElementPresenter
    {
        private RadChartBase chart;

        internal PresenterCollection(RadChartBase control)
        {
            this.chart = control;
        }

        /// <summary>
        /// Inserts an element into the collection at the specified index.
        /// </summary>
        protected override void InsertItem(int index, T item)
        {
            base.InsertItem(index, item);

            this.chart.OnPresenterAdded(item);
        }

        /// <summary>
        /// Removes the element at the specified index of the collection.
        /// </summary>
        protected override void RemoveItem(int index)
        {
            T presenter = this[index];

            base.RemoveItem(index);

            this.chart.OnPresenterRemoved(presenter);
        }

        /// <summary>
        /// Removes all elements from the <see cref="T:System.Collections.ObjectModel.Collection`1"/>.
        /// </summary>
        protected override void ClearItems()
        {
            T[] presenters = new T[this.Items.Count];
            this.Items.CopyTo(presenters, 0);

            base.ClearItems();

            foreach (T presenter in presenters)
            {
                this.chart.OnPresenterRemoved(presenter);
            }
        }

        /// <summary>
        /// Replaces the element at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index of the element to replace.</param>
        /// <param name="newPresenter">The new value for the element at the specified index. The value can be null for reference types.</param>
        /// <exception cref="T:System.ArgumentOutOfRangeException"><paramref name="index"/> is less than zero.
        /// -or-
        /// <paramref name="index"/> is greater than <see cref="P:System.Collections.ObjectModel.Collection`1.Count"/>.
        /// </exception>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId = "1#", Justification = "Argument name is changed according the scope of the collection.")]
        protected override void SetItem(int index, T newPresenter)
        {
            T oldPresenter = this[index];

            base.SetItem(index, newPresenter);

            this.chart.OnPresenterRemoved(oldPresenter);
            this.chart.OnPresenterAdded(newPresenter);
        }
    }
}
