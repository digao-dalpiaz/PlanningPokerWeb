# Etapa 1: Construção
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Definindo o diretório de trabalho
WORKDIR /app

# Copiando o arquivo .csproj e restaurando dependências
COPY *.csproj ./
RUN dotnet restore

# Copiando o restante dos arquivos e construindo a aplicação
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: Execução
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final

# Definindo o diretório de trabalho para o contêiner final
WORKDIR /app

# Copiando os arquivos publicados da etapa anterior
COPY --from=build /app/publish .

# Expondo a porta que o aplicativo vai rodar
EXPOSE 8080

# Definindo o comando para rodar a API
ENTRYPOINT ["dotnet", "api.dll"]
