using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TaskOneDraft.Models;

namespace TaskOneDraft.Areas.Identity.Data;

//class to define application dbcontext inheriting from IdentityDbContext
public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    //constructor to initialize dbcontext options
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    //to add your claims table into the database
    public DbSet<Claims> Claims { get; set; } //dbset property for claims table
    public DbSet<FilesModel> Files { get; set; }

    //method to configure the model creation process
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder); //call base implementation
        //customize the ASP.NET Identity model and override the defaults if needed.
        //for example, you can rename the ASP.NET Identity table names and more.
        //add your customizations after calling base.OnModelCreating(builder);
    }
}
