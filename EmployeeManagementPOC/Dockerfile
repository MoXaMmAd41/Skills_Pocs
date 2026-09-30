# Build from the repository root (the API references ../DataStructuresAlgorithmsPOC):
#   docker build -f EmployeeManagementPOC/Dockerfile .

# ---- Angular client ---------------------------------------------------------
FROM node:22-alpine AS client
WORKDIR /client

COPY EmployeeManagementPOC/client/package.json EmployeeManagementPOC/client/package-lock.json ./
RUN npm ci

COPY EmployeeManagementPOC/client/ ./
RUN npx ng build --configuration production

# ---- .NET API ----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS api
WORKDIR /repo

# Restore first so the layer is cached until a project file changes.
COPY EmployeeManagementPOC/Directory.Build.props EmployeeManagementPOC/Directory.Packages.props EmployeeManagementPOC/
COPY EmployeeManagementPOC/src/EmployeeManagement.Domain/*.csproj EmployeeManagementPOC/src/EmployeeManagement.Domain/
COPY EmployeeManagementPOC/src/EmployeeManagement.Application/*.csproj EmployeeManagementPOC/src/EmployeeManagement.Application/
COPY EmployeeManagementPOC/src/EmployeeManagement.Infrastructure/*.csproj EmployeeManagementPOC/src/EmployeeManagement.Infrastructure/
COPY EmployeeManagementPOC/src/EmployeeManagement.Api/*.csproj EmployeeManagementPOC/src/EmployeeManagement.Api/
COPY DataStructuresAlgorithmsPOC/*.csproj DataStructuresAlgorithmsPOC/
RUN dotnet restore EmployeeManagementPOC/src/EmployeeManagement.Api/EmployeeManagement.Api.csproj

COPY EmployeeManagementPOC/src/ EmployeeManagementPOC/src/
COPY DataStructuresAlgorithmsPOC/ DataStructuresAlgorithmsPOC/
RUN dotnet publish EmployeeManagementPOC/src/EmployeeManagement.Api/EmployeeManagement.Api.csproj \
    -c Release -o /app/publish --no-restore /p:UseAppHost=false

# ---- Runtime -----------------------------------------------------------------
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=api /app/publish .
COPY --from=client /client/dist/client/browser ./wwwroot

USER $APP_UID
EXPOSE 8080
ENTRYPOINT ["dotnet", "EmployeeManagement.Api.dll"]
