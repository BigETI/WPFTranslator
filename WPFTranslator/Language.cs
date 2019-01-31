/// <summary>
/// Windows Presentation Foundation translator namespace
/// </summary>
namespace WPFTranslator
{
    /// <summary>
    /// Language class
    /// </summary>
    public class Language
    {
        /// <summary>
        /// Name key
        /// </summary>
        private readonly string nameKey;

        /// <summary>
        /// Culture
        /// </summary>
        public readonly string Culture;

        /// <summary>
        /// Name
        /// </summary>
        public string Name
        {
            get
            {
                return Translator.GetTranslation(nameKey);
            }
        }

        /// <summary>
        /// Language
        /// </summary>
        /// <param name="nameKey">Name key</param>
        /// <param name="culture">Culture</param>
        public Language(string nameKey, string culture)
        {
            this.nameKey = nameKey;
            Culture = culture;
        }

        /// <summary>
        /// To string
        /// </summary>
        /// <returns>String representation</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}
