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

            @ViewBag.Text = "Внесите монеты";
            @ViewBag.Amount = amount;

            foreach (var drink in drinks)
            {
                ViewData["Count-" + drink.Id] = drink.Count;
            }

            var model = new Datas 
            { 
                Drinks = drinks, 
                Coins = coins
            };
            
            return View(model);
        }

        private bool DrinksExists(int id)
        {
            return _context.Drinks.Any(e => e.Id == id);
        }

        public IActionResult DropCoin(int coin)
        {
            Debug.Print(">>>> coin = " + coin);

            if (coin == null)
            {
                ViewBag.Text = "ID монеты неизвестен";
                ViewBag.Amount = amount;
                return PartialView("_GetMessage", ViewBag);
            }

            amount = amount + coin;
            ViewBag.Text = "Внесите монеты";
            ViewBag.Amount = amount;
            return PartialView("_GetMessage", ViewBag);
        }

        public IActionResult Select(int? id)
        {
            Debug.Print(">>>> drink id = " + id);

            if (id == null)
            {
                return PartialView("");
            }

            var drinks = _context.Drinks.FirstOrDefault(m => m.Id == id);

            if (drinks.Cost > amount)
            {
                ViewBag.Text = "Недостаточная сумма. Внесите ещё монеты";
                ViewBag.Amount = amount;
                return PartialView("_GetMessage", ViewBag);
            }

            if (drinks.Count == 0)
            {
                ViewBag.Text = "Напиток закончился. Выберите другой";
                ViewBag.Amount = amount;
                return PartialView("_GetMessage", ViewBag);
            }
            else
            {
                amount = amount - drinks.Cost;
                
                drinks.Count--;
                ViewData["Count-" + drinks.Id] = drinks.Count;

                _context.SaveChanges();

                ViewBag.Text = "Напиток приготовлен";
                ViewBag.Amount = amount;
                return PartialView("_GetMessage", ViewBag);
            }
        }

        public IActionResult GetChange()
        {
            amount = 0;
            Debug.Print(">>>> Выдать сдачу");

            ViewBag.Text = "Сдача выдана";
            ViewBag.Amount = amount;
            
            return PartialView("_GetMessage", ViewBag);
        }
    }
}
