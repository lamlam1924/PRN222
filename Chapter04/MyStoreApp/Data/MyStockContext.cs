using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using MyStoreApp.Models;

    public class MyStockContext : DbContext
    {
        public MyStockContext (DbContextOptions<MyStockContext> options)
            : base(options)
        {
        }

        public DbSet<MyStoreApp.Models.Product> Product { get; set; } = default!;
    }
