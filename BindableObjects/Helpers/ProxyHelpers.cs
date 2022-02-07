// <copyright file="ProxyHelpers.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.BindableObjects.Helpers
{
    /// <summary>
    /// Helpers to enable creating bindable object proxy properties.
    /// </summary>
    public static class ProxyHelpers
    {
        /// <summary>
        /// Returns a binding property changed delegate which simply calls an action on the view with the new value.
        /// This is particularly helpful when you are trying to support type converters from XAML on proxy properties.
        /// For example, if you have a property named TextColor, you will likely want to support the Color type converters.
        /// By creating a BindableProperty and adding a change delegate, you can simply set the property on the view object
        /// when it changes.
        /// </summary>
        /// <param name="onUpdate">The method to call with the new value of the property when updated.</param>
        /// <typeparam name="TView">The type of view.</typeparam>
        /// <typeparam name="TProperty">The type of property.</typeparam>
        /// <returns>A binding property changed delegate.</returns>
        public static BindableProperty.BindingPropertyChangedDelegate OnUpdateProxyProperty<TView, TProperty>(
            Action<TView, TProperty, TProperty> onUpdate)
            where TView : BindableObject =>
            (bindable, oldValue, newValue) => onUpdate.Invoke((TView)bindable, (TProperty)oldValue, (TProperty)newValue);
    }
}
