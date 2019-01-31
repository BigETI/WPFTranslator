using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

/// <summary>
/// Windows presentation foundation translator namespace
/// </summary>
namespace WPFTranslator
{
    /// <summary>
    /// Translator class
    /// </summary>
    public static class Translator
    {
        /// <summary>
        /// Language resource manager
        /// </summary>
        private static ResourceManager languageResourceManager;

        /// <summary>
        /// Fallback language resource manager
        /// </summary>
        private static ResourceManager fallbackLanguageResourceManager;

        /// <summary>
        /// Translator interface
        /// </summary>
        private static ITranslatorInterface translatorInterface;

        /// <summary>
        /// Translator interface
        /// </summary>
        public static ITranslatorInterface TranslatorInterface
        {
            get
            {
                return translatorInterface;
            }
            set
            {
                if (value != null)
                {
                    translatorInterface = value;
                }
            }
        }

        /// <summary>
        /// Initialize language
        /// </summary>
        public static void InitLanguage()
        {
            if (translatorInterface != null)
            {
                if (languageResourceManager == null)
                {
                    try
                    {
                        Assembly a = Assembly.Load(translatorInterface.AssemblyName);
                        languageResourceManager = new ResourceManager(translatorInterface.AssemblyName + ".Languages." + translatorInterface.Language, a);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                    }
                }
                if (fallbackLanguageResourceManager == null)
                {
                    try
                    {
                        Assembly a = Assembly.Load(translatorInterface.AssemblyName);
                        fallbackLanguageResourceManager = new ResourceManager(translatorInterface.AssemblyName + ".Languages." + translatorInterface.FallbackLanguage, a);
                    }
                    catch (Exception e)
                    {
                        Console.Error.WriteLine(e.Message);
                    }
                }
            }
        }

        /// <summary>
        /// Try translatie
        /// </summary>
        /// <param name="input">Input</param>
        /// <param name="output">Output</param>
        /// <returns>Success</returns>
        public static bool TryTranslate(string input, out string output)
        {
            bool ret = false;
            output = input;
            if (input.StartsWith("{$") && input.EndsWith("$}") && (input.Length > 4))
            {
                output = GetTranslation(input.Substring(2, input.Length - 4));
                ret = (input != output);
            }
            return ret;
        }

