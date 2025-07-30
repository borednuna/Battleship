using System.Configuration;
using System.Data;
using System.Windows;
using Battleship.Classes;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {   protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            GameController gameController = GameController.GetInstance();
            gameController.Reset();

            MainWindow mainWindow = new();
            mainWindow.Show();
        }
    }
}
