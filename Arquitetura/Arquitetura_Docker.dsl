workspace "Desafio Carrefour Docker" {

    model {
    
            properties {
                "structurizr.groupSeparator" "/"
            }
            user = person "Administrador Financeiro" "Gerenciar o fluxo de caixa" "External"
            user2 = person "Avaliador" "Arquiteto" "external"
            
            desafio = softwareSystem "Desafio Carrefour"{
                user2 -> this "Avalia o "
                sgbd = container "Documentos" "Registra Documentos" "DynamoDB" "database" {
                    
                }
                
                backend = container "Back-End" "Ecossistema de MicroServiços" "Minimal API" {

                    autenticacao = component "MicroServiço FluxoCaixa.Autenticacao" "Gerencia Usuários, Senhas, API-KEYS e processa a Autenticação" "C#" {

                    }
                    
                    fluxocaixaLancamentos = component "MicroServico FluxoCaixa.Lancamentos" "Registra e exibe lançamentos" "C#"{
                      this -> autenticacao "Valida API-KEY na " "HTTPS/JSON"
                    }
                
                    fluxocaixaConsolidadoDiario = component "MicroServico FluxoCaixa.ConsolidadoDiario" "Processa e exibe os lançamentos consolidados" "C#"{
                      this -> autenticacao "Valida API-KEY na " "HTTPS/JSON"
                    }
                    fluxocaixaIntegracoes = component "MicroServico FluxoCaixa.Integracoes" "Processa e persiste os lançamentos no SGBD" "C#"{
                     this -> autenticacao "Valida API-KEY na " "HTTPS/JSON"
                     rel3 = this -> sgbd "Armazena Lançamentos, Consolidados em" "DynamoDB Protocol"

                    }
                rel1 = this -> sgbd "Armazena Documentos em" "DynamoDB Protocol" "tag1"
                rel2 = sgbd -> this "Envia consultas para" "DynamoDB Protocol" "tag2"
                }

                rabbitmq = container "RabbitMQ" "Responsável por enviar as solicitações de Dcoumentos para a Integração" "RabbitMQ Protocol"{
                    fluxocaixaIntegracoes -> this "Consome de" "(Integracoes)->fluxo-caixa-queue"
                    rel4 = fluxocaixaLancamentos -> this  "Produz para" "fluxo-caixa-queue"
                    rel5 = fluxocaixaConsolidadoDiario -> this  "Produz para" "fluxo-caixa-queue"
                    
                }
                
                frontend = container "Fluxo de Caixa" "Registra e exibe os lançamentos financeiros" "React/Vite" "Navegador Web"{
                    user -> this "Registra e consulta lançamentos no" "HTTPS"
                    this -> fluxocaixaLancamentos "Recupera informações de lançamento" "DynamoDB Protocol"
                    this -> fluxocaixaConsolidadoDiario "Recupera lançamentos consolidados" "DynamoDB Protocol"
                    this -> autenticacao "Autentica usuário no" "HTTPS/JSON"
                }
           }
        }
    
    views {
        systemContext desafio ContextDiagram {
            include *
            exclude user
            autolayout
        }
        container desafio "ContainerDiagram" {
            include *
            exclude rel3
            exclude rel4
            autoLayout lr
        }
        component backend "ComponentDiagram" {
            include *
            autoLayout rl
        }
        theme default
        styles {
            relationship "tag1" {
                routing Curved
            }
            relationship "tag2" {
                routing Curved
            }
            element "softwareSystem"{
                stroke #ffffff
            }
            element "Navegador Web" {
                shape WebBrowser
                background #88BBF7
            }
            element "database"{
                shape Cylinder
            }
            
        }
        themes https://static.structurizr.com/themes/amazon-web-services-2023.01.31/theme.json
    }

}
