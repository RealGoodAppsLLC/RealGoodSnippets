// <copyright file="AppBoxIcon.xaml.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.BindableObjects.Views
{
    /// <summary>
    /// An icon with a box around it.
    /// </summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AppBoxIcon : Grid
    {
        /// <summary>
        /// The icon font size proxy property.
        /// </summary>
        public static readonly BindableProperty IconFontSizeProperty =
            BindableProperty.Create(
                nameof(IconFontSize),
                typeof(double),
                typeof(AppBoxIcon),
                -1D,
                propertyChanged: ProxyHelpers.OnUpdateProxyProperty<AppBoxIcon, double>((self, _, newValue) => self.IconFontSize = newValue));

        /// <summary>
        /// The icon font family proxy property.
        /// </summary>
        public static readonly BindableProperty IconFontFamilyProperty =
            BindableProperty.Create(
                nameof(IconFontFamily),
                typeof(string),
                typeof(AppBoxIcon),
                default(string),
                propertyChanged: ProxyHelpers.OnUpdateProxyProperty<AppBoxIcon, string>((self, _, newValue) => self.IconFontFamily = newValue));

        /// <summary>
        /// The icon text proxy property.
        /// </summary>
        public static readonly BindableProperty IconTextProperty =
            BindableProperty.Create(
                nameof(IconText),
                typeof(string),
                typeof(AppBoxIcon),
                default(string),
                propertyChanged: ProxyHelpers.OnUpdateProxyProperty<AppBoxIcon, string>((self, _, newValue) => self.IconText = newValue));

        /// <summary>
        /// Initializes a new instance of the <see cref="AppBoxIcon"/> class.
        /// </summary>
        public AppBoxIcon()
        {
            this.InitializeComponent();

            this.SetupProxyNotifications(this.Icon, nameof(this.Icon));
        }

        /// <summary>
        /// Gets or sets the internal icon's font size.
        /// </summary>
        public double IconFontSize
        {
            get => this.Icon.FontSize;
            set => this.Icon.FontSize = value;
        }

        /// <summary>
        /// Gets or sets the internal icon's font family.
        /// </summary>
        public string IconFontFamily
        {
            get => this.Icon.FontFamily;
            set => this.Icon.FontFamily = value;
        }

        /// <summary>
        /// Gets or sets the internal icon's text.
        /// </summary>
        public string IconText
        {
            get => this.Icon.Text;
            set => this.Icon.Text = value;
        }
    }
}
