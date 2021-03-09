using SCRCommon.Viewmodels;
using System.Collections.ObjectModel;
using System.IO;


namespace SCRLanguageTest
{
    public struct LanguageOption
    {
        /// <summary>
        /// Name of the file (english name of the language)
        /// </summary>
        public string FileName { get; }

        /// <summary>
        /// Name of the language (in the language itself)
        /// </summary>
        public string LanguageName { get; }

        /// <summary>
        /// Display name
        /// </summary>
        public string ListName => $"{FileName} / {LanguageName}";

        public LanguageOption(string fileName, string languageName)
        {
            FileName = fileName;
            LanguageName = languageName;
        }
    }

    public class MainViewModel : BaseViewModel
    {
        /// <summary>
        /// Language reader
        /// </summary>
        public LanguageReader LangReader { get; }

        /// <summary>
        /// Loaded languages
        /// </summary>
        public ObservableCollection<LanguageOption> Languages { get; }

        /// <summary>
        /// Object of the current language
        /// </summary>
        private LanguageOption _currentLanguage;

        /// <summary>
        /// Gets and sets the current language
        /// </summary>
        public LanguageOption CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                LoadLanguage(value);
            }
        }

        /// <summary>
        /// Text access
        /// </summary>
        public VM_Texts Texts { get; }

        public MainViewModel()
        {
            Languages = new ObservableCollection<LanguageOption>();

            string[] files = Directory.GetFiles("Languages");
            foreach(string f in files)
            {
                if(Path.GetExtension(f) == ".lang")
                {
                    string[] lines = File.ReadAllLines(f);
                    Languages.Add(new LanguageOption(lines[2], lines[5]));
                }
            }

            // Initialize the language reader with the default language of english
            LangReader = new LanguageReader("Languages/Base.langkey", "Languages/English.lang");

            // Search for the English language object and set it as the current one
            foreach(LanguageOption l in Languages)
            {
                if(l.FileName == "English")
                {
                    _currentLanguage = l;
                    break;
                }
            }

            // Initialize the text access
            Texts = new VM_Texts(this);
        }

        /// <summary>
        /// Used to load other languages
        /// </summary>
        /// <param name="lang"></param>
        public void LoadLanguage(LanguageOption lang)
        {
            if(CurrentLanguage.FileName == lang.FileName)
                return;

            _currentLanguage = lang;

            LangReader.LoadLanguage($"Languages/{lang.FileName}.lang");

            OnPropertyChanged(nameof(Texts));
        }
    }
}
