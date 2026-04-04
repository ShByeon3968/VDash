using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;

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

        public MainWindowViewModel()
        {
            // X axis Initialize
            for (int i = 0; i < DataX.Length; i++)
            {
                DataX[i] = i;
            }
        }

        public void UpdateDummyData()
        {
            // 배열을 왼쪽으로 쉬프트 (가장 오래된 데이터 버림)
            Array.Copy(AltitudeY, 1, AltitudeY, 0, AltitudeY.Length - 1);

            // 새로운 가짜 고도 데이터 생성 (사인파 + 노이즈)
            double newValue = Math.Sin(_currentIndex * 0.1) * 50 + 1000 + (new Random().NextDouble() * 5);
            AltitudeY[AltitudeY.Length - 1] = newValue;

            _currentIndex++;

            // View에 차트를 다시 그리라고 알림
            DataUpdated?.Invoke();
        }
    }
}
