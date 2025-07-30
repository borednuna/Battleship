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
    /// Interaction logic for StrategyView.xaml
    /// </summary>
    public partial class StrategyView : Page
    {
        private GameController _gameController = GameController.GetInstance();
        public StrategyView()
        {
            InitializeComponent();
            InitializeBattleGrid();
        }
        private void InitializeBattleGrid()
        {
            StrategyPanelTitle.Text = $"{_gameController.GetCurrentPlayer().GetName()}'s Strategy";

            for (int row = 0; row < GameController.BOARD_WIDTH; row++)
            {
                for (int col = 0; col < GameController.BOARD_HEIGHT; col++)
                {
                    // For Tracking Grid
                    var button = new Button
                    {
                        Content = $"{(char)('A' + row)}{col + 1}",
                        Margin = new Thickness(1)
                    };

                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    OwnGrid.Children.Add(button);

                    //Add logic placement ships ...
                }
            }
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            _gameController.SwitchTurn();

            if (_gameController.GetCurrentGameState() == GameStates.PLAYING)
            {
                NavigationService?.Navigate(new Uri("Views/BattleView.xaml", UriKind.Relative));
            }
            else
            {
                NavigationService?.Navigate(new StrategyView());
            }
        }
    }
}