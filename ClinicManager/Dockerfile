# ---- Build stage ----
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY ["*.csproj", "./"]
RUN dotnet restore

COPY . .
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false

# ---- Final Container ----
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish ./

# Instead use a sensible volume:
# COPY appsettings.json ./

EXPOSE 8080

ENTRYPOINT ["dotnet", "ClinicManager.dll", "--urls=http://0.0.0.0:8080"]
