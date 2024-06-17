namespace Budgetizer
{
        public static class Defs
        {
                public static readonly string databaseName = Environment.GetEnvironmentVariable("CosmosDbDatabaseName") ?? "";
        }
}
