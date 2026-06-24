using System;

namespace OrbitalTracker.Helpers
{
    public static class CoordinateConverter
    {
        private const double EarthRadiusKm = 6371.0;

        // Offset değerini burada denemeye devam edebilirsin
        private const double MapOffset = -180.0;

        public static (double X, double Y, double Z) ToCartesian(
            double latitudeDeg,
            double longitudeDeg,
            double altitudeKm)
        {
            var lat = latitudeDeg * Math.PI / 180.0;

            // DÜZELTME BURADA: longitudeDeg'in başına EKSİ (-) koyarak ayna efektini kırıyoruz!
            var lon = (-longitudeDeg + MapOffset) * Math.PI / 180.0;

            var radius = EarthRadiusKm + altitudeKm;

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

            var longitudeAngle = Math.Atan2(x, z) * 180.0 / Math.PI;

            // DÜZELTME BURADA: Tersine çevirdiğimiz matematiği geri topluyoruz
            var longitude = -(longitudeAngle - MapOffset);

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