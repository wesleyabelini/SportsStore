﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SportsStore.Domain.Entities;
using System.Data.Entity;

namespace SportsStore.Domain.Concrete
{
    class EFDbContext: DbContext
    {
        public EFDbContext() : base("EFDbContext") { }
        public DbSet<Product> Products { get; set; }
    }
}
