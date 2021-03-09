using System.Collections.Generic;
using System.IO;

namespace SCRLanguageTest
{
    public class LanguageReader
    {
        /// <summary>
        /// The dictionary that contains all strings mapped to their value names
        /// </summary>
        private readonly Dictionary<string, string> _strings;

        /// <summary>
        /// only used for changing the language to the dictionary
        /// </summary>
        private readonly string[] _keys;

        /// <summary>
        /// Name of the current language
        /// </summary>
        public string CurrentLanguage { get; private set; }

        /// <summary>
        /// Initialises the language reader for use
        /// </summary>
        public LanguageReader(string langkeyfile, string initLanguageFile)
        {
            _strings = new Dictionary<string, string>();
            string[] lines = File.ReadAllLines(langkeyfile);
            _keys = new string[lines.Length];
            int i = 0;
            foreach(string l in lines)
            {
                string t = l.Trim('\r');
                _keys[i] = t;
                _strings.Add(t, null);
                i++;
            }

            if(string.IsNullOrWhiteSpace(initLanguageFile))
                return;

            LoadLanguage(initLanguageFile);
        }

        public void LoadLanguage(string fileName)
        {
            string[] lines = File.ReadAllLines(fileName);
            string newLanguage = lines[2];

            if(CurrentLanguage == newLanguage)
                return;
            CurrentLanguage = newLanguage;

            for(int i = 4, j = 0; i < lines.Length; i++, j++)
            {
                _strings[_keys[j]] = lines[i].Replace("\\n", "\n").Trim('\r');
            }
        }

        /// <summary>
        /// Returns a string by looking for their key
        /// </summary>
        /// <param name="name">Key of the value</param>
        /// <returns></returns>
        public string GetString(string name)
        {
            try
            {
                return _strings[name.ToLower()];
            }
            catch(KeyNotFoundException)
            {
                return "ERROR";
            }
        }
    }
}
