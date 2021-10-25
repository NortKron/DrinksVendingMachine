using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using VendingMachineDrinks.Models;

namespace VendingMachineDrinks.Models
{
    // ...
    public class Drinks
    {
        [Key]
        public int Id { get; set; }

        public string Name { get; set; }

        public int Cost { get; set; }

        public int Count { get; set; }

    }

    public class Coins
    {
        [Key]
        public int CoinId { get; set; }

        public int Coin { get; set; }

        public bool Allow { get; set; }

    }

    public class DataModel
    {
        public IEnumerable<Drinks> Drinks { get; set; }
        public IEnumerable<Coins> Coins { get; set; }
    }

    public class DataContext : DbContext
    {
        public DbSet<Drinks> Drinks { get; set; }
        public DbSet<Coins> Coins { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {

        }        
    }
}
