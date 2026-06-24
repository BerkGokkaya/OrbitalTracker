using OrbitalTracker.Helpers;
using System.Windows;
using System.Windows.Media;

namespace OrbitalTracker.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            AddMockSatellite();
        }

        private void AddMockSatellite()
        {
            // Çanakkale koordinatları! (Bunu değiştirmeyi unutma)
            double mockLat = 40.15;
            double mockLon = 26.40;
            double mockAlt = 420.0;

            SatelliteMarker mockSatellite = new SatelliteMarker(mockLat, mockLon, mockAlt, Colors.Red);
            MainViewport.Children.Add(mockSatellite.Visual);
        }
    }
}