using System.Windows.Controls;

namespace OrbitalTracker.Views
{
    public partial class SatelliteDetailView : UserControl
    {
        public SatelliteDetailView()
        {
            InitializeComponent();
        }

        // MainWindow'dan çağrılacak veri aktarım metodu
        public void UpdateDetails(string name, double altitude, double speed, double lat, double lon)
        {
            SatNameText.Text = name;

            // F1 ve F2 string formatlayıcıları, ondalıklı sayıların virgülden sonra 
            // sadece 1 veya 2 hanesini göstermesini sağlar (örnek: 420.5 km)
            SatAltText.Text = $"{altitude:F1} km";
            SatSpeedText.Text = $"{speed:F1} (Birim Hız)";
            SatLatText.Text = $"{lat:F2}°";
            SatLonText.Text = $"{lon:F2}°";
        }
    }
}