using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

using System.Diagnostics;
using VendingMachineDrinks.Models;

namespace VendingMachineDrinks.Controllers
{
    public class AppSettings
    {
        public string Token { get; set; }
    }

    public class MyControllerAttribute : Attribute, IRouteTemplateProvider
    {
        public string Template => "adminkey123/";
        public int? Order => 0;
        public string Name { get; set; }
    }

    //[MyController]
    public class AdminController : Controller
    {
        private readonly DataContext _context;
        private readonly ILogger<AdminController> _logger;

        string _key = "123";

        public AdminController(DataContext context, ILogger<AdminController> logger, IOptions<AppSettings> settings)
        {
            _context = context;
            _logger = logger;

            _key = settings.Value.Token;
            Debug.Print(">>>> admin key : " + _key);
        }

        // GET: Admin
        //[HttpGet("")]
        public async Task<IActionResult> Index()
        {
            var drinks = await _context.Drinks.ToListAsync();
            var coins = await _context.Coins.ToListAsync();

            var model = new DataModel
            {
                Drinks = drinks,
                Coins = coins
            };

            return View(model);
        }

        // GET: Admin/Create
        //[HttpGet("Create")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Cost,Count")] Drinks drinks)
        {
            if (ModelState.IsValid)
            {
                _context.Add(drinks);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(drinks);
        }

        // GET: Admin/Edit/5
        //[HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drinks = await _context.Drinks.FindAsync(id);
            if (drinks == null)
            {
                return NotFound();
            }
            return View(drinks);
        }

        // POST: Admin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[HttpPost("Edit/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Cost,Count")] Drinks drinks)
        {
            if (id != drinks.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(drinks);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DrinksExists(drinks.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(drinks);
        }

        // GET: Admin/Delete/5
        //[HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var drinks = await _context.Drinks
                .FirstOrDefaultAsync(m => m.Id == id);
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
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var drinks = await _context.Drinks.FindAsync(id);
            _context.Drinks.Remove(drinks);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DrinksExists(int id)
        {
            return _context.Drinks.Any(e => e.Id == id);
        }

        [HttpPost]
        //[HttpPost("Save/{id}")]
        [ValidateAntiForgeryToken]
        public async Task Save([Bind("CoinId,Coin,Allow")] Coins coins)
        {
            _context.Update(coins);
            await _context.SaveChangesAsync();

            //return View("Index");
        }

        // Импорт напитков из файла JSON
        //[HttpPost("Import/{file}")]
        public void ImportDrinks()
        {
            // чтение файла
            while (true)
            {
                var drinks = new Drinks
                {
                    Name = "",
                    Cost = 0,
                    Count = 0
                };

                _context.SaveChanges();
            }
        }
    }
}
