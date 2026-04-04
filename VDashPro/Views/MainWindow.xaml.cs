using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System;
using System.Windows.Threading;
using VDashPro.ViewModels;

namespace VDashPro.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private MainWindowViewModel _viewModel;
        private DispatcherTimer _renderTimer;
        public MainWindow()
        {
            InitializeComponent();
            
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            _viewModel = this.DataContext as MainWindowViewModel;
            if (_viewModel != null)
            {
                WpfPlot1.Plot.Add.Signal(_viewModel.AltitudeY);
                WpfPlot1.Plot.FigureBackground.Color = ScottPlot.Color.FromHex("#2D2D30");
                WpfPlot1.Plot.DataBackground.Color = ScottPlot.Color.FromHex("#1E1E1E");
                WpfPlot1.Plot.Axes.Color(ScottPlot.Color.FromHex("#FFFFFF"));
                WpfPlot1.Plot.Grid.MajorLineColor = ScottPlot.Color.FromHex("#404040");

                // ViewModel Event
                _viewModel.DataUpdated += () =>
                {
                    WpfPlot1.Refresh();
                };

                // 10ms Timer
                _renderTimer = new DispatcherTimer();
                _renderTimer.Interval = TimeSpan.FromMilliseconds(10);
                _renderTimer.Tick += (s, ev) =>
                {
                    _viewModel.UpdateDummyData();
                };
                _renderTimer.Start();
            }
        }
    }
}