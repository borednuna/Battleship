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
        public const int BOARD_WIDTH = 14;
        public const int BOARD_HEIGHT = 14;
        private const uint PLAYERS_AMOUNT = 2;

        int _currentPlayerIndex = 0;
        private List<IPlayer> _players = [];
        private Dictionary<IPlayer, List<IShip>> _fleet = [];
        private Dictionary<IPlayer, List<IBoard>> _boards = [];
        private GameStates _gameState = GameStates.INITIALIZING;
        Action<IPlayer, Coordinate>? OnShotFired;
        private static GameController? _instance;

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
                Player player = new($"DefaultPlayer{i}");
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
        }

        public GameStates GetCurrentGameState()
        {
            return _gameState;
        }

        public void SetGameState(GameStates gameState)
        {
            _gameState = gameState;
        }

        public void SetPlayerNames(string player1Name, string player2Name)
        {
            if (_players.Count < 2)
            {
                Debug.WriteLine("Not enough players registered.");
            }

            _players[0].SetName(player1Name);
            _players[1].SetName(player2Name);

            _gameState = GameStates.PLACING_SHIPS;
        }

        public IPlayer GetCurrentPlayer()
        {
            return _players[_currentPlayerIndex];
        }

        public int GetCurrentPlayerIndex()
        {
            return _currentPlayerIndex;
        }

        public void StartGame() { }

        public void TakeTurn(Coordinate position) { }

        public void SwitchTurn()
        {
            //Math.Abs(_currentPlayerIndex--);
            _currentPlayerIndex++;
            if (_currentPlayerIndex >= _players.Count)
            {
                _currentPlayerIndex = 0;

                if (_gameState == GameStates.PLACING_SHIPS)
                {
                    _gameState = GameStates.PLAYING;
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

        public bool PlaceShip(ShipType type, List<Coordinate> position)
        {
            return false;
        }

        public bool ReceiveShot(Coordinate position)
        {
            return false;
        }

        public bool AllShipsSunk()
        {
            return false;
        }

        public void RegisterHit(Coordinate position) { }

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
