using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CRUP_App.Entities;

public partial class DotnetdbContext : DbContext
{
    public DotnetdbContext()
    {
    }

    public DotnetdbContext(DbContextOptions<DotnetdbContext> options)
        : base(options)
    {
    }
 


    public virtual DbSet<User> User { get; set; }
    public virtual DbSet<Product> Product { get; set; }
    public virtual DbSet<Order> Order { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { }

}
