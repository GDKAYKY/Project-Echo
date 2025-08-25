# Demonstração do CSharpDatabaseAnalyzer

## Funcionalidades Implementadas

### 1. Detecção de Tipos de Banco de Dados
O aplicativo pode identificar os seguintes tipos de banco de dados com base em strings de conexão:

- **SQL Server**: Detecta strings contendo "sqlserver", "server=" com "database=", ou "data source=" com "initial catalog="
- **MySQL**: Detecta strings contendo "mysql", "server=" com "port=3306", ou "uid=" com "pwd="
- **PostgreSQL**: Detecta strings contendo "postgresql", "host=" com "port=5432", ou "username=" com "password="
- **Oracle**: Detecta strings contendo "oracle", "data source=" com "port=1521", ou "user id=" com "initial catalog="

### 2. Análise de Queries SQL
O aplicativo analisa queries SQL para detectar a presença de:

- **CPF**: Busca por padrões como "cpf" ou "cadastro_pessoa_fisica"
- **Nome**: Busca por padrões como "nome", "primeiro_nome", ou "sobrenome"
- **Email**: Busca por padrões como "email" ou "e-mail"
- **Endereço**: Busca por padrões como "endereco" ou "address"

### 3. Interface de Entrada
O aplicativo oferece duas formas de entrada:

- **Manual**: Digite queries diretamente no console
- **Arquivo**: Use o comando "file <caminho_do_arquivo>" para ler queries de um arquivo

## Exemplos de Uso

### Detecção de Banco de Dados

#### SQL Server
```
Data Source=myServer;Initial Catalog=myDatabase;Integrated Security=True;
```
**Resultado**: SQL Server

#### MySQL
```
Server=localhost;Port=3306;Database=testdb;Uid=root;Pwd=password;
```
**Resultado**: MySQL

#### PostgreSQL
```
Host=localhost;Port=5432;Database=mydatabase;Username=myuser;Password=mypassword;
```
**Resultado**: PostgreSQL

#### Oracle
```
Data Source=(DESCRIPTION=(ADDRESS=(PROTOCOL=TCP)(HOST=myhost)(PORT=1521))(CONNECT_DATA=(SERVICE_NAME=myservicename)));User Id=myuser;Password=mypassword;
```
**Resultado**: Oracle

### Análise de Queries

#### Query com CPF
```sql
SELECT * FROM users WHERE cpf = '123.456.789-00';
```
**Resultado**: ContainsCPF: True

#### Query com Nome e Email
```sql
SELECT id, nome, email FROM clientes WHERE nome LIKE '%João%';
```
**Resultado**: ContainsName: True, ContainsEmail: True

#### Leitura de Arquivo
```
file /home/ubuntu/CSharpDatabaseAnalyzer/test_queries.txt
```
**Resultado**: Analisa todas as queries do arquivo simultaneamente

## Compatibilidade
O aplicativo é compatível com os principais SGBDs do mercado e pode ser facilmente estendido para suportar outros tipos de banco de dados.

