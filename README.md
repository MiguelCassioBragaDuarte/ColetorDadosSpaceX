# 🚀 Coletor de Dados SpaceX & Integração em Nuvem  
### Aplicação Desktop WPF com Arquitetura MVVM, SQLite e Integração Batch

---

# 📖 Visão Geral

Este projeto simula um ecossistema corporativo de integração de dados distribuídos, funcionando como uma ponte ETL (*Extract, Transform and Load*) entre uma API pública e uma infraestrutura em nuvem.

O sistema realiza:

- 📥 **Extração** de dados públicos da API oficial da SpaceX
- 🔄 **Transformação** e tratamento dos dados recebidos
- 💾 **Persistência local** utilizando SQLite (*fallback offline*)
- ☁️ **Integração em lote (Batch)** com a API de outro sistema parceiro

O objetivo principal é demonstrar conceitos modernos de:

- Arquitetura MVVM
- Comunicação entre microsserviços
- Persistência local resiliente
- Integração HTTP assíncrona
- Processamento em lote
- Tratamento robusto de falhas

---

# 🛠️ Tecnologias Utilizadas

| Tecnologia | Finalidade |
|---|---|
| **.NET 8 / .NET 6** | Plataforma principal da aplicação |
| **WPF (Windows Presentation Foundation)** | Interface gráfica desktop |
| **MVVM** | Separação entre UI e lógica de negócio |
| **SQLite** | Banco de dados local embarcado |
| **Microsoft.Data.Sqlite** | Driver de acesso ao SQLite |
| **HttpClient** | Comunicação HTTP assíncrona |
| **System.Text.Json** | Serialização e desserialização JSON |
| **ObservableCollection** | Atualização reativa da interface |
| **INotifyPropertyChanged** | Atualização dinâmica da UI |

---

# 🏗️ Estrutura do Projeto

```text
ColetorDadosSpaceX/
│
├── Data/
│   └── DataBase.cs
│
├── Models/
│   ├── Launch.cs
│   ├── Rocket.cs
│   └── Stats.cs
│
├── Services/
│   ├── SpaceXApiService.cs
│   └── EnviaDados.cs
│
├── ViewModels/
│   └── MainViewModel.cs
│
└── Views/
    ├── MainWindow.xaml
    └── MainWindow.xaml.cs
```

---

# 📂 Explicação das Pastas e Arquivos

## 📁 Data

### `DataBase.cs`

Responsável pelo gerenciamento do banco SQLite local.

### Funções principais:
- Criar automaticamente o banco local
- Gerenciar conexões
- Executar scripts SQL
- Criar tabelas caso não existam

### Estratégia utilizada:
O banco é criado dentro de:

```text
%localappdata%/SpaceXApp
```

Isso evita problemas de permissão administrativa no Windows.

### Tabelas criadas:
- `Launch`
- `Rocket`
- `Stats`

---

## 📁 Models

### `Launch.cs`

Representa os dados de lançamentos da SpaceX.

```csharp
public class Launch
{
    public string Id { get; set; }
    public string Name { get; set; }
    public DateTime DateUtc { get; set; }
    public bool Success { get; set; }
    public string Details { get; set; }
}
```

---

### `Rocket.cs`

Representa os dados dos foguetes.

```csharp
public class Rocket
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public bool Active { get; set; }
    public int SuccessRatePct { get; set; }
}
```

---

### `Stats.cs`

Armazena métricas calculadas para exibição na interface.

Exemplos:
- Total de lançamentos
- Quantidade de sucessos
- Quantidade de falhas
- Taxa percentual de sucesso

---

## 📁 Services

### `SpaceXApiService.cs`

Serviço responsável pela comunicação com a API pública da SpaceX.

### Responsabilidades:
- Executar requisições HTTP GET
- Consumir endpoints REST
- Converter JSON em objetos C#

### Tecnologias utilizadas:
- `HttpClient`
- `GetFromJsonAsync<T>()`
- `async/await`

---

### `EnviaDados.cs`

Serviço responsável pela integração com a API do Aluno 2.

### Responsabilidades:
- Ler dados locais
- Transformar payloads
- Reformatar JSON
- Enviar dados em lote (*Batch*)

---

## 📁 ViewModels

### `MainViewModel.cs`

Camada principal da lógica da aplicação.

Implementa:

```csharp
INotifyPropertyChanged
```

### Responsabilidades:
- Gerenciar estado da UI
- Carregar dados da API
- Gerenciar fallback offline
- Atualizar listas observáveis
- Coordenar sincronizações

### Estratégia Offline

Caso a API pública falhe:

1. O sistema detecta a indisponibilidade
2. Acessa o SQLite local
3. Recupera o histórico salvo
4. Atualiza automaticamente a interface

---

## 📁 Views

### `MainWindow.xaml`

Define toda a interface gráfica da aplicação.

### Componentes principais:
- `TabControl`
- Tabelas de dados
- Painel de estatísticas
- Botões de sincronização
- Botão de envio em lote

