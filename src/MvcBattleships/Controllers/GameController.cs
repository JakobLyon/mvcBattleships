using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MvcBattleships.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;

namespace MvcBattleships.Controllers
{
    public class GameController : Controller
    {
        private static IEnumerable<int> rowSize = Enumerable.Range(1, 10).ToList();
        private static IEnumerable<int> colSize = Enumerable.Range(1, 10).ToList();
        private static List<string> dirList = new List<string> { "S", "E", "N", "W" };
        private HelloworldModel settingsModel = new HelloworldModel
        {
            rowSelectList = new SelectList(rowSize),
            colSelectList = new SelectList(colSize),
            dirSelectList = new SelectList(dirList)
        };

        // GET: /<controller>/
        public IActionResult Index()
        {
            //Initialize game session data

            List<int> validShips = new List<int> { 2, 3, 3, 4, 5 };
            List<string> dirList = new List<string> { "S", "E", "N", "W" };
            GameBoardModel player = new GameBoardModel(10, 10);
            ezbot opponent = new ezbot(10, 10);
            //HttpContext.Session.SetString("dirList", dirList);
            HttpContext.Session.SetString("validShips", JsonConvert.SerializeObject(validShips));
            HttpContext.Session.SetString("opponentModel", JsonConvert.SerializeObject(opponent.model));
            HttpContext.Session.SetString("player", JsonConvert.SerializeObject(player));
            //var value = JsonConvert.DeserializeObject<List<string>>(HttpContext.Session.GetString("bro"));
            return View("/Views/Game/PlaceShips.cshtml", settingsModel);
        }

        [HttpPost]
        public IActionResult PlaceShips()
        {
            GameBoardModel player = JsonConvert.DeserializeObject<GameBoardModel>(HttpContext.Session.GetString("player"));
            //parse data and send to player model in form PlaceShips(List<Tuple<int,int,int,string>> shipPlacements)
            var shipPlacementList = new List<Tuple<int, int, int, string>>();
            foreach (string field in Request.Form.Keys.Select(n => n).Where(n => n.Length == 2))
            {
                var tempShip = new Tuple<int, int, int, string>(Convert.ToInt32(field.Substring(0,1)), Convert.ToInt32(Request.Form[field][0]), Convert.ToInt32(Request.Form[field][1]), Request.Form[field][2] );
                shipPlacementList.Add(tempShip);
            }
            if (player.PlaceShips(shipPlacementList))
            {
                HttpContext.Session.SetString("player", JsonConvert.SerializeObject(player));
                return View("/Views/Game/TakeShots.cshtml", player);
            }
            else
            {
                settingsModel.error = player.Error;
                return View("/Views/Game/PlaceShips.cshtml", settingsModel);
            }
        }

        //This method is disgusting -REWRITE THIS- Opponent can take their shot after we have determined if theyve been hit/if player has won.
        [HttpPost]
        public JsonResult TakeShot([FromBody]TakeShotViewModel shot)
        {
            GameBoardModel player = JsonConvert.DeserializeObject<GameBoardModel>(HttpContext.Session.GetString("player"));
            GameBoardModel opponentModel = JsonConvert.DeserializeObject<GameBoardModel>(HttpContext.Session.GetString("opponentModel"));
            ezbot opponent = new ezbot(opponentModel);
            Tuple<int, int> incomingShotGuess;
            //Resolve player shot
            var opponentHit = false;
            switch (opponent.ReceiveShot(shot.row, shot.col))
            {
                case ShotStatus.Hit:
                    opponentHit = true;
                    break;
                case ShotStatus.Miss:
                    opponentHit = false;
                    break;
                case ShotStatus.OutofBounds:
                    return Json(new { modelResponse = opponent.model.Error });
            }
            player.UpdateAfterShot(shot.row, shot.col, opponentHit);
            //Check for player win condition
            var playerWin = false;
            playerWin = opponent.model.CheckLose();
            //Opponent takes a shot
            incomingShotGuess = opponent.TakeShot();
            //Player Receives shot
            var playerHit = player.ReceiveShot(incomingShotGuess.Item1, incomingShotGuess.Item2);
            bool playerHitData = true;
            if (playerHit == ShotStatus.Hit)
            {
                playerHitData = true;
            }
            else if (playerHit == ShotStatus.Miss)
            {
                playerHitData = false;
            }
            opponent.model.UpdateAfterShot(incomingShotGuess.Item1, incomingShotGuess.Item2, playerHitData);
            //check for opponent win condition
            var opponentWin = false;
            opponentWin = player.CheckLose();
            //return the results
            

            HttpContext.Session.SetString("player", JsonConvert.SerializeObject(player));
            HttpContext.Session.SetString("opponentModel", JsonConvert.SerializeObject(opponent.model));
            return Json(new { win = new { playerWin = playerWin, opponentWin = opponentWin }, modelResponse = opponentHit, incomingShot = new { playerHit = playerHitData, row = incomingShotGuess.Item1, col = incomingShotGuess.Item2 } });
        }
    }
}
