using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

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
            return ViewData;
        }

        public Object GetChange()
        {
            string resultString = ""; ;
            int amountTemp = amount;

            #region Рассчитать монеты для выдачи сдачи
            while (true)
            {
                if (amountTemp == 0)
                {
                    resultString = resultString.Remove(resultString.Length - 1);
                    break;
                }

                if (amountTemp - 10 >= 0)
                {
                    amountTemp -= 10;
                    resultString += "10, ";
                }
                else
                {
                    if (amountTemp - 5 >= 0)
                    {
                        amountTemp -= 5;
                        resultString += "5, ";
                    }
                    else
                    {
                        if (amountTemp - 2 >= 0)
                        {
                            amountTemp -= 2;
                            resultString += "2, ";
                        }
                        else
                        {
                            amountTemp--;
                            resultString += "1, ";
                        }
                    }
                }
            }
            #endregion

            amount = 0;

            ViewData["ListCoins"] = "Выданы монеты: " + resultString;
            ViewData["Message"] = "Сдача выдана";
            ViewData["Amount"] = amount;
            DrinksEnabled();

            return ViewData;
        }
    }
}
