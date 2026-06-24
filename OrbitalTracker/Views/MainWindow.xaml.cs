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
            // Örnek: Türkiye'nin tam üzerinde, 420 km yükseklikte bir uydu (Örn: Enlem 39, Boylam 35)
            double mockLat = 39.0;
            double mockLon = 35.0;
            double mockAlt = 420.0;

            // Kırmızı renkli bir uydu markörü oluştur
            SatelliteMarker mockSatellite = new SatelliteMarker(mockLat, mockLon, mockAlt, Colors.Red);

            // Uyduyu HelixViewport3D'nin içine (Dünya'nın yanına) ekle
            MainViewport.Children.Add(mockSatellite.Visual);
        }
    }
}