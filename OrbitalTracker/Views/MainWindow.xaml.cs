using HelixToolkit.Wpf;
using OrbitalTracker.Helpers;
using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace OrbitalTracker.Views
{
    public partial class MainWindow : Window
    {
        private SatelliteMarker _mockSatellite;
        private double _currentLongitude = 26.40;
        private DispatcherTimer _timer;
        private bool _isPanelOpen = false;

        public MainWindow()
        {
            InitializeComponent();
            SetupMockSatellite();
        }

        private void SetupMockSatellite()
        {
            _mockSatellite = new SatelliteMarker(40.15, _currentLongitude, 420.0, Colors.Red);

            MainViewport.Children.Add(_mockSatellite.Visual);
            MainViewport.Children.Add(_mockSatellite.Trail.Visual);

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(50);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _currentLongitude += 0.5;
            _mockSatellite.UpdatePosition(40.15, _currentLongitude, 420.0);
        }

        // 3D Sahneye tıklandığında çalışacak metot
        private void MainViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            // Tıklanan piksel koordinatını al
            Point mousePos = e.GetPosition(MainViewport);

            // Fare hizasındaki 3B nesneleri tara
            var hits = MainViewport.Viewport.FindHits(mousePos);

            // Eğer bir nesneye tıklandıysa ve bu nesne bizim uydumuzun görseliyse
            if (hits != null && hits.Any(h => h.Visual == _mockSatellite.Visual))
            {
                if (!_isPanelOpen)
                {
                    // Paneli açma animasyonunu başlat
                    Storyboard slideIn = (Storyboard)FindResource("SlideInAnimation");
                    slideIn.Begin(this);
                    _isPanelOpen = true;
                }
            }
            else
            {
                // Uzay boşluğuna veya Dünya'ya tıklandıysa paneli geri kapat
                if (_isPanelOpen)
                {
                    Storyboard slideOut = (Storyboard)FindResource("SlideOutAnimation");
                    slideOut.Begin(this);
                    _isPanelOpen = false;
                }
            }
        }
    }
}