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
    class Board : IBoard
    {
        private Cell[,] _grid;
        private Dictionary<Coordinate, Ship> _ships;

        public Board(int width, int height)
        {
            _grid = new Cell[width, height];
            _ships = new Dictionary<Coordinate, Ship>();
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
    }
}
