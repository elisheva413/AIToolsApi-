using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repositeries;
using System;


namespace Tests
{
    public class DatabaseFixture : IDisposable
    {
        public Store_215962135Context Context { get; private set; }
        public DatabaseFixture()
        {
            var options = new DbContextOptionsBuilder<Store_215962135Context>()
                .UseSqlServer("Server=localhost;Database=Store_215962135_Test;Trusted_Connection=True;TrustServerCertificate=True;")
                .Options;

            Context = new Store_215962135Context(options);
            Context.Database.EnsureCreated();
        }

        public void Dispose()
        {
            Context.Database.EnsureDeleted();
            Context.Dispose();
        }
    }

    [CollectionDefinition("Database Collection")]
    public class DatabaseCollection : ICollectionFixture<DatabaseFixture>
    {
    }
}