# 🚀 Microserviço de Autenticação

## 📝 Descrição

O microserviço **fluxo_caixa_autenticacao** é responsável por gerenciar a autenticação e autorização dos usuários dentro do sistema de fluxo de caixa. Ele garante que apenas usuários autorizados possam acessar os demais microserviços.

## 🔥 Funcionalidades

- 🆕 **Criar usuário**: Permite cadastrar um novo usuário no sistema e gera uma API Key para ele.

- ❌ **Desativar usuário**: Exclui um usuário e sua respectiva API Key do sistema.

- 🔑 **Login de usuários**: Permite a autenticação de usuários e retorna um token JWT.

- 🔄 **Renovação de token**: Fornece um novo token JWT válido antes da expiração do token atual.

- 🔐 **Validação de token**: Verifica a autenticidade de um token JWT e retorna os detalhes do usuário autenticado.

- 🔑 **Alterar senha**: Permite que um usuário altere sua senha.

- 🔄 **Estender API Key**: Prolonga a validade da API Key de um usuário.

- 🏷️ **Gerenciamento de API Keys**: Geração e renovação automática de API Keys.

- 🔒 

  Segurança

  :

  - Usa **JWT** para autenticação.
  - Utiliza **BCrypt** para armazenar senhas de forma segura.
  - Integra com **Amazon DynamoDB** para armazenar credenciais e chaves de API.

- 📜 **Swagger UI**: API documentada com Swagger.

## 🌐 Endpoints

### 🆕 Criar usuário

**POST** `/auth/criar`

- Cadastra um novo usuário e gera uma API Key para ele.

### ❌ Desativar usuário

**POST** `/auth/desativar`

- Remove um usuário e sua API Key do sistema.

### 🔑 Login de usuários

**POST** `/auth/login`

- Autentica o usuário e retorna um token JWT.

### 🔄 Renovação de token

**POST** `/auth/refresh-token`

- Recebe um token expirado e retorna um novo token válido.

### 🔐 Validação de token

**GET** `/auth/validar-token`

- Verifica se um token JWT é válido e retorna as informações do usuário autenticado.

### 🔑 Alterar senha

**POST** `/auth/alterar-senha`

- Permite que um usuário altere sua senha.

### 🔄 Estender API Key

**POST** `/auth/estender-apikey`

- Estende a validade de uma API Key para mais dias.

## 🛠 Tecnologias Utilizadas

- ⚡ .NET 8 Minimal API
- 🐳 Docker
- 🔒 JWT (JSON Web Token) para autenticação
- 🔑 BCrypt para armazenamento seguro de senhas
- 📦 Amazon DynamoDB (armazenamento de usuários e API Keys)
- 📜 Swagger UI para documentação da API

## ⚙️ Configuração

- Variáveis de ambiente necessárias:

  ```env
  JWT_SECRET=supersecreto
  JWT_EXPIRATION=3600
  AWS_ACCESS_KEY=your_access_key
  AWS_SECRET_KEY=your_secret_key
  AWS_SERVICE_URL=https://dynamodb.us-east-1.amazonaws.com
  ```

- Para rodar localmente:

  ```sh
  dotnet run
  ```

## 🚨 Informações Importantes

- A autenticação é baseada no uso de **JWT**, garantindo segurança na troca de informações.
- Pode ser integrado com um **banco de dados** para armazenamento de credenciais caso necessário.
- A senha dos usuários é armazenada de forma segura utilizando **BCrypt**.

------

