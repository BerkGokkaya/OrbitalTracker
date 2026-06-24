using System;
using OrbitalTracker.Models;
using SGPdotNET.CoordinateSystem;
using SGPdotNET.TLE;
using SGPdotNET.Propagation;
using SGPdotNET.Observation;

namespace OrbitalTracker.Services
{
    public class Sgp4Calculator
    {
        public OrbitalPosition Calculate(TleData tle, DateTime time)
        {
            try
            {
                var tleObject = new Tle(tle.Name, tle.Line1, tle.Line2);
                var satellite = new Satellite(tleObject);
                var position = satellite.Predict(time);
                var geo = position.ToGeodetic();

                return new OrbitalPosition
                {
                    SatelliteName = tle.Name,
                    Latitude = geo.Latitude.Degrees,
                    Longitude = geo.Longitude.Degrees,
                    AltitudeKm = geo.Altitude,
                    SpeedKmS = Math.Sqrt(
                        position.Velocity.X * position.Velocity.X +
                        position.Velocity.Y * position.Velocity.Y +
                        position.Velocity.Z * position.Velocity.Z
                    ),
                    Timestamp = time
                };
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Hesaplama hatası ({tle.Name}): {ex.Message}",
                    "Hata", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return null;
            }
        }
    }
}