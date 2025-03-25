FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Build Stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia o arquivo de solução e restaura as dependências
COPY ["api.csproj", "./"]
RUN dotnet restore "api.csproj"

# Copia o restante do código e compila
COPY . .
RUN dotnet publish "api.csproj" -c Release -o /app/publish --no-restore

# Runtime Stage
FROM base AS final
WORKDIR /app
COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "api.dll"]
