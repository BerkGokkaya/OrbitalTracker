using System;

namespace OrbitalTracker.Helpers
{
    public static class CoordinateConverter
    {
        private const double EarthRadiusKm = 6371.0;

        public static (double X, double Y, double Z) ToCartesian(
            double latitudeDeg,
            double longitudeDeg,
            double altitudeKm)
        {
            var lat = latitudeDeg * Math.PI / 180.0;

            // --- BOYLAM İNCE AYARI ---
            // Noktayı sağa/sola kaydırmak için sadece bu değeri değiştir.
            // Önce 0 yapıp nerede olduğuna bak. Eğer hala okyanustaysa;
            // 90, -90, 180, -180 veya tam Çanakkale'ye oturana kadar ara bir değer (örn: 70, -110) gir.
            double longitudeOffset = 180.0;

            var lon = (longitudeDeg + longitudeOffset) * Math.PI / 180.0;

            var radius = EarthRadiusKm + altitudeKm;

            // En standart WPF 3D küresel koordinat formülü
            var x = radius * Math.Cos(lat) * Math.Sin(lon);
            var y = radius * Math.Sin(lat);
            var z = radius * Math.Cos(lat) * Math.Cos(lon);

            return (x, y, z);
        }

        public static (double Latitude, double Longitude, double Altitude) ToGeodetic(
            double x, double y, double z)
        {
            var radius = Math.Sqrt(x * x + y * y + z * z);
            var altitude = radius - EarthRadiusKm;

            var latitude = Math.Asin(y / radius) * 180.0 / Math.PI;

            // ToCartesian'daki offset değerinin aynısını buraya yazıyoruz
            double longitudeOffset = 180.0;
            var longitude = (Math.Atan2(x, z) * 180.0 / Math.PI) - longitudeOffset;

            // Boylamı -180 ile 180 arasına çek (Normalize)
            while (longitude <= -180) longitude += 360;
            while (longitude > 180) longitude -= 360;

            return (latitude, longitude, altitude);
        }

        public static (double X, double Y, double Z) ToNormalized(
            double latitudeDeg,
            double longitudeDeg,
            double altitudeKm,
            double scale = 1.0)
        {
            var (x, y, z) = ToCartesian(latitudeDeg, longitudeDeg, altitudeKm);
            var radius = EarthRadiusKm + altitudeKm;

            return (
                x / radius * scale,
                y / radius * scale,
                z / radius * scale
            );
        }
    }
}