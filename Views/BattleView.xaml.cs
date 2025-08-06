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

            List<IBoard> currentPlayerBoards = _gameController.GetPlayerBoards(_gameController.GetCurrentPlayer());
            IBoard ownBoard = currentPlayerBoards[GameController.OWN_BOARD_INDEX];
            IBoard trackingBoard = currentPlayerBoards[GameController.TRACKING_BOARD_INDEX];
            Dictionary<Coordinate, IShip> shipsOnOwnBoard = ownBoard.GetShipsOnBoard();

            EnemyBoard.Text = $"Enemy {_gameController.GetCurrentEnemy().GetName()} Board";
            BattlePanelTitle.Text = $"Battle - {_gameController.GetCurrentPlayer().GetName()}'s Turn";

            for (int row = 0; row < GameController.BOARD_WIDTH; row++)
            {
                for (int col = 0; col < GameController.BOARD_HEIGHT; col++)
                {
                    Coordinate cellPosition = new();
                    cellPosition.SetX(col);
                    cellPosition.SetY(row);

                    bool cellIsHit = ownBoard.GetBoard(cellPosition).IsHit();
                    bool cellHasShip = shipsOnOwnBoard.ContainsKey(cellPosition);

                    bool trackingCellIsHit = trackingBoard.GetBoard(cellPosition).IsHit();
                    bool trackingCellHasShip = trackingBoard.GetShipsOnBoard().ContainsKey(cellPosition);

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

                    var ownBoardButton = new Button
                    {
                        Margin = new Thickness(1),
                        Content = cellHasShip ? "🚢" : "",
                        Background = cellIsHit ? Brushes.Red : Brushes.LightGray,
                    };

                    Grid.SetRow(ownBoardButton, row);
                    Grid.SetColumn(ownBoardButton, col);
                    OwnGrid.Children.Add(ownBoardButton);
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
                if (_gameController.GetIsPlayingWithBot())
                {
                    Thread.Sleep(500);
                    RepaintGrids();
                }
                else
                {
                    Thread.Sleep(500);
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
