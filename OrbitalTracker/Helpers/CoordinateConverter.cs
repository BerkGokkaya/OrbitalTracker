using System;
using System.Windows.Media.Media3D;

namespace OrbitalTracker.Helpers
{
    public static class CoordinateConverter
    {
        private const double EarthRadius = 6371.0;

        public static Point3D ToCartesian(double latitude, double longitude, double altitude)
        {
            double latRad = (Math.PI / 180) * latitude;

            // Kaplamanın başlangıç noktasını telafi etmek için boylamı 90 derece kaydırıyoruz
            // (Eğer bu sefer de Çin'e veya Amerika'ya düşerse burayı +90, -180 veya +180 yapıp tam noktayı bulabilirsin)
            double longitudeOffset = longitude - 90.0;
            double lonRad = (Math.PI / 180) * longitudeOffset;

            double totalRadius = EarthRadius + altitude;

            // Z ekseninin eksi (-) olması WPF 3D'nin sol-sağ sarım yönüyle eşleşmesini sağlar
            double x = totalRadius * Math.Cos(latRad) * Math.Cos(lonRad);
            double y = totalRadius * Math.Sin(latRad);
            double z = -totalRadius * Math.Cos(latRad) * Math.Sin(lonRad);

            return new Point3D(x, y, z);
        }
    }
}