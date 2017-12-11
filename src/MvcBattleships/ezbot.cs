using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MvcBattleships.Models;

namespace MvcBattleships
{
    //Class to define an MvcBattleships bot. It needs to create and return a model with ships placed, as well as define a method to take a shot on the board.
    public class ezbot
    {
        public GameBoardModel model { get; set; }
        private static List<string> dirList = new List<string> { "S", "E", "N", "W" };
        private int rowSize = 10;
        private int colSize = 10;
        private List<int> validShips = new List<int> { 2, 3, 3, 4, 5 };
        public ezbot(int rowSize, int colSize)
        {
            this.rowSize = rowSize;
            this.colSize = colSize;
            model = new GameBoardModel(rowSize, colSize);
            List<Tuple<int, int, int, string>> shipList;
            var r = new Random();
            //Create shipList, choose random spots until we get a valid setup.
            do
            {
                var count = 1;
                shipList = new List<Tuple<int, int, int, string>>();
                foreach(int shipSize in validShips)
                {
                    //    shipList.Add(new Tuple<int, int, int, string>(shipSize, r.Next(1, rowSize+1), r.Next(1, colSize+1), dirList[r.Next(4)]));
                    shipList.Add(new Tuple<int, int, int, string>(shipSize, 1,count,"S"));
                    count++;
                }
                

            } while (!model.PlaceShips(shipList));
        }
        public ezbot(GameBoardModel model)
        {
            this.model = model;
        }

        //This method is invoked when it is our turn to take a shot. Pick a spot on the opponent's board to shoot.
        public Tuple<int, int> TakeShot()
        {
            int rowGuess;
            int colGuess;
            do
            {
                var r = new Random();
                rowGuess = r.Next(rowSize);
                colGuess = r.Next(colSize);
            } while (model.OpponentBoard[rowGuess][colGuess] != null);
            rowGuess++;
            colGuess++;
            return new Tuple<int, int>(rowGuess, colGuess);
        }

        public ShotStatus ReceiveShot(int row, int col)
        {
            return model.ReceiveShot(row, col);
        }
    }
}
