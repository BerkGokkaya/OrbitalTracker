using System;

namespace OrbitalTracker.Helpers
{
    public static class CoordinateConverter
    {
        // Dünya'nın yarıçapı (km)
        private const double EarthRadiusKm = 6371.0;

        /// <summary>
        /// Enlem, boylam ve yüksekliği 3D XYZ koordinatına dönüştürür.
        /// HelixToolkit bu koordinatları kullanarak uyduyu küre üzerine çizer.
        /// </summary>
        public static (double X, double Y, double Z) ToCartesian(
            double latitudeDeg,
            double longitudeDeg,
            double altitudeKm)
        {
            // Dereceyi radyana çevir
            var lat = latitudeDeg * Math.PI / 180.0;
            var lon = longitudeDeg * Math.PI / 180.0;

            // Dünya merkezi + yükseklik
            var radius = EarthRadiusKm + altitudeKm;

            var x = radius * Math.Cos(lat) * Math.Cos(lon);
            var y = radius * Math.Cos(lat) * Math.Sin(lon);
            var z = radius * Math.Sin(lat);

            return (x, y, z);
        }

        /// <summary>
        /// 3D XYZ koordinatını enlem/boylama geri dönüştürür.
        /// </summary>
        public static (double Latitude, double Longitude, double Altitude) ToGeodetic(
            double x, double y, double z)
        {
            var radius = Math.Sqrt(x * x + y * y + z * z);
            var altitude = radius - EarthRadiusKm;
            var latitude = Math.Asin(z / radius) * 180.0 / Math.PI;
            var longitude = Math.Atan2(y, x) * 180.0 / Math.PI;

            return (latitude, longitude, altitude);
        }

        /// <summary>
        /// HelixToolkit için koordinatları normalize eder (birim küre).
        /// Arkadaşın kullanacağı metod.
        /// </summary>
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