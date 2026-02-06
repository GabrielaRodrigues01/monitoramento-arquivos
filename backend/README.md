# Monitoramento de Arquivos Backend MVP

Aplicação MVP para monitoramento automático da recepção de arquivos enviados por adquirentes UfCard e FagammonCard, com processamento assíncrono, validação mínima de layout, persistência em banco de dados e exposição de APIs REST para consumo por um portal web.

## Objetivo do Case MVP

A aplicação atende aos requisitos propostos, sendo capaz de:

- Recepcionar arquivos enviados por adquirentes UfCard e FagammonCard
- Registrar e armazenar informações relevantes em banco de dados SQL Server
- Realizar backup dos arquivos processados em diretório seguro
- Expor APIs REST para consulta por portal web por meio do Swagger

## Arquitetura da Solução

A solução foi organizada em camadas, seguindo princípios de separação de responsabilidades e arquitetura em camadas.

### MonitoramentoArquivos.Domain

Camada de domínio puro, sem dependências externas.

Contém:

#### Entities
- FileReceipt: entidade principal persistida no banco

#### Enums
- AcquirerType: UfCard e FagammonCard
- FileReceiptStatus: Recepcionado e NãoRecepcionado

### MonitoramentoArquivos.Application

Camada responsável pelas regras de negócio e orquestração do processamento.

Contém:

#### Contracts
- IFileReceiptParser: contrato para parsers de layout

#### Services
- FileReceiptIngestionService: responsável por orquestrar parsing, validação e geração do resultado
- FileReceiptLineParser: parser de linha fixed width

#### Models
- IngestionResult: resultado do processamento do arquivo

#### Exceptions
- LayoutParseException: erro de layout ou validação mínima

Importante: esta camada não possui dependência direta de Entity Framework Core ou infraestrutura.

### MonitoramentoArquivos.Infrastructure

Camada de infraestrutura e persistência.

Contém:

#### Persistence
- AppDbContext (Entity Framework Core)
- Configurações de entidades
- Migrations

#### Repositories
- IFileReceiptRepository
- FileReceiptRepository

#### DependencyInjection
- Registro de serviços e DbContext

### MonitoramentoArquivos.Api

Camada de entrada da aplicação, composta por API REST e worker em background.

Contém:

#### Controllers
- FileReceiptsController: listagem e consulta de arquivos
- DashboardController: dados agregados para dashboard e gráfico

#### Worker
- FolderMonitorHostedService
  - Monitora a pasta inbox
  - Processa arquivos de forma assíncrona

#### Configuração
- Program.cs
- appsettings.json
- Swagger OpenAPI

## Tecnologias Utilizadas

- .NET 6
- ASP.NET Core Web API
- Entity Framework Core 6
- SQL Server ou LocalDB
- BackgroundService com FileSystemWatcher
- Swagger OpenAPI

## Pré-requisitos

Antes de rodar o projeto, verificar:

- .NET SDK 6.x
  ```bash
  dotnet --version

- SQL Server LocalDB recomendado ou SQL Server instalado  
  ```bash
  sqllocaldb info

- Como baixar o projeto
  ```bash
  git clone <URL_DO_REPOSITORIO>
  cd MonitoramentoArquivos

- Configurações de Connection String
- Arquivo:
  ```bash
  MonitoramentoArquivos.Api/appsettings.Development.json

- Exemplo usando LocalDB:
  ```bash
  {
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=MonitoramentoArquivosDb;Trusted_Connection=True;MultipleActiveResultSets=true"
   }
  }

- Pastas de ingestão
  ```bash
  {
  "FileIngestion": {
    "InboxPath": "C:\\Monitoramento Arquivos\\storage\\inbox",
    "BackupPath": "C:\\Monitoramento Arquivos\\storage\\backup",
    "RejectedPath": "C:\\Monitoramento Arquivos\\storage\\rejected",
    "FileFilter": "*.txt"
    }
  }
  Observação: as pastas são criadas automaticamente caso não existam.

