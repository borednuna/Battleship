using Battleship.Enums;
using Battleship.Interfaces;
using Battleship.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Battleship.Classes
{
    public class GameController
    {
        public static uint PLAYERS_AMOUNT = 2;
        public const int BOARD_WIDTH = 10;
        public const int BOARD_HEIGHT = 10;
        public const int OWN_BOARD_INDEX = 0;
        public const int TRACKING_BOARD_INDEX = 1;
        public const int MAX_PLAYERS_AMOUNT = 5;

        int _currentPlayerIndex = 0;
        int _currentEnemyIndex = 1;
        private List<IPlayer> _players = [];
        private Dictionary<IPlayer, List<IShip>> _fleet = [];
        private Dictionary<IPlayer, List<IBoard>> _boards = [];
        private GameStates _gameState;
        Action<IPlayer, Coordinate>? OnShotFired;
        private static GameController? _instance;
        private bool _isCurrentShipPlacementVertical;

        public static GameController GetInstance()
        {
            if (_instance == null)
            {
                _instance = new();
            }
            return _instance;
        }

        public void Reset()
        {
            _players.Clear();
            _fleet.Clear();
            _boards.Clear();
            _currentPlayerIndex = 0;
            _gameState = GameStates.INITIALIZING;

            for (int i = 0; i < PLAYERS_AMOUNT; i++)
            {
                InitializeAndAddPlayerProperties();
            }
        }

        public bool IsCurrentShipVertical()
        {
            return _isCurrentShipPlacementVertical;
        }

        public void SetCurrentShipOrientation(bool isVertical)
        {
            _isCurrentShipPlacementVertical = isVertical;
        }

        private void InitializeAndAddPlayerProperties()
        {
            Player player = new("DefaultPlayerName");
            _players.Add(player);

            List<IShip> ships = [];
            foreach (ShipType shipType in Enum.GetValues<ShipType>())
            {
                Ship ship = new(shipType);
                ships.Add(ship);
            }
            _fleet.Add(player, ships);

            List<IBoard> boards = [];
            foreach (BoardType boardType in Enum.GetValues(typeof(BoardType)))
            {
                Board board = new(BOARD_WIDTH, BOARD_HEIGHT, boardType);
                boards.Add(board);
            }
            _boards.Add(player, boards);
        }

        public GameStates GetCurrentGameState()
        {
            return _gameState;
        }

        public void SetPlayerNames(List<string> playerNames)
        {
            for (int i = 0; i < playerNames.Count; i++)
            {
                if (i >= _players.Count)
                {
                    InitializeAndAddPlayerProperties();
                }

                _players[i].SetName(playerNames[i]);
            }

            _gameState = GameStates.PLACING_SHIPS;
        }

        public IPlayer GetCurrentPlayer()
        {
            return _players[_currentPlayerIndex];
        }

        public IPlayer GetCurrentEnemy()
        {
            return _players[_currentEnemyIndex];
        }

        public List<IShip> GetCurrentPlayerFleet()
        {
            return _fleet[_players[_currentPlayerIndex]];
        }

        public List<IBoard> GetCurrentPlayerBoard()
        {
            return _boards[_players[_currentPlayerIndex]];
        }

        public void StartGame()
        {
            _gameState = GameStates.PLAYING;
        }

        public void TakeTurn(Coordinate position)
        {
            RegisterHit(position);
            SwitchTurn();
        }

        public void IsHitAccurate(Coordinate position, out bool isAccurate, out IShip? cellShip)
        {
            IBoard currentEnemyBoard = _boards[_players[_currentEnemyIndex]][OWN_BOARD_INDEX];
            Cell cell = currentEnemyBoard.GetBoard(position);
            isAccurate = cell.IsHit();
            cellShip = cell.GetShip();
        }

        public void SwitchTurn()
        {
            _currentPlayerIndex++;
            _currentEnemyIndex++;

            if (_currentEnemyIndex >= _players.Count)
            {
                _currentEnemyIndex = 0;
            }

            if (_currentPlayerIndex >= _players.Count)
            {
                _currentPlayerIndex = 0;
                if (_gameState == GameStates.PLACING_SHIPS)
                {
                    StartGame();
                }
            }
        }

        public void EndGame() { }

        public IPlayer CheckWinner()
        {
            // Logic to determine the winner
            return _players.FirstOrDefault() ?? new Player("Nunski"); // Placeholder for actual winner logic
        }

        public bool Shoot(Coordinate position)
        {
            return false;
        }

        public int RemainingShips()
        {
            return 0;
        }

        public string? PlaceShip(ShipType? type, List<Coordinate> position)
        {
            if (type == null)
            {
                return ErrorMessage.SHIP_NOT_SELECTED_ERROR;
            }

            if (position.Max(c => c.GetX()) >= BOARD_WIDTH || position.Max(c => c.GetY()) >= BOARD_HEIGHT)
            {
                return ErrorMessage.BOUNDARY_ERROR;
            }

            IBoard currentBoard = _boards[_players[_currentPlayerIndex]][OWN_BOARD_INDEX];
            List<IShip> currentFleet = GetCurrentPlayerFleet();
            Dictionary<Coordinate, Ship> shipsOnBoard = currentBoard.GetShipsOnBoard();
            Cell[,] cells = currentBoard.GetBoardCells();

            foreach (Coordinate coordinate in position)
            {
                if (shipsOnBoard.ContainsKey(coordinate))
                {
                    return ErrorMessage.SHIP_PLACEMENT_ERROR;
                }
            }

            IShip ship = currentFleet.FirstOrDefault(s => s.GetType() == type);
            if (ship == null)
            {
                return ErrorMessage.SHIP_NOT_FOUND_ERROR;
            }

            foreach (Coordinate coordinate in position)
            {
                //TODO: recheck the ship inside cells
                shipsOnBoard[coordinate] = (Ship)ship;
                cells[coordinate.GetX(), coordinate.GetY()].SetShip((Ship)ship);
            }

            return null;
        }

        public bool ReceiveShot(Coordinate position)
        {
            return false;
        }

        public bool AllShipsSunk()
        {
            return false;
        }

        public void RegisterHit(Coordinate position)
        {
            IBoard currentPlayerTrackingBoard = _boards[_players[_currentPlayerIndex]][TRACKING_BOARD_INDEX];
            Cell cell = currentPlayerTrackingBoard.GetBoard(position);
            cell.setIsHit(true);

            IsHitAccurate(position, out bool isAccurate, out IShip? cellShip);
            if (isAccurate && cellShip != null)
            {
                cell.SetShip((Ship)cellShip);
            }
        }

        public bool IsSunk()
        {
            return false;
        }

        public void MarkHit(Coordinate position) { }

        public bool HasShip(Coordinate position)
        {
            return false;
        }
    }
}
