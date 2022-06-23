using System.Windows;
using System.Windows.Input;

namespace SCR.Tools.WPF
{
    public class InputBindingAttach
    {
        public static readonly DependencyProperty InputBindingsProperty =
            DependencyProperty.RegisterAttached("InputBindings", typeof(InputBindingCollection), typeof(InputBindingAttach),
            new FrameworkPropertyMetadata(new InputBindingCollection(),
            (sender, e) =>
            {
                if (sender is not UIElement element)
                    return;

                element.InputBindings.Clear();
                element.InputBindings.AddRange((InputBindingCollection)e.NewValue);
            }));

        public static InputBindingCollection GetInputBindings(UIElement element)
        {
            return (InputBindingCollection)element.GetValue(InputBindingsProperty);
        }

        public static void SetInputBindings(UIElement element, InputBindingCollection inputBindings)
        {
            element.SetValue(InputBindingsProperty, inputBindings);
        }
    }
}
