using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositeries;




namespace Tests
{
    public class DatabaseFixture: IDisposable
    {
            public Store_215962135Context Context { get; private set; }

            public DatabaseFixture()
            {

                // Set up the test database connection and initialize the context
                var options = new DbContextOptionsBuilder<Store_215962135Context>()

                    .UseSqlServer("Server=localhost;Database=Store_215962135;Trusted_Connection=True;TrustServerCertificate=True;")
                    .Options;
                Context = new Store_215962135Context(options);
                Context.Database.EnsureCreated();
            }

            public void Dispose()
            {
                // Clean up the test database after all tests are completed
                Context.Database.EnsureDeleted();
                Context.Dispose();
            }
        
    }
}
