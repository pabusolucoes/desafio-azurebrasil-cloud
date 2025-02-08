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

#### **3️⃣ Avaliação de Escalabilidade**

Atualmente, a arquitetura suporta **50 requisições por segundo** com uma tolerância de 5% de perda. Para suportar **10x a carga (500 req/s)**, as seguintes ações são necessárias:

- **Escalabilidade horizontal do API Gateway**: O API Gateway escala automaticamente, mas pode exigir **Ajuste de RPS (Requisições por Segundo) e Rate Limiting**.
- **Aumento da concorrência de AWS Lambda**: Por padrão, cada Lambda pode executar **1000 chamadas simultâneas**. Isso pode ser aumentado com um ajuste de **Concurrency Limits e Auto Scaling Policies**.
- **DynamoDB Auto Scaling**: O DynamoDB precisa ajustar automaticamente as **Read/Write Units** para atender ao pico de carga.
- **SQS e Lambda Workers**: Aumentar a concorrência dos Workers para garantir que as mensagens sejam processadas em tempo hábil.

#### **4️⃣ Comparação com Arquitetura Baseada em Containers (Fargate/Kubernetes)**

Se **custo não fosse um problema**, a arquitetura baseada em **ECS Fargate ou Kubernetes (EKS)** traria os seguintes benefícios:

**✅ Benefícios de AWS Fargate**:

- Maior **controle sobre os recursos** de CPU e memória.
- **Execução constante** sem limite de tempo (diferente do Lambda).
- **Possibilidade de escalar horizontalmente com Auto Scaling**.
- Ideal para **workloads previsíveis e processamento constante**.

**✅ Benefícios de AWS EKS (Kubernetes)**:

- **Total flexibilidade na orquestração de workloads**.
- Melhor gerenciamento de **tráfego interno entre serviços**.
- Possibilidade de rodar **múltiplas instâncias de microserviços** sem limitação de execução.
- Ideal para **aplicações que exigem alta personalização**.

**❌ Motivo pelo qual escolhemos AWS Serverless:**

- **Custo Zero Inicial** com o Free Tier de AWS Lambda, API Gateway, DynamoDB e SQS.
- **Autoescalável sem necessidade de gerenciar servidores.**
- **Simplicidade de Deploy e Manutenção**, sem precisar gerenciar clusters.
- **Menos sobrecarga operacional**, já que não há necessidade de provisionar e gerenciar nós Kubernetes.

#### **5️⃣ Conclusão**

A arquitetura Serverless **consegue escalar para 10x a carga** com ajustes na concorrência do Lambda, Auto Scaling do DynamoDB e otimização das filas SQS. Se tivéssemos **altíssima carga constante**, a escolha de **Fargate ou EKS** poderia ser mais eficiente, pois **garantiria controle total sobre a infraestrutura** e otimização de custos em larga escala.

#### **6️⃣ Próximos Passos**

- Criar o `docker-compose.yml` para ambiente local.
- Implementar o Terraform para provisionamento da AWS.
- Desenvolver os endpoints da API e a lógica de persistência.

------

**Dúvidas? Algo que queira adicionar no diagrama?**