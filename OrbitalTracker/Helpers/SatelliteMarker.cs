using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OrbitalTracker.Helpers
{
    public class SatelliteMarker
    {
        public SphereVisual3D Visual { get; private set; }
        public OrbitTrailRenderer Trail { get; private set; }

        // Kimlik ve Anlık Konum Özellikleri
        public string Name { get; set; }
        public double CurrentLat { get; set; }
        public double CurrentLon { get; set; }
        public double CurrentAlt { get; set; }
        public double Speed { get; set; } // Her uydunun kendi dönüş hızı

        public SatelliteMarker(string name, double startLat, double startLon, double altitude, Color color, double speed = 0.5)
        {
            Name = name;
            CurrentLat = startLat;
            CurrentLon = startLon;
            CurrentAlt = altitude;
            Speed = speed;

            Visual = new SphereVisual3D();
            Visual.Radius = 150;
            Visual.Fill = new SolidColorBrush(color);

            Trail = new OrbitTrailRenderer(color);

            // Başlangıç konumuna oturt
            UpdatePosition(CurrentLat, CurrentLon, CurrentAlt);
        }

        // Uydunun kendi kendini hareket ettirmesini sağlayan metot
        public void MoveForward()
        {
            CurrentLon += Speed;

            // Dünya'nın etrafından tam tur atınca boylamı sıfırla/normalize et
            if (CurrentLon > 180) CurrentLon -= 360;

            UpdatePosition(CurrentLat, CurrentLon, CurrentAlt);
        }

        public void UpdatePosition(double latitude, double longitude, double altitude)
        {
            var coords = CoordinateConverter.ToCartesian(latitude, longitude, altitude);
            Point3D newPos = new Point3D(coords.X, coords.Y, coords.Z);

            Visual.Center = newPos;
            Trail.AddPoint(newPos);
        }
    }
}