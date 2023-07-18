using System.Windows.Controls;

namespace SCR.Tools.WPF.Controls
{
    public class SettingsControl : UserControl
    {
        public virtual double WindowWidth => 400;
        public virtual double WindowHeight => 200;
        public virtual void OnSettingsClose() { }
    }
}
