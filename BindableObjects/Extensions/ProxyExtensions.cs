// <copyright file="ProxyExtensions.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.BindableObjects.Extensions
{
    /// <summary>
    /// Extension methods that enable proxying bindable objects.
    /// </summary>
    public static class ProxyExtensions
    {
        /// <summary>
        /// An extension method to assist in proxying property change notifications to a parent bindable object.
        /// For example, if the child contains some properties that are mirrored on the parent object, this will fire property change notifications
        /// on the parent object any time the child object properties change.
        /// However, it is important that the name of the property on the parent object is prefixed. For example,
        /// imagine the child object is an <see cref="Entry"/> and has a property named TextColor.
        /// The parent object may have a property named EntryTextColor to mirror the property on the child object.
        /// </summary>
        /// <param name="parentObject">The parent object that mirrors the properties of the child object.</param>
        /// <param name="childObject">The source of the properties.</param>
        /// <param name="propertyNamePrefix">The prefix to use for the property names.</param>
        public static void SetupProxyNotifications(
            this BindableObject parentObject,
            BindableObject childObject,
            string propertyNamePrefix)
        {
            var onPropertyChanging = typeof(BindableObject)
                .GetMethod("OnPropertyChanging", BindingFlags.Instance | BindingFlags.NonPublic);

            var onPropertyChanged = typeof(BindableObject)
                .GetMethod("OnPropertyChanged", BindingFlags.Instance | BindingFlags.NonPublic);

            if (onPropertyChanging == null || onPropertyChanged == null)
            {
                return;
            }

            childObject.PropertyChanging += (_, args) => onPropertyChanging.Invoke(parentObject, new object[] { $"{propertyNamePrefix}{args.PropertyName}" });
            childObject.PropertyChanged += (_, args) => onPropertyChanged.Invoke(parentObject, new object[] { $"{propertyNamePrefix}{args.PropertyName}" });
        }
    }
}
