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
        private bool _isShipVertical = true;

        public StrategyView(GameController gameController)
        {
            _gameController = gameController;
            InitializeComponent();
            InitializePlayerData();
            InitializeBattleGrid();
        }

        private void InitializePlayerData()
        {
            _fleet = _gameController.GetPlayerFleet(_gameController.GetCurrentPlayerIndex());
            _ownBoard = _gameController.GetCurrentPlayerBoard()[GameController.OWN_BOARD_INDEX];
        }

        private void InitializeBattleGrid()
        {
            StrategyPanelTitle.Text = $"{_gameController.GetCurrentPlayer().GetName()}'s Strategy";
            OwnGrid.Rows = GameController.BOARD_HEIGHT;
            OwnGrid.Columns = GameController.BOARD_WIDTH;

            if (_gameController.GetIsPlayingWithBot())
            {
                AutoPlaceBotFleet();
            }

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
            int currentPlayerIndex = _gameController.GetCurrentPlayerIndex();
            int enemyPlayerIndex = _gameController.GetCurrentEnemyIndex();

            for (int row = 0; row < GameController.BOARD_WIDTH; row++)
            {
                for (int col = 0; col < GameController.BOARD_HEIGHT; col++)
                {
                    Coordinate coordinate = new();
                    coordinate.SetX(col);
                    coordinate.SetY(row);
                    Button? targetButton = OwnGrid.Children
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
                    Button? targetButton;
                    if (_isShipVertical)
                    {
                        int targetRow = buttonRow + i;
                        targetButton = OwnGrid.Children
                            .Cast<UIElement>()
                            .OfType<Button>()
                            .FirstOrDefault(b => Grid.GetRow(b) == targetRow && Grid.GetColumn(b) == buttonCol);
                    }
                    else
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
                MessageBox.Show(ErrorMessage.SHIP_NOT_SELECTED_ERROR);
                return;
            }

            Button button = (Button)sender;
            int shipSize = Ship.GetShipSize(_selectedShipType.Value);
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);
            List<Coordinate> occupyCoordinate = [];

            for (int i = 0; i < shipSize; i++)
            {
                if (_isShipVertical)
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

            string? errorMessage = _gameController.PlaceShipValidate(_selectedShipType, occupyCoordinate);
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage);
                return;
            }
            bool placeShipSuccess = _gameController.PlaceShip((ShipType)_selectedShipType, occupyCoordinate);

            RepaintBoard();
            foreach (Button selectedShipOption in ShipSelectionPanel.Children)
            {
                if (selectedShipOption.Name == _selectedShipType.ToString())
                {
                    selectedShipOption.IsEnabled = false;
                }
            }
            _selectedShipType = null;
        }

        private void AutoPlaceBotFleet()
        {
            IPlayer? botPlayer = _gameController.GetBotPlayer();
            if (botPlayer == null)
            {
                Debug.WriteLine(ErrorMessage.NO_BOT_FOUND_ERROR);
                return;
            }

            List<IShip> botFleet = _gameController.GetPlayerFleet(botPlayer);
            IBoard botBoard = _gameController.GetPlayerBoards(botPlayer)[GameController.OWN_BOARD_INDEX];

            Random random = new();
            foreach (IShip ship in botFleet)
            {
                bool isPlaced = false;
                while (!isPlaced)
                {
                    List<Coordinate> positions = [];
                    int startX = random.Next(0, GameController.BOARD_WIDTH);
                    int startY = random.Next(0, GameController.BOARD_HEIGHT);
                    bool isVertical = random.Next(0, 2) == 0;

                    for (int i = 0; i < ship.GetSize(); i++)
                    {
                        int x = isVertical ? startX : startX + i;
                        int y = isVertical ? startY + i : startY;
                        Coordinate position = new();
                        position.SetX(x);
                        position.SetY(y);
                        positions.Add(position);
                    }

                    string? errorMessage = _gameController.PlaceShipValidateBot(ship.GetType(), positions);
                    if (errorMessage == null)
                    {
                        _gameController.PlaceShipBot(ship.GetType(), positions);
                        isPlaced = true;
                    }
                }
            }
        }

        private void SwitchOrientation_Click(object sender, RoutedEventArgs e)
        {
            _isShipVertical = !_isShipVertical;
        }

        private void Continue_Click(object sender, RoutedEventArgs e)
        {
            _gameController.SwitchTurn();

            if (_gameController.GetCurrentGameState() == GameStates.PLAYING || _gameController.GetIsPlayingWithBot())
            {
                NavigationService?.Navigate(new BattleView(_gameController));
            }
            else
            {
                NavigationService?.Navigate(new StrategyView(_gameController));
            }
        }
    }
}
