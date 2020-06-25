using SharpDX;
using SharpDX.Direct2D1;
using SharpDX.Mathematics.Interop;
using System;

namespace LottieSharp
{
    internal class PathMeasure : IDisposable
    {
        private CachedPathIteratorFactory _originalPathIterator;
        private Path _path;
        private Geometry _geometry;

        public PathMeasure(Path path)
        {
            _originalPathIterator = new CachedPathIteratorFactory(new FullPathIterator(path));
            _path = path;

            using (var factory = new Factory(FactoryType.SingleThreaded))
                _geometry = _path.GetGeometry(factory);
            Length = _geometry.ComputeLength();
        }

        public void SetPath(Path path)
        {
            _originalPathIterator = new CachedPathIteratorFactory(new FullPathIterator(path));
            _path = path;
            _geometry?.Dispose();
            using (var factory = new Factory(FactoryType.SingleThreaded))
                _geometry = _path.GetGeometry(factory);
            Length = _geometry.ComputeLength();
        }

        public float Length { get; private set; }

        public Vector2 GetPosTan(float distance)
        {
            if (distance < 0)
                distance = 0;

            var length = Length;
            if (distance > length)
                distance = length;

            RawVector2 vectOutput;
            var vect2 = _geometry.ComputePointAtLength(distance, out vectOutput);

            return new Vector2(vect2.X, vect2.Y);
        }

        public bool GetSegment(float startD, float stopD, ref Path dst, bool startWithMoveTo)
        {
            var length = Length;

            if (startD < 0)
            {
                startD = 0;
            }

            if (stopD > length)
            {
                stopD = length;
            }

            if (startD >= stopD)
            {
                return false;
            }

            var iterator = _originalPathIterator.Iterator();

            var accLength = startD;
            var isZeroLength = true;

            var points = new float[6];

            iterator.JumpToSegment(accLength);

            while (!iterator.Done && stopD - accLength > 0.1f)
            {
                var type = iterator.CurrentSegment(points, stopD - accLength);

                if (accLength - iterator.CurrentSegmentLength <= stopD)
                {
                    if (startWithMoveTo)
                    {
                        startWithMoveTo = false;

                        if (type != PathIterator.ContourType.MoveTo == false)
                        {
                            var lastPoint = new float[2];
                            iterator.GetCurrentSegmentEnd(lastPoint);
                            dst.MoveTo(lastPoint[0], lastPoint[1]);
                        }
                    }

                    isZeroLength = isZeroLength && iterator.CurrentSegmentLength > 0;
                    switch (type)
                    {
                        case PathIterator.ContourType.MoveTo:
                            dst.MoveTo(points[0], points[1]);
                            break;
                        case PathIterator.ContourType.Line:
                            dst.LineTo(points[0], points[1]);
                            break;
                        case PathIterator.ContourType.Close:
                            dst.Close();
                            break;
                        case PathIterator.ContourType.Bezier:
                        case PathIterator.ContourType.Arc:
                            dst.CubicTo(points[0], points[1],
                                points[2], points[3],
                                points[4], points[5]);
                            break;
                    }
                }

                accLength += iterator.CurrentSegmentLength;
                iterator.Next();
            }

            return !isZeroLength;
        }

        internal void Dispose(bool disposing)
        {
            if (_geometry != null)
            {
                try
                {
                    if (disposing)
                    {
                        _geometry.Dispose();
                    }
                    else
                    {
                        if (System.Runtime.InteropServices.Marshal.IsComObject(_geometry))
                        {
                            System.Runtime.InteropServices.Marshal.ReleaseComObject(_geometry);
                        }
                    }
                }
                catch (Exception)
                {
                    // Ignore, but should not happen
                }
                finally
                {
                    _geometry = null;
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~PathMeasure()
        {
            Dispose(false);
        }
    }
}
