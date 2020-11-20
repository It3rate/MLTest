using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;

namespace MLTest.Sim
{
    /// <summary>
    /// Specialized series for 2D Beziers, holds the extra move data.
    /// </summary>
    public class SimBezier
    {
        public static readonly int[] MoveSize = new int[] { 2, 2, 4, 6, 0 };

        protected float[] _points;
        public int Count => _points.Length;

        public SimBezierMove[] Moves { get; }
        public double Length => _polylineLength;

        private int _polyLineCount = 32;
        private float[] _polylineDistances;
        private float _polylineLength;
        private bool _evenlySpaced = false;
        public bool EvenlySpaced
        {
            get => _evenlySpaced;
            set
            {
                // Use dirty flag if this gets repetitious.
                if (value)
                {
                    // Need to use normal bezier sampling to convert to polylines.
                    _evenlySpaced = false;
                    MakeEvenlySpaced();
                }

                _evenlySpaced = value;
            }
        }

        public SimBezier(float[] values, SimBezierMove[] moves = null)
        {
            _points = values;

            // default to polyline if moves is empty
            if (moves == null)
            {
                var len = values.Length / 2;
                moves = new SimBezierMove[len];
                for (var i = 0; i < len; i++)
                {
                    moves[i] = SimBezierMove.LineTo;
                }
            }

            Moves = moves;
            EvenlySpaced = true;
        }
        public SimBezier(PointF[] values)
        {
            List<SimBezierMove> moves = new List<SimBezierMove>();
            List<float> pts = new List<float>();
            pts.AddRange(new[] { values[0].X, values[0].Y });
            moves.Add(SimBezierMove.MoveTo);
            if(values.Length == 2)
            {
                pts.AddRange(new[] { values[1].X, values[1].Y });
                moves.Add(SimBezierMove.LineTo);
            }
            else
            {
                int index = 1;
                while(values.Length > index + 2)
                {
                    pts.AddRange(new[]{
                        values[index].X, values[index].Y,
                        values[index + 1].X, values[index + 1].Y,
                        values[index + 2].X, values[index + 2].Y,
                        });
                    moves.Add(SimBezierMove.CubeTo);
                    index += 3;
                }
                //pts.AddRange(new[] { values[values.Length - 2].X, values[values.Length - 1].Y });
            }
            //moves.Add(SimBezierMove.End);

            _points = pts.ToArray();
            Moves = moves.ToArray();
            EvenlySpaced = true;
        }

        private void MakeEvenlySpaced()
        {
            MeasureBezier();

        }

        private List<PointF> _polyPoints = new List<PointF>();
        private void MeasureBezier()
        {
            _polyPoints.Clear();
            float len = 0;
            _polylineDistances = new float[_polyLineCount];

            if (Moves.Length > 1)
            {
                var pt0 = GetPointAtT(0);
                _polyPoints.Add(pt0);
                var x0 = pt0.X;
                var y0 = pt0.Y;
                for (int i = 1; i < _polyLineCount; i++)
                {
                    var pt1 = GetPointAtT(i / (float)(_polyLineCount - 1.0));
                    _polyPoints.Add(pt1);
                    var x1 = pt1.X;
                    var y1 = pt1.Y;
                    var xDif = x1 - x0;
                    var yDif = y1 - y0;
                    len += (float)Math.Sqrt(xDif * xDif + yDif * yDif);
                    _polylineDistances[i] = len;
                    x0 = x1;
                    y0 = y1;
                }
            }
            _polylineLength = len;
        }

        public float[] GetRawDataAt(int index)
        {
            index = Math.Max(0, Math.Min(Moves.Length - 1, index));
            var start = 0;
            for (var i = 0; i < index; i++)
            {
                start += MoveSize[(int)Moves[i]];
            }

            var size = MoveSize[(int)Moves[index]];
            var result = new float[size];
            Array.Copy(_points, start, result, 0, size);
            return result; // new SimBezier(result, new[] { Moves[index] });
        }

        public PointF GetPointAtT(double _t)
        {
            var fullT = (float)_t;
            //if(_t >= 1.0)
            //{
            //    return new PointF(_points[_points.Length - 2], _points[_points.Length - 1]);
            //}
            float t;
            int startIndex, endIndex;

            if (EvenlySpaced)
            {
                GetEvenSpacedSegmentFromT(fullT, out t, out startIndex, out endIndex);
            }
            else
            {
                GetSegmentFromT(fullT, out t, out startIndex, out endIndex);
            }

            float[] aSeries = GetRawDataAt(startIndex);
            var p0 = new[] { aSeries[aSeries.Length - 2], aSeries[aSeries.Length - 1] };
            float[] p1 = GetRawDataAt(endIndex);
            var moveType = endIndex < Moves.Length ? Moves[endIndex] : SimBezierMove.End;

            float[] result = { 0, 0 };
            var mt = 1f - t;
            var t2 = t * t;
            var mt2 = mt * mt;
            switch (moveType)
            {
                case SimBezierMove.MoveTo:
                    result[0] = p0[0];
                    result[1] = p0[1];
                    break;
                case SimBezierMove.LineTo:
                    result[0] = p0[0] + (p1[0] - p0[0]) * t;
                    result[1] = p0[1] + (p1[1] - p0[1]) * t;
                    break;
                case SimBezierMove.QuadTo:
                    result[0] = mt2 * p0[0] + 2 * mt * t * p1[0] + t2 * p1[2];
                    result[1] = mt2 * p0[1] + 2 * mt * t * p1[1] + t2 * p1[3];
                    break;
                case SimBezierMove.CubeTo:
                    var t3 = t * t * t;
                    var mt3 = mt * mt * mt;
                    result[0] = mt3 * p0[0] + 3 * mt2 * t * p1[0] + 3 * mt * t2 * p1[2] + t3 * p1[4];
                    result[1] = mt3 * p0[1] + 3 * mt2 * t * p1[1] + 3 * mt * t2 * p1[3] + t3 * p1[5];
                    break;
                case SimBezierMove.End: // special case when t == 1
                    result[0] = p0[0];
                    result[1] = p0[1];
                    break;
                default:
                    result = p1;
                    break;
            }
            return new PointF(result[0], result[1]);
        }

