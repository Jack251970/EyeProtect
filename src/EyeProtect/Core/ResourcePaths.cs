namespace EyeProtect.Core
{
    /// <summary>
    /// Centralized resource path management
    /// </summary>
    public static class ResourcePaths
    {
        // Base paths
        private const string ResourceBase = "/EyeProtect;component/Resources/";
        private const string ImagesBase = ResourceBase + "Images/";

        // Icon resources
        public static class Icons
        {
            public const string Sunglasses = ResourceBase + "sunglasses.ico";
            public const string Dizzy = ResourceBase + "dizzy.ico";
            public const string Sleeping = ResourceBase + "sleeping.ico";
            public const string Overheated = ResourceBase + "overheated.ico";
        }

        // Image resources
        public static class Images
        {
            public const string Sunglasses = ImagesBase + "sunglasses.png";
            public const string Explosion = ImagesBase + "explosion.png";
            public const string Ecology = ResourceBase + "ecology.png";
            public const string TipImageDark = ImagesBase + "Dark/tipImage.png";
            public const string TipImageLight = ImagesBase + "Light/tipImage.png";
        }

        // Sound resources
        public static class Sounds
        {
            public const string Relentless = ResourceBase + "relentless.wav";
        }

        // Language resources
        public static class Languages
        {
            private const string LanguageBase = ResourceBase + "Language/";
            public const string English = LanguageBase + "en.xaml";
            public const string ChineseSimplified = LanguageBase + "zh-cn.xaml";
            public const string ChineseTraditional = LanguageBase + "zh-tw.xaml";

            public static string GetLanguagePath(string languageCode)
            {
                return LanguageBase + languageCode + ".xaml";
            }
        }

        /// <summary>
        /// Get theme-specific tip image path with pack URI scheme
        /// </summary>
        /// <param name="isDarkTheme">Whether the current theme is dark</param>
        /// <returns>The pack URI to the appropriate tip image</returns>
        public static string GetTipImagePackUri(bool isDarkTheme)
        {
            var relativePath = isDarkTheme ? Images.TipImageDark : Images.TipImageLight;
            return $"pack://application:,,,/EyeProtect;component{relativePath}";
        }

        /// <summary>
        /// Get pack URI for an image resource
        /// </summary>
        /// <param name="relativePath">The relative resource path</param>
        /// <returns>The full pack URI</returns>
        public static string GetPackUri(string relativePath)
        {
            return $"pack://application:,,,/EyeProtect;component{relativePath}";
        }

        /// <summary>
        /// Get icon path by name (for backward compatibility)
        /// </summary>
        /// <param name="iconName">Icon name (sunglasses, dizzy, sleeping, overheated)</param>
        /// <returns>The full path to the icon resource</returns>
        public static string GetIconPath(string iconName)
        {
            if (string.IsNullOrEmpty(iconName))
            {
                return Icons.Sunglasses;
            }

            return iconName.ToLower() switch
            {
                "sunglasses" => Icons.Sunglasses,
                "dizzy" => Icons.Dizzy,
                "sleeping" => Icons.Sleeping,
                "overheated" => Icons.Overheated,
                _ => Icons.Sunglasses
            };
        }
    }
}
