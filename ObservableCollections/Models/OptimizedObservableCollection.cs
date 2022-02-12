// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the MICROSOFT_LICENSE file in the project root for more information.

namespace RealGoodApps.ObservableCollections.Models
{
    /// <summary>
    /// Implementation of a dynamic data collection based on generic Collection&lt;T&gt;,
    /// implementing INotifyCollectionChanged to notify listeners
    /// when items get added, removed or the whole list is refreshed.
    /// </summary>
    /// <typeparam name="T">The type of the items kept in the collection.</typeparam>
    public class OptimizedObservableCollection<T> : ObservableCollection<T>
    {
        [NonSerialized]
        private DeferredEventsCollection? deferredEvents;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizedObservableCollection{T}"/> class.
        /// The new instance is empty and has default initial capacity.
        /// </summary>
        public OptimizedObservableCollection()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizedObservableCollection{T}"/> class
        /// that contains elements copied from the specified list.
        /// </summary>
        /// <param name="list">The list whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the list.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The list is a null reference.</exception>
        public OptimizedObservableCollection(List<T> list)
            : base(list)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizedObservableCollection{T}"/> class that contains
        /// elements copied from the specified collection and has sufficient capacity
        /// to accommodate the number of elements copied.
        /// </summary>
        /// <param name="collection">The collection whose elements are copied to the new list.</param>
        /// <remarks>
        /// The elements are copied onto the ObservableCollection in the
        /// same order they are read by the enumerator of the collection.
        /// Please do not use this as it uses an enumerable and will lead to multiple enumeration.
        /// </remarks>
        /// <exception cref="ArgumentNullException">The collection is null.</exception>
        public OptimizedObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
            throw new ArgumentException(
                "Please use the list overloads instead! Enumerables and observable collections are dangerous.",
                nameof(collection));
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="collection">
        /// The collection whose elements should be added to the end of the <see cref="ObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        public void AddRange(List<T> collection)
        {
            this.InsertRange(this.Count, collection);
        }

        /// <summary>
        /// Adds the elements of the specified collection to the end of the <see cref="ObservableCollection{T}"/>.
        /// </summary>
        /// <param name="valueImmutableList">
        /// The collection whose elements should be added to the end of the <see cref="ObservableCollection{T}"/>.
        /// The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="valueImmutableList"/> is null.</exception>
        public void AddRange(ValueImmutableList<T> valueImmutableList)
        {
            this.AddRange(valueImmutableList.ToList());
        }

        /// <summary>
        /// Inserts the elements of a collection into the <see cref="ObservableCollection{T}"/> at the specified index.
        /// </summary>
        /// <param name="index">The zero-based index at which the new elements should be inserted.</param>
        /// <param name="collection">The collection whose elements should be inserted into the List{T}. The collection itself cannot be null, but it can contain elements that are null, if type T is a reference type.</param>
        /// <exception cref="ArgumentNullException"><paramref name="collection"/> is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException"><paramref name="index"/> is not in the collection range.</exception>
        public void InsertRange(int index, List<T> collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (index > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (collection is ICollection<T> countable)
            {
                if (countable.Count == 0)
                {
                    return;
                }
            }

            this.CheckReentrancy();

            // Expand the following couple of lines when adding more constructors.
            var target = (List<T>)this.Items;
            target.InsertRange(index, collection);

            this.OnEssentialPropertiesChanged();
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, collection, index));
        }

