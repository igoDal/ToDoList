using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ToDoListMVC.Models;

namespace ToDoListMVC.Context;

public class ApplicationDbContext : IdentityDbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
        
    }

    public DbSet<Tasks> Tasks { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        modelBuilder.Entity<Tasks>()
            .HasKey(x => x.Id);

        modelBuilder.Entity<Tasks>()
            .Property(x => x.Id)
            .ValueGeneratedOnAdd();
        modelBuilder.Entity<IdentityUser>(entity => {
            entity.ToTable(name: "Users");
        });
        modelBuilder.Entity<IdentityRole>(entity => {
            entity.ToTable(name: "Roles");
        });
    }
    

}