using System;
using System.IO;

namespace CSharpDatabaseAnalyzer.InputHandler
{
    public class InputHandler
    {
        public string GetQueryFromManualInput()
        {
            Console.WriteLine("Digite sua query SQL (ou 'exit' para sair, 'file <caminho_do_arquivo>' para ler de um arquivo):");
            return Console.ReadLine();
        }

        public string GetQueryFromFile(string filePath)
        {
            try
            {
                return File.ReadAllText(filePath);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine($"Erro: Arquivo não encontrado em {filePath}");
                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erro ao ler o arquivo: {ex.Message}");
                return null;
            }
        }
    }
}

