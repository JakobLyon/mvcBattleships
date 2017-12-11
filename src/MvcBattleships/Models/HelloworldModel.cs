using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MvcBattleships.Models
{
    public class HelloworldModel
    {
        public int rowSelect { get; set; }
        public SelectList rowSelectList { get; set; }
        public int colSelect { get; set; }
        public string dirSelect { get; set; }
        public SelectList dirSelectList { get; set; }
        public SelectList colSelectList { get; set; }
        public List<int> validShips = new List<int> { 2, 3, 3, 4, 5 };
        public string error { get; set; }
    }
}
