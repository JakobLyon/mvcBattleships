using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MvcBattleships.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace MvcBattleships.Controllers
{
    public class HelloworldController : Controller
    {
        // 
        // GET: /HelloWorld/
        [HttpGet]
        public IActionResult Index()
        {
            var rowSize = Enumerable.Range(1, 10).ToList();
            var colSize = Enumerable.Range(1, 10).ToList();
            var dirList = new List<string> { "N", "E", "S", "W" };
            HelloworldModel rowColModel = new HelloworldModel
            {
                rowSelect = 1,
                rowSelectList = new SelectList(rowSize),
                colSelect = 1,
                colSelectList = new SelectList(colSize),
                dirSelect = "N",
                dirSelectList = new SelectList(dirList)
            };
            return View(rowColModel);
        }

        // 
        // GET: /HelloWorld/Welcome/ 

        public IActionResult Welcome(string name, int num = 1)
        {
            ViewData["message"] = "Hello" + name;
            ViewData["num"] = num;

            return View();
        }

        [HttpPost]
        public IActionResult TakeShot()
        {
            //process shot, if valid, send back success message, if fail, fail message
            //valid conditions: inside gameboard: return out of bounds, not checked: return location already shot
            var temp = new Tuple<string, string>(Request.Form["rowChoice"], Request.Form["colChoice"]);

            return View("/Views/Helloworld/TestRedirect.cshtml", temp);
        }
    }
}