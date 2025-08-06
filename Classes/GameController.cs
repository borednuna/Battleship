using Battleship.Enums;
using Battleship.Interfaces;
using Battleship.Structs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Navigation;

namespace Battleship.Classes
{
    public class GameController
    {
        public const int BOARD_WIDTH = 10;
        public const int BOARD_HEIGHT = 10;
        public const int OWN_BOARD_INDEX = 0;
        public const int TRACKING_BOARD_INDEX = 1;
        public const int MAX_PLAYERS_AMOUNT = 5;

        int _currentPlayerIndex;
        int _currentEnemyIndex;
        bool _isPlayingWithBot;
        private Random _random = new Random();
        private List<IPlayer> _players = [];
        private Dictionary<IPlayer, List<IShip>> _fleet = [];
        private Dictionary<IPlayer, List<IBoard>> _boards = [];
        private GameStates _gameState;
        private IPlayer? _winner;
        public event Action<IPlayer, Coordinate>? OnShotFired;

        public void Reset()
        {
            _players.Clear();
            _fleet.Clear();
            _boards.Clear();
            _currentPlayerIndex = 0;
            _currentEnemyIndex = 1;
            _winner = null;
            _gameState = GameStates.INITIALIZING;
            _isPlayingWithBot = false;

            OnShotFired = null;
            OnShotFired += SetTrackingBoardCellHit;
            OnShotFired += SetEnemyBoardCellHit;
        }

        public Random GetRandomInstance()
        {
            return _random;
        }

        public void StartGame()
        {
            _gameState = GameStates.PLAYING;
        }

        public void EndGame()
        {
            _winner = CheckWinner();
            _gameState = GameStates.GAME_OVER;
        }

        public void SetGameState(GameStates state)
        {
            _gameState = state;
        }

        public GameStates GetCurrentGameState()
        {
            return _gameState;
        }

        public bool IsPlayerFleetPlaced(IPlayer player)
        {
            List<IShip> playerFleet = GetPlayerFleet(player);

            foreach (IShip ship in playerFleet)
            {
                if (!ship.GetIsPlaced())
                {
                    return false;
                }
            }

            return true;
        }

        public void AddPlayer(IPlayer player)
        {
            _players.Add(player);
        }

        public void AddPlayerFleet(IPlayer player, List<IShip> fleet)
        {
            _fleet[player] = fleet;
        }

        public void AddPlayerBoard(IPlayer player, List<IBoard> boards)
        {
            _boards[player] = boards;
        }

        public bool GetIsPlayingWithBot()
        {
            return _isPlayingWithBot;
        }

        public void SetIsPlayingWithBot(bool isPlayingWithBot)
        {
            _isPlayingWithBot = isPlayingWithBot;
        }

        public IPlayer? GetBotPlayer()
        {
            foreach (IPlayer player in _players)
            {
                if (player.GetPlayerType() == PlayerType.BOT)
                {
                    return player;
                }
            }
            return null;
        }

        public string? PlaceShipValidateBot(ShipType? type, List<Coordinate> position)
        {
            if (type == null)
            {
                return ErrorMessage.SHIP_NOT_SELECTED_ERROR;
            }
            if (
                position.Max(c => c.GetX()) >= BOARD_WIDTH
                || position.Max(c => c.GetY()) >= BOARD_HEIGHT
                || position.Min(c => c.GetX()) < 0
                || position.Min(c => c.GetY()) < 0
            ) {
                return ErrorMessage.BOUNDARY_ERROR;
            }

            IPlayer? botPlayer = GetBotPlayer();
            if (botPlayer == null)
            {
                return ErrorMessage.NO_BOT_FOUND_ERROR;
            }

            IBoard botBoard = _boards[botPlayer][OWN_BOARD_INDEX];
            List<IShip> botFleet = _fleet[botPlayer];
            Dictionary<Coordinate, IShip> shipsOnBoard = botBoard.GetShipsOnBoard();

            foreach (Coordinate coordinate in position)
            {
                if (shipsOnBoard.ContainsKey(coordinate))
                {
                    return ErrorMessage.SHIP_PLACEMENT_ERROR;
                }
            }

            IShip? ship = botFleet.FirstOrDefault(s => s.GetType() == type);
            if (ship == null)
            {
                return ErrorMessage.SHIP_NOT_FOUND_ERROR;
            }

            return null;
        }

