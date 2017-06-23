using System.Globalization;

namespace MafiaX.BotEngine
{
    public static class LocalizedString
    {
        public static string GetLocalized(this string key, string culture = "en")
        {
            return key.GetLocalized(new CultureInfo(culture));
        }

        public static string GetLocalized(this string key, CultureInfo culture)
        {
            return Properties.Resources.ResourceManager.GetString(key, culture);
        }
    }
}
