Storing Secrets in SQL Server
=====================================

In a real world implementation, you probably want to store your secrets in a persistent store. This could be anything from a simple file based implementation to a Redis cache, MongoDB or whatever, as long as it implements ``ISecretStore``.

A default SQL Server storage provider is avaiable for your use, follow these steps to set it up:

=====================================
Install Passaword.Storage.Sql Package
=====================================

Install the required package.

``Install-Package Passaword.Storage.Sql``

=====================================
Add the connection string
=====================================

Add the connection string to your appsettings.json file.

::

	"ConnectionStrings": {
		"Passaword": "Data Source=.\\SQLEXPRESS;Initial Catalog=Passaword;Integrated Security=True"
	}

=====================================
Add the dependency
=====================================

In ``Startup.cs`` replace ``AddInMemorySecretStore`` with ``AddSqlSecretStore`` and add ``using Passaword.Storage.Sql;``

=====================================
Initialize database
=====================================

Migrations are included with each build of the package. To ensure your database is always up to date, you can include the call to ``Migrate`` in the ``Startup.cs`` ``ConfigureServices`` call::

	var sp = services.BuildServiceProvider();
    using (var db = sp.GetService<PassawordDbContext>())
    {
        db.Database.Migrate();
    }

Alternatively you can use the ``update-database`` command in the Package Manager Console. Read more: https://docs.microsoft.com/en-us/ef/core/managing-schemas/migrations/