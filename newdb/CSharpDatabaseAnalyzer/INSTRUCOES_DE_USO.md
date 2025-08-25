# Instruções de Uso - CSharpDatabaseAnalyzer

## Pré-requisitos
- .NET 8.0 Runtime ou SDK instalado no sistema

## Como Executar

### Opção 1: Executar a partir do código-fonte
1. Navegue até o diretório `CSharpDatabaseAnalyzer/CSharpDatabaseAnalyzer`
2. Execute o comando: `dotnet run`

### Opção 2: Executar a partir do pacote publicado
1. Navegue até o diretório `publish`
2. Execute o comando: `dotnet CSharpDatabaseAnalyzer.dll`

## Como Usar

### 1. Entrada Manual
- Digite sua query SQL ou string de conexão diretamente no console
- Pressione Enter para analisar
- Digite "exit" para sair do programa

### 2. Entrada via Arquivo
- Digite: `file <caminho_do_arquivo>`
- Exemplo: `file /home/ubuntu/CSharpDatabaseAnalyzer/test_queries.txt`
- O programa lerá e analisará todo o conteúdo do arquivo

## Exemplos de Strings de Conexão

### SQL Server
```
Data Source=myServer;Initial Catalog=myDatabase;Integrated Security=True;
```

### MySQL
```
Server=localhost;Port=3306;Database=testdb;Uid=root;Pwd=password;
```

### PostgreSQL
```
Host=localhost;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword;
```

### Oracle
```
Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=myhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=myservicename)));User Id=myuser;Password=mypassword;
```

## Exemplos de Queries SQL

### Query com CPF
```sql
SELECT * FROM users WHERE cpf = '123.456.789-00';
```

### Query com Nome e Email
```sql
SELECT id, nome, email FROM clientes WHERE nome LIKE '%João%';
```

### Query com Endereço
```sql
UPDATE clientes SET endereco = 'Rua das Flores, 123' WHERE id = 1;
```

## Saída do Programa

O programa exibirá:
1. **Tipo de Banco de Dados Detectado**: SQL Server, MySQL, PostgreSQL, Oracle ou Unknown
2. **Análise da Query**: Indica se a query contém referências a:
   - CPF (ContainsCPF)
   - Nome (ContainsName)
   - Email (ContainsEmail)
   - Endereço (ContainsAddress)

## Estrutura do Projeto

```
CSharpDatabaseAnalyzer/
├── CSharpDatabaseAnalyzer/
│   ├── DatabaseDetector/
│   │   └── DatabaseTypeDetector.cs
│   ├── QueryAnalyzer/
│   │   └── QueryAnalyzer.cs
│   ├── InputHandler/
│   │   └── InputHandler.cs
│   └── Program.cs
├── publish/                    # Aplicativo publicado
├── test_queries.txt           # Arquivo de exemplo para testes
├── README.md                  # Documentação do projeto
├── DEMO.md                    # Demonstração das funcionalidades
└── INSTRUCOES_DE_USO.md       # Este arquivo
```

## Extensibilidade

O aplicativo foi projetado para ser facilmente extensível:

1. **Novos tipos de banco de dados**: Adicione novos padrões na classe `DatabaseTypeDetector`
2. **Novos tipos de dados**: Adicione novos padrões regex na classe `QueryAnalyzer`
3. **Novas funcionalidades**: Crie novos módulos seguindo a estrutura existente

