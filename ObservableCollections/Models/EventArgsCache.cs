// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the MICROSOFT_LICENSE file in the project root for more information.

namespace RealGoodApps.ObservableCollections.Models
{
    /// <summary>
    /// A cache of event argument definitions used for <see cref="OptimizedObservableCollection{T}"/>.
    /// </summary>
    /// <remarks>
    /// To be kept outside <see cref="OptimizedObservableCollection{T}"/>, since otherwise, a new instance will be created for each generic type used.
    /// </remarks>
    internal static class EventArgsCache
    {
        /// <summary>
        /// The count property changed event arguments.
        /// </summary>
        internal static readonly PropertyChangedEventArgs CountPropertyChanged = new PropertyChangedEventArgs("Count");

        /// <summary>
        /// The indexer property changed event arguments.
        /// </summary>
        internal static readonly PropertyChangedEventArgs IndexerPropertyChanged = new PropertyChangedEventArgs("Item[]");

        /// <summary>
        /// The reset collection event arguments.
        /// </summary>
        internal static readonly NotifyCollectionChangedEventArgs ResetCollectionChanged = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
    }
}
