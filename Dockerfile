# Etapa 1: build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar sln y csproj, restaurar dependencias
COPY *.sln .
COPY FitRank-API/*.csproj ./FitRank-API/
RUN dotnet restore

# Copiar el resto del código y compilar
COPY . .
WORKDIR /src/FitRank-API
RUN dotnet publish -c Release -o /app/publish

# Etapa 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Render usará su variable PORT, pero por defecto es 8080
EXPOSE 8080

# Comando de inicio
ENTRYPOINT ["dotnet", "FitRank-API.dll"]
