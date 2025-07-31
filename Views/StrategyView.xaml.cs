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
using Battleship.Structs;

namespace Battleship.Views
{
    /// <summary>
    /// Interaction logic for StrategyView.xaml
    /// </summary>
    public partial class StrategyView : Page
    {
        private GameController _gameController;
        private ShipType? _selectedShipType;
        private List<IShip> _fleet;
        private IBoard _ownBoard;
        private bool _isPlacementVertical;

        public StrategyView()
        {
            InitializeComponent();
            InitializePlayerData();
            InitializeBattleGrid();
        }

        private void InitializePlayerData()
        {
            _gameController = GameController.GetInstance();
            _fleet = _gameController.GetCurrentPlayerFleet();
            _ownBoard = _gameController.GetCurrentPlayerBoard()[GameController.OWN_BOARD_INDEX];
            _isPlacementVertical = true;
        }

        private void InitializeBattleGrid()
        {
            StrategyPanelTitle.Text = $"{_gameController.GetCurrentPlayer().GetName()}'s Strategy";
            OwnGrid.Rows = GameController.BOARD_HEIGHT;
            OwnGrid.Columns = GameController.BOARD_WIDTH;

            for (int row = 0; row < GameController.BOARD_WIDTH; row++)
            {
                for (int col = 0; col < GameController.BOARD_HEIGHT; col++)
                {
                    var button = new Button
                    {
                        Margin = new Thickness(1),
                        Background = Brushes.LightGray,
                    };

                    button.MouseEnter += PreviewShip_MouseEnter;
                    button.MouseLeave += PreviewShip_MouseLeave;
                    button.Click += PlaceShip_Click;

                    Grid.SetRow(button, row);
                    Grid.SetColumn(button, col);
                    OwnGrid.Children.Add(button);
                }
            }

            foreach (IShip ship in _fleet)
            {
                var button = new Button
                {
                    Margin = new Thickness(1),
                    Height = 100,
                    Width = 150,
                    Name = ship.GetType().ToString(),
                    Background = Brushes.LightGray,
                    Content = $"{ship.GetType().ToString()} ({ship.GetSize().ToString()})",
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

        private void RepaintBoard()
        {
            for (int row = 0; row < GameController.BOARD_WIDTH; row++)
            {
                for (int col = 0; col < GameController.BOARD_HEIGHT; col++)
                {
                    Coordinate coordinate = new();
                    coordinate.SetX(col);
                    coordinate.SetY(row);
                    Button targetButton = OwnGrid.Children
                            .Cast<UIElement>()
                            .OfType<Button>()
                            .FirstOrDefault(b => Grid.GetRow(b) == row && Grid.GetColumn(b) == col);

                    if (targetButton != null)
                    {
                        if (_ownBoard.GetBoard(coordinate).GetShip() != null)
                        {
                            targetButton.Background = Brushes.DarkSeaGreen;
                        }
                        else
                        {
                            targetButton.Background = Brushes.LightGray;
                        }
                    }
                }
            }
        }

        private void PreviewShip_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_selectedShipType != null)
            {
                Button button = (Button)sender;
                button.Background = Brushes.LightSalmon;

                int buttonRow = Grid.GetRow(button);
                int buttonCol = Grid.GetColumn(button);

                int shipSize = Ship.GetShipSize(_selectedShipType.Value);

                RepaintBoard();
                for (int i = 0; i < shipSize; i++)
                {
                    Button targetButton;
                    if (_isPlacementVertical)
                    {
                        int targetRow = buttonRow + i;
                        targetButton = OwnGrid.Children
                            .Cast<UIElement>()
                            .OfType<Button>()
                            .FirstOrDefault(b => Grid.GetRow(b) == targetRow && Grid.GetColumn(b) == buttonCol);
                    } else
                    {
                        int targetCol = buttonCol + i;
                        targetButton = OwnGrid.Children
                            .Cast<UIElement>()
                            .OfType<Button>()
                            .FirstOrDefault(b => Grid.GetRow(b) == buttonRow && Grid.GetColumn(b) == targetCol);
                    }
                    if (targetButton != null)
                    {
                        targetButton.Background = Brushes.LightSalmon;
                    }
                }
            }
        }

        private void PreviewShip_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_selectedShipType != null)
            {
                Button button = (Button)sender;
                button.Background = Brushes.LightSalmon;

                RepaintBoard();
            }
        }

        private void PlaceShip_Click(object sender, RoutedEventArgs e)
        {
            if (_selectedShipType == null)
            {
                MessageBox.Show("Please select a ship type first.");
                return;
            }

            Button button = (Button)sender;
            int shipSize = Ship.GetShipSize(_selectedShipType.Value);
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);
            List<Coordinate> occupyCoordinate = [];

            if (row + shipSize > GameController.BOARD_HEIGHT && _isPlacementVertical
                || col + shipSize > GameController.BOARD_WIDTH && !_isPlacementVertical)
            {
                MessageBox.Show("Ship cannot be placed outside the grid.");
                return;
            }

            for (int i = 0; i < shipSize; i++)
            {
                if (_isPlacementVertical)
                {
                    int targetRow = row + i;
                    Coordinate coordinate = new();
                    coordinate.SetX(col);
                    coordinate.SetY(targetRow);
                    occupyCoordinate.Add(coordinate);
                }
                else
                {
                    int targetCol = col + i;
                    Coordinate coordinate = new();
                    coordinate.SetX(targetCol);
                    coordinate.SetY(row);
                    occupyCoordinate.Add(coordinate);
                }
            }

            if (_gameController.PlaceShip((ShipType)_selectedShipType, occupyCoordinate))
            {
                RepaintBoard();

                foreach (Button child in ShipSelectionPanel.Children)
                {
                    if (child.Name == _selectedShipType.ToString())
                    {
                        child.IsEnabled = false;
                    }
                }

                _selectedShipType = null;
            } else
            {
                MessageBox.Show("Ship cannot be placed here. Please try again.");
            }
        }

        private void SwitchOrientation_Click(object sender, RoutedEventArgs e)
        {
            _isPlacementVertical = !_isPlacementVertical;
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