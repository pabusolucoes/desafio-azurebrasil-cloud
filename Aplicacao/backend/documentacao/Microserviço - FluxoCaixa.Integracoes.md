# 🚀 Microserviço de Integrações

## 📝 Descrição

O microserviço **fluxo_caixa_integracoes** é responsável por processar e armazenar os dados de lançamentos consolidados, garantindo a comunicação assíncrona entre os microserviços do sistema.

## 🔥 Funcionalidades

- 📥 **Receber mensagens de lançamentos**: Consome mensagens enviadas pelo microserviço de lançamentos via RabbitMQ.
- 🔄 **Processar consolidado diário**: Armazena e atualiza os dados no DynamoDB, garantindo que as informações sejam persistidas corretamente.
- 📊 **Consultar lançamentos armazenados**: Retorna os lançamentos já processados e armazenados no banco de dados.

## 🌐 Endpoints

### 📥 Receber mensagens de lançamentos

**Este microserviço funciona de forma assíncrona e não possui endpoints de criação de dados, pois recebe eventos via RabbitMQ.**

### 📊 Consultar lançamentos armazenados

**GET** `/integracoes/lancamentos`

- Retorna todos os lançamentos que foram processados e armazenados.

### 🔄 Reprocessar lançamentos

**POST** `/integracoes/reprocessar`

- Reprocessa os lançamentos armazenados para garantir a consistência dos dados.

## 🛠 Tecnologias Utilizadas

- ⚡ .NET 8 Minimal API
- 🐳 Docker
- 📨 RabbitMQ (para comunicação assíncrona)
- 📦 DynamoDB (armazenamento)

## ⚙️ Configuração

- Variáveis de ambiente necessárias:

  ```env
  RABBITMQ_HOST=localhost
  DYNAMODB_TABLE=fluxo_caixa_integracoes
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

- 📊 **Consulta de lançamentos armazenados**
  - Testa a recuperação dos lançamentos armazenados no DynamoDB.
  - Verifica se a resposta contém os campos esperados.
  - Mede o tempo de resposta.
- 🔄 **Reprocessamento de lançamentos**
  - Garante que os lançamentos podem ser reprocessados corretamente.
  - Valida o código de resposta da API.
  - Confirma a atualização dos dados no banco de dados.

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
   - `FluxoCaixa-Integracoes API.postman_collection.json`

5. ✅ Após a importação, os testes estarão disponíveis no Postman para execução.

## 🗂 Testes de Integração - Arquivos Postman

Os arquivos JSON dos testes podem ser encontrados no repositório Git:

- 📂 [Environment Docker](https://chatgpt.com/c/backend/Tests/IntegrationTests_Postman/Docker.postman_environment.json)
- 📂 [Environment Local](https://chatgpt.com/c/backend/Tests/IntegrationTests_Postman/Local.postman_environment.json)
- 📂 [Coleção de Testes do Postman](https://chatgpt.com/c/backend/Tests/IntegrationTests_Postman/FluxoCaixa-Integracoes API.postman_collection.json)