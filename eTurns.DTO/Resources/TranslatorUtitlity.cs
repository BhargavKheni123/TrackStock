using System;
using System.Collections.Generic;


namespace eTurns.DTO.Resources
{
    public class TranslatorUtitlity
    {
        public static string GetTranslatedText(string sourceText, string target, string source = "English", bool usingPaidAPI = false)
        {
            string translatedText = string.Empty;
            try
            {
                if (usingPaidAPI)
                {
                    //TranslationClient client = TranslationClient.Create();
                    //string sourceCulture = LanguageEnumToIdentifier(source);
                    //string targetCulture = LanguageEnumToIdentifier(target);
                    //var response = client.TranslateText(sourceText, targetCulture, sourceCulture);
                    //translatedText = response.TranslatedText;
                }
                else
                {
                    string targetCulture = target.Split('-')[0];
                    Translator obj = new Translator();
                    translatedText = obj.Translate(sourceText, source, targetCulture);
                    if (string.IsNullOrWhiteSpace(translatedText))
                    {
                        translatedText = sourceText;
                    }
                }
            }
            catch (Exception ex)
            {
                translatedText = ex.Message;
            }
            return translatedText;
        }


        private static string LanguageEnumToIdentifier(string language)
        {
            string mode = string.Empty;
            EnsureInitialized();
            _languageModeMap.TryGetValue(language, out mode);
            return mode;
        }

        private static void EnsureInitialized()
        {
            if (_languageModeMap == null)
            {
                _languageModeMap = new Dictionary<string, string>();
                _languageModeMap.Add("Afrikaans", "af");
                _languageModeMap.Add("Albanian", "sq");
                _languageModeMap.Add("Arabic", "ar");
                _languageModeMap.Add("Armenian", "hy");
                _languageModeMap.Add("Azerbaijani", "az");
                _languageModeMap.Add("Basque", "eu");
                _languageModeMap.Add("Belarusian", "be");
                _languageModeMap.Add("Bengali", "bn");
                _languageModeMap.Add("Bulgarian", "bg");
                _languageModeMap.Add("Catalan", "ca");
                _languageModeMap.Add("Chinese", "zh-CN");
                _languageModeMap.Add("Croatian", "hr");
                _languageModeMap.Add("Czech", "cs");
                _languageModeMap.Add("Danish", "da");
                _languageModeMap.Add("Dutch", "nl");
                _languageModeMap.Add("English", "en");
                _languageModeMap.Add("Esperanto", "eo");
                _languageModeMap.Add("Estonian", "et");
                _languageModeMap.Add("Filipino", "tl");
                _languageModeMap.Add("Finnish", "fi");
                _languageModeMap.Add("French", "fr");
                _languageModeMap.Add("Galician", "gl");
                _languageModeMap.Add("German", "de");
                _languageModeMap.Add("Georgian", "ka");
                _languageModeMap.Add("Greek", "el");
                _languageModeMap.Add("Haitian Creole", "ht");
                _languageModeMap.Add("Hebrew", "iw");
                _languageModeMap.Add("Hindi", "hi");
                _languageModeMap.Add("Hungarian", "hu");
                _languageModeMap.Add("Icelandic", "is");
                _languageModeMap.Add("Indonesian", "id");
                _languageModeMap.Add("Irish", "ga");
                _languageModeMap.Add("Italian", "it");
                _languageModeMap.Add("Japanese", "ja");
                _languageModeMap.Add("Korean", "ko");
                _languageModeMap.Add("Lao", "lo");
                _languageModeMap.Add("Latin", "la");
                _languageModeMap.Add("Latvian", "lv");
                _languageModeMap.Add("Lithuanian", "lt");
                _languageModeMap.Add("Macedonian", "mk");
                _languageModeMap.Add("Malay", "ms");
                _languageModeMap.Add("Maltese", "mt");
                _languageModeMap.Add("Norwegian", "no");
                _languageModeMap.Add("Persian", "fa");
                _languageModeMap.Add("Polish", "pl");
                _languageModeMap.Add("Portuguese", "pt");
                _languageModeMap.Add("Romanian", "ro");
                _languageModeMap.Add("Russian", "ru");
                _languageModeMap.Add("Serbian", "sr");
                _languageModeMap.Add("Slovak", "sk");
                _languageModeMap.Add("Slovenian", "sl");
                _languageModeMap.Add("Spanish", "es");
                _languageModeMap.Add("Swahili", "sw");
                _languageModeMap.Add("Swedish", "sv");
                _languageModeMap.Add("Tamil", "ta");
                _languageModeMap.Add("Telugu", "te");
                _languageModeMap.Add("Thai", "th");
                _languageModeMap.Add("Turkish", "tr");
                _languageModeMap.Add("Ukrainian", "uk");
                _languageModeMap.Add("Urdu", "ur");
                _languageModeMap.Add("Vietnamese", "vi");
                _languageModeMap.Add("Welsh", "cy");
                _languageModeMap.Add("Yiddish", "yi");
            }
        }

        private static Dictionary<string, string> _languageModeMap;
    }
}
