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
        private bool _isShipVertical = false;

        public StrategyView(GameController gameController)
        {
            _gameController = gameController;
            InitializeComponent();
            InitializeBattleGrid();
        }

        private void InitializeBattleGrid()
        {
            StrategyPanelTitle.Text = $"{_gameController.GetCurrentPlayer().GetName()}'s Strategy";
            OwnGrid.Rows = GameController.BOARD_HEIGHT;
            OwnGrid.Columns = GameController.BOARD_WIDTH;

            if (_gameController.GetIsPlayingWithBot())
            {
                _ = AutoPlaceBotFleetAsync();
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

            PaintShipPanel();
        }

        private void PaintShipPanel()
        {
            List<IShip> currentPlayerFleet = _gameController.GetPlayerFleet(_gameController.GetCurrentPlayer());

            foreach (IShip ship in currentPlayerFleet)
            {
                bool isPlaced = ship.GetIsPlaced();
                Debug.WriteLine($"Ship {ship.GetShipType()} is {isPlaced}");
                var cellButton = new Button
                {
                    Margin = new Thickness(1),
                    Height = 100,
                    Width = 150,
                    Name = ship.GetShipType().ToString(),
                    Background = Brushes.LightGray,
                    IsEnabled = !isPlaced,
                    Content = $"{ship.GetShipType().ToString()} ({ship.GetSize().ToString()})",
                };

                cellButton.Click += PickShip_Click;
                cellButton.ClickMode = ClickMode.Press;

                ShipSelectionPanel.Children.Add(cellButton);
            }
        }

        private void PickShip_Click(object sender, RoutedEventArgs e)
        {
            Button cellButton = (Button)sender;

            foreach (Button child in ShipSelectionPanel.Children)
            {
                child.Background = Brushes.LightGray;
            }

            if (Enum.TryParse<ShipType>(cellButton.Name, out var shipType))
            {
                _selectedShipType = shipType;
                cellButton.Background = Brushes.LightBlue;
            }
        }

        private void RepaintBoard()
        {
            IBoard? currentPlayerBoard = _gameController.GetPlayerBoardByType(_gameController.GetCurrentPlayer(), BoardType.OWN_BOARD);
            if (currentPlayerBoard == null)
            {
                return;
            }

            for (int row = 0; row < GameController.BOARD_WIDTH; row++)
            {
                for (int col = 0; col < GameController.BOARD_HEIGHT; col++)
                {
                    Coordinate cellCoordinate = new();
                    cellCoordinate.SetX(col);
                    cellCoordinate.SetY(row);
                    Button? cellButton = OwnGrid.Children
                            .Cast<UIElement>()
                            .OfType<Button>()
                            .FirstOrDefault(b => Grid.GetRow(b) == row && Grid.GetColumn(b) == col);

                    if (cellButton != null)
                    {
                        if (currentPlayerBoard.GetBoard(cellCoordinate).GetShip() != null)
                        {
                            cellButton.Background = Brushes.DarkSeaGreen;
                        }
                        else
                        {
                            cellButton.Background = Brushes.LightGray;
                        }
                    }
                }
            }
        }

        private void PreviewShip_MouseEnter(object sender, MouseEventArgs e)
        {
            if (_selectedShipType != null)
            {
                ShipType shipType = (ShipType)_selectedShipType;
                IShip? selectedShip = _gameController.GetShipByType(shipType, _gameController.GetCurrentPlayer());

                if (selectedShip == null)
                {
                    MessageBox.Show(ErrorMessage.SHIP_NOT_FOUND_ERROR);
                    return;
                }

                int shipSize = selectedShip.GetSize();
                RepaintBoard();

                Button shipHeadButton = (Button)sender;
                shipHeadButton.Background = Brushes.LightSalmon;

                int shipHeadButtonRow = Grid.GetRow(shipHeadButton);
                int shipHeadButtonColumn = Grid.GetColumn(shipHeadButton);
                Coordinate cellPosition = new();
                cellPosition.SetX(shipHeadButtonColumn);
                cellPosition.SetY(shipHeadButtonRow);

                for (int i = 0; i < shipSize; i++)
                {
                    Button? shipBodyButton;
                    if (_isShipVertical)
                    {
                        int shipBodyRow = shipHeadButtonRow + i;
                        shipBodyButton = OwnGrid.Children
                            .Cast<UIElement>()
                            .OfType<Button>()
                            .FirstOrDefault(b => Grid.GetRow(b) == shipBodyRow && Grid.GetColumn(b) == shipHeadButtonColumn);
                    }
                    else
                    {
                        int shipBodyColumn = shipHeadButtonColumn + i;
                        shipBodyButton = OwnGrid.Children
                            .Cast<UIElement>()
                            .OfType<Button>()
                            .FirstOrDefault(b => Grid.GetRow(b) == shipHeadButtonRow && Grid.GetColumn(b) == shipBodyColumn);
                    }
                    if (shipBodyButton != null)
                    {
                        shipBodyButton.Background = Brushes.LightSalmon;
                    }
                }
            }
        }

        private void PreviewShip_MouseLeave(object sender, MouseEventArgs e)
        {
            if (_selectedShipType != null)
            {
                Button cellButton = (Button)sender;
                cellButton.Background = Brushes.LightSalmon;

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

            ShipType shipType = (ShipType)_selectedShipType;
            IShip? selectedShip = _gameController.GetShipByType(shipType, _gameController.GetCurrentPlayer());

            if (selectedShip == null)
            {
                MessageBox.Show(ErrorMessage.SHIP_NOT_FOUND_ERROR);
                return;
            }

            int shipSize = selectedShip.GetSize();
            List<Coordinate> occupyCoordinate = [];

            Button shipHeadButton = (Button)sender;
            shipHeadButton.Background = Brushes.LightSalmon;
            int shipHeadButtonRow = Grid.GetRow(shipHeadButton);
            int shipHeadButtonColumn = Grid.GetColumn(shipHeadButton);

            Coordinate cellPosition = new();
            cellPosition.SetX(shipHeadButtonColumn);
            cellPosition.SetY(shipHeadButtonRow);

            for (int i = 0; i < shipSize; i++)
            {
                if (_isShipVertical)
                {
                    int shipBodyRow = shipHeadButtonRow + i;
                    Coordinate shipBodyCoordinate = new();
                    shipBodyCoordinate.SetX(shipHeadButtonColumn);
                    shipBodyCoordinate.SetY(shipBodyRow);
                    occupyCoordinate.Add(shipBodyCoordinate);
                }
                else
                {
                    int shipBodyColumn = shipHeadButtonColumn + i;
                    Coordinate shipBodyCoordinate = new();
                    shipBodyCoordinate.SetX(shipBodyColumn);
                    shipBodyCoordinate.SetY(shipHeadButtonRow);
                    occupyCoordinate.Add(shipBodyCoordinate);
                }
            }

            string? shipPlacementError = _gameController.PlaceShipValidate(_gameController.GetCurrentPlayer(), _selectedShipType, occupyCoordinate);
            if (shipPlacementError != null)
            {
                MessageBox.Show(shipPlacementError);
                return;
            }

            bool _ = _gameController.PlaceShip(_gameController.GetCurrentPlayer(), (ShipType)_selectedShipType, occupyCoordinate);

            RepaintBoard();

            ShipSelectionPanel.Children.Clear();
            PaintShipPanel();

            foreach (Button selectedShipOption in ShipSelectionPanel.Children)
            {
                if (selectedShipOption.Name == _selectedShipType.ToString())
                {
                    selectedShipOption.IsEnabled = false;
                }
            }
            _selectedShipType = null;
        }

        private async Task AutoPlaceBotFleetAsync()
        {
            IPlayer? botPlayer = _gameController.GetBotPlayer();
            if (botPlayer == null)
            {
                Debug.WriteLine(ErrorMessage.NO_BOT_FOUND_ERROR);
                return;
            }

            await Task.Yield();

            List<IShip> botFleet = _gameController.GetPlayerFleet(botPlayer);
            Random random = _gameController.GetRandomInstance();

            foreach (IShip ship in botFleet)
            {
                bool isPlaced = false;
                while (!isPlaced)
                {
                    List<Coordinate> shipCoordinates = [];
                    int shipHeadColumn = random.Next(0, GameController.BOARD_WIDTH);
                    int shipHeadRow = random.Next(0, GameController.BOARD_HEIGHT);
                    bool isVertical = random.Next(0, 2) == 0;

                    for (int i = 0; i < ship.GetSize(); i++)
                    {
                        int shipBodyColumn = isVertical ? shipHeadColumn : shipHeadColumn + i;
                        int shipBodyRow = isVertical ? shipHeadRow + i : shipHeadRow;

                        Coordinate shipCoordinate = new();
                        shipCoordinate.SetX(shipBodyColumn);
                        shipCoordinate.SetY(shipBodyRow);
                        shipCoordinates.Add(shipCoordinate);
                    }

                    string? shipPlacementError = _gameController.PlaceShipValidate(botPlayer, ship.GetShipType(), shipCoordinates);
                    if (shipPlacementError == null)
                    {
                        _gameController.PlaceShip(botPlayer, ship.GetShipType(), shipCoordinates);
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
            if (!_gameController.IsPlayerFleetPlaced(_gameController.GetCurrentPlayer()))
            {
                MessageBox.Show(ErrorMessage.SHIP_NOT_PLACED_ERROR);
                return;
            }

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
