// <copyright file="ImageResourceExtension.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Images.MarkupExtensions
{
    /// <summary>
    /// A markup extension for XAML to specify embedded image resources.
    /// </summary>
    [ContentProperty(nameof(Source))]
    public class ImageResourceExtension : IMarkupExtension
    {
        /// <summary>
        /// Gets or sets the source of the image defined in XAML.
        /// </summary>
        public string? Source { get; set; }

        /// <summary>
        /// Translate the image name to the resource path.
        /// </summary>
        /// <param name="serviceProvider">An instance of <see cref="IServiceProvider"/>.</param>
        /// <returns>The image source.</returns>
        public object? ProvideValue(IServiceProvider serviceProvider)
        {
            if (this.Source == null)
            {
                return null;
            }

            var imageSource = ImageSource.FromResource(
                $"{ImageConstants.ResourceNamePrefix}{this.Source}",
                typeof(ImageResourceExtension).GetTypeInfo().Assembly);

            return imageSource;
        }
    }
}