        public void GetSegmentFromT(float t, out float remainder, out int startIndex, out int endIndex)
        {
            int drawableSegments = Moves.Length - 1;
            startIndex = (int)Math.Floor(t * drawableSegments);
            remainder = (t - startIndex / (float)drawableSegments) * drawableSegments;
            endIndex = startIndex + 1;
        }
        public void GetEvenSpacedSegmentFromT(float t, out float remainder, out int startIndex, out int endIndex)
        {
            var targetLength = _polylineLength * t;
            int pos = 0;
            for (; pos < _polyLineCount - 1; pos++)
            {
                if (_polylineDistances[pos + 1] > targetLength)
                {
                    break;
                }
            }

            float newT = 1;
            if (pos < _polyLineCount - 1)
            {
                newT = pos / (float)(_polyLineCount - 1);
                float rem = (targetLength - _polylineDistances[pos]) / (_polylineDistances[pos + 1] - _polylineDistances[pos]);
                newT += rem / (_polyLineCount - 1);
            }

            int drawableSegments = Moves.Length - 1;
            startIndex = (int)Math.Floor(newT * drawableSegments);
            remainder = (newT - startIndex / (float)drawableSegments) * drawableSegments;
            endIndex = startIndex + 1;
        }
        public void ReverseEachElement()
        {
            _points.Reverse();
            Array.Reverse(Moves);
        }

        public GraphicsPath Path()
        {
            var path = new GraphicsPath();
            path.FillMode = FillMode.Alternate;
            for (int i = 1; i < _polyPoints.Count; i++)
            {
                path.AddLine(_polyPoints[i - 1], _polyPoints[i]);
            }
            return path;
            var index = 0;
            float posX = 0;
            float posY = 0;
            foreach (var moveType in Moves)
            {
                switch (moveType)
                {
                    case SimBezierMove.MoveTo:
                        posX = _points[index];
                        posY = _points[index + 1];
                        break;
                    case SimBezierMove.LineTo:
                        path.AddLine(posX, posY, _points[index], _points[index + 1]);
                        posX = _points[index];
                        posY = _points[index + 1];
                        break;
                    case SimBezierMove.QuadTo:
                        // must convert to cubic for gdi
                        var cx = _points[index];
                        var cy = _points[index + 1];
                        var a1x = _points[index + 2];
                        var a1y = _points[index + 3];
                        var c1x = (cx - posX) * 2 / 3 + posX;
                        var c1y = (cy - posY) * 2 / 3 + posY;
                        var c2x = a1x - (a1x - cx) * 2 / 3;
                        var c2y = a1y - (a1y - cy) * 2 / 3;
                        path.AddBezier(posX, posY, c1x, c1y, c2x, c2y, a1x, a1y);
                        posX = a1x;
                        posY = a1y;
                        break;
                    case SimBezierMove.CubeTo:
                        path.AddBezier(posX, posY,
                            _points[index], _points[index + 1],
                            _points[index + 2], _points[index + 3],
                            _points[index + 4], _points[index + 5]);
                        posX = _points[index + 4];
                        posY = _points[index + 5];
                        break;
                    default:
                        path.CloseFigure();
                        break;
                }
                index += MoveSize[(int)moveType];
            }

            return path;
        }
        public SimBezier Copy()
        {
            SimBezier result = new SimBezier((float[])_points.Clone(), (SimBezierMove[])Moves.Clone());
            return result;
        }
        //// todo: need an insertDataAtIndex
        //public override void SetRawDataAt(int index, Series series)
        //{
        //    index = Math.Max(0, Math.Min(Moves.Length - 1, index));
        //    var start = 0;
        //    for (var i = 0; i < index; i++)
        //    {
        //        start += MoveSize[(int)Moves[i]];
        //    }

        //    // todo: need to adjust float array length if move size is different
        //    var size = MoveSize[(int)Moves[index]];
        //    if (series is BezierSeries)
        //    {
        //        var newMove = ((BezierSeries)series).Moves[0];
        //        var newSize = MoveSize[(int)newMove];
        //        if (newSize != size)
        //        {
        //            int diff = newSize - size;
        //            float[] newFloats = new float[_floatValues.Length + diff];
        //            Array.Copy(_floatValues, 0, newFloats, 0, start);
        //            Array.Copy(series.FloatDataRef, 0, newFloats, start, newSize);
        //            Array.Copy(_floatValues, start + 1, newFloats, start + newSize, _floatValues.Length - start);
        //            _floatValues = newFloats; // todo: make immutable, return a series (or this if no change) to store.
        //        }
        //        else
        //        {
        //            Array.Copy(series.FloatDataRef, 0, _floatValues, start, newSize);
        //        }
        //        Moves[index] = newMove;
        //    }
        //    else
        //    {
        //        Array.Copy(series.FloatDataRef, 0, _floatValues, start, size);
        //    }
        //}
    }

    // maybe store all values as quadratic, allowing easier blending?
    // todo: split into all quadratic beziers and all poly lines. 
    // Use identical first and second coordinate for lines in quadratic, *or use midpoint and Move data determines lines etc.
    // try blending with len/angle from zero vs interpolate points.
    public enum SimBezierMove : int
    {
        MoveTo,
        LineTo,
        QuadTo,
        CubeTo,
        End,
    }

}