using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using System.Diagnostics;

using VendingMachineDrinks.Models;

namespace VendingMachineDrinks.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<HomeController> _logger;

        private static int amount = 0;

        public HomeController(DataContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;

            Debug.Print(">>>> start");
        }

        // GET: Main
        public IActionResult Index()
        {
            var drinks = _context.Drinks.ToList();
            var coins = _context.Coins.ToList();

            ViewData["Message"] = "Внесите монеты";
            ViewData["Amount"] = amount;
            DrinksEnabled();

            var model = new DataModel
            { 
                Drinks = drinks, 
                Coins = coins
            };

            Debug.Print(">>>> " + ViewData["Amount"]);

            return View(model);
        }

        private void DrinksEnabled()
        {
            var listIndx = new List<string>();
            var listVal = new List<bool>();

            foreach (var drink in _context.Drinks)
            {
                listIndx.Add("drink-" + drink.Id);
                listVal.Add( (drink.Count == 0 || drink.Cost > amount) );
            }

            ViewData["indexes"] = listIndx;
            ViewData["enabled"] = listVal;
        }

        public Object DropCoin(int coin)
        {
            amount += coin;
            ViewData["Amount"] = amount;
            DrinksEnabled();

            return ViewData;
        }

        //public IActionResult Select(int? id)
        public Object Select(int? id)
        {
            var drinks = _context.Drinks.FirstOrDefault(m => m.Id == id);
            
            amount -= drinks.Cost;                
            drinks.Count--;

            ViewData["Drink-Id"] = "drink-" + drinks.Id;
            ViewData["Drink-Count"] = drinks.Count;
            ViewData["Message"] = "Напиток приготовлен";
            ViewData["Amount"] = amount;

            _context.SaveChanges();

            DrinksEnabled();
            //return PartialView("_GetMessage", ViewBag);
            return ViewData;
        }

        public Object GetChange()
        {
            amount = 0;
            //Debug.Print(">>>> Выдать сдачу");

            ViewData["Message"] = "Сдача выдана";
            ViewData["Amount"] = amount;
            DrinksEnabled();

            //return PartialView("_GetMessage", ViewBag);
            return ViewData;
        }
    }
}
