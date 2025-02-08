### Arquitetura da Solução - AWS Serverless

#### **1️⃣ Visão Geral da Arquitetura**

A solução será baseada em **AWS Serverless**, utilizando:

- **Backend**: AWS Lambda (.NET Core)
- **Mensageria**: Amazon SQS
- **Banco de Dados**: DynamoDB
- **API Gateway**: Comunicação entre o front e backend
- **Monitoramento**: AWS CloudWatch
- **Front-end**: .NET Core hospedado no AWS Amplify

#### **2️⃣ Diagrama Arquitetural**

```
 +------------+        +-------------+        +------------------+
 | Front-end  | -----> | API Gateway | -----> | AWS Lambda (C#)  |
 +------------+        +-------------+        +------------------+
                                                      |
                                                      v
                                        +----------------------+
                                        | Amazon DynamoDB      |
                                        +----------------------+
                                                      |
                                                      v
                                        +----------------------+
                                        | Amazon SQS (Fila)    |
                                        +----------------------+
                                                      |
                                                      v
                                        +----------------------+
                                        | AWS Lambda (Worker)  |
                                        +----------------------+
                                                      |
                                                      v
                                        +----------------------+
                                        | CloudWatch Logs      |
                                        +----------------------+
```

#### **3️⃣ Fluxo de Dados**

1. O **front-end** faz requisições para o **API Gateway**.
2. O **API Gateway** encaminha para o **AWS Lambda**.
3. O Lambda realiza operações no **DynamoDB** (inserção, consulta, etc.).
4. Para operações assíncronas, os eventos são enviados para **Amazon SQS**.
5. Um segundo Lambda (Worker) consome mensagens da fila e realiza processamentos adicionais.
6. Os logs de execução são enviados para **CloudWatch**.

#### **4️⃣ Infraestrutura como Código**

- Podemos provisionar a infraestrutura via **Terraform** para automação do ambiente AWS.
- Os serviços serão configuráveis via **AWS SAM** ou **Serverless Framework**.

#### **5️⃣ Execução Local via Docker**

A aplicação será empacotada em **Docker** para rodar localmente. O `docker-compose.yml` incluirá:

- API .NET Core
- DynamoDB Local
- LocalStack para simular AWS Lambda e SQS

#### **6️⃣ Segurança e Monitoramento**

- **Autenticação JWT** integrada ao API Gateway.
- **CloudWatch + X-Ray** para rastreamento de chamadas Lambda.
- **IAM Roles** restritivas para os serviços AWS.

#### **7️⃣ Próximos Passos**

- Criar o `docker-compose.yml` para ambiente local.
- Implementar o Terraform para provisionamento da AWS.
- Desenvolver os endpoints da API e a lógica de persistência.

------

**Dúvidas? Algo que queira adicionar no diagrama?**