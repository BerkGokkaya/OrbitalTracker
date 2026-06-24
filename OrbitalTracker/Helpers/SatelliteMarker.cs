using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OrbitalTracker.Helpers
{
    public class SatelliteMarker
    {
        public SphereVisual3D Visual { get; private set; }

        public SatelliteMarker(double latitude, double longitude, double altitude, Color color)
        {
            Visual = new SphereVisual3D();
            Visual.Radius = 150;
            Visual.Fill = new SolidColorBrush(color);

            UpdatePosition(latitude, longitude, altitude);
        }

        public void UpdatePosition(double latitude, double longitude, double altitude)
        {
            // YENİ TUPLE YAPISINA GÖRE GÜNCELLEDİK
            var coords = CoordinateConverter.ToCartesian(latitude, longitude, altitude);
            Visual.Center = new Point3D(coords.X, coords.Y, coords.Z);
        }
    }
}