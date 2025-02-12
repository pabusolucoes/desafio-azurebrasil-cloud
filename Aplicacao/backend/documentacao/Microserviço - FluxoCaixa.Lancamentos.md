# 🚀 Microserviço de Lançamentos

## 📝 Descrição

O microserviço **fluxo_caixa_lancamentos** é responsável pelo gerenciamento dos lançamentos financeiros no fluxo de caixa.

## 🔥 Funcionalidades

- 🆕 **Criar um lançamento**: Permite adicionar um novo lançamento financeiro no fluxo de caixa.
- 📋 **Listar lançamentos**: Retorna todos os lançamentos cadastrados no sistema.
- 🔍 **Obter um lançamento específico**: Recupera os detalhes de um lançamento por meio do seu identificador único (ID).
- ✏️ **Atualizar um lançamento**: Modifica os detalhes de um lançamento existente.
- ❌ **Remover um lançamento**: Exclui um lançamento específico do sistema.

## 🌐 Endpoints

### ➕ Criar um lançamento

**POST** `/fluxo-caixa/lancamentos`

- Cria um novo lançamento financeiro.

### 📜 Listar lançamentos

**GET** `/fluxo-caixa/lancamentos`

- Retorna todos os lançamentos cadastrados.

### 🔎 Obter um lançamento específico

**GET** `/fluxo-caixa/lancamentos/{id}`

- Retorna os detalhes de um lançamento pelo ID.

### 🔄 Atualizar um lançamento

**PUT** `/fluxo-caixa/lancamentos/{id}`

- Atualiza os detalhes de um lançamento.

### 🗑️ Remover um lançamento

**DELETE** `/fluxo-caixa/lancamentos/{id}`

- Remove um lançamento pelo ID.

## 🛠 Tecnologias Utilizadas

- ⚡ .NET 8 Minimal API
- 🐳 Docker
- 📨 RabbitMQ (para comunicação assíncrona)
- 📦 DynamoDB (armazenamento)

## ⚙️ Configuração

- Variáveis de ambiente necessárias:

  ```env
  RABBITMQ_HOST=localhost
  DYNAMODB_TABLE=fluxo_caixa_lancamentos
  ```

- Para rodar localmente:

  ```sh
  dotnet run
  ```

## ✅ Como Executar os Testes

- Certifique-se de que todas as dependências estão instaladas e configuradas corretamente.

- Execute os testes unitários utilizando o comando:

  ```sh
  dotnet test
  ```

- Caso esteja rodando dentro de um container Docker, utilize:

  ```sh
  docker exec -it <nome-do-container> dotnet test
  ```

## 🔬 Testes de Integração

Os testes de integração executados no Postman incluem:

- 📊 **Testes de Lançamentos**

### 📥 Como Importar os Arquivos do Postman

Para importar os arquivos de testes no Postman, siga os passos abaixo:

1. 🏁 Abra o **Postman**.

2. 📂 No menu lateral, clique em **Import**.

3. 📑 Selecione a aba **File**.

4. ⬆️ Clique em 

   Upload Files

    e selecione os arquivos JSON listados abaixo:

   - `Docker.postman_environment.json`
   - `Local.postman_environment.json`
   - `FluxoCaixa-ConsolidadoDiario API.postman_collection.json`

5. ✅ Após a importação, os testes estarão disponíveis no Postman para execução.

## 🗂 Testes de Integração - Arquivos Postman

Os arquivos JSON dos testes podem ser encontrados no repositório Git:

- 📂 Environment Docker
- 📂 Environment Local
- 📂 Coleção de Testes do Postman