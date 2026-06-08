# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY DFDSVisitManagementAPI.slnx .
COPY DFDSVisitManagementAPI.Domain/DFDSVisitManagementAPI.Domain.csproj DFDSVisitManagementAPI.Domain/
COPY DFDSVisitManagementAPI.Business/DFDSVisitManagementAPI.Business.csproj DFDSVisitManagementAPI.Business/
COPY DFDSVisitManagementAPI.Application/DFDSVisitManagementAPI.Application.csproj DFDSVisitManagementAPI.Application/
COPY DFDSVisitManagementAPI.Framework/DFDSVisitManagementAPI.Framework.csproj DFDSVisitManagementAPI.Framework/
COPY DFDSVisitManagementAPI.Tests/DFDSVisitManagementAPI.Tests.csproj DFDSVisitManagementAPI.Tests/

RUN dotnet restore

COPY . .

RUN dotnet publish DFDSVisitManagementAPI.Framework/DFDSVisitManagementAPI.Framework.csproj \
    -c Release \
    -o /app/publish

# Find and copy Application XML to publish output
RUN find /src -name "DFDSVisitManagementAPI.Application.xml" -exec cp {} /app/publish/ \; 2>/dev/null || echo "Application XML not found"

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app

COPY --from=build /app/publish .

EXPOSE 8080

ENTRYPOINT ["dotnet", "DFDSVisitManagementAPI.Framework.dll"]