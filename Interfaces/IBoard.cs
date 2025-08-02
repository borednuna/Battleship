using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Battleship.Classes;
using Battleship.Structs;

namespace Battleship.Interfaces
{
    public interface IBoard
    {
        public Cell GetBoard(Coordinate coordinate);
        public Dictionary<Coordinate, Ship> GetShipsOnBoard();
        public Cell[,] GetBoardCells();
        public void AppendShipsOnBoard(Coordinate coordinate, IShip ship);
    }
}
