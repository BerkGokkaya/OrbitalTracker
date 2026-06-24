using HelixToolkit.Wpf;
using System;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OrbitalTracker.Helpers
{
    public class OrbitTrailRenderer
    {
        public LinesVisual3D Visual { get; private set; }
        private Point3DCollection _points;

        // Hıza ve uydu türüne göre anlık değişecek limit
        public int MaxTrailLength { get; set; }
        private Point3D _lastAddedPoint;

        public OrbitTrailRenderer(Color trailColor, int defaultMaxValues, double thickness = 1.5)
        {
            Visual = new LinesVisual3D();
            Visual.Color = trailColor;
            Visual.Thickness = thickness;
            MaxTrailLength = defaultMaxValues;
            _points = new Point3DCollection();
        }

        public void AddPoint(Point3D newPoint)
        {
            if (_points.Count > 0)
            {
                double distance = newPoint.DistanceTo(_lastAddedPoint);
                // Çizgilerin kesik kesik durmaması için pürüzsüzlük eşiği
                if (distance < 5.0)
                    return;
            }

            _points.Add(newPoint);
            _lastAddedPoint = newPoint;

            // Limit düştüğünde çizgiyi aniden silmek yerine geriye doğru zarifçe budar
            while (_points.Count > MaxTrailLength)
            {
                _points.RemoveAt(0);
            }

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            var linePoints = new Point3DCollection(_points.Count * 2);

            for (int i = 0; i < _points.Count - 1; i++)
            {
                linePoints.Add(_points[i]);
                linePoints.Add(_points[i + 1]);
            }

            Visual.Points = linePoints;
        }
    }
}