        public bool PlaceShipBot(ShipType? type, List<Coordinate> position)
        {
            IPlayer? player = GetBotPlayer();
            if (player == null)
            {
                return false;
            }

            IBoard botBoard = _boards[player][OWN_BOARD_INDEX];
            List<IShip> botFleet = _fleet[player];
            Dictionary<Coordinate, IShip> shipsOnBoard = botBoard.GetShipsOnBoard();
            Cell[,] cells = botBoard.GetBoardCells();

            foreach (Coordinate coordinate in position)
            {
                if (shipsOnBoard.ContainsKey(coordinate))
                {
                    return false;
                }
            }

            IShip? ship = botFleet.FirstOrDefault(s => s.GetType() == type);
            if (ship == null)
            {
                return false;
            }

            foreach (Coordinate coordinate in position)
            {
                shipsOnBoard[coordinate] = (Ship)ship;
                cells[coordinate.GetX(), coordinate.GetY()].SetShip((Ship)ship);
            }

            return true;
        }
        
        public IPlayer GetCurrentPlayer()
        {
            return _players[_currentPlayerIndex];
        }

        public IPlayer GetCurrentEnemy()
        {
            return _players[_currentEnemyIndex];
        }

        public List<IShip> GetPlayerFleet(IPlayer player)
        {
            return _fleet[player];
        }

        public List<IBoard> GetPlayerBoards(IPlayer player)
        {
            return _boards[player];
        }

        public string? TakeTurnValidate(Coordinate coordinate)
        {
            bool isHitAlready = _boards[_players[_currentPlayerIndex]][TRACKING_BOARD_INDEX].GetBoard(coordinate).IsHit();
            if (isHitAlready)
            {
                return ErrorMessage.CELL_ALREADY_HIT_ERROR;
            }

            return null;
        }

        public void TakeTurn(Coordinate position)
        {
            OnShotFired?.Invoke(GetCurrentPlayer(), position);
            RegisterHit(position);
            SwitchTurn();
            
            if (AllShipsSunk())
            {
                EndGame();
            }

            if (_isPlayingWithBot && GetCurrentPlayer().GetPlayerType() == PlayerType.BOT)
            {
                TakeTurnBot();
            }
        }

        private void TakeTurnBot()
        {
            bool isPickedCoordinate = false;

            while (!isPickedCoordinate)
            {
                Coordinate position = new();
                int x = _random.Next(0, BOARD_WIDTH);
                int y = _random.Next(0, BOARD_HEIGHT);
                
                position.SetX(x);
                position.SetY(y);

                string? errorMessage = TakeTurnValidate(position);
                if (errorMessage == null)
                {
                    TakeTurn(position);
                    isPickedCoordinate = true;
                }
            }
        }

        public string? PlaceShipValidate(ShipType? type, List<Coordinate> position)
        {
            if (type == null)
            {
                return ErrorMessage.SHIP_NOT_SELECTED_ERROR;
            }

            if (position.Max(c => c.GetX()) >= BOARD_WIDTH || position.Max(c => c.GetY()) >= BOARD_HEIGHT)
            {
                return ErrorMessage.BOUNDARY_ERROR;
            }

            IBoard currentBoard = GetPlayerBoards(GetCurrentPlayer())[OWN_BOARD_INDEX];
            List<IShip> currentFleet = GetPlayerFleet(GetCurrentPlayer());
            Dictionary<Coordinate, IShip> shipsOnBoard = currentBoard.GetShipsOnBoard();

            foreach (Coordinate coordinate in position)
            {
                if (shipsOnBoard.ContainsKey(coordinate))
                {
                    return ErrorMessage.SHIP_PLACEMENT_ERROR;
                }
            }

            IShip? ship = currentFleet.FirstOrDefault(s => s.GetType() == type);
            if (ship == null)
            {
                return ErrorMessage.SHIP_NOT_FOUND_ERROR;
            }

            return null;
        }

        public bool PlaceShip(ShipType? type, List<Coordinate> position)
        {
            IBoard currentBoard = _boards[_players[_currentPlayerIndex]][OWN_BOARD_INDEX];
            List<IShip> currentFleet = GetPlayerFleet(GetCurrentPlayer());
            Dictionary<Coordinate, IShip> shipsOnBoard = currentBoard.GetShipsOnBoard();
            Cell[,] cells = currentBoard.GetBoardCells();

            foreach (Coordinate coordinate in position)
            {
                if (shipsOnBoard.ContainsKey(coordinate))
                {
                    return false;
                }
            }

            IShip? ship = currentFleet.FirstOrDefault(s => s.GetType() == type);
            if (ship == null)
            {
                return false;
            }

            ship.SetIsPlaced(true);
            foreach (Coordinate coordinate in position)
            {
                shipsOnBoard[coordinate] = (Ship)ship;
                cells[coordinate.GetX(), coordinate.GetY()].SetShip((Ship)ship);
            }

            return true;
        }

