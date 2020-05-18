using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Resources;

namespace NthDimension.Utilities
{
    /// <summary>
    /// Implements routine for application localization.
    /// Each string that is visible to user (e.g. button.text, lable.text, ...) has to be
    /// encapsulate with translate function.
    /// </summary>
    public static class Localization
    {

        /// <summary>
        /// Localization resources file name extension.
        /// </summary>
        private const string RESFILE_EXT = ".resources";

        /// <summary>
        /// Extended resource file format with specific culture string.
        /// </summary>
        private const string RESFILE_FORMAT = "{0}.*" + RESFILE_EXT;

        /// <summary>
        /// Resource manager for access to the resource files.
        /// </summary>
        private static ResourceManager locResManager = null;

        /// <summary>
        /// Current cultrure info used to translate strings.
        /// </summary>
        private static CultureInfo currentCulInfo = null;

        /// <summary>
        /// Array of all available culture strings.
        /// </summary>
        private static string[] availableCultureNames = null;

        /// <summary>
        /// Initialize resource manager, check if resource files are available.
        /// If no resources (neither the BaseResourceName.resources) are not 
        /// present in the specified directory, or specified paths does not exist
        /// method returns false, and localization resources manager remains uninitialized.
        /// </summary>
        /// <param name="resourceBaseName">Base filename for the resource files.</param>
        /// <param name="resourcePath">Path to the resources directory.</param>
        /// <param name="culture">Culture string that represents culture we want to use
        /// as current culture for string translation. If this string is null, or culture
        /// that should spcify does not exist, CultureInfo from the current thread is used
        /// as current culture.
        /// </param>
        /// <returns>True if initialization was successful, otherwise false.</returns>
        public static bool InitResources(string resourceBaseName, string resourcePath, string culture)
        {
            if (resourceBaseName != null &&
                resourceBaseName != string.Empty &&
                Directory.Exists(resourcePath) &&
                File.Exists(Path.Combine(resourcePath, resourceBaseName + RESFILE_EXT)))
            {

                // Scan for available language resources
                try
                {
                    string[] resFiles = Directory.GetFiles(resourcePath, string.Format(RESFILE_FORMAT, resourceBaseName), SearchOption.TopDirectoryOnly);
                    if (resFiles != null && resFiles.Length > 0)
                    {
                        for (int fileIdx = 0; fileIdx < resFiles.Length; fileIdx++)
                        {
                            resFiles[fileIdx] = Path.GetFileNameWithoutExtension(resFiles[fileIdx]);        // remove extension
                            resFiles[fileIdx] = resFiles[fileIdx].Substring(resourceBaseName.Length + 1);   // remove resource base name with dot
                        }
                        availableCultureNames = resFiles;
                    }
                    else
                    {
                        availableCultureNames = null;
                    }
                }
                catch
                {
                    // Unable to construct culture names, this is not fatal error, continue
                    availableCultureNames = null;
                }

                // Initialize resource manager
                locResManager = ResourceManager.CreateFileBasedResourceManager(resourceBaseName, resourcePath, null);

                // Initialize culture current info
                try
                {
                    currentCulInfo = new CultureInfo(culture); ;
                }
                catch
                {
                    currentCulInfo = CultureInfo.CurrentCulture;
                }
                return true;
            }
            else
            {
                // Invalid resourcePath, resourceBaseName, or missing default resource file in the resourcePath
                locResManager = null;
                currentCulInfo = null;
                availableCultureNames = null;
                return false;
            }
        }

        /// Initialize resource manager, check if resource files are available.
        /// If no resources (neither the BaseResourceName.resources) are not 
        /// present in the specified directory, or specified paths does not exist
        /// method returns false, and localization resources manager remains uninitialized.
        /// </summary>
        /// <param name="resourceBaseName">Base filename for the resource files.</param>
        /// <param name="resourcePath">Path to the resources directory.</param>
        /// <returns>True if initialization was successful, otherwise false.</returns>
        public static bool InitResources(string resourceBaseName, string resourcePath)
        {
            return InitResources(resourceBaseName, resourcePath, null);
        }

