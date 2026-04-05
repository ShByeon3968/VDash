using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using VDashPro.Core;

namespace VDashPro.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "V-Dash Pro";
        public string Title
        {
            get { return _title; }
            set => SetProperty(ref _title, value);
        }

        // Dummy Data Array
        public double[] DataX { get; private set; } = new double[1000];
        public double[] AltitudeY { get; private set; } = new double[1000];

        private int _currentIndex = 0;
        public event Action DataUpdated; // Render Event

        public MainWindowViewModel(INetworkService networkService)
        {
            networkService.TelemetryReceived += (packet) =>
            {
                UpdateRealData(packet.Altitude);
            };

            networkService.StartUdpListener(12345);
        }

        public void UpdateRealData(float altitude)
        {
            Array.Copy(AltitudeY, 1, AltitudeY, 0, AltitudeY.Length-1);
            AltitudeY[AltitudeY.Length - 1] = altitude;
            DataUpdated?.Invoke();
        }
    }
}
