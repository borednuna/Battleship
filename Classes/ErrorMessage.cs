using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Battleship.Classes
{
    public static class ErrorMessage
    {
        public static readonly string BOUNDARY_ERROR = "Ships cannot be placed outside the board boundaries.";
        public static readonly string SHIP_PLACEMENT_ERROR = "Ship cannot be placed here. Please choose a different location.";
        public static readonly string SHIP_NOT_SELECTED_ERROR = "Please select a ship type first.";
        public static readonly string SHIP_NOT_FOUND_ERROR = "Ship not found in the fleet.";
        public static readonly string CELL_ALREADY_HIT_ERROR = "This cell has already been hit.";
        public static readonly string NO_BOT_FOUND_ERROR = "No bot found.";
        public static readonly string CONFIRM_TO_PLAY_WITH_BOT = "Are you sure you want to play with a bot?";
        public static readonly string MAX_PLAYERS_AMOUNT_ERROR = "Maximum players amount exceed!";
    }
}
