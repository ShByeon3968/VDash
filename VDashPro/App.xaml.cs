using System.Configuration;
using System.Data;
using System.Windows;
using Prism.Ioc;
using Prism.DryIoc;
using VDashPro.Views;

namespace VDashPro
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : PrismApplication
    {
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            
        }
    }

}
