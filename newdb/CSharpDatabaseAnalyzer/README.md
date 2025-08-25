# CSharpDatabaseAnalyzer

Este projeto é um aplicativo em C# para identificar tipos de banco de dados, analisar queries SQL e detectar informações sensíveis como CPF e nome.

## Estrutura do Projeto

- `CSharpDatabaseAnalyzer/`: Diretório raiz do projeto.
  - `CSharpDatabaseAnalyzer.csproj`: Arquivo de projeto C#.
  - `Program.cs`: Ponto de entrada principal da aplicação.
  - `README.md`: Este arquivo.
  - `DatabaseDetector/`: Módulo para detecção de tipo de banco de dados.
  - `QueryAnalyzer/`: Módulo para análise de queries e detecção de dados sensíveis.
  - `InputHandler/`: Módulo para lidar com entrada manual e via arquivo.

## Tecnologias e Bibliotecas

- **.NET 8.0**: Framework principal para desenvolvimento.
- **Regex**: Para análise de padrões em queries SQL.
- **AD.NET (System.Data.Common)**: Para manipulação de dados e conexão com bancos de dados (se necessário para detecção de tipo de DB).

## Módulos Principais

- **DatabaseDetector**: Contém lógica para identificar o tipo de SGBD (e.g., SQL Server, MySQL, PostgreSQL, Oracle) com   base em strings de conexão ou características de queries.
- **QueryAnalyzer**: Responsável por analisar queries SQL e identificar a presença de padrões que indicam busca por CPF, nome, etc.
- **InputHandler**: Gerencia a entrada de queries, seja via console (manual) ou lendo de um arquivo.

## Como Usar

(Instruções detalhadas serão adicionadas após a implementação.)


