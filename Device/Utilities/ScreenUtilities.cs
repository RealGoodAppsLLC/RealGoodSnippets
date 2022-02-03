// <copyright file="ScreenUtilities.cs" company="Real Good Apps">
// Copyright (c) Real Good Apps. All rights reserved.
// </copyright>

namespace RealGoodApps.Device.Utilities
{
    /// <summary>
    /// Utility functions to detect various aspects of the device's screen.
    /// </summary>
    public static class ScreenUtilities
    {
        /// <summary>
        /// Gets the width of the screen.
        /// </summary>
        public static double Width => DeviceDisplay.MainDisplayInfo.Width / DeviceDisplay.MainDisplayInfo.Density;

        /// <summary>
        /// Gets the height of the screen.
        /// </summary>
        public static double Height => DeviceDisplay.MainDisplayInfo.Height / DeviceDisplay.MainDisplayInfo.Density;
    }
}
