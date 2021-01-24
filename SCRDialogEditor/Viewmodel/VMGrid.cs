using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Shapes;
using SCRCommon.Viewmodels;

namespace SCRDialogEditor.Viewmodel
{
    public class VMGrid : BaseViewModel
    {
        public ObservableCollection<VMNode> Nodes { get; private set; }

        public VisualBrush Background { get; private set; }

        public int OffsetX { get; set; }

        public int OffsetY { get; set; }

        public int Zoom { get; set; }

        public VMGrid()
        {
            Nodes = new ObservableCollection<VMNode>();
        }

        public void UpdateBackground()
        {
            Background = new VisualBrush()
            {
                TileMode = TileMode.Tile,
                ViewportUnits = BrushMappingMode.Absolute,
                ViewboxUnits = BrushMappingMode.Absolute
            };

            Background.Viewport = new System.Windows.Rect(0, 0, 30, 30);
            Background.Viewbox = new System.Windows.Rect(0, 0, 30, 30);
            Background.Visual = new Path
            {
                Fill = new SolidColorBrush(Colors.Red),
                Data = new RectangleGeometry(new System.Windows.Rect(0, 0, 29, 29))
            };
        }
    }
}
