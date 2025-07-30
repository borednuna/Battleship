using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Enums;
using Battleship.Interfaces;
using Battleship.Structs;

namespace Battleship.Classes
{
    class GameController
    {
        private List<IPlayer> _players;
        int _currentPlayerIndex;
        private IBoard _ownBoard;
        private IBoard _trackingBoard;
        private Dictionary<IPlayer, List<IShip>> _fleet;
        Action<IPlayer, Coordinate>? OnShotFired;

        public GameController(List<IPlayer> players)
        {
            _players = players;
            _currentPlayerIndex = 0;
            _ownBoard = new Board(10, 10); // Example size
            _trackingBoard = new Board(10, 10); // Example size
            _fleet = new Dictionary<IPlayer, List<IShip>>();
        }

        public void StartGame() { }

        public void TakeTurn(Coordinate position) { }

        public void SwitchTurn() { }

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
