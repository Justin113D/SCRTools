namespace SCR.Tools.TranslationEditor.Test
{
    public class VM_Texts
    {
        /* 
        Note: Usually its better to read the texts when switching languages into some variables, instead
        of getting them via properties from the reader. In this case its ok, since they only get the texts
        when the program forces an update in the MainViewModel. In other words, this program already ensures
        that the texts only get loaded once on language-switch.
         */

        private MainViewModel mv;

        public string Info => mv.LangReader.GetString("info");
        public string Language => mv.LangReader.GetString("language");
        public string List => mv.LangReader.GetString("list");
        public string Loaded => $"{mv.LangReader.GetString("loaded")} {mv.CurrentLanguage.ListName}";
        public string Status => mv.LangReader.GetString("status").Replace("#COUNT", mv.Languages.Count.ToString());
        public string Thanks => mv.LangReader.GetString("thanks");
        public string Title => mv.LangReader.GetString("title");

        public VM_Texts(MainViewModel mv)
        {
            this.mv = mv;
        }
    }
}
