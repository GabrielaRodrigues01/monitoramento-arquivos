# Monitoramento de Arquivos

Projeto full stack para monitoramento automático da recepção e processamento de arquivos enviados por adquirentes, com persistência em banco de dados e visualização via portal web.

O MVP é composto por uma API backend em .NET responsável pelo processamento e ingestão dos arquivos, e uma aplicação frontend em Angular para visualização de status, listagens e dashboards.

## Visão Geral da Arquitetura

O projeto é dividido em dois módulos principais:

1. Backend  
API desenvolvida em .NET responsável por:
- Monitoramento de diretórios
- Processamento assíncrono de arquivos
- Validação mínima de layout
- Deduplicação por hash
- Persistência em banco de dados
- Exposição de APIs REST

2. Frontend  
Aplicação Angular responsável por:
- Listagem de arquivos processados
- Exibição de status e erros
- Dashboard com gráficos
- Consumo exclusivo das APIs REST

Estrutura do repositório:

```
/
├── backend
│   └── README.md
├── frontend
│   └── monitoramento-arquivos-web
│       └── README.md
└── README.md
```

## Tecnologias Utilizadas

Backend:
- .NET 6
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server ou LocalDB
- BackgroundService com FileSystemWatcher
- Swagger OpenAPI

Frontend:
- Angular
- TypeScript
- SCSS
- Angular Router
- HttpClient
- Chart.js

## Como rodar o projeto completo

1. Subir o backend  
Siga as instruções disponíveis no README do backend:

```
backend/README.md
```

2. Subir o frontend  
Após o backend estar em execução, siga as instruções do README do frontend:

```
frontend/monitoramento-arquivos-web/README.md
```

O frontend consome a API local via proxy configurado.

## Acessos locais

API Swagger:
```
https://localhost:7042/swagger
```

Frontend:
```
http://localhost:4200
```

## Observações

Este projeto foi desenvolvido como um case técnico com foco em:
- Organização de arquitetura
- Separação de responsabilidades
- Clareza de código
- Simplicidade de execução
- Facilidade de leitura e manutenção