        /// <summary>
        /// Removes a range of elements from the <see cref="ObservableCollection{T}"/>>.
        /// </summary>
        /// <param name="index">The zero-based starting index of the range of elements to remove.</param>
        /// <param name="count">The number of elements to remove.</param>
        /// <exception cref="ArgumentOutOfRangeException">The specified range is exceeding the collection.</exception>
        public void RemoveRange(int index, int count)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(count));
            }

            if (index + count > this.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index));
            }

            if (count == 0)
            {
                return;
            }

            if (count == 1)
            {
                this.RemoveItem(index);
                return;
            }

            var items = (List<T>)this.Items;
            var removedItems = items.GetRange(index, count);

            this.CheckReentrancy();

            items.RemoveRange(index, count);

            this.OnEssentialPropertiesChanged();

            if (this.Count == 0)
            {
                this.OnCollectionReset();
            }
            else
            {
                this.OnCollectionChanged(
                    new NotifyCollectionChangedEventArgs(
                        NotifyCollectionChangedAction.Remove,
                        removedItems,
                        index));
            }
        }

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when the list is being cleared;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        protected override void ClearItems()
        {
            if (this.Count == 0)
            {
                return;
            }

            this.CheckReentrancy();
            base.ClearItems();
            this.OnEssentialPropertiesChanged();
            this.OnCollectionReset();
        }

        /// <summary>
        /// Called by base class Collection&lt;T&gt; when an item is set in list;
        /// raises a CollectionChanged event to any listeners.
        /// </summary>
        /// <param name="index">The item index.</param>
        /// <param name="item">The item being set.</param>
        protected override void SetItem(int index, T item)
        {
            if (Equals(this[index], item))
            {
                return;
            }

            this.CheckReentrancy();
            var originalItem = this[index];
            base.SetItem(index, item);

            this.OnIndexerPropertyChanged();
            this.OnCollectionChanged(NotifyCollectionChangedAction.Replace, originalItem, item, index);
        }

        /// <summary>
        /// Raise CollectionChanged event to any listeners.
        /// Properties/methods modifying this ObservableCollection will raise
        /// a collection changed event through this virtual method.
        /// </summary>
        /// <param name="e">The collection changed event arguments.</param>
        /// <remarks>
        /// When overriding this method, either call its base implementation
        /// or call BlockReentrancy to guard against reentrant collection changes.
        /// </remarks>
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            if (this.deferredEvents != null)
            {
                this.deferredEvents.Add(e);
                return;
            }

            base.OnCollectionChanged(e);
        }

        /// <summary>
        /// Defer the events for the collection.
        /// </summary>
        /// <returns>A disposable.</returns>
        protected virtual IDisposable DeferEvents() => new DeferredEventsCollection(this);

        /// <summary>
        /// Helper to raise Count property and the Indexer property.
        /// </summary>
        private void OnEssentialPropertiesChanged()
        {
            this.OnPropertyChanged(EventArgsCache.CountPropertyChanged);
            this.OnIndexerPropertyChanged();
        }

        /// <summary>
        /// Helper to raise a PropertyChanged event for the Indexer property.
        /// </summary>
        private void OnIndexerPropertyChanged() =>
            this.OnPropertyChanged(EventArgsCache.IndexerPropertyChanged);

        /// <summary>
        /// Helper to raise CollectionChanged event to any listeners.
        /// </summary>
        private void OnCollectionChanged(
            NotifyCollectionChangedAction action,
            object? oldItem,
            object? newItem,
            int index) =>
            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(action, newItem, oldItem, index));

        /// <summary>
        /// Helper to raise CollectionChanged event with action == Reset to any listeners.
        /// </summary>
        private void OnCollectionReset() =>
            this.OnCollectionChanged(EventArgsCache.ResetCollectionChanged);

        private sealed class DeferredEventsCollection : List<NotifyCollectionChangedEventArgs>, IDisposable
        {
            private readonly OptimizedObservableCollection<T> collection;

            public DeferredEventsCollection(OptimizedObservableCollection<T> collection)
            {
                Debug.Assert(collection != null, "collection != null");
                Debug.Assert(collection.deferredEvents == null, "collection.deferredEvents == null");
                this.collection = collection;
                this.collection.deferredEvents = this;
            }

            public void Dispose()
            {
                this.collection.deferredEvents = null;
                foreach (var args in this)
                {
                    this.collection.OnCollectionChanged(args);
                }
            }
        }
    }
}
