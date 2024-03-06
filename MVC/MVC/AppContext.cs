using MVC.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC
{
    public class AppContext : DbContext
    {
        public AppContext()
        : base("DefaultConnection")
        {
        }
        public DbSet<Client> Clients { get; set; }
    }
}