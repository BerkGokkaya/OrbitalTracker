using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OrbitalTracker.Helpers
{
    public class OrbitTrailRenderer
    {
        public LinesVisual3D Visual { get; private set; }
        private Point3DCollection _points;
        private int _maxTrailLength = 100; // Çizgi uzunluğu 150'den 100'e düştü (optimizasyon)

        // YENİ: Son eklenen noktayı hafızada tutacağız
        private Point3D _lastAddedPoint;

        public OrbitTrailRenderer(Color trailColor, double thickness = 1.5)
        {
            Visual = new LinesVisual3D();
            Visual.Color = trailColor;
            Visual.Thickness = thickness;
            _points = new Point3DCollection();
        }

        public void AddPoint(Point3D newPoint)
        {
            // --- MESAFE OPTİMİZASYONU ---
            // Eğer daha önce nokta eklendiyse ve yeni nokta eskisine çok yakınsa 
            // (örn: 50 birimden az) yeni bir çizgi çekme, pas geç!
            if (_points.Count > 0)
            {
                double distance = newPoint.DistanceTo(_lastAddedPoint);
                if (distance < 50.0)
                    return;
            }

            _points.Add(newPoint);
            _lastAddedPoint = newPoint;

            if (_points.Count > _maxTrailLength)
            {
                _points.RemoveAt(0);
            }

            UpdateVisual();
        }

        private void UpdateVisual()
        {
            // YENİ: Kapasiteyi önceden belirlemek RAM (Allocation) tasarrufu sağlar
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