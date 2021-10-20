using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VendingMachineDrinks.Models;

namespace VendingMachineDrinks.Controllers
{
    public class CoinsController : Controller
    {
        private readonly DataContext _context;

        public CoinsController(DataContext context)
        {
            _context = context;
        }

        // GET: Coins
        public async Task<IActionResult> Index()
        {
            return View(await _context.Coins.ToListAsync());
        }

        // GET: Coins/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coins = await _context.Coins
                .FirstOrDefaultAsync(m => m.CoinId == id);
            if (coins == null)
            {
                return NotFound();
            }

            return View(coins);
        }

        // GET: Coins/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Coins/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CoinId,Coin,Allow")] Coins coins)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coins);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coins);
        }

        // GET: Coins/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coins = await _context.Coins.FindAsync(id);
            if (coins == null)
            {
                return NotFound();
            }
            return View(coins);
        }

        // POST: Coins/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CoinId,Coin,Allow")] Coins coins)
        {
            if (id != coins.CoinId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coins);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoinsExists(coins.CoinId))
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
            return View(coins);
        }

        // GET: Coins/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coins = await _context.Coins
                .FirstOrDefaultAsync(m => m.CoinId == id);
            if (coins == null)
            {
                return NotFound();
            }

            return View(coins);
        }

        // POST: Coins/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coins = await _context.Coins.FindAsync(id);
            _context.Coins.Remove(coins);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoinsExists(int id)
        {
            return _context.Coins.Any(e => e.CoinId == id);
        }
    }
}
