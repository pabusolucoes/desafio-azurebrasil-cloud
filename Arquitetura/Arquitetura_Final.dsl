workspace "Desafio Carrefour ALta Disponibilidade" {

    model {
    
            properties {
                "structurizr.groupSeparator" "/"
            }
            user = person "Administrador Financeiro" "Gerenciar o fluxo de caixa" "External"
            user2 = person "Avaliador" "Arquiteto" "External"
            desafio = softwareSystem "Desafio Carrefour"{

                sgbd = container "Documentos" "Registra Documentos" "DynamoDB" "Amazon Web Services - DynamoDB"{
                    
                }
                kafka = container "Amazon Kafka" "Responsável por disponibilizar as mensagens em tópicos" "Apache Kafka" "Amazon Web Services - Managed Streaming for Apache Kafka"{
                    
                }
                apigateway = container "Gateway" "Responsável pela comunicação com o Fargate" "Gateway Protocol" "Amazon Web Services - API Gateway"{
                    
                }
                
                nlb = container "NLB" "Network Loader Balance" "TCP/SSL" "Amazon Web Services - Elastic Load Balancing Network Load Balancer"{

                    
                }                
                
                fargate = container "Fargate" "Responsável por hospedar os ECS Fargate (Service) " "ECS" "Amazon Web Services - Fargate" {

                    kafkaconnector = component "Kafka Consumer personalizado" "java" {
                        
                        nlb -> this "Encaminha requisições para" "TCP"
                        
                    }

                    autenticacao = component "MicroServiço FluxoCaixa.Autenticacao" "Gerencia Usuários, Senhas, API-KEYS e processa a Autenticação" "C#" "Amazon Web Services - ECS Anywhere"{
                        this -> sgbd "Armazena Documentos" "DynamoDB Protocol"
                    }

                    fluxocaixaLancamentos = component "MicroServico FluxoCaixa.Lancamentos" "Registra e exibe lançamentos" "C#" "Amazon Web Services - ECS Anywhere"{
                    }
                
                    fluxocaixaConsolidadoDiario = component "MicroServico FluxoCaixa.ConsolidadoDiario" "Processa e exibe os lançamentos consolidados" "C#" "Amazon Web Services - ECS Anywhere"{
                    }
                    fluxocaixaIntegracoes = component "MicroServico FluxoCaixa.Integracoes" "Processa e persiste os lançamentos no SGBD" "C#" "Amazon Web Services - ECS Anywhere"{
                     fluxocaixaIntegracoes -> kafka "Consome mensagens dos tópicos"
                     fluxocaixaIntegracoes -> sgbd "Persiste lançamentos e consolidados"

                    }
                    
                    kafkaconnector -> nlb "Encaminha mensagens Kafka para consumidores"

                }
                alb = container "ALB" "Application Loader Balance" "ALB" "Amazon Web Services - Elastic Load Balancing Application Load Balancer" {
                    this -> fargate "Distribui uniformemente as requests para"
                    apigateway -> this "Encaminha requisições para"
                    this -> kafkaconnector "Encaminha requisições para"
                    alb -> fluxocaixaLancamentos "Roteia chamadas para consultas"
                    alb -> fluxocaixaConsolidadoDiario "Roteia chamadas para consultas"
                    alb -> autenticacao "Roteia chamadas para consultas"
                    
                }

                
                frontend = container "Fluxo de Caixa" "Registra e exibe os lançamentos financeiros" "React/Vite" "Amazon Web Services - Amplify"{
                    user -> this "Registra e consulta lançamentos no" "HTTPS"
                    rel4 = this -> apigateway "Recebe e Envia requests ao " "HTTPS" "tag4"
                    
                }
                cloudwatch = container "CloudWacth" "Responsável pelo Monitoramento e Logs" "CloudWatch" "Amazon Web Services - CloudWatch"{
                    fargate -> this "Envia métricas/Logs para"
                    kafka -> this "Envia métricas/Logs para"
                    apigateway -> this "Envia métricas/Logs para"
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
            exclude sgbd
            autoLayout lr
        }
        component fargate "ComponentDiagram" {
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
