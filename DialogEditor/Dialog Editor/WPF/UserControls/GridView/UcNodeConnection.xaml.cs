using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace SCR.Tools.DialogEditor.WPF.UserControls.GridView
{
    /// <summary>
    /// Interaction logic for UcNodeConnection.xaml
    /// </summary>
    public partial class UcNodeConnection : UserControl
    {
        public double CanvasX
        {
            get => Canvas.GetLeft(this);
            set => Canvas.SetLeft(this, value);
        }

        public double CanvasY
        {
            get => Canvas.GetTop(this);
            set => Canvas.SetTop(this, value);
        }

        private Point _canvasEndPosition = default;

        public UcNodeConnection()
        {
            InitializeComponent();
        }

        private Geometry GenerateDisconnected(Point endPosition)
        {
            return Geometry.Parse($"m 0 0 h 20 m 4 0 h 8 m 6 0 h 6 M {(int)endPosition.X} {(int)endPosition.Y} h -20 m -4 0 h -8 m -6 0 h -6");
        }

        private Geometry GenerateBezierCurve(Point endPosition)
        {
            int bt = (int)(endPosition.X / 3d);
            return Geometry.Parse($"m 0 0 C {bt} 0 {bt * 2} {(int)endPosition.Y} {(int)endPosition.X} {(int)endPosition.Y}");
        }

        public void SetStartPosition(Point position)
        {
            CanvasX = position.X;
            CanvasY = position.Y;

            UpdateLine();
        }

        public void SetEndPosition(Point position)
        {
            _canvasEndPosition = position;
            UpdateLine();
        }

        private void UpdateLine()
        {
            Point relativeEndPos = new(
                _canvasEndPosition.X - CanvasX,
                _canvasEndPosition.Y - CanvasY);

            if(relativeEndPos.X < Math.Abs(relativeEndPos.Y * 0.2))
            {
                LinePath.Data = GenerateDisconnected(relativeEndPos);
            }
            else
            {
                LinePath.Data = GenerateBezierCurve(relativeEndPos);
            }
        }
    }
}
