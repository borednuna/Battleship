using Battleship.Classes;
using Battleship.Views;
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

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        MediaPlayer mediaPlayer = new MediaPlayer();

        public MainWindow()
        {
            InitializeComponent();
            
            mediaPlayer.Open(new Uri("Assets/powerup.mp3", UriKind.RelativeOrAbsolute));
            mediaPlayer.Volume = 0.4;
            mediaPlayer.MediaEnded += (s, e) => mediaPlayer.Position = TimeSpan.Zero;
            mediaPlayer.Play();

            GameController gameController = new();
            gameController.Reset();

            var registrationView = new RegisterView(gameController);
            MainFrame.Navigate(registrationView);
        }
    }
}