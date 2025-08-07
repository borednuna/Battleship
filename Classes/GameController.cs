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

        public IShip? GetShipByType(ShipType type, IPlayer player)
        {
            List<IShip> playerFleet = GetPlayerFleet(player);

            foreach (IShip ship in playerFleet)
            {
                if (ship.GetShipType() == type)
                {
                    return ship;
                }
            }

            return null;
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
            IPlayer? botPlayer = null;

            foreach (IPlayer player in _players)
            {
                if (player.GetPlayerType() == PlayerType.BOT)
                {
                    botPlayer = player;
                }
            }

            return botPlayer;
        }

        public string? PlaceShipValidate(IPlayer player, ShipType? type, List<Coordinate> position)
        {
            if (type == null)
            {
                return ErrorMessage.SHIP_NOT_SELECTED_ERROR;
            }

            if (position.Max(c => c.GetX()) >= BOARD_WIDTH || position.Max(c => c.GetY()) >= BOARD_HEIGHT)
            {
                return ErrorMessage.BOUNDARY_ERROR;
            }

            IBoard? currentBoard = GetPlayerBoardByType(player, BoardType.OWN_BOARD);
            if (currentBoard == null)
            {
                return null;
            }

            List<IShip> currentFleet = GetPlayerFleet(player);
            Dictionary<Coordinate, IShip> shipsOnBoard = currentBoard.GetAllShipCoordinates();

            foreach (Coordinate coordinate in position)
            {
                if (shipsOnBoard.ContainsKey(coordinate))
                {
                    return ErrorMessage.SHIP_PLACEMENT_ERROR;
                }
            }

            IShip? ship = currentFleet.FirstOrDefault(s => s.GetShipType() == type);
            if (ship == null)
            {
                return ErrorMessage.SHIP_NOT_FOUND_ERROR;
            }

            return null;
        }

        public bool PlaceShip(IPlayer player, ShipType? type, List<Coordinate> position)
        {
            IBoard? currentBoard = GetPlayerBoardByType(player, BoardType.OWN_BOARD);
            if (currentBoard == null)
            {
                return false;
            }

            List<IShip> currentFleet = GetPlayerFleet(player);

            IShip? ship = currentFleet.FirstOrDefault(s => s.GetShipType() == type);
            if (ship == null)
            {
                return false;
            }

            ship.SetIsPlaced(true);
            foreach (Coordinate coordinate in position)
            {
                Cell cellToBeAssignedShip = currentBoard.GetBoard(coordinate);
                cellToBeAssignedShip.SetShip((Ship)ship);
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

        public IBoard? GetPlayerBoardByType(IPlayer player, BoardType boardType)
        {
            IBoard? board = null;
            List<IBoard> playerBoards = _boards[player];

            foreach (IBoard playerBoard in playerBoards)
            {
                if (playerBoard.GetBoardType() == boardType)
                {
                    board = playerBoard;
                }
            }

            return board;
        }

        public string? TakeTurnValidate(Coordinate coordinate)
        {
            IBoard? trackingBoard = GetPlayerBoardByType(_players[_currentPlayerIndex], BoardType.TRACKING_BOARD);
            if (trackingBoard == null)
            {
                return null;
            }

            bool isHitAlready = trackingBoard.GetBoard(coordinate).IsHit();
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

        public IPlayer? CheckWinner()
        {
            IPlayer? winner = null;

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

                    winner = _players[winnerIndex];
                }
            }

            return winner;
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
            IBoard? currentEnemyBoard = GetPlayerBoardByType(GetCurrentEnemy(), BoardType.OWN_BOARD);
            if (currentEnemyBoard == null)
            {
                return false;
            }

            Cell enemyCell = currentEnemyBoard.GetBoard(position);
            IShip? enemyShipOnCell = enemyCell.GetShip();
            bool cellHasShip = enemyShipOnCell != null;
            return cellHasShip;
        }

        public bool AllShipsSunk()
        {
            foreach (var player in _fleet)
            {
                bool allPlayerShipsSunk = true;
                List<IShip> fleet = player.Value;

                foreach (IShip ship in fleet)
                {
                    if (ship.GetHits() >= ship.GetSize())
                    {
                        allPlayerShipsSunk &= true;
                    } else
                    {
                        allPlayerShipsSunk &= false;
                    }
                }

                if (allPlayerShipsSunk)
                {
                    return true;
                }
            }

            return false;
        }

        public void SetTrackingBoardCellHit(IPlayer player, Coordinate position)
        {
            IBoard? currentPlayerTrackingBoard = GetPlayerBoardByType(GetCurrentPlayer(), BoardType.TRACKING_BOARD);
            if (currentPlayerTrackingBoard == null)
            {
                return;
            }

            Cell cell = currentPlayerTrackingBoard.GetBoard(position);
            cell.setIsHit(true);
        }

        public void SetEnemyBoardCellHit(IPlayer player, Coordinate position)
        {
            IBoard? currentEnemyBoard = GetPlayerBoardByType(GetCurrentEnemy(), BoardType.OWN_BOARD);
            if (currentEnemyBoard == null)
            {
                return;
            }

            Cell cell = currentEnemyBoard.GetBoard(position);
            cell.setIsHit(true);
        }

        public void RegisterHit(Coordinate position)
        {
            IBoard? currentPlayerTrackingBoard = GetPlayerBoardByType(GetCurrentPlayer(), BoardType.TRACKING_BOARD);
            if (currentPlayerTrackingBoard == null)
            {
                return;
            }

            IBoard? currentEnemyBoard = GetPlayerBoardByType(GetCurrentEnemy(), BoardType.OWN_BOARD);
            if (currentEnemyBoard == null)
            {
                return;
            }

            Cell enemyCell = currentEnemyBoard.GetBoard(position);
            IShip? cellShip = enemyCell.GetShip();

            bool isAccurate = HasShip(position);
            if (isAccurate && cellShip != null)
            {
                currentPlayerTrackingBoard.GetBoard(position).SetShip((Ship)cellShip);
                MarkHit(position);
            }
        }

        public void MarkHit(Coordinate position)
        {
            IBoard? currentEnemyBoard = GetPlayerBoardByType(GetCurrentEnemy(), BoardType.OWN_BOARD);
            if (currentEnemyBoard == null)
            {
                return;
            }

            Cell cell = currentEnemyBoard.GetBoard(position);
            IShip? ship = cell.GetShip();
            ship?.SetHits(ship.GetHits() + 1);
        }
    }
}
