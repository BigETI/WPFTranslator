using System.Collections.Generic;

/// <summary>
/// Windows Presentation Foundation translator namespace
/// </summary>
namespace WPFTranslator
{
    /// <summary>
    /// Translator interface
    /// </summary>
    public interface ITranslatorInterface
    {
        /// <summary>
        /// Language
        /// </summary>
        string Language
        {
            get;
            set;
        }

        /// <summary>
        /// Fallback language
        /// </summary>
        string FallbackLanguage
        {
            get;
        }

        /// <summary>
        /// Assembly name
        /// </summary>
        string AssemblyName
        {
            get;
        }

        /// <summary>
        /// Languages
        /// </summary>
        IEnumerable<Language> Languages
        {
            get;
        }

        /// <summary>
        /// Save settings
        /// </summary>
        void SaveSettings();
    }
}
