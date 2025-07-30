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
using Battleship.Classes;
using Battleship.Enums;

namespace Battleship.Views
{
    /// <summary>
    /// Interaction logic for RegisterView.xaml
    /// </summary>
    public partial class RegisterView : Page
    {
        private GameController _gameController = GameController.GetInstance();
        public RegisterView()
        {
            InitializeComponent();
        }

        private void RegisterPlayers_Click(object sender, RoutedEventArgs e)
        {
            string player1Name = Player1Textbox.Text.Trim();
            string player2Name = Player2Textbox.Text.Trim();

            _gameController.SetPlayerNames(player1Name, player2Name);
            NavigationService?.Navigate(new Uri("/Views/StrategyView.xaml", UriKind.RelativeOrAbsolute));
        }
    }
}
