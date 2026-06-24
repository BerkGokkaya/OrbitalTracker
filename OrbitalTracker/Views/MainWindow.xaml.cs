using HelixToolkit.Wpf;
using OrbitalTracker.Helpers;
using System;
using System.Collections.Generic;
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
        // Artık tek bir uydu değil, bir liste yönetiyoruz
        private List<SatelliteMarker> _satellites = new List<SatelliteMarker>();
        private DispatcherTimer _timer;
        private bool _isPanelOpen = false;

        public MainWindow()
        {
            InitializeComponent();
            SetupSatellites();
        }

        private void SetupSatellites()
        {
            // 1. ISS (Alçak Yörünge, Hızlı)
            var iss = new SatelliteMarker("ISS (ZARYA)", 40.15, 26.40, 420.0, Colors.Red, 2.0);

            // 2. Hubble Teleskobu (Farklı başlangıç noktası, orta hız)
            var hubble = new SatelliteMarker("HUBBLE", 28.5, -80.5, 540.0, Colors.Blue, 1.5);

            // 3. Türksat 4A (Çok yüksek, yavaş - GEO)
            var turksat = new SatelliteMarker("TURKSAT 4A", 0.0, 42.0, 35786.0, Colors.Gold, 0.2);

            _satellites.Add(iss);
            _satellites.Add(hubble);
            _satellites.Add(turksat);

            // Tüm uyduları ve kuyruklarını sahneye ekle
            foreach (var sat in _satellites)
            {
                MainViewport.Children.Add(sat.Visual);
                MainViewport.Children.Add(sat.Trail.Visual);
            }

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += Timer_Tick;
            _timer.Start();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Tek bir döngüyle tüm filoyu hareket ettir
            foreach (var sat in _satellites)
            {
                sat.MoveForward();
            }
        }

        private void MainViewport_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Point mousePos = e.GetPosition(MainViewport);
            var hits = MainViewport.Viewport.FindHits(mousePos);

            // 3D uzayda tıklanan objeyi al
            var clickedVisual = hits?.FirstOrDefault()?.Visual;

            // Filomuzun içinde bu görsele sahip olan uyduyu bul
            var clickedSatellite = _satellites.FirstOrDefault(s => s.Visual == clickedVisual);

            if (clickedSatellite != null)
            {
                // YENİ: Panelin içeriğini tıklanan uydunun verileriyle güncelle!
                SatelliteDetailPanel.UpdateDetails(
                    clickedSatellite.Name,
                    clickedSatellite.CurrentAlt,
                    clickedSatellite.Speed,
                    clickedSatellite.CurrentLat,
                    clickedSatellite.CurrentLon
                );

                if (!_isPanelOpen)
                {
                    Storyboard slideIn = (Storyboard)FindResource("SlideInAnimation");
                    slideIn.Begin(this);
                    _isPanelOpen = true;
                }
            }
        }
    }
}