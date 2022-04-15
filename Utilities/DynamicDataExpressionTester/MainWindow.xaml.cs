using SCR.Tools.DynamicDataExpressionTester.Data;
using System;
using System.Windows;
using System.Windows.Input;
using System.Text.Json;
using SCR.Tools.DynamicDataExpression;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Linq;
using SCR.Tools.Viewmodeling;

namespace SCR.Tools.DynamicDataExpressionTester
{
    public struct SyntaxHighlight
    {
        public int Index { get; }
        public int Length { get; }
        public string Message { get; }

        public string Display
            => new string(' ', Index) + new string('~', Length);

        public SyntaxHighlight(int index, int length, string message)
        {
            Index = index;
            Length = length;
            Message = message;
        }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // random example data i came up with on the fly
        private static readonly MockSCRData resetData = new()
        {
            Rings = 100,
            Flags = new() { { 1, true }, { 2, false } },
            Cards = new() { 1, 2, 3 },
            Items = new() { { 1, 5 }, { 4, 6 }, { 3, 5 }, { 22, 1 } },
            Chao = new() { { 50, new(1, 2) }, { 51, new(2, 50) } },
            TeamMembers = new() { { 1, new(2, 20, 10, new int[] { 3, 5, 18 }) }, { 2, new(5, 45, 2, new int[] { 4 }) } },
            PartyMembers = new() { 1, 2, 4 }
        };

        private MockSCRData testData;

        private ObservableCollection<SyntaxHighlight> _syntaxErrors;

        private ToolTip syntaxHighlight;

        private int currentSyntaxLine = -1;

        public MainWindow()
        {
            _syntaxErrors = new();
            InitializeComponent();

            expressions.Text = "";
            syntaxHighlighting.Text = "";
            syntaxHighlight = new ToolTip();
            expressions.ToolTip = syntaxHighlight;

            InputBindings.Add(new KeyBinding(new RelayCommand(() => Evaluate(default, default)), new KeyGesture(Key.Enter, ModifierKeys.Control)));

            Reset(this, null);
        }

        private void UpdateJson()
        {
            JsonSerializerOptions options = new()
            {
                WriteIndented = true
            };

            jsonBox.Text = JsonSerializer.Serialize(testData, options);
        }

        private void UpdateData()
        {
            try
            {
                MockSCRData newData = JsonSerializer.Deserialize<MockSCRData>(jsonBox.Text);
                testData = newData;
                MessageBox.Show("Successfully updated data!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch(Exception e)
            {
                MessageBox.Show("Error occured while parsing data:\n" + e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Update(object sender, RoutedEventArgs e)
        {
            UpdateData();
            Evaluate(sender, e);
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            testData = resetData;
            UpdateJson();
            Evaluate(sender, e);
        }

        private void jsonBox_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.Key == Key.Tab)
            {
                var data = Clipboard.GetDataObject();
                var tab = new string(' ', 4);
                Clipboard.SetData(DataFormats.Text, tab);
                jsonBox.Paste();
                //put the original clipboard data back
                if(data != null)
                {
                    Clipboard.SetDataObject(data);
                }
                e.Handled = true;
            }
        }

        private void Evaluate(object sender, RoutedEventArgs e)
        {
            string[] lines = expressions.Text.Split("\n");
            string result = "";

            _syntaxErrors.Clear();

            foreach(var l in lines)
            {
                string exp = l.TrimEnd();

                if(string.IsNullOrWhiteSpace(exp))
                {
                    result += "-\n";
                    _syntaxErrors.Add(new());
                    continue;
                }

                try
                {
                    var t = DataExpression<MockSCRData>.ParseExpression(exp, DataAccessor.DA);
                    result += $"{t.Evaluate(testData)}\n";
                    _syntaxErrors.Add(new());
                }
                catch(DynamicDataExpressionException d)
                {
                    result += "Error\n";
                    if(d.Index == -1)
                    {
                        _syntaxErrors.Add(new(0, exp.Length, d.Message));
                    }
                    else
                    {
                        _syntaxErrors.Add(new(d.Index, 1, d.Message));
                    }
                }
            }

            syntaxHighlighting.Text = string.Join('\n',_syntaxErrors.Select(x => x.Display));

            results.Text = result;
        }

        private void expressions_MouseMove(object sender, MouseEventArgs e)
        {
            int charIndex = expressions.GetCharacterIndexFromPoint(e.GetPosition(expressions), false);
            if(charIndex == -1)
            {
                if(currentSyntaxLine != -1)
                {
                    currentSyntaxLine = -1;
                    syntaxHighlight.IsOpen = false;
                }
                return;
            }

            int lineIndex = expressions.GetLineIndexFromCharacterIndex(charIndex);
            if(currentSyntaxLine == lineIndex)
                return;
            currentSyntaxLine = lineIndex;

            SyntaxHighlight t;
            try
            {
                t = _syntaxErrors[currentSyntaxLine];
            }
            catch(ArgumentOutOfRangeException)
            {
                syntaxHighlight.IsOpen = false;
                return;
            }

            if(t.Length == 0)
            {
                syntaxHighlight.IsOpen = false;
            }
            else
            {
                syntaxHighlight.IsOpen = true;
                syntaxHighlight.Content = t.Message;
            }
        }
    }
}
