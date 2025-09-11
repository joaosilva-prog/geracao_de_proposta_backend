# Estágio 1: Build - A "Oficina"
# Usa a imagem completa do .NET 8 SDK para compilar seu código.
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copia e restaura dependências primeiro para otimizar o cache do Docker.
COPY ["GeracaoPropostaOrigo/GeracaoPropostaOrigo.csproj", "GeracaoPropostaOrigo/"]
RUN dotnet restore "GeracaoPropostaOrigo/GeracaoPropostaOrigo.csproj"

# Copia o resto do código-fonte.
COPY . .
WORKDIR "/src/GeracaoPropostaOrigo"
# Publica a aplicação em modo Release, gerando os arquivos otimizados.
RUN dotnet publish "GeracaoPropostaOrigo.csproj" -c Release -o /app/publish

# Estágio 2: Final - A "Vitrine"
# Usa a imagem leve de runtime do ASP.NET, que é menor e mais segura.
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app

# Copia apenas os arquivos publicados do estágio de build.
COPY --from=build /app/publish .

# Define a porta interna que a aplicação usará. O Railway detectará isso.
ENV ASPNETCORE_URLS=http://*:8080

# Comando final que inicia sua API quando o contêiner rodar.
ENTRYPOINT ["dotnet", "GeracaoPropostaOrigo.dll"]