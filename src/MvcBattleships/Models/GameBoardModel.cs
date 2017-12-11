using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcBattleships.Models
{
    public class GameBoardModel
    {
        //Data

        public int RowSize { get; set; }
        public int ColSize { get; set; }
        //Tuple<True - placed ship; false - no ship , True - hit; false - not hit>
        private readonly List<List<Tuple<bool, bool>>> _myBoard;
        public List<List<Tuple<bool, bool>>> MyBoard
        {
            get
            {
                return _myBoard;
            }
        }
        //True - hit; false - miss; null - unexplored
        public List<List<bool?>> OpponentBoard;
        //TODO: Make validShips be developed in the construction
        private readonly List<int> _validShips = new List<int> { 2, 3, 3, 4, 5 };

        public string Error { get; private set; }


        //Methods

        //traverse grid and plot out all potential spots for a ship
        public List<Tuple<int, int>> TraverseGrid(int size, int row, int col, string dir)
        {
            var shipTiles = new List<Tuple<int, int>>();

            for (int i = 0; i < size; i++)
            {
                shipTiles.Add(new Tuple<int, int>(row, col));
                switch (dir)
                {
                    case "N":
                        row--;
                        break;
                    case "E":
                        col++;
                        break;
                    case "S":
                        row++;
                        break;
                    case "W":
                        col--;
                        break;
                }
            }

            return shipTiles;
        }

        public bool ValidatePlacement(List<Tuple<int, int>> potentialSpots)
        {
            try
            {
                foreach (var tile in potentialSpots)
                {
                    var row = tile.Item1;
                    var col = tile.Item2;
                    if (!_myBoard[row][col].Item1) continue;
                    Error = "Invalid ship size " + potentialSpots.Count + " placement: tile on ship path is already used.";
                    return false;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Error = "Ship size " + potentialSpots.Count + " placement out of bounds.";
                return false;
            }
            catch (Exception e)
            {
                Error = "Unexpected error: " + e.Message;
                return false;
            }
            return true;
        }

        private bool IsShipAvailable(int size)
        {
            if (size == 0)
            {
                Error = "Ship of size 0 is not possible. Something went wrong.";
            }
            else if (_validShips.Contains(size))
            {
                return true;
            }
            else
            {
                Error = "Ship of size " + size + " has been placed already or is not a valid ship size.";
            }
            return false;
        }

        //take list of spots and place the ship
        private void PlaceShip(IEnumerable<Tuple<int, int>> shipTiles)
        {
            foreach (var tile in shipTiles)
            {
                _myBoard[tile.Item1][tile.Item2] = new Tuple<bool, bool>(true, false);
            }
        }

        //Take a list of locations paired with directions
        //Takes all ship placements at once
        //returns true if ships were placed, false if ships were not placed
        //shipPlacements
        // int item1 - ship size
        // int item2 - row
        // int item3 - col
        // str item4 - direction - "N", "S", "E", "W"

        public bool PlaceShips(List<Tuple<int,int,int,string>> shipPlacements)
        {
            //A temporary list of required ships. If not all are used, break out
            //make sure manipulating this doesn't change validShips
            List<int> validShipsTemp = new List<int>(_validShips);
            List<int> usedShips = new List<int>();

            foreach (Tuple<int,int,int,string> ship in shipPlacements)
            {
                //Check if all ships are used, if true, allShipsUsed = true;
                if (validShipsTemp.Count == 0)
                {
                    Error = "More ships provided than can be placed on board.";
                    return false;
                }
                //traverse board, verify ship placement
                //if ship is invalid, rebuild myboard, return false
                var shipTiles = TraverseGrid(ship.Item1, ship.Item2 - 1, ship.Item3 - 1, ship.Item4);
                //Ship placement is valid and ship of appropriate size is unused
                if (ValidatePlacement(shipTiles) && IsShipAvailable(shipTiles.Count))
                {
                    PlaceShip(shipTiles);
                    validShipsTemp.Remove(shipTiles.Count());
                    usedShips.Add(shipTiles.Count());
                }
                else
                {
                    BuildMyBoard();
                    return false;
                }
            }

            return true;
        }

        //Return true if ship is hit, false otherwise
        public ShotStatus ReceiveShot(int row, int col)
        {
            //adjust for indexing
            row--;
            col--;
            //If ship is shot (true at [row, col].Item1), change tile data to true, true. return true
            try
            {
                if (_myBoard[row][col].Item1)
                {
                    _myBoard[row][col] = new Tuple<bool, bool>(true, true);
                    return ShotStatus.Hit;
                }
            }
            catch (IndexOutOfRangeException e)
            {
                Error = "Shot out of bounds.";
                return ShotStatus.OutofBounds;
            }

            return ShotStatus.Miss;
        }

        public void UpdateAfterShot(int row, int col, bool status)
        {
            row--;
            col--;
            OpponentBoard[row][col] = status;
        }

        //Return true if we lost
        public bool CheckLose()
        {
            //If we have a ship that hasn't been hit, we have not yet lost.
            return _myBoard.SelectMany(row => row).All(col => col.Item1 != true || col.Item2);
        }

        public void BuildMyBoard()
        {
            for (var row = 0; row < RowSize; row++)
            {
                _myBoard.Add(new List<Tuple<bool, bool>>(ColSize));
                for (var col = 0; col < ColSize; col++)
                {
                    _myBoard[row].Add(new Tuple<bool, bool>(false, false));
                }
            }
        }

        //Constructor
        public GameBoardModel(int numOfRows, int numOfCols)
        {
            RowSize = numOfRows;
            ColSize = numOfCols;
            OpponentBoard = new List<List<bool?>>(numOfRows);
            _myBoard = new List<List<Tuple<bool, bool>>>(numOfRows);

            //Initialize our representation of opponent's board with null (unexplored) and myBoard with false (no ship)
            BuildMyBoard();
            for (var row = 0; row < numOfRows; row++)
            {
                OpponentBoard.Add(new List<bool?>(numOfCols));
                for (var col = 0; col < numOfCols; col++)
                {
                    OpponentBoard[row].Add(null);
                }
            }
        }
    }
}
