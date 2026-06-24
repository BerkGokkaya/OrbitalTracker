using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OrbitalTracker.Helpers
{
    public class SatelliteMarker
    {
        public SphereVisual3D Visual { get; private set; }

        // YENİ: Uydunun kuyruğu
        public OrbitTrailRenderer Trail { get; private set; }

        public SatelliteMarker(double latitude, double longitude, double altitude, Color color)
        {
            Visual = new SphereVisual3D();
            Visual.Radius = 150;
            Visual.Fill = new SolidColorBrush(color);

            // YENİ: Kuyruğu uydu ile aynı renkte oluştur
            Trail = new OrbitTrailRenderer(color);

            UpdatePosition(latitude, longitude, altitude);
        }

        public void UpdatePosition(double latitude, double longitude, double altitude)
        {
            var coords = CoordinateConverter.ToCartesian(latitude, longitude, altitude);
            Point3D newPos = new Point3D(coords.X, coords.Y, coords.Z);

            // Kürenin merkezini güncelle
            Visual.Center = newPos;

            // YENİ: Uydunun yeni konumunu kuyruğa da bildir
            Trail.AddPoint(newPos);
        }
    }
}