FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY Librarium.slnx ./
COPY src/Librarium.Api/Librarium.Api.csproj src/Librarium.Api/
COPY src/Librarium.Data/Librarium.Data.csproj src/Librarium.Data/

RUN dotnet restore src/Librarium.Api/Librarium.Api.csproj

COPY src/ src/

RUN dotnet publish src/Librarium.Api/Librarium.Api.csproj -c Release -o /app/publish --no-restore

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Librarium.Api.dll"]
