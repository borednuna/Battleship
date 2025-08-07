using Battleship.Classes;
using Battleship.Enums;
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
using Battleship.Structs;
using System.Diagnostics;

namespace Battleship.Views
{
    /// <summary>
    /// Interaction logic for BattleView.xaml
    /// </summary>
    public partial class BattleView : Page
    {
        private GameController _gameController;

        public BattleView(GameController gameController)
        {
            _gameController = gameController;
            InitializeComponent();
            InitializeBattleGrid();
            InitializeShipPanel();
        }

        private void InitializeBattleGrid()
        {
            OwnGrid.Rows = GameController.BOARD_HEIGHT;
            OwnGrid.Columns = GameController.BOARD_WIDTH;
            TrackingGrid.Rows = GameController.BOARD_HEIGHT;
            TrackingGrid.Columns = GameController.BOARD_WIDTH;

            IBoard? ownBoard = _gameController.GetPlayerBoardByType(_gameController.GetCurrentPlayer(), BoardType.OWN_BOARD);
            if (ownBoard == null) return;

            IBoard? trackingBoard = _gameController.GetPlayerBoardByType(_gameController.GetCurrentPlayer(), BoardType.TRACKING_BOARD);
            if (trackingBoard == null) return;

            EnemyBoard.Text = $"Enemy {_gameController.GetCurrentEnemy().GetName()} Board";
            BattlePanelTitle.Text = $"Battle - {_gameController.GetCurrentPlayer().GetName()}'s Turn";

            Dictionary<Coordinate, IShip> ownBoardShips = ownBoard.GetAllShipCoordinates();
            Dictionary<Coordinate, IShip> trackingBoardShips = trackingBoard.GetAllShipCoordinates();

            for (int row = 0; row < GameController.BOARD_WIDTH; row++)
            {
                for (int col = 0; col < GameController.BOARD_HEIGHT; col++)
                {
                    Coordinate cellPosition = new();
                    cellPosition.SetX(col);
                    cellPosition.SetY(row);

                    bool trackingCellIsHit = trackingBoard.GetBoard(cellPosition).IsHit();
                    bool trackingCellHasShip = trackingBoardShips.ContainsKey(cellPosition);

                    var trackingCellGrid = new Grid();

                    var trackingButton = new Button
                    {
                        Style = (Style)FindResource("BoardGridButton"),
                        FontFamily = new FontFamily("Segoe UI Emoji"),
                        Background = trackingCellIsHit
                            ? trackingCellHasShip ? Brushes.Green : Brushes.Red
                            : Brushes.SkyBlue,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Tag = cellPosition
                    };

                    trackingCellGrid.Children.Add(trackingButton);

                    if (trackingCellHasShip)
                    {
                        string iconPath = $"pack://application:,,,/Assets/{GameController.DEFAULT_SHIP_PATH}";
                        var icon = new Image
                        {
                            Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute)),
                            Width = 32,
                            Height = 32,
                            Stretch = Stretch.Uniform,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            IsHitTestVisible = false
                        };
                        trackingCellGrid.Children.Add(icon);
                    }

                    Grid.SetRow(trackingButton, row);
                    Grid.SetColumn(trackingButton, col);
                    TrackingGrid.Children.Add(trackingButton);
                    trackingButton.Click += TakeTurn_Click;

                    bool cellIsHit = ownBoard.GetBoard(cellPosition).IsHit();
                    bool cellHasShip = ownBoardShips.ContainsKey(cellPosition);
                    IShip? ship = ownBoard.GetBoard(cellPosition).GetShip();

                    var cellGrid = new Grid();

                    var ownBoardButton = new Button
                    {
                        Style = (Style)FindResource("BoardGridButton"),
                        FontFamily = new FontFamily("Segoe UI Emoji"),
                        Background = cellIsHit
                            ? cellHasShip ? Brushes.Green : Brushes.Red
                            : Brushes.SkyBlue,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center,
                        Tag = cellPosition
                    };

                    cellGrid.Children.Add(ownBoardButton);

                    if (cellHasShip && ship != null)
                    {
                        string iconPath = $"pack://application:,,,/Assets/{Ship.GetShipAsset(ship.GetShipType())}";
                        var icon = new Image
                        {
                            Source = new BitmapImage(new Uri(iconPath, UriKind.Absolute)),
                            Width = 32,
                            Height = 32,
                            Stretch = Stretch.Uniform,
                            HorizontalAlignment = HorizontalAlignment.Center,
                            VerticalAlignment = VerticalAlignment.Center,
                            IsHitTestVisible = false
                        };
                        cellGrid.Children.Add(icon);
                    }

                    Grid.SetRow(cellGrid, row);
                    Grid.SetColumn(cellGrid, col);
                    OwnGrid.Children.Add(cellGrid);
                }
            }
        }

        private void InitializeShipPanel()
        {
            List<IShip> currentPlayerShips = _gameController.GetPlayerFleet(_gameController.GetCurrentPlayer());
            List<IShip> enemyShips = _gameController.GetPlayerFleet(_gameController.GetCurrentEnemy());
            YourShips.Text = $"Your ships: {_gameController.RemainingShips()} ships remaining";

            foreach (IShip ship in currentPlayerShips)
            {
                StackPanel shipPanelStack = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FlowDirection = FlowDirection.LeftToRight,
                    Background = ship.GetHits() >= ship.GetSize() ? Brushes.Red : Brushes.LightGreen,
                };
                string path = $"pack://application:,,,/Assets/{Ship.GetShipAsset(ship.GetShipType())}";
                Image icon = new()
                {
                    Source = new BitmapImage(new Uri(path, UriKind.Absolute)),
                    Width = 16,
                    Height = 16,
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                TextBlock shipBlock = new TextBlock
                {
                    Text = $"{ship.GetShipType()} ({ship.GetHits()}/{ship.GetSize()})",
                    Margin = new Thickness(5),
                    FontSize = 8,
                    Foreground = ship.GetHits() >= ship.GetSize() ? Brushes.White : Brushes.Black,
                };
                shipPanelStack.Children.Add(icon);
                shipPanelStack.Children.Add(shipBlock);

                OwnShipsPanel.Children.Add(shipPanelStack);
            }

            foreach (IShip ship in enemyShips)
            {
                StackPanel shipPanelStack = new StackPanel()
                {
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                    FlowDirection = FlowDirection.LeftToRight,
                    Background = ship.GetHits() >= ship.GetSize() ? Brushes.Red : Brushes.LightGreen,
                };
                string path = $"pack://application:,,,/Assets/{Ship.GetShipAsset(ship.GetShipType())}";
                Image icon = new()
                {
                    Source = new BitmapImage(new Uri(path, UriKind.Absolute)),
                    Width = 16,
                    Height = 16,
                    Stretch = Stretch.Uniform,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center,
                };
                TextBlock shipBlock = new TextBlock
                {
                    Text = $"{ship.GetShipType()}",
                    Margin = new Thickness(5),
                    FontSize = 8,
                    Foreground = Brushes.Black,
                };
                shipPanelStack.Children.Add(icon);
                shipPanelStack.Children.Add(shipBlock);

                EnemyShipsPanel.Children.Add(shipPanelStack);
            }
        }

        private void TakeTurn_Click(object sender, RoutedEventArgs e)
        {
            Button targetHitButton = (Button)sender;
            int row = Grid.GetRow(targetHitButton);
            int col = Grid.GetColumn(targetHitButton);

            Coordinate targetCoordinate = new();
            targetCoordinate.SetX(col);
            targetCoordinate.SetY(row);

            string? takeTurnError = _gameController.TakeTurnValidate(targetCoordinate);
            if (takeTurnError != null)
            {
                MessageBox.Show(takeTurnError);
                return;
            }

            _gameController.TakeTurn(targetCoordinate);
            bool cellHasShip = _gameController.HasShip(targetCoordinate);

            if (_gameController.GetCurrentGameState() == GameStates.GAME_OVER)
            {
                NavigationService?.Navigate(new GameOverView(_gameController));
            }
            else
            {
                Thread.Sleep(500);
                if (_gameController.GetIsPlayingWithBot())
                {
                    RepaintGrids();
                }
                else
                {
                    NavigationService?.Navigate(new BattleView(_gameController));
                }
            }
        }

        private void RepaintGrids()
        {
            OwnGrid.Children.Clear();
            TrackingGrid.Children.Clear();
            OwnShipsPanel.Children.Clear();
            EnemyShipsPanel.Children.Clear();

            InitializeBattleGrid();
            InitializeShipPanel();
        }
    }
}
