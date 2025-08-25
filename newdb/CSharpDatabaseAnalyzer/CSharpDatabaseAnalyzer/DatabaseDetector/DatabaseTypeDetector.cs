namespace CSharpDatabaseAnalyzer.DatabaseDetector
{
    public class DatabaseTypeDetector
    {
        public string DetectDatabaseType(string connectionString)
        {
            string lowerCaseConnectionString = connectionString.ToLower();

            // MySQL
            if (lowerCaseConnectionString.Contains("mysql") || (lowerCaseConnectionString.Contains("server=") && lowerCaseConnectionString.Contains("port=3306")) || lowerCaseConnectionString.Contains("uid=") && lowerCaseConnectionString.Contains("pwd="))
            {
                return "MySQL";
            }
            // PostgreSQL
            else if (lowerCaseConnectionString.Contains("postgresql") || (lowerCaseConnectionString.Contains("host=") && lowerCaseConnectionString.Contains("port=5432")) || lowerCaseConnectionString.Contains("username=") && lowerCaseConnectionString.Contains("password="))
            {
                return "PostgreSQL";
            }
            // Oracle
            else if (lowerCaseConnectionString.Contains("oracle") || (lowerCaseConnectionString.Contains("data source=") && lowerCaseConnectionString.Contains("port=1521")) || lowerCaseConnectionString.Contains("user id=") && lowerCaseConnectionString.Contains("initial catalog="))
            {
                return "Oracle";
            }
            // SQL Server
            else if (lowerCaseConnectionString.Contains("sqlserver") || (lowerCaseConnectionString.Contains("server=") && lowerCaseConnectionString.Contains("database=")) || lowerCaseConnectionString.Contains("data source=") && lowerCaseConnectionString.Contains("initial catalog="))
            {
                return "SQL Server";
            }
            else
            {
                return "Unknown";
            }
        }
    }
}

