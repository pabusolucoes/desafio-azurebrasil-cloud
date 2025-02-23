
# Desafio Carrefour - Frontend

Este é o frontend de uma aplicação para controle financeiro, desenvolvida em React e configurada para rodar em um contêiner Docker. O sistema permite gerenciamento de lançamentos financeiros, visualização de relatórios consolidados e exportação de dados.

---

## **Funcionalidades**

1. **Login**: 
   - Autenticação simulada com mock para acesso ao sistema.
   - Tela estilizada com campos para `usuário` e `senha`.

2. **Dashboard**:
   - CRUD completo para lançamentos financeiros:
     - **Campos:** ID, Data, Valor, Descrição, Tipo (Crédito/Débito) e Categoria.
     - Permite criação, edição e exclusão de lançamentos.
   - **Ordenação e Filtro**:
     - Ordenação crescente e decrescente em qualquer coluna.
     - Filtro por Tipo ou Categoria.
   - Interface responsiva com layout clean.

3. **Relatório Consolidado**:
   - Geração de relatórios financeiros consolidados por **data**.
   - Apresenta:
     - Data.
     - Total de Débitos.
     - Total de Créditos.
     - Saldo final.
   - Exportação do relatório nos formatos:
     - **PDF**
     - **Excel**
     - **Markdown**

---

## **Requisitos**

- **Docker**: Certifique-se de que o Docker está instalado e funcionando corretamente.
- **Docker Compose**: Necessário para orquestrar os contêineres.

---

## **Como Rodar a Aplicação**

### **1. Clone o Repositório**

Clone o repositório para o seu ambiente local:

```bash
git clone https://github.com/seu-usuario/desafio-carrefour.git
cd desafio-carrefour
```

---

### **2. Configurar o Docker**

Certifique-se de que os arquivos `docker-compose.yml` e `Dockerfile` estão na raiz do projeto.

---

### **3. Subir o Contêiner**

Execute o comando abaixo para construir e iniciar o contêiner:

```bash
docker-compose up --build
```

> **Nota:** O processo pode demorar alguns minutos na primeira execução, pois será necessário baixar as imagens do Docker e instalar as dependências.

---

### **4. Acesse a Aplicação**

Depois que o contêiner estiver em execução, acesse a aplicação no navegador:

**URL:** [http://localhost:5173](http://localhost:5173)

---

## **Estrutura do Projeto**

```plaintext
desafio-carrefour/
├── dist/                 # Arquivos gerados pelo build (não utilizados no desenvolvimento)
├── node_modules/         # Dependências do projeto (instaladas via npm)
├── public/               # Arquivos públicos estáticos (favicon, index.html)
├── src/                  # Código-fonte do projeto
│   ├── assets/           # Logos e imagens
│   ├── components/       # Componentes React
│   ├── services/         # Configuração da API
│   ├── styles/           # Estilos CSS organizados por funcionalidade
│   ├── App.jsx           # Componente raiz do aplicativo
│   ├── main.jsx          # Ponto de entrada do React
├── Dockerfile            # Configuração do contêiner Docker
├── docker-compose.yml    # Orquestração de contêineres
├── package.json          # Dependências e scripts do projeto
├── vite.config.js        # Configuração do Vite para desenvolvimento
```

---

## **Comandos Úteis**

- **Subir o Contêiner:**
  ```bash
  docker-compose up --build
  ```

- **Parar o Contêiner:**
  ```bash
  docker-compose down
  ```

- **Acessar o Log do Contêiner:**
  ```bash
  docker logs frontend
  ```

---

## **Futuras Implementações**

- Integração com APIs reais para login, lançamentos e relatórios.
- Melhorias no layout e experiência do usuário.
- Implementação de testes unitários e e2e.

---

Desenvolvido com 💙 por **Fabrizio Buttazzi**
