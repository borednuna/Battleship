using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using Battleship.Interfaces;

namespace Battleship.Views
{
    /// <summary>
    /// Interaction logic for StrategyView.xaml
    /// </summary>
    public partial class StrategyView : Page
    {
        private GameController _gameController = GameController.GetInstance();
        private ShipType? _selectedShipType;

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
                    var button = new Button
                    {
                        Margin = new Thickness(1)
                    };

                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    OwnGrid.Children.Add(button);
                }
            }

            List<IShip> fleet = _gameController.GetCurrentPlayerFleet();
            foreach (IShip ship in fleet)
            {
                var button = new Button
                {
                    Margin = new Thickness(1),
                    Height = 100,
                    Width = 150,
                    Name = ship.GetType().ToString(),
                    Background = Brushes.LightGray,
                    Content = ship.GetType().ToString(),
                };

                button.Click += PickShip_Click;
                button.ClickMode = ClickMode.Press;

                ShipSelectionPanel.Children.Add(button);
            }
        }

        private void PickShip_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;

            foreach (Button child in ShipSelectionPanel.Children)
            {
                child.Background = Brushes.LightGray;
            }

            if (Enum.TryParse<ShipType>(button.Name, out var shipType))
            {
                _selectedShipType = shipType;
                button.Background = Brushes.LightBlue;
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