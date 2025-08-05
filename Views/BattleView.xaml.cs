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
        private IBoard _ownBoard;
        private IBoard _trackingBoard;
        private Dictionary<Coordinate, Ship> _shipsOnOwnBoard;

        public BattleView(GameController gameController)
        {
            _gameController = gameController;
            InitializeComponent();
            InitializePlayerData();
            InitializeBattleGrid();
            InitializeShipPanel();
        }

        private void InitializePlayerData()
        {
            var currentPlayer = _gameController.GetCurrentPlayer();
            _ownBoard = _gameController.GetCurrentPlayerBoard()[GameController.OWN_BOARD_INDEX];
            _shipsOnOwnBoard = _ownBoard.GetShipsOnBoard();
            _trackingBoard = _gameController.GetCurrentPlayerBoard()[GameController.TRACKING_BOARD_INDEX];

            BattlePanelTitle.Text = $"{_gameController.GetCurrentPlayer().GetName()}'s Turn";
        }

        private void InitializeBattleGrid()
        {
            OwnGrid.Rows = GameController.BOARD_HEIGHT;
            OwnGrid.Columns = GameController.BOARD_WIDTH;
            TrackingGrid.Rows = GameController.BOARD_HEIGHT;
            TrackingGrid.Columns = GameController.BOARD_WIDTH;

            EnemyBoard.Text = $"Enemy {_gameController.GetCurrentEnemy().GetName()} Board";

            for (int row = 0; row < GameController.BOARD_WIDTH; row++)
            {
                for (int col = 0; col < GameController.BOARD_HEIGHT; col++)
                {
                    Coordinate cellPosition = new();
                    cellPosition.SetX(col);
                    cellPosition.SetY(row);

                    bool cellIsHit = _ownBoard.GetBoard(cellPosition).IsHit();
                    bool cellHasShip = _shipsOnOwnBoard.ContainsKey(cellPosition);

                    bool trackingCellIsHit = _trackingBoard.GetBoard(cellPosition).IsHit();
                    bool trackingCellHasShip = _trackingBoard.GetShipsOnBoard().ContainsKey(cellPosition);

                    var trackingButton = new Button
                    {
                        Margin = new Thickness(1),
                        Background = trackingCellIsHit ? trackingCellHasShip ? Brushes.Green : Brushes.Red : Brushes.LightYellow,
                        Content = trackingCellHasShip ? "🚢" : "",
                    };

                    Grid.SetRow(trackingButton, row);
                    Grid.SetColumn(trackingButton, col);
                    TrackingGrid.Children.Add(trackingButton);
                    trackingButton.Click += TakeTurn_Click;

                    var ownButton = new Button
                    {
                        Margin = new Thickness(1),
                        Content = cellHasShip ? "🚢" : "",
                        Background = cellIsHit ? Brushes.Red : Brushes.LightGray,
                    };

                    Grid.SetRow(ownButton, row);
                    Grid.SetColumn(ownButton, col);
                    OwnGrid.Children.Add(ownButton);
                }
            }
        }

        private void InitializeShipPanel()
        {
            List<IShip> currentPlayerShips = _gameController.GetPlayerFleet(_gameController.GetCurrentPlayerIndex());
            List<IShip> enemyShips = _gameController.GetPlayerFleet(_gameController.GetCurrentEnemyIndex());

            YourShips.Text = $"Your ships: {_gameController.RemainingShips()} ships remaining";

            foreach (IShip ship in currentPlayerShips)
            {
                TextBlock shipBlock = new TextBlock
                {
                    Text = $"{ship.GetName()} - ({ship.GetHits()}/{ship.GetSize()})",
                    Margin = new Thickness(5),
                    FontSize = 13,
                    Foreground = Brushes.Black,
                    Padding = new Thickness(5),
                    Background = ship.GetHits() >= ship.GetSize() ? Brushes.Red : Brushes.LightGreen,
                };

                OwnShipsPanel.Children.Add(shipBlock);
            }

            foreach (IShip ship in enemyShips)
            {
                TextBlock shipBlock = new TextBlock
                {
                    Text = $"{ship.GetName()}",
                    Margin = new Thickness(5),
                    FontSize = 13,
                    Foreground = Brushes.Black,
                    Padding = new Thickness(5),
                    Background = ship.GetHits() >= ship.GetSize() ? Brushes.Red : Brushes.LightGreen,
                };
                EnemyShipsPanel.Children.Add(shipBlock);
            }
        }

        private void TakeTurn_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            int row = Grid.GetRow(button);
            int col = Grid.GetColumn(button);

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
                NavigationService?.Navigate(new Uri("/Views/GameOverView.xaml", UriKind.Relative));
            }
            else
            {
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
            var currentPlayer = _gameController.GetCurrentPlayer();
            OwnGrid.Children.Clear();
            TrackingGrid.Children.Clear();
            OwnShipsPanel.Children.Clear();
            EnemyShipsPanel.Children.Clear();

            InitializeBattleGrid();
            InitializeShipPanel();
        }
    }
}
