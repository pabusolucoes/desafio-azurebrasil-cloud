# **📌 Requisitos do Sistema**

## **1️⃣ Requisitos Funcionais (RF)**

Os requisitos funcionais descrevem **o que o sistema deve fazer** e **como os usuários e processos interagem com ele**.

### **📌 RF-01 - Integração com o DynamoDB**

✅ **RF-01.1** - O microserviço de **Integrações** deve criar automaticamente as tabelas **`Lancamentos`** e **`ConsolidadosDiarios`** na **primeira execução**, caso não existam.
✅ **RF-01.2** - O sistema deve garantir que cada lançamento esteja associado a um **ContaId** exclusivo, permitindo a recuperação apenas dos dados pertencentes ao usuário autenticado.
✅ **RF-01.3** - As tabelas devem armazenar as datas no formato **ISO 8601 (`yyyy-MM-ddTHH:mm:ssZ`)**, permitindo maior precisão nas consultas.
✅ **RF-01.4** - O sistema deve permitir a consulta de lançamentos por **período de tempo**, otimizando a busca via **índice global (`DataIndex`)**.

------

### **📌 RF-02 - Comunicação entre Microserviços via RabbitMQ**

✅ **RF-02.1** - O microserviço de **Lançamentos** deve publicar mensagens na fila **`fluxo-caixa-queue`** sempre que um lançamento for **criado, atualizado ou excluído**.
✅ **RF-02.2** - O microserviço de **Integrações** deve consumir mensagens da fila e executar a ação correspondente no DynamoDB.
✅ **RF-02.3** - O sistema deve garantir que mensagens enviadas ao RabbitMQ sejam **persistentes**, evitando perdas em caso de falha no serviço.
✅ **RF-02.4** - O sistema deve **descartar mensagens inválidas** e registrar logs detalhados para depuração.

------

### **📌 RF-03 - Endpoint de Reprocessamento do Consolidado Diário**

✅ **RF-03.1** - O sistema deve disponibilizar um **endpoint para reprocessamento do consolidado diário** no microserviço de **Integrações**.
✅ **RF-03.2** - O endpoint deve permitir as seguintes opções de reprocessamento:

- **Reprocessar tudo** → `POST /integracoes/reprocessar`
- **Reprocessar um dia específico** → `POST /integracoes/reprocessar?dataInicio=YYYY-MM-DD&dataFim=YYYY-MM-DD`
- **Reprocessar um período** → `POST /integracoes/reprocessar?dataInicio=YYYY-MM-DD&dataFim=YYYY-MM-DD` ✅ **RF-03.3** - O sistema deve garantir que **a data de início (`dataInicio`) não seja maior que a data de fim (`dataFim`)**, retornando erro `400 Bad Request` caso isso ocorra.
  ✅ **RF-03.4** - O sistema deve garantir que **o reprocessamento considere todo o intervalo do dia (`00:00:00` até `23:59:59`)**, garantindo que todos os lançamentos sejam incluídos corretamente.

------

### **📌 RF-04 - Segurança e Permissões**

✅ **RF-04.1** - O sistema deve garantir que um usuário só consiga acessar os lançamentos **da sua própria conta (`ContaId`)**, impedindo o acesso a dados de terceiros.
✅ **RF-04.2** - O sistema deve validar que apenas microserviços autorizados possam consumir mensagens da fila RabbitMQ.

------

## **2️⃣ Requisitos Não Funcionais (RNF)**

Os requisitos não funcionais descrevem **como o sistema deve operar**, incluindo **desempenho, segurança, escalabilidade e manutenção**.

### **📌 RNF-01 - Estruturação e Rotação dos Logs**

✅ **RNF-01.1** - Todos os microserviços devem registrar logs no formato **JSON estruturado**, facilitando a análise via **CloudWatch, Grafana, Kibana e Loki**.
✅ **RNF-01.2** - O sistema deve gerar um **arquivo de log por dia** e por **microserviço**, no formato:

```
lua


CopiarEditar
logs/{nome-do-microservico}_YYYY-MM-DD.log
```

Exemplo:

```
luaCopiarEditarlogs/
  fluxo-caixa-lancamentos_2025-02-09.log
  fluxo-caixa-consolidado-diario_2025-02-09.log
  fluxo-caixa-integracoes_2025-02-09.log
```

✅ **RNF-01.3** - O nome do microserviço deve ser **capturado automaticamente via Reflection**, sem necessidade de ser informado manualmente no código.
✅ **RNF-01.4** - O sistema deve evitar caracteres Unicode escapados nos logs, garantindo que mensagens como `"Reprocessamento concluído com sucesso!"` sejam exibidas corretamente.

------

### **📌 RNF-02 - Escalabilidade e Desempenho**

✅ **RNF-02.1** - O sistema deve utilizar **índices globais no DynamoDB (`DataIndex`)** para otimizar buscas por período de tempo.
✅ **RNF-02.2** - O sistema deve **evitar buscas desnecessárias** no DynamoDB, filtrando corretamente os resultados via chave de partição (`ContaId`) e índice de data (`Data`).
✅ **RNF-02.3** - O sistema deve garantir **alta disponibilidade e tolerância a falhas**, utilizando filas persistentes no RabbitMQ e logs detalhados para depuração.

------

### **📌 RNF-03 - Manutenibilidade e Observabilidade**

✅ **RNF-03.1** - O sistema deve fornecer logs detalhados para todas as operações críticas, incluindo:

- Criação, atualização e remoção de lançamentos.
- Publicação e consumo de mensagens no RabbitMQ.
- Erros durante o processamento de mensagens e acesso ao DynamoDB. ✅ **RNF-03.2** - Todos os logs devem seguir um formato **padrão e estruturado**, garantindo fácil análise e correlação entre eventos.

------

# **📢 Resumo**

### ✅ **Requisitos Funcionais Implementados**

- 🔹 **DynamoDB:** Criação automática de tabelas, segurança via `ContaId`, timestamp no formato ISO 8601.
- 🔹 **RabbitMQ:** Mensagens estruturadas, persistentes e com tratamento de erro.
- 🔹 **Reprocessamento do Consolidado Diário:** Endpoint flexível com validações de datas.
- 🔹 **Segurança:** Restrições de acesso para evitar exposição de dados.

### ✅ **Requisitos Não Funcionais Implementados**

- 🔹 **Logs:** JSON estruturado, arquivos separados por microserviço e data, rotação automática.
- 🔹 **Escalabilidade:** Uso de índices globais no DynamoDB, processamento assíncrono no RabbitMQ.
- 🔹 **Desempenho:** Otimização das consultas e operações para reduzir carga no banco.