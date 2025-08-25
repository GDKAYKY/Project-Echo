using System;
using CSharpDatabaseAnalyzer.DatabaseDetector;
using CSharpDatabaseAnalyzer.QueryAnalyzer;
using CSharpDatabaseAnalyzer.InputHandler;

namespace CSharpDatabaseAnalyzer
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Bem-vindo ao CSharpDatabaseAnalyzer!");

            var dbDetector = new DatabaseTypeDetector();
            var queryAnalyzer = new CSharpDatabaseAnalyzer.QueryAnalyzer.QueryAnalyzer();
            var inputHandler = new CSharpDatabaseAnalyzer.InputHandler.InputHandler();

            while (true)
            {
                string input = inputHandler.GetQueryFromManualInput();

                if (string.IsNullOrWhiteSpace(input))
                {
                    continue;
                }

                if (input.ToLower() == "exit")
                {
                    break;
                }

                if (input.ToLower().StartsWith("file "))
                {
                    string filePath = input.Substring(5).Trim();
                    input = inputHandler.GetQueryFromFile(filePath);
                    if (input == null)
                    {
                        continue;
                    }
                }

                Console.WriteLine($"\nQuery/String de Conexão recebida: {input}");

                // Detecção do tipo de banco de dados (usando a query como string de conexão para simplificar a demo)
                string dbType = dbDetector.DetectDatabaseType(input);
                Console.WriteLine($"Tipo de Banco de Dados Detectado: {dbType}");

                // Análise da query
                var analysisResult = queryAnalyzer.AnalyzeQuery(input);
                Console.WriteLine("Análise da Query:");
                foreach (var item in analysisResult)
                {
                    Console.WriteLine($"  {item.Key}: {item.Value}");
                }
                Console.WriteLine("----------------------------------------");
            }

            Console.WriteLine("Obrigado por usar o CSharpDatabaseAnalyzer!");
        }
    }
}
