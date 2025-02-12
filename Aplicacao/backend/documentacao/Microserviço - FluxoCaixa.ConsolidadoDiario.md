# 🚀 Microserviço de Consolidado Diário

## 📝 Descrição

O microserviço **fluxo_caixa_consolidado** é responsável por consolidar diariamente os lançamentos financeiros e disponibilizar relatórios de fechamento do dia.

## 🔥 Funcionalidades

- 📊 **Obter consolidado diário**: Retorna os valores consolidados do fluxo de caixa de um dia específico.
- 📅 **Consultar consolidado por data**: Permite buscar o consolidado diário filtrando por uma data específica.
- 🔄 **Reprocessar consolidado diário**: Recalcula os valores consolidados para um determinado período, garantindo a consistência dos dados.

## 🌐 Endpoints

### 📊 Obter consolidado diário

**GET** `/consolidado-diario`

- Retorna os valores consolidados do dia atual.

### 📅 Consultar consolidado por data

**GET** `/consolidado-diario/{data}`

- Retorna os valores consolidados para a data informada.

### 🔄 Reprocessar consolidado diário

**POST** `/consolidado-diario/reprocessar`

- Recalcula os valores consolidados, garantindo a atualização dos dados.

## 🛠 Tecnologias Utilizadas

- ⚡ .NET 8 Minimal API
- 🐳 Docker
- 📨 RabbitMQ (para comunicação assíncrona)
- 📦 DynamoDB (armazenamento)

## ⚙️ Configuração

- Variáveis de ambiente necessárias:

  ```env
  RABBITMQ_HOST=localhost
  DYNAMODB_TABLE=fluxo_caixa_consolidado
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

- 🔄 **Reprocessamento do Consolidado Diário**
  - Verifica se a API responde com código 200 ou 201.
  - Confirma que a resposta está no formato JSON.
  - Mede o tempo de resposta, garantindo que seja inferior a 1 segundo.
  - Confirma que a mensagem de sucesso é "Reprocessamento iniciado.".
- 📅 **Consulta de consolidado diário**
  - Testa a recuperação dos valores consolidados para uma data específica.
  - Verifica se a resposta contém os campos esperados.
  - Mede o tempo de resposta.

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