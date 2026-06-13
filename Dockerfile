FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /app

COPY EnviosRapidosGT.sln .
COPY src/EnviosRapidosGT.API/*.csproj ./src/EnviosRapidosGT.API/
COPY src/EnviosRapidosGT.Tests/*.csproj ./src/EnviosRapidosGT.Tests/
RUN dotnet restore

COPY . .
RUN dotnet publish src/EnviosRapidosGT.API/EnviosRapidosGT.API.csproj -c Release -o /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:10000
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 10000

ENTRYPOINT ["dotnet", "EnviosRapidosGT.API.dll"]
