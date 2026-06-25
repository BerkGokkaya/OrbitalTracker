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
        public OrbitalPosition? Calculate(TleData tle, DateTime time)
        {
            try
            {
                var tleObject = new Tle(tle.Name, tle.Line1, tle.Line2);
                var satellite = new Satellite(tleObject);
                return Calculate(satellite, tle.Name, time);
            }
            catch
            {
                return null;
            }
        }

        public OrbitalPosition? Calculate(Satellite satellite, string name, DateTime time)
        {
            try
            {
                var position = satellite.Predict(time);
                var geo = position.ToGeodetic();

                return new OrbitalPosition
                {
                    SatelliteName = name,
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
            catch
            {
                return null;
            }
        }
    }
}