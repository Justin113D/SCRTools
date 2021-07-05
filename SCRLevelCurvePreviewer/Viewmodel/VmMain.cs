using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using SCRCommon.Viewmodels;
using SCRLevelCurvePreviewer.Data;
using System;
using System.Collections.ObjectModel;

namespace SCRLevelCurvePreviewer.Viewmodel
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

        private LineSeries _series;

        public VmMain()
        {
            _series = new LineSeries()
            {
                Title = "Experience per level"
            };

            UpdatePlot();
            
            ExperienceModel = new PlotModel()
            {
                Title = "Exp growth graph",
                
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
                Title = "Experience",
                IsZoomEnabled = false,
                IsPanEnabled = false
            });

            ExperienceModel.Series.Add(_series);

        }

        private double f(double x)
        {
            return _expCalculator.GetExperience((uint)x);
        }

        private void UpdatePlot()
        {
            _expCalculator = new ExpCalculator(_expMin, _expMax, _expXShift, _expYShift);

            _series.Points.Clear();
            for(uint x = 1; x <= 100; x++)
                _series.Points.Add(new DataPoint(x, _expCalculator.GetExperience(x)));

            if(ExperienceModel != null)
            {
                ExperienceModel.InvalidatePlot(true);
            }
        }
    }
}