        /// <summary>
        /// Checks whether localization resources are initialized 
        /// and methods can be used for string translation.
        /// </summary>
        /// <returns>True if initialization was successful, otherwise returns false.</returns>
        public static bool IsInitialized()
        {
            return locResManager != null;
        }

        /// <summary>
        /// Returns all available culture info strings. These 
        /// strings have following format:
        /// languageCode-country/regionCode
        /// Strings are retrieved from culture info specific resource
        /// files in the resource directory specified in the initialization
        /// method.
        /// </summary>
        /// <returns>IEnumerable type of all available culture strings, or
        /// null if resources were not initialized.</returns>
        public static IEnumerable<string> GetAvailableCultureNames()
        {
            return availableCultureNames;
        }

        /// <summary>
        /// Sets current culture used for string translation.
        /// </summary>
        /// <param name="culture">Culture string.</param>
        /// <returns>True if the culture was succesfully changed.</returns>
        public static bool SetCurrentCulture(string culture)
        {
            try
            {
                currentCulInfo = new CultureInfo(culture);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Sets current culture used for string translation.
        /// </summary>
        /// <param name="culture">CultureInfo instance.</param>
        /// <returns>True if the culture was succesfully changed.</returns>
        public static bool SetCurrentCulture(CultureInfo culture)
        {
            if (culture != null)
            {
                currentCulInfo = culture;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Returns translation of the specified string, or null if the 
        /// current culture or localization resource manager does not 
        /// exist.
        /// </summary>
        /// <param name="msgId">String Id of the string to localize.</param>
        /// <param name="culture">CultureInfo instance used to get the proper translation for the string.</param>
        /// <returns>Translated string, or null if an error occured.</returns>
        public static string GetString(string msgId, CultureInfo culture)
        {
            if (msgId != null)
            {
                if (locResManager != null)
                {
                    try
                    {
                        if (culture != null)
                        {
                            return locResManager.GetString(msgId, culture);
                        }
                        else if (currentCulInfo != null)
                        {
                            return locResManager.GetString(msgId, currentCulInfo);
                        }
                        else
                        {
                            return null;
                        }
                    }
                    catch
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Returns translation of the specified string, or null if the 
        /// current culture or localization resource manager does not 
        /// exist.
        /// </summary>
        /// <param name="msgId">String Id of the string to localize.</param>
        /// <returns>Translated string, or null if an error occured.</returns>
        public static string GetString(string msgId)
        {
            return GetString(msgId, null);
        }

        /// <summary>
        /// Returns translation of the specified string, or null if the 
        /// current culture or localization resource manager does not 
        /// exist. After translation, string.Format method is used to format
        /// the resulting string according to the specified format parameters.
        /// </summary>
        /// <param name="msgId">String Id of the string to localize.</param>
        /// <param name="culture">CultureInfo instance used to get the proper translation for the string.</param>
        /// <param name="formatParams">Formatting parameters.</param>
        /// <returns>Translated string, or null if an error occured.</returns>
        public static string GetStringFmt(string msgId, CultureInfo culture, params object[] formatParams)
        {
            return string.Format(GetString(msgId, culture), formatParams);
        }

        /// <summary>
        /// Returns translation of the specified string, or null if the 
        /// current culture or localization resource manager does not 
        /// exist. After translation, string.Format method is used to format
        /// the resulting string according to the specified format parameters.
        /// </summary>
        /// <param name="msgId">String Id of the string to localize.</param>
        /// <param name="formatParams">Formatting parameters.</param>
        /// <returns>Translated string, or null if an error occured.</returns>
        public static string GetStringFmt(string msgId, params object[] formatParams)
        {
            return GetStringFmt(msgId, null, formatParams);
        }
    }
}
