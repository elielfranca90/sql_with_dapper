FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copiar csproj e restaurar dependências
COPY ["dbsecrets.api.csproj", "./"]
RUN dotnet restore "dbsecrets.api.csproj"

# Copiar o restante dos arquivos e buildar
COPY . .
RUN dotnet build "dbsecrets.api.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "dbsecrets.api.csproj" -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "dbsecrets.api.dll"]
