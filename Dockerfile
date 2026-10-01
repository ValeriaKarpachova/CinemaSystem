FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Cinema.Domain/Cinema.Domain.csproj Cinema.Domain/
COPY Cinema.Infrastructure/Cinema.Infrastructure.csproj Cinema.Infrastructure/
COPY Cinema.Web/Cinema.Web.csproj Cinema.Web/

RUN dotnet restore Cinema.Web/Cinema.Web.csproj

COPY . .

RUN dotnet publish Cinema.Web/Cinema.Web.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .
RUN mkdir -p /app/keys

EXPOSE 8080

ENTRYPOINT ["dotnet", "Cinema.Web.dll"]