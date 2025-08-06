using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Interfaces;
using Battleship.Structs;
using Battleship.Enums;

namespace Battleship.Classes
{
    public class Board : IBoard
    {
        private Cell[,] _grid;
        private Dictionary<Coordinate, IShip> _ships; // TODO: better pake yang cell
        private BoardType _boardType;
        public Board(int width, int height, BoardType boardType)
        {
            _boardType = boardType;
            _grid = new Cell[width, height];
            _ships = [];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    _grid[x, y] = new Cell();
                }
            }
        }

        public Cell GetBoard(Coordinate coordinate)
        {
            return _grid[coordinate.GetX(), coordinate.GetY()];
        }

        public Cell[,] GetBoardCells()
        {
            return _grid;
        }

        public Dictionary<Coordinate, IShip> GetShipsOnBoard()
        {
            return _ships;
        }

        public void AppendShipsOnBoard(Coordinate coordinate, IShip ship)
        {
            _ships[coordinate] = (Ship)ship;
        }
    }
}
