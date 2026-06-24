using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace OrbitalTracker.Helpers
{
    public class SatelliteMarker
    {
        public SphereVisual3D Visual { get; private set; }
        public OrbitTrailRenderer Trail { get; private set; }

        public string Name { get; set; }
        public double CurrentLat { get; set; }
        public double CurrentLon { get; set; }
        public double CurrentAlt { get; set; }
        public double Speed { get; set; }

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

            // --- AKILLI TABAN LİMİTİ ---
            // Eğer uydu yüksek yöründeyse (Türksat gibi), çizgisi sönük kalmasın diye 
            // ona 500 noktalık devasa bir hafıza veriyoruz. Normal uydulara 120 yetiyor.
            int baseTrailLimit = altitude > 10000 ? 500 : 120;
            Trail = new OrbitTrailRenderer(color, baseTrailLimit);

            UpdatePosition(CurrentLat, CurrentLon, CurrentAlt);
        }

        public void MoveForward(double speedMultiplier = 1.0)
        {
            CurrentLon += (Speed * speedMultiplier);

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