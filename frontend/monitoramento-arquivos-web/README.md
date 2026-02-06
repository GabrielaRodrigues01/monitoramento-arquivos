# Linx Monitoramento de Arquivos Front-end

Aplicação front-end desenvolvida em Angular para o monitoramento de arquivos processados, exibindo status, listagem e dashboard com gráficos. O consumo de dados é feito exclusivamente via API REST.

## Tecnologias Utilizadas

1. Angular com standalone components  
2. TypeScript  
3. SCSS  
4. Angular Router  
5. HttpClient  
6. Chart.js com ng2-charts  
7. API REST desenvolvida em .NET  

## Como iniciar o projeto localmente

### Pré-requisitos

1. Node.js versão 18 ou superior  
2. NPM versão 9 ou superior  
3. Angular CLI instalado globalmente  

Instalação do Angular CLI:

```bash
npm install -g @angular/cli
```

### Instalação das dependências

Na raiz do projeto, execute:

```bash
npm install
```

### Subir o backend

A API deve estar rodando localmente. Exemplo:

```
http://localhost:5270
```

O front-end utiliza proxy para evitar problemas de CORS.

### Iniciar o front-end

```bash
npm start
```

A aplicação ficará disponível em:

```
http://localhost:4200
```

## Proxy de API

Arquivo de configuração:

```
proxy.conf.json
```

Configuração:

```json
{
  "/api": {
    "target": "http://localhost:5270",
    "secure": false,
    "changeOrigin": true
  }
}
```

Todas as chamadas HTTP utilizam o prefixo `/api`.

## Estrutura de Pastas

```
src/
├── app/
│   ├── core/
│   │   ├── api/
│   │   │   ├── api.config.ts
│   │   │   └── api.service.ts
│   │   └── models/
│   │       ├── file-receipt.model.ts
│   │       └── dashboard-summary.model.ts
│   │
│   ├── layout/
│   │   └── shell/
│   │       ├── shell.component.ts
│   │       ├── shell.component.html
│   │       └── shell.component.scss
│   │
│   ├── pages/
│   │   ├── dashboard/
│   │   │   ├── dashboard.component.ts
│   │   │   ├── dashboard.component.html
│   │   │   └── dashboard.component.scss
│   │   │
│   │   └── file-receipts/
│   │       ├── file-receipts.component.ts
│   │       ├── file-receipts.component.html
│   │       └── file-receipts.component.scss
│   │
│   ├── app.routes.ts
│   └── app.config.ts
│
├── assets/
│   └── linx-logo.png
│
├── styles.scss
└── main.ts
```

## Descrição das Pastas

### core

Camada central da aplicação, sem dependência de interface visual.

#### core/api

Responsável pela comunicação com a API.

Arquivo api.config.ts  
Define configurações base como baseUrl.

Arquivo api.service.ts  
Centraliza todas as chamadas HTTP da aplicação.

Regra adotada: nenhum componente utiliza HttpClient diretamente.

#### core/models

Modelos TypeScript que representam os contratos da API.

Exemplos:

FileReceipt  
DashboardSummary  

### layout

Componentes estruturais da aplicação.

#### layout/shell

Layout principal da aplicação, composto por cabeçalho e navegação.

Contém:

Logo Linx  
Menu de navegação  
router-outlet  

Funciona como o frame da aplicação.

### pages

Páginas reais da aplicação, associadas às rotas.

#### pages/dashboard

Dashboard inicial com:

Resumo de arquivos recepcionados e não recepcionados  
Gráficos utilizando Chart.js  

#### pages/file-receipts

Listagem de arquivos processados contendo:

Nome do arquivo  
Status  
Datas  
Mensagem de erro quando aplicável  

### assets

Arquivos estáticos públicos.

Imagens  
Ícones  
Logos  

Todo o conteúdo é acessível via `/assets`.

## Rotas da Aplicação

Arquivo:

```
app.routes.ts
```

Configuração:

```ts
export const routes: Routes = [
  {
    path: '',
    component: ShellComponent,
    children: [
      { path: '', pathMatch: 'full', redirectTo: 'dashboard' },
      { path: 'dashboard', component: DashboardComponent },
      { path: 'arquivos', component: FileReceiptsComponent }
    ]
  }
];
```

## Fluxo de Navegação

1. Acessa a rota raiz  
2. Redirecionamento automático para dashboard  
3. Navegação ocorre sem reload da página  
4. Dados são sempre obtidos via API  

## Comunicação com a API

Exemplo de chamada no ApiService:

```ts
getFileReceipts() {
  return this.http.get<FileReceipt[]>('/api/FileReceipts');
}
```

Todas as chamadas passam pelo proxy configurado.

Nenhuma lógica de negócio é implementada no front-end.
