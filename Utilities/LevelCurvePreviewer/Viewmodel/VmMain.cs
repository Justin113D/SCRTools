using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SCR.Tools.LevelCurvePreviewer.Data;
using System;
using System.Collections.ObjectModel;
using SCR.Tools.WPF.Viewmodeling;

namespace SCR.Tools.LevelCurvePreviewer.Viewmodel
{
    public class VmMain : BaseViewModel
    {
        public PlotModel ExperienceModel { get; private set; }

        private ulong _expMin = 200;
        private ulong _expMax = 1000000000;
        private double _expXShift = 0;
        private double _expYShift = 1;

        public double ExpMin
        {
            get => _expMin;
            set
            {
                ulong val = (ulong)Math.Round(value);
                if(val == _expMin)
                    return;
                _expMin = val;
                UpdatePlot();
            }
        }

        public double ExpMax
        {
            get => _expMax;
            set
            {
                ulong val = (ulong)Math.Round(value);
                if(val == _expMax)
                    return;
                _expMax = val;
                UpdatePlot();
            }
        }

        public double ExpXShift
        {
            get => _expXShift;
            set
            {
                if(value == _expXShift)
                    return;
                _expXShift = value;
                UpdatePlot();
            }
        }

        public double ExpYShift
        {
            get => _expYShift;
            set
            {
                if(value == _expYShift)
                    return;
                _expYShift = value;
                UpdatePlot();
            }
        }

        private ExpCalculator _expCalculator;

        private LineSeries _absolute;

        private LineSeries _relative;

        public VmMain()
        {
            _absolute = new LineSeries()
            {
                Title = "Absolute Exp per Level",
                YAxisKey = "Abs"
            };

            _relative = new LineSeries()
            {
                Title = "Relative Exp per Level",
                YAxisKey = "Rel"
            };

            UpdatePlot();
            
            ExperienceModel = new PlotModel()
            {
                Title = "Exp growth graph"
            };

            ExperienceModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Bottom,
                Title = "Level",
                IsZoomEnabled = false,
                IsPanEnabled = false
            });

            ExperienceModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Left,
                Title = "Absolute Exp",
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Key = "Abs"
            });

            ExperienceModel.Axes.Add(new LinearAxis()
            {
                Position = AxisPosition.Right,
                Title = "Relative Exp",
                IsZoomEnabled = false,
                IsPanEnabled = false,
                Key = "Rel"
            });

            ExperienceModel.Series.Add(_absolute);
            ExperienceModel.Series.Add(_relative);
        }

        private double f(double x)
        {
            return _expCalculator.GetExperience((uint)x);
        }

        private void UpdatePlot()
        {
            _expCalculator = new ExpCalculator(_expMin, _expMax, _expXShift, _expYShift);

            _absolute.Points.Clear();
            _relative.Points.Clear();
            double oldVal = 0;
            for(uint x = 1; x <= 100; x++)
            {
                double newVal = _expCalculator.GetExperience(x);
                _absolute.Points.Add(new DataPoint(x, _expCalculator.GetExperience(x)));
                _relative.Points.Add(new DataPoint(x, newVal - oldVal));
                oldVal = newVal;
            }

            if(ExperienceModel != null)
            {
                ExperienceModel.InvalidatePlot(true);
            }
        }
    }
}