---

### `MainWindow.xaml.cs`

Responsável pelos eventos da janela.

### Funções:
- Captura de cliques
- Controle temporário de botões
- Exibição de mensagens (`MessageBox`)
- Prevenção de cliques duplicados

---

# 🌐 Comunicação com a API da SpaceX

O sistema consome dados diretamente da API oficial da SpaceX utilizando chamadas REST assíncronas.

## Endpoints utilizados

### 🚀 Lançamentos

```http
GET https://api.spacexdata.com/v5/launches
```

### 🚀 Foguetes

```http
GET https://api.spacexdata.com/v4/rockets
```

---

## Exemplo de consumo

```csharp
var launches = await httpClient
    .GetFromJsonAsync<List<Launch>>(url);
```

---

# 💾 Persistência Local com SQLite

Após o download dos dados, o sistema salva automaticamente as informações no banco local.

---

## Estratégia de Performance

A aplicação utiliza transações SQL para melhorar a velocidade de gravação:

```csharp
using (var transaction = connection.BeginTransaction())
{
    // Inserts
    transaction.Commit();
}
```

---

## Benefícios

- 🚀 Escrita extremamente rápida
- 🔒 Segurança transacional
- 📦 Inserções em lote
- 🧠 Redução de I/O de disco

---

## Estratégia de Atualização

O sistema utiliza:

```sql
INSERT OR REPLACE
```

Isso garante:

- Atualização automática
- Ausência de duplicidade
- Integridade das chaves primárias

---

# ☁️ Integração com a API do Aluno 2

A comunicação com a infraestrutura do parceiro ocorre via envio em lote (*Batch Processing*).

---

## Estratégia Batch

Ao invés de enviar centenas de requisições individuais:

❌ 1 request por registro

O sistema envia:

✅ 1 request contendo toda a coleção

---

## Benefícios

- Menor latência
- Menor uso de rede
- Menor carga no servidor
- Melhor escalabilidade

---

## Rotas utilizadas

### 🚀 Lançamentos

```http
POST /api/SpaceX/launches/batch
```

### 🚀 Foguetes

```http
POST /api/SpaceX/rockets/batch
```

---

# 📦 Exemplos de Payload JSON

## Payload de Lançamentos

```json
[
  {
    "Id": "5fe3af54f359185b56879943",
    "Name": "FalconSat",
    "Success": false,
    "Details": "Engine failure at launch"
  }
]
```

---

## Payload de Foguetes

```json
[
  {
    "Id": "5e9d0d95eda69955f709d1eb",
    "Name": "Falcon 1",
    "Description": "The Falcon 1 was an expendable launch vehicle privately developed.",
    "Active": false,
    "SuccessRatePct": 40
  }
]
```

---

# ⚠️ Tratamento de Exceções

O sistema possui tratamento robusto para falhas de comunicação HTTP.

---

## Exemplo de Validação

```csharp
if (response.IsSuccessStatusCode)
{
    Console.WriteLine("🚀 [SUCESSO] Lote enviado com sucesso!");
}
else
{
    string errorContent = await response.Content.ReadAsStringAsync();

    Console.WriteLine($"❌ Status Code: {(int)response.StatusCode}");
    Console.WriteLine($"Erro retornado: {errorContent}");
}
```

---

# 🚨 Cenários Tratados

## ❌ 404 — Endpoint inexistente

Detecta URLs incorretas ou alterações de rota.

---

## ❌ 400 — Bad Request

Detecta:
- Violação de schema
- Campos inválidos
- Dados incompatíveis

---

## ❌ Falha de conexão

Caso:
- O servidor esteja offline
- A internet caia
- O endpoint fique indisponível

O sistema:
- Captura a exceção
- Evita crash da aplicação
- Exibe mensagem amigável ao usuário

---

# 🧠 Conceitos Demonstrados no Projeto

✅ Arquitetura MVVM  
✅ Comunicação REST  
✅ SQLite embarcado  
✅ Processamento Batch  
✅ Programação Assíncrona  
✅ Fallback Offline  
✅ Serialização JSON  
✅ Tratamento de Exceções  
✅ Integração entre Microsserviços  
✅ Atualização Reativa da Interface  

---

# ▶️ Como Executar o Projeto

## 1. Clone o repositório

```bash
git clone <url-do-repositorio>
```

---

## 2. Abra no Visual Studio

Recomendado:
- Visual Studio 2022+
- .NET 8 SDK instalado

---

## 3. Execute o projeto

```bash
Ctrl + F5
```

---

# 📌 Observações Finais

Este projeto foi desenvolvido com foco em:

- Arquitetura limpa
- Separação de responsabilidades
- Escalabilidade
- Resiliência
- Simulação de integração corporativa real

A aplicação demonstra um fluxo completo de engenharia de software moderna, desde o consumo de APIs públicas até sincronização distribuída entre sistemas independentes.
