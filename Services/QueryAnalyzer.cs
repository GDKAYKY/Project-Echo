using System.Text.RegularExpressions;

namespace Project_Echo.Services
{
    public interface IQueryAnalyzer
    {
        Dictionary<string, bool> AnalyzeQuery(string queryText);
    }

    public class QueryAnalyzer : IQueryAnalyzer
    {
        public Dictionary<string, bool> AnalyzeQuery(string queryText)
        {
            var result = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);

            if (string.IsNullOrWhiteSpace(queryText))
            {
                return result;
            }

            string q = queryText.Trim();

            // Simple heuristics: detect CRUD verbs and risky clauses
            var patterns = new Dictionary<string, string>
            {
                { "SELECT", @"\bSELECT\b" },
                { "INSERT", @"\bINSERT\b" },
                { "UPDATE", @"\bUPDATE\b" },
                { "DELETE", @"\bDELETE\b" },
                { "DROP",   @"\bDROP\b" },
                { "ALTER",  @"\bALTER\b" },
                { "WHERE",  @"\bWHERE\b" },
                { "JOIN",   @"\bJOIN\b" },
                { "LIMIT",  @"\bLIMIT\b" },
                { "OFFSET", @"\bOFFSET\b" },
                { "ORDER BY", @"\bORDER\s+BY\b" },
                { "GROUP BY", @"\bGROUP\s+BY\b" },
                { "TRUNCATE", @"\bTRUNCATE\b" },
                { "UNION", @"\bUNION\b" },
                { "SEMICOLON", @";" }
            };

            foreach (var kv in patterns)
            {
                result[kv.Key] = Regex.IsMatch(q, kv.Value, RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
            }

            // Potentially dangerous: write operations without WHERE
            bool isUpdateOrDelete = result.GetValueOrDefault("UPDATE") || result.GetValueOrDefault("DELETE") || result.GetValueOrDefault("TRUNCATE");
            bool hasWhere = result.GetValueOrDefault("WHERE");
            result["WRITE_WITHOUT_WHERE"] = isUpdateOrDelete && !hasWhere;

            return result;
        }
    }
}


