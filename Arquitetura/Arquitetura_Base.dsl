workspace "Desafio Carrefour Base" {

    model {
    
            properties {
                "structurizr.groupSeparator" "/"
            }
            user = person "Administrador Financeiro" "Gerenciar o fluxo de caixa" "External"
            user2 = person "Avaliador" "Arquiteto" "External"
            desafio = softwareSystem "Desafio Carrefour"{

                sgbd = container "Documentos" "Registra Documentos" "DynamoDB" "Amazon Web Services - DynamoDB"{
                    
                }
                rabbitmq = container "Amazon SNS" "Responsável por disponibilizar as mensagens em tópicos" "SNS"{
                    
                }
                backend = container "Gateway" "Resposanvel pela comunicação com os Lambdas" "API Gateway" "Amazon Web Services - API Gateway" {

                    autenticacao = component "MicroServiço FluxoCaixa.Autenticacao" "Gerencia Usuários, Senhas, API-KEYS e processa a Autenticação" "C#" "Amazon Web Services - AWS Lambda Lambda Function"{
                        this -> rabbitmq "Consome de" "SNS Topicos"
                        this -> sgbd "Armazena Documentos" "DynamoDB Protocol"
                    }
                    
                    fluxocaixaLancamentos = component "MicroServico FluxoCaixa.Lancamentos" "Registra e exibe lançamentos" "C#" "Amazon Web Services - AWS Lambda Lambda Function"{
                      this -> rabbitmq "Consome de" "SNS Topicos"
                    }
                
                    fluxocaixaConsolidadoDiario = component "MicroServico FluxoCaixa.ConsolidadoDiario" "Processa e exibe os lançamentos consolidados" "C#" "Amazon Web Services - AWS Lambda Lambda Function"{
                      this -> rabbitmq "Consome de" "SNS Topicos"
                    }
                    fluxocaixaIntegracoes = component "MicroServico FluxoCaixa.Integracoes" "Processa e persiste os lançamentos no SGBD" "C#" "Amazon Web Services - AWS Lambda Lambda Function"{
                     this -> rabbitmq "Consome de" "SNS Topicos"
                     rel3 = this -> sgbd "Armazena Lançamentos, Consolidados em" "DynamoDB Protocol"

                    }
                    this -> sgbd "Requests entregues a "
                }

                frontend = container "Fluxo de Caixa" "Registra e exibe os lançamentos financeiros" "React/Vite" "Amazon Web Services - Amplify"{
                    user -> this "Registra e consulta lançamentos no" "HTTPS"
                    rel4 = this -> backend "Recebe e Envia requests ao " "HTTPS" "tag4"
                }
                cloudwatch = container "CloudWacth" "Responsável pelo Monitoramento e Logs" "CloudWatch" "Amazon Web Services - CloudWatch"{
                    backend -> this
                }
                user2 -> this "Avalia o "
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
            exclude sgbd
            autoLayout lr
        }
        component backend "ComponentDiagram" {
            include *
            autoLayout
        }
        theme default
        styles {
            relationship "tag1" {
                routing Curved
            }
            relationship "tag2" {
                routing Curved
            }
            relationship "tag4" {
                routing Orthogonal
            }
            element "softwareSystem"{
                stroke #ffffff
            }
            element "Amazon Web Services - Amplify" {
                shape WebBrowser
                background #88BBF7
            }
            element "Amazon Web Services - DynamoDB"{
                shape Cylinder
            }
            
        }
        themes https://static.structurizr.com/themes/amazon-web-services-2023.01.31/theme.json
    }

}
