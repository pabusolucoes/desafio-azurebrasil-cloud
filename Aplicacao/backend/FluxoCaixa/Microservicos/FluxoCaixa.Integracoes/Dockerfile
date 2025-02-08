# Usa a imagem oficial do .NET SDK 8
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copia o arquivo de projeto e restaura as dependências
COPY *.csproj ./
RUN dotnet restore

# Copia todo o código-fonte e compila a aplicação
COPY . ./
RUN dotnet publish -c Release -o out

# Runtime para execução da aplicação
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .
EXPOSE 5003
ENTRYPOINT ["dotnet", "FluxoCaixa.Integracoes.dll"]
