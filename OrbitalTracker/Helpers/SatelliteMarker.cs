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

            // Uydunun 3D boyutu (Dünya 6371 olduğu için uydular biraz büyük olmalı ki görünsün)
            Visual.Radius = 150;

            // Uydu rengi (örn: Kırmızı)
            Visual.Fill = new SolidColorBrush(color);

            // Koordinatı hesapla ve uyduyu o konuma taşı
            UpdatePosition(latitude, longitude, altitude);
        }

        public void UpdatePosition(double latitude, double longitude, double altitude)
        {
            // Converter'ı kullanarak yeni konumu al
            Point3D newPosition = CoordinateConverter.ToCartesian(latitude, longitude, altitude);

            // SphereVisual3D'nin merkez noktasını güncelle
            Visual.Center = newPosition;
        }
    }
}