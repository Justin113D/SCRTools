using SCR.Tools.Dialog.Editor.Viewmodeling;
using SCR.Tools.DynamicDataExpression;
using SCR.Tools.Viewmodeling;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace SCR.Tools.Dialog.Editor.WPF.UserControls
{
    /// <summary>
    /// Interaction logic for UcNodeOutputInspector.xaml
    /// </summary>
    public partial class UcNodeOutputInspector : UserControl
    {
        public static readonly DependencyProperty CmdFocusNodeProperty =
           DependencyProperty.Register(
               nameof(CmdFocusNode),
               typeof(RelayCommand<VmNode>),
               typeof(UcNodeOutputInspector)
           );

        public RelayCommand<VmNode> CmdFocusNode
        {
            get => (RelayCommand<VmNode>)GetValue(CmdFocusNodeProperty);
            set => SetValue(CmdFocusNodeProperty, value);
        }

        public VmNodeOutput ViewModel
            => ((VmNodeOutput)DataContext);


        public UcNodeOutputInspector()
        {
            DataContextChanged += UcNodeOutputInspector_DataContextChanged;
            Unloaded += UcNodeOutputInspector_Unloaded;
            InitializeComponent();

        }

        private void UcNodeOutputInspector_Unloaded(object sender, RoutedEventArgs e)
        {
            ViewModel.PropertyChanged -= PropertyChanged;
        }

        private void UcNodeOutputInspector_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.OldValue != null)
            {
                // unexpected behavior
                throw new InvalidOperationException();
            }

            if (e.NewValue is INotifyPropertyChanged newINotify)
            {
                newINotify.PropertyChanged += PropertyChanged;
            }
            
        }

        private void PropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(VmNodeOutput.Condition))
            {
                UpdateConditionErrorHighlight();
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            IconSelection.SelectedItem = null;
        }

        private void UpdateConditionErrorHighlight()
        {

            try
            {
                if(!string.IsNullOrWhiteSpace(ViewModel.Condition))
                {
                    DataExpression.ValidateExpression(ViewModel.Condition);
                }
            }
            catch(DynamicDataExpressionException e)
            {
                int index = 0;
                int length = 1;
                if (e.Index == -1)
                {
                    length = ViewModel.Condition.Length;
                }
                else
                {
                    index = e.Index;
                }
                syntaxHighlighting.Text = new string('\u00A0', index) + new string('~', length);
                SyntaxError.Text = e.Message;
                return;
            }

            syntaxHighlighting.Text = "";
            SyntaxError.Text = "";
        }

        private void TextBox_PreviewKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if(e.Key == System.Windows.Input.Key.Enter)
            {
                TextBox textbox = (TextBox)sender;
                BindingExpression? binding = BindingOperations.GetBindingExpression(textbox, TextBox.TextProperty);
                binding.UpdateSource();
                e.Handled = true;
            }
        }
    }
}
