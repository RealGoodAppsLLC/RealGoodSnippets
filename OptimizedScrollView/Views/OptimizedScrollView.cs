// <copyright file="OptimizedScrollView.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.OptimizedScrollView.Views
{
    /// <summary>
    /// An optimized scroll view which handles removing items that are outside of a page from the view and filling the space with a virtual empty space.
    /// This was introduced since <see cref="CollectionView"/> is buggy and we actually don't need full on virtualization.
    /// The goal of this isn't to make scrolling as smooth as possible, but rather to ensure that memory usage remains reasonable.
    /// </summary>
    /// <typeparam name="TItem">The type of the item view models displayed in the scroll view.</typeparam>
    public class OptimizedScrollView<TItem> : ScrollView
        where TItem : class
    {
        /// <summary>
        /// A property pointing to the item template selector.
        /// </summary>
        public static readonly BindableProperty ItemTemplateSelectorProperty =
            BindableProperty.CreateAttached(
                nameof(ItemTemplateSelector),
                typeof(DataTemplateSelector),
                typeof(OptimizedScrollView<TItem>),
                default(DataTemplateSelector),
                propertyChanged: (bindableObject, _, newValue) =>
                {
                    if (!(bindableObject is OptimizedScrollView<TItem> appOptimizedScrollView))
                    {
                        return;
                    }

                    appOptimizedScrollView.OnDataTemplateSelectorChanged(newValue as DataTemplateSelector);
                });

        /// <summary>
        /// A property pointing to the items source, which should be an observable collection of items.
        /// </summary>
        public static readonly BindableProperty ItemsSourceProperty =
            BindableProperty.CreateAttached(
                nameof(ItemsSource),
                typeof(OptimizedObservableCollection<TItem>),
                typeof(OptimizedScrollView<TItem>),
                default(OptimizedObservableCollection<TItem>),
                propertyChanged: (bindableObject, oldValue, newValue) =>
                {
                    if (!(bindableObject is OptimizedScrollView<TItem> appOptimizedScrollView))
                    {
                        return;
                    }

                    appOptimizedScrollView.OnItemsSourceChanged(
                        oldValue as OptimizedObservableCollection<TItem>,
                        newValue as OptimizedObservableCollection<TItem>);
                });

        /// <summary>
        /// A property pointing the spacing between items, which is used to determine the virtual page detection.
        /// </summary>
        public static readonly BindableProperty SpacingProperty =
            BindableProperty.CreateAttached(
                nameof(Spacing),
                typeof(double),
                typeof(OptimizedScrollView<TItem>),
                default(double));

        /// <summary>
        /// A property pointing to the virtual page buffer, which is used to determine when items go out of view.
        /// This value does not have to be perfect. Ideally it is not too small to prevent items from being prematurely removed from the tree.
        /// However, it should not be too large as to make sure items are removed from the tree reasonably.
        /// </summary>
        public static readonly BindableProperty VirtualPageBufferProperty =
            BindableProperty.CreateAttached(
                nameof(VirtualPageBuffer),
                typeof(double),
                typeof(OptimizedScrollView<TItem>),
                1000D);

        private readonly OptimizedObservableCollection<TItem> absoluteLayoutItemsSource;
        private readonly Dictionary<TItem, double> renderHeightCache;
        private double oldContentSize;

        /// <summary>
        /// Initializes a new instance of the <see cref="OptimizedScrollView{TItem}"/> class.
        /// </summary>
        public OptimizedScrollView()
        {
            // The stack layout is where the items actually appear.
            this.AbsoluteLayout = new AbsoluteLayout();

            // Assign to content of our scroll view to the stack layout.
            this.Content = this.AbsoluteLayout;

            this.absoluteLayoutItemsSource = new OptimizedObservableCollection<TItem>();
            this.renderHeightCache = new Dictionary<TItem, double>();

            BindableLayout.SetItemsSource(this.AbsoluteLayout, this.absoluteLayoutItemsSource);
            BindableLayout.SetItemTemplateSelector(this.AbsoluteLayout, this.ItemTemplateSelector);

            this.AbsoluteLayout.ChildAdded += this.OnChildAdded;
            this.AbsoluteLayout.ChildRemoved += this.OnChildRemoved;
            this.Scrolled += this.OnScrolled;
        }

        /// <summary>
        /// Gets the absolute layout.
        /// Be careful with modifying this from an external place since it is very dynamic.
        /// </summary>
        public AbsoluteLayout AbsoluteLayout { get; }

        /// <summary>
        /// Gets or sets the item template selector.
        /// </summary>
        public DataTemplateSelector ItemTemplateSelector
        {
            get => (DataTemplateSelector)this.GetValue(ItemTemplateSelectorProperty);
            set => this.SetValue(ItemTemplateSelectorProperty, value);
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        public OptimizedObservableCollection<TItem>? ItemsSource
        {
            get => (OptimizedObservableCollection<TItem>?)this.GetValue(ItemsSourceProperty);
            set => this.SetValue(ItemsSourceProperty, value);
        }

        /// <summary>
        /// Gets or sets the virtual page buffer.
        /// </summary>
        public double VirtualPageBuffer
        {
            get => (double)this.GetValue(VirtualPageBufferProperty);
            set => this.SetValue(VirtualPageBufferProperty, value);
        }

        /// <summary>
        /// Gets or sets the item spacing.
        /// </summary>
        public double Spacing
        {
            get => (double)this.GetValue(SpacingProperty);
            set => this.SetValue(SpacingProperty, value);
        }

        /// <summary>
        /// Helper method to scroll to the bottom.
        /// </summary>
        /// <returns>An instance of <see cref="Task"/>.</returns>
        public async Task ScrollToBottomAsync()
        {
            var contentHeight = this.ContentSize.Height;
            var visibleHeight = this.Height;

            var scrollToY = Math.Max(0, contentHeight - visibleHeight);

            await this.ScrollToAsync(0, scrollToY, false);
        }

        /// <summary>
        /// Check for a change in the content size property and adjust the scroll position if necessary.
        /// </summary>
        /// <param name="propertyName">The name of the property changing.</param>
        protected override void OnPropertyChanging(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName == nameof(this.ContentSize))
            {
                this.oldContentSize = this.ContentSize.Height;
            }
        }

        /// <summary>
        /// Finalize the check for a changing content size and adjust the scroll position.
        /// </summary>
        /// <param name="propertyName">The name of the property changed.</param>
        protected override void OnPropertyChanged(string? propertyName = null)
        {
            base.OnPropertyChanged(propertyName);

            if (propertyName != nameof(this.ContentSize))
            {
                return;
            }

            var scrollY = this.ScrollY;
            var newContentSize = this.ContentSize.Height;
            var deltaY = newContentSize - this.oldContentSize;

            if (deltaY < 0)
            {
                return;
            }

            var newScrollY = Math.Max(0, scrollY + deltaY);

            // TODO: Is there a way to optimize this?
            this.ScrollToAsync(0, newScrollY, false);
        }

        private void OnChildAdded(object sender, ElementEventArgs e)
        {
            if (!(e.Element is View elementView))
            {
                return;
            }

            elementView.SizeChanged += this.OnChildSizeChanged;
        }

        private void OnChildRemoved(object sender, ElementEventArgs e)
        {
            if (!(e.Element is View elementView))
            {
                return;
            }

            elementView.SizeChanged -= this.OnChildSizeChanged;
            this.RepositionChildren();
        }

        private void OnChildSizeChanged(object sender, EventArgs e)
        {
            if (!(sender is View senderView))
            {
                throw new InvalidCastException("The sender view is null.");
            }

            if (senderView.BindingContext == null)
            {
                return;
            }

            if (!(senderView.BindingContext is TItem bindingContext))
            {
                throw new InvalidCastException("Binding context is not of type TItem.");
            }

            this.renderHeightCache[bindingContext] = senderView.Height;
            this.RepositionChildren();
        }

        private void RepositionChildren()
        {
            var itemsSource = this.ItemsSource;

            // We are unable to perform virtual DOM calculations until items source has been bound, so defer.
            if (itemsSource == null)
            {
                return;
            }

            var approximateY = 0D;
            var itemSpacing = this.Spacing;
            var width = this.Width;

            var itemToViewDictionary = new Dictionary<TItem, View>();

            var allChildren = this.AbsoluteLayout.Children;

            foreach (var singleChild in allChildren)
            {
                if (singleChild.BindingContext == null)
                {
                    continue;
                }

                if (!(singleChild.BindingContext is TItem childBindingContext))
                {
                    throw new InvalidCastException("The binding context found is not of type TItem.");
                }

                itemToViewDictionary[childBindingContext] = singleChild;
            }

            var allHeightsFound = true;

            foreach (var item in itemsSource)
            {
                var itemIsInTree = itemToViewDictionary.ContainsKey(item);

                if (!this.renderHeightCache.ContainsKey(item))
                {
                    allHeightsFound = false;
                    continue;
                }

                var itemHeight = this.renderHeightCache[item];

                if (itemIsInTree)
                {
                    itemHeight = itemToViewDictionary[item].Height;

                    // TODO: Right now the item height is being ignored. We just need to test in Android.
                    AbsoluteLayout.SetLayoutBounds(itemToViewDictionary[item], new Rectangle(0, approximateY, width, -1));
                }

                approximateY += itemHeight + itemSpacing;
            }

            if (allHeightsFound)
            {
                this.AbsoluteLayout.HeightRequest = approximateY;
            }
        }

        private void OnScrolled(object sender, ScrolledEventArgs e)
        {
            var scrollY = e.ScrollY;
            var visibleHeight = this.Height;
            var endY = scrollY + visibleHeight;
            var midpointY = scrollY + (visibleHeight / 2);

            var itemsSource = this.ItemsSource;

            // We are unable to perform virtual DOM calculations until items source has been bound, so defer.
            if (itemsSource == null)
            {
                return;
            }

            var itemToViewDictionary = new Dictionary<TItem, View>();

            var allChildren = this.AbsoluteLayout.Children;

            foreach (var singleChild in allChildren)
            {
                if (singleChild.BindingContext == null)
                {
                    continue;
                }

                if (!(singleChild.BindingContext is TItem childBindingContext))
                {
                    throw new InvalidCastException("Binding context is not of type TItem.");
                }

                itemToViewDictionary[childBindingContext] = singleChild;
            }

            var approximateY = 0D;
            var itemSpacing = this.Spacing;

            var lastIndex = -1;
            var lastDirectionWasUp = true;

            var buffer = Math.Max(0, this.VirtualPageBuffer);

            foreach (var item in itemsSource)
            {
                var isItemInTree = itemToViewDictionary.ContainsKey(item);
                var itemHeight = this.renderHeightCache.ContainsKey(item) ? this.renderHeightCache[item] : 0;
                var itemHeightIncludingSpacing = itemHeight + itemSpacing;
                var currentApproximateY = approximateY;
                var currentApproximateEndY = currentApproximateY + itemHeight;
                var isItemInVirtualPage = (currentApproximateY >= (scrollY - buffer) && currentApproximateY <= (endY + buffer))
                                          || (currentApproximateEndY >= (scrollY - buffer) && currentApproximateEndY <= (endY + buffer));

                var upDirection = currentApproximateY <= midpointY;

                // Adjust approximate Y for the next item in the scan.
                approximateY += itemHeightIncludingSpacing;

                if ((!isItemInTree && !isItemInVirtualPage)
                    || (isItemInTree && isItemInVirtualPage))
                {
                    continue;
                }

                if (isItemInTree)
                {
                    this.absoluteLayoutItemsSource.Remove(item);
                    continue;
                }

                // The most complicated case since we have to track the appropriate index to place the item at.
                // TODO: This is technically performing decently, but it might be better to compare using indexes directly.
                int currentIndex;

                if (lastIndex == -1 || (lastDirectionWasUp != upDirection))
                {
                    currentIndex = upDirection ? 0 : this.absoluteLayoutItemsSource.Count;
                }
                else
                {
                    currentIndex = lastIndex + 1;
                }

                this.absoluteLayoutItemsSource.Insert(currentIndex, item);

                lastIndex = currentIndex;
                lastDirectionWasUp = upDirection;
            }
        }

        private void OnItemsSourceChanged(
            OptimizedObservableCollection<TItem>? oldValue,
            OptimizedObservableCollection<TItem>? newValue)
        {
            this.renderHeightCache.Clear();

            if (oldValue != null)
            {
                oldValue.CollectionChanged -= this.OnCollectionChanged;
            }

            if (newValue != null)
            {
                newValue.CollectionChanged += this.OnCollectionChanged;

                this.absoluteLayoutItemsSource.Clear();
                this.absoluteLayoutItemsSource.AddRange(newValue.ToList());
            }
        }

        private void OnDataTemplateSelectorChanged(DataTemplateSelector? newValue)
        {
            this.renderHeightCache.Clear();

            if (newValue != null)
            {
                BindableLayout.SetItemTemplateSelector(this.AbsoluteLayout, this.ItemTemplateSelector);
            }
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            var itemsSource = this.ItemsSource;

            if (itemsSource == null)
            {
                return;
            }

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                {
                    this.absoluteLayoutItemsSource.Clear();
                    return;
                }

                case NotifyCollectionChangedAction.Add:
                {
                    var addedItemCount = e.NewItems.Count;

                    switch (addedItemCount)
                    {
                        case 0:
                            return;

                        default:
                        {
                            // Load the new items from the event arguments and drop anything that is a duplicate.
                            var newItemsStartSourceIndex = e.NewStartingIndex;
                            var newItems = e.NewItems.Cast<TItem>()
                                .Where(newItem => !this.absoluteLayoutItemsSource.Contains(newItem))
                                .ToList();

                            // After dropping duplicates, we may not have anything left to add.
                            if (newItems.Count == 0)
                            {
                                return;
                            }

                            // Build a map of indexes from the items source
                            var sourceIndexMap = new Dictionary<TItem, int>();

                            for (var sourceIndex = 0; sourceIndex < itemsSource.Count; sourceIndex++)
                            {
                                sourceIndexMap[itemsSource[sourceIndex]] = sourceIndex;
                            }

                            // Go through the stack layout source and insert in the most optimal position
                            for (var renderIndex = 0; renderIndex < this.absoluteLayoutItemsSource.Count; renderIndex++)
                            {
                                var item = this.absoluteLayoutItemsSource[renderIndex];

                                if (!sourceIndexMap.ContainsKey(item))
                                {
                                    continue;
                                }

                                var sourceIndex = sourceIndexMap[item];

                                if (newItemsStartSourceIndex == sourceIndex + 1)
                                {
                                    this.absoluteLayoutItemsSource.InsertRange(renderIndex + 1, newItems);
                                    return;
                                }

                                if (sourceIndex > newItemsStartSourceIndex)
                                {
                                    this.absoluteLayoutItemsSource.InsertRange(renderIndex, newItems);
                                    return;
                                }
                            }

                            // If we failed to find a good spot for it, let's just put it at the end.
                            // This really shouldn't happen, but it is better than dropping the item.
                            this.absoluteLayoutItemsSource.AddRange(newItems);

                            return;
                        }
                    }
                }

                case NotifyCollectionChangedAction.Remove:
                {
                    var removalCount = e.OldItems.Count;

                    switch (removalCount)
                    {
                        case 0:
                            return;

                        case 1:
                        {
                            if (e.OldItems[0] == null)
                            {
                                return;
                            }

                            if (!(e.OldItems[0] is TItem removedItem))
                            {
                                throw new InvalidCastException("Old item is not of type TItem.");
                            }

                            this.absoluteLayoutItemsSource.Remove(removedItem);
                            return;
                        }

                        default:
                        {
                            var removedItems = e.OldItems.Cast<TItem>();

                            // Create a fast look-up for our items we plan to remove.
                            var removedItemsMap = new Dictionary<TItem, bool>();

                            foreach (var removedItem in removedItems)
                            {
                                removedItemsMap[removedItem] = true;
                            }

                            // Loop through the absolute layout source and remove chunks where we identify matches.
                            var foundSectionStartIndex = -1;

                            for (var renderIndex = 0; renderIndex < this.absoluteLayoutItemsSource.Count; renderIndex++)
                            {
                                var item = this.absoluteLayoutItemsSource[renderIndex];

                                if (!removedItemsMap.ContainsKey(item))
                                {
                                    if (foundSectionStartIndex != -1)
                                    {
                                        var itemsInChunk = renderIndex - foundSectionStartIndex;
                                        this.absoluteLayoutItemsSource.RemoveRange(foundSectionStartIndex, itemsInChunk);
                                        renderIndex -= itemsInChunk;
                                        foundSectionStartIndex = -1;
                                    }

                                    continue;
                                }

                                if (foundSectionStartIndex == -1)
                                {
                                    foundSectionStartIndex = renderIndex;
                                }
                            }

                            // We may have just finally reached the end of a chunk.
                            if (foundSectionStartIndex != -1)
                            {
                                var itemsInChunk = this.absoluteLayoutItemsSource.Count - foundSectionStartIndex;
                                this.absoluteLayoutItemsSource.RemoveRange(foundSectionStartIndex, itemsInChunk);
                            }

                            return;
                        }
                    }
                }

                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Replace:
                    throw new NotSupportedException("Optimized scroll view does not support moving or replacing items.");

                default:
                    throw ExhaustiveMatch.Failed();
            }
        }
    }
}
