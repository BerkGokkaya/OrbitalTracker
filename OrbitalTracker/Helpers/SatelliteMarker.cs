using HelixToolkit.Wpf;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.TLE;
using SGPdotNET.Propagation;
using SGPdotNET.Observation;
using System;

namespace OrbitalTracker.Helpers
{
    public class SatelliteMarker
    {
        public SphereVisual3D Visual { get; private set; }
        public OrbitTrailRenderer Trail { get; private set; }

        // DÜZELTME 1: Helix Toolkit'te Koniler için TruncatedConeVisual3D kullanılır
        public TruncatedConeVisual3D CoverageCone { get; private set; }
        public double Speed { get; set; }


        private double _inclination;
        private double _phaseOffset;
        private Color _originalColor;
        public string Name { get; set; }
        public double CurrentLat { get; set; }
        public double CurrentLon { get; set; }
        public double CurrentAlt { get; set; }
        public double Speed { get; set; }
        public bool IsVisible { get; set; } = true;

        // Reference to the actual SGP4 propagator
        public Satellite Propagator { get; private set; }

        // Orijinal rengi hafızada tutmak için


        public SatelliteMarker(string name, Satellite propagator, double startLat, double startLon, double altitude, Color color, double speed = 0.5)
        {
            Name = name;
            Propagator = propagator;
            CurrentLat = startLat;
            CurrentLon = startLon;
            CurrentAlt = altitude;
            Speed = speed;
            _originalColor = color; // Rengi kaydet
            _inclination = startLat;

            Visual = new SphereVisual3D();
            Visual.Radius = 150;
            Visual.Fill = new SolidColorBrush(color);

            Random rnd = new Random(name.GetHashCode());
            _phaseOffset = rnd.NextDouble() * 360.0;

            int baseTrailLimit = altitude > 10000 ? 500 : 120;

            Trail = new OrbitTrailRenderer(color, baseTrailLimit);

            // --- DÜZELTME 1 DEVAMI: KONİYİ OLUŞTURMA ---
            CoverageCone = new TruncatedConeVisual3D();
            CoverageCone.TopRadius = 0; // Tepe noktasını sıfırla ki ucu sivri tam bir koni olsun!

            var coneColor = Color.FromArgb(35, 0, 255, 255);
            CoverageCone.Fill = new SolidColorBrush(coneColor);

            UpdatePosition(CurrentLat, CurrentLon, CurrentAlt);
        }

        // --- DÜZELTME 2: UNUTTUĞUMUZ VURGULAMA METOTLARI ---
        public void Highlight()
        {
            Visual.Fill = new SolidColorBrush(Colors.Cyan);
            Visual.Radius = 250;
        }

        public void RemoveHighlight()
        {
            Visual.Fill = new SolidColorBrush(_originalColor);
            Visual.Radius = 150;
        }

        public void UpdatePositionByTime(DateTime time, OrbitalTracker.Services.Sgp4Calculator calculator)
        {
            var pos = calculator.Calculate(Propagator, Name, time);
            if (pos != null)
            {
                CurrentLat = pos.Latitude;
                CurrentLon = pos.Longitude;
                CurrentAlt = pos.AltitudeKm;
                Speed = pos.SpeedKmS;

                UpdatePosition(CurrentLat, CurrentLon, CurrentAlt);
            }
        }

        public void UpdatePosition(double latitude, double longitude, double altitude)
        {
            var coords = CoordinateConverter.ToCartesian(latitude, longitude, altitude);
            Point3D satPos = new Point3D(coords.X, coords.Y, coords.Z);

            Visual.Center = satPos;
            Trail.AddPoint(satPos);

            if (CoverageCone != null)
            {
                var surfaceCoords = CoordinateConverter.ToCartesian(latitude, longitude, 0);
                Point3D basePos = new Point3D(surfaceCoords.X, surfaceCoords.Y, surfaceCoords.Z);

                // Koninin taban merkez noktası Dünya yüzeyi
                CoverageCone.Origin = basePos;

                // Koninin baktığı yön (Dünya yüzeyinden uyduya doğru bir vektör)
                CoverageCone.Normal = new Vector3D(satPos.X - basePos.X, satPos.Y - basePos.Y, satPos.Z - basePos.Z);
                CoverageCone.Height = altitude;
                CoverageCone.BaseRadius = altitude * 1.5;
            }
        }
    }
}