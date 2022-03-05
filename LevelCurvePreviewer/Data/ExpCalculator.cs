using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCR.Tools.LevelCurvePreviewer.Data
{
    /// <summary>
    /// Used to calculate experience requirements based on bezier values <br/>
    /// The code is altered code from Blenders fcurve.c (7/4/2021)
    /// </summary>
    internal class ExpCalculator
    {
        private const double _small = -1.0e-10;

        private readonly ulong[] _origX;

        private readonly ulong[] _origY;

        /// <summary>
        /// processed x values
        /// </summary>
        private readonly double[] _xValues;

        /// <summary>
        /// processed y values
        /// </summary>
        private readonly double[] _yValues;

        internal ExpCalculator(ulong minExp, ulong maxExp, double xShift, double yShift)
        {
            _origX = new ulong[] { 2, 100 };
            _origY = new ulong[] { minExp, maxExp };

            xShift = Math.Max(Math.Min(xShift, 1d), 0d);
            yShift = Math.Max(Math.Min(yShift, 1d), 0d);

            _xValues = new double[] { 2, 2 + xShift * 98, 100, 100 };
            _yValues = new double[] { minExp, minExp, minExp + yShift * (maxExp - minExp), maxExp };

            // clamping in both directions, to ensure that no "hills" are created
            ClampBezier(_xValues, _yValues);
            ClampBezier(_yValues, _xValues);

            _xValues = ProcessValues(_xValues);
            _yValues = ProcessValues(_yValues);
        }

        /// <summary>
        /// Clamps the bezier curve
        /// </summary>
        private static void ClampBezier(double[] x, double[] y)
        {
            double h1x = x[0] - x[1];
            double h2x = x[3] - x[2];

            double len1 = Math.Abs(h1x);
            double len2 = Math.Abs(h2x);

            if((len1 + len2) == 0)
                return;

            double len = x[3] - x[0];

            if(len1 > len)
            {
                double fac = len / len1;
                double hy = y[0] - y[1];
                x[1] = x[0] - fac * h1x;
                y[1] = y[0] - fac * hy;
            }

            if(len2 > len)
            {
                double fac = len / len2;
                double hy = y[3] - y[2];
                x[2] = x[3] - fac * h2x;
                y[2] = y[3] - fac * hy;
            }
        }

        /// <summary>
        /// Processes values
        /// </summary>
        private static double[] ProcessValues(double[] input)
        {
            return new double[]
            {
                input[0],
                3.0f * (input[1] - input[0]),
                3.0f * (input[0] - 2.0f * input[1] + input[2]),
                input[3] - input[0] + 3.0f * (input[1] - input[2])
            };
        }

        private static double Sqrt3D(double d)
        {
            if(d == 0)
                return 0;
            else if(d < 0)
                return -Math.Exp(Math.Log(-d) / 3d);
            else
                return Math.Exp(Math.Log(d) / 3d);
        }

        private static double GetValue(double[] from, ulong value)
        {
            double t, result = 0;

            double c0 = from[0] - value;

            double a = from[2] / from[3];
            double b = from[1] / from[3];
            double c = c0 / from[3];
            a /= 3d;

            double p = b / 3d - a * a;
            double q = (2d * a * a * a - a * b + c) / 2d;
            double d = q * q + p * p * p;

            if(d > 0d)
            {
                t = Math.Sqrt(d);
                return Sqrt3D(-q + t) + Sqrt3D(-q - t) - a;
            }

            if(d == 0d)
            {
                t = Math.Sqrt(-q);

                result = 2 * t - a;
                if((result >= _small) && (result <= 1.000001f))
                    return result;

                return -t - a;
            }

            double phi = Math.Acos(-q / Math.Sqrt(-(p * p * p)));
            t = Math.Sqrt(-p);
            p = Math.Cos(phi / 3);
            q = Math.Sqrt(3 - 3 * p * p);

            result = 2 * t * p - a;
            if((result >= _small) && (result <= 1.000001f))
                return result;

            result = -t * (p + q) - a;
            if((result >= _small) && (result <= 1.000001f))
                return result;

            return -t * (p - q) - a;
        }

        private static double BezierEvaluate(double[] to, double o)
        {
            double o2 = o * o;
            double o3 = o2 * o;
            return to[0] + o * to[1] + o2 * to[2] + o3 * to[3];
        }

        private static double GetValue(double[] from, ulong[] origFrom, double[] to, ulong t)
        {
            double o;
            if(t <= origFrom[0])
                o = 0;
            else if(t >= origFrom[1])
                o = 1;
            else
                o = GetValue(from, t);
            return BezierEvaluate(to, o);
        }

        internal ulong GetExperience(uint level)
        {
            if(level < 2)
                return 0;
            else
                return (ulong)GetValue(_xValues, _origX, _yValues, level);
        }

        internal double GetLevel(ulong experience)
        {
            if(experience < 0)
                return 0;
            else if(experience <= _yValues[0])
                return experience / _yValues[0];
            else
                return GetValue(_yValues, _origY, _xValues, experience);
        }
    }
}
