using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Interfaces;
using Battleship.Structs;
using Battleship.Enums;
using System.Diagnostics;

namespace Battleship.Classes
{
    public class Board : IBoard
    {
        private Cell[,] _grid;
        private readonly BoardType _boardType;
        public Board(int width, int height, BoardType boardType)
        {
            _boardType = boardType;
            _grid = new Cell[width, height];
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Coordinate position = new();
                    position.SetX(x);
                    position.SetY(y);
                    _grid[x, y] = new Cell(position);
                }
            }
        }

        public Cell GetBoard(Coordinate coordinate)
        {
            return _grid[coordinate.GetX(), coordinate.GetY()];
        }

        public BoardType GetBoardType()
        {
            return _boardType;
        }

        public Cell[,] GetBoardCells()
        {
            return _grid;
        }

        public Dictionary<Coordinate, IShip> GetAllShipCoordinates()
        {
            Dictionary<Coordinate, IShip> shipCoordinateDictionary = [];

            for (int i = 0; i < _grid.GetLength(0); i++)
            {
                for (int j = 0; j < _grid.GetLength(1); j++)
                {
                    Cell cell = _grid[i, j];
                    IShip? ship = cell.GetShip();
                    if (ship != null)
                    {
                        shipCoordinateDictionary.Add(cell.GetPosition(), ship);
                    }
                }
            }

            return shipCoordinateDictionary;
        }
    }
}
