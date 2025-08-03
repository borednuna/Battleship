using Battleship.Classes;
using Battleship.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Battleship.Views
{
    /// <summary>
    /// Interaction logic for GameOverView.xaml
    /// </summary>
    public partial class GameOverView : Page
    {
        GameController _gameController = GameController.GetInstance();
        public GameOverView()
        {
            InitializeComponent();
        }

        public void SetWinnerMessage(string message)
        {
            IPlayer winner = _gameController.GetWinner();

            WinnerField.Text = $"{winner.GetName()} won the game!";
        }

        public void RestartGame_Click(object sender, RoutedEventArgs e)
        {
            _gameController.Reset();
            NavigationService?.Navigate(new Uri("/Views/RegisterView.xaml", UriKind.Relative));
        }
    }
}