- Como rodar a API
  ```bash
  Na raiz da solution onde está o arquivo .sln:
  dotnet run --project MonitoramentoArquivos.Api

- Logs esperados:
  ```bash
  Now listening on: https://localhost:7042
  Monitoramento iniciado em C:\Monitoramento Arquivos\storage\inbox

- Swagger
  ```bash
  Disponível em:
  https://localhost:7042/swagger

- Banco de Dados e Migrations
  ```bash
  Criar migration se necessário:
  dotnet ef migrations add InitialCreate \
  --project MonitoramentoArquivos.Infrastructure \
  --startup-project MonitoramentoArquivos.Api \
  --output-dir Persistence/Migrations

  - Aplicar migration:
  dotnet ef database update \
  --project MonitoramentoArquivos.Infrastructure \
  --startup-project MonitoramentoArquivos.Api

- Resultado esperado:
  - Banco MonitoramentoArquivosDb
  - Tabela FileReceipts
  - Índice único em FileHash para deduplicação
 
  #### Fluxo de Funcionamento – Visão Operacional
- Arquivo é colocado em storage/inbox
- Worker detecta o arquivo usando FileSystemWatcher
- Aguarda o arquivo estar pronto e não sendo escrito
- Lê o conteúdo no formato fixed width
- Executa validação mínima de layout
- Gera FileHash para deduplicação
  
Se o hash já existir:
- Não reprocessa
- Move o arquivo para backup
  
Se processar:
- Salva o registro no banco
- Em caso de sucesso move para backup
- Em caso de falha move para rejected
As APIs expõem os dados para consulta.

## Testes Manuais – Cenários Principais

Teste 1 – Fagammon válido
- Arquivo:
  ```bash
  fagammon_ok_01.txt

- Conteúdo:
  ```bash
  12019052632165487FagammonCard0002451

- Resultado esperado:
  - Sucesso true
  - Registro salvo no banco
  - Arquivo movido para backup
 
  Teste 2 – UfCard inválido
- Conteúdo inválido com tamanho menor que o esperado:
  ```bash
  009875643212019062620190625201906250000001Uf

- Resultado esperado:
  - Sucesso false
  - Status NãoRecepcionado
  - ErrorMessage preenchido
  - Arquivo movido para rejected
 
   Teste 3 – Deduplicação por hash
- Mesmo conteúdo de um arquivo já processado:

- Resultado esperado:
  - Log indicando arquivo duplicado
  - Arquivo movido para backup
  - Nenhum novo registro duplicado inserido
 
  ## Endpoints Principais
Listagem de arquivos:
```bash
GET /api/FileReceipts
```
Dashboard:
```bash
GET /api/Dashboard/summary
```
- Observações Importantes:
  - Erro de DLL em uso durante build indica que a API está em execução.
  Solução:
  - Encerrar a API com Ctrl C
  - Ou finalizar o processo MonitoramentoArquivos.Api
  - Executar dotnet build novamente
 
  ## Diagrama de Fluxo (Visão Técnica)

  ```bash
  ┌──────────────┐
  │ Adquirente   │
  │ (Uf/Fagammon)│
  └──────┬───────┘
         │
         ▼
  ┌──────────────────────┐
  │ storage/inbox        │
  └────────┬─────────────┘
           │
           ▼
  ┌────────────────────────────┐
  │ FolderMonitorHostedService │
  │ (Worker / Background)      │
  └────────┬───────────────────┘
           │
           ▼
  ┌────────────────────────────┐
  │ FileReceiptIngestionService│
  │ - Validação mínima         │
  │ - Parser fixed-width       │
  │ - Geração de hash          │
  └────────┬───────────────────┘
           │
           ▼
  ┌────────────────────────────┐
  │ Banco de Dados             │
  │ FileReceipts               │
  └────────┬───────────────────┘
     ├── Sucesso ──▶ storage/backup
     └── Falha   ──▶ storage/rejected
  ```




 
  









 


 
 
  

 

  

  

  

  

  
  
   

  



  



  






 
