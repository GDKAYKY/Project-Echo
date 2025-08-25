using System.Text.RegularExpressions;

namespace CSharpDatabaseAnalyzer.QueryAnalyzer
{
    public class QueryAnalyzer
    {
        public Dictionary<string, bool> AnalyzeQuery(string query)
        {
            var analysisResult = new Dictionary<string, bool>();

            // Padrões para CPF, Nome e outros
            // Simplificado para demonstração. Em um cenário real, seria mais robusto.
            analysisResult["ContainsCPF"] = Regex.IsMatch(query, @"\b(cpf|cadastro_pessoa_fisica)\b", RegexOptions.IgnoreCase);
            analysisResult["ContainsName"] = Regex.IsMatch(query, @"\b(nome|primeiro_nome|sobrenome)\b", RegexOptions.IgnoreCase);
            analysisResult["ContainsEmail"] = Regex.IsMatch(query, @"\b(email|e-mail)\b", RegexOptions.IgnoreCase);
            analysisResult["ContainsAddress"] = Regex.IsMatch(query, @"\b(endereco|address)\b", RegexOptions.IgnoreCase);

            return analysisResult;
        }
    }
}