        public void SwitchTurn()
        {
            bool isPlacing = _gameState == GameStates.PLACING_SHIPS;

            if (isPlacing && _isPlayingWithBot)
            {
                StartGame();
                return;
            }

            _currentPlayerIndex++;
            _currentEnemyIndex++;

            if (_currentPlayerIndex >= _players.Count)
                _currentPlayerIndex = 0;

            if (_currentEnemyIndex >= _players.Count)
                _currentEnemyIndex = 0;

            if (isPlacing && !_isPlayingWithBot && _currentPlayerIndex == 0)
            {
                StartGame();
            }
        }

        public IPlayer CheckWinner()
        {
            foreach (var playerFleet in _fleet)
            {
                List<IShip> fleet = playerFleet.Value;
                bool playerAllShipSunk = true;

                foreach (IShip ship in fleet)
                {
                    if (ship.GetHits() < ship.GetSize())
                    {
                        playerAllShipSunk = false;
                    }
                }

                if (playerAllShipSunk)
                {
                    int loserIndex = _players.IndexOf(playerFleet.Key);
                    int winnerIndex = loserIndex - 1;
                    if (winnerIndex < 0)
                    {
                        winnerIndex = _players.Count - 1;
                    }

                    return _players[winnerIndex];
                }
            }

            return new Player("DefaultWinner", PlayerType.HUMAN);
        }

        public IPlayer? GetWinner()
        {
            return _winner;
        }

        public int RemainingShips()
        {
            List<IShip> ships = GetPlayerFleet(GetCurrentPlayer());
            int remainingShips = 0;

            foreach (IShip ship in ships)
            {
                if (ship.GetHits() < ship.GetSize())
                {
                    remainingShips++;
                }
            }
            return remainingShips;
        }

        public bool HasShip(Coordinate position)
        {
            IBoard currentEnemyBoard = _boards[_players[_currentEnemyIndex]][OWN_BOARD_INDEX];
            Cell enemyCell = currentEnemyBoard.GetBoard(position);
            IShip? enemyShipOnCell = enemyCell.GetShip();
            return enemyShipOnCell != null; // TODO: better dimasukin variable
        }

        public bool AllShipsSunk()
        {
            foreach (var playerFleet in _fleet)
            {
                List<IShip> fleet = playerFleet.Value;
                bool playerAllShipSunk = true;

                foreach (IShip ship in fleet)
                {
                    if (ship.GetHits() < ship.GetSize())
                    {
                        playerAllShipSunk = false;
                    }
                }

                if (playerAllShipSunk)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetTrackingBoardCellHit(IPlayer player, Coordinate position)
        {
            List<IBoard> currentPlayerBoards = _boards[player];
            IBoard currentPlayerTrackingBoard = currentPlayerBoards[TRACKING_BOARD_INDEX];
            Cell cell = currentPlayerTrackingBoard.GetBoard(position);
            cell.setIsHit(true);
        }

        public void SetEnemyBoardCellHit(IPlayer player, Coordinate position)
        {
            List<IBoard> currentEnemyBoards = _boards[GetCurrentEnemy()];
            IBoard currentEnemyBoard = currentEnemyBoards[OWN_BOARD_INDEX];
            Cell cell = currentEnemyBoard.GetBoard(position);
            cell.setIsHit(true);
        }

        public void RegisterHit(Coordinate position)
        {
            IBoard currentPlayerTrackingBoard = GetPlayerBoards(GetCurrentPlayer())[TRACKING_BOARD_INDEX];
            IBoard currentEnemyBoard = _boards[_players[_currentEnemyIndex]][OWN_BOARD_INDEX];
            Cell enemyCell = currentEnemyBoard.GetBoard(position);
            IShip? cellShip = enemyCell.GetShip();

            bool isAccurate = HasShip(position);
            if (isAccurate && cellShip != null)
            {
                currentPlayerTrackingBoard.GetBoard(position).SetShip((Ship)cellShip);
                currentPlayerTrackingBoard.AppendShipsOnBoard(position, cellShip);
                MarkHit(position);
            }
        }

        public void MarkHit(Coordinate position)
        {
            IBoard currentEnemyBoard = _boards[_players[_currentEnemyIndex]][OWN_BOARD_INDEX];
            Cell cell = currentEnemyBoard.GetBoard(position);
            IShip? ship = cell.GetShip();
            ship?.SetHits(ship.GetHits() + 1);
        }

        public bool Shoot(Coordinate position)
        {
            return false;
        }

        public bool ReceiveShot(Coordinate position)
        {
            return false;
        }

        public bool IsSunk()
        {
            return false;
        }
    }
}
