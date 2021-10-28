using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

using VendingMachineDrinks.Models;

namespace VendingMachineDrinks.Controllers
{
    public class AppSettings
    {
        public string AdminKeyAccess { get; set; }
    }

    public class AdminController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<AdminController> _logger;

        string _key = "adminkey";

        public AdminController(DataContext context, ILogger<AdminController> logger, IOptions<AppSettings> settings)
        {
            _context = context;
            _logger = logger;

            _key = settings.Value.AdminKeyAccess;
        }

        // GET: Admin
        //[HttpGet("")]
        public IActionResult Index(string? key)
        {
            if (_key != key || key == null)
            {
                return View("No_access");
            }

            var drinks = _context.Drinks.ToList();
            var coins = _context.Coins.ToList();

            var model = new DataModel
            {
                Drinks = drinks,
                Coins = coins
            };

            return View(model);
        }

        // GET: Admin/Create
        //[HttpGet("Create")]
        public IActionResult Create() => View();

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public IActionResult Create([Bind("Id,Name,Cost,Count")] Drinks drinks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drinks);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(drinks);
        }

        //[HttpPost("Edit")]
        public Object Edit([Bind("Id,Name,Cost,Count")] Drinks drinks)
        {
            if (drinks.Name == null || drinks.Cost <= 0)
            {
                ViewData["Message"] = "Данные по напитку (название, цена) неверно заполнены";
            }
            else
            {
                _context.Update(drinks);
                _context.SaveChanges();

                ViewData["Message"] = "Данные по напиткам сохранены";
            }
            return ViewData;
        }

        // GET: Admin/Delete/5
        //[HttpGet("Delete/{id}")]
        public IActionResult Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drinks = _context.Drinks.FirstOrDefault(m => m.Id == id);
            if (drinks == null)
            {
                return NotFound();
            }

            return View(drinks);
        }

        // POST: Admin/Delete/5
        [HttpPost, ActionName("Delete")]
        //[HttpPost("Delete/{id}")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteConfirmed(int id)
        {
            var drinks = _context.Drinks.Find(id);
            _context.Drinks.Remove(drinks);
            _context.SaveChanges();
            return RedirectToAction(nameof(Index));
        }

        private bool DrinksExists(int id)
        {
            return _context.Drinks.Any(e => e.Id == id);
        }

        //[HttpPost("Save")]
        public Object Save(Dictionary<string, string> coinDic)
        {
            var coins = _context.Coins;

            foreach(var coin in coins)
            {
                coin.Allow = coinDic.ContainsKey("Coin-" + coin.CoinId.ToString());
            }          

            _context.SaveChanges();

            ViewData["Message"] = "Данные по монетам сохранены";

            return ViewData;
        }

        // Импорт напитков из файла JSON
        //[HttpPost("Import/{file}")]
        [HttpPost]
        public Object ImportDrinks(IFormFile file)
        {
            var content = string.Empty;

            using (var reader = new StreamReader(file.OpenReadStream()))
            {
                content = reader.ReadToEnd();
            }
            
            List<Drinks> drinksObjects = JsonConvert.DeserializeObject<List<Drinks>>(content);
            try
            {
                drinksObjects = JsonConvert.DeserializeObject<List<Drinks>>(content);
            }
            catch
            {
                ViewData["Message"] = "Не удалось загрузить данные из файла " + file.FileName;
                return ViewData;
            }

            foreach (var drink in drinksObjects)
            {

                Drinks dnk = new Drinks
                {
                    Name = drink.Name,
                    Cost = drink.Cost,
                    Count = drink.Count
                };

                _context.Drinks.Add(dnk);
                _context.SaveChanges();
            }

            ViewData["Message"] = "Данные импортированы. Всего добавлено напитков в БД : " + drinksObjects.Count();
            return ViewData;
        }
    }
}
