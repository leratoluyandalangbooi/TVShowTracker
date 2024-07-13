using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TVShowTracker.Infrastructure.Persistence;


public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionBuilder.UseSqlServer("data source=.;initial catalog=tvshowtracker;TrustServerCertificate=True;Trusted_Connection=True;");

        return new ApplicationDbContext(optionBuilder.Options);

    }
}
