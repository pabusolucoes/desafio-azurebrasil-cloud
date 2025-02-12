# Documentação dos Microserviços do Fluxo de Caixa

## Introdução

Este documento descreve a estrutura e funcionamento dos microserviços que compõem a solução **Fluxo de Caixa**, utilizada para gestão de transações financeiras, consolidação e autenticação.

A solução é composta pelos seguintes microserviços:

- **Autenticação (`fluxo-caixa-autenticacao`)**: Responsável por autenticação e gestão de usuários.
- **Lançamentos (`fluxo-caixa-lancamentos`)**: Gerencia e armazena transações financeiras.
- **Consolidação (`fluxo-caixa-consolidado`)**: Agrega e calcula os totais de entradas e saídas.
- **Integrações (`fluxo-caixa-integracoes`)**: Consome eventos financeiros e interage com outros sistemas.

------

## Requisitos

Para executar os microserviços, é necessário ter:

- **Docker** instalado e configurado corretamente.
- **Docker Compose** para orquestração dos contêineres.
- **AWS CLI** configurado caso deseje integrar com a AWS DynamoDB.
- **.NET 8 SDK** instalado para desenvolvimento e execução local.

------

## Configuração e Execução

### **1. Clone o Repositório**

```bash
git clone https://github.com/seu-usuario/fluxo-caixa.git
cd fluxo-caixa
```

### **2. Configurar o Docker**

Certifique-se de que os arquivos `docker-compose.yml` e `Dockerfile` estão configurados corretamente.

### **3. Subir os Microserviços**

Execute o comando abaixo para construir e iniciar os contêineres:

```bash
docker-compose up --build
```

> **Nota:** O processo pode demorar alguns minutos na primeira execução.

### **4. Acesse os Microserviços**

Após a inicialização, os microserviços estarão disponíveis nas seguintes URLs:

- **Autenticação**: `http://localhost:5004`
- **Lançamentos**: `http://localhost:5001`
- **Consolidação**: `http://localhost:5002`
- **Integrações**: `http://localhost:5003`

------

## Estrutura dos Microserviços

Cada microserviço segue uma estrutura modular e desacoplada. Os serviços se comunicam via HTTP e mensagens assíncronas no RabbitMQ.

```plaintext
/FluxoCaixa
├── Microservicos/
│   ├── fluxo_caixa_autenticacao/
│   ├── fluxo_caixa_lancamentos/
│   ├── fluxo_caixa_consolidado/
│   ├── fluxo_caixa_integracoes/
```

------

## Rotas Disponíveis

### **1. Autenticação (`fluxo-caixa-autenticacao`)**

- `POST /auth/login` → Autentica o usuário e retorna um JWT.
- `POST /auth/criar` → Cria um novo usuário e gera uma API Key.
- `POST /auth/alterar-senha` → Altera a senha do usuário.
- `POST /auth/estender-apikey` → Renova a API Key do usuário.

### **2. Lançamentos (`fluxo-caixa-lancamentos`)**

- `POST /fluxo-caixa/lancamentos` → Registra um novo lançamento.
- `GET /fluxo-caixa/lancamentos` → Lista todos os lançamentos.
- `GET /fluxo-caixa/lancamentos/{id}` → Detalhes de um lançamento.
- `PUT /fluxo-caixa/lancamentos/{id}` → Atualiza um lançamento.
- `DELETE /fluxo-caixa/lancamentos/{id}` → Remove um lançamento.

### **3. Consolidação (`fluxo-caixa-consolidado`)**

- `GET /consolidado-diario` → Obtém o consolidado de todos os dias.
- `GET /consolidado-diario/{data}` → Consulta o consolidado de uma data específica.
- `POST /consolidado-diario/reprocessar` → Reprocessa o consolidado diário.

### **4. Integrações (`fluxo-caixa-integracoes`)**

- `POST /integracoes/processar` → Processa dados recebidos de sistemas externos.
- `GET /integracoes/status` → Retorna o status das integrações ativas.

------

## Monitoramento e Logs

- Os logs são gerados em formato JSON para integração com ferramentas como **CloudWatch**, **Grafana** e **Kibana**.
- Logs de autenticação, falhas e eventos críticos são registrados para auditoria.

------

## Futuras Melhorias

- Implementação de cache para otimização das consultas.
- Melhorias na segurança e criptografia das credenciais.
- Implementação de testes unitários e de integração.

------

**Desenvolvido por Fabrizio Buttazzi**