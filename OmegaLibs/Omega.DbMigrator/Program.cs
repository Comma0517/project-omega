using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using Dapper;
using DbUp;
using DbUp.Engine;
using EnvironmentSettings.Logic;
using Omega.Plumbing;
using Omega.Utils;

namespace Omega.DbMigrator
{
    class Program
    {
        private const string DEFAULT_DB_NAME = "Omega";
        private static readonly EnvSettings _envSettings = new(new EnvironmentVariableProvider());
        private static readonly ConnectionStringProvider _connStringProvider = new(_envSettings);

        static int Main(string[] args)
        {
            try
            {
                DotEnv.Load();
                _envSettings.AddSettings<GlobalSettings>();
                Console.WriteLine(_envSettings.GetAllAsSafeLogString());

                // Reset option is only meant for development
                if (args.Length > 0 && args[0] == "deleteAll")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Are you sure you want to completely drop all Omega databases? (y/n)");
                    Console.ResetColor();
                    var input = Console.ReadKey();
                    Console.WriteLine("");
                    if (input.KeyChar != 'y' && input.KeyChar != 'Y')
                    {
                        Console.WriteLine("Aborting");
                        return 0;
                    }

                    return ResetAllDatabases();
                }

                return DoAllMigrations();
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Unhandled exception running DB migrations");
                Console.WriteLine(e);
                Console.ResetColor();
                return 1;
            }
        }

        private static int DoAllMigrations()
        {
            var serviceWithDbInfos = GetServiceWithDbInfos();

            foreach (var serviceWithDbInfo in serviceWithDbInfos)
            {
                Console.WriteLine($"Ensuring the database {serviceWithDbInfo.DbInfo.DbName} exists");
                EnsureDatabase.For.SqlDatabase(BuildConnectionString(serviceWithDbInfo.DbInfo));
            }

            List<DatabaseUpgradeResult> results = new();

            // We can parallelize this if we start to get a lot of services, or some are
            // slow because so many scripts have built up over time for each service.
            foreach (var serviceWithDbInfo in serviceWithDbInfos)
            {
                Console.WriteLine($"\nRunning migrations for service {serviceWithDbInfo.ServiceName}...\n");
                results.Add(DoMigrations(serviceWithDbInfo));
            }

            // We could show some more specific results for each result here,
            // but for now we're relying on the default logging.
            // foreach (var result in results) { }

            return results.Any(r => !r.Successful) ? 1 : 0;
        }

        private static DatabaseUpgradeResult DoMigrations(ServiceWithDbInfo serviceWithDbInfo)
        {
            var connectionString = BuildConnectionString(serviceWithDbInfo.DbInfo);

            EnsureDatabase.For.SqlDatabase(connectionString);

            // Note: if there is a use-case to embed other sql scripts not related to migrations in the services, we would need
            // to use a folder convention and manually load only the specific scripts that we want to be DB migrations. 
            var upgrader = DeployChanges.To
                .SqlDatabase(connectionString)
                .WithScriptsEmbeddedInAssembly(serviceWithDbInfo.Service.Assembly)
                .WithTransaction()
                .LogToConsole()
                .Build();

            return upgrader.PerformUpgrade();
        }

        private static int ResetAllDatabases()
        {
            var serviceWithDbInfos = GetServiceWithDbInfos();

            Console.WriteLine("Deleting all databases...");

            Console.WriteLine("");
            using (var connection = new SqlConnection(BuildMasterConnectionString()))
            {
                foreach (var serviceWithDbInfo in serviceWithDbInfos)
                {
                    string dbName = serviceWithDbInfo.DbInfo.DbName;

                    var dbId = connection.ExecuteScalar($"SELECT db_id('{dbName}')");
                    Console.WriteLine($"Service {serviceWithDbInfo.ServiceName} with database {dbName} has dbId of {dbId ?? "null"}");
                    if (dbId == null)
                    {
                        Console.WriteLine($"No database to delete");
                    }
                    else
                    {
                        Console.WriteLine("Deleting database " + dbName);
                        connection.Execute($"DROP DATABASE {dbName}");
                    }

                    Console.WriteLine("");
                }
            }

            Console.WriteLine("All databases deleted");

            return 0;
        }

        private static List<ServiceWithDbInfo> GetServiceWithDbInfos()
        {
            List<ServiceWithDbInfo> serviceWithDbInfos = new();

            var services = new OmegaServiceRegistration().LoadOmegaServices(Environment.GetEnvironmentVariable("SERVICE_KEY"));

            foreach (var service in services)
            {
                var serviceWithDbInfo = new ServiceWithDbInfo {ServiceName = service.GetType().Name, Service = service};
                var dbInfo = (DbInfo) service.GetType().GetCustomAttribute(typeof(DbInfo));
                serviceWithDbInfo.DbInfo = dbInfo ?? new DbInfo {DbName = DEFAULT_DB_NAME};
                serviceWithDbInfos.Add(serviceWithDbInfo);
            }

            return serviceWithDbInfos;
        }

        private static string BuildConnectionString(DbInfo dbInfo)
        {
            return _connStringProvider.GetConnectionString(dbInfo.DbName);
        }

        private static string BuildMasterConnectionString()
        {
            return _connStringProvider.GetConnectionString("master");
        }
    }

    internal class ServiceWithDbInfo
    {
        public string ServiceName { get; init; }
        public ProjectOmegaService Service { get; init; }
        public DbInfo DbInfo { get; set; }
    }
}