        /// <summary>
        /// Load language
        /// </summary>
        /// <param name="parent">Parent UI element</param>
        private static void LoadLanguage(UIElement parent)
        {
            try
            {
                string translated = "";
                InitLanguage();
                IEnumerable<UIElement> ui_elements = GetSelfAndChildrenRecursive(parent);
                foreach (UIElement ui_element in ui_elements)
                {
                    if (ui_element is ContentControl)
                    {
                        ContentControl content_control = (ContentControl)ui_element;
                        if (content_control.HasContent)
                        {
                            if (content_control.Content is string)
                            {
                                if (TryTranslate((string)(content_control.Content), out translated))
                                {
                                    content_control.Content = translated;
                                }
                            }
                            else if (content_control.Content is ITranslatable)
                            {
                                ITranslatable translatable = (ITranslatable)(content_control.Content);
                                if (TryTranslate(translatable.TranslatableText, out translated))
                                {
                                    translatable.TranslatableText = translated;
                                }
                            }
                        }
                    }
                    if (ui_element is ItemsControl)
                    {
                        ItemsControl item_control = (ItemsControl)ui_element;
                        if (item_control.Items != null)
                        {
                            for (int i = 0, count = item_control.Items.Count; i < count; i++)
                            {
                                object item = item_control.Items[i];
                                if (item is string)
                                {
                                    if (TryTranslate((string)item, out translated))
                                    {
                                        item_control.Items[i] = translated;
                                    }
                                }
                                else if (item is ITranslatable)
                                {
                                    ITranslatable translatable = (ITranslatable)item;
                                    if (TryTranslate(translatable.TranslatableText, out translated))
                                    {
                                        translatable.TranslatableText = translated;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// Get translation
        /// </summary>
        /// <param name="key">Key</param>
        /// <returns>Value</returns>
        public static string GetTranslation(string key)
        {
            string ret = null;
            if (languageResourceManager != null)
            {
                try
                {
                    ret = languageResourceManager.GetString(key);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
                if (ret == null)
                {
                    if (fallbackLanguageResourceManager != null)
                    {
                        try
                        {
                            ret = fallbackLanguageResourceManager.GetString(key);
                        }
                        catch (Exception e)
                        {
                            Console.Error.WriteLine(e.Message);
                        }
                    }
                }
            }
            else if (fallbackLanguageResourceManager != null)
            {
                try
                {
                    ret = fallbackLanguageResourceManager.GetString(key);
                }
                catch (Exception e)
                {
                    Console.Error.WriteLine(e.Message);
                }
            }
            if (ret == null)
            {
                ret = "{$" + key + "$}";
            }
            return ret;
        }

        /// <summary>
        /// Change language
        /// </summary>
        /// <param name="language">Language</param>
        /// <returns>Success</returns>
        public static bool ChangeLanguage(Language language)
        {
            bool ret = false;
            if (translatorInterface.Language != language.Culture)
            {
                translatorInterface.Language = language.Culture;
                translatorInterface.SaveSettings();
                ret = true;
            }
            return ret;
        }

        /// <summary>
        /// Load translation
        /// </summary>
        /// <param name="parent">Parent UI element</param>
        public static void LoadTranslation(UIElement parent)
        {
            if (translatorInterface != null)
            {
                LoadLanguage(parent);
            }
        }

        /// <summary>
        /// Get self and children recursive
        /// </summary>
        /// <param name="parent">Parent UI element</param>
        /// <returns>All children recursive</returns>
        public static IEnumerable<UIElement> GetSelfAndChildrenRecursive(UIElement parent)
        {
            List<UIElement> ret = new List<UIElement>();
            if (parent != null)
            {
                if (parent is ItemsControl)
                {
                    ItemsControl items_control = (ItemsControl)parent;
                    if (items_control.Items != null)
                    {
                        foreach (object child in items_control.Items)
                        {
                            if (child is UIElement)
                            {
                                ret.AddRange(GetSelfAndChildrenRecursive((UIElement)child));
                            }
                        }
                    }
                }
                if (parent is Panel)
                {
                    Panel panel = (Panel)parent;
                    foreach (UIElement child in panel.Children)
                    {
                        ret.AddRange(GetSelfAndChildrenRecursive(child));
                    }
                }
                ret.Add(parent);
            }
            return ret;
        }

        /// <summary>
        /// Enumerator to combo box
        /// </summary>
        /// <typeparam name="T">Enumerator type</typeparam>
        /// <param name="comboBox">Combo box</param>
        public static void EnumToComboBox<T>(ComboBox comboBox)
        {
            EnumToComboBox<T>(comboBox, null);
        }

        /// <summary>
        /// Enumerator to combo box
        /// </summary>
        /// <typeparam name="T">Enumerator type</typeparam>
        /// <param name="comboBox">Combo box</param>
        /// <param name="exclusions">Exclusions</param>
        public static void EnumToComboBox<T>(ComboBox comboBox, T[] exclusions)
        {
            comboBox.Items.Clear();
            Array arr = Enum.GetValues(typeof(T));
            foreach (var e in arr)
            {
                bool s = true;
                if (exclusions != null)
                {
                    foreach (var ex in exclusions)
                    {
                        if (ex.Equals(e))
                        {
                            s = false;
                            break;
                        }
                    }
                }
                if (s)
                {
                    comboBox.Items.Add(e);
                }
            }
        }

        /// <summary>
        /// Enumerable to combo box
        /// </summary>
        /// <typeparam name="T">Type to enumerate</typeparam>
        /// <param name="comboBox">Combo box</param>
        /// <param name="items">Items</param>
        public static void EnumerableToComboBox<T>(ComboBox comboBox, IEnumerable<T> items)
        {
            comboBox.Items.Clear();
            foreach (var item in items)
            {
                comboBox.Items.Add(item);
            }
        }
    }
}